using ElectronicObserver.Data;
using ElectronicObserver.Data.Battle;
using ElectronicObserver.Data.Battle.Detail;
using ElectronicObserver.Data.Battle.Phase;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Window.Control;
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

	public partial class FormBattle : DockContent
	{
        private Color WinRankColor_Win = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
        private readonly Color WinRankColor_Lose = Color.Red;

		private readonly Size DefaultBarSize = new Size(80, 20);
		private readonly Size SmallBarSize = new Size(60, 20);

		private List<ShipStatusHP> HPBars;

		public Font MainFont { get; set; }
		public Font SubFont { get; set; }



		public FormBattle(FormMain parent)
		{
            this.InitializeComponent();

			ControlHelper.SetDoubleBuffered(this.TableTop);
			ControlHelper.SetDoubleBuffered(this.TableBottom);


            this.HPBars = new List<ShipStatusHP>(24);


            this.TableBottom.SuspendLayout();
			for (int i = 0; i < 24; i++)
			{
                this.HPBars.Add(new ShipStatusHP());
                this.HPBars[i].Size = this.DefaultBarSize;
                this.HPBars[i].AutoSize = false;
                this.HPBars[i].AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.HPBars[i].Margin = new Padding(2, 0, 2, 0);
                this.HPBars[i].Anchor = AnchorStyles.Left | AnchorStyles.Right;
                this.HPBars[i].MainFont = this.MainFont;
                this.HPBars[i].ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                this.HPBars[i].SubFont = this.SubFont;
                this.HPBars[i].SubFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.SubFontColor);
                this.HPBars[i].UsePrevValue = true;
                this.HPBars[i].ShowDifference = true;
                this.HPBars[i].MaximumDigit = 9999;

				if (i < 6)
				{
                    this.TableBottom.Controls.Add(this.HPBars[i], 0, i + 1);
				}
				else if (i < 12)
				{
                    this.TableBottom.Controls.Add(this.HPBars[i], 1, i - 5);
				}
				else if (i < 18)
				{
                    this.TableBottom.Controls.Add(this.HPBars[i], 3, i - 11);
				}
				else
				{
                    this.TableBottom.Controls.Add(this.HPBars[i], 2, i - 17);
				}
			}
            this.TableBottom.ResumeLayout();


            this.Searching.ImageList =
            this.SearchingFriend.ImageList =
            this.SearchingEnemy.ImageList =
            this.AACutin.ImageList =
            this.AirStage1Friend.ImageList =
            this.AirStage1Enemy.ImageList =
            this.AirStage2Friend.ImageList =
            this.AirStage2Enemy.ImageList =
            this.FleetFriend.ImageList =
				ResourceManager.Instance.Equipments;


            this.ConfigurationChanged();

            this.BaseLayoutPanel.Visible = false;


            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBattle]);

		}



		private void FormBattle_Load(object sender, EventArgs e)
		{

			APIObserver o = APIObserver.Instance;

			o["api_port/port"].ResponseReceived += this.Updated;
			o["api_req_map/start"].ResponseReceived += this.Updated;
			o["api_req_map/next"].ResponseReceived += this.Updated;
			o["api_req_sortie/battle"].ResponseReceived += this.Updated;
			o["api_req_sortie/battleresult"].ResponseReceived += this.Updated;
			o["api_req_battle_midnight/battle"].ResponseReceived += this.Updated;
			o["api_req_battle_midnight/sp_midnight"].ResponseReceived += this.Updated;
			o["api_req_sortie/airbattle"].ResponseReceived += this.Updated;
			o["api_req_sortie/ld_airbattle"].ResponseReceived += this.Updated;
			o["api_req_sortie/night_to_day"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/battle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/midnight_battle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/sp_midnight"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/airbattle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/battle_water"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/ld_airbattle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/ec_battle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/ec_midnight_battle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/ec_night_to_day"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/each_battle"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/each_battle_water"].ResponseReceived += this.Updated;
			o["api_req_combined_battle/battleresult"].ResponseReceived += this.Updated;
			o["api_req_practice/battle"].ResponseReceived += this.Updated;
			o["api_req_practice/midnight_battle"].ResponseReceived += this.Updated;
			o["api_req_practice/battle_result"].ResponseReceived += this.Updated;
            o["api_req_sortie/ld_shooting"].ResponseReceived += this.Updated;
            o["api_req_combined_battle/ld_shooting"].ResponseReceived += this.Updated;

            Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

		}


		private void Updated(string apiname, dynamic data)
		{

			KCDatabase db = KCDatabase.Instance;
			BattleManager bm = db.Battle;
			bool hideDuringBattle = Utility.Configuration.Config.FormBattle.HideDuringBattle;

            this.BaseLayoutPanel.SuspendLayout();
            this.TableTop.SuspendLayout();
            this.TableBottom.SuspendLayout();
			switch (apiname)
			{

				case "api_port/port":
                    this.BaseLayoutPanel.Visible = false;
                    this.ToolTipInfo.RemoveAll();
					break;

				case "api_req_map/start":
				case "api_req_map/next":
					if (!bm.Compass.HasAirRaid)
						goto case "api_port/port";

                    this.SetFormation(bm);
                    this.ClearSearchingResult();
                    this.ClearBaseAirAttack();
                    this.SetAerialWarfare(null, ((BattleBaseAirRaid)bm.BattleDay).BaseAirRaid);
                    this.SetHPBar(bm.BattleDay);
                    this.SetDamageRate(bm);

                    this.BaseLayoutPanel.Visible = !hideDuringBattle;
					break;


				case "api_req_sortie/battle":
				case "api_req_practice/battle":
				case "api_req_sortie/ld_airbattle":
                case "api_req_sortie/ld_shooting":
                    {

                        this.SetFormation(bm);
                        this.SetSearchingResult(bm.BattleDay);
                        this.SetBaseAirAttack(bm.BattleDay.BaseAirAttack);
                        this.SetAerialWarfare(bm.BattleDay.JetAirBattle, bm.BattleDay.AirBattle);
                        this.SetHPBar(bm.BattleDay);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_battle_midnight/battle":
				case "api_req_practice/midnight_battle":
					{

                        this.SetNightBattleEvent(bm.BattleNight.NightInitial);
                        this.SetHPBar(bm.BattleNight);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_battle_midnight/sp_midnight":
					{

                        this.SetFormation(bm);
                        this.ClearBaseAirAttack();
                        this.ClearAerialWarfare();
                        this.ClearSearchingResult();
                        this.SetNightBattleEvent(bm.BattleNight.NightInitial);
                        this.SetHPBar(bm.BattleNight);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_sortie/airbattle":
					{

                        this.SetFormation(bm);
                        this.SetSearchingResult(bm.BattleDay);
                        this.SetBaseAirAttack(bm.BattleDay.BaseAirAttack);
                        this.SetAerialWarfare(bm.BattleDay.JetAirBattle, bm.BattleDay.AirBattle, ((BattleAirBattle)bm.BattleDay).AirBattle2);
                        this.SetHPBar(bm.BattleDay);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_sortie/night_to_day":
					{
						// 暫定
						var battle = bm.BattleNight as BattleDayFromNight;

                        this.SetFormation(bm);
                        this.ClearAerialWarfare();
                        this.ClearSearchingResult();
                        this.ClearBaseAirAttack();
                        this.SetNightBattleEvent(battle.NightInitial);

						if (battle.NextToDay)
						{
                            this.SetSearchingResult(battle);
                            this.SetBaseAirAttack(battle.BaseAirAttack);
                            this.SetAerialWarfare(battle.JetAirBattle, battle.AirBattle);
						}

                        this.SetHPBar(bm.BattleDay);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_combined_battle/battle":
				case "api_req_combined_battle/battle_water":
				case "api_req_combined_battle/ld_airbattle":
				case "api_req_combined_battle/ec_battle":
				case "api_req_combined_battle/each_battle":
				case "api_req_combined_battle/each_battle_water":
                case "api_req_combined_battle/ld_shooting":
                    {

                        this.SetFormation(bm);
                        this.SetSearchingResult(bm.BattleDay);
                        this.SetBaseAirAttack(bm.BattleDay.BaseAirAttack);
                        this.SetAerialWarfare(bm.BattleDay.JetAirBattle, bm.BattleDay.AirBattle);
                        this.SetHPBar(bm.BattleDay);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_combined_battle/airbattle":
					{

                        this.SetFormation(bm);
                        this.SetSearchingResult(bm.BattleDay);
                        this.SetBaseAirAttack(bm.BattleDay.BaseAirAttack);
                        this.SetAerialWarfare(bm.BattleDay.JetAirBattle, bm.BattleDay.AirBattle, ((BattleCombinedAirBattle)bm.BattleDay).AirBattle2);
                        this.SetHPBar(bm.BattleDay);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_combined_battle/midnight_battle":
				case "api_req_combined_battle/ec_midnight_battle":
					{

                        this.SetNightBattleEvent(bm.BattleNight.NightInitial);
                        this.SetHPBar(bm.BattleNight);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_combined_battle/sp_midnight":
					{

                        this.SetFormation(bm);
                        this.ClearAerialWarfare();
                        this.ClearSearchingResult();
                        this.ClearBaseAirAttack();
                        this.SetNightBattleEvent(bm.BattleNight.NightInitial);
                        this.SetHPBar(bm.BattleNight);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_combined_battle/ec_night_to_day":
					{
						var battle = bm.BattleNight as BattleDayFromNight;

                        this.SetFormation(bm);
                        this.ClearAerialWarfare();
                        this.ClearSearchingResult();
                        this.ClearBaseAirAttack();
                        this.SetNightBattleEvent(battle.NightInitial);

						if (battle.NextToDay)
						{
                            this.SetSearchingResult(battle);
                            this.SetBaseAirAttack(battle.BaseAirAttack);
                            this.SetAerialWarfare(battle.JetAirBattle, battle.AirBattle);
						}

                        this.SetHPBar(battle);
                        this.SetDamageRate(bm);

                        this.BaseLayoutPanel.Visible = !hideDuringBattle;
					}
					break;

				case "api_req_sortie/battleresult":
				case "api_req_combined_battle/battleresult":
				case "api_req_practice/battle_result":
					{

                        this.SetMVPShip(bm);

                        this.BaseLayoutPanel.Visible = true;
					}
					break;

			}

            this.TableTop.ResumeLayout();
            this.TableBottom.ResumeLayout();

            this.BaseLayoutPanel.ResumeLayout();


			if (Utility.Configuration.Config.UI.IsLayoutFixed)
                this.TableTop.Width = this.TableTop.GetPreferredSize(this.BaseLayoutPanel.Size).Width;
			else
                this.TableTop.Width = this.TableBottom.ClientSize.Width;
            this.TableTop.Height = this.TableTop.GetPreferredSize(this.BaseLayoutPanel.Size).Height;

		}


		/// <summary>
		/// 陣形・交戦形態を設定します。
		/// </summary>
		private void SetFormation(BattleManager bm)
		{
            this.FormationFriend.Text = Constants.GetFormationShort(bm.FirstBattle.Searching.FormationFriend);
            this.FormationEnemy.Text = Constants.GetFormationShort(bm.FirstBattle.Searching.FormationEnemy);
            this.Formation.Text = Constants.GetEngagementForm(bm.FirstBattle.Searching.EngagementForm);

			if (bm.Compass != null && bm.Compass.EventID == 5)
                this.FleetEnemy.ForeColor = Color.Red;
			else
                this.FleetEnemy.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

            if (bm.IsEnemyCombined && bm.StartsFromDayBattle)
            {
                bool willMain = bm.WillNightBattleWithMainFleet();
                this.FleetEnemy.BackColor = willMain ? 
                    Utility.ThemeManager.GetColor(Utility.ThemeColors.WiilMainColor) : 
                    Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);

                this.FleetEnemyEscort.BackColor = willMain ?
                    Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor) :
                    Utility.ThemeManager.GetColor(Utility.ThemeColors.WiilMainColor); 
            }
            else
            {
                this.FleetEnemy.BackColor =
                this.FleetEnemyEscort.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            }
        }

		/// <summary>
		/// 索敵結果を設定します。
		/// </summary>
		private void SetSearchingResult(BattleData bd)
		{
			void SetResult(ImageLabel label, int search)
			{
				label.Text = Constants.GetSearchingResultShort(search);
				label.ImageAlign = search > 0 ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
				label.ImageIndex = search > 0 ? (int)(search < 4 ? ResourceManager.EquipmentContent.Seaplane : ResourceManager.EquipmentContent.Radar) : -1;
                this.ToolTipInfo.SetToolTip(label, null);
			}

			SetResult(this.SearchingFriend, bd.Searching.SearchingFriend);
			SetResult(this.SearchingEnemy, bd.Searching.SearchingEnemy);
		}

		/// <summary>
		/// 索敵結果をクリアします。
		/// 索敵フェーズが発生しなかった場合にこれを設定します。
		/// </summary>
		private void ClearSearchingResult()
		{
			void ClearResult(ImageLabel label)
			{
				label.Text = "-";
				label.ImageAlign = ContentAlignment.MiddleCenter;
				label.ImageIndex = -1;
                this.ToolTipInfo.SetToolTip(label, null);
			}

			ClearResult(this.SearchingFriend);
			ClearResult(this.SearchingEnemy);
		}

		/// <summary>
		/// 基地航空隊フェーズの結果を設定します。
		/// </summary>
		private void SetBaseAirAttack(PhaseBaseAirAttack pd)
		{
			if (pd != null && pd.IsAvailable)
			{

                this.Searching.Text = "기지항공대";
                this.Searching.ImageAlign = ContentAlignment.MiddleLeft;
                this.Searching.ImageIndex = (int)ResourceManager.EquipmentContent.LandAttacker;

				var sb = new StringBuilder();
				int index = 1;

				foreach (var phase in pd.AirAttackUnits)
				{

					sb.AppendFormat("{0} 회 - #{1} :\r\n",
						index, phase.AirUnitID);

					if (phase.IsStage1Available)
					{
						sb.AppendFormat("　St1: 아군 -{0}/{1} | 적군 -{2}/{3} | {4}\r\n",
							phase.AircraftLostStage1Friend, phase.AircraftTotalStage1Friend,
							phase.AircraftLostStage1Enemy, phase.AircraftTotalStage1Enemy,
							Constants.GetAirSuperiority(phase.AirSuperiority));
					}
					if (phase.IsStage2Available)
					{
						sb.AppendFormat("　St2: 아군 -{0}/{1} | 적군 -{2}/{3}\r\n",
							phase.AircraftLostStage2Friend, phase.AircraftTotalStage2Friend,
							phase.AircraftLostStage2Enemy, phase.AircraftTotalStage2Enemy);
					}

					index++;
				}

                this.ToolTipInfo.SetToolTip(this.Searching, sb.ToString());


			}
			else
			{
                this.ClearBaseAirAttack();
			}

		}

		/// <summary>
		/// 基地航空隊フェーズの結果をクリアします。
		/// </summary>
		private void ClearBaseAirAttack()
		{
            this.Searching.Text = "색적";
            this.Searching.ImageAlign = ContentAlignment.MiddleCenter;
            this.Searching.ImageIndex = -1;
            this.ToolTipInfo.SetToolTip(this.Searching, null);
		}



		/// <summary>
		/// 航空戦表示用ヘルパー
		/// </summary>
		private class AerialWarfareFormatter
		{
			public readonly PhaseAirBattleBase Air;
			public string PhaseName;

			public AerialWarfareFormatter(PhaseAirBattleBase air, string phaseName)
			{
                this.Air = air;
                this.PhaseName = phaseName;
			}

			public bool Enabled => this.Air != null && this.Air.IsAvailable;
			public bool Stage1Enabled => this.Enabled && this.Air.IsStage1Available;
			public bool Stage2Enabled => this.Enabled && this.Air.IsStage2Available;

			public bool GetEnabled(int stage)
			{
				if (stage == 1)
					return this.Stage1Enabled;
				else if (stage == 2)
					return this.Stage2Enabled;
				else
					throw new ArgumentOutOfRangeException();
			}

			public int GetAircraftLost(int stage, bool isFriend)
			{
				if (stage == 1)
					return isFriend ? this.Air.AircraftLostStage1Friend : this.Air.AircraftLostStage1Enemy;
				else if (stage == 2)
					return isFriend ? this.Air.AircraftLostStage2Friend : this.Air.AircraftLostStage2Enemy;
				else
					throw new ArgumentOutOfRangeException();
			}

			public int GetAircraftTotal(int stage, bool isFriend)
			{
				if (stage == 1)
					return isFriend ? this.Air.AircraftTotalStage1Friend : this.Air.AircraftTotalStage1Enemy;
				else if (stage == 2)
					return isFriend ? this.Air.AircraftTotalStage2Friend : this.Air.AircraftTotalStage2Enemy;
				else
					throw new ArgumentOutOfRangeException();
			}

			public int GetTouchAircraft(bool isFriend) => isFriend ? this.Air.TouchAircraftFriend : this.Air.TouchAircraftEnemy;

		}

		void ClearAircraftLabel(ImageLabel label)
		{
			label.Text = "-";
			label.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            label.ImageAlign = ContentAlignment.MiddleCenter;
			label.ImageIndex = -1;
            this.ToolTipInfo.SetToolTip(label, null);
		}


		
		private void SetAerialWarfare(PhaseAirBattleBase phaseJet, PhaseAirBattleBase phase1) => this.SetAerialWarfare(phaseJet, phase1, null);

		/// <summary>
		/// 航空戦情報を設定します。
		/// </summary>
		/// <param name="phaseJet">噴式航空戦のデータ。発生していなければ null</param>
		/// <param name="phase1">第一次航空戦（通常航空戦）のデータ。</param>
		/// <param name="phase2">第二次航空戦のデータ。発生していなければ null</param>
		private void SetAerialWarfare(PhaseAirBattleBase phaseJet, PhaseAirBattleBase phase1, PhaseAirBattleBase phase2)
		{
			var phases = new[] {
				new AerialWarfareFormatter( phaseJet, "분식전: " ),
				new AerialWarfareFormatter( phase1, "제1차: "),
				new AerialWarfareFormatter( phase2, "제2차: "),
			};

			if (!phases[0].Enabled && !phases[2].Enabled)
				phases[1].PhaseName = "";


			void SetShootdown(ImageLabel label, int stage, bool isFriend, bool needAppendInfo)
			{
				var phasesEnabled = phases.Where(p => p.GetEnabled(stage));

				if (needAppendInfo)
				{
					label.Text = string.Join(",", phasesEnabled.Select(p => "-" + p.GetAircraftLost(stage, isFriend)));
                    this.ToolTipInfo.SetToolTip(label, string.Join("", phasesEnabled.Select(p => $"{p.PhaseName}-{p.GetAircraftLost(stage, isFriend)}/{p.GetAircraftTotal(stage, isFriend)}\r\n")));
				}
				else
				{
					label.Text = $"-{phases[1].GetAircraftLost(stage, isFriend)}/{phases[1].GetAircraftTotal(stage, isFriend)}";
                    this.ToolTipInfo.SetToolTip(label, null);
				}

				if (phasesEnabled.Any(p => p.GetAircraftTotal(stage, isFriend) > 0 && p.GetAircraftLost(stage, isFriend) == p.GetAircraftTotal(stage, isFriend)))
					label.ForeColor = Color.Red;
				else
					label.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

                label.ImageAlign = ContentAlignment.MiddleCenter;
                label.ImageIndex = -1;
            }

			void ClearAACutinLabel()
			{
                this.AACutin.Text = "대공포화";
                this.AACutin.ImageAlign = ContentAlignment.MiddleCenter;
                this.AACutin.ImageIndex = -1;
                this.ToolTipInfo.SetToolTip(this.AACutin, null);
			}



			if (phases[1].Stage1Enabled)
			{
				bool needAppendInfo = phases[0].Stage1Enabled || phases[2].Stage1Enabled;
				var phases1 = phases.Where(p => p.Stage1Enabled);

                this.AirSuperiority.Text = Constants.GetAirSuperiority(phases[1].Air.AirSuperiority);

                this.ToolTipInfo.SetToolTip(this.AirSuperiority,
					needAppendInfo ? string.Join("", phases1.Select(p => $"{p.PhaseName}{Constants.GetAirSuperiority(p.Air.AirSuperiority)}\r\n")) : null);


				SetShootdown(this.AirStage1Friend, 1, true, needAppendInfo);
				SetShootdown(this.AirStage1Enemy, 1, false, needAppendInfo);

				void SetTouch(ImageLabel label, bool isFriend)
				{
					if (phases1.Any(p => p.GetTouchAircraft(isFriend) > 0))
					{
						label.ImageAlign = ContentAlignment.MiddleLeft;
						label.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;

                        this.ToolTipInfo.SetToolTip(label, this.ToolTipInfo.GetToolTip(label) +
							"촉접중\r\n" + string.Join("\r\n", phases1.Select(p => $"{p.PhaseName}{(KCDatabase.Instance.MasterEquipments[p.GetTouchAircraft(isFriend)]?.Name ?? "(없음)")}")));
					}
					else
					{
						label.ImageAlign = ContentAlignment.MiddleCenter;
						label.ImageIndex = -1;
					}
				}
				SetTouch(this.AirStage1Friend, true);
				SetTouch(this.AirStage1Enemy, false);
			}
			else
			{
                this.AirSuperiority.Text = Constants.GetAirSuperiority(-1);
                this.ToolTipInfo.SetToolTip(this.AirSuperiority, null);

                this.ClearAircraftLabel(this.AirStage1Friend);
                this.ClearAircraftLabel(this.AirStage1Enemy);
			}


			if (phases[1].Stage2Enabled)
			{
				bool needAppendInfo = phases[0].Stage2Enabled || phases[2].Stage2Enabled;
				var phases2 = phases.Where(p => p.Stage2Enabled);

				SetShootdown(this.AirStage2Friend, 2, true, needAppendInfo);
				SetShootdown(this.AirStage2Enemy, 2, false, needAppendInfo);


				if (phases2.Any(p => p.Air.IsAACutinAvailable))
				{
                    this.AACutin.Text = "#" + string.Join("/", phases2.Select(p => p.Air.IsAACutinAvailable ? (p.Air.AACutInIndex + 1).ToString() : "-"));
                    this.AACutin.ImageAlign = ContentAlignment.MiddleLeft;
                    this.AACutin.ImageIndex = (int)ResourceManager.EquipmentContent.HighAngleGun;

                    this.ToolTipInfo.SetToolTip(this.AACutin, "대공컷인\r\n" +
						string.Join("\r\n", phases2.Select(p => p.PhaseName + (p.Air.IsAACutinAvailable ? $"{p.Air.AACutInShip.NameWithLevel}\r\n컷인종류: {p.Air.AACutInKind} ({Constants.GetAACutinKind(p.Air.AACutInKind)})" : "(발동안함)"))));
				}
				else
				{
					ClearAACutinLabel();
				}
			}
			else
			{
                this.ClearAircraftLabel(this.AirStage2Friend);
                this.ClearAircraftLabel(this.AirStage2Enemy);
				ClearAACutinLabel();
			}
		}

		private void ClearAerialWarfare()
		{
            this.AirSuperiority.Text = "-";
            this.ToolTipInfo.SetToolTip(this.AirSuperiority, null);

            this.ClearAircraftLabel(this.AirStage1Friend);
            this.ClearAircraftLabel(this.AirStage1Enemy);
            this.ClearAircraftLabel(this.AirStage2Friend);
            this.ClearAircraftLabel(this.AirStage2Enemy);

            this.AACutin.Text = "-";
            this.AACutin.ImageAlign = ContentAlignment.MiddleCenter;
            this.AACutin.ImageIndex = -1;
            this.ToolTipInfo.SetToolTip(this.AACutin, null);
		}



		/// <summary>
		/// 両軍のHPゲージを設定します。
		/// </summary>
		private void SetHPBar(BattleData bd)
		{

			KCDatabase db = KCDatabase.Instance;
			bool isPractice = bd.IsPractice;
			bool isFriendCombined = bd.IsFriendCombined;
			bool isEnemyCombined = bd.IsEnemyCombined;
			bool isBaseAirRaid = bd.IsBaseAirRaid;
			bool hasFriend7thShip = bd.Initial.FriendMaxHPs.Count(hp => hp > 0) == 7;

			var initial = bd.Initial;
			var resultHPs = bd.ResultHPs;
			var attackDamages = bd.AttackDamages;


			foreach (var bar in this.HPBars)
				bar.SuspendUpdate();


			void EnableHPBar(int index, int initialHP, int resultHP, int maxHP)
			{
                this.HPBars[index].Value = resultHP;
                this.HPBars[index].PrevValue = initialHP;
                this.HPBars[index].MaximumValue = maxHP;
                this.HPBars[index].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
                this.HPBars[index].ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                this.HPBars[index].Visible = true;
			}

			void DisableHPBar(int index)
			{
                this.HPBars[index].Visible = false;
			}



			// friend main
			for (int i = 0; i < initial.FriendInitialHPs.Length; i++)
			{
				int refindex = BattleIndex.Get(BattleSides.FriendMain, i);

				if (initial.FriendInitialHPs[i] != -1)
				{
					EnableHPBar(refindex, initial.FriendInitialHPs[i], resultHPs[refindex], initial.FriendMaxHPs[i]);

					string name;
					bool isEscaped;
					bool isLandBase;

					var bar = this.HPBars[refindex];

					if (isBaseAirRaid)
					{
						name = string.Format("제{0}기지", i + 1);
						isEscaped = false;
						isLandBase = true;
						bar.Text = "LB";        //note: Land Base (Landing Boat もあるらしいが考えつかなかったので)

					}
					else
					{
						ShipData ship = bd.Initial.FriendFleet.MembersInstance[i];
						name = ship.NameWithLevel;
						isEscaped = bd.Initial.FriendFleet.EscapedShipList.Contains(ship.MasterID);
						isLandBase = ship.MasterShip.IsLandBase;
						bar.Text = Constants.GetShipClassClassification(ship.MasterShip.ShipType);
					}

                    this.ToolTipInfo.SetToolTip(bar, string.Format
						("{0}\r\nHP: ({1} → {2})/{3} ({4}) [{5}]\r\n데미지: {6}\r\n\r\n{7}",
						name,
						Math.Max(bar.PrevValue, 0),
						Math.Max(bar.Value, 0),
						bar.MaximumValue,
						bar.Value - bar.PrevValue,
						Constants.GetDamageState((double)bar.Value / bar.MaximumValue, isPractice, isLandBase, isEscaped),
						attackDamages[refindex],
						bd.GetBattleDetail(refindex)
						));

					if (isEscaped) bar.BackColor = Color.Silver;
					else bar.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
                }
				else
				{
					DisableHPBar(refindex);
				}
			}


			// enemy main
			for (int i = 0; i < initial.EnemyInitialHPs.Length; i++)
			{
				int refindex = BattleIndex.Get(BattleSides.EnemyMain, i);

				if (initial.EnemyInitialHPs[i] != -1)
				{
					EnableHPBar(refindex, initial.EnemyInitialHPs[i], resultHPs[refindex], initial.EnemyMaxHPs[i]);
					ShipDataMaster ship = bd.Initial.EnemyMembersInstance[i];

					var bar = this.HPBars[refindex];
					bar.Text = Constants.GetShipClassClassification(ship.ShipType);

                    this.ToolTipInfo.SetToolTip(bar,
						string.Format("{0} Lv. {1}\r\nHP: ({2} → {3})/{4} ({5}) [{6}]\r\n\r\n{7}",
							ship.NameWithClass,
							initial.EnemyLevels[i],
							Math.Max(bar.PrevValue, 0),
							Math.Max(bar.Value, 0),
							bar.MaximumValue,
							bar.Value - bar.PrevValue,
							Constants.GetDamageState((double)bar.Value / bar.MaximumValue, isPractice, ship.IsLandBase),
							bd.GetBattleDetail(refindex)
							)
						);
				}
				else
				{
					DisableHPBar(refindex);
				}
			}


			// friend escort
			if (isFriendCombined)
			{
                this.FleetFriendEscort.Visible = true;

				for (int i = 0; i < initial.FriendInitialHPsEscort.Length; i++)
				{
					int refindex = BattleIndex.Get(BattleSides.FriendEscort, i);

					if (initial.FriendInitialHPsEscort[i] != -1)
					{
						EnableHPBar(refindex, initial.FriendInitialHPsEscort[i], resultHPs[refindex], initial.FriendMaxHPsEscort[i]);

						ShipData ship = bd.Initial.FriendFleetEscort.MembersInstance[i];
						bool isEscaped = bd.Initial.FriendFleetEscort.EscapedShipList.Contains(ship.MasterID);

						var bar = this.HPBars[refindex];
						bar.Text = Constants.GetShipClassClassification(ship.MasterShip.ShipType);

                        this.ToolTipInfo.SetToolTip(bar, string.Format(
							"{0} Lv. {1}\r\nHP: ({2} → {3})/{4} ({5}) [{6}]\r\n데미지: {7}\r\n\r\n{8}",
							ship.MasterShip.NameWithClass,
							ship.Level,
							Math.Max(bar.PrevValue, 0),
							Math.Max(bar.Value, 0),
							bar.MaximumValue,
							bar.Value - bar.PrevValue,
							Constants.GetDamageState((double)bar.Value / bar.MaximumValue, isPractice, ship.MasterShip.IsLandBase, isEscaped),
							attackDamages[refindex],
							bd.GetBattleDetail(refindex)
							));

						if (isEscaped) bar.BackColor = Color.Silver;
						else bar.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
                    }
					else
					{
						DisableHPBar(refindex);
					}
				}

			}
			else
			{
                this.FleetFriendEscort.Visible = false;

				foreach (var i in BattleIndex.FriendEscort.Skip(Math.Max(bd.Initial.FriendFleet.Members.Count - 6, 0)))
					DisableHPBar(i);
			}

            this.MoveHPBar(hasFriend7thShip);



			// enemy escort
			if (isEnemyCombined)
			{
                this.FleetEnemyEscort.Visible = true;

				for (int i = 0; i < 6; i++)
				{
					int refindex = BattleIndex.Get(BattleSides.EnemyEscort, i);

					if (initial.EnemyInitialHPsEscort[i] != -1)
					{
						EnableHPBar(refindex, initial.EnemyInitialHPsEscort[i], resultHPs[refindex], initial.EnemyMaxHPsEscort[i]);

						ShipDataMaster ship = bd.Initial.EnemyMembersEscortInstance[i];

						var bar = this.HPBars[refindex];
						bar.Text = Constants.GetShipClassClassification(ship.ShipType);

                        this.ToolTipInfo.SetToolTip(bar,
							string.Format("{0} Lv. {1}\r\nHP: ({2} → {3})/{4} ({5}) [{6}]\r\n\r\n{7}",
								ship.NameWithClass,
								bd.Initial.EnemyLevelsEscort[i],
								Math.Max(bar.PrevValue, 0),
								Math.Max(bar.Value, 0),
								bar.MaximumValue,
								bar.Value - bar.PrevValue,
								Constants.GetDamageState((double)bar.Value / bar.MaximumValue, isPractice, ship.IsLandBase),
								bd.GetBattleDetail(refindex)
								)
							);
					}
					else
					{
						DisableHPBar(refindex);
					}
				}

			}
			else
			{
                this.FleetEnemyEscort.Visible = false;

				foreach (var i in BattleIndex.EnemyEscort)
					DisableHPBar(i);
			}




			if ((isFriendCombined || (hasFriend7thShip && !Utility.Configuration.Config.FormBattle.Display7thAsSingleLine)) && isEnemyCombined)
			{
				foreach (var bar in this.HPBars)
				{
					bar.Size = this.SmallBarSize;
					bar.Text = null;
				}
			}
			else
			{
				bool showShipType = Utility.Configuration.Config.FormBattle.ShowShipTypeInHPBar;

				foreach (var bar in this.HPBars)
				{
					bar.Size = this.DefaultBarSize;

					if (!showShipType)
						bar.Text = "HP:";
				}
			}


			{   // support
				PhaseSupport support = null;

				if (bd is BattleDayFromNight bddn)
				{
					if (bddn.NightSupport?.IsAvailable ?? false)
						support = bddn.NightSupport;
				}
				if (support == null)
					support = bd.Support;

				if (support?.IsAvailable ?? false)
				{

					switch (support.SupportFlag)
					{
						case 1:
                            this.FleetFriend.ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedTorpedo;
							break;
						case 2:
                            this.FleetFriend.ImageIndex = (int)ResourceManager.EquipmentContent.MainGunL;
							break;
						case 3:
                            this.FleetFriend.ImageIndex = (int)ResourceManager.EquipmentContent.Torpedo;
							break;
						case 4:
                            this.FleetFriend.ImageIndex = (int)ResourceManager.EquipmentContent.DepthCharge;
							break;
						default:
                            this.FleetFriend.ImageIndex = (int)ResourceManager.EquipmentContent.Unknown;
							break;
					}

                    this.FleetFriend.ImageAlign = ContentAlignment.MiddleLeft;
                    this.ToolTipInfo.SetToolTip(this.FleetFriend, "지원공격\r\n" + support.GetBattleDetail());

					if ((isFriendCombined || hasFriend7thShip) && isEnemyCombined)
                        this.FleetFriend.Text = "아군";
					else
                        this.FleetFriend.Text = "아군함대";

				}
				else
				{
                    this.FleetFriend.ImageIndex = -1;
                    this.FleetFriend.ImageAlign = ContentAlignment.MiddleCenter;
                    this.FleetFriend.Text = "아군함대";
                    this.ToolTipInfo.SetToolTip(this.FleetFriend, null);

				}
			}


			if (bd.Initial.IsBossDamaged)
                this.HPBars[BattleIndex.EnemyMain1].BackColor = Color.MistyRose;

			if (!isBaseAirRaid)
			{
				foreach (int i in bd.MVPShipIndexes)
                    this.HPBars[BattleIndex.Get(BattleSides.FriendMain, i)].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MVPHighlight);

                if (isFriendCombined)
				{
					foreach (int i in bd.MVPShipCombinedIndexes)
                        this.HPBars[BattleIndex.Get(BattleSides.FriendEscort, i)].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MVPHighlight);
                }
			}

			foreach (var bar in this.HPBars)
				bar.ResumeUpdate();
		}


		private bool _hpBarMoved = false;
		/// <summary>
		/// 味方遊撃部隊７人目のHPゲージ（通常時は連合艦隊第二艦隊旗艦のHPゲージ）を移動します。
		/// </summary>
		private void MoveHPBar(bool hasFriend7thShip)
		{
			if (Utility.Configuration.Config.FormBattle.Display7thAsSingleLine && hasFriend7thShip)
			{
				if (this._hpBarMoved)
					return;
                this.TableBottom.SetCellPosition(this.HPBars[BattleIndex.FriendEscort1], new TableLayoutPanelCellPosition(0, 7));
				bool fixSize = Utility.Configuration.Config.UI.IsLayoutFixed;
				bool showHPBar = Utility.Configuration.Config.FormBattle.ShowHPBar;
				ControlHelper.SetTableRowStyle(this.TableBottom, 7, fixSize ? new RowStyle(SizeType.Absolute, showHPBar ? 21 : 16) : new RowStyle(SizeType.AutoSize));
                this._hpBarMoved = true;
			}
			else
			{
				if (!this._hpBarMoved)
					return;
                this.TableBottom.SetCellPosition(this.HPBars[BattleIndex.FriendEscort1], new TableLayoutPanelCellPosition(1, 1));
				ControlHelper.SetTableRowStyle(this.TableBottom, 7, new RowStyle(SizeType.Absolute, 0));
                this._hpBarMoved = false;
			}

		}


		/// <summary>
		/// 損害率と戦績予測を設定します。
		/// </summary>
		private void SetDamageRate(BattleManager bm)
		{
			int rank = bm.PredictWinRank(out double friendrate, out double enemyrate);

            this.DamageFriend.Text = friendrate.ToString("p1");
            this.DamageEnemy.Text = enemyrate.ToString("p1");

			if (bm.IsBaseAirRaid)
			{
				int kind = bm.Compass.AirRaidDamageKind;
                this.WinRank.Text = Constants.GetAirRaidDamageShort(kind);
                this.WinRank.ForeColor = (1 <= kind && kind <= 3) ? this.WinRankColor_Lose : this.WinRankColor_Win;
			}
			else
			{
                this.WinRank.Text = Constants.GetWinRank(rank);
                this.WinRank.ForeColor = rank >= 4 ? this.WinRankColor_Win : this.WinRankColor_Lose;
			}

            this.WinRank.MinimumSize = Utility.Configuration.Config.UI.IsLayoutFixed ? new Size(this.DefaultBarSize.Width, 0) : new Size(this.HPBars[0].Width, 0);
		}


		/// <summary>
		/// 夜戦における各種表示を設定します。
		/// </summary>
		/// <param name="hp">戦闘開始前のHP。</param>
		/// <param name="isCombined">連合艦隊かどうか。</param>
		/// <param name="bd">戦闘データ。</param>
		private void SetNightBattleEvent(PhaseNightInitial pd)
		{

			FleetData fleet = pd.FriendFleet;

			//味方探照灯判定
			{
				int index = pd.SearchlightIndexFriend;

				if (index != -1)
				{
					ShipData ship = fleet.MembersInstance[index];

                    this.AirStage1Friend.Text = "#" + (index + (pd.IsFriendEscort ? 6 : 0) + 1);
                    this.AirStage1Friend.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                    this.AirStage1Friend.ImageAlign = ContentAlignment.MiddleLeft;
                    this.AirStage1Friend.ImageIndex = (int)ResourceManager.EquipmentContent.Searchlight;
                    this.ToolTipInfo.SetToolTip(this.AirStage1Friend, "탐조등조사: " + ship.NameWithLevel);
				}
				else
				{
                    this.ToolTipInfo.SetToolTip(this.AirStage1Friend, null);
				}
			}

			//敵探照灯判定
			{
				int index = pd.SearchlightIndexEnemy;
				if (index != -1)
				{
                    this.AirStage1Friend.Text = "#" + (index + (pd.IsFriendEscort ? 6 : 0) + 1);
                    this.AirStage1Enemy.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                    this.AirStage1Enemy.ImageAlign = ContentAlignment.MiddleLeft;
                    this.AirStage1Enemy.ImageIndex = (int)ResourceManager.EquipmentContent.Searchlight;
                    this.ToolTipInfo.SetToolTip(this.AirStage1Enemy, "탐조등조사: " + pd.SearchlightEnemyInstance.NameWithClass);
				}
				else
				{
                    this.ToolTipInfo.SetToolTip(this.AirStage1Enemy, null);
				}
			}


			//夜間触接判定
			if (pd.TouchAircraftFriend != -1)
			{
                this.SearchingFriend.Text = "야간촉접";
                this.SearchingFriend.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;
                this.SearchingFriend.ImageAlign = ContentAlignment.MiddleLeft;
                this.ToolTipInfo.SetToolTip(this.SearchingFriend, "야간촉접중: " + KCDatabase.Instance.MasterEquipments[pd.TouchAircraftFriend].Name);
			}
			else
			{
                this.ToolTipInfo.SetToolTip(this.SearchingFriend, null);
			}

			if (pd.TouchAircraftEnemy != -1)
			{
                this.SearchingEnemy.Text = "야간촉접";
                this.SearchingEnemy.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;
                this.SearchingFriend.ImageAlign = ContentAlignment.MiddleLeft;
                this.ToolTipInfo.SetToolTip(this.SearchingEnemy, "야간촉접중: " + KCDatabase.Instance.MasterEquipments[pd.TouchAircraftEnemy].Name);
			}
			else
			{
                this.ToolTipInfo.SetToolTip(this.SearchingEnemy, null);
			}

			//照明弾投射判定
			{
				int index = pd.FlareIndexFriend;

				if (index != -1)
				{
                    this.AirStage2Friend.Text = "#" + (index + 1);
                    this.AirStage2Friend.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                    this.AirStage2Friend.ImageAlign = ContentAlignment.MiddleLeft;
                    this.AirStage2Friend.ImageIndex = (int)ResourceManager.EquipmentContent.Flare;
                    this.ToolTipInfo.SetToolTip(this.AirStage2Friend, "조명탄사용: " + pd.FlareFriendInstance.NameWithLevel);

				}
				else
				{
                    this.ToolTipInfo.SetToolTip(this.AirStage2Friend, null);
				}
			}

			{
				int index = pd.FlareIndexEnemy;

				if (index != -1)
				{
                    this.AirStage2Enemy.Text = "#" + (index + 1);
                    this.AirStage2Enemy.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                    this.AirStage2Enemy.ImageAlign = ContentAlignment.MiddleLeft;
                    this.AirStage2Enemy.ImageIndex = (int)ResourceManager.EquipmentContent.Flare;
                    this.ToolTipInfo.SetToolTip(this.AirStage2Enemy, "조명탄사용: " + pd.FlareEnemyInstance.NameWithClass);
				}
				else
				{
                    this.ToolTipInfo.SetToolTip(this.AirStage2Enemy, null);
				}
			}
		}


		/// <summary>
		/// 戦闘終了後に、MVP艦の表示を更新します。
		/// </summary>
		/// <param name="bm">戦闘データ。</param>
		private void SetMVPShip(BattleManager bm)
		{

			bool isCombined = bm.IsCombinedBattle;

			var bd = bm.StartsFromDayBattle ? (BattleData)bm.BattleDay : (BattleData)bm.BattleNight;
			var br = bm.Result;

			var friend = bd.Initial.FriendFleet;
			var escort = !isCombined ? null : bd.Initial.FriendFleetEscort;


			/*// DEBUG
			{
				BattleData lastbattle = bm.StartsFromDayBattle ? (BattleData)bm.BattleNight ?? bm.BattleDay : (BattleData)bm.BattleDay ?? bm.BattleNight;
				if ( lastbattle.MVPShipIndexes.Count() > 1 || !lastbattle.MVPShipIndexes.Contains( br.MVPIndex - 1 ) ) {
					Utility.Logger.Add( 1, "MVP is wrong : [" + string.Join( ",", lastbattle.MVPShipIndexes ) + "] => " + ( br.MVPIndex - 1 ) );
				}
				if ( isCombined && ( lastbattle.MVPShipCombinedIndexes.Count() > 1 || !lastbattle.MVPShipCombinedIndexes.Contains( br.MVPIndexCombined - 1 ) ) ) {
					Utility.Logger.Add( 1, "MVP is wrong (escort) : [" + string.Join( ",", lastbattle.MVPShipCombinedIndexes ) + "] => " + ( br.MVPIndexCombined - 1 ) );
				}
			}
			//*/


			for (int i = 0; i < friend.Members.Count; i++)
			{
				if (friend.EscapedShipList.Contains(friend.Members[i]))
                    this.HPBars[i].BackColor = Color.Silver;

				else if (br.MVPIndex == i + 1)
                    this.HPBars[i].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MVPHighlight);

                else
                    this.HPBars[i].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            }

			if (escort != null)
			{
				for (int i = 0; i < escort.Members.Count; i++)
				{
					if (escort.EscapedShipList.Contains(escort.Members[i]))
                        this.HPBars[i + 6].BackColor = Color.Silver;

					else if (br.MVPIndexCombined == i + 1)
                        this.HPBars[i + 6].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MVPHighlight);

                    else
                        this.HPBars[i + 6].BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
                }
			}

			/*// debug
			if ( WinRank.Text.First().ToString() != bm.Result.Rank ) {
				Utility.Logger.Add( 1, string.Format( "戦闘評価予測が誤っています。(予測: {0}, 実際: {1})", WinRank.Text.First().ToString(), bm.Result.Rank ) );
			}
			//*/

		}


		private void RightClickMenu_Opening(object sender, CancelEventArgs e)
		{

			var bm = KCDatabase.Instance.Battle;

			if (bm == null || bm.BattleMode == BattleManager.BattleModes.Undefined)
				e.Cancel = true;

            this.RightClickMenu_ShowBattleResult.Enabled = !this.BaseLayoutPanel.Visible;
		}

		private void RightClickMenu_ShowBattleDetail_Click(object sender, EventArgs e)
		{
			var bm = KCDatabase.Instance.Battle;

			if (bm == null || bm.BattleMode == BattleManager.BattleModes.Undefined)
				return;

			var dialog = new Dialog.DialogBattleDetail
			{
				BattleDetailText = BattleDetailDescriptor.GetBattleDetail(bm),
				Location = this.RightClickMenu.Location
			};
			dialog.Show(this);

		}

		private void RightClickMenu_ShowBattleResult_Click(object sender, EventArgs e)
		{
            this.BaseLayoutPanel.Visible = true;
		}




		void ConfigurationChanged()
		{

			var config = Utility.Configuration.Config;

            this.MainFont = this.TableTop.Font = this.TableBottom.Font = this.Font = config.UI.MainFont;
            this.SubFont = config.UI.SubFont;

            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);

            this.BaseLayoutPanel.AutoScroll = config.FormBattle.IsScrollable;


			bool fixSize = config.UI.IsLayoutFixed;
			bool showHPBar = config.FormBattle.ShowHPBar;

            this.TableBottom.SuspendLayout();
			if (fixSize)
			{
				ControlHelper.SetTableColumnStyles(this.TableBottom, new ColumnStyle(SizeType.AutoSize));
				ControlHelper.SetTableRowStyle(this.TableBottom, 0, new RowStyle(SizeType.Absolute, 21));
				for (int i = 1; i <= 6; i++)
					ControlHelper.SetTableRowStyle(this.TableBottom, i, new RowStyle(SizeType.Absolute, showHPBar ? 21 : 16));
				ControlHelper.SetTableRowStyle(this.TableBottom, 8, new RowStyle(SizeType.Absolute, 21));
			}
			else
			{
				ControlHelper.SetTableColumnStyles(this.TableBottom, new ColumnStyle(SizeType.AutoSize));
				ControlHelper.SetTableRowStyles(this.TableBottom, new RowStyle(SizeType.AutoSize));
			}
			if (this.HPBars != null)
			{
				foreach (var b in this.HPBars)
				{
					b.MainFont = this.MainFont;
					b.SubFont = this.SubFont;
					b.AutoSize = !fixSize;
					if (!b.AutoSize)
					{
						b.Size = (this.HPBars[12].Visible && this.HPBars[18].Visible) ? this.SmallBarSize : this.DefaultBarSize;
					}
					b.HPBar.ColorMorphing = config.UI.BarColorMorphing;
					b.HPBar.SetBarColorScheme(config.UI.BarColorScheme.Select(col => col.ColorData).ToArray());
					b.ShowHPBar = showHPBar;
				}
			}
            this.FleetFriend.MaximumSize =
            this.FleetFriendEscort.MaximumSize =
            this.FleetEnemy.MaximumSize =
            this.FleetEnemyEscort.MaximumSize =
            this.DamageFriend.MaximumSize =
            this.DamageEnemy.MaximumSize =
				fixSize ? this.DefaultBarSize : Size.Empty;

            this.WinRank.MinimumSize = fixSize ? new Size(80, 0) : new Size(this.HPBars[0].Width, 0);

            this.TableBottom.ResumeLayout();

            this.TableTop.SuspendLayout();
			if (fixSize)
			{
				ControlHelper.SetTableColumnStyles(this.TableTop, new ColumnStyle(SizeType.Absolute, 21 * 4));
				ControlHelper.SetTableRowStyles(this.TableTop, new RowStyle(SizeType.Absolute, 21));
                this.TableTop.Width = this.TableTop.GetPreferredSize(this.BaseLayoutPanel.Size).Width;
			}
			else
			{
				ControlHelper.SetTableColumnStyles(this.TableTop, new ColumnStyle(SizeType.Percent, 100));
				ControlHelper.SetTableRowStyles(this.TableTop, new RowStyle(SizeType.AutoSize));
                this.TableTop.Width = this.TableBottom.ClientSize.Width;
			}
            this.TableTop.Height = this.TableTop.GetPreferredSize(this.BaseLayoutPanel.Size).Height;
            this.TableTop.ResumeLayout();

		}



		private void TableTop_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			if (e.Row == 1 || e.Row == 3)
				e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		private void TableBottom_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			if (e.Row == 8)
				e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}


		protected override string GetPersistString()
		{
			return "Battle";
		}

        private void AirSuperiority_Click(object sender, EventArgs e)
        {

        }
    }

}
