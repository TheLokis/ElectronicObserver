using ElectronicObserver.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogConfigurationBGMPlayer : Form
	{

		public SyncBGMPlayer.SoundHandle ResultHandle { get; private set; }

		public DialogConfigurationBGMPlayer(SyncBGMPlayer.SoundHandle handle)
		{
            this.InitializeComponent();

            this.FilePath.Text = handle.Path;
            this.IsLoop.Checked = handle.IsLoop;
            this.LoopHeadPosition.Value = (decimal)handle.LoopHeadPosition;
            this.Volume.Value = handle.Volume;

            this.Text = "BGM 설정 - " + SyncBGMPlayer.SoundHandleIDToString(handle.HandleID);
            this.ResultHandle = handle.Clone();
		}

		private void DialogConfigurationBGMPlayer_Load(object sender, EventArgs e)
		{
            this.OpenMusicDialog.Filter = "음악 파일|" + string.Join(";", MediaPlayer.SupportedExtensions.Select(s => "*." + s)) + "|File|*";
		}

		private void ButtonAccept_Click(object sender, EventArgs e)
		{
            this.ResultHandle.Path = this.FilePath.Text;
            this.ResultHandle.IsLoop = this.IsLoop.Checked;
            this.ResultHandle.LoopHeadPosition = (double)this.LoopHeadPosition.Value;
            this.ResultHandle.Volume = (int)this.Volume.Value;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void FilePath_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void FilePath_DragDrop(object sender, DragEventArgs e)
		{
            this.FilePath.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
		}

		private void FilePathSearch_Click(object sender, EventArgs e)
		{
			if (this.OpenMusicDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.FilePath.Text = this.OpenMusicDialog.FileName;
			}
		}

		private void FilePathToDirectory_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.FilePath.Text))
                this.FilePath.Text = Path.GetDirectoryName(this.FilePath.Text);
		}
	}
}
