using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace パズドラWPF
{
    /// <summary>
    /// 汎用的に使える共通関数を提供します。
    /// </summary>
    class Common
    {
        /// <summary>
        /// DPIを考慮したマウス座標を取得します。
        /// </summary>
        /// <returns>DPIを考慮したマウス座標</returns>
        internal static Point GetMouseCursorPosition()
        {
            //マウスカーソルの座標取得
            System.Drawing.Point cursorPoint = System.Windows.Forms.Cursor.Position;

            //DPI取得
            WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);
            System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(wih.Handle);
            double dpiX = desktop.DpiX;
            int defaultDpi = 96;

            return new Point(cursorPoint.X * (defaultDpi / dpiX), cursorPoint.Y * (defaultDpi / dpiX));
        }
    }
}
