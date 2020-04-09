using ElectronicObserver.Notifier;
using ElectronicObserver.Utility.Data;
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

	/// <summary>
	/// 通知システムの設定ダイアログを扱います。
	/// </summary>
	public partial class DialogConfigurationNotifier : Form
	{

		private NotifierBase _notifier;
		private bool _soundChanged;
		private bool _imageChanged;

		public DialogConfigurationNotifier(NotifierBase notifier)
		{
            this.InitializeComponent();

            this._notifier = notifier;

            //init base
            this._soundChanged = false;
            this._imageChanged = false;

            this.GroupSound.AllowDrop = true;
            this.GroupImage.AllowDrop = true;



            //init from data

            this.IsEnabled.Checked = notifier.IsEnabled;

            this.PlaysSound.Checked = notifier.PlaysSound;
            this.SoundPath.Text = notifier.SoundPath;
            this.SoundVolume.Value = notifier.SoundVolume;
            this.LoopsSound.Checked = notifier.LoopsSound;

            this.DrawsImage.Checked = notifier.DialogData.DrawsImage;
            this.ImagePath.Text = notifier.DialogData.ImagePath;

            this.ShowsDialog.Checked = notifier.ShowsDialog;
            this.TopMostFlag.Checked = notifier.DialogData.TopMost;
            this.Alignment.SelectedIndex = (int)notifier.DialogData.Alignment;
            this.LocationX.Value = notifier.DialogData.Location.X;
            this.LocationY.Value = notifier.DialogData.Location.Y;
            this.DrawsMessage.Checked = notifier.DialogData.DrawsMessage;
            this.HasFormBorder.Checked = notifier.DialogData.HasFormBorder;
            this.AccelInterval.Value = notifier.AccelInterval / 1000;
            this.ClosingInterval.Value = notifier.DialogData.ClosingInterval / 1000;
			for (int i = 0; i < (int)NotifierDialogClickFlags.HighestBit; i++)
                this.CloseList.SetItemChecked(i, ((int)notifier.DialogData.ClickFlag & (1 << i)) != 0);
            this.CloseList.SetItemChecked((int)NotifierDialogClickFlags.HighestBit, notifier.DialogData.CloseOnMouseMove);
            this.ShowWithActivation.Checked = notifier.DialogData.ShowWithActivation;
            this.ForeColorPreview.ForeColor = notifier.DialogData.ForeColor;
            this.BackColorPreview.ForeColor = notifier.DialogData.BackColor;
            this.LevelBorder.Maximum = ExpTable.ShipMaximumLevel;

			NotifierDamage ndmg = notifier as NotifierDamage;
			if (ndmg != null)
			{
                this.NotifiesBefore.Checked = ndmg.NotifiesBefore;
                this.NotifiesNow.Checked = ndmg.NotifiesNow;
                this.NotifiesAfter.Checked = ndmg.NotifiesAfter;
                this.ContainsNotLockedShip.Checked = ndmg.ContainsNotLockedShip;
                this.ContainsSafeShip.Checked = ndmg.ContainsSafeShip;
                this.ContainsFlagship.Checked = ndmg.ContainsFlagship;
                this.LevelBorder.Value = ndmg.LevelBorder;
                this.NotifiesAtEndpoint.Checked = ndmg.NotifiesAtEndpoint;

			}
			else
			{
                this.GroupDamage.Visible = false;
                this.GroupDamage.Enabled = false;
			}

			NotifierAnchorageRepair nanc = notifier as NotifierAnchorageRepair;
			if (nanc != null)
			{
                this.AnchorageRepairNotificationLevel.SelectedIndex = nanc.NotificationLevel;

			}
			else
			{
                this.GroupAnchorageRepair.Visible = false;
                this.GroupAnchorageRepair.Enabled = false;
			}

            var nbac = notifier as NotifierBaseAirCorps;
            if (nbac != null)
            {
                this.BaseAirCorps_NotSupplied.Checked = nbac.NotifiesNotSupplied;
                this.BaseAirCorps_Tired.Checked = nbac.NotifiesTired;
                this.BaseAirCorps_NotOrganized.Checked = nbac.NotifiesNotOrganized;

                this.BaseAirCorps_Standby.Checked = nbac.NotifiesStandby;
                this.BaseAirCorps_Retreat.Checked = nbac.NotifiesRetreat;
                this.BaseAirCorps_Rest.Checked = nbac.NotifiesRest;

                this.BaseAirCorps_NormalMap.Checked = nbac.NotifiesNormalMap;
                this.BaseAirCorps_EventMap.Checked = nbac.NotifiesEventMap;

                this.BaseAirCorps_SquadronRelocation.Checked = nbac.NotifiesSquadronRelocation;
                this.BaseAirCorps_EquipmentRelocation.Checked = nbac.NotifiesEquipmentRelocation;
            }
            else
            {
                this.GroupBaseAirCorps.Visible = false;
                this.GroupBaseAirCorps.Enabled = false;
            }


            this.DialogOpenSound.Filter = "사운드파일|" + string.Join(";", Utility.MediaPlayer.SupportedExtensions.Select(s => "*." + s)) + "|File|*";

		}

		private void DialogConfigurationNotifier_Load(object sender, EventArgs e)
		{

		}



		private void GroupSound_DragEnter(object sender, DragEventArgs e)
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

		private void GroupSound_DragDrop(object sender, DragEventArgs e)
		{

            this.SoundPath.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
		}

		private void GroupImage_DragEnter(object sender, DragEventArgs e)
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

		private void GroupImage_DragDrop(object sender, DragEventArgs e)
		{

            this.ImagePath.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
		}



		private void SoundPath_TextChanged(object sender, EventArgs e)
		{

            this._soundChanged = true;
		}

		private void ImagePath_TextChanged(object sender, EventArgs e)
		{

            this._imageChanged = true;
		}

		private void SoundPathSearch_Click(object sender, EventArgs e)
		{

			if (this.SoundPath.Text != "")
			{
				try
				{
                    this.DialogOpenSound.InitialDirectory = System.IO.Path.GetDirectoryName(this.SoundPath.Text);

				}
				catch (Exception) { }
			}

			if (this.DialogOpenSound.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.SoundPath.Text = this.DialogOpenSound.FileName;
			}

		}

		private void ImagePathSearch_Click(object sender, EventArgs e)
		{

			if (this.ImagePath.Text != "")
			{
				try
				{
                    this.DialogOpenImage.InitialDirectory = System.IO.Path.GetDirectoryName(this.ImagePath.Text);

				}
				catch (Exception) { }
			}

			if (this.DialogOpenImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.ImagePath.Text = this.DialogOpenImage.FileName;
			}
		}


		private void ForeColorSelect_Click(object sender, EventArgs e)
		{

            this.DialogColor.Color = this.ForeColorPreview.ForeColor;
			if (this.DialogColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.ForeColorPreview.ForeColor = this.DialogColor.Color;
			}
		}

		private void BackColorSelect_Click(object sender, EventArgs e)
		{

            this.DialogColor.Color = this.BackColorPreview.ForeColor;
			if (this.DialogColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.BackColorPreview.ForeColor = this.DialogColor.Color;
			}
		}


		private void ForeColorPreview_ForeColorChanged(object sender, EventArgs e)
		{

			if (this.ForeColorPreview.ForeColor.GetBrightness() >= 0.5)
			{
                this.ForeColorPreview.BackColor = Color.Black;
			}
			else
			{
                this.ForeColorPreview.BackColor = Color.White;
			}
		}

		private void BackColorPreview_ForeColorChanged(object sender, EventArgs e)
		{

			if (this.BackColorPreview.ForeColor.GetBrightness() >= 0.5)
			{
                this.BackColorPreview.BackColor = Color.Black;
			}
			else
			{
                this.BackColorPreview.BackColor = Color.White;
			}
		}



		private void ButtonOK_Click(object sender, EventArgs e)
		{

			if (!this.SetConfiguration()) return;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}


		private bool SetConfiguration()
		{


			if (this._soundChanged)
			{
				if (!this._notifier.LoadSound(this.SoundPath.Text) && this.PlaysSound.Checked)
				{
					MessageBox.Show("사운드파일 불러오기에 실패했습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
			if (this._imageChanged)
			{
				if (!this._notifier.DialogData.LoadImage(this.ImagePath.Text) && this.DrawsImage.Checked)
				{
					MessageBox.Show("사운드파일 불러오기에 실패했습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}


            //set configuration
            this._notifier.IsEnabled = this.IsEnabled.Checked;

            this._notifier.PlaysSound = this.PlaysSound.Checked;
            this._notifier.DialogData.DrawsImage = this.DrawsImage.Checked;
            this._notifier.SoundVolume = (int)this.SoundVolume.Value;
            this._notifier.LoopsSound = this.LoopsSound.Checked;

            this._notifier.ShowsDialog = this.ShowsDialog.Checked;
            this._notifier.DialogData.TopMost = this.TopMostFlag.Checked;
            this._notifier.DialogData.Alignment = (NotifierDialogAlignment)this.Alignment.SelectedIndex;
            this._notifier.DialogData.Location = new Point((int)this.LocationX.Value, (int)this.LocationY.Value);
            this._notifier.DialogData.DrawsMessage = this.DrawsMessage.Checked;
            this._notifier.DialogData.HasFormBorder = this.HasFormBorder.Checked;
            this._notifier.AccelInterval = (int)(this.AccelInterval.Value * 1000);
            this._notifier.DialogData.ClosingInterval = (int)(this.ClosingInterval.Value * 1000);
			{
				int flag = 0;
				for (int i = 0; i < (int)NotifierDialogClickFlags.HighestBit; i++)
					flag |= (this.CloseList.GetItemChecked(i) ? 1 : 0) << i;
                this._notifier.DialogData.ClickFlag = (NotifierDialogClickFlags)flag;
			}
            this._notifier.DialogData.CloseOnMouseMove = this.CloseList.GetItemChecked((int)NotifierDialogClickFlags.HighestBit);
            this._notifier.DialogData.ForeColor = this.ForeColorPreview.ForeColor;
            this._notifier.DialogData.BackColor = this.BackColorPreview.ForeColor;
            this._notifier.DialogData.ShowWithActivation = this.ShowWithActivation.Checked;

			var ndmg = this._notifier as NotifierDamage;
			if (ndmg != null)
			{
				ndmg.NotifiesBefore = this.NotifiesBefore.Checked;
				ndmg.NotifiesNow = this.NotifiesNow.Checked;
				ndmg.NotifiesAfter = this.NotifiesAfter.Checked;
				ndmg.ContainsNotLockedShip = this.ContainsNotLockedShip.Checked;
				ndmg.ContainsSafeShip = this.ContainsSafeShip.Checked;
				ndmg.ContainsFlagship = this.ContainsFlagship.Checked;
				ndmg.LevelBorder = (int)this.LevelBorder.Value;
				ndmg.NotifiesAtEndpoint = this.NotifiesAtEndpoint.Checked;
			}

			var nanc = this._notifier as NotifierAnchorageRepair;
			if (nanc != null)
			{
				nanc.NotificationLevel = this.AnchorageRepairNotificationLevel.SelectedIndex;
			}

            var nbac = this._notifier as NotifierBaseAirCorps;
            if (nbac != null)
            {
                nbac.NotifiesNotSupplied = this.BaseAirCorps_NotSupplied.Checked;
                nbac.NotifiesTired = this.BaseAirCorps_Tired.Checked;
                nbac.NotifiesNotOrganized = this.BaseAirCorps_NotOrganized.Checked;
                nbac.NotifiesStandby = this.BaseAirCorps_Standby.Checked;
                nbac.NotifiesRetreat = this.BaseAirCorps_Retreat.Checked;
                nbac.NotifiesRest = this.BaseAirCorps_Rest.Checked;
                nbac.NotifiesNormalMap = this.BaseAirCorps_NormalMap.Checked;
                nbac.NotifiesEventMap = this.BaseAirCorps_EventMap.Checked;
                nbac.NotifiesSquadronRelocation = this.BaseAirCorps_SquadronRelocation.Checked;
                nbac.NotifiesEquipmentRelocation = this.BaseAirCorps_EquipmentRelocation.Checked;
            }

            return true;
		}


		private void ButtonTest_Click(object sender, EventArgs e)
		{

			if (!this.SetConfiguration()) return;

			if (this._notifier.DialogData.Alignment == NotifierDialogAlignment.Custom)
			{
                this._notifier.DialogData.Message = "테스트 알림입니다.\r\n창을 이동시킨후 닫으면 해당위치로 좌표가 수정됩니다.";
                this._notifier.Notify((_sender, _e) =>
				{
					var dialog = _sender as DialogNotifier;
					if (dialog != null)
					{
                        this._notifier.DialogData.Location = dialog.Location;
                        this.LocationX.Value = dialog.Location.X;
                        this.LocationY.Value = dialog.Location.Y;
					}
				});
			}
			else
			{
                this._notifier.DialogData.Message = "테스트 알림입니다.";
                this._notifier.Notify();
			}
		}

		private void SoundPathDirectorize_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.SoundPath.Text))
			{
				try
				{
                    this.SoundPath.Text = System.IO.Path.GetDirectoryName(this.SoundPath.Text);
				}
				catch (Exception)
				{
					// *ぷちっ*
				}
			}
		}


	}
}
