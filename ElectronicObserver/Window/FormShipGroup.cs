using ElectronicObserver.Data;
using ElectronicObserver.Data.ShipGroup;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Dialog;
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
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window
{
	public partial class FormShipGroup : DockContent
	{


        /// <summary>タブ背景色(アクティブ)</summary>
        private readonly Color TabActiveColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.SubFontColor);

        /// <summary>タブ背景色(非アクティブ)</summary>
        private readonly Color TabInactiveColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);



        // セル背景色
        private readonly Color CellColorRed = Utility.ThemeManager.GetColor(Utility.ThemeColors.RedHighlight);
        private readonly Color CellColorOrange = Utility.ThemeManager.GetColor(Utility.ThemeColors.OrangeHighlight);
        private readonly Color CellColorYellow = Utility.ThemeManager.GetColor(Utility.ThemeColors.YellowHighlight);
        private readonly Color CellColorGreen = Utility.ThemeManager.GetColor(Utility.ThemeColors.GreenHighlight);
        private readonly Color CellColorGray = Utility.ThemeManager.GetColor(Utility.ThemeColors.GrayHighlight);
        private readonly Color CellColorCherry = Utility.ThemeManager.GetColor(Utility.ThemeColors.PinkHighlight);

        //セルスタイル
        private DataGridViewCellStyle CSDefaultLeft, CSDefaultCenter, CSDefaultRight,
			CSRedRight, CSOrangeRight, CSYellowRight, CSGreenRight, CSGrayRight, CSCherryRight,
			CSIsLocked;

		/// <summary>選択中のタブ</summary>
		private ImageLabel SelectedTab = null;

		/// <summary>選択中のグループ</summary>
		private ShipGroupData CurrentGroup => this.SelectedTab == null ? null : KCDatabase.Instance.ShipGroup[(int)this.SelectedTab.Tag];


		private bool IsRowsUpdating;
		private int _splitterDistance;
		private int _shipNameSortMethod;


		public FormShipGroup(FormMain parent)
		{
            this.InitializeComponent();

			ControlHelper.SetDoubleBuffered(this.ShipView);

            this.IsRowsUpdating = true;
            this._splitterDistance = -1;

			foreach (DataGridViewColumn column in this.ShipView.Columns)
			{
				column.MinimumWidth = 2;
			}

            #region set CellStyle

            this.CSDefaultLeft = new DataGridViewCellStyle
			{
				Alignment = DataGridViewContentAlignment.MiddleLeft,
				BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor),
                Font = Font,
				ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor),
                SelectionBackColor = Color.FromArgb(0xFF, 0xFF, 0xCC),
				SelectionForeColor = SystemColors.ControlText,
				WrapMode = DataGridViewTriState.False
			};

            this.CSDefaultCenter = new DataGridViewCellStyle(this.CSDefaultLeft)
			{
				Alignment = DataGridViewContentAlignment.MiddleCenter
			};

            this.CSDefaultRight = new DataGridViewCellStyle(this.CSDefaultLeft)
			{
				Alignment = DataGridViewContentAlignment.MiddleRight
			};

            this.CSRedRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSRedRight.BackColor =
            this.CSRedRight.SelectionBackColor = this.CellColorRed;

            this.CSOrangeRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSOrangeRight.BackColor =
            this.CSOrangeRight.SelectionBackColor = this.CellColorOrange;

            this.CSYellowRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSYellowRight.BackColor =
            this.CSYellowRight.SelectionBackColor = this.CellColorYellow;

            this.CSGreenRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSGreenRight.BackColor =
            this.CSGreenRight.SelectionBackColor = this.CellColorGreen;

            this.CSGrayRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSGrayRight.ForeColor =
            this.CSGrayRight.SelectionForeColor = this.CellColorGray;

            this.CSCherryRight = new DataGridViewCellStyle(this.CSDefaultRight);
            this.CSCherryRight.BackColor =
            this.CSCherryRight.SelectionBackColor = this.CellColorCherry;

            this.CSIsLocked = new DataGridViewCellStyle(this.CSDefaultCenter);
            this.CSIsLocked.ForeColor =
            this.CSIsLocked.SelectionForeColor = Color.FromArgb(0xFF, 0x88, 0x88);


            this.ShipView.DefaultCellStyle = this.CSDefaultRight;
            this.ShipView_Name.DefaultCellStyle = this.CSDefaultLeft;
            this.ShipView_Slot1.DefaultCellStyle = this.CSDefaultLeft;
            this.ShipView_Slot2.DefaultCellStyle = this.CSDefaultLeft;
            this.ShipView_Slot3.DefaultCellStyle = this.CSDefaultLeft;
            this.ShipView_Slot4.DefaultCellStyle = this.CSDefaultLeft;
            this.ShipView_Slot5.DefaultCellStyle = this.CSDefaultLeft;
            this.ShipView_ExpansionSlot.DefaultCellStyle = this.CSDefaultLeft;

			#endregion


			SystemEvents.SystemShuttingDown += this.SystemShuttingDown;
		}


		private void FormShipGroup_Load(object sender, EventArgs e)
		{

			ShipGroupManager groups = KCDatabase.Instance.ShipGroup;


			// 空(≒初期状態)の時、おなじみ全所属艦を追加
			if (groups.ShipGroups.Count == 0)
			{

				Utility.Logger.Add(3, "ShipGroup: 그룹을 찾을 수 없습니다. 기본값으로 돌리려면 종료후 " + ShipGroupManager.DefaultFilePath + " 를 삭제하십시오.");

				var group = KCDatabase.Instance.ShipGroup.Add();
				group.Name = "모든함선";

				for (int i = 0; i < this.ShipView.Columns.Count; i++)
				{
					var newdata = new ShipGroupData.ViewColumnData(this.ShipView.Columns[i]);
					if (this.SelectedTab == null)
						newdata.Visible = true;     //初期状態では全行が非表示のため
					group.ViewColumns.Add(this.ShipView.Columns[i].Name, newdata);
				}
			}


			foreach (var g in groups.ShipGroups.Values)
			{
                this.TabPanel.Controls.Add(this.CreateTabLabel(g.GroupID));
			}


			//*/
			{
				int columnCount = this.ShipView.Columns.Count;
				for (int i = 0; i < columnCount; i++)
				{
                    this.ShipView.Columns[i].Visible = false;
				}
			}
            //*/


            this.ConfigurationChanged();


			APIObserver o = APIObserver.Instance;

			o.APIList["api_port/port"].ResponseReceived += this.APIUpdated;
			o.APIList["api_get_member/ship2"].ResponseReceived += this.APIUpdated;
			o.APIList["api_get_member/ship_deck"].ResponseReceived += this.APIUpdated;


			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

            this.IsRowsUpdating = false;
            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormShipGroup]);

		}


		void ConfigurationChanged()
		{

			var config = Utility.Configuration.Config;

            this.ShipView.Font = this.StatusBar.Font = this.Font = config.UI.MainFont;

            this.CSDefaultLeft.Font =
            this.CSDefaultCenter.Font =
            this.CSDefaultRight.Font =
            this.CSRedRight.Font =
            this.CSOrangeRight.Font =
            this.CSYellowRight.Font =
            this.CSGreenRight.Font =
            this.CSGrayRight.Font =
            this.CSCherryRight.Font =
            this.CSIsLocked.Font =
				config.UI.MainFont;

			foreach (System.Windows.Forms.Control c in this.TabPanel.Controls)
				c.Font = this.Font;

            this.MenuGroup_AutoUpdate.Checked = config.FormShipGroup.AutoUpdate;
            this.MenuGroup_ShowStatusBar.Checked = config.FormShipGroup.ShowStatusBar;
            this._shipNameSortMethod = config.FormShipGroup.ShipNameSortMethod;

            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.ShipView.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.ShipView.BackgroundColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.StatusBar.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.StatusBar.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);


            int rowHeight;
			if (config.UI.IsLayoutFixed)
			{
                this.ShipView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                this.ShipView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
				rowHeight = 21;
			}
			else
			{
                this.ShipView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                this.ShipView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

				if (this.ShipView.Rows.Count > 0)
					rowHeight = this.ShipView.Rows[0].GetPreferredHeight(0, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, false);
				else
					rowHeight = 21;
			}

			foreach (DataGridViewRow row in this.ShipView.Rows)
			{
				row.Height = rowHeight;
			}

		}


		// レイアウトロード時に呼ばれる
		public void ConfigureFromPersistString(string persistString)
		{

			string[] args = persistString.Split("?=&".ToCharArray());

			for (int i = 1; i < args.Length - 1; i += 2)
			{
				switch (args[i])
				{
					case "SplitterDistance":
                        // 直接変えるとサイズが足りないか何かで変更が適用されないことがあるため、 Resize イベント中に変更する(ために値を記録する)
                        // しかし Resize イベントだけだと呼ばれないことがあるため、直接変えてもおく
                        // つらい
                        this.splitContainer1.SplitterDistance = this._splitterDistance = int.Parse(args[i + 1]);
						break;
				}
			}
		}


		protected override string GetPersistString() => "ShipGroup?SplitterDistance=" + this.splitContainer1.SplitterDistance;



		/// <summary>
		/// 指定したグループIDに基づいてタブ ラベルを生成します。
		/// </summary>
		private ImageLabel CreateTabLabel(int id)
		{

			ImageLabel label = new ImageLabel
			{
				Text = KCDatabase.Instance.ShipGroup[id].Name,
				Anchor = AnchorStyles.Left,
				Font = this.ShipView.Font,
				BackColor = TabInactiveColor,
				BorderStyle = BorderStyle.FixedSingle,
				Padding = new Padding(4, 4, 4, 4),
				Margin = new Padding(0, 0, 0, 0),
				ImageAlign = ContentAlignment.MiddleCenter,
				AutoSize = true,
				Cursor = Cursors.Hand
			};

			//イベントと固有IDの追加(内部データとの紐付)
			label.Click += this.TabLabel_Click;
			label.MouseDown += this.TabLabel_MouseDown;
			label.MouseMove += this.TabLabel_MouseMove;
			label.MouseUp += this.TabLabel_MouseUp;
			label.ContextMenuStrip = this.MenuGroup;
			label.Tag = id;

			return label;
		}




		void TabLabel_Click(object sender, EventArgs e)
		{
            this.ChangeShipView(sender as ImageLabel);
		}

		private void APIUpdated(string apiname, dynamic data)
		{
			if (this.MenuGroup_AutoUpdate.Checked)
                this.ChangeShipView(this.SelectedTab);
		}






		/// <summary>
		/// ShipView用の新しい行のインスタンスを作成します。
		/// </summary>
		/// <param name="ship">追加する艦娘データ。</param>
		private DataGridViewRow CreateShipViewRow(ShipData ship)
		{

			if (ship == null) return null;

			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(this.ShipView);
			row.Height = 21;

			row.SetValues(
				ship.MasterID,
				ship.MasterShip.ShipType,
				ship.MasterShip.Name,
				ship.Level,
				ship.ExpTotal,
				ship.ExpNext,
				ship.ExpNextRemodel,
				new Fraction(ship.HPCurrent, ship.HPMax),
				ship.Condition,
				new Fraction(ship.Fuel, ship.FuelMax),
				new Fraction(ship.Ammo, ship.AmmoMax),
                this.GetEquipmentString(ship, 0),
                this.GetEquipmentString(ship, 1),
                this.GetEquipmentString(ship, 2),
                this.GetEquipmentString(ship, 3),
                this.GetEquipmentString(ship, 4),
                this.GetEquipmentString(ship, 5),        //補強スロット
				new Fraction(ship.Aircraft[0], ship.MasterShip.Aircraft[0]),
				new Fraction(ship.Aircraft[1], ship.MasterShip.Aircraft[1]),
				new Fraction(ship.Aircraft[2], ship.MasterShip.Aircraft[2]),
				new Fraction(ship.Aircraft[3], ship.MasterShip.Aircraft[3]),
				new Fraction(ship.Aircraft[4], ship.MasterShip.Aircraft[4]),
				new Fraction(ship.AircraftTotal, ship.MasterShip.AircraftTotal),
				ship.FleetWithIndex,
				ship.RepairingDockID == -1 ? ship.RepairTime : -1000 + ship.RepairingDockID,
				ship.RepairSteel,
				ship.RepairFuel,
				ship.FirepowerBase,
				ship.FirepowerRemain,
				ship.FirepowerTotal,
				ship.TorpedoBase,
				ship.TorpedoRemain,
				ship.TorpedoTotal,
				ship.AABase,
				ship.AARemain,
				ship.AATotal,
				ship.ArmorBase,
				ship.ArmorRemain,
				ship.ArmorTotal,
				ship.ASWBase,
				ship.ASWTotal,
				ship.EvasionBase,
				ship.EvasionTotal,
				ship.LOSBase,
				ship.LOSTotal,
				ship.LuckBase,
				ship.LuckRemain,
				ship.LuckTotal,
				ship.BomberTotal,
				ship.Speed,
				ship.Range,
				ship.AirBattlePower,
                ship.SumTorPower,
				ship.ShellingPower,
				ship.AircraftPower,
				ship.AntiSubmarinePower,
				ship.TorpedoPower,
				ship.NightBattlePower,
				ship.IsLocked ? 1 : ship.IsLockedByEquipment ? 2 : 0,
				ship.SallyArea
				);


			row.Cells[this.ShipView_Name.Index].Tag = ship.ShipID;
			//row.Cells[ShipView_Level.Index].Tag = ship.ExpTotal;


			{
				DataGridViewCellStyle cs;
				double hprate = ship.HPRate;
				if (hprate <= 0.25)
					cs = this.CSRedRight;
				else if (hprate <= 0.50)
					cs = this.CSOrangeRight;
				else if (hprate <= 0.75)
					cs = this.CSYellowRight;
				else if (hprate < 1.00)
					cs = this.CSGreenRight;
				else
					cs = this.CSDefaultRight;

				row.Cells[this.ShipView_HP.Index].Style = cs;
			}
			{
				DataGridViewCellStyle cs;
				if (ship.Condition < 20)
					cs = this.CSRedRight;
				else if (ship.Condition < 30)
					cs = this.CSOrangeRight;
				else if (ship.Condition < Utility.Configuration.Config.Control.ConditionBorder)
					cs = this.CSYellowRight;
				else if (ship.Condition < 50)
					cs = this.CSDefaultRight;
				else
					cs = this.CSGreenRight;

				row.Cells[this.ShipView_Condition.Index].Style = cs;
			}
			row.Cells[this.ShipView_Fuel.Index].Style = ship.Fuel < ship.FuelMax ? this.CSYellowRight : this.CSDefaultRight;
			row.Cells[this.ShipView_Ammo.Index].Style = ship.Ammo < ship.AmmoMax ? this.CSYellowRight : this.CSDefaultRight;
			{
				var current = ship.Aircraft;
				var max = ship.MasterShip.Aircraft;
				row.Cells[this.ShipView_Aircraft1.Index].Style = (max[0] > 0 && current[0] == 0) ? this.CSRedRight : (current[0] < max[0]) ? this.CSYellowRight : this.CSDefaultRight;
				row.Cells[this.ShipView_Aircraft2.Index].Style = (max[1] > 0 && current[1] == 0) ? this.CSRedRight : (current[1] < max[1]) ? this.CSYellowRight : this.CSDefaultRight;
				row.Cells[this.ShipView_Aircraft3.Index].Style = (max[2] > 0 && current[2] == 0) ? this.CSRedRight : (current[2] < max[2]) ? this.CSYellowRight : this.CSDefaultRight;
				row.Cells[this.ShipView_Aircraft4.Index].Style = (max[3] > 0 && current[3] == 0) ? this.CSRedRight : (current[3] < max[3]) ? this.CSYellowRight : this.CSDefaultRight;
				row.Cells[this.ShipView_Aircraft5.Index].Style = (max[4] > 0 && current[4] == 0) ? this.CSRedRight : (current[4] < max[4]) ? this.CSYellowRight : this.CSDefaultRight;
				row.Cells[this.ShipView_AircraftTotal.Index].Style = (ship.MasterShip.AircraftTotal > 0 && ship.AircraftTotal == 0) ? this.CSRedRight : (ship.AircraftTotal < ship.MasterShip.AircraftTotal) ? this.CSYellowRight : this.CSDefaultRight;
			}
			{
				DataGridViewCellStyle cs;
				if (ship.RepairTime == 0)
					cs = this.CSDefaultRight;
				else if (ship.RepairTime < 1000 * 60 * 60)
					cs = this.CSYellowRight;
				else if (ship.RepairTime < 1000 * 60 * 60 * 6)
					cs = this.CSOrangeRight;
				else
					cs = this.CSRedRight;

				row.Cells[this.ShipView_RepairTime.Index].Style = cs;
			}
			row.Cells[this.ShipView_FirepowerRemain.Index].Style = ship.FirepowerRemain == 0 ? this.CSGrayRight : this.CSDefaultRight;
			row.Cells[this.ShipView_TorpedoRemain.Index].Style = ship.TorpedoRemain == 0 ? this.CSGrayRight : this.CSDefaultRight;
			row.Cells[this.ShipView_AARemain.Index].Style = ship.AARemain == 0 ? this.CSGrayRight : this.CSDefaultRight;
			row.Cells[this.ShipView_ArmorRemain.Index].Style = ship.ArmorRemain == 0 ? this.CSGrayRight : this.CSDefaultRight;
			row.Cells[this.ShipView_LuckRemain.Index].Style = ship.LuckRemain == 0 ? this.CSGrayRight : this.CSDefaultRight;

			row.Cells[this.ShipView_Locked.Index].Style = ship.IsLocked ? this.CSIsLocked : this.CSDefaultCenter;

			return row;
		}


		/// <summary>
		/// 指定したタブのグループのShipViewを作成します。
		/// </summary>
		/// <param name="target">作成するビューのグループデータ</param>
		private void BuildShipView(ImageLabel target)
		{
			if (target == null)
				return;

			ShipGroupData group = KCDatabase.Instance.ShipGroup[(int)target.Tag];

            this.IsRowsUpdating = true;
            this.ShipView.SuspendLayout();

            this.UpdateMembers(group);

            this.ShipView.Rows.Clear();

			var ships = group.MembersInstance;
			var rows = new List<DataGridViewRow>(ships.Count());

			foreach (var ship in ships)
			{
				if (ship == null) continue;

				DataGridViewRow row = this.CreateShipViewRow(ship);
				rows.Add(row);
			}

			for (int i = 0; i < rows.Count; i++)
				rows[i].Tag = i;

            this.ShipView.Rows.AddRange(rows.ToArray());



			// 設定に抜けがあった場合補充
			if (group.ViewColumns == null)
			{
				group.ViewColumns = new Dictionary<string, ShipGroupData.ViewColumnData>();
			}
			if (this.ShipView.Columns.Count != group.ViewColumns.Count)
			{
				foreach (DataGridViewColumn column in this.ShipView.Columns)
				{

					if (group.ViewColumns.ContainsKey(column.Name) == false)
					{
						var newdata = new ShipGroupData.ViewColumnData(column)
						{
							Visible = true     //初期状態でインビジだと不都合なので
						};

						group.ViewColumns.Add(newdata.Name, newdata);
					}
				}
			}

            this.ApplyViewData(group);
            this.ApplyAutoSort(group);

			// 高さ設定(追加直後に実行すると高さが0になることがあるのでここで実行)
			int rowHeight = 21;
			if (!Utility.Configuration.Config.UI.IsLayoutFixed)
			{
				if (this.ShipView.Rows.Count > 0)
					rowHeight = this.ShipView.Rows[0].GetPreferredHeight(0, DataGridViewAutoSizeRowMode.AllCellsExceptHeader, false);
			}

			foreach (DataGridViewRow row in this.ShipView.Rows)
				row.Height = rowHeight;


            this.ShipView.ResumeLayout();
            this.IsRowsUpdating = false;

			//status bar
			if (KCDatabase.Instance.Ships.Count > 0)
			{
                int levelsum = group.MembersInstance.Sum(s => s?.Level ?? 0);
                int expsum = group.MembersInstance.Sum(s => s?.ExpTotal ?? 0);
                int membersCount = group.MembersInstance.Count(s => s != null);

                this.Status_ShipCount.Text = $"소속: {membersCount}척";
                this.Status_LevelTotal.Text = $"Lv: 합계 {levelsum} / 평균 {levelsum / Math.Max(membersCount, 1.0):F2}";
                this.Status_LevelAverage.Text = $"Exp: 합계 {expsum} / 평균 {expsum / Math.Max(membersCount, 1.0):F2}";
            }
		}


		/// <summary>
		/// ShipViewを指定したタブに切り替えます。
		/// </summary>
		private void ChangeShipView(ImageLabel target)
		{

			if (target == null) return;


			var group = KCDatabase.Instance.ShipGroup[(int)target.Tag];
			var currentGroup = this.CurrentGroup;

			int headIndex = 0;
			List<int> selectedIDList = new List<int>();

			if (group == null)
			{
				Utility.Logger.Add(3, "에러：존재하지 않는 그룹을 참조하였습니다. 개발자에게 연락하십시오.");
				return;
			}

			if (currentGroup != null)
			{

                this.UpdateMembers(currentGroup);

				if (this.CurrentGroup.GroupID != group.GroupID)
				{
                    this.ShipView.Rows.Clear();      //別グループの行の並び順を引き継がせないようにする

				}
				else
				{
					headIndex = this.ShipView.FirstDisplayedScrollingRowIndex;
					selectedIDList = this.ShipView.SelectedRows.Cast<DataGridViewRow>().Select(r => (int)r.Cells[this.ShipView_ID.Index].Value).ToList();
				}
			}


			if (this.SelectedTab != null)
                this.SelectedTab.BackColor = this.TabInactiveColor;

            this.SelectedTab = target;


            this.BuildShipView(this.SelectedTab);
            this.SelectedTab.BackColor = this.TabActiveColor;


			if (0 <= headIndex && headIndex < this.ShipView.Rows.Count)
			{
				try
				{
                    this.ShipView.FirstDisplayedScrollingRowIndex = headIndex;

				}
				catch (InvalidOperationException)
				{
					// 1行も表示できないサイズのときに例外が出るので握りつぶす
				}
			}

			if (selectedIDList.Count > 0)
			{
                this.ShipView.ClearSelection();
				for (int i = 0; i < this.ShipView.Rows.Count; i++)
				{
					var row = this.ShipView.Rows[i];
					if (selectedIDList.Contains((int)row.Cells[this.ShipView_ID.Index].Value))
					{
						row.Selected = true;
					}
				}
			}

		}


		private string GetEquipmentString(ShipData ship, int index)
		{

			if (index < 5)
			{
				return (index >= ship.SlotSize && ship.Slot[index] == -1) ? "" :
					ship.SlotInstance[index]?.NameWithLevel ?? "(없음)";
			}
			else
			{
				return ship.ExpansionSlot == 0 ? "" :
					ship.ExpansionSlotInstance?.NameWithLevel ?? "(없음)";
			}

		}


		/// <summary>
		/// 現在選択している艦船のIDリストを求めます。
		/// </summary>
		private IEnumerable<int> GetSelectedShipID()
		{
			return this.ShipView.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index).Select(r => (int)r.Cells[this.ShipView_ID.Index].Value);
		}


		/// <summary>
		/// 現在の表を基に、グループメンバーを更新します。
		/// </summary>
		private void UpdateMembers(ShipGroupData group)
		{
			group.UpdateMembers(this.ShipView.Rows.Cast<DataGridViewRow>().Select(r => (int)r.Cells[this.ShipView_ID.Index].Value));
		}


		private void ShipView_SelectionChanged(object sender, EventArgs e)
		{

			var group = this.CurrentGroup;
			if (KCDatabase.Instance.Ships.Count > 0 && group != null)
			{
				if (this.ShipView.Rows.GetRowCount(DataGridViewElementStates.Selected) >= 2)
				{
					var levels = this.ShipView.SelectedRows.Cast<DataGridViewRow>().Select(r => (int)r.Cells[this.ShipView_Level.Index].Value);
                    this.Status_ShipCount.Text = string.Format("선택: {0} / {1}척", this.ShipView.Rows.GetRowCount(DataGridViewElementStates.Selected), group.Members.Count);
                    this.Status_LevelTotal.Text = string.Format("합계Lv: {0}", levels.Sum());
                    this.Status_LevelAverage.Text = string.Format("평균Lv: {0:F2}", levels.Average());


				}
				else
				{
                    this.Status_ShipCount.Text = string.Format("소속: {0}척", group.Members.Count);
                    this.Status_LevelTotal.Text = string.Format("합계Lv: {0}", group.MembersInstance.Where(s => s != null).Sum(s => s.Level));
                    this.Status_LevelAverage.Text = string.Format("평균Lv: {0:F2}", group.Members.Count > 0 ? group.MembersInstance.Where(s => s != null).Average(s => s.Level) : 0);
				}

			}
			else
			{
                this.Status_ShipCount.Text =
                this.Status_LevelTotal.Text =
                this.Status_LevelAverage.Text = "";
			}
		}


		private void ShipView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == this.ShipView_ShipType.Index)
			{
                e.Value = FormMain.Instance.Translator.GetTranslation(KCDatabase.Instance.ShipTypes[(int)e.Value].Name, DataType.ShipType);
                e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ShipView_Fleet.Index)
			{
				if (e.Value == null)
					e.Value = "";
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ShipView_RepairTime.Index)
			{

				if ((int)e.Value < 0)
				{
					e.Value = "입거 #" + ((int)e.Value + 1000);
				}
				else
				{
					e.Value = DateTimeHelper.ToTimeRemainString(DateTimeHelper.FromAPITimeSpan((int)e.Value));
				}
				e.FormattingApplied = true;

			}
			else if ((
			  e.ColumnIndex == this.ShipView_FirepowerRemain.Index ||
			  e.ColumnIndex == this.ShipView_TorpedoRemain.Index ||
			  e.ColumnIndex == this.ShipView_AARemain.Index ||
			  e.ColumnIndex == this.ShipView_ArmorRemain.Index ||
			  e.ColumnIndex == this.ShipView_LuckRemain.Index
			  ) && (int)e.Value == 0)
			{
				e.Value = "MAX";
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ShipView_Aircraft1.Index ||
			   e.ColumnIndex == this.ShipView_Aircraft2.Index ||
			   e.ColumnIndex == this.ShipView_Aircraft3.Index ||
			   e.ColumnIndex == this.ShipView_Aircraft4.Index ||
			   e.ColumnIndex == this.ShipView_Aircraft5.Index)
			{   // AircraftTotal は 0 でも表示する
				if (((Fraction)e.Value).Max == 0)
				{
					e.Value = "";
					e.FormattingApplied = true;
				}

			}
			else if (e.ColumnIndex == this.ShipView_Locked.Index)
			{
				e.Value = (int)e.Value == 1 ? "❤" : (int)e.Value == 2 ? "■" : "";
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ShipView_SallyArea.Index && (int)e.Value == -1)
			{
				e.Value = "";
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ShipView_Range.Index)
			{
				e.Value = Constants.GetRange((int)e.Value);
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ShipView_Speed.Index)
			{
				e.Value = Constants.GetSpeed((int)e.Value);
				e.FormattingApplied = true;

			}

		}


		private void ShipView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			if (e.Column.Index == this.ShipView_Name.Index)
			{

				ShipDataMaster ship1 = KCDatabase.Instance.MasterShips[(int)this.ShipView.Rows[e.RowIndex1].Cells[e.Column.Index].Tag],
								ship2 = KCDatabase.Instance.MasterShips[(int)this.ShipView.Rows[e.RowIndex2].Cells[e.Column.Index].Tag];

				switch (this._shipNameSortMethod)
				{
					case 0:     // 図鑑番号順
					default:
						e.SortResult = ship1.AlbumNo - ship2.AlbumNo;
						break;

					case 1:     // あいうえお順
						e.SortResult = ship1.NameReading.CompareTo(ship2.NameReading);

						if (e.SortResult == 0)
							e.SortResult = ship1.Name.CompareTo(ship2.Name);
						break;

                    case 2:     // ソートキー順
                        e.SortResult = ship1.SortID - ship2.SortID;
                        break;
                }

			}
			else if (e.Column.Index == this.ShipView_Exp.Index)
			{
				e.SortResult = (int)e.CellValue1 - (int)e.CellValue2;
				if (e.SortResult == 0)  //for Lv.99-100
					e.SortResult = (int)this.ShipView[this.ShipView_Level.Index, e.RowIndex1].Value - (int)this.ShipView[this.ShipView_Level.Index, e.RowIndex2].Value;

			}
			else if (
			  e.Column.Index == this.ShipView_HP.Index ||
			  e.Column.Index == this.ShipView_Fuel.Index ||
			  e.Column.Index == this.ShipView_Ammo.Index ||
			  e.Column.Index == this.ShipView_Aircraft1.Index ||
			  e.Column.Index == this.ShipView_Aircraft2.Index ||
			  e.Column.Index == this.ShipView_Aircraft3.Index ||
			  e.Column.Index == this.ShipView_Aircraft4.Index ||
			  e.Column.Index == this.ShipView_Aircraft5.Index ||
			  e.Column.Index == this.ShipView_AircraftTotal.Index
			  )
			{
				Fraction frac1 = (Fraction)e.CellValue1, frac2 = (Fraction)e.CellValue2;

				double rate = frac1.Rate - frac2.Rate;

				if (rate > 0.0)
					e.SortResult = 1;
				else if (rate < 0.0)
					e.SortResult = -1;
				else
					e.SortResult = frac1.Current - frac2.Current;

			}
			else if (e.Column.Index == this.ShipView_Fleet.Index)
			{
				if ((string)e.CellValue1 == "")
				{
					if ((string)e.CellValue2 == "")
						e.SortResult = 0;
					else
						e.SortResult = 1;
				}
				else
				{
					if ((string)e.CellValue2 == "")
						e.SortResult = -1;
					else
						e.SortResult = ((string)e.CellValue1).CompareTo(e.CellValue2);
				}

			}
			else
			{
				e.SortResult = (int)e.CellValue1 - (int)e.CellValue2;
			}



			if (e.SortResult == 0)
			{
				e.SortResult = (int)this.ShipView.Rows[e.RowIndex1].Tag - (int)this.ShipView.Rows[e.RowIndex2].Tag;

				if (this.ShipView.SortOrder == SortOrder.Descending)
					e.SortResult = -e.SortResult;
			}

			e.Handled = true;
		}



		private void ShipView_Sorted(object sender, EventArgs e)
		{

			int count = this.ShipView.Rows.Count;
			var direction = this.ShipView.SortOrder;

			for (int i = 0; i < count; i++)
                this.ShipView.Rows[i].Tag = i;

		}





		// 列のサイズ変更関連
		private void ShipView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{

			if (this.IsRowsUpdating)
				return;

			var group = this.CurrentGroup;
			if (group != null)
			{

				if (!group.ViewColumns[e.Column.Name].AutoSize)
				{
					group.ViewColumns[e.Column.Name].Width = e.Column.Width;
				}
			}

		}

		private void ShipView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
		{

			if (this.IsRowsUpdating)
				return;

			var group = this.CurrentGroup;
			if (group != null)
			{

				foreach (DataGridViewColumn column in this.ShipView.Columns)
				{
					group.ViewColumns[column.Name].DisplayIndex = column.DisplayIndex;
				}
			}

		}




		#region メニュー:グループ操作

		private void MenuGroup_Add_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogTextInput("그룹을 추가", "그룹 이름을 입력하십시오："))
			{

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					var group = KCDatabase.Instance.ShipGroup.Add();


					group.Name = dialog.InputtedText.Trim();

					for (int i = 0; i < this.ShipView.Columns.Count; i++)
					{
						var newdata = new ShipGroupData.ViewColumnData(this.ShipView.Columns[i]);
						if (this.SelectedTab == null)
							newdata.Visible = true;     //初期状態では全行が非表示のため
						group.ViewColumns.Add(this.ShipView.Columns[i].Name, newdata);
					}

                    this.TabPanel.Controls.Add(this.CreateTabLabel(group.GroupID));

				}

			}

		}

		private void MenuGroup_Copy_Click(object sender, EventArgs e)
		{

			ImageLabel senderLabel = this.MenuGroup.SourceControl as ImageLabel;
			if (senderLabel == null)
				return;     //想定外

			using (var dialog = new DialogTextInput("그룹을 복사", "그룹 이름을 입력하십시오："))
			{

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					var group = KCDatabase.Instance.ShipGroup[(int)senderLabel.Tag].Clone();

					group.GroupID = KCDatabase.Instance.ShipGroup.GetUniqueID();
					group.Name = dialog.InputtedText.Trim();

					KCDatabase.Instance.ShipGroup.ShipGroups.Add(group);

                    this.TabPanel.Controls.Add(this.CreateTabLabel(group.GroupID));

				}
			}

		}

		private void MenuGroup_Delete_Click(object sender, EventArgs e)
		{

			ImageLabel senderLabel = this.MenuGroup.SourceControl as ImageLabel;
			if (senderLabel == null)
				return;     //想定外

			ShipGroupData group = KCDatabase.Instance.ShipGroup[(int)senderLabel.Tag];

			if (group != null)
			{
				if (MessageBox.Show(string.Format("그룹 [{0}] 을 삭제하시겠습니까?\r\n이 작업은 취소할 수 없습니다.", group.Name), "확인",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
					== System.Windows.Forms.DialogResult.Yes)
				{

					if (this.SelectedTab == senderLabel)
					{
                        this.ShipView.Rows.Clear();
                        this.SelectedTab = null;
					}
					KCDatabase.Instance.ShipGroup.ShipGroups.Remove(group);
                    this.TabPanel.Controls.Remove(senderLabel);
					senderLabel.Dispose();
				}

			}
			else
			{
				MessageBox.Show("이 그룹은 삭제할 수 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void MenuGroup_Rename_Click(object sender, EventArgs e)
		{

			ImageLabel senderLabel = this.MenuGroup.SourceControl as ImageLabel;
			if (senderLabel == null) return;

			ShipGroupData group = KCDatabase.Instance.ShipGroup[(int)senderLabel.Tag];

			if (group != null)
			{

				using (var dialog = new DialogTextInput("그룹 이름 변경", "그룹 이름을 입력하십시오："))
				{

					dialog.InputtedText = group.Name;

					if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{

						group.Name = senderLabel.Text = dialog.InputtedText.Trim();

					}
				}

			}
			else
			{
				MessageBox.Show("이 그룹의 이름을 변경할 수 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

		}


		private void TabPanel_DoubleClick(object sender, EventArgs e)
		{

            this.MenuGroup_Add.PerformClick();

		}



		#endregion


		#region メニューON/OFF操作
		private void MenuGroup_Opening(object sender, CancelEventArgs e)
		{

			if (this.MenuGroup.SourceControl == this.TabPanel || this.SelectedTab == null)
			{
                this.MenuGroup_Add.Enabled = true;
                this.MenuGroup_Copy.Enabled = false;
                this.MenuGroup_Rename.Enabled = false;
                this.MenuGroup_Delete.Enabled = false;
			}
			else
			{
                this.MenuGroup_Add.Enabled = true;
                this.MenuGroup_Copy.Enabled = true;
                this.MenuGroup_Rename.Enabled = true;
                this.MenuGroup_Delete.Enabled = true;
			}

		}

		private void MenuMember_Opening(object sender, CancelEventArgs e)
		{

			if (this.SelectedTab == null)
			{

				e.Cancel = true;
				return;
			}

			if (KCDatabase.Instance.Ships.Count == 0)
			{
                this.MenuMember_Filter.Enabled = false;
                this.MenuMember_CSVOutput.Enabled = false;

			}
			else
			{
                this.MenuMember_Filter.Enabled = true;
                this.MenuMember_CSVOutput.Enabled = true;
			}

			if (this.ShipView.Rows.GetRowCount(DataGridViewElementStates.Selected) == 0)
			{
                this.MenuMember_AddToGroup.Enabled = false;
                this.MenuMember_CreateGroup.Enabled = false;
                this.MenuMember_Exclude.Enabled = false;

			}
			else
			{
                this.MenuMember_AddToGroup.Enabled = true;
                this.MenuMember_CreateGroup.Enabled = true;
                this.MenuMember_Exclude.Enabled = true;

			}

		}
		#endregion


		#region メニュー:メンバー操作

		private void MenuMember_ColumnFilter_Click(object sender, EventArgs e)
		{

			ShipGroupData group = this.CurrentGroup;

			if (group == null)
			{
				MessageBox.Show("이 그룹은 변경할 수 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}


			try
			{
				using (var dialog = new DialogShipGroupColumnFilter(this.ShipView, group))
				{

					if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{

						group.ViewColumns = dialog.Result.ToDictionary(r => r.Name);
						group.ScrollLockColumnCount = dialog.ScrollLockColumnCount;

                        this.ApplyViewData(group);
					}



				}
			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "ShipGroup: 열 설정 창에서 오류가 발생했습니다");
			}
		}





		private void MenuMember_Filter_Click(object sender, EventArgs e)
		{

			var group = this.CurrentGroup;
			if (group != null)
			{

				try
				{
					if (group.Expressions == null)
						group.Expressions = new ExpressionManager();

					using (var dialog = new DialogShipGroupFilter(group))
					{

						if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
						{

							// replace
							int id = group.GroupID;
							group = dialog.ExportGroupData();
							group.GroupID = id;
							group.Expressions.Compile();

							KCDatabase.Instance.ShipGroup.ShipGroups.Remove(id);
							KCDatabase.Instance.ShipGroup.ShipGroups.Add(group);

                            this.ChangeShipView(this.SelectedTab);
						}
					}
				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "ShipGroup: 필터 창에서 오류가 발생했습니다.");
				}

			}
		}



		/// <summary>
		/// 表示設定を反映します。
		/// </summary>
		private void ApplyViewData(ShipGroupData group)
		{

            this.IsRowsUpdating = true;

			// いったん解除しないと列入れ替え時にエラーが起きる
			foreach (DataGridViewColumn column in this.ShipView.Columns)
			{
				column.Frozen = false;
			}

			foreach (var data in group.ViewColumns.Values.OrderBy(g => g.DisplayIndex))
			{
				data.ToColumn(this.ShipView.Columns[data.Name]);
			}

			int count = 0;
			foreach (var column in this.ShipView.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex))
			{
				column.Frozen = count < group.ScrollLockColumnCount;
				count++;
			}

            this.IsRowsUpdating = false;
		}


		private void MenuMember_SortOrder_Click(object sender, EventArgs e)
		{

			var group = this.CurrentGroup;

			if (group != null)
			{

				try
				{
					using (var dialog = new DialogShipGroupSortOrder(this.ShipView, group))
					{

						if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
						{

							group.AutoSortEnabled = dialog.AutoSortEnabled;
							group.SortOrder = dialog.Result;

                            this.ApplyAutoSort(group);
						}

					}
				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "ShipGroup: 자동 정렬 순서 창에서 오류가 발생했습니다.");
				}
			}

		}


		private void ApplyAutoSort(ShipGroupData group)
		{

			if (!group.AutoSortEnabled || group.SortOrder == null)
				return;

			// 一番上/最後に実行したほうが優先度が高くなるので逆順で
			for (int i = group.SortOrder.Count - 1; i >= 0; i--)
			{

				var order = group.SortOrder[i];
				ListSortDirection dir = order.Value;

				if (this.ShipView.Columns[order.Key].SortMode != DataGridViewColumnSortMode.NotSortable)
                    this.ShipView.Sort(this.ShipView.Columns[order.Key], dir);
			}


		}





		private void MenuMember_AddToGroup_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogTextSelect("그룹 선택", "추가할 그룹을 선택하십시오.：",
				KCDatabase.Instance.ShipGroup.ShipGroups.Values.ToArray()))
			{

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					var group = (ShipGroupData)dialog.SelectedItem;
					if (group != null)
					{
						group.AddInclusionFilter(this.GetSelectedShipID());

						if (group.ID == this.CurrentGroup.ID)
                            this.ChangeShipView(this.SelectedTab);
					}

				}
			}

		}

		private void MenuMember_CreateGroup_Click(object sender, EventArgs e)
		{

			var ships = this.GetSelectedShipID();
			if (ships.Count() == 0)
				return;

			using (var dialog = new DialogTextInput("그룹 추가", "그룹 이름을 입력하십시오.："))
			{

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					var group = KCDatabase.Instance.ShipGroup.Add();

					group.Name = dialog.InputtedText.Trim();

					for (int i = 0; i < this.ShipView.Columns.Count; i++)
					{
						var newdata = new ShipGroupData.ViewColumnData(this.ShipView.Columns[i]);
						if (this.SelectedTab == null)
							newdata.Visible = true;     //初期状態では全行が非表示のため
						group.ViewColumns.Add(this.ShipView.Columns[i].Name, newdata);
					}

					// 艦船ID == 0 を作成(空リストを作る)
					group.Expressions.Expressions.Add(new ExpressionList(false, true, false));
					group.Expressions.Expressions[0].Expressions.Add(new ExpressionData(".MasterID", ExpressionData.ExpressionOperator.Equal, 0));
					group.Expressions.Compile();

					group.AddInclusionFilter(ships);

                    this.TabPanel.Controls.Add(this.CreateTabLabel(group.GroupID));

				}

			}
		}

		private void MenuMember_Exclude_Click(object sender, EventArgs e)
		{

			var group = this.CurrentGroup;
			if (group != null)
			{

				group.AddExclusionFilter(this.GetSelectedShipID());

                this.ChangeShipView(this.SelectedTab);
			}

		}






        private static readonly string ShipCSVHeaderUser = "고유ID,함종,함명,Lv,Exp,next,개장까지,내구,최대내구,피로,연료,탄약,슬롯1,슬롯2,슬롯3,슬롯4,슬롯5,보강슬롯,입거,기본화력,화력개수,화력,기본뇌장,뇌장개수,뇌장,기본대공,대공개수,대공,기본장갑,장갑개수,장갑,기본대잠,대잠,기본회피,회피,기본색적,색적,기본운,운 개수,운,사정,속력,잠금,출격지,제공,화뇌합,포격위력,항공화력,대잠위력,뇌격위력,야간화력";

        private static readonly string ShipCSVHeaderData = "고유ID,함종,함명,함선ID,Lv,Exp,next,개장까지,내구,최대내구,피로,연료,탄약,슬롯1,슬롯2,슬롯3,슬롯4,슬롯5,보강슬롯,장비ID1,장비ID2,장비ID3,장비ID4,장비ID5,보강슬롯ID,탑재1,탑재2,탑재3,탑재4,탑재5,입거,입거연료,입거강재,기본화력,화력개수,화력,기본뇌장,뇌장개수,뇌장,기본대공,대공개수,대공,기본장갑,장갑개수,장갑,기본대잠,대잠,기본회피,회피,기본색적,색적,기본운,운 개수,운,사정,속력,잠금,출격지,제공,화뇌합,포격위력,항공화력,대잠공격,뇌격위력,야간화력";


		private void MenuMember_CSVOutput_Click(object sender, EventArgs e)
		{

			IEnumerable<ShipData> ships;

			if (this.SelectedTab == null)
			{
				ships = KCDatabase.Instance.Ships.Values;
			}
			else
			{
				//*/
				ships = this.ShipView.Rows.Cast<DataGridViewRow>().Select(r => KCDatabase.Instance.Ships[(int)r.Cells[this.ShipView_ID.Index].Value]);
				/*/
				var group = KCDatabase.Instance.ShipGroup[(int)SelectedTab.Tag];
				if ( group == null )
					ships = KCDatabase.Instance.Ships.Values;
				else
					ships = group.MembersInstance;
				//*/
			}


			using (var dialog = new DialogShipGroupCSVOutput())
			{

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					try
					{

						using (StreamWriter sw = new StreamWriter(dialog.OutputPath, false, Utility.Configuration.Config.Log.FileEncoding))
						{

							string header = dialog.OutputFormat == DialogShipGroupCSVOutput.OutputFormatConstants.User ? ShipCSVHeaderUser : ShipCSVHeaderData;
							sw.WriteLine(header);


							foreach (ShipData ship in ships.Where(s => s != null))
							{

								if (dialog.OutputFormat == DialogShipGroupCSVOutput.OutputFormatConstants.User)
								{

									sw.WriteLine(string.Join(",",
										ship.MasterID,
										ship.MasterShip.ShipTypeName,
										ship.MasterShip.NameWithClass,
										ship.Level,
										ship.ExpTotal,
										ship.ExpNext,
										ship.ExpNextRemodel,
										ship.HPCurrent,
										ship.HPMax,
										ship.Condition,
										ship.Fuel,
										ship.Ammo,
                                        this.GetEquipmentString(ship, 0),
                                        this.GetEquipmentString(ship, 1),
                                        this.GetEquipmentString(ship, 2),
                                        this.GetEquipmentString(ship, 3),
                                        this.GetEquipmentString(ship, 4),
                                        this.GetEquipmentString(ship, 5),
										DateTimeHelper.ToTimeRemainString(DateTimeHelper.FromAPITimeSpan(ship.RepairTime)),
										ship.FirepowerBase,
										ship.FirepowerRemain,
										ship.FirepowerTotal,
										ship.TorpedoBase,
										ship.TorpedoRemain,
										ship.TorpedoTotal,
										ship.AABase,
										ship.AARemain,
										ship.AATotal,
										ship.ArmorBase,
										ship.ArmorRemain,
										ship.ArmorTotal,
										ship.ASWBase,
										ship.ASWTotal,
										ship.EvasionBase,
										ship.EvasionTotal,
										ship.LOSBase,
										ship.LOSTotal,
										ship.LuckBase,
										ship.LuckRemain,
										ship.LuckTotal,
										Constants.GetRange(ship.Range),
										Constants.GetSpeed(ship.Speed),
										ship.IsLocked ? "●" : ship.IsLockedByEquipment ? "■" : "-",
										ship.SallyArea,
										ship.AirBattlePower,
                                        ship.SumTorPower,
                                        ship.ShellingPower,
										ship.AircraftPower,
										ship.AntiSubmarinePower,
										ship.TorpedoPower,
										ship.NightBattlePower
										));

								}
								else
								{       //data

									sw.WriteLine(string.Join(",",
										ship.MasterID,
										(int)ship.MasterShip.ShipType,
										ship.MasterShip.NameWithClass,
										ship.ShipID,
										ship.Level,
										ship.ExpTotal,
										ship.ExpNext,
										ship.ExpNextRemodel,
										ship.HPCurrent,
										ship.HPMax,
										ship.Condition,
										ship.Fuel,
										ship.Ammo,
                                        this.GetEquipmentString(ship, 0),
                                        this.GetEquipmentString(ship, 1),
                                        this.GetEquipmentString(ship, 2),
                                        this.GetEquipmentString(ship, 3),
                                        this.GetEquipmentString(ship, 4),
                                        this.GetEquipmentString(ship, 5),
										ship.Slot[0],
										ship.Slot[1],
										ship.Slot[2],
										ship.Slot[3],
										ship.Slot[4],
										ship.ExpansionSlot,
										ship.Aircraft[0],
										ship.Aircraft[1],
										ship.Aircraft[2],
										ship.Aircraft[3],
										ship.Aircraft[4],
										ship.RepairTime,
										ship.RepairFuel,
										ship.RepairSteel,
										ship.FirepowerBase,
										ship.FirepowerRemain,
										ship.FirepowerTotal,
										ship.TorpedoBase,
										ship.TorpedoRemain,
										ship.TorpedoTotal,
										ship.AABase,
										ship.AARemain,
										ship.AATotal,
										ship.ArmorBase,
										ship.ArmorRemain,
										ship.ArmorTotal,
										ship.ASWBase,
										ship.ASWTotal,
										ship.EvasionBase,
										ship.EvasionTotal,
										ship.LOSBase,
										ship.LOSTotal,
										ship.LuckBase,
										ship.LuckRemain,
										ship.LuckTotal,
										ship.Range,
										ship.Speed,
										ship.IsLocked ? 1 : ship.IsLockedByEquipment ? 2 : 0,
										ship.SallyArea,
										ship.AirBattlePower,
                                        ship.SumTorPower,
                                        ship.ShellingPower,
										ship.AircraftPower,
										ship.AntiSubmarinePower,
										ship.TorpedoPower,
										ship.NightBattlePower
										));

								}

							}

						}

						Utility.Logger.Add(2, "함선 그룹 CSV를 " + dialog.OutputPath + " 에 저장했습니다.");

					}
					catch (Exception ex)
					{
						Utility.ErrorReporter.SendErrorReport(ex, "함선 그룹 CSV 의 출력에 실패했습니다.");
						MessageBox.Show("함선 그룹 CSV의 출력에 실패했습니다.\r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}

				}
			}

		}




		#endregion


		#region タブ操作系

		private Point? _tempMouse = null;
		void TabLabel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
                this._tempMouse = this.TabPanel.PointToClient(e.Location);
			}
			else
			{
                this._tempMouse = null;
			}
		}

		void TabLabel_MouseMove(object sender, MouseEventArgs e)
		{

			if (this._tempMouse != null)
			{

				Rectangle move = new Rectangle(
                    this._tempMouse.Value.X - SystemInformation.DragSize.Width / 2,
                    this._tempMouse.Value.Y - SystemInformation.DragSize.Height / 2,
					SystemInformation.DragSize.Width,
					SystemInformation.DragSize.Height
					);

				if (!move.Contains(this.TabPanel.PointToClient(e.Location)))
				{
                    this.TabPanel.DoDragDrop(sender, DragDropEffects.All);
                    this._tempMouse = null;
				}
			}

		}

		void TabLabel_MouseUp(object sender, MouseEventArgs e)
		{
            this._tempMouse = null;
		}


		private void TabPanel_DragEnter(object sender, DragEventArgs e)
		{

			if (e.Data.GetDataPresent(typeof(ImageLabel)))
			{
				e.Effect = DragDropEffects.Move;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}

		}

		private void TabPanel_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{

			//右クリックでキャンセル
			if ((e.KeyState & 2) != 0)
			{
				e.Action = DragAction.Cancel;
			}

		}


		private void TabPanel_DragDrop(object sender, DragEventArgs e)
		{

			//fixme:カッコカリ　範囲外にドロップすると端に行く

			Point mp = this.TabPanel.PointToClient(new Point(e.X, e.Y));

			var item = this.TabPanel.GetChildAtPoint(mp);

			int index = this.TabPanel.Controls.GetChildIndex(item, false);

            this.TabPanel.Controls.SetChildIndex((System.Windows.Forms.Control)e.Data.GetData(typeof(ImageLabel)), index);

            this.TabPanel.Invalidate();

		}

		#endregion



		private void MenuGroup_ShowStatusBar_CheckedChanged(object sender, EventArgs e)
		{

            this.StatusBar.Visible = this.MenuGroup_ShowStatusBar.Checked;

		}



		void SystemShuttingDown()
		{

			Utility.Configuration.Config.FormShipGroup.AutoUpdate = this.MenuGroup_AutoUpdate.Checked;
			Utility.Configuration.Config.FormShipGroup.ShowStatusBar = this.MenuGroup_ShowStatusBar.Checked;



			ShipGroupManager groups = KCDatabase.Instance.ShipGroup;


			List<ImageLabel> list = this.TabPanel.Controls.OfType<ImageLabel>().OrderBy(c => this.TabPanel.Controls.GetChildIndex(c)).ToList();

			for (int i = 0; i < list.Count; i++)
			{

				ShipGroupData group = groups[(int)list[i].Tag];
				if (group != null)
					group.GroupID = i + 1;
			}

		}


		private void FormShipGroup_Resize(object sender, EventArgs e)
		{
			if (this._splitterDistance != -1 && this.splitContainer1.Height > 0)
			{
				try
				{
                    this.splitContainer1.SplitterDistance = this._splitterDistance;
                    this._splitterDistance = -1;

				}
				catch (Exception)
				{
					// *ぷちっ*
				}
			}
		}


	}
}
