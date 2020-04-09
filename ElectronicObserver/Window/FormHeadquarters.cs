using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
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
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Window.Support;
using ElectronicObserver.Resource.Record;

namespace ElectronicObserver.Window
{

	public partial class FormHeadquarters : DockContent
	{

		private Form _parentForm;

		public FormHeadquarters(FormMain parent)
		{
            this.InitializeComponent();

            this._parentForm = parent;


			ImageList icons = ResourceManager.Instance.Icons;

            this.ShipCount.ImageList = icons;
            this.ShipCount.ImageIndex = (int)ResourceManager.IconContent.HeadQuartersShip;
            this.EquipmentCount.ImageList = icons;
            this.EquipmentCount.ImageIndex = (int)ResourceManager.IconContent.HeadQuartersEquipment;
            this.InstantRepair.ImageList = icons;
            this.InstantRepair.ImageIndex = (int)ResourceManager.IconContent.ItemInstantRepair;
            this.InstantConstruction.ImageList = icons;
            this.InstantConstruction.ImageIndex = (int)ResourceManager.IconContent.ItemInstantConstruction;
            this.DevelopmentMaterial.ImageList = icons;
            this.DevelopmentMaterial.ImageIndex = (int)ResourceManager.IconContent.ItemDevelopmentMaterial;
            this.ModdingMaterial.ImageList = icons;
            this.ModdingMaterial.ImageIndex = (int)ResourceManager.IconContent.ItemModdingMaterial;
            this.FurnitureCoin.ImageList = icons;
            this.FurnitureCoin.ImageIndex = (int)ResourceManager.IconContent.ItemFurnitureCoin;
            this.Fuel.ImageList = icons;
            this.Fuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            this.Ammo.ImageList = icons;
            this.Ammo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            this.Steel.ImageList = icons;
            this.Steel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
            this.Bauxite.ImageList = icons;
            this.Bauxite.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;
            this.DisplayUseItem.ImageList = icons;
            this.DisplayUseItem.ImageIndex = (int)ResourceManager.IconContent.ItemPresentBox;


			ControlHelper.SetDoubleBuffered(this.FlowPanelMaster);
			ControlHelper.SetDoubleBuffered(this.FlowPanelAdmiral);
			ControlHelper.SetDoubleBuffered(this.FlowPanelFleet);
			ControlHelper.SetDoubleBuffered(this.FlowPanelUseItem);
			ControlHelper.SetDoubleBuffered(this.FlowPanelResource);


            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormHeadQuarters]);

		}


		private void FormHeadquarters_Load(object sender, EventArgs e)
		{

			APIObserver o = APIObserver.Instance;

			o.APIList["api_req_nyukyo/start"].RequestReceived += this.Updated;
			o.APIList["api_req_nyukyo/speedchange"].RequestReceived += this.Updated;
			o.APIList["api_req_kousyou/createship"].RequestReceived += this.Updated;
			o.APIList["api_req_kousyou/createship_speedchange"].RequestReceived += this.Updated;
			o.APIList["api_req_kousyou/destroyship"].RequestReceived += this.Updated;
			o.APIList["api_req_kousyou/destroyitem2"].RequestReceived += this.Updated;
			o.APIList["api_req_member/updatecomment"].RequestReceived += this.Updated;

			o.APIList["api_get_member/basic"].ResponseReceived += this.Updated;
			o.APIList["api_get_member/slot_item"].ResponseReceived += this.Updated;
			o.APIList["api_port/port"].ResponseReceived += this.Updated;
			o.APIList["api_get_member/ship2"].ResponseReceived += this.Updated;
			o.APIList["api_req_kousyou/getship"].ResponseReceived += this.Updated;
			o.APIList["api_req_hokyu/charge"].ResponseReceived += this.Updated;
			o.APIList["api_req_kousyou/destroyship"].ResponseReceived += this.Updated;
			o.APIList["api_req_kousyou/destroyitem2"].ResponseReceived += this.Updated;
			o.APIList["api_req_kaisou/powerup"].ResponseReceived += this.Updated;
			o.APIList["api_req_kousyou/createitem"].ResponseReceived += this.Updated;
			o.APIList["api_req_kousyou/remodel_slot"].ResponseReceived += this.Updated;
			o.APIList["api_get_member/material"].ResponseReceived += this.Updated;
			o.APIList["api_get_member/ship_deck"].ResponseReceived += this.Updated;
			o.APIList["api_req_air_corps/set_plane"].ResponseReceived += this.Updated;
			o.APIList["api_req_air_corps/supply"].ResponseReceived += this.Updated;
			o.APIList["api_get_member/useitem"].ResponseReceived += this.Updated;


			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
			Utility.SystemEvents.UpdateTimerTick += this.SystemEvents_UpdateTimerTick;

            this.FlowPanelResource.SetFlowBreak(this.Ammo, true);

            this.FlowPanelMaster.Visible = false;

		}



		void ConfigurationChanged()
		{

            this.Font = this.FlowPanelMaster.Font = Utility.Configuration.Config.UI.MainFont;
            this.HQLevel.MainFont = Utility.Configuration.Config.UI.MainFont;
            this.HQLevel.SubFont = Utility.Configuration.Config.UI.SubFont;

            this.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.HQLevel.MainFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.HQLevel.SubFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.SubFontColor);

            // 点滅しない設定にしたときに消灯状態で固定されるのを防ぐ
            if (!Utility.Configuration.Config.FormHeadquarters.BlinkAtMaximum)
			{
                if (this.ShipCount.Tag as bool? ?? false)
                {
                    this.ShipCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }

                if (this.EquipmentCount.Tag as bool? ?? false)
                {
                    this.EquipmentCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }
            }

			//visibility
			CheckVisibilityConfiguration();
			{
				var visibility = Utility.Configuration.Config.FormHeadquarters.Visibility.List;
                this.AdmiralName.Visible = visibility[0];
                this.AdmiralComment.Visible = visibility[1];
                this.HQLevel.Visible = visibility[2];
                this.ShipCount.Visible = visibility[3];
                this.EquipmentCount.Visible = visibility[4];
                this.InstantRepair.Visible = visibility[5];
                this.InstantConstruction.Visible = visibility[6];
                this.DevelopmentMaterial.Visible = visibility[7];
                this.ModdingMaterial.Visible = visibility[8];
                this.FurnitureCoin.Visible = visibility[9];
                this.Fuel.Visible = visibility[10];
                this.Ammo.Visible = visibility[11];
                this.Steel.Visible = visibility[12];
                this.Bauxite.Visible = visibility[13];
                this.DisplayUseItem.Visible = visibility[14];
			}

            this.UpdateDisplayUseItem();
		}


		/// <summary>
		/// VisibleFlags 設定をチェックし、不正な値だった場合は初期値に戻します。
		/// </summary>
		public static void CheckVisibilityConfiguration()
		{
			const int count = 15;
			var config = Utility.Configuration.Config.FormHeadquarters;

			if (config.Visibility == null)
				config.Visibility = new Utility.Storage.SerializableList<bool>(Enumerable.Repeat(true, count).ToList());

			for (int i = config.Visibility.List.Count; i < count; i++)
			{
				config.Visibility.List.Add(true);
			}

		}

		/// <summary>
		/// 各表示項目の名称を返します。
		/// </summary>
		public static IEnumerable<string> GetItemNames()
		{
			yield return "제독명";
			yield return "제독코멘트";
			yield return "사령부Lv";
			yield return "함선수";
			yield return "장비수";
			yield return "고속수복재";
			yield return "고속건조재";
			yield return "개발자재";
			yield return "개수자재";
			yield return "가구코인";
			yield return "연료";
			yield return "탄약";
			yield return "강재";
			yield return "보크사이트";
			yield return "자유항목";
		}


		void Updated(string apiname, dynamic data)
		{

			KCDatabase db = KCDatabase.Instance;


			if (!db.Admiral.IsAvailable)
				return;


			// 資源上限超過時の色
			Color overcolor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.OrangeHighlight);



            this.FlowPanelMaster.SuspendLayout();

            //Admiral
            this.FlowPanelAdmiral.SuspendLayout();
            this.AdmiralName.Text = string.Format("{0} {1}", db.Admiral.AdmiralName, Constants.GetAdmiralRank(db.Admiral.Rank));
			{
				StringBuilder tooltip = new StringBuilder();

				var sortieCount = db.Admiral.SortieWin + db.Admiral.SortieLose;
				tooltip.AppendFormat("출격회수: {0} / 승리회수: {1} ({2:p2}) / 패배회수: {3}\r\n",
					sortieCount, db.Admiral.SortieWin, db.Admiral.SortieWin / Math.Max(sortieCount, 1.0), db.Admiral.SortieLose);

				tooltip.AppendFormat("출격당평균획득Exp: {0:n2} / 승리시 {1:n2}\r\n",
					 db.Admiral.Exp / Math.Max(sortieCount, 1.0),
					 db.Admiral.Exp / Math.Max(db.Admiral.SortieWin, 1.0));

				tooltip.AppendFormat("원정회수: {0} / 성공회수: {1} ({2:p2}) / 실패회수: {3}\r\n",
					db.Admiral.MissionCount, db.Admiral.MissionSuccess, db.Admiral.MissionSuccess / Math.Max(db.Admiral.MissionCount, 1.0), db.Admiral.MissionCount - db.Admiral.MissionSuccess);

				var practiceCount = db.Admiral.PracticeWin + db.Admiral.PracticeLose;
				tooltip.AppendFormat("연습회수: {0} / 승리: {1} ({2:p2}) / 패배: {3}\r\n",
					practiceCount, db.Admiral.PracticeWin, db.Admiral.PracticeWin / Math.Max(practiceCount, 1.0), db.Admiral.PracticeLose);

				tooltip.AppendFormat("갑훈장 보유 수: {0}\r\n", db.Admiral.Medals);

                this.ToolTipInfo.SetToolTip(this.AdmiralName, tooltip.ToString());
			}
            this.AdmiralComment.Text = db.Admiral.Comment;
            this.FlowPanelAdmiral.ResumeLayout();

            //HQ Level
            this.HQLevel.Value = db.Admiral.Level;
			{
				StringBuilder tooltip = new StringBuilder();
				if (db.Admiral.Level < ExpTable.AdmiralExp.Max(e => e.Key))
				{
                    this.HQLevel.TextNext = "next:";
                    this.HQLevel.ValueNext = ExpTable.GetNextExpAdmiral(db.Admiral.Exp);
					tooltip.AppendFormat("{0} / {1}\r\n", db.Admiral.Exp, ExpTable.AdmiralExp[db.Admiral.Level + 1].Total);
				}
				else
				{
                    this.HQLevel.TextNext = "exp:";
                    this.HQLevel.ValueNext = db.Admiral.Exp;
				}

				//戦果ツールチップ
				//fixme: もっとましな書き方はなかっただろうか
				{
					var res = RecordManager.Instance.Resource.GetRecordPrevious();
					if (res != null)
					{
						int diff = db.Admiral.Exp - res.HQExp;
						tooltip.AppendFormat("이번: +{0} exp. / 전과 {1:n2}\r\n", diff, diff * 7 / 10000.0);
					}
				}
				{
					var res = RecordManager.Instance.Resource.GetRecordDay();
					if (res != null)
					{
						int diff = db.Admiral.Exp - res.HQExp;
						tooltip.AppendFormat("오늘: +{0} exp. / 전과 {1:n2}\r\n", diff, diff * 7 / 10000.0);
					}
				}
				{
					var res = RecordManager.Instance.Resource.GetRecordMonth();
					if (res != null)
					{
						int diff = db.Admiral.Exp - res.HQExp;
						tooltip.AppendFormat("이달: +{0} exp. / 전과 {1:n2}\r\n", diff, diff * 7 / 10000.0);
					}
				}

                this.ToolTipInfo.SetToolTip(this.HQLevel, tooltip.ToString());
			}

            //Fleet
            this.FlowPanelFleet.SuspendLayout();
			{

                this.ShipCount.Text = string.Format("{0}/{1}", this.RealShipCount, db.Admiral.MaxShipCount);
				if (this.RealShipCount > db.Admiral.MaxShipCount - 5)
				{
                    this.ShipCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }
				else
				{
                    this.ShipCount.BackColor = Color.Transparent;
				}
                this.ShipCount.Tag = this.RealShipCount >= db.Admiral.MaxShipCount;

                this.EquipmentCount.Text = string.Format("{0}/{1}", this.RealEquipmentCount, db.Admiral.MaxEquipmentCount);
				if (this.RealEquipmentCount > db.Admiral.MaxEquipmentCount + 3 - 20)
				{
                    this.EquipmentCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }
				else
				{
                    this.EquipmentCount.BackColor = Color.Transparent;
				}
                this.EquipmentCount.Tag = this.RealEquipmentCount >= db.Admiral.MaxEquipmentCount;

			}
            this.FlowPanelFleet.ResumeLayout();



			var resday = RecordManager.Instance.Resource.GetRecord(DateTime.Now.AddHours(-5).Date.AddHours(5));
			var resweek = RecordManager.Instance.Resource.GetRecord(DateTime.Now.AddHours(-5).Date.AddDays(-(((int)DateTime.Now.AddHours(-5).DayOfWeek + 6) % 7)).AddHours(5)); //月曜日起点
			var resmonth = RecordManager.Instance.Resource.GetRecord(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddHours(5));


            //UseItems
            this.FlowPanelUseItem.SuspendLayout();

            this.InstantRepair.Text = db.Material.InstantRepair.ToString();
            this.InstantRepair.BackColor = db.Material.InstantRepair >= 3000 ? overcolor : Color.Transparent;
            this.ToolTipInfo.SetToolTip(this.InstantRepair, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.InstantRepair - resday.InstantRepair),
					resweek == null ? 0 : (db.Material.InstantRepair - resweek.InstantRepair),
					resmonth == null ? 0 : (db.Material.InstantRepair - resmonth.InstantRepair)));

            this.InstantConstruction.Text = db.Material.InstantConstruction.ToString();
            this.InstantConstruction.BackColor = db.Material.InstantConstruction >= 3000 ? overcolor : Color.Transparent;
            this.ToolTipInfo.SetToolTip(this.InstantConstruction, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.InstantConstruction - resday.InstantConstruction),
					resweek == null ? 0 : (db.Material.InstantConstruction - resweek.InstantConstruction),
					resmonth == null ? 0 : (db.Material.InstantConstruction - resmonth.InstantConstruction)));

            this.DevelopmentMaterial.Text = db.Material.DevelopmentMaterial.ToString();
            this.DevelopmentMaterial.BackColor = db.Material.DevelopmentMaterial >= 3000 ? overcolor : Color.Transparent;
            this.ToolTipInfo.SetToolTip(this.DevelopmentMaterial, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.DevelopmentMaterial - resday.DevelopmentMaterial),
					resweek == null ? 0 : (db.Material.DevelopmentMaterial - resweek.DevelopmentMaterial),
					resmonth == null ? 0 : (db.Material.DevelopmentMaterial - resmonth.DevelopmentMaterial)));

            this.ModdingMaterial.Text = db.Material.ModdingMaterial.ToString();
            this.ModdingMaterial.BackColor = db.Material.ModdingMaterial >= 3000 ? overcolor : Color.Transparent;
            this.ToolTipInfo.SetToolTip(this.ModdingMaterial, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.ModdingMaterial - resday.ModdingMaterial),
					resweek == null ? 0 : (db.Material.ModdingMaterial - resweek.ModdingMaterial),
					resmonth == null ? 0 : (db.Material.ModdingMaterial - resmonth.ModdingMaterial)));

            this.FurnitureCoin.Text = db.Admiral.FurnitureCoin.ToString();
            this.FurnitureCoin.BackColor = db.Admiral.FurnitureCoin >= 200000 ? overcolor : Color.Transparent;
			{
				int small = db.UseItems[10]?.Count ?? 0;
				int medium = db.UseItems[11]?.Count ?? 0;
				int large = db.UseItems[12]?.Count ?? 0;

                this.ToolTipInfo.SetToolTip(this.FurnitureCoin,
						string.Format("(소) x {0} ( +{1} )\r\n(중) x {2} ( +{3} )\r\n(대) x {4} ( +{5} )\r\n",
							small, small * 200,
							medium, medium * 400,
							large, large * 700));
			}
            this.UpdateDisplayUseItem();
            this.FlowPanelUseItem.ResumeLayout();


            //Resources
            this.FlowPanelResource.SuspendLayout();
			{

                this.Fuel.Text = db.Material.Fuel.ToString();
                this.Fuel.BackColor = db.Material.Fuel < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
                this.ToolTipInfo.SetToolTip(this.Fuel, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Fuel - resday.Fuel),
					resweek == null ? 0 : (db.Material.Fuel - resweek.Fuel),
					resmonth == null ? 0 : (db.Material.Fuel - resmonth.Fuel)));

                this.Ammo.Text = db.Material.Ammo.ToString();
                this.Ammo.BackColor = db.Material.Ammo < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
                this.ToolTipInfo.SetToolTip(this.Ammo, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Ammo - resday.Ammo),
					resweek == null ? 0 : (db.Material.Ammo - resweek.Ammo),
					resmonth == null ? 0 : (db.Material.Ammo - resmonth.Ammo)));

                this.Steel.Text = db.Material.Steel.ToString();
                this.Steel.BackColor = db.Material.Steel < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
                this.ToolTipInfo.SetToolTip(this.Steel, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Steel - resday.Steel),
					resweek == null ? 0 : (db.Material.Steel - resweek.Steel),
					resmonth == null ? 0 : (db.Material.Steel - resmonth.Steel)));

                this.Bauxite.Text = db.Material.Bauxite.ToString();
                this.Bauxite.BackColor = db.Material.Bauxite < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
                this.ToolTipInfo.SetToolTip(this.Bauxite, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Bauxite - resday.Bauxite),
					resweek == null ? 0 : (db.Material.Bauxite - resweek.Bauxite),
					resmonth == null ? 0 : (db.Material.Bauxite - resmonth.Bauxite)));

			}
            this.FlowPanelResource.ResumeLayout();

            this.FlowPanelMaster.ResumeLayout();
			if (!this.FlowPanelMaster.Visible)
                this.FlowPanelMaster.Visible = true;
            this.AdmiralName.Refresh();
            this.AdmiralComment.Refresh();

		}


		void SystemEvents_UpdateTimerTick()
		{

			KCDatabase db = KCDatabase.Instance;

			if (db.Ships.Count <= 0) return;

			if (Utility.Configuration.Config.FormHeadquarters.BlinkAtMaximum)
			{
                if (this.ShipCount.Tag as bool? ?? false)
                {
                    this.ShipCount.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight) : Color.Transparent;
                }

                if (this.EquipmentCount.Tag as bool? ?? false)
                {
                    this.EquipmentCount.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight) : Color.Transparent;
                }
            }
		}


		private void Resource_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				new Dialog.DialogResourceChart().Show(this._parentForm);
		}

		private void Resource_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				try
				{
					var mat = KCDatabase.Instance.Material;
					Clipboard.SetText($"{mat.Fuel}/{mat.Ammo}/{mat.Steel}/{mat.Bauxite}/修復{mat.InstantRepair}/開発{mat.DevelopmentMaterial}/建造{mat.InstantConstruction}/改修{mat.ModdingMaterial}");
				}
				catch (Exception ex)
				{
					Utility.Logger.Add(3, "자원의 클립보드 복사에 실패했습니다." + ex.Message);
				}
			}
		}


		private void UpdateDisplayUseItem()
		{
			var db = KCDatabase.Instance;
			var itemID = Utility.Configuration.Config.FormHeadquarters.DisplayUseItemID;
			var item = db.UseItems[itemID];
			var itemMaster = db.MasterUseItems[itemID];
			string tail = "\r\n(설정에서 항목 변경 가능)\r\n(우클릭으로 모든 항목 표시)";



			switch (itemMaster?.Name)
			{
				case null:
                    this.DisplayUseItem.Text = "???";
                    this.ToolTipInfo.SetToolTip(this.DisplayUseItem, "알수없는항목 (ID: " + Utility.Configuration.Config.FormHeadquarters.DisplayUseItemID + ")" + tail);
					break;

				// '18 spring event special mode
				case "쌀":
				case "매실":
				case "김":
				case "차":
                    this.DisplayUseItem.Text = (item?.Count ?? 0).ToString();
                    this.ToolTipInfo.SetToolTip(this.DisplayUseItem,
						$"쌀: {db.UseItems[85]?.Count ?? 0}\r\n매실: {db.UseItems[86]?.Count ?? 0}\r\n김: {db.UseItems[87]?.Count ?? 0}\r\n차: {db.UseItems[88]?.Count ?? 0}\r\n{tail}");
					break;
                case "꽁치":
                case "정어리":
                    this.DisplayUseItem.Text = (item?.Count ?? 0).ToString();
                    this.ToolTipInfo.SetToolTip(this.DisplayUseItem,
                        $"꽁치: {db.UseItems[68]?.Count ?? 0}\r\n정어리: {db.UseItems[93]?.Count ?? 0}\r\n{tail}");
                    break;

                default:
                    this.DisplayUseItem.Text = (item?.Count ?? 0).ToString();
                    this.ToolTipInfo.SetToolTip(this.DisplayUseItem,
						itemMaster.Name + tail);
					break;
			}
			
		}

        private void DisplayUseItem_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var db = KCDatabase.Instance;
                var sb = new StringBuilder();
                foreach (var item in db.UseItems.Values)
                {
                    sb.Append(item.MasterUseItem.Name).Append(" x ").Append(item.Count).AppendLine();
                }

                MessageBox.Show(sb.ToString(), "보유 아이템 목록", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private int RealShipCount
		{
			get
			{
				if (KCDatabase.Instance.Battle != null)
					return KCDatabase.Instance.Ships.Count + KCDatabase.Instance.Battle.DroppedShipCount;

				return KCDatabase.Instance.Ships.Count;
			}
		}

		private int RealEquipmentCount
		{
			get
			{
				if (KCDatabase.Instance.Battle != null)
					return KCDatabase.Instance.Equipments.Count + KCDatabase.Instance.Battle.DroppedEquipmentCount;

				return KCDatabase.Instance.Equipments.Count;
			}
		}


		protected override string GetPersistString()
		{
			return "HeadQuarters";
		}


	}

}
