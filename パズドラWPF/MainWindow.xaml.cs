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

        /// <summary>
        /// ドロップ移動用ウィンドウ
        /// </summary>
        DropWindow dropWind = null;

		public MainWindow()
        {
            InitializeComponent();

            FieldDrop = new Ellipse[Field.FieldHeight, Field.FieldWidth]
            { 
                { drop00, drop01, drop02, drop03, drop04, drop05 },
                { drop10, drop11, drop12, drop13, drop14, drop15 },
                { drop20, drop21, drop22, drop23, drop24, drop25 },
                { drop30, drop31, drop32, drop33, drop34, drop35 },
                { drop40, drop41, drop42, drop43, drop44, drop45 }
            };

            //盤面をランダムに設定
            Random random = new Random();
            for(int y = 0; y < field.DropStates.GetLength(0); y++)
            {
                for (int x = 0; x < field.DropStates.GetLength(1); x++)
                {
					field.DropStates[y,x] = (DropState)random.Next((int)DropState.DropState1, (int)DropState.DropState6);
                }
            }

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

				//消すドロップ一覧を取得
			 	List<List<System.Drawing.Point>> groups = field.GetConnectedDropGroup();

				foreach (var group in groups)
				{
					foreach (var point in group)
					{
						field.DropStates[point.Y, point.X] = DropState.None;
					}

					DrawField();
					await Task.Delay(500);
				}
            }
        }
	}
}
