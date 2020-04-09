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

	public partial class FormDock : DockContent
	{

		private class TableDockControl : IDisposable
		{

			public Label ShipName;
			public Label RepairTime;
			public ToolTip ToolTipInfo;

			public TableDockControl(FormDock parent)
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

                this.RepairTime = new Label
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

                this.ToolTipInfo = parent.ToolTipInfo;

				#endregion

			}


			public TableDockControl(FormDock parent, TableLayoutPanel table, int row)
				: this(parent)
			{

                this.AddToTable(table, row);
			}

			public void AddToTable(TableLayoutPanel table, int row)
			{

				table.Controls.Add(this.ShipName, 0, row);
				table.Controls.Add(this.RepairTime, 1, row);

			}


			//データ更新時
			public void Update(int dockID)
			{

				KCDatabase db = KCDatabase.Instance;

				DockData dock = db.Docks[dockID];

                this.RepairTime.BackColor = Color.Transparent;
                this.ToolTipInfo.SetToolTip(this.ShipName, null);
                this.ToolTipInfo.SetToolTip(this.RepairTime, null);

				if (dock == null || dock.State == -1)
				{
                    //locked
                    this.ShipName.Text = "";
                    this.RepairTime.Text = "";
                    this.RepairTime.Tag = null;

				}
				else if (dock.State == 0)
				{
                    //empty
                    this.ShipName.Text = "----";
                    this.RepairTime.Text = "";
                    this.RepairTime.Tag = null;

				}
				else
				{
                    //repairing
                    this.ShipName.Text = db.Ships[dock.ShipID].Name;
                    this.ToolTipInfo.SetToolTip(this.ShipName, db.Ships[dock.ShipID].NameWithLevel);
                    this.RepairTime.Text = DateTimeHelper.ToTimeRemainString(dock.CompletionTime);
                    this.RepairTime.Tag = dock.CompletionTime;
                    this.ToolTipInfo.SetToolTip(this.RepairTime, "完了日時 : " + DateTimeHelper.TimeToCSVString(dock.CompletionTime));

				}

			}

			//タイマー更新時
			public void Refresh(int dockID)
			{

				if (this.RepairTime.Tag != null)
				{

					var time = (DateTime)this.RepairTime.Tag;

                    this.RepairTime.Text = DateTimeHelper.ToTimeRemainString(time);

					if (Utility.Configuration.Config.FormDock.BlinkAtCompletion && (time - DateTime.Now).TotalMilliseconds <= Utility.Configuration.Config.NotifierRepair.AccelInterval)
					{
                        this.RepairTime.BackColor = DateTime.Now.Second % 2 == 0 ? Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.GreenHighlight) : Color.Transparent;
					}
				}
			}


			public void ConfigurationChanged(FormDock parent)
			{

				var config = Utility.Configuration.Config.FormDock;

                this.ShipName.Font = parent.Font;
                this.RepairTime.Font = parent.Font;
                this.RepairTime.BackColor = Color.Transparent;
                this.ShipName.ForeColor = parent.ForeColor;
                this.RepairTime.ForeColor = parent.ForeColor;


                this.ShipName.MaximumSize = new Size(config.MaxShipNameWidth, this.ShipName.MaximumSize.Height);
			}

			public void Dispose()
			{
                this.ShipName.Dispose();
                this.RepairTime.Dispose();
			}
		}



		private TableDockControl[] ControlDock;




		public FormDock(FormMain parent)
		{
            this.InitializeComponent();

			Utility.SystemEvents.UpdateTimerTick += this.UpdateTimerTick;


			ControlHelper.SetDoubleBuffered(this.TableDock);


            this.TableDock.SuspendLayout();
            this.ControlDock = new TableDockControl[4];
			for (int i = 0; i < this.ControlDock.Length; i++)
			{
                this.ControlDock[i] = new TableDockControl(this, this.TableDock, i);
			}
            this.TableDock.ResumeLayout();


            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormDock]);

		}


		private void FormDock_Load(object sender, EventArgs e)
		{

			APIObserver o = APIObserver.Instance;

			o.APIList["api_req_nyukyo/start"].RequestReceived += this.Updated;
			o.APIList["api_req_nyukyo/speedchange"].RequestReceived += this.Updated;

			o.APIList["api_port/port"].ResponseReceived += this.Updated;
			o.APIList["api_get_member/ndock"].ResponseReceived += this.Updated;

			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
		}



		void Updated(string apiname, dynamic data)
		{

            this.TableDock.SuspendLayout();
            this.TableDock.RowCount = KCDatabase.Instance.Docks.Values.Count(d => d.State != -1);
			for (int i = 0; i < this.ControlDock.Length; i++)
                this.ControlDock[i].Update(i + 1);
            this.TableDock.ResumeLayout();

		}


		void UpdateTimerTick()
		{

            this.TableDock.SuspendLayout();
			for (int i = 0; i < this.ControlDock.Length; i++)
                this.ControlDock[i].Refresh(i + 1);
            this.TableDock.ResumeLayout();

		}



		private void TableDock_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}


		void ConfigurationChanged()
		{

            this.Font = Utility.Configuration.Config.UI.MainFont;
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);

            if (this.ControlDock != null)
			{
                this.TableDock.SuspendLayout();

				foreach (var c in this.ControlDock)
					c.ConfigurationChanged(this);

				ControlHelper.SetTableRowStyles(this.TableDock, ControlHelper.GetDefaultRowStyle());

                this.TableDock.ResumeLayout();
			}
		}


		protected override string GetPersistString()
		{
			return "Dock";
		}

	}

}
