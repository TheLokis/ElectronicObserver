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
			InitializeComponent();

			_parentForm = parent;


			ImageList icons = ResourceManager.Instance.Icons;

			ShipCount.ImageList = icons;
			ShipCount.ImageIndex = (int)ResourceManager.IconContent.HeadQuartersShip;
			EquipmentCount.ImageList = icons;
			EquipmentCount.ImageIndex = (int)ResourceManager.IconContent.HeadQuartersEquipment;
			InstantRepair.ImageList = icons;
			InstantRepair.ImageIndex = (int)ResourceManager.IconContent.ItemInstantRepair;
			InstantConstruction.ImageList = icons;
			InstantConstruction.ImageIndex = (int)ResourceManager.IconContent.ItemInstantConstruction;
			DevelopmentMaterial.ImageList = icons;
			DevelopmentMaterial.ImageIndex = (int)ResourceManager.IconContent.ItemDevelopmentMaterial;
			ModdingMaterial.ImageList = icons;
			ModdingMaterial.ImageIndex = (int)ResourceManager.IconContent.ItemModdingMaterial;
			FurnitureCoin.ImageList = icons;
			FurnitureCoin.ImageIndex = (int)ResourceManager.IconContent.ItemFurnitureCoin;
			Fuel.ImageList = icons;
			Fuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
			Ammo.ImageList = icons;
			Ammo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
			Steel.ImageList = icons;
			Steel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
			Bauxite.ImageList = icons;
			Bauxite.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;
			DisplayUseItem.ImageList = icons;
			DisplayUseItem.ImageIndex = (int)ResourceManager.IconContent.ItemPresentBox;


			ControlHelper.SetDoubleBuffered(FlowPanelMaster);
			ControlHelper.SetDoubleBuffered(FlowPanelAdmiral);
			ControlHelper.SetDoubleBuffered(FlowPanelFleet);
			ControlHelper.SetDoubleBuffered(FlowPanelUseItem);
			ControlHelper.SetDoubleBuffered(FlowPanelResource);


			ConfigurationChanged();

			Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormHeadQuarters]);

		}


		private void FormHeadquarters_Load(object sender, EventArgs e)
		{

			APIObserver o = APIObserver.Instance;

			o.APIList["api_req_nyukyo/start"].RequestReceived += Updated;
			o.APIList["api_req_nyukyo/speedchange"].RequestReceived += Updated;
			o.APIList["api_req_kousyou/createship"].RequestReceived += Updated;
			o.APIList["api_req_kousyou/createship_speedchange"].RequestReceived += Updated;
			o.APIList["api_req_kousyou/destroyship"].RequestReceived += Updated;
			o.APIList["api_req_kousyou/destroyitem2"].RequestReceived += Updated;
			o.APIList["api_req_member/updatecomment"].RequestReceived += Updated;

			o.APIList["api_get_member/basic"].ResponseReceived += Updated;
			o.APIList["api_get_member/slot_item"].ResponseReceived += Updated;
			o.APIList["api_port/port"].ResponseReceived += Updated;
			o.APIList["api_get_member/ship2"].ResponseReceived += Updated;
			o.APIList["api_req_kousyou/getship"].ResponseReceived += Updated;
			o.APIList["api_req_hokyu/charge"].ResponseReceived += Updated;
			o.APIList["api_req_kousyou/destroyship"].ResponseReceived += Updated;
			o.APIList["api_req_kousyou/destroyitem2"].ResponseReceived += Updated;
			o.APIList["api_req_kaisou/powerup"].ResponseReceived += Updated;
			o.APIList["api_req_kousyou/createitem"].ResponseReceived += Updated;
			o.APIList["api_req_kousyou/remodel_slot"].ResponseReceived += Updated;
			o.APIList["api_get_member/material"].ResponseReceived += Updated;
			o.APIList["api_get_member/ship_deck"].ResponseReceived += Updated;
			o.APIList["api_req_air_corps/set_plane"].ResponseReceived += Updated;
			o.APIList["api_req_air_corps/supply"].ResponseReceived += Updated;
			o.APIList["api_get_member/useitem"].ResponseReceived += Updated;


			Utility.Configuration.Instance.ConfigurationChanged += ConfigurationChanged;
			Utility.SystemEvents.UpdateTimerTick += SystemEvents_UpdateTimerTick;

			FlowPanelResource.SetFlowBreak(Ammo, true);

			FlowPanelMaster.Visible = false;

		}



		void ConfigurationChanged()
		{

			Font = FlowPanelMaster.Font = Utility.Configuration.Config.UI.MainFont;
			HQLevel.MainFont = Utility.Configuration.Config.UI.MainFont;
			HQLevel.SubFont = Utility.Configuration.Config.UI.SubFont;

            BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);
            ForeColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            HQLevel.MainFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            HQLevel.SubFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.SubFontColor);

            // 点滅しない設定にしたときに消灯状態で固定されるのを防ぐ
            if (!Utility.Configuration.Config.FormHeadquarters.BlinkAtMaximum)
			{
                if (ShipCount.Tag as bool? ?? false)
                {
                    ShipCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }

                if (EquipmentCount.Tag as bool? ?? false)
                {
                    EquipmentCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }
            }

			//visibility
			CheckVisibilityConfiguration();
			{
				var visibility = Utility.Configuration.Config.FormHeadquarters.Visibility.List;
				AdmiralName.Visible = visibility[0];
				AdmiralComment.Visible = visibility[1];
				HQLevel.Visible = visibility[2];
				ShipCount.Visible = visibility[3];
				EquipmentCount.Visible = visibility[4];
				InstantRepair.Visible = visibility[5];
				InstantConstruction.Visible = visibility[6];
				DevelopmentMaterial.Visible = visibility[7];
				ModdingMaterial.Visible = visibility[8];
				FurnitureCoin.Visible = visibility[9];
				Fuel.Visible = visibility[10];
				Ammo.Visible = visibility[11];
				Steel.Visible = visibility[12];
				Bauxite.Visible = visibility[13];
				DisplayUseItem.Visible = visibility[14];
			}

			UpdateDisplayUseItem();
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



            FlowPanelMaster.SuspendLayout();

			//Admiral
			FlowPanelAdmiral.SuspendLayout();
			AdmiralName.Text = string.Format("{0} {1}", db.Admiral.AdmiralName, Constants.GetAdmiralRank(db.Admiral.Rank));
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

				ToolTipInfo.SetToolTip(AdmiralName, tooltip.ToString());
			}
			AdmiralComment.Text = db.Admiral.Comment;
			FlowPanelAdmiral.ResumeLayout();

			//HQ Level
			HQLevel.Value = db.Admiral.Level;
			{
				StringBuilder tooltip = new StringBuilder();
				if (db.Admiral.Level < ExpTable.AdmiralExp.Max(e => e.Key))
				{
					HQLevel.TextNext = "next:";
					HQLevel.ValueNext = ExpTable.GetNextExpAdmiral(db.Admiral.Exp);
					tooltip.AppendFormat("{0} / {1}\r\n", db.Admiral.Exp, ExpTable.AdmiralExp[db.Admiral.Level + 1].Total);
				}
				else
				{
					HQLevel.TextNext = "exp:";
					HQLevel.ValueNext = db.Admiral.Exp;
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

				ToolTipInfo.SetToolTip(HQLevel, tooltip.ToString());
			}

			//Fleet
			FlowPanelFleet.SuspendLayout();
			{

				ShipCount.Text = string.Format("{0}/{1}", RealShipCount, db.Admiral.MaxShipCount);
				if (RealShipCount > db.Admiral.MaxShipCount - 5)
				{
                    ShipCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }
				else
				{
					ShipCount.BackColor = Color.Transparent;
				}
				ShipCount.Tag = RealShipCount >= db.Admiral.MaxShipCount;

				EquipmentCount.Text = string.Format("{0}/{1}", RealEquipmentCount, db.Admiral.MaxEquipmentCount);
				if (RealEquipmentCount > db.Admiral.MaxEquipmentCount + 3 - 20)
				{
                    EquipmentCount.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight);
                }
				else
				{
					EquipmentCount.BackColor = Color.Transparent;
				}
				EquipmentCount.Tag = RealEquipmentCount >= db.Admiral.MaxEquipmentCount;

			}
			FlowPanelFleet.ResumeLayout();



			var resday = RecordManager.Instance.Resource.GetRecord(DateTime.Now.AddHours(-5).Date.AddHours(5));
			var resweek = RecordManager.Instance.Resource.GetRecord(DateTime.Now.AddHours(-5).Date.AddDays(-(((int)DateTime.Now.AddHours(-5).DayOfWeek + 6) % 7)).AddHours(5)); //月曜日起点
			var resmonth = RecordManager.Instance.Resource.GetRecord(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddHours(5));


			//UseItems
			FlowPanelUseItem.SuspendLayout();

			InstantRepair.Text = db.Material.InstantRepair.ToString();
			InstantRepair.BackColor = db.Material.InstantRepair >= 3000 ? overcolor : Color.Transparent;
			ToolTipInfo.SetToolTip(InstantRepair, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.InstantRepair - resday.InstantRepair),
					resweek == null ? 0 : (db.Material.InstantRepair - resweek.InstantRepair),
					resmonth == null ? 0 : (db.Material.InstantRepair - resmonth.InstantRepair)));

			InstantConstruction.Text = db.Material.InstantConstruction.ToString();
			InstantConstruction.BackColor = db.Material.InstantConstruction >= 3000 ? overcolor : Color.Transparent;
			ToolTipInfo.SetToolTip(InstantConstruction, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.InstantConstruction - resday.InstantConstruction),
					resweek == null ? 0 : (db.Material.InstantConstruction - resweek.InstantConstruction),
					resmonth == null ? 0 : (db.Material.InstantConstruction - resmonth.InstantConstruction)));

			DevelopmentMaterial.Text = db.Material.DevelopmentMaterial.ToString();
			DevelopmentMaterial.BackColor = db.Material.DevelopmentMaterial >= 3000 ? overcolor : Color.Transparent;
			ToolTipInfo.SetToolTip(DevelopmentMaterial, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.DevelopmentMaterial - resday.DevelopmentMaterial),
					resweek == null ? 0 : (db.Material.DevelopmentMaterial - resweek.DevelopmentMaterial),
					resmonth == null ? 0 : (db.Material.DevelopmentMaterial - resmonth.DevelopmentMaterial)));

			ModdingMaterial.Text = db.Material.ModdingMaterial.ToString();
			ModdingMaterial.BackColor = db.Material.ModdingMaterial >= 3000 ? overcolor : Color.Transparent;
			ToolTipInfo.SetToolTip(ModdingMaterial, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.ModdingMaterial - resday.ModdingMaterial),
					resweek == null ? 0 : (db.Material.ModdingMaterial - resweek.ModdingMaterial),
					resmonth == null ? 0 : (db.Material.ModdingMaterial - resmonth.ModdingMaterial)));

			FurnitureCoin.Text = db.Admiral.FurnitureCoin.ToString();
			FurnitureCoin.BackColor = db.Admiral.FurnitureCoin >= 200000 ? overcolor : Color.Transparent;
			{
				int small = db.UseItems[10]?.Count ?? 0;
				int medium = db.UseItems[11]?.Count ?? 0;
				int large = db.UseItems[12]?.Count ?? 0;

				ToolTipInfo.SetToolTip(FurnitureCoin,
						string.Format("(소) x {0} ( +{1} )\r\n(중) x {2} ( +{3} )\r\n(대) x {4} ( +{5} )\r\n",
							small, small * 200,
							medium, medium * 400,
							large, large * 700));
			}
			UpdateDisplayUseItem();
			FlowPanelUseItem.ResumeLayout();


			//Resources
			FlowPanelResource.SuspendLayout();
			{

				Fuel.Text = db.Material.Fuel.ToString();
				Fuel.BackColor = db.Material.Fuel < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
				ToolTipInfo.SetToolTip(Fuel, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Fuel - resday.Fuel),
					resweek == null ? 0 : (db.Material.Fuel - resweek.Fuel),
					resmonth == null ? 0 : (db.Material.Fuel - resmonth.Fuel)));

				Ammo.Text = db.Material.Ammo.ToString();
				Ammo.BackColor = db.Material.Ammo < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
				ToolTipInfo.SetToolTip(Ammo, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Ammo - resday.Ammo),
					resweek == null ? 0 : (db.Material.Ammo - resweek.Ammo),
					resmonth == null ? 0 : (db.Material.Ammo - resmonth.Ammo)));

				Steel.Text = db.Material.Steel.ToString();
				Steel.BackColor = db.Material.Steel < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
				ToolTipInfo.SetToolTip(Steel, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Steel - resday.Steel),
					resweek == null ? 0 : (db.Material.Steel - resweek.Steel),
					resmonth == null ? 0 : (db.Material.Steel - resmonth.Steel)));

				Bauxite.Text = db.Material.Bauxite.ToString();
				Bauxite.BackColor = db.Material.Bauxite < db.Admiral.MaxResourceRegenerationAmount ? Color.Transparent : overcolor;
				ToolTipInfo.SetToolTip(Bauxite, string.Format("오늘: {0:+##;-##;±0}\n이번주: {1:+##;-##;±0}\n이달: {2:+##;-##;±0}",
					resday == null ? 0 : (db.Material.Bauxite - resday.Bauxite),
					resweek == null ? 0 : (db.Material.Bauxite - resweek.Bauxite),
					resmonth == null ? 0 : (db.Material.Bauxite - resmonth.Bauxite)));

			}
			FlowPanelResource.ResumeLayout();

			FlowPanelMaster.ResumeLayout();
			if (!FlowPanelMaster.Visible)
				FlowPanelMaster.Visible = true;
			AdmiralName.Refresh();
			AdmiralComment.Refresh();

		}


		void SystemEvents_UpdateTimerTick()
		{

			KCDatabase db = KCDatabase.Instance;

			if (db.Ships.Count <= 0) return;

			if (Utility.Configuration.Config.FormHeadquarters.BlinkAtMaximum)
			{
                if (ShipCount.Tag as bool? ?? false)
                {
                    ShipCount.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight) : Color.Transparent;
                }

                if (EquipmentCount.Tag as bool? ?? false)
                {
                    EquipmentCount.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.RedHighlight) : Color.Transparent;
                }
            }
		}


		private void Resource_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				new Dialog.DialogResourceChart().Show(_parentForm);
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
			string tail = "\r\n(설정에서 변경 가능)";



			switch (itemMaster?.Name)
			{
				case null:
					DisplayUseItem.Text = "???";
					ToolTipInfo.SetToolTip(DisplayUseItem, "알수없는항목 (ID: " + Utility.Configuration.Config.FormHeadquarters.DisplayUseItemID + ")" + tail);
					break;

				// '18 spring event special mode
				case "쌀":
				case "매실":
				case "김":
				case "차":
					DisplayUseItem.Text = (item?.Count ?? 0).ToString();
					ToolTipInfo.SetToolTip(DisplayUseItem,
						$"쌀: {db.UseItems[85]?.Count ?? 0}\r\n매실: {db.UseItems[86]?.Count ?? 0}\r\n김: {db.UseItems[87]?.Count ?? 0}\r\n차: {db.UseItems[88]?.Count ?? 0}\r\n{tail}");
					break;

				default:
					DisplayUseItem.Text = (item?.Count ?? 0).ToString();
					ToolTipInfo.SetToolTip(DisplayUseItem,
						itemMaster.Name + tail);
					break;
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
