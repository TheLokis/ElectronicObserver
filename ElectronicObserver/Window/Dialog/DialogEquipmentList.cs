using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogEquipmentList : Form
	{


		private DataGridViewCellStyle CSDefaultLeft, CSDefaultRight,
			CSUnselectableLeft, CSUnselectableRight;


		public DialogEquipmentList()
		{

            this.InitializeComponent();

			ControlHelper.SetDoubleBuffered(this.EquipmentView);

            this.Font = Utility.Configuration.Config.UI.MainFont;

			foreach (DataGridViewColumn column in this.EquipmentView.Columns)
			{
				column.MinimumWidth = 2;
			}



            #region CellStyle

            this.CSDefaultLeft = new DataGridViewCellStyle
			{
				Alignment = DataGridViewContentAlignment.MiddleLeft,
				BackColor = SystemColors.Control,
				Font = Font,
				ForeColor = SystemColors.ControlText,
				SelectionBackColor = Color.FromArgb(0xFF, 0xFF, 0xCC),
				SelectionForeColor = SystemColors.ControlText,
				WrapMode = DataGridViewTriState.False
			};

            this.CSDefaultRight = new DataGridViewCellStyle(this.CSDefaultLeft)
			{
				Alignment = DataGridViewContentAlignment.MiddleRight
			};

            this.CSUnselectableLeft = new DataGridViewCellStyle(this.CSDefaultLeft);
            this.CSUnselectableLeft.SelectionForeColor = this.CSUnselectableLeft.ForeColor;
            this.CSUnselectableLeft.SelectionBackColor = this.CSUnselectableLeft.BackColor;

            this.CSUnselectableRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSUnselectableRight.SelectionForeColor = this.CSUnselectableRight.ForeColor;
            this.CSUnselectableRight.SelectionBackColor = this.CSUnselectableRight.BackColor;


            this.EquipmentView.DefaultCellStyle = this.CSDefaultRight;
            this.EquipmentView_Name.DefaultCellStyle = this.CSDefaultLeft;
            this.EquipmentView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            this.DetailView.DefaultCellStyle = this.CSUnselectableRight;
            this.DetailView_EquippedShip.DefaultCellStyle = this.CSUnselectableLeft;
            this.DetailView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

			#endregion

		}

		private void DialogEquipmentList_Load(object sender, EventArgs e)
		{

            this.UpdateView();

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormEquipmentList]);

		}


		private void EquipmentView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			if (e.Column.Index == this.EquipmentView_Name.Index)
			{

				int id1 = (int)this.EquipmentView[this.EquipmentView_ID.Index, e.RowIndex1].Value;
				int id2 = (int)this.EquipmentView[this.EquipmentView_ID.Index, e.RowIndex2].Value;

				e.SortResult =
					KCDatabase.Instance.MasterEquipments[id1].CategoryType -
					KCDatabase.Instance.MasterEquipments[id2].CategoryType;

				if (e.SortResult == 0)
				{
					e.SortResult = id1 - id2;
				}

			}
			else
			{

				e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);

			}


			if (e.SortResult == 0)
			{
				e.SortResult = (this.EquipmentView.Rows[e.RowIndex1].Tag as int? ?? 0) - (this.EquipmentView.Rows[e.RowIndex2].Tag as int? ?? 0);
			}

			e.Handled = true;

		}

		private void EquipmentView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < this.EquipmentView.Rows.Count; i++)
                this.EquipmentView.Rows[i].Tag = i;

		}

		private void EquipmentView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == this.EquipmentView_Icon.Index)
			{
				e.Value = ResourceManager.GetEquipmentImage((int)e.Value);
				e.FormattingApplied = true;
			}

		}

		private void EquipmentView_SelectionChanged(object sender, EventArgs e)
		{

			if (this.EquipmentView.Enabled && this.EquipmentView.SelectedRows != null && this.EquipmentView.SelectedRows.Count > 0)
                this.UpdateDetailView((int)this.EquipmentView[this.EquipmentView_ID.Index, this.EquipmentView.SelectedRows[0].Index].Value);

		}


		private void DetailView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex != this.DetailView_EquippedShip.Index)
			{
				if (this.AreEqual(e.RowIndex, e.RowIndex - 1))
				{

					e.Value = "";
					e.FormattingApplied = true;
					return;
				}
			}

			if (e.ColumnIndex == this.DetailView_Level.Index)
			{

				e.Value = "+" + e.Value;
				e.FormattingApplied = true;

			}

		}

		private void DetailView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{

			e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;

			if (this.AreEqual(e.RowIndex, e.RowIndex + 1))
			{
				e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
			}
			else
			{
				e.AdvancedBorderStyle.Bottom = this.DetailView.AdvancedCellBorderStyle.Bottom;
			}

		}

		/// <summary>
		/// DetailView 内の行が同じレベルであるかを判定します。
		/// </summary>
		private bool AreEqual(int rowIndex1, int rowIndex2)
		{

			if (rowIndex1 < 0 ||
				rowIndex1 >= this.DetailView.Rows.Count ||
				rowIndex2 < 0 ||
				rowIndex2 >= this.DetailView.Rows.Count)
				return false;

			return ((IComparable)this.DetailView[this.DetailView_Level.Index, rowIndex1].Value)
				.CompareTo(this.DetailView[this.DetailView_Level.Index, rowIndex2].Value) == 0 &&
				((IComparable)this.DetailView[this.DetailView_AircraftLevel.Index, rowIndex1].Value)
				.CompareTo(this.DetailView[this.DetailView_AircraftLevel.Index, rowIndex2].Value) == 0;
		}


		/// <summary>
		/// 一覧ビューを更新します。
		/// </summary>
		private void UpdateView()
		{

			var ships = KCDatabase.Instance.Ships.Values;
			var equipments = KCDatabase.Instance.Equipments.Values;
			var masterEquipments = KCDatabase.Instance.MasterEquipments;
			int masterCount = masterEquipments.Values.Count(eq => !eq.IsAbyssalEquipment);

			var allCount = equipments.GroupBy(eq => eq.EquipmentID).ToDictionary(group => group.Key, group => group.Count());

			var remainCount = new Dictionary<int, int>(allCount);


			//剰余数計算
			foreach (var eq in ships
				.SelectMany(s => s.AllSlotInstanceMaster)
				.Where(eq => eq != null))
			{

				remainCount[eq.EquipmentID]--;
			}

			foreach (var eq in KCDatabase.Instance.BaseAirCorps.Values
				.SelectMany(corps => corps.Squadrons.Values.Select(sq => sq.EquipmentInstance))
				.Where(eq => eq != null))
			{

				remainCount[eq.EquipmentID]--;
			}

			foreach (var eq in KCDatabase.Instance.RelocatedEquipments.Values
				.Where(eq => eq.EquipmentInstance != null))
			{

				remainCount[eq.EquipmentInstance.EquipmentID]--;
			}



            //表示処理
            this.EquipmentView.SuspendLayout();

            this.EquipmentView.Enabled = false;
            this.EquipmentView.Rows.Clear();


			var rows = new List<DataGridViewRow>(allCount.Count);
			var ids = allCount.Keys;

			foreach (int id in ids)
			{

				var row = new DataGridViewRow();
				row.CreateCells(this.EquipmentView);
				row.SetValues(
					id,
					masterEquipments[id].IconType,
					masterEquipments[id].Name,
					allCount[id],
					remainCount[id]
					);

                {
                    StringBuilder sb = new StringBuilder();
                    var eq = masterEquipments[id];

                    sb.AppendFormat("{0} {1}\r\n", eq.CategoryTypeInstance.Name, eq.Name, eq.EquipmentID);
                    if (eq.Firepower != 0) sb.AppendFormat("화력 {0:+0;-0}\r\n", eq.Firepower);
                    if (eq.Torpedo != 0) sb.AppendFormat("뇌장 {0:+0;-0}\r\n", eq.Torpedo);
                    if (eq.AA != 0) sb.AppendFormat("대공 {0:+0;-0}\r\n", eq.AA);
                    if (eq.Armor != 0) sb.AppendFormat("장갑 {0:+0;-0}\r\n", eq.Armor);
                    if (eq.ASW != 0) sb.AppendFormat("대잠 {0:+0;-0}\r\n", eq.ASW);
                    if (eq.Evasion != 0) sb.AppendFormat("{0} {1:+0;-0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? "요격" : "회피", eq.Evasion);
                    if (eq.LOS != 0) sb.AppendFormat("색적 {0:+0;-0}\r\n", eq.LOS);
                    if (eq.Accuracy != 0) sb.AppendFormat("{0} {1:+0;-0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? "대폭" : "명중", eq.Accuracy);
                    if (eq.Bomber != 0) sb.AppendFormat("폭장 {0:+0;-0}\r\n", eq.Bomber);
                    sb.AppendLine("(우클릭으로 도감에)");

					row.Cells[2].ToolTipText = sb.ToString();
                }

                rows.Add(row);
			}

			for (int i = 0; i < rows.Count; i++)
				rows[i].Tag = i;

            this.EquipmentView.Rows.AddRange(rows.ToArray());

            this.EquipmentView.Sort(this.EquipmentView_Name, ListSortDirection.Ascending);


            this.EquipmentView.Enabled = true;
            this.EquipmentView.ResumeLayout();

			if (this.EquipmentView.Rows.Count > 0)
                this.EquipmentView.CurrentCell = this.EquipmentView[0, 0];

		}

        private void EquipmentView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (0 <= e.RowIndex && e.RowIndex < this.EquipmentView.RowCount)
            {
                int equipmentID = (int)this.EquipmentView.Rows[e.RowIndex].Cells[0].Value;

                if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
                {
                    this.Cursor = Cursors.AppStarting;
                    new DialogAlbumMasterEquipment(equipmentID).Show(this.Owner);
                    this.Cursor = Cursors.Default;
                }
            }
        }


        private class DetailCounter : IIdentifiable
		{

			public int level;
			public int aircraftLevel;
			public int countAll;
			public int countRemain;
			public int countRemainPrev;

			public List<string> equippedShips;

			public DetailCounter(int lv, int aircraftLv)
			{
                this.level = lv;
                this.aircraftLevel = aircraftLv;
                this.countAll = 0;
                this.countRemainPrev = 0;
                this.countRemain = 0;
                this.equippedShips = new List<string>();
			}

			public static int CalculateID(int level, int aircraftLevel)
			{
				return level + aircraftLevel * 100;
			}

			public static int CalculateID(EquipmentData eq)
			{
				return CalculateID(eq.Level, eq.AircraftLevel);
			}

			public int ID => CalculateID(this.level, this.aircraftLevel); }


		/// <summary>
		/// 詳細ビューを更新します。
		/// </summary>
		private void UpdateDetailView(int equipmentID)
		{

            this.DetailView.SuspendLayout();

            this.DetailView.Rows.Clear();

			//装備数カウント
			var eqs = KCDatabase.Instance.Equipments.Values.Where(eq => eq.EquipmentID == equipmentID);
			var countlist = new IDDictionary<DetailCounter>();

			foreach (var eq in eqs)
			{
				var c = countlist[DetailCounter.CalculateID(eq)];
				if (c == null)
				{
					countlist.Add(new DetailCounter(eq.Level, eq.AircraftLevel));
					c = countlist[DetailCounter.CalculateID(eq)];
				}
				c.countAll++;
				c.countRemain++;
				c.countRemainPrev++;
			}

			//装備艦集計
			foreach (var ship in KCDatabase.Instance.Ships.Values)
			{

				foreach (var eq in ship.AllSlotInstance.Where(s => s != null && s.EquipmentID == equipmentID))
				{

					countlist[DetailCounter.CalculateID(eq)].countRemain--;
				}

				foreach (var c in countlist.Values)
				{
					if (c.countRemain != c.countRemainPrev)
					{

						int diff = c.countRemainPrev - c.countRemain;

						c.equippedShips.Add(ship.NameWithLevel + (diff > 1 ? (" x" + diff) : ""));

						c.countRemainPrev = c.countRemain;
					}
				}

			}

			// 基地航空隊 - 配備中の装備を集計
			foreach (var corps in KCDatabase.Instance.BaseAirCorps.Values)
			{

				foreach (var sq in corps.Squadrons.Values.Where(sq => sq != null && sq.EquipmentID == equipmentID))
				{

					countlist[DetailCounter.CalculateID(sq.EquipmentInstance)].countRemain--;
				}

				foreach (var c in countlist.Values)
				{
					if (c.countRemain != c.countRemainPrev)
					{
						int diff = c.countRemainPrev - c.countRemain;

						c.equippedShips.Add(string.Format("#{0} {1}{2}", corps.MapAreaID, corps.Name, diff > 1 ? (" x" + diff) : ""));

						c.countRemainPrev = c.countRemain;
					}
				}

			}

			// 基地航空隊 - 配置転換中の装備を集計
			foreach (var eq in KCDatabase.Instance.RelocatedEquipments.Values
				.Select(v => v.EquipmentInstance)
				.Where(eq => eq != null && eq.EquipmentID == equipmentID))
			{

				countlist[DetailCounter.CalculateID(eq)].countRemain--;
			}

			foreach (var c in countlist.Values)
			{
				if (c.countRemain != c.countRemainPrev)
				{

					int diff = c.countRemainPrev - c.countRemain;

					c.equippedShips.Add("배치전환중" + (diff > 1 ? (" x" + diff) : ""));

					c.countRemainPrev = c.countRemain;
				}
			}


			//行に反映
			var rows = new List<DataGridViewRow>(eqs.Count());

			foreach (var c in countlist.Values)
			{

				if (c.equippedShips.Count() == 0)
				{
					c.equippedShips.Add("");
				}

				foreach (var s in c.equippedShips)
				{

					var row = new DataGridViewRow();
					row.CreateCells(this.DetailView);
					row.SetValues(c.level, c.aircraftLevel, c.countAll, c.countRemain, s);
					rows.Add(row);
				}

			}

            this.DetailView.Rows.AddRange(rows.ToArray());
            this.DetailView.Sort(this.DetailView_AircraftLevel, ListSortDirection.Ascending);
            this.DetailView.Sort(this.DetailView_Level, ListSortDirection.Ascending);

            this.DetailView.ResumeLayout();

            this.Text = "장비목록 - " + KCDatabase.Instance.MasterEquipments[equipmentID].Name;
		}


		private void DetailView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);

			if (e.SortResult == 0)
			{
				e.SortResult = (this.DetailView.Rows[e.RowIndex1].Tag as int? ?? 0) - (this.DetailView.Rows[e.RowIndex2].Tag as int? ?? 0);

				if (this.DetailView.SortOrder == SortOrder.Descending)
					e.SortResult = -e.SortResult;
			}

			e.Handled = true;
		}

		private void DetailView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < this.DetailView.Rows.Count; i++)
                this.DetailView.Rows[i].Tag = i;

		}



		private void Menu_File_CSVOutput_Click(object sender, EventArgs e)
		{

			if (this.SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{

				try
				{

					using (StreamWriter sw = new StreamWriter(this.SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
					{

						sw.WriteLine("고유ID,장비ID,장비명,개수Lv,함재기Lv,잠금,장비함ID,장비함");
						string arg = string.Format("{{{0}}}", string.Join("},{", Enumerable.Range(0, 8)));

						foreach (var eq in KCDatabase.Instance.Equipments.Values)
						{

							if (eq.Name == "なし") continue;

							ShipData equippedShip = KCDatabase.Instance.Ships.Values.FirstOrDefault(s => s.AllSlot.Contains(eq.MasterID));


							sw.WriteLine(arg,
								eq.MasterID,
								eq.EquipmentID,
								eq.Name,
								eq.Level,
								eq.AircraftLevel,
								eq.IsLocked ? 1 : 0,
								equippedShip?.MasterID ?? -1,
								equippedShip?.NameWithLevel ?? ""
								);

						}

					}

				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "장비 목록 CSV의 출력에 실패했습니다.");
					MessageBox.Show("장비 목록 CSV의 출력에 실패했습니다.\r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);

				}

			}

		}

        /// <summary>
        /// 「艦隊分析」装備情報反映用
        /// https://kancolle-fleetanalysis.firebaseapp.com/
        /// </summary>
        private void TopMenu_File_CopyToFleetAnalysis_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(
                "[" + string.Join(",", KCDatabase.Instance.Equipments.Values.Where(eq => eq?.IsLocked ?? false)
                .Select(eq => $"{{\"api_slotitem_id\":{eq.EquipmentID},\"api_level\":{eq.Level}}}")) + "]");
        }

        private void DialogEquipmentList_FormClosed(object sender, FormClosedEventArgs e)
		{

			ResourceManager.DestroyIcon(this.Icon);

		}

		private void TopMenu_File_Update_Click(object sender, EventArgs e)
		{

            this.UpdateView();
		}


	}
}
