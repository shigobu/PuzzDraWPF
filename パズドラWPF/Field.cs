using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace パズドラWPF
{
	class Field
	{
		/// <summary>
		/// 盤面高さ
		/// </summary>
		internal const int FieldHeight = 5;
		/// <summary>
		/// 盤面幅
		/// </summary>
		internal const int FieldWidth = 6;

		internal bool falling = false;

		/// <summary>
		/// 盤面のドロップ状態を表します。
		/// </summary>
		internal DropState[,] DropStates = new DropState[FieldHeight, FieldWidth]
		{
			{ DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
			{ DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
			{ DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
			{ DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None },
			{ DropState.None, DropState.None, DropState.None, DropState.None, DropState.None, DropState.None }
		};

		/// <summary>
		/// 接続チェックフラグ
		/// </summary>
		private bool[,] FieldChecked = new bool[FieldHeight, FieldWidth]
		{
			{ false, false, false, false, false, false },
			{ false, false, false, false, false, false },
			{ false, false, false, false, false, false },
			{ false, false, false, false, false, false },
			{ false, false, false, false, false, false }
		};

		/// <summary>
		/// つながっている、削除される予定のドロップのグループを取得します。
		/// </summary>
		internal List<List<System.Drawing.Point>> GetConnectedDropGroup()
		{
			List<List<System.Drawing.Point>> connectedDropLists = new List<List<System.Drawing.Point>>();

			//横方向
			for (int y = 0; y < DropStates.GetLength(0); y++)
			{
				for (int x = 0; x < DropStates.GetLength(1) - 2; x++)
				{
					int count = GetConnectedDropCountHorizontal(y, x, DropStates[y, x], 0);
					if (count >= 3)
					{
						connectedDropLists.Add(GetConnectedDropListH(y, x, DropStates[y, x], new List<System.Drawing.Point>()));
					}
				}
			}

			//フラグリセット
			ResetFieldChecked();

			//縦方向
			List<List<System.Drawing.Point>> connectedDropListsV = new List<List<System.Drawing.Point>>();
			for (int x = 0; x < DropStates.GetLength(1); x++)
			{
				for (int y = 0; y < DropStates.GetLength(0) - 2; y++)
				{
					int count = GetConnectedDropCountVertical(y, x, DropStates[y, x], 0);
					if (count >= 3)
					{
						connectedDropListsV.Add(GetConnectedDropListV(y, x, DropStates[y, x], new List<System.Drawing.Point>()));
					}
				}
			}


			//フラグリセット
			ResetFieldChecked();

			connectedDropLists.AddRange(connectedDropListsV);

			return connectedDropLists;

		}

		/// <summary>
		/// 水平方向で同じドロップの接続されている個数を取得します。
		/// </summary>
		/// <param name="y"></param>
		/// <param name="x"></param>
		/// <param name="dropState"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private int GetConnectedDropCountHorizontal(int y, int x, DropState dropState, int count)
		{
			if (
				x < 0 || y < 0 || y >= DropStates.GetLength(0) || x >= DropStates.GetLength(1)
				|| FieldChecked[y, x]
				|| DropStates[y, x] == DropState.None
				|| DropStates[y, x] != dropState
				)
			{
				return count;
			}

			count++;
			FieldChecked[y, x] = true;

			count = GetConnectedDropCountHorizontal(y, x + 1, dropState, count);
			return count;
		}

		/// <summary>
		/// 垂直方向で同じドロップの接続されている個数を取得します。
		/// </summary>
		/// <param name="y"></param>
		/// <param name="x"></param>
		/// <param name="dropState"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private int GetConnectedDropCountVertical(int y, int x, DropState dropState, int count)
		{
			if (
				x < 0 || y < 0 || y >= DropStates.GetLength(0) || x >= DropStates.GetLength(1)
				|| FieldChecked[y, x]
				|| DropStates[y, x] == DropState.None
				|| DropStates[y, x] != dropState
				)
			{
				return count;
			}

			count++;
			FieldChecked[y, x] = true;

			count = GetConnectedDropCountVertical(y + 1, x, dropState, count);
			return count;
		}

		/// <summary>
		/// 水平方向で指定の連続したドロップの座標一覧を返します。
		/// </summary>
		/// <param name="y"></param>
		/// <param name="x"></param>
		/// <param name="dropState"></param>
		/// <param name="points"></param>
		private List<System.Drawing.Point> GetConnectedDropListH(int y, int x, DropState dropState, List<System.Drawing.Point> points)
		{
			if (
				x < 0 || y < 0 || y >= DropStates.GetLength(0) || x >= DropStates.GetLength(1)
				|| DropStates[y, x] == DropState.None
				|| DropStates[y, x] != dropState
				)
			{
				return points;
			}

			points.Add(new System.Drawing.Point(x, y));

			return GetConnectedDropListH(y, x + 1, dropState, points);
		}

		/// <summary>
		/// 垂直方向で指定の連続したドロップの座標一覧を返します。
		/// </summary>
		/// <param name="y"></param>
		/// <param name="x"></param>
		/// <param name="dropState"></param>
		/// <param name="points"></param>
		private List<System.Drawing.Point> GetConnectedDropListV(int y, int x, DropState dropState, List<System.Drawing.Point> points)
		{
			if (
				x < 0 || y < 0 || y >= DropStates.GetLength(0) || x >= DropStates.GetLength(1)
				|| DropStates[y, x] == DropState.None
				|| DropStates[y, x] != dropState
				)
			{
				return points;
			}

			points.Add(new System.Drawing.Point(x, y));

			return GetConnectedDropListV(y + 1, x, dropState, points);
		}

		/// <summary>
		/// 盤面チェックフラグをリセットします。
		/// </summary>
		private void ResetFieldChecked()
		{
			//フラグリセット
			for (int y = 0; y < FieldChecked.GetLength(0); y++)
			{
				for (int x = 0; x < FieldChecked.GetLength(1); x++)
				{
					FieldChecked[y, x] = false;
				}
			}

		}

	}
}
