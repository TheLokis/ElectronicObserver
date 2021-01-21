using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
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

	public partial class FormFleetOverview : DockContent
	{

		private class TableFleetControl : IDisposable
		{

			public ImageLabel Number;
			public FleetState State;
			public ToolTip ToolTipInfo;
			private int fleetID;

			public TableFleetControl(FormFleetOverview parent, int fleetID)
			{

                #region Initialize

                this.Number = new ImageLabel
				{
					Anchor = AnchorStyles.Left,
					ImageAlign = ContentAlignment.MiddleCenter,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					Text = $"#{fleetID}:",
					AutoSize = true
				};

                this.State = new FleetState
				{
					Anchor = AnchorStyles.Left,
					Padding = new Padding(),
					Margin = new Padding(),
					AutoSize = true
				};

                this.ConfigurationChanged(parent);

				this.fleetID = fleetID;
                this.ToolTipInfo = parent.ToolTipInfo;

				#endregion

			}

			public TableFleetControl(FormFleetOverview parent, int fleetID, TableLayoutPanel table)
				: this(parent, fleetID)
			{

                this.AddToTable(table, fleetID - 1);
			}

			public void AddToTable(TableLayoutPanel table, int row)
			{

				table.Controls.Add(this.Number, 0, row);
				table.Controls.Add(this.State, 1, row);

			}


			public void Update()
			{

				FleetData fleet = KCDatabase.Instance.Fleet[this.fleetID];
				if (fleet == null) return;

                this.State.UpdateFleetState(fleet, this.ToolTipInfo);

                this.ToolTipInfo.SetToolTip(this.Number, fleet.Name);
			}


			public void Refresh()
			{

                this.State.RefreshFleetState();
			}


			public void ConfigurationChanged(FormFleetOverview parent)
			{
                this.Number.Font = parent.Font;
                this.State.Font = parent.Font;
                this.State.BackColor = Color.Transparent;
                this.Update();
			}

			public void Dispose()
			{
                this.Number.Dispose();
                this.State.Dispose();
			}
		}


		private List<TableFleetControl> ControlFleet;
		private ImageLabel CombinedTag;
		private ImageLabel AnchorageRepairingTimer;


		public FormFleetOverview(FormMain parent)
		{
            this.InitializeComponent();

			ControlHelper.SetDoubleBuffered(this.TableFleet);


            this.ControlFleet = new List<TableFleetControl>(4);
			for (int i = 0; i < 4; i++)
			{
                this.ControlFleet.Add(new TableFleetControl(this, i + 1, this.TableFleet));
			}

			{
                this.AnchorageRepairingTimer = new ImageLabel
				{
					Anchor = AnchorStyles.Left,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					ImageList = ResourceManager.Instance.Icons,
					ImageIndex = (int)ResourceManager.IconContent.FleetAnchorageRepairing,
					Text = "-",
					AutoSize = true
				};
                //AnchorageRepairingTimer.Visible = false;

                this.TableFleet.Controls.Add(this.AnchorageRepairingTimer, 1, 4);

			}

			#region CombinedTag
			{
                this.CombinedTag = new ImageLabel
				{
					Anchor = AnchorStyles.Left,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					ImageList = ResourceManager.Instance.Icons,
					ImageIndex = (int)ResourceManager.IconContent.FleetCombined,
					Text = "-",
					AutoSize = true,
					Visible = false
				};

                this.TableFleet.Controls.Add(this.CombinedTag, 1, 5);

			}
            #endregion



            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleet]);

			Utility.SystemEvents.UpdateTimerTick += this.UpdateTimerTick;
		}



		private void FormFleetOverview_Load(object sender, EventArgs e)
		{

			//api register
			APIObserver o = APIObserver.Instance;

            o["api_req_nyukyo/start"].RequestReceived += this.Updated;
            o["api_req_nyukyo/speedchange"].RequestReceived += this.Updated;
            o["api_req_hensei/change"].RequestReceived += this.Updated;
            o["api_req_kousyou/destroyship"].RequestReceived += this.Updated;
            o["api_req_member/updatedeckname"].RequestReceived += this.Updated;
            o["api_req_map/start"].RequestReceived += this.Updated;
            o["api_req_hensei/combined"].RequestReceived += this.Updated;
            o["api_req_kaisou/open_exslot"].RequestReceived += this.Updated;

            o["api_port/port"].ResponseReceived += this.Updated;
            o["api_get_member/ship2"].ResponseReceived += this.Updated;
            o["api_get_member/ndock"].ResponseReceived += this.Updated;
            o["api_req_kousyou/getship"].ResponseReceived += this.Updated;
            o["api_req_hokyu/charge"].ResponseReceived += this.Updated;
            o["api_req_kousyou/destroyship"].ResponseReceived += this.Updated;
            o["api_get_member/ship3"].ResponseReceived += this.Updated;
            o["api_req_kaisou/powerup"].ResponseReceived += this.Updated;        //requestのほうは面倒なのでこちらでまとめてやる
            o["api_get_member/deck"].ResponseReceived += this.Updated;
            o["api_req_map/start"].ResponseReceived += this.Updated;
            o["api_req_map/next"].ResponseReceived += this.Updated;
            o["api_get_member/ship_deck"].ResponseReceived += this.Updated;
            o["api_req_hensei/preset_select"].ResponseReceived += this.Updated;
            o["api_req_kaisou/slot_exchange_index"].ResponseReceived += this.Updated;
            o["api_get_member/require_info"].ResponseReceived += this.Updated;
            o["api_req_kaisou/slot_deprive"].ResponseReceived += this.Updated;
            o["api_req_kaisou/marriage"].ResponseReceived += this.Updated;
            o["api_req_map/anchorage_repair"].ResponseReceived += this.Updated;


            Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
		}

		void ConfigurationChanged()
		{

            this.TableFleet.SuspendLayout();

            this.Font = Utility.Configuration.Config.UI.MainFont;
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

            this.AutoScroll = Utility.Configuration.Config.FormFleet.IsScrollable;

			foreach (var c in this.ControlFleet)
				c.ConfigurationChanged(this);

            this.CombinedTag.Font = this.Font;
            this.AnchorageRepairingTimer.Font = this.Font;
            this.AnchorageRepairingTimer.Visible = Utility.Configuration.Config.FormFleet.ShowAnchorageRepairingTimer;

            this.LayoutSubInformation();

            ControlHelper.SetTableRowStyles(this.TableFleet, ControlHelper.GetDefaultRowStyle());

            this.TableFleet.ResumeLayout();
		}


		private void Updated(string apiname, dynamic data)
		{

            this.TableFleet.SuspendLayout();

            this.TableFleet.RowCount = KCDatabase.Instance.Fleet.Fleets.Values.Count(f => f.IsAvailable);
			for (int i = 0; i < this.ControlFleet.Count; i++)
			{
                this.ControlFleet[i].Update();
			}

			if (KCDatabase.Instance.Fleet.CombinedFlag > 0)
			{
                this.CombinedTag.Text = Constants.GetCombinedFleet(KCDatabase.Instance.Fleet.CombinedFlag);

				var fleet1 = KCDatabase.Instance.Fleet[1];
				var fleet2 = KCDatabase.Instance.Fleet[2];

				int tp = Calculator.GetTPDamage(fleet1) + Calculator.GetTPDamage(fleet2);

				var members = fleet1.MembersWithoutEscaped.Concat(fleet2.MembersWithoutEscaped).Where(s => s != null);

				// 各艦ごとの ドラム缶 or 大発系 を搭載している個数
				var transport = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.CategoryType == EquipmentTypes.TransportContainer));
				var landing = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.CategoryType == EquipmentTypes.LandingCraft || eq?.CategoryType == EquipmentTypes.SpecialAmphibiousTank));


                this.ToolTipInfo.SetToolTip(this.CombinedTag, string.Format("드럼탑재: {0}개\r\n대발탑재: {1}개\r\n수송량(TP): S {2} / A {3}\r\n\r\n제공합계: {4}\r\n색적합계: {5:f2}\r\n신식(33):\r\n 분기점계수1: {6:f2}\r\n 분기점계수2: {7:f2}\r\n 분기점계수3: {8:f2}\r\n　분기점계수4: {9:f2}",
					transport.Sum(),
					landing.Sum(),
					tp,
					(int)Math.Floor(tp * 0.7),
					Calculator.GetAirSuperiority(fleet1) + Calculator.GetAirSuperiority(fleet2),
					Math.Floor(fleet1.GetSearchingAbility() * 100) / 100 + Math.Floor(fleet2.GetSearchingAbility() * 100) / 100,
					Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 1) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 1) * 100) / 100,
					Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 2) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 2) * 100) / 100,
					Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 3) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 3) * 100) / 100,
					Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 4) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 4) * 100) / 100
					));


                this.CombinedTag.Visible = true;
			}
			else
			{
                this.CombinedTag.Visible = false;
			}

			if (KCDatabase.Instance.Fleet.AnchorageRepairingTimer > DateTime.MinValue)
			{
                this.AnchorageRepairingTimer.Text = DateTimeHelper.ToTimeElapsedString(KCDatabase.Instance.Fleet.AnchorageRepairingTimer);
                this.AnchorageRepairingTimer.Tag = KCDatabase.Instance.Fleet.AnchorageRepairingTimer;
                this.ToolTipInfo.SetToolTip(this.AnchorageRepairingTimer, "아카시타이머\r\n시작: " + DateTimeHelper.TimeToCSVString(KCDatabase.Instance.Fleet.AnchorageRepairingTimer) + "\r\n회복: " + DateTimeHelper.TimeToCSVString(KCDatabase.Instance.Fleet.AnchorageRepairingTimer.AddMinutes(20)));
			}

            this.LayoutSubInformation();

            this.TableFleet.ResumeLayout();
		}

        void LayoutSubInformation()
        {
            if (this.CombinedTag.Visible && !this.AnchorageRepairingTimer.Visible)
            {
                if (this.TableFleet.GetPositionFromControl(this.AnchorageRepairingTimer).Row != 5)
                {
                    this.TableFleet.Controls.Remove(this.CombinedTag);
                    this.TableFleet.Controls.Remove(this.AnchorageRepairingTimer);
                    this.TableFleet.Controls.Add(this.CombinedTag, 1, 4);
                    this.TableFleet.Controls.Add(this.AnchorageRepairingTimer, 1, 5);
                }
            }
            else
            {
                if (this.TableFleet.GetPositionFromControl(this.AnchorageRepairingTimer).Row != 4)
                {
                    this.TableFleet.Controls.Remove(this.CombinedTag);
                    this.TableFleet.Controls.Remove(this.AnchorageRepairingTimer);
                    this.TableFleet.Controls.Add(this.AnchorageRepairingTimer, 1, 4);
                    this.TableFleet.Controls.Add(this.CombinedTag, 1, 5);
                }
            }
        }



        void UpdateTimerTick()
		{
			for (int i = 0; i < this.ControlFleet.Count; i++)
			{
                this.ControlFleet[i].Refresh();
			}

			if (this.AnchorageRepairingTimer.Visible && this.AnchorageRepairingTimer.Tag != null)
                this.AnchorageRepairingTimer.Text = DateTimeHelper.ToTimeElapsedString((DateTime)this.AnchorageRepairingTimer.Tag);
		}



		private void TableFleet_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

		}



		protected override string GetPersistString()
		{
			return "FleetOverview";
		}

	}

}
