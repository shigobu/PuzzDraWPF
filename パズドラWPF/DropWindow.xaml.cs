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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace パズドラWPF
{
    /// <summary>
    /// DropWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DropWindow : Window
    {
        private MainWindow ownerWindow = null;

        private Rect[,] DropRectArr = new Rect[MainWindow.FieldHeight, MainWindow.FieldWidth];

        internal DropState dropState = DropState.None;

        public DropWindow(Brush brush)
        {
            InitializeComponent();

            //ドロップに色設定
            if (brush != null)
            {
                drop.Fill = brush;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //マウスの中心に移動させる。
            MoveToMousePos();

            //オーナーウィンドウ取得＆キャスト
            ownerWindow = this.Owner as MainWindow;

            //ドロップの矩形を計算して格納
            if (ownerWindow != null)
            {
                for (int y = 0; y < ownerWindow.Field.GetLength(0); y++)
                {
                    for (int x = 0; x < ownerWindow.Field.GetLength(1); x++)
                    {
                        Point dropPos = ownerWindow.FieldDrop[y, x].PointToScreen(new Point(0, 0));
                        DropRectArr[y, x] = new Rect(dropPos, new Size(44, 44));//サイズは、50のマージンが３だから、50-3。
                    }
                }
            }
        }

        /// <summary>
        /// マウスの中心に円の中心が来るように移動させます。
        /// </summary>
        internal void MoveToMousePos()
        {
            //マウス座標取得
            Point mousePos = Common.GetMouseCursorPosition();

            //ウィンドウ移動
            this.Left = mousePos.X - (this.ActualWidth / 2);
			this.Top = mousePos.Y - (this.ActualHeight / 2);
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
            //ウィンドウをマウス座標へ移動
			MoveToMousePos();

            //盤面上のドロップ位置と比較して、ドロップ領域にマウスが入ったら、隣のドロップと入れ替える。
            CheckMouseEnterToDrop();
        }

        /// <summary>
        /// マウスがドロップ領域に入っているか確認して、隣のドロップ領域に入ったら、入れ替えをいこないます。
        /// </summary>
        private void CheckMouseEnterToDrop()
        {
            //マウスカーソルの座標取得(DPI考慮無し)
            System.Drawing.Point noDpiPoint = System.Windows.Forms.Cursor.Position;
            Point mousePoint = new Point(noDpiPoint.X, noDpiPoint.Y);

            if (ownerWindow == null)
            {
                return;
            }

            //マウスがドロップ領域に入っているか確認して、入れ替え。
            for (int y = 0; y < ownerWindow.Field.GetLength(0); y++)
            {
                for (int x = 0; x < ownerWindow.Field.GetLength(1); x++)
                {
                    //rect領域に入っているか確認
                    if (DropRectArr[y, x].Contains(mousePoint))
                    {
                        //ドロップ状態が無し(移動元のドロップ)だったら終了
                        if (ownerWindow.Field[y, x] == DropState.None)
                        {
                            return;
                        }
                        else
                        {
                            //上下左右斜めのドロップ状態が無しだったら、そのドロップと入れ替え。
                            ChangeToNextDrop(y, x);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 指定の座標のドロップの上下左右斜めに、ドロップ状態無しがあったら、それと入れ替えて、再描画。
        /// </summary>
        /// <param name="y">ｙ座標</param>
        /// <param name="x">ｘ座標</param>
        private void ChangeToNextDrop(int y, int x)
        {
            //左と入れ替え
            if (x > 0)
            {
                int tonariX = x - 1;
                if (ownerWindow.Field[y, tonariX] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[y, tonariX];
                    ownerWindow.Field[y, tonariX] = temp;
                }
            }
            //右と入れ替え
            if (x < ownerWindow.Field.GetLength(1) - 1)
            {
                int tonariX = x + 1;
                if (ownerWindow.Field[y, tonariX] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[y, tonariX];
                    ownerWindow.Field[y, tonariX] = temp;
                }
            }
            //上と入れ替え
            if (y > 0)
            {
                int tonariY = y - 1;
                if (ownerWindow.Field[tonariY, x] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[tonariY, x];
                    ownerWindow.Field[tonariY, x] = temp;
                }
            }
            //下と入れ替え
            if (y < ownerWindow.Field.GetLength(0) - 1)
            {
                int tonariY = y + 1;
                if (ownerWindow.Field[tonariY, x] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[tonariY, x];
                    ownerWindow.Field[tonariY, x] = temp;
                }
            }
            //左上と入れ替え
            if (x > 0 && y > 0)
            {
                int tonariX = x - 1;
                int tonariY = y - 1;
                if (ownerWindow.Field[tonariY, tonariX] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[tonariY, tonariX];
                    ownerWindow.Field[tonariY, tonariX] = temp;
                }
            }
            //右上と入れ替え
            if (x < ownerWindow.Field.GetLength(1) - 1 && y > 0)
            {
                int tonariX = x + 1;
                int tonariY = y - 1;
                if (ownerWindow.Field[tonariY, tonariX] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[tonariY, tonariX];
                    ownerWindow.Field[tonariY, tonariX] = temp;
                }
            }
            //左下と入れ替え
            if (x > 0 && y < ownerWindow.Field.GetLength(0) - 1)
            {
                int tonariX = x - 1;
                int tonariY = y + 1;
                if (ownerWindow.Field[tonariY, tonariX] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[tonariY, tonariX];
                    ownerWindow.Field[tonariY, tonariX] = temp;
                }
            }
            //右下と入れ替え
            if (x < ownerWindow.Field.GetLength(1) - 1 && y < ownerWindow.Field.GetLength(0) - 1)
            {
                int tonariX = x + 1;
                int tonariY = y + 1;
                if (ownerWindow.Field[tonariY, tonariX] == DropState.None)
                {
                    DropState temp = ownerWindow.Field[y, x];
                    ownerWindow.Field[y, x] = ownerWindow.Field[tonariY, tonariX];
                    ownerWindow.Field[tonariY, tonariX] = temp;
                }
            }

            ownerWindow.DrawField();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ReleaseDrop();
        }

        private void ReleaseDrop()
        {
            //ドロップ状態無しを探して、移動中ドロップに状態を変更
            for (int y = 0; y < ownerWindow.Field.GetLength(0); y++)
            {
                for (int x = 0; x < ownerWindow.Field.GetLength(1); x++)
                {
                    if (ownerWindow.Field[y, x] == DropState.None)
                    {
                        ownerWindow.Field[y, x] = dropState;
                        ownerWindow.DrawField();
                    }
                }
            }
            ownerWindow.DropRemoveAndDawn();
        }
    }
}
