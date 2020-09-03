using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Support;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window
{
	public partial class FormBaseAirCorps : DockContent
	{


		private class TableBaseAirCorpsControl : IDisposable
		{

			public ImageLabel Name;
			public ImageLabel ActionKind;
			public ImageLabel AirSuperiority;
			public ImageLabel Distance;
			public ShipStatusEquipment Squadrons;

			public ToolTip ToolTipInfo;

			public TableBaseAirCorpsControl(FormBaseAirCorps parent)
			{

                #region Initialize

                this.Name = new ImageLabel
				{
					Name = "Name",
					Text = "*",
					Anchor = AnchorStyles.Left,
					TextAlign = ContentAlignment.MiddleLeft,
					ImageAlign = ContentAlignment.MiddleRight,
					ImageList = ResourceManager.Instance.Icons,
					Padding = new Padding(2, 2, 2, 2),
					Margin = new Padding(2, 1, 2, 1),      // ここを 2,0,2,0 にすると境界線の描画に問題が出るので
					AutoSize = true,
					ContextMenuStrip = parent.ContextMenuBaseAirCorps,
					Visible = false,
					Cursor = Cursors.Help
				};

                this.ActionKind = new ImageLabel
				{
					Text = "*",
					Anchor = AnchorStyles.Left,
					TextAlign = ContentAlignment.MiddleLeft,
					ImageAlign = ContentAlignment.MiddleCenter,
					//ActionKind.ImageList =
					Padding = new Padding(2, 2, 2, 2),
					Margin = new Padding(2, 0, 2, 0),
					AutoSize = true,
					Visible = false
				};

                this.AirSuperiority = new ImageLabel
				{
					Text = "*",
					Anchor = AnchorStyles.Left,
					TextAlign = ContentAlignment.MiddleLeft,
					ImageAlign = ContentAlignment.MiddleLeft,
					ImageList = ResourceManager.Instance.Equipments,
					ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter,
					Padding = new Padding(2, 2, 2, 2),
					Margin = new Padding(2, 0, 2, 0),
					AutoSize = true,
					Visible = false
				};

                this.Distance = new ImageLabel
				{
					Text = "*",
					Anchor = AnchorStyles.Left,
					TextAlign = ContentAlignment.MiddleLeft,
					ImageAlign = ContentAlignment.MiddleLeft,
					ImageList = ResourceManager.Instance.Icons,
					ImageIndex = (int)ResourceManager.IconContent.ParameterAircraftDistance,
					Padding = new Padding(2, 2, 2, 2),
					Margin = new Padding(2, 0, 2, 0),
					AutoSize = true,
					Visible = false
				};

                this.Squadrons = new ShipStatusEquipment
				{
					Anchor = AnchorStyles.Left,
					Padding = new Padding(0, 1, 0, 2),
					Margin = new Padding(2, 0, 2, 0),
					Size = new Size(40, 20),
					AutoSize = true,
					Visible = false
				};
                this.Squadrons.ResumeLayout();

                this.ConfigurationChanged(parent);

                this.ToolTipInfo = parent.ToolTipInfo;

				#endregion

			}


			public TableBaseAirCorpsControl(FormBaseAirCorps parent, TableLayoutPanel table, int row)
				: this(parent)
			{
                this.AddToTable(table, row);
			}

			public void AddToTable(TableLayoutPanel table, int row)
			{

				table.SuspendLayout();

				table.Controls.Add(this.Name, 0, row);
				table.Controls.Add(this.ActionKind, 1, row);
				table.Controls.Add(this.AirSuperiority, 2, row);
				table.Controls.Add(this.Distance, 3, row);
				table.Controls.Add(this.Squadrons, 4, row);
				table.ResumeLayout();

				ControlHelper.SetTableRowStyle(table, row, ControlHelper.GetDefaultRowStyle());
			}


			public void Update(int baseAirCorpsID)
			{

				KCDatabase db = KCDatabase.Instance;
				var corps = db.BaseAirCorps[baseAirCorpsID];

				if (corps == null)
				{
					baseAirCorpsID = -1;

				}
				else
				{

                    this.Name.Text = string.Format("#{0} - {1}", corps.MapAreaID, corps.Name);
                    this.Name.Tag = corps.MapAreaID;
					var sb = new StringBuilder();


					string areaName = KCDatabase.Instance.MapArea.ContainsKey(corps.MapAreaID) ? KCDatabase.Instance.MapArea[corps.MapAreaID].Name : "버뮤다 해역";

					sb.AppendLine("소속해역: " + areaName);

					// state 

					if (corps.Squadrons.Values.Any(sq => sq != null && sq.Condition > 1))
					{
						// 疲労
						int tired = corps.Squadrons.Values.Max(sq => sq?.Condition ?? 0);

						if (tired == 2)
						{
                            this.Name.ImageAlign = ContentAlignment.MiddleRight;
                            this.Name.ImageIndex = (int)ResourceManager.IconContent.ConditionTired;
							sb.AppendLine("피로");

						}
						else
						{
                            this.Name.ImageAlign = ContentAlignment.MiddleRight;
                            this.Name.ImageIndex = (int)ResourceManager.IconContent.ConditionVeryTired;
							sb.AppendLine("과로");

						}

					}
					else if (corps.Squadrons.Values.Any(sq => sq != null && sq.AircraftCurrent < sq.AircraftMax))
					{
                        // 未補給
                        this.Name.ImageAlign = ContentAlignment.MiddleRight;
                        this.Name.ImageIndex = (int)ResourceManager.IconContent.FleetNotReplenished;
						sb.AppendLine("미보급");

					}
					else
					{
                        this.Name.ImageAlign = ContentAlignment.MiddleCenter;
                        this.Name.ImageIndex = -1;

					}

                    sb.AppendLine(string.Format("총 제공: 방공 {0} / 대고고도 {1}",
                db.BaseAirCorps.Values.Where(c => c.MapAreaID == corps.MapAreaID && c.ActionKind == 2).Select(c => Calculator.GetAirSuperiority(c)).DefaultIfEmpty(0).Sum(),
                db.BaseAirCorps.Values.Where(c => c.MapAreaID == corps.MapAreaID && c.ActionKind == 2).Select(c => Calculator.GetAirSuperiority(c, isHighAltitude: true)).DefaultIfEmpty(0).Sum()
                ));

                    this.ToolTipInfo.SetToolTip(this.Name, sb.ToString());


                    this.ActionKind.Text = "[" + Constants.GetBaseAirCorpsActionKind(corps.ActionKind) + "]";

					{
						int airSuperiority = Calculator.GetAirSuperiority(corps);
						if (Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange)
						{
							int airSuperiority_max = Calculator.GetAirSuperiority(corps, true);
							if (airSuperiority < airSuperiority_max)
                                this.AirSuperiority.Text = string.Format("{0} ～ {1}", airSuperiority, airSuperiority_max);
							else
                                this.AirSuperiority.Text = airSuperiority.ToString();
						}
						else
						{
                            this.AirSuperiority.Text = airSuperiority.ToString();
						}

                        var tip = new StringBuilder();
                        tip.AppendFormat(string.Format("확보: {0}\r\n우세: {1}\r\n균등: {2}\r\n열세: {3}\r\n",
							(int)(airSuperiority / 3.0),
							(int)(airSuperiority / 1.5),
							Math.Max((int)(airSuperiority * 1.5 - 1), 0),
							Math.Max((int)(airSuperiority * 3.0 - 1), 0)));

                        if (corps.ActionKind == 2)
                        {
                            int airSuperiorityHighAltitude = Calculator.GetAirSuperiority(corps, isHighAltitude: true);
                            tip.AppendFormat("\r\n대고고도폭격：제공 {0}\r\n확보: {1}\r\n우세: {2}\r\n균형: {3}\r\n열세: {4}\r\n",
                                airSuperiorityHighAltitude,
                                (int)(airSuperiorityHighAltitude / 3.0),
                                (int)(airSuperiorityHighAltitude / 1.5),
                                Math.Max((int)(airSuperiorityHighAltitude * 1.5 - 1), 0),
                                Math.Max((int)(airSuperiorityHighAltitude * 3.0 - 1), 0));
                        }

                        this.ToolTipInfo.SetToolTip(this.AirSuperiority, tip.ToString());
                    }

                    this.Distance.Text = corps.Distance.ToString();

                    this.Squadrons.SetSlotList(corps);
                    this.ToolTipInfo.SetToolTip(this.Squadrons, this.GetEquipmentString(corps));

				}


                this.Name.Visible =
                this.ActionKind.Visible =
                this.AirSuperiority.Visible =
                this.Distance.Visible =
                this.Squadrons.Visible =
					baseAirCorpsID != -1;
			}


			public void ConfigurationChanged(FormBaseAirCorps parent)
			{

				var config = Utility.Configuration.Config;

				var mainfont = config.UI.MainFont;
				var subfont = config.UI.SubFont;



                this.Name.Font = mainfont;
                this.ActionKind.Font = mainfont;
                this.AirSuperiority.Font = mainfont;
                this.Distance.Font = mainfont;
                this.Squadrons.Font = subfont;

                this.Squadrons.ShowAircraft = config.FormFleet.ShowAircraft;
                this.Squadrons.ShowAircraftLevelByNumber = config.FormFleet.ShowAircraftLevelByNumber;
                this.Squadrons.LevelVisibility = config.FormFleet.EquipmentLevelVisibility;

			}


			private string GetEquipmentString(BaseAirCorpsData corps)
			{
				var sb = new StringBuilder();

				if (corps == null)
					return "(미개방)\r\n";

				foreach (var squadron in corps.Squadrons.Values)
				{
					if (squadron == null)
						continue;

					var eq = squadron.EquipmentInstance;

					switch (squadron.State)
					{
						case 0:     // 未配属
						default:
							sb.AppendLine("(없음)");
							break;

						case 1:     // 配属済み
							if (eq == null)
								goto case 0;
							sb.AppendFormat("[{0}/{1}] ",
								squadron.AircraftCurrent,
								squadron.AircraftMax);

							switch (squadron.Condition)
							{
								case 1:
								default:
									break;
								case 2:
									sb.Append("[피로] ");
									break;
								case 3:
									sb.Append("[과로] ");
									break;
							}

							sb.AppendFormat("{0} (반경: {1})\r\n", eq.NameWithLevel, eq.MasterEquipment.AircraftDistance);
							break;

						case 2:     // 配置転換中
							sb.AppendFormat("배치전환중 (시작시간: {0})\r\n",
								DateTimeHelper.TimeToCSVString(squadron.RelocatedTime));
							break;
					}
				}

				return sb.ToString();
			}

			public void Dispose()
			{
                this.Name.Dispose();
                this.ActionKind.Dispose();
                this.AirSuperiority.Dispose();
                this.Distance.Dispose();
                this.Squadrons.Dispose();
			}
		}


		private TableBaseAirCorpsControl[] ControlMember;

		public FormBaseAirCorps(FormMain parent)
		{
            this.InitializeComponent();


            this.ControlMember = new TableBaseAirCorpsControl[9];
            this.TableMember.SuspendLayout();
			for (int i = 0; i < this.ControlMember.Length; i++)
			{
                this.ControlMember[i] = new TableBaseAirCorpsControl(this, this.TableMember, i);
			}
            this.TableMember.ResumeLayout();

            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBaseAirCorps]);
		}

		private void FormBaseAirCorps_Load(object sender, EventArgs e)
		{

			var api = Observer.APIObserver.Instance;

			api["api_port/port"].ResponseReceived += this.Updated;
			api["api_get_member/mapinfo"].ResponseReceived += this.Updated;
			api["api_get_member/base_air_corps"].ResponseReceived += this.Updated;
			api["api_req_air_corps/change_name"].ResponseReceived += this.Updated;
			api["api_req_air_corps/set_action"].ResponseReceived += this.Updated;
			api["api_req_air_corps/set_plane"].ResponseReceived += this.Updated;
			api["api_req_air_corps/supply"].ResponseReceived += this.Updated;
			api["api_req_air_corps/expand_base"].ResponseReceived += this.Updated;

			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

		}


		private void ConfigurationChanged()
		{

			var c = Utility.Configuration.Config;

            this.TableMember.SuspendLayout();

            this.Font = c.UI.MainFont;

			foreach (var control in this.ControlMember)
				control.ConfigurationChanged(this);

			ControlHelper.SetTableRowStyles(this.TableMember, ControlHelper.GetDefaultRowStyle());

            this.TableMember.ResumeLayout();

            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);

            if (KCDatabase.Instance.BaseAirCorps.Any())
                this.Updated(null, null);
		}


		void Updated(string apiname, dynamic data)
		{

			var keys = KCDatabase.Instance.BaseAirCorps.Keys;

			if (Utility.Configuration.Config.FormBaseAirCorps.ShowEventMapOnly)
			{
				var eventAreaCorps = KCDatabase.Instance.BaseAirCorps.Values.Where(b =>
				{
					var maparea = KCDatabase.Instance.MapArea[b.MapAreaID];
					return maparea != null && maparea.MapType == 1;
				}).Select(b => b.ID);

				if (eventAreaCorps.Any())
					keys = eventAreaCorps;
			}


            this.TableMember.SuspendLayout();
            this.TableMember.RowCount = keys.Count();
			for (int i = 0; i < this.ControlMember.Length; i++)
			{
                this.ControlMember[i].Update(i < keys.Count() ? keys.ElementAt(i) : -1);
			}
            this.TableMember.ResumeLayout();

			// set icon
			{
				var squadrons = KCDatabase.Instance.BaseAirCorps.Values.Where(b => b != null)
					.SelectMany(b => b.Squadrons.Values)
					.Where(s => s != null);
				bool isNotReplenished = squadrons.Any(s => s.State == 1 && s.AircraftCurrent < s.AircraftMax);
				bool isTired = squadrons.Any(s => s.State == 1 && s.Condition == 2);
				bool isVeryTired = squadrons.Any(s => s.State == 1 && s.Condition == 3);

				int imageIndex;

				if (isNotReplenished)
					imageIndex = (int)ResourceManager.IconContent.FleetNotReplenished;
				else if (isVeryTired)
					imageIndex = (int)ResourceManager.IconContent.ConditionVeryTired;
				else if (isTired)
					imageIndex = (int)ResourceManager.IconContent.ConditionTired;
				else
					imageIndex = (int)ResourceManager.IconContent.FormBaseAirCorps;

				if (this.Icon != null) ResourceManager.DestroyIcon(this.Icon);
                this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[imageIndex]);
				if (this.Parent != null) this.Parent.Refresh();       //アイコンを更新するため
			}

		}


		private void ContextMenuBaseAirCorps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (KCDatabase.Instance.BaseAirCorps.Count == 0)
			{
				e.Cancel = true;
				return;
			}

			if (this.ContextMenuBaseAirCorps.SourceControl.Name == "Name")
                this.ContextMenuBaseAirCorps_CopyOrganization.Tag = this.ContextMenuBaseAirCorps.SourceControl.Tag as int? ?? -1;
			else
                this.ContextMenuBaseAirCorps_CopyOrganization.Tag = -1;
		}

		private void ContextMenuBaseAirCorps_CopyOrganization_Click(object sender, EventArgs e)
		{

			var sb = new StringBuilder();
			int areaid = this.ContextMenuBaseAirCorps_CopyOrganization.Tag as int? ?? -1;

			var baseaircorps = KCDatabase.Instance.BaseAirCorps.Values;
			if (areaid != -1)
				baseaircorps = baseaircorps.Where(c => c.MapAreaID == areaid);

			foreach (var corps in baseaircorps)
			{

				string areaName = KCDatabase.Instance.MapArea.ContainsKey(corps.MapAreaID) ? KCDatabase.Instance.MapArea[corps.MapAreaID].Name : "バミューダ海域";

				sb.AppendFormat("{0}\t[{1}] 제공치{2}/행동반경{3}\r\n",
					(areaid == -1 ? (areaName + "：") : "") + corps.Name,
					Constants.GetBaseAirCorpsActionKind(corps.ActionKind),
					Calculator.GetAirSuperiority(corps),
					corps.Distance);

				var sq = corps.Squadrons.Values.ToArray();

				for (int i = 0; i < sq.Length; i++)
				{
					if (i > 0)
						sb.Append(", ");

					if (sq[i] == null)
					{
						sb.Append("(소식불명)");
						continue;
					}

					switch (sq[i].State)
					{
						case 0:
							sb.Append("(미배속)");
							break;
						case 1:
							{
								var eq = sq[i].EquipmentInstance;

								sb.Append(eq?.NameWithLevel ?? "(없음)");

								if (sq[i].AircraftCurrent < sq[i].AircraftMax)
									sb.AppendFormat("[{0}/{1}]", sq[i].AircraftCurrent, sq[i].AircraftMax);
							}
							break;
						case 2:
							sb.Append("(배치전환중)");
							break;
					}
				}

				sb.AppendLine();
			}

			Clipboard.SetData(DataFormats.StringFormat, sb.ToString());
		}

		private void ContextMenuBaseAirCorps_DisplayRelocatedEquipments_Click(object sender, EventArgs e)
		{

			string message = string.Join("\r\n", KCDatabase.Instance.RelocatedEquipments.Values
				.Where(eq => eq.EquipmentInstance != null)
				.Select(eq => string.Format("{0} ({1}～)", eq.EquipmentInstance.NameWithLevel, DateTimeHelper.TimeToCSVString(eq.RelocatedTime))));

			if (message.Length == 0)
				message = "현재 배치전환중인 장비가 없습니다.";

			MessageBox.Show(message, "배치전환중장비", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}


		private void TableMember_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		protected override string GetPersistString()
		{
			return "BaseAirCorps";
		}




	}
}
