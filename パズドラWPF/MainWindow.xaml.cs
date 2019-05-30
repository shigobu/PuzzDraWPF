﻿using System;
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
        /// 盤面高さ
        /// </summary>
        private const int FieldHeight = 5;
        /// <summary>
        /// 盤面幅
        /// </summary>
        private const int FieldWidth  = 6;

        /// <summary>
        /// 盤面のドロップ状態を表します。
        /// </summary>
        private DropState[,] Field = new DropState[FieldHeight, FieldWidth] { { DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
                                                                              { DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
                                                                              { DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
                                                                              { DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
                                                                              { DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None }};

        /// <summary>
        /// 盤面に配置してある円オブジェクトを表します。
        /// </summary>
        private Ellipse[,] FieldDrop = null;

        public MainWindow()
        {
            InitializeComponent();

            FieldDrop = new Ellipse[FieldHeight, FieldWidth] { { drop00, drop01, drop02, drop03, drop04, drop05 },
                                                               { drop10, drop11, drop12, drop13, drop14, drop15 },
                                                               { drop20, drop21, drop22, drop23, drop24, drop25 },
                                                               { drop30, drop31, drop32, drop33, drop34, drop35 },
                                                               { drop40, drop41, drop42, drop43, drop44, drop45 }};

            //盤面をランダムに設定
            Random random = new Random();
            for(int y = 0; y < Field.GetLength(0); y++)
            {
                for (int x = 0; x < Field.GetLength(1); x++)
                {
                    Field[y,x] = (DropState)random.Next((int)DropState.DropState1, (int)DropState.DropState6);
                }
            }

            //盤面更新
            DrawField();
        }

        /// <summary>
        /// 盤面を更新します。
        /// </summary>
        private void DrawField()
        {
            //nullチェック
            if (FieldDrop == null)
            {
                return;
            }

            for (int y = 0; y < Field.GetLength(0); y++)
            {
                for (int x = 0; x < Field.GetLength(1); x++)
                {
                    FieldDrop[y, x].Fill = GetDropBrush(Field[y, x]);
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
    }
}
