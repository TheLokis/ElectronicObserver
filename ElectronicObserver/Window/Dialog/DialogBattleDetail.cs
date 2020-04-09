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

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogBattleDetail : Form
	{

		public string BattleDetailText
		{
			get { return this.TextBattleDetail.Text; }
			set { this.TextBattleDetail.Text = value; }
		}

		public DialogBattleDetail()
		{
            this.InitializeComponent();

            this.Font = Utility.Configuration.Config.UI.MainFont;
		}

		private void DialogBattleDetail_Load(object sender, EventArgs e)
		{
			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBattle]);
		}

		private void DialogBattleDetail_FormClosed(object sender, FormClosedEventArgs e)
		{
			ResourceManager.DestroyIcon(this.Icon);
		}


		private void DialogBattleDetail_Shown(object sender, EventArgs e)
		{
            this.ClientSize = new Size(
				Math.Min(this.TextBattleDetail.Location.X * 2 + this.TextBattleDetail.Width + this.TextBattleDetail.Margin.Horizontal, 800),
				Math.Min(this.TextBattleDetail.Location.Y * 2 + this.TextBattleDetail.Height + this.TextBattleDetail.Margin.Vertical, 600));

			var workingScreen = Screen.GetWorkingArea(this.Location);
			var dialogRectangle = new Rectangle(this.Left, this.Top, this.Right, this.Bottom);

			if (!workingScreen.Contains(dialogRectangle))
			{

				if (this.Right > workingScreen.Right && this.Bottom > workingScreen.Bottom)
				{
                    this.Location = new Point(workingScreen.Right - this.Width, workingScreen.Bottom - this.Height);
				}
				else if (this.Right > workingScreen.Right)
				{
                    this.Location = new Point(workingScreen.Right - this.Width, this.Top);
				}
				else if (this.Bottom > workingScreen.Bottom)
				{
                    this.Location = new Point(this.Left, workingScreen.Bottom - this.Height);
				}
				else
				{
					return; // モニターを Location で指定してあるので例外はないはず。
				}
			}
		}
	}
}
