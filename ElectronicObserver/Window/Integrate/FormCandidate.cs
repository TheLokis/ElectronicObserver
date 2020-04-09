using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Integrate
{

	/// <summary>
	/// 選択中のウィンドウに表示する枠
	/// </summary>
	public partial class FormCandidate : Form
	{

		private Pen blackPen = new Pen(Color.Black, 5);

		public FormCandidate()
		{
            this.InitializeComponent();
		}

		private void FormCandidate_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.DrawRectangle(this.blackPen, 2, 2, this.Width - 4, this.Height - 4);
		}

		private void FormCandidate_FormClosed(object sender, FormClosedEventArgs e)
		{
            this.blackPen.Dispose();
		}

		private void FormCandidate_Resize(object sender, EventArgs e)
		{
            this.Invalidate();
		}
	}
}
