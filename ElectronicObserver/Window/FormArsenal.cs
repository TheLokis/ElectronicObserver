using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
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

	public partial class FormArsenal : DockContent
	{

		private class TableArsenalControl : IDisposable
		{

			public Label ShipName;
			public Label CompletionTime;
			private ToolTip tooltip;

			public TableArsenalControl(FormArsenal parent)
			{

                #region Initialize

                this.ShipName = new ImageLabel
				{
					Text = "???",
					Anchor = AnchorStyles.Left,
					ForeColor = parent.ForeColor,
					TextAlign = ContentAlignment.MiddleLeft,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					MaximumSize = new Size(60, int.MaxValue),
					//ShipName.AutoEllipsis = true;
					ImageAlign = ContentAlignment.MiddleCenter,
					AutoSize = true,
					Visible = true
				};

                this.CompletionTime = new Label
				{
					Text = "",
					Anchor = AnchorStyles.Left,
					ForeColor = parent.ForeColor,
					Tag = null,
					TextAlign = ContentAlignment.MiddleLeft,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					MinimumSize = new Size(60, 10),
					AutoSize = true,
					Visible = true
				};

                this.ConfigurationChanged(parent);

                this.tooltip = parent.ToolTipInfo;
				#endregion

			}


			public TableArsenalControl(FormArsenal parent, TableLayoutPanel table, int row)
				: this(parent)
			{

                this.AddToTable(table, row);
			}


			public void AddToTable(TableLayoutPanel table, int row)
			{

				table.Controls.Add(this.ShipName, 0, row);
				table.Controls.Add(this.CompletionTime, 1, row);

			}


			public void Update(int arsenalID)
			{

				KCDatabase db = KCDatabase.Instance;
				ArsenalData arsenal = db.Arsenals[arsenalID];
				bool showShipName = Utility.Configuration.Config.FormArsenal.ShowShipName;

                this.CompletionTime.BackColor = Color.Transparent;
                this.tooltip.SetToolTip(this.ShipName, null);
                this.tooltip.SetToolTip(this.CompletionTime, null);

				if (arsenal == null || arsenal.State == -1)
				{
                    //locked
                    this.ShipName.Text = "";
                    this.CompletionTime.Text = "";
                    this.CompletionTime.Tag = null;

				}
				else if (arsenal.State == 0)
				{
                    //empty
                    this.ShipName.Text = "----";
                    this.CompletionTime.Text = "";
                    this.CompletionTime.Tag = null;

				}
				else if (arsenal.State == 2)
				{
					//building
					string name = showShipName ? db.MasterShips[arsenal.ShipID].Name : "???";
                    this.ShipName.Text = name;
                    this.tooltip.SetToolTip(this.ShipName, name);
                    this.CompletionTime.Text = DateTimeHelper.ToTimeRemainString(arsenal.CompletionTime);
                    this.CompletionTime.Tag = arsenal.CompletionTime;
                    this.tooltip.SetToolTip(this.CompletionTime, "완료시간 : " + DateTimeHelper.TimeToCSVString(arsenal.CompletionTime));

				}
				else if (arsenal.State == 3)
				{
					//complete!
					string name = showShipName ? db.MasterShips[arsenal.ShipID].Name : "???";
                    this.ShipName.Text = name;
                    this.tooltip.SetToolTip(this.ShipName, name);
                    this.CompletionTime.Text = "완성！";
                    this.CompletionTime.Tag = null;

				}

			}


			public void Refresh(int arsenalID)
			{

				if (this.CompletionTime.Tag != null)
				{

					var time = (DateTime)this.CompletionTime.Tag;

                    this.CompletionTime.Text = DateTimeHelper.ToTimeRemainString(time);

					if (Utility.Configuration.Config.FormArsenal.BlinkAtCompletion && (time - DateTime.Now).TotalMilliseconds <= Utility.Configuration.Config.NotifierConstruction.AccelInterval)
					{
                        this.CompletionTime.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.ThemeColors.GreenHighlight) : Color.Transparent;
					}

				}
				else if (Utility.Configuration.Config.FormArsenal.BlinkAtCompletion && !string.IsNullOrWhiteSpace(this.CompletionTime.Text))
				{
                    //完成しているので
                    this.CompletionTime.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.ThemeColors.GreenHighlight) : Color.Transparent;
				}
			}


			public void ConfigurationChanged(FormArsenal parent)
			{

				var config = Utility.Configuration.Config.FormArsenal;

                this.ShipName.Font = parent.Font;
                this.CompletionTime.Font = parent.Font;
                this.CompletionTime.BackColor = Color.Transparent;
                this.ShipName.MaximumSize = new Size(config.MaxShipNameWidth, this.ShipName.MaximumSize.Height);
                this.ShipName.ForeColor = parent.ForeColor;
                this.CompletionTime.ForeColor = parent.ForeColor;

            }

			public void Dispose()
			{
                this.ShipName.Dispose();
                this.CompletionTime.Dispose();
			}
		}


		private TableArsenalControl[] ControlArsenal;
		private int _buildingID;

		public FormArsenal(FormMain parent)
		{
            this.InitializeComponent();

			Utility.SystemEvents.UpdateTimerTick += this.UpdateTimerTick;

			ControlHelper.SetDoubleBuffered(this.TableArsenal);

            this.TableArsenal.SuspendLayout();
            this.ControlArsenal = new TableArsenalControl[4];
			for (int i = 0; i < this.ControlArsenal.Length; i++)
			{
                this.ControlArsenal[i] = new TableArsenalControl(this, this.TableArsenal, i);
			}
            this.TableArsenal.ResumeLayout();

            this._buildingID = -1;

            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormArsenal]);
		}



		private void FormArsenal_Load(object sender, EventArgs e)
		{

			APIObserver o = APIObserver.Instance;

			o["api_req_kousyou/createship"].RequestReceived += this.Updated;
			o["api_req_kousyou/createship_speedchange"].RequestReceived += this.Updated;

			o["api_get_member/kdock"].ResponseReceived += this.Updated;
			o["api_req_kousyou/getship"].ResponseReceived += this.Updated;
			o["api_get_member/require_info"].ResponseReceived += this.Updated;

			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

		}


		void Updated(string apiname, dynamic data)
		{

			if (this._buildingID != -1 && apiname == "api_get_member/kdock")
			{

				ArsenalData arsenal = KCDatabase.Instance.Arsenals[this._buildingID];
				ShipDataMaster ship = KCDatabase.Instance.MasterShips[arsenal.ShipID];
				string name;

				if (Utility.Configuration.Config.Log.ShowSpoiler && Utility.Configuration.Config.FormArsenal.ShowShipName)
				{

					name = string.Format("{0}「{1}」", ship.ShipTypeName, ship.NameWithClass);

				}
				else
				{

					name = "함명";
				}

				Utility.Logger.Add(2, string.Format("공창독 #{0}에 {1}의 건조를 시작했습니다. ({2}/{3}/{4}/{5}-{6} 비서함: {7})",
                    this._buildingID,
					name,
					arsenal.Fuel,
					arsenal.Ammo,
					arsenal.Steel,
					arsenal.Bauxite,
					arsenal.DevelopmentMaterial,
					KCDatabase.Instance.Fleet[1].MembersInstance[0].NameWithLevel
					));

                this._buildingID = -1;
			}

			if (apiname == "api_req_kousyou/createship")
			{
                this._buildingID = int.Parse(data["api_kdock_id"]);
			}

            this.UpdateUI();
		}

		void UpdateUI()
		{

			if (this.ControlArsenal == null) return;

            this.TableArsenal.SuspendLayout();
            this.TableArsenal.RowCount = KCDatabase.Instance.Arsenals.Values.Count(a => a.State != -1);
			for (int i = 0; i < this.ControlArsenal.Length; i++)
                this.ControlArsenal[i].Update(i + 1);
            this.TableArsenal.ResumeLayout();

		}

		void UpdateTimerTick()
		{

            this.TableArsenal.SuspendLayout();
			for (int i = 0; i < this.ControlArsenal.Length; i++)
                this.ControlArsenal[i].Refresh(i + 1);
            this.TableArsenal.ResumeLayout();

		}


		void ConfigurationChanged()
		{

            this.Font = Utility.Configuration.Config.UI.MainFont;
            this.MenuMain_ShowShipName.Checked = Utility.Configuration.Config.FormArsenal.ShowShipName;
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            if (this.ControlArsenal != null)
			{
                this.TableArsenal.SuspendLayout();

				foreach (var c in this.ControlArsenal)
					c.ConfigurationChanged(this);

				ControlHelper.SetTableRowStyles(this.TableArsenal, ControlHelper.GetDefaultRowStyle());

                this.TableArsenal.ResumeLayout();
			}


        }


		private void MenuMain_ShowShipName_CheckedChanged(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormArsenal.ShowShipName = this.MenuMain_ShowShipName.Checked;

            this.UpdateUI();
		}


		private void TableArsenal_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}



		protected override string GetPersistString()
		{
			return "Arsenal";
		}



	}

}
