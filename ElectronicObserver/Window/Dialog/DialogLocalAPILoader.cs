using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ElectronicObserver.Observer;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogLocalAPILoader : Form
	{

		public string FilePath => this.TextFilePath.Text;

		public string FileData
		{
			get
			{
				try
				{

					using (var sr = new StreamReader(this.TextFilePath.Text))
					{
						return sr.ReadToEnd();
					}

				}
				catch (Exception)
				{

					return null;
				}
			}
		}

		public string APIName
		{
			get
			{
				if (this.APIList.SelectedIndex != -1)
					return this.APIList.SelectedItem.ToString();
				else
					return null;
			}
		}

		public string APIPath
		{
			get
			{
				if (this.APIList.SelectedIndex != -1)
					return "/kcsapi/" + this.APIList.SelectedItem.ToString();
				else
					return null;
			}
		}

		public bool IsRequest => this.APICategory.SelectedIndex == 0;

		public bool IsResponse => this.APICategory.SelectedIndex == 1;


		public DialogLocalAPILoader()
		{
            this.InitializeComponent();
		}


		private void DialogLocalAPILoader_Load(object sender, EventArgs e)
		{

			Icon iconWarning = SystemIcons.Warning;
			Bitmap bmp = new Bitmap(this.PictureWarning.Width, this.PictureWarning.Height);
			using (Graphics g = Graphics.FromImage(bmp))
			{

				g.DrawIcon(iconWarning, 0, 0);
                this.PictureWarning.Image = bmp;

			}


            this.APICategory.SelectedIndex = 1;

            this.FileOpener.InitialDirectory = Utility.Configuration.Config.Connection.SaveDataPath;
		}


		private void APICategory_SelectedIndexChanged(object sender, EventArgs e)
		{

            this.APIList.Items.Clear();
			if (this.APICategory.SelectedIndex == 0)
			{
				//request
				foreach (string s in APIObserver.Instance.APIList.Values.Where(a => a.IsRequestSupported).Select(a => a.APIName))
				{
                    this.APIList.Items.Add(s);
				}

			}
			else
			{
				//response
				foreach (string s in APIObserver.Instance.APIList.Values.Where(a => a.IsResponseSupported).Select(a => a.APIName))
				{
                    this.APIList.Items.Add(s);
				}
			}

            this.APIList.SelectedIndex = 0;

		}


		private void ButtonSearchFilePath_Click(object sender, EventArgs e)
		{

			if (File.Exists(this.TextFilePath.Text))
                this.FileOpener.FileName = this.TextFilePath.Text;

            this.FileOpener.Filter = this.APIList.SelectedItem.ToString() + "|*" + (this.APICategory.SelectedIndex == 0 ? "Q" : "S") + "@" + this.APIList.SelectedItem.ToString().Replace('/', '@') + ".json|JSON|*.json;*.js|File|*";

			if (this.FileOpener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.TextFilePath.Text = this.FileOpener.FileName;
			}

		}



		private void ButtonOpen_Click(object sender, EventArgs e)
		{
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}


		private void TextFilePath_DragEnter(object sender, DragEventArgs e)
		{

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;

			}
			else
			{
				e.Effect = DragDropEffects.None;
			}

		}

		private void TextFilePath_DragDrop(object sender, DragEventArgs e)
		{

			string[] path = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (path.Length > 0 && File.Exists(path[0]))
                this.TextFilePath.Text = path[0];

		}

	}
}
