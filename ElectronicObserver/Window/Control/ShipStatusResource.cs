using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Control
{
	public partial class ShipStatusResource : UserControl
	{

		private ToolTip ResourceTip;
		public StatusBarModule BarFuel { get; private set; }
		public StatusBarModule BarAmmo { get; private set; }


		#region Properties

		public int FuelCurrent
		{
			get { return this.BarFuel.Value; }
			set
			{
                this.BarFuel.Value = value;
                this.PropertyChanged();
			}
		}

		public int FuelMax
		{
			get { return this.BarFuel.MaximumValue; }
			set
			{
                this.BarFuel.MaximumValue = value;
                this.PropertyChanged();
			}
		}

		public int AmmoCurrent
		{
			get { return this.BarAmmo.Value; }
			set
			{
                this.BarAmmo.Value = value;
                this.PropertyChanged();
			}
		}

		public int AmmoMax
		{
			get { return this.BarAmmo.MaximumValue; }
			set
			{
                this.BarAmmo.MaximumValue = value;
                this.PropertyChanged();
			}
		}

		#endregion


		public ShipStatusResource(ToolTip resourceTip)
		{
            this.InitializeComponent();

            this.BarFuel = new StatusBarModule();
            this.BarAmmo = new StatusBarModule();

            this.BarFuel.UsePrevValue = this.BarAmmo.UsePrevValue = false;

            this.ResourceTip = resourceTip;
		}



		private void PropertyChanged()
		{

			//FIXME: サブウィンドウ状態のときToolTipが出現しない不具合を確認。

			string tiptext = string.Format("연료 : {0}/{1} ({2}%)\r\n탄약 : {3}/{4} ({5}%)",
                this.FuelCurrent, this.FuelMax, (int)Math.Ceiling(100.0 * this.FuelCurrent / this.FuelMax),
                this.AmmoCurrent, this.AmmoMax, (int)Math.Ceiling(100.0 * this.AmmoCurrent / this.AmmoMax));

            this.ResourceTip.SetToolTip(this, tiptext);

            this.Invalidate();
		}


		/// <summary>
		/// 資源を一度に設定します。
		/// </summary>
		/// <param name="fuelCurrent">燃料の現在値。</param>
		/// <param name="fuelMax">燃料の最大値。</param>
		/// <param name="ammoCurrent">弾薬の現在値。</param>
		/// <param name="ammoMax">燃料の最大値。</param>
		public void SetResources(int fuelCurrent, int fuelMax, int ammoCurrent, int ammoMax)
		{

            this.BarFuel.Value = fuelCurrent;
            this.BarFuel.MaximumValue = fuelMax;
            this.BarAmmo.Value = ammoCurrent;
            this.BarAmmo.MaximumValue = ammoMax;

            this.PropertyChanged();
		}



		private void ShipStatusResource_Paint(object sender, PaintEventArgs e)
		{

			const int margin = 3;

            this.BarFuel.Paint(e.Graphics, new Rectangle(0, margin, this.Width, this.BarFuel.GetPreferredSize().Height));
            this.BarAmmo.Paint(e.Graphics, new Rectangle(0, this.Height - margin - this.BarFuel.GetPreferredSize().Height, this.Width, this.BarFuel.GetPreferredSize().Height));

		}

	}
}
