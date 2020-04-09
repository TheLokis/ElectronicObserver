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

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogTextSelect : Form
	{

		public int SelectedIndex => this.TextSelect.SelectedIndex;

		public object SelectedItem => this.TextSelect.SelectedItem;

		public DialogTextSelect()
		{
            this.InitializeComponent();

			ControlHelper.SetDoubleBuffered(this.tableLayoutPanel1);
		}

		public DialogTextSelect(string title, string description, object[] items)
			: this()
		{

            this.Initialize(title, description, items);
		}

		public void Initialize(string title, string description, object[] items)
		{
			this.Text = title;

            this.tableLayoutPanel1.SuspendLayout();

            this.Description.Text = description;

            this.TextSelect.BeginUpdate();
            this.TextSelect.Items.Clear();
            this.TextSelect.Items.AddRange(items);
			if (this.TextSelect.Items.Count > 0)
                this.TextSelect.SelectedIndex = 0;
            this.TextSelect.EndUpdate();

            this.tableLayoutPanel1.ResumeLayout();

		}


		private void ButtonOK_Click(object sender, EventArgs e)
		{
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

	}
}
