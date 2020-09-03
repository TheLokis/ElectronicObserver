using ElectronicObserver.Data;
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
	public partial class FormFleetPreset : DockContent
	{

		private class TablePresetControl : IDisposable
		{
			public ImageLabel Name;
			public ImageLabel[] Ships;

			private ToolTip _tooltip;

			public TablePresetControl(FormFleetPreset parent)
			{
				ImageLabel CreateDefaultLabel()
				{
					return new ImageLabel
					{
						Text = "",
						Anchor = AnchorStyles.Left,
						ForeColor = parent.ForeColor,
						Tag = null,
						TextAlign = ContentAlignment.MiddleLeft,
						Padding = new Padding(0, 1, 0, 1),
						Margin = new Padding(2, 1, 2, 1),
						AutoEllipsis = false,
						AutoSize = true,
						Visible = true,

						ImageList = ResourceManager.Instance.Icons,
						ImageAlign = ContentAlignment.MiddleCenter,
						ImageIndex = -1
					};
				}

                this.Name = CreateDefaultLabel();
                this.Name.ImageAlign = ContentAlignment.MiddleRight;

                // TODO: 本体側がもし 7 隻編成に対応したら変更してください
                this.Ships = new ImageLabel[6];
				for (int i = 0; i < this.Ships.Length; i++)
				{
                    this.Ships[i] = CreateDefaultLabel();

				}

                this._tooltip = parent.ToolTipInfo;
			}


			public void AddToTable(TableLayoutPanel table, int row)
			{
				table.Controls.Add(this.Name, 0, row);
				for (int i = 0; i < this.Ships.Length; i++)
				{
					table.Controls.Add(this.Ships[i], 1 + i, row);
				}
			}

			public void Update(int presetID)
			{
				var preset = KCDatabase.Instance.FleetPreset[presetID];

				if (preset == null)
				{
                    this.Name.Text = "----";
                    this._tooltip.SetToolTip(this.Name, null);

					foreach (var ship in this.Ships)
					{
						ship.Text = string.Empty;
                        this._tooltip.SetToolTip(ship, null);
					}
					return;
				}


                this.Name.Text = preset.Name;

                int lowestCondition = preset.MembersInstance.Where(s => s != null).Select(s => s.Condition).DefaultIfEmpty(49).Min();
                FormFleet.SetConditionDesign(this.Name, lowestCondition);

                this._tooltip.SetToolTip(this.Name, $"최하피로도: {lowestCondition}");

				for (int i = 0; i < this.Ships.Length; i++)
				{
					var ship = i >= preset.Members.Count ? null : preset.MembersInstance.ElementAt(i);
					var label = this.Ships[i];

                    this.Ships[i].Text = ship?.Name ?? "-";

					if (ship == null)
					{
                        this._tooltip.SetToolTip(this.Ships[i], null);
					}
					else
					{
						var sb = new StringBuilder();
						sb.AppendLine($"{ship.MasterShip.ShipTypeName} {ship.NameWithLevel}");
						sb.AppendLine($"HP: {ship.HPCurrent} / {ship.HPMax} ({ship.HPRate:p1}) [{Constants.GetDamageState(ship.HPRate)}]");
						sb.AppendLine($"cond: {ship.Condition}");
						sb.AppendLine();

						var slot = ship.AllSlotInstance;
						for (int e = 0; e < slot.Count; e++)
						{
							if (slot[e] == null)
								continue;

							if (e < ship.MasterShip.Aircraft.Count)
							{
								sb.AppendLine($"[{ship.Aircraft[e]}/{ship.MasterShip.Aircraft[e]}] {slot[e].NameWithLevel}");
							}
							else
							{
								sb.AppendLine(slot[e].NameWithLevel);
							}
						}

                        this._tooltip.SetToolTip(this.Ships[i], sb.ToString());
					}
				}

			}


            public void ConfigurationChanged(FormFleetPreset parent)
			{
				var config = Utility.Configuration.Config;
				var font = Utility.Configuration.Config.UI.MainFont;

                this.Name.Font = font;
                this.Name.ImageAlign = config.FormFleet.ShowConditionIcon ? ContentAlignment.MiddleRight : ContentAlignment.MiddleCenter;

				foreach (var ship in this.Ships)
				{
					ship.Font = font;

					if (config.FormFleet.FixShipNameWidth)
					{
						ship.AutoSize = false;
						ship.Size = new Size(config.FormFleet.FixedShipNameWidth, 20);
					}
					else
					{
						ship.AutoSize = true;
					}
				}
			}

			public void Dispose()
			{
                this.Name.Dispose();
				foreach (var ship in this.Ships)
					ship.Dispose();
			}
		}


		private List<TablePresetControl> TableControls;



		public FormFleetPreset(FormMain parent)
		{
            this.InitializeComponent();

            // some initialization
            this.TableControls = new List<TablePresetControl>();
            this.ConfigurationChanged();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleetPreset]);
		}

		private void FormFleetPreset_Load(object sender, EventArgs e)
		{
			KCDatabase.Instance.FleetPreset.PresetChanged += this.Updated;

			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
		}

		private void ConfigurationChanged()
		{
			var config = Utility.Configuration.Config;
            this.Font = Utility.Configuration.Config.UI.MainFont;
			bool fixShipNameWidth = config.FormFleet.FixShipNameWidth;

            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

            this.TablePresets.SuspendLayout();
			foreach (var item in this.TableControls)
				item.ConfigurationChanged(this);

			for (int i = 1; i < this.TablePresets.ColumnCount; i++)
				ControlHelper.SetTableColumnStyle(this.TablePresets, i, fixShipNameWidth ?
					new ColumnStyle(SizeType.Absolute, config.FormFleet.FixedShipNameWidth + 4) :
					new ColumnStyle(SizeType.AutoSize));
			ControlHelper.SetTableRowStyles(this.TablePresets, ControlHelper.GetDefaultRowStyle());
            this.TablePresets.ResumeLayout();
		}

		private void Updated()
		{
			var presets = KCDatabase.Instance.FleetPreset;
			if (presets == null || presets.MaximumCount <= 0)
				return;

            this.TablePresets.Enabled = false;
            this.TablePresets.SuspendLayout();

			if (this.TableControls.Count < presets.MaximumCount)
			{
				for (int i = this.TableControls.Count; i < presets.MaximumCount; i++)
				{
					var control = new TablePresetControl(this);
					control.ConfigurationChanged(this);
                    this.TableControls.Add(control);
					control.AddToTable(this.TablePresets, i);
				}

				ControlHelper.SetTableRowStyles(this.TablePresets, ControlHelper.GetDefaultRowStyle());
			}

			for (int i = 0; i < this.TableControls.Count; i++)
			{
                this.TableControls[i].Update(i + 1);
			}

            this.TablePresets.ResumeLayout();
            this.TablePresets.Enabled = true;
		}


		private void TablePresets_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(e.Row % 5 == 4 && e.Column == 0 ? Pens.Gray : Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}


		protected override string GetPersistString()
		{
			return "FleetPreset";
		}

		private void FormFleetPreset_Click(object sender, EventArgs e)
		{
			Utility.Logger.Add(1, this.Font.Name);
            this.ConfigurationChanged();
		}
	}
}
