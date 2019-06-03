using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MIDIIOCSWrapper;

namespace パズドラWPF
{
    /// <summary>
    /// ドロップの状態を表します。
    /// </summary>
    enum DropState
    {
        /// <summary>
        /// なし
        /// </summary>
        None,
        /// <summary>
        /// ドロップ状態1
        /// </summary>
        DropState1,
        /// <summary>
        /// ドロップ状態2
        /// </summary>
        DropState2,
        /// <summary>
        /// ドロップ状態3
        /// </summary>
        DropState3,
        /// <summary>
        /// ドロップ状態4
        /// </summary>
        DropState4,
        /// <summary>
        /// ドロップ状態5
        /// </summary>
        DropState5,
        /// <summary>
        /// ドロップ状態6
        /// </summary>
        DropState6,
        /// <summary>
        /// ドロップ状態最大値
        /// </summary>
        DropState_MAX
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 盤面に配置してある円オブジェクトを表します。
        /// </summary>
        internal Ellipse[,] FieldDrop = null;

		internal Field field = new Field();

		private bool mouseRocked = false;

        /// <summary>
        /// ドロップ移動用ウィンドウ
        /// </summary>
        DropWindow dropWind = null;

		/// <summary>
		/// ドロップをランダムで選択するためのランダムオブジェクト
		/// </summary>
		Random random = new Random();

		/// <summary>
		/// midi出力デバイス
		/// </summary>
		MIDIOUT midiOut = null;

		const byte DefaultDeleteSoundNoteNum = 76;
		byte deleteSoundNoteNum = DefaultDeleteSoundNoteNum;

		public MainWindow()
        {
            InitializeComponent();

			//MIDIデバイスオープン
			int midiDeviceNum = MIDIOUT.GetDeviceNum();
			string midiDeviceName = "";
			for (int i = 0; i < midiDeviceNum; i++)
			{
				midiDeviceName = MIDIOUT.GetDeviceName(i);
				break;
			}
			try
			{
				midiOut = new MIDIOUT(midiDeviceName);
			}
			catch (MIDIIOException ex)
			{
				MessageBox.Show("MIDIデバイスが開けませんでした。\n音はなりません。", this.Title);
			}

            FieldDrop = new Ellipse[Field.FieldHeight, Field.FieldWidth]
            { 
                { drop00, drop01, drop02, drop03, drop04, drop05 },
                { drop10, drop11, drop12, drop13, drop14, drop15 },
                { drop20, drop21, drop22, drop23, drop24, drop25 },
                { drop30, drop31, drop32, drop33, drop34, drop35 },
                { drop40, drop41, drop42, drop43, drop44, drop45 }
            };

            //盤面をランダムに設定
            for(int y = 0; y < field.DropStates.GetLength(0); y++)
            {
                for (int x = 0; x < field.DropStates.GetLength(1); x++)
                {
					field.DropStates[y,x] = (DropState)random.Next((int)DropState.DropState1, (int)DropState.DropState6);
                }
            }

			//盤面を、削除できるドロップがない状態で開始
			DeleteAndFallDrop(false);

            //盤面更新
            DrawField();
        }

        /// <summary>
        /// 盤面を更新します。
        /// </summary>
        internal void DrawField()
        {
            //nullチェック
            if (FieldDrop == null)
            {
                return;
            }

            for (int y = 0; y < field.DropStates.GetLength(0); y++)
            {
                for (int x = 0; x < field.DropStates.GetLength(1); x++)
                {
					//塗りつぶし色のしてい
                    FieldDrop[y, x].Fill = GetDropBrush(field.DropStates[y, x]);
					//塗りつぶし色を透明にしても、縁が表示されちゃうから、非表示に。
					if (field.DropStates[y, x] == DropState.None)
					{
						FieldDrop[y, x].Visibility = Visibility.Hidden;
					}
                    else
                    {
                        FieldDrop[y, x].Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// ドロップ状態に対応した色ブラシを返します。
        /// </summary>
        /// <param name="state">ドロップ状態</param>
        /// <returns></returns>
        private SolidColorBrush GetDropBrush(DropState state)
        {
            SolidColorBrush brush = null;

            switch (state)
            {
                case DropState.DropState1:
                    brush = new SolidColorBrush(Colors.Red);
                    break;
                case DropState.DropState2:
                    brush = new SolidColorBrush(Colors.Green);
                    break;
                case DropState.DropState3:
                    brush = new SolidColorBrush(Colors.Blue);
                    break;
                case DropState.DropState4:
                    brush = new SolidColorBrush(Colors.Purple);
                    break;
                case DropState.DropState5:
                    brush = new SolidColorBrush(Colors.Yellow);
                    break;
                case DropState.DropState6:
                    brush = new SolidColorBrush(Colors.Pink);
                    break;
                default:
                    brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    break;
            }

            return brush;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void drop00_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//落下処理中は終了
			if (mouseRocked)
			{
				return;
			}

			//マウスロック開始
			mouseRocked = true;

			if (sender is Ellipse)
			{
				//移動用ウィンドウ
				dropWind = new DropWindow(((Ellipse)sender).Fill);
				dropWind.Owner = this;

				//移動元ドロップ検索＆非表示
				for (int y = 0; y < field.DropStates.GetLength(0); y++)
				{
					for (int x = 0; x < field.DropStates.GetLength(1); x++)
					{
						if (sender.Equals(FieldDrop[y, x]))
						{
							//移動用ウィンドウに、ドロップ状態渡す。
							dropWind.dropState = field.DropStates[y, x];
							//移動元ドロップを無しに変更して、再描画。
							field.DropStates[y, x] = DropState.None;
							DrawField();
						}
					}
				}

				//表示
				dropWind.ShowDialog();

				//ドロップ状態無しを探して、移動中ドロップに状態を変更
				for (int y = 0; y < field.DropStates.GetLength(0); y++)
				{
					for (int x = 0; x < field.DropStates.GetLength(1); x++)
					{
						if (field.DropStates[y, x] == DropState.None)
						{
							field.DropStates[y, x] = dropWind.dropState;
						}
					}
				}
				DrawField();

				await Task.Delay(100);

				DeleteAndFallDrop();

				//ドロップの削除が終わったら、MIDIノートナンバーをリセット
				deleteSoundNoteNum = DefaultDeleteSoundNoteNum;

				//マウスロック解除
				mouseRocked = false;
			}
		}

		/// <summary>
		/// ドロップを消して空いたところに落下させます。
		/// 落ちコンも考慮してます。
		/// </summary>
		/// <param name="delay">falseにすると、遅延なしで消せるドロップの無い盤面を作成します。</param>
		private async void DeleteAndFallDrop(bool delay = true)
		{
			//落ちコンのための無限ループ
			while (true)
			{
				//消すドロップ一覧を取得
				List<List<System.Drawing.Point>> groups = field.GetConnectedDropGroup();

				//消すドロップがなかったら抜ける
				if (groups.Count == 0)
				{
					break;
				}

				//ドロップを消します。
				foreach (var group in groups)
				{
					foreach (var point in group)
					{
						field.DropStates[point.Y, point.X] = DropState.None;
					}
					//ゲーム中のみ
					if (delay)
					{
						DrawField();
						//消えるときの音再生()
						PlayDeleteSound(deleteSoundNoteNum++);
						await Task.Delay(500);
					}
				}

				//ドロップを落下させます。
				//一番上のドロップが一番下まで来ると終了
				for (int i = 0; i < Field.FieldHeight; i++)
				{
					//下から二番目から調べて、下がnoneだったら入れ替え
					for (int y = Field.FieldHeight - 2; y >= 0; y--)
					{
						for (int x = 0; x < field.DropStates.GetLength(1); x++)
						{
							if (field.DropStates[y, x] != DropState.None
								&& field.DropStates[y + 1, x] == DropState.None)
							{
								field.DropStates[y + 1, x] = field.DropStates[y, x];
								field.DropStates[y, x] = DropState.None;
							}
						}
					}

					//新しいドロップをランダムで追加
					for (int x = 0; x < Field.FieldWidth; x++)
					{
						if (field.DropStates[0, x] == DropState.None)
						{
							field.DropStates[0, x] = (DropState)random.Next((int)DropState.DropState1, (int)DropState.DropState6);
						}
					}

					//ゲーム中のみ
					if (delay)
					{
						await Task.Delay(200);
						DrawField();
					}
				}
			}
		}

		private async void PlayDeleteSound(byte noteNum)
		{
			if (midiOut == null)
			{
				return;
			}

			//ノートオン
			byte[] message = new byte[] { 0x90, noteNum, 0x7f };
			midiOut.PutMIDIMessage(message);

			await Task.Delay(500);

			//ノートオフ
			message = new byte[] { 0x80, noteNum, 0x7f };
			midiOut.PutMIDIMessage(message);
		}

		/// <summary>
		/// ウィンドウCloseイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Closed(object sender, EventArgs e)
		{
			//MIDIデバイスを閉じる
			if (midiOut != null)
			{
				midiOut.Dispose();
			}
		}
	}
}
