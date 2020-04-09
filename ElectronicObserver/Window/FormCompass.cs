using ElectronicObserver.Data;
using ElectronicObserver.Data.Battle;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Dialog;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window
{

	public partial class FormCompass : DockContent
	{


		private class TableEnemyMemberControl : IDisposable
		{

			public ImageLabel ShipName;
			public ShipStatusEquipment Equipments;

			public FormCompass Parent;
			public ToolTip ToolTipInfo;


			public TableEnemyMemberControl(FormCompass parent)
			{

                #region Initialize

                this.Parent = parent;
                this.ToolTipInfo = parent.ToolTipInfo;


                this.ShipName = new ImageLabel
				{
					Anchor = AnchorStyles.Left,
					ForeColor = parent.MainFontColor,
					ImageAlign = ContentAlignment.MiddleCenter,
					Padding = new Padding(2, 2, 2, 2),
					Margin = new Padding(2, 0, 2, 1),
					AutoEllipsis = true,
					AutoSize = true,
					Cursor = Cursors.Help
				};
                this.ShipName.MouseClick += this.ShipName_MouseClick;

                this.Equipments = new ShipStatusEquipment();
                this.Equipments.SuspendLayout();
                this.Equipments.Anchor = AnchorStyles.Left;
                this.Equipments.Padding = new Padding(0, 1, 0, 2);
                this.Equipments.Margin = new Padding(2, 0, 2, 0);
                this.Equipments.AutoSize = true;
                this.Equipments.ResumeLayout();

                this.ConfigurationChanged();

				#endregion

			}


			public TableEnemyMemberControl(FormCompass parent, TableLayoutPanel table, int row)
				: this(parent)
			{

                this.AddToTable(table, row);
			}

			public void AddToTable(TableLayoutPanel table, int row)
			{

				table.Controls.Add(this.ShipName, 0, row);
				table.Controls.Add(this.Equipments, 1, row);

			}


			public void Update(int shipID)
			{
				var slot = shipID != -1 ? KCDatabase.Instance.MasterShips[shipID].DefaultSlot : null;
                this.Update(shipID, slot?.ToArray());
			}


			public void Update(int shipID, int[] slot)
			{

                this.ShipName.Tag = shipID;

				if (shipID == -1)
				{
                    //なし
                    this.ShipName.Text = "-";
                    this.ShipName.ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
                    this.Equipments.Visible = false;
                    this.ToolTipInfo.SetToolTip(this.ShipName, null);
                    this.ToolTipInfo.SetToolTip(this.Equipments, null);

				}
				else
				{

					ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];


                    this.ShipName.Text = ship.Name;
                    this.ShipName.ForeColor = ship.GetShipNameColor();
                    this.ToolTipInfo.SetToolTip(this.ShipName, GetShipString(shipID, slot));

                    this.Equipments.SetSlotList(shipID, slot);
                    this.Equipments.Visible = true;
                    this.ToolTipInfo.SetToolTip(this.Equipments, GetEquipmentString(shipID, slot));
				}

			}

			public void UpdateEquipmentToolTip(int shipID, int[] slot, int level, int hp, int firepower, int torpedo, int aa, int armor)
			{

                this.ToolTipInfo.SetToolTip(this.ShipName, GetShipString(shipID, slot, level, hp, firepower, torpedo, aa, armor));
			}


			void ShipName_MouseClick(object sender, MouseEventArgs e)
			{

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				{
					int shipID = this.ShipName.Tag as int? ?? -1;

					if (shipID != -1)
						new DialogAlbumMasterShip(shipID).Show(this.Parent);
				}

			}


			public void ConfigurationChanged()
			{
                this.ShipName.Font = this.Parent.MainFont;
                this.Equipments.Font = this.Parent.SubFont;

                this.ShipName.MaximumSize = new Size(Utility.Configuration.Config.FormCompass.MaxShipNameWidth, int.MaxValue);
			}

			public void Dispose()
			{
                this.ShipName.Dispose();
                this.Equipments.Dispose();
			}
		}


		private class TableEnemyCandidateControl
		{

			public ImageLabel[] ShipNames;
			public ImageLabel Formation;
			public ImageLabel AirSuperiority;

			public FormCompass Parent;
			public ToolTip ToolTipInfo;


			public TableEnemyCandidateControl(FormCompass parent)
			{

                #region Initialize

                this.Parent = parent;
                this.ToolTipInfo = parent.ToolTipInfo;


                this.ShipNames = new ImageLabel[6];
				for (int i = 0; i < this.ShipNames.Length; i++)
				{
                    this.ShipNames[i] = this.InitializeImageLabel();
                    this.ShipNames[i].Cursor = Cursors.Help;
                    this.ShipNames[i].MouseClick += this.TableEnemyCandidateControl_MouseClick;
				}

                this.Formation = this.InitializeImageLabel();
                this.Formation.Anchor = AnchorStyles.None;
                /*
				Formation.ImageAlign = ContentAlignment.MiddleLeft;
				Formation.ImageList = ResourceManager.Instance.Icons;
				Formation.ImageIndex = -1;
				*/

                this.AirSuperiority = this.InitializeImageLabel();
                this.AirSuperiority.Anchor = AnchorStyles.Right;
                this.AirSuperiority.ImageAlign = ContentAlignment.MiddleLeft;
                this.AirSuperiority.ImageList = ResourceManager.Instance.Equipments;
                this.AirSuperiority.ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter;


                this.ConfigurationChanged();

				#endregion

			}

			private ImageLabel InitializeImageLabel()
			{
				var label = new ImageLabel
				{
					Anchor = AnchorStyles.Left,
					ForeColor = this.Parent.MainFontColor,
					ImageAlign = ContentAlignment.MiddleCenter,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(4, 0, 4, 1),
					AutoEllipsis = true,
					AutoSize = true
				};

				return label;
			}



			public TableEnemyCandidateControl(FormCompass parent, TableLayoutPanel table, int column)
				: this(parent)
			{

                this.AddToTable(table, column);
			}

			public void AddToTable(TableLayoutPanel table, int column)
			{

				table.ColumnCount = Math.Max(table.ColumnCount, column + 1);
				table.RowCount = Math.Max(table.RowCount, 8);

				for (int i = 0; i < 6; i++)
					table.Controls.Add(this.ShipNames[i], column, i);
				table.Controls.Add(this.Formation, column, 6);
				table.Controls.Add(this.AirSuperiority, column, 7);

			}


			public void ConfigurationChanged()
			{
				for (int i = 0; i < this.ShipNames.Length; i++)
                    this.ShipNames[i].Font = this.Parent.MainFont;
                this.Formation.Font = this.AirSuperiority.Font = this.Parent.MainFont;

				var maxSize = new Size(Utility.Configuration.Config.FormCompass.MaxShipNameWidth, int.MaxValue);
				foreach (var label in this.ShipNames)
					label.MaximumSize = maxSize;
                this.Formation.MaximumSize = maxSize;
                this.AirSuperiority.MaximumSize = maxSize;
			}

			public void Update(EnemyFleetRecord.EnemyFleetElement fleet)
			{

				if (fleet == null)
				{
					for (int i = 0; i < 6; i++)
                        this.ShipNames[i].Visible = false;
                    this.Formation.Visible = false;
                    this.AirSuperiority.Visible = false;
                    this.ToolTipInfo.SetToolTip(this.AirSuperiority, null);

					return;
				}

				for (int i = 0; i < 6; i++)
				{

					var ship = KCDatabase.Instance.MasterShips[fleet.FleetMember[i]];

					// カッコカリ 上のとマージするといいかもしれない

					if (ship == null)
					{
                        // nothing
                        this.ShipNames[i].Text = "-";
                        this.ShipNames[i].ForeColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BlackFontColor);
                        this.ShipNames[i].Tag = -1;
                        this.ShipNames[i].Cursor = Cursors.Default;
                        this.ToolTipInfo.SetToolTip(this.ShipNames[i], null);

					}
					else
					{

                        this.ShipNames[i].Text = ship.Name;
                        this.ShipNames[i].ForeColor = ship.GetShipNameColor();
                        this.ShipNames[i].Tag = ship.ShipID;
                        this.ShipNames[i].Cursor = Cursors.Help;
                        this.ToolTipInfo.SetToolTip(this.ShipNames[i], GetShipString(ship.ShipID, ship.DefaultSlot?.ToArray()));
					}

                    this.ShipNames[i].Visible = true;

				}

                this.Formation.Text = Constants.GetFormationShort(fleet.Formation);
                //Formation.ImageIndex = (int)ResourceManager.IconContent.BattleFormationEnemyLineAhead + fleet.Formation - 1;
                this.Formation.Visible = true;

				{
					int air = Calculator.GetAirSuperiority(fleet.FleetMember);
                    this.AirSuperiority.Text = air.ToString();
                    this.ToolTipInfo.SetToolTip(this.AirSuperiority, GetAirSuperiorityString(air));
                    this.AirSuperiority.Visible = true;
				}

			}


			void TableEnemyCandidateControl_MouseClick(object sender, MouseEventArgs e)
			{

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				{
					int shipID = ((ImageLabel)sender).Tag as int? ?? -1;

					if (shipID != -1)
						new DialogAlbumMasterShip(shipID).Show(this.Parent);
				}
			}

		}



		#region ***Control method

		private static string GetShipString(int shipID, int[] slot)
		{

			ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];
			if (ship == null) return null;

			return GetShipString(shipID, slot, -1, ship.HPMin, ship.FirepowerMax, ship.TorpedoMax, ship.AAMax, ship.ArmorMax,
				 ship.ASW != null && !ship.ASW.IsMaximumDefault ? ship.ASW.Maximum : -1,
				 ship.Evasion != null && !ship.Evasion.IsMaximumDefault ? ship.Evasion.Maximum : -1,
				 ship.LOS != null && !ship.LOS.IsMaximumDefault ? ship.LOS.Maximum : -1,
				 ship.LuckMin);
		}

		private static string GetShipString(int shipID, int[] slot, int level, int hp, int firepower, int torpedo, int aa, int armor)
		{
			ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];
			if (ship == null) return null;

			return GetShipString(shipID, slot, level, hp, firepower, torpedo, aa, armor,
				ship.ASW != null && ship.ASW.IsAvailable ? ship.ASW.GetParameter(level) : -1,
				ship.Evasion != null && ship.Evasion.IsAvailable ? ship.Evasion.GetParameter(level) : -1,
				ship.LOS != null && ship.LOS.IsAvailable ? ship.LOS.GetParameter(level) : -1,
				level > 99 ? Math.Min(ship.LuckMin + 3, ship.LuckMax) : ship.LuckMin);
		}

		private static string GetShipString(int shipID, int[] slot, int level, int hp, int firepower, int torpedo, int aa, int armor, int asw, int evasion, int los, int luck)
		{

			ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];
			if (ship == null) return null;

			int firepower_c = firepower;
			int torpedo_c = torpedo;
			int aa_c = aa;
			int armor_c = armor;
			int asw_c = asw;
			int evasion_c = evasion;
			int los_c = los;
			int luck_c = luck;
			int range = ship.Range;

			asw = Math.Max(asw, 0);
			evasion = Math.Max(evasion, 0);
			los = Math.Max(los, 0);

			if (slot != null)
			{
				int count = slot.Length;
				for (int i = 0; i < count; i++)
				{
					EquipmentDataMaster eq = KCDatabase.Instance.MasterEquipments[slot[i]];
					if (eq == null) continue;

					firepower += eq.Firepower;
					torpedo += eq.Torpedo;
					aa += eq.AA;
					armor += eq.Armor;
					asw += eq.ASW;
					evasion += eq.Evasion;
					los += eq.LOS;
					luck += eq.Luck;
					range = Math.Max(range, eq.Range);
				}
			}


			var sb = new StringBuilder();

			sb.Append(ship.ShipTypeName).Append(" ").AppendLine(ship.NameWithClass);
			if (level > 0)
				sb.Append("Lv. ").Append(level.ToString());
			sb.Append(" (ID: ").Append(shipID).AppendLine(")");

			sb.Append("내구: ").Append(hp).AppendLine();

			sb.Append("화력: ").Append(firepower_c);
			if (firepower_c != firepower)
				sb.Append("/").Append(firepower);
			sb.AppendLine();

			sb.Append("뇌장: ").Append(torpedo_c);
			if (torpedo_c != torpedo)
				sb.Append("/").Append(torpedo);
			sb.AppendLine();

			sb.Append("대공: ").Append(aa_c);
			if (aa_c != aa)
				sb.Append("/").Append(aa);
			sb.AppendLine();

			sb.Append("장갑: ").Append(armor_c);
			if (armor_c != armor)
				sb.Append("/").Append(armor);
			sb.AppendLine();

			sb.Append("대잠: ");
			if (asw_c < 0) sb.Append("???");
			else sb.Append(asw_c);
			if (asw_c != asw)
				sb.Append("/").Append(asw);
			sb.AppendLine();

			sb.Append("회피: ");
			if (evasion_c < 0) sb.Append("???");
			else sb.Append(evasion_c);
			if (evasion_c != evasion)
				sb.Append("/").Append(evasion);
			sb.AppendLine();

			sb.Append("색적: ");
			if (los_c < 0) sb.Append("???");
			else sb.Append(los_c);
			if (los_c != los)
				sb.Append("/").Append(los);
			sb.AppendLine();

			sb.Append("운: ").Append(luck_c);
			if (luck_c != luck)
				sb.Append("/").Append(luck);
			sb.AppendLine();

			sb.AppendFormat("사정: {0} / 속력: {1}\r\n(우클릭으로 도감에)\r\n",
				Constants.GetRange(range),
				Constants.GetSpeed(ship.Speed));

			return sb.ToString();

		}

		private static string GetEquipmentString(int shipID, int[] slot)
		{
			StringBuilder sb = new StringBuilder();
			ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];

			if (ship == null || slot == null) return null;

			for (int i = 0; i < slot.Length; i++)
			{
				var eq = KCDatabase.Instance.MasterEquipments[slot[i]];
				if (eq != null)
					sb.AppendFormat("[{0}] {1}\r\n", ship.Aircraft[i], eq.Name);
			}

			sb.AppendFormat("\r\n주간전: {0}\r\n야간전: {1}\r\n",
				Constants.GetDayAttackKind(Calculator.GetDayAttackKind(slot, ship.ShipID, -1)),
				Constants.GetNightAttackKind(Calculator.GetNightAttackKind(slot, ship.ShipID, -1)));

			{
				int aacutin = Calculator.GetAACutinKind(shipID, slot);
				if (aacutin != 0)
				{
					sb.AppendFormat("대공: {0}\r\n", Constants.GetAACutinKind(aacutin));
				}
			}
			{
				int airsup = Calculator.GetAirSuperiority(slot, ship.Aircraft.ToArray());
				if (airsup > 0)
				{
					sb.AppendFormat("제공전력: {0}\r\n", airsup);
				}
			}

			return sb.ToString();
		}

		private static string GetAirSuperiorityString(int air)
		{
			if (air > 0)
			{
				return string.Format("확보: {0}\r\n우세: {1}\r\n균등: {2}\r\n열세: {3}\r\n",
							(int)(air * 3.0),
							(int)Math.Ceiling(air * 1.5),
							(int)(air / 1.5 + 1),
							(int)(air / 3.0 + 1));
			}
			return null;
		}

		#endregion




		public Font MainFont { get; set; }
		public Font SubFont { get; set; }
		public Color MainFontColor { get; set; }
		public Color SubFontColor { get; set; }


		private TableEnemyMemberControl[] ControlMembers;
		private TableEnemyCandidateControl[] ControlCandidates;

		private int _candidatesDisplayCount;


		/// <summary>
		/// 次に遭遇する敵艦隊候補
		/// </summary>
		private List<EnemyFleetRecord.EnemyFleetElement> _enemyFleetCandidate = null;

		/// <summary>
		/// 表示中の敵艦隊候補のインデックス
		/// </summary>
		private int _enemyFleetCandidateIndex = 0;




		public FormCompass(FormMain parent)
		{
            this.InitializeComponent();



            this.MainFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.SubFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.SubFontColor);


            ControlHelper.SetDoubleBuffered(this.BasePanel);
			ControlHelper.SetDoubleBuffered(this.TableEnemyFleet);
			ControlHelper.SetDoubleBuffered(this.TableEnemyMember);


            this.TableEnemyMember.SuspendLayout();
            this.ControlMembers = new TableEnemyMemberControl[6];
			for (int i = 0; i < this.ControlMembers.Length; i++)
			{
                this.ControlMembers[i] = new TableEnemyMemberControl(this, this.TableEnemyMember, i);
			}
            this.TableEnemyMember.ResumeLayout();

            this.TableEnemyCandidate.SuspendLayout();
            this.ControlCandidates = new TableEnemyCandidateControl[6];
			for (int i = 0; i < this.ControlCandidates.Length; i++)
			{
                this.ControlCandidates[i] = new TableEnemyCandidateControl(this, this.TableEnemyCandidate, i);
			}
            this.TableEnemyCandidate.ResumeLayout();


            //BasePanel.SetFlowBreak( TextMapArea, true );
            this.BasePanel.SetFlowBreak(this.TextDestination, true);
            //BasePanel.SetFlowBreak( TextEventKind, true );
            this.BasePanel.SetFlowBreak(this.TextEventDetail, true);


            this.TextDestination.ImageList = ResourceManager.Instance.Equipments;
            this.TextEventKind.ImageList = ResourceManager.Instance.Equipments;
            this.TextEventDetail.ImageList = ResourceManager.Instance.Equipments;
            this.TextFormation.ImageList = ResourceManager.Instance.Icons;
            this.TextAirSuperiority.ImageList = ResourceManager.Instance.Equipments;
            this.TextAirSuperiority.ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter;



            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormCompass]);

		}


		private void FormCompass_Load(object sender, EventArgs e)
		{

            this.BasePanel.Visible = false;


			APIObserver o = APIObserver.Instance;

			o["api_port/port"].ResponseReceived += this.Updated;
			o["api_req_map/start"].ResponseReceived += this.Updated;
			o["api_req_map/next"].ResponseReceived += this.Updated;
			o["api_req_member/get_practice_enemyinfo"].ResponseReceived += this.Updated;

			o["api_req_sortie/battle"].ResponseReceived += this.BattleStarted;
			o["api_req_battle_midnight/sp_midnight"].ResponseReceived += this.BattleStarted;
			o["api_req_sortie/night_to_day"].ResponseReceived += this.BattleStarted;
			o["api_req_sortie/airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_sortie/ld_airbattle"].ResponseReceived += this.BattleStarted;
            o["api_req_sortie/ld_shooting"].ResponseReceived += this.BattleStarted;
            o["api_req_combined_battle/battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/sp_midnight"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/battle_water"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ld_airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ec_battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/each_battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/each_battle_water"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ec_night_to_day"].ResponseReceived += this.BattleStarted;
            o["api_req_combined_battle/ld_shooting"].ResponseReceived += this.BattleStarted;
            o["api_req_practice/battle"].ResponseReceived += this.BattleStarted;


			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
		}


		private void Updated(string apiname, dynamic data)
		{

            Color getColorFromEventKind(int kind)
            {
				switch (kind)
				{
					case 0:
					case 1:
					default:    //昼夜戦・その他
                        return Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
                    case 2:
					case 3:     //夜戦・夜昼戦
						return Color.Navy;
					case 4:     //航空戦
					case 6:     //長距離空襲戦
						return Color.DarkGreen;
					case 5:     // 敵連合
						return Color.DarkRed;
					case 7:     // 夜昼戦(対連合艦隊)
						return Color.Navy;
                    case 8:     // 레이더사격
                        return Color.Navy;
                }
			};

			if (apiname == "api_port/port")
			{

                this.BasePanel.Visible = false;

			}
			else if (apiname == "api_req_member/get_practice_enemyinfo")
			{

                this.TextMapArea.Text = "연습";
                this.TextDestination.Text = string.Format("{0} {1}", data.api_nickname, Constants.GetAdmiralRank((int)data.api_rank));
                this.TextDestination.ImageAlign = ContentAlignment.MiddleCenter;
                this.TextDestination.ImageIndex = -1;
                this.ToolTipInfo.SetToolTip(this.TextDestination, null);
                this.TextEventKind.Text = data.api_cmt;

                this.TextEventKind.ForeColor = getColorFromEventKind(0);
                this.TextEventKind.ImageAlign = ContentAlignment.MiddleCenter;
                this.TextEventKind.ImageIndex = -1;
                this.ToolTipInfo.SetToolTip(this.TextEventKind, null);
                this.TextEventDetail.Text = string.Format("Lv. {0} / {1} exp.", data.api_level, data.api_experience[0]);
                this.TextEventDetail.ImageAlign = ContentAlignment.MiddleCenter;
                this.TextEventDetail.ImageIndex = -1;
                this.ToolTipInfo.SetToolTip(this.TextEventDetail, null);
                this.TextEnemyFleetName.Text = data.api_deckname;

			}
			else
			{

				CompassData compass = KCDatabase.Instance.Battle.Compass;


                this.BasePanel.SuspendLayout();
                this.PanelEnemyFleet.Visible = false;
                this.PanelEnemyCandidate.Visible = false;

                this._enemyFleetCandidate = null;
                this._enemyFleetCandidateIndex = -1;


                this.TextMapArea.Text = string.Format("출격해역 : {0}-{1}{2}", compass.MapAreaID, compass.MapInfoID,
					compass.MapInfo.EventDifficulty > 0 ? " [" + Constants.GetDifficulty(compass.MapInfo.EventDifficulty) + "]" : "");
				{
					var mapinfo = compass.MapInfo;

					if (mapinfo.IsCleared)
					{
                        this.ToolTipInfo.SetToolTip(this.TextMapArea, null);

					}
                    if (mapinfo.RequiredDefeatedCount != -1 && mapinfo.CurrentDefeatedCount < mapinfo.RequiredDefeatedCount)
                    {
                        this.ToolTipInfo.SetToolTip(this.TextMapArea, string.Format("격파: {0} / {1} 회",
                            mapinfo.CurrentGaugeIndex > 0 ? $"#{mapinfo.CurrentGaugeIndex} " : "",
                            mapinfo.CurrentDefeatedCount, mapinfo.RequiredDefeatedCount));

                    }
					else if (mapinfo.MapHPMax > 0)
					{
                        int current = compass.MapHPCurrent > 0 ? compass.MapHPCurrent : mapinfo.MapHPCurrent;
						int max = compass.MapHPMax > 0 ? compass.MapHPMax : mapinfo.MapHPMax;

                        this.ToolTipInfo.SetToolTip(this.TextMapArea, string.Format("{0}{1}: {2} / {3}",
                            mapinfo.CurrentGaugeIndex > 0 ? $"#{mapinfo.CurrentGaugeIndex} " : "",
                            mapinfo.GaugeType == 3 ? "TP" : "HP", current, max));

                    }
					else
					{
                        this.ToolTipInfo.SetToolTip(this.TextMapArea, null);
					}
				}

                this.TextDestination.Text = string.Format("다음노드 : {0}{1}", compass.Destination_Name, (compass.IsEndPoint ? " (마지막)" : ""));
				if (compass.LaunchedRecon != 0)
				{
                    this.TextDestination.ImageAlign = ContentAlignment.MiddleRight;
                    this.TextDestination.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;

					string tiptext;
					switch (compass.CommentID)
					{
						case 1:
							tiptext = "적함대발견！";
							break;
						case 2:
							tiptext = "공격목표발견！";
							break;
						case 3:
							tiptext = "진로초계！";
							break;
						default:
							tiptext = "색적기발함！";
							break;
					}
                    this.ToolTipInfo.SetToolTip(this.TextDestination, tiptext);

				}
				else
				{
                    this.TextDestination.ImageAlign = ContentAlignment.MiddleCenter;
                    this.TextDestination.ImageIndex = -1;
                    this.ToolTipInfo.SetToolTip(this.TextDestination, null);
				}

                //とりあえずリセット
                this.TextEventDetail.ImageAlign = ContentAlignment.MiddleCenter;
                this.TextEventDetail.ImageIndex = -1;
                this.ToolTipInfo.SetToolTip(this.TextEventDetail, null);


                this.TextEventKind.ForeColor = getColorFromEventKind(0);

				{
					string eventkind = Constants.GetMapEventID(compass.EventID);

					switch (compass.EventID)
					{

						case 0:     //初期位置
                            this.TextEventDetail.Text = "어째서 이렇게 됬냐";
							break;

						case 2:     //資源
						case 8:     //船団護衛成功
                            this.TextEventDetail.Text = this.GetMaterialInfo(compass);
							break;

						case 3:     //渦潮
							{
								int materialmax = KCDatabase.Instance.Fleet.Fleets.Values
									.Where(f => f != null && f.IsInSortie)
									.SelectMany(f => f.MembersWithoutEscaped)
									.Max(s =>
									{
										if (s == null) return 0;
										switch (compass.WhirlpoolItemID)
										{
											case 1:
												return s.Fuel;
											case 2:
												return s.Ammo;
											default:
												return 0;
										}
									});

                                this.TextEventDetail.Text = string.Format("{0} x {1} ({2:p0})",
									Constants.GetMaterialName(compass.WhirlpoolItemID),
									compass.WhirlpoolItemAmount,
									(double)compass.WhirlpoolItemAmount / Math.Max(materialmax, 1));

							}
							break;

						case 4:     //通常戦闘
							if (compass.EventKind >= 2)
							{
								eventkind += "/" + Constants.GetMapEventKind(compass.EventKind);

                                this.TextEventKind.ForeColor = getColorFromEventKind(compass.EventKind);
							}
                            this.UpdateEnemyFleet();
							break;

						case 5:     //ボス戦闘
                            this.TextEventKind.ForeColor = Color.Red;

							if (compass.EventKind >= 2)
							{
								eventkind += "/" + Constants.GetMapEventKind(compass.EventKind);
							}
                            this.UpdateEnemyFleet();
							break;

						case 1:     //イベントなし
						case 6:     //気のせいだった
							switch (compass.EventKind)
							{

								case 0:     //気のせいだった
								default:
									break;
								case 1:
									eventkind = "적 발견 못함";
									break;
								case 2:
									eventkind = "능동분기";
									break;
								case 3:
									eventkind = "잔잔한 바다다.";
									break;
								case 4:
									eventkind = "온화한 해협이다.";
									break;
								case 5:
									eventkind = "경계 필요 구역";
									break;
								case 6:
									eventkind = "조용한 바다다.";
									break;
							}
                            if (compass.RouteChoices != null)
                            {
                                this.TextEventDetail.Text = string.Join(" / ", compass.RouteChoices);
                            }
                            else if (compass.FlavorTextType != -1)
                            {
                                this.TextEventDetail.Text = "◆";
                                this.ToolTipInfo.SetToolTip(this.TextEventDetail, compass.FlavorText);
                            }
                            else
                            {
                                this.TextEventDetail.Text = "";
                            }

							break;

						case 7:     //航空戦or航空偵察
                            this.TextEventKind.ForeColor = getColorFromEventKind(compass.EventKind);

							switch (compass.EventKind)
							{
								case 0:     //航空偵察
									eventkind = "항공정찰";

									switch (compass.AirReconnaissanceResult)
									{
										case 0:
										default:
                                            this.TextEventDetail.Text = "실패";
											break;
										case 1:
                                            this.TextEventDetail.Text = "성공";
											break;
										case 2:
                                            this.TextEventDetail.Text = "대성공";
											break;
									}

									switch (compass.AirReconnaissancePlane)
									{
										case 0:
										default:
                                            this.TextEventDetail.ImageAlign = ContentAlignment.MiddleCenter;
                                            this.TextEventDetail.ImageIndex = -1;
											break;
										case 1:
                                            this.TextEventDetail.ImageAlign = ContentAlignment.MiddleLeft;
                                            this.TextEventDetail.ImageIndex = (int)ResourceManager.EquipmentContent.FlyingBoat;
											break;
										case 2:
                                            this.TextEventDetail.ImageAlign = ContentAlignment.MiddleLeft;
                                            this.TextEventDetail.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;
											break;
									}

									if (compass.GetItems.Any())
									{
                                        this.TextEventDetail.Text += "　" + this.GetMaterialInfo(compass);
									}

									break;

								case 4:     //航空戦
								default:
                                    this.UpdateEnemyFleet();
									break;
							}
							break;

						case 9:     //揚陸地点
                            this.TextEventDetail.Text = "";
							break;

						default:
                            this.TextEventDetail.Text = "";
							break;

					}
                    this.TextEventKind.Text = eventkind;
				}


				if (compass.HasAirRaid)
				{
                    this.TextEventKind.ImageAlign = ContentAlignment.MiddleRight;
                    this.TextEventKind.ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedBomber;
                    this.ToolTipInfo.SetToolTip(this.TextEventKind, "공습 - " + Constants.GetAirRaidDamage(compass.AirRaidDamageKind));
				}
				else
				{
                    this.TextEventKind.ImageAlign = ContentAlignment.MiddleCenter;
                    this.TextEventKind.ImageIndex = -1;
                    this.ToolTipInfo.SetToolTip(this.TextEventKind, null);
				}


                this.BasePanel.ResumeLayout();

                this.BasePanel.Visible = true;
			}


		}


		private string GetMaterialInfo(CompassData compass)
		{

			var strs = new LinkedList<string>();

			foreach (var item in compass.GetItems)
			{

				string itemName;

				if (item.ItemID == 4)
				{
					itemName = Constants.GetMaterialName(item.Metadata);

				}
				else
				{
					var itemMaster = KCDatabase.Instance.MasterUseItems[item.Metadata];
					if (itemMaster != null)
						itemName = itemMaster.Name;
					else
						itemName = "알수없는아이템";
				}

				strs.AddLast(itemName + " x " + item.Amount);
			}

			if (!strs.Any())
			{
				return "(없음)";

			}
			else
			{
				return string.Join(", ", strs);
			}
		}



		private void BattleStarted(string apiname, dynamic data)
		{
            this.UpdateEnemyFleetInstant(apiname.Contains("practice"));
		}





		private void UpdateEnemyFleet()
		{

			CompassData compass = KCDatabase.Instance.Battle.Compass;

            this._enemyFleetCandidate = RecordManager.Instance.EnemyFleet.Record.Values.Where(
				r =>
					r.MapAreaID == compass.MapAreaID &&
					r.MapInfoID == compass.MapInfoID &&
					r.CellID == compass.Destination &&
					r.Difficulty == compass.MapInfo.EventDifficulty
				).ToList();
            this._enemyFleetCandidateIndex = 0;


			if (this._enemyFleetCandidate.Count == 0)
			{
                this.TextEventDetail.Text = "(적함대없음)";
                this.TextEnemyFleetName.Text = "(적함대명불명)";


                this.TableEnemyCandidate.Visible = false;

			}
			else
			{
                this._enemyFleetCandidate.Sort((a, b) =>
				{
					for (int i = 0; i < a.FleetMember.Length; i++)
					{
						int diff = a.FleetMember[i] - b.FleetMember[i];
						if (diff != 0)
							return diff;
					}
					return a.Formation - b.Formation;
				});

                this.NextEnemyFleetCandidate(0);
			}


            this.PanelEnemyFleet.Visible = false;

		}


		private void UpdateEnemyFleetInstant(bool isPractice = false)
		{

			BattleManager bm = KCDatabase.Instance.Battle;
			BattleData bd = bm.FirstBattle;

			int[] enemies = bd.Initial.EnemyMembers;
			int[][] slots = bd.Initial.EnemySlots;
			int[] levels = bd.Initial.EnemyLevels;
			int[][] parameters = bd.Initial.EnemyParameters;
			int[] hps = bd.Initial.EnemyMaxHPs;


            this._enemyFleetCandidate = null;
            this._enemyFleetCandidateIndex = -1;



			if (!bm.IsPractice)
			{
				var efcurrent = EnemyFleetRecord.EnemyFleetElement.CreateFromCurrentState();
				var efrecord = RecordManager.Instance.EnemyFleet[efcurrent.FleetID];
				if (efrecord != null)
				{
                    this.TextEnemyFleetName.Text = efrecord.FleetName;
                    this.TextEventDetail.Text = "Exp: " + efrecord.ExpShip;
				}
                this.ToolTipInfo.SetToolTip(this.TextEventDetail, "적함대ID: " + efcurrent.FleetID.ToString("x16"));
			}

            this.TextFormation.Text = Constants.GetFormationShort((int)bd.Searching.FormationEnemy);
            //TextFormation.ImageIndex = (int)ResourceManager.IconContent.BattleFormationEnemyLineAhead + bd.Searching.FormationEnemy - 1;
            this.TextFormation.Visible = true;
			{
				int air = Calculator.GetAirSuperiority(enemies, slots);
                this.TextAirSuperiority.Text = isPractice ?
					air.ToString() + " ～ " + Calculator.GetAirSuperiorityAtMaxLevel(enemies, slots).ToString() :
					air.ToString();
                this.ToolTipInfo.SetToolTip(this.TextAirSuperiority, GetAirSuperiorityString(isPractice ? 0 : air));
                this.TextAirSuperiority.Visible = true;
			}

            this.TableEnemyMember.SuspendLayout();
			for (int i = 0; i < this.ControlMembers.Length; i++)
			{
				int shipID = enemies[i];
                this.ControlMembers[i].Update(shipID, shipID != -1 ? slots[i] : null);

				if (shipID != -1)
                    this.ControlMembers[i].UpdateEquipmentToolTip(shipID, slots[i], levels[i], hps[i], parameters[i][0], parameters[i][1], parameters[i][2], parameters[i][3]);
			}
            this.TableEnemyMember.ResumeLayout();
            this.TableEnemyMember.Visible = true;

            this.PanelEnemyFleet.Visible = true;

            this.PanelEnemyCandidate.Visible = false;

            this.BasePanel.Visible = true;           //checkme

		}



		private void TextEnemyFleetName_MouseDown(object sender, MouseEventArgs e)
		{

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this.NextEnemyFleetCandidate();
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                this.NextEnemyFleetCandidate(-this._candidatesDisplayCount);
		}


		private void NextEnemyFleetCandidate()
		{
            this.NextEnemyFleetCandidate(this._candidatesDisplayCount);
		}

		private void NextEnemyFleetCandidate(int offset)
		{

			if (this._enemyFleetCandidate != null && this._enemyFleetCandidate.Count != 0)
			{

                this._enemyFleetCandidateIndex += offset;
				if (this._enemyFleetCandidateIndex < 0)
                    this._enemyFleetCandidateIndex = (this._enemyFleetCandidate.Count - 1) - (this._enemyFleetCandidate.Count - 1) % this._candidatesDisplayCount;
				else if (this._enemyFleetCandidateIndex >= this._enemyFleetCandidate.Count)
                    this._enemyFleetCandidateIndex = 0;


				var candidate = this._enemyFleetCandidate[this._enemyFleetCandidateIndex];


                this.TextEventDetail.Text = this.TextEnemyFleetName.Text = candidate.FleetName;

				if (this._enemyFleetCandidate.Count > this._candidatesDisplayCount)
				{
                    this.TextEventDetail.Text += " ▼";
                    this.ToolTipInfo.SetToolTip(this.TextEventDetail, string.Format("편성수: {0} / {1}\r\n(좌우 클릭으로 페이지 넘김)\r\n", this._enemyFleetCandidateIndex + 1, this._enemyFleetCandidate.Count));
				}
				else
				{
                    this.ToolTipInfo.SetToolTip(this.TextEventDetail, string.Format("편성수: {0}\r\n", this._enemyFleetCandidate.Count));
				}

                this.TableEnemyCandidate.SuspendLayout();
				for (int i = 0; i < this.ControlCandidates.Length; i++)
				{
					if (i + this._enemyFleetCandidateIndex >= this._enemyFleetCandidate.Count || i >= this._candidatesDisplayCount)
					{
                        this.ControlCandidates[i].Update(null);
						continue;
					}

                    this.ControlCandidates[i].Update(this._enemyFleetCandidate[i + this._enemyFleetCandidateIndex]);
				}
                this.TableEnemyCandidate.ResumeLayout();
                this.TableEnemyCandidate.Visible = true;

                this.PanelEnemyCandidate.Visible = true;

			}
		}


		void ConfigurationChanged()
		{

            this.Font = this.PanelEnemyFleet.Font = this.MainFont = Utility.Configuration.Config.UI.MainFont;
            this.SubFont = Utility.Configuration.Config.UI.SubFont;

            this.TextMapArea.Font =
            this.TextDestination.Font =
            this.TextEventKind.Font =
            this.TextEventDetail.Font = this.Font;

            this.BasePanel.AutoScroll = Utility.Configuration.Config.FormCompass.IsScrollable;

            this._candidatesDisplayCount = Utility.Configuration.Config.FormCompass.CandidateDisplayCount;
            this._enemyFleetCandidateIndex = 0;
			if (this.PanelEnemyCandidate.Visible)
                this.NextEnemyFleetCandidate(0);

			if (this.ControlMembers != null)
			{
                this.TableEnemyMember.SuspendLayout();

                this.TableEnemyMember.Location = new Point(this.TableEnemyMember.Location.X, this.TableEnemyFleet.Bottom + 6);

				bool flag = Utility.Configuration.Config.FormFleet.ShowAircraft;
				for (int i = 0; i < this.ControlMembers.Length; i++)
				{
                    this.ControlMembers[i].Equipments.ShowAircraft = flag;
                    this.ControlMembers[i].ConfigurationChanged();
				}

				ControlHelper.SetTableRowStyles(this.TableEnemyMember, ControlHelper.GetDefaultRowStyle());
                this.TableEnemyMember.ResumeLayout();
			}

			if (this.ControlCandidates != null)
			{
                this.TableEnemyCandidate.SuspendLayout();

				for (int i = 0; i < this.ControlCandidates.Length; i++)
                    this.ControlCandidates[i].ConfigurationChanged();

				ControlHelper.SetTableRowStyles(this.TableEnemyCandidate, new RowStyle(SizeType.AutoSize));
				ControlHelper.SetTableColumnStyles(this.TableEnemyCandidate, ControlHelper.GetDefaultColumnStyle());
                this.TableEnemyCandidate.ResumeLayout();
			}

            this.ForeColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);
            this.MainFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.SubFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.SubFontColor);
        }



		protected override string GetPersistString()
		{
			return "Compass";
		}

		private void TableEnemyMember_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		private void TableEnemyCandidateMember_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{

			if (this._enemyFleetCandidate == null || this._enemyFleetCandidateIndex + e.Column >= this._enemyFleetCandidate.Count)
				return;


			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

			if (e.Row == 5 || e.Row == 7)
			{
				e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
			}
		}

	}

}
