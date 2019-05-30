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
using System.Windows.Shapes;

namespace パズドラWPF
{
    /// <summary>
    /// DropWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DropWindow : Window
    {
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

		}

        /// <summary>
        /// マウスの中心に円の中心が来るように移動させます。
        /// </summary>
        internal void MoveToMousePos()
        {
            System.Drawing.Point cursorPoint = System.Windows.Forms.Cursor.Position;
			this.Left = cursorPoint.X - 25;
			this.Top = cursorPoint.Y - 25;
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			MoveToMousePos();
		}

		private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}
	}
}
