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

	public partial class DialogTextInput : Form
	{

		/// <summary>
		/// 入力されたテキスト
		/// </summary>
		public string InputtedText
		{
			get { return this.TextInput.Text; }
			set { this.TextInput.Text = value; }
		}


		public DialogTextInput()
		{
            this.InitializeComponent();

			ControlHelper.SetDoubleBuffered(this.tableLayoutPanel1);
		}

		public DialogTextInput(string title, string description)
			: this()
		{

            this.Initialize(title, description);
		}


		/// <summary>
		/// 指定されたタイトルと説明文でダイアログ ボックスを初期化します。
		/// </summary>
		/// <param name="title">タイトル。</param>
		/// <param name="description">説明文。</param>
		public void Initialize(string title, string description)
		{
			this.Text = title;

            this.tableLayoutPanel1.SuspendLayout();

            this.Description.Text = description;

            this.tableLayoutPanel1.ResumeLayout();

		}

		/// <summary>
		/// テキストボックスを複数行にするかを指定します。
		/// </summary>
		/// <param name="flag">複数行にするか。既定値は true です。</param>
		public void SetMultiline(bool flag = true)
		{

			if (flag)
			{
                this.TextInput.Multiline = true;
                this.TextInput.Dock = DockStyle.Fill;
			}
			else
			{
                this.TextInput.Multiline = false;
                this.TextInput.Dock = DockStyle.None;
			}

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
