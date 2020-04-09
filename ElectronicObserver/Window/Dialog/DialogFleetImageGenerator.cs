using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogFleetImageGenerator : Form
	{

		private FleetImageArgument CurrentArgument;
		private Font GeneralFont;

		private readonly TextBox[] TextFontList;
		private Regex LFtoCRLF = new Regex(@"\n|\r\n", RegexOptions.Multiline | RegexOptions.Compiled);


		public DialogFleetImageGenerator()
		{
            this.InitializeComponent();

            this.TextFontList = new TextBox[]{
                this.TextTitleFont,
                this.TextLargeFont,
                this.TextMediumFont,
                this.TextSmallFont,
                this.TextMediumDigitFont,
                this.TextSmallDigitFont,
			};

			for (int i = 0; i < this.TextFontList.Length; i++)
			{
				int x = i;
				this.Controls.Find("Select" + this.TextFontList[i].Name.Remove(0, 4), true).First().Click += (sender, e) => this.SelectFont_Click(sender, e, x);
			}

            this.LoadConfiguration();

		}

		public DialogFleetImageGenerator(int fleetID)
			: this()
		{

			if (KCDatabase.Instance.Fleet.CombinedFlag > 0 && fleetID <= 2)
                this.CurrentArgument.FleetIDs = new int[] { 1, 2 };
			else
                this.CurrentArgument.FleetIDs = new int[] { fleetID };
		}



		private void DialogFleetImageGenerator_Load(object sender, EventArgs e)
		{

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleetImageGenerator]);

            this.ApplyToUI(this.CurrentArgument);

            this.UpdateButtonAlert();
		}



		private void LoadConfiguration()
		{
			var config = Utility.Configuration.Config.FleetImageGenerator;

            this.CurrentArgument = config.Argument.Clone();


			switch (config.ImageType)
			{
				case 0:
				default:
                    this.ImageTypeCard.Checked = true;
					break;
				case 1:
                    this.ImageTypeCutin.Checked = true;
					break;
				case 2:
                    this.ImageTypeBanner.Checked = true;
					break;
			}

            this.OutputToClipboard.Checked = config.OutputType == 1;
            this.OpenImageAfterOutput.Checked = config.OpenImageAfterOutput;
            this.DisableOverwritePrompt.Checked = config.DisableOverwritePrompt;
            this.CheckBox_ToJapanese.Checked = config.FleetImageToJapanese;

            this.OutputPath.Text = config.LastOutputPath;
			try
			{
                this.SaveImageDialog.FileName = Path.GetFileName(config.LastOutputPath);
                this.SaveImageDialog.InitialDirectory = string.IsNullOrWhiteSpace(config.LastOutputPath) ? "" : Path.GetDirectoryName(config.LastOutputPath);
            }
            catch (Exception)
			{
			}

            this.SyncronizeTitleAndFileName.Checked = config.SyncronizeTitleAndFileName;
            this.AutoSetFileNameToDate.Checked = config.AutoSetFileNameToDate;

		}

		private void SaveConfiguration()
		{
			var config = Utility.Configuration.Config.FleetImageGenerator;

			if (config.Argument != null)
				config.Argument.DisposeResources();

			config.Argument = this.CurrentArgument.Clone();

			if (this.ImageTypeCard.Checked)
				config.ImageType = 0;
			else if (this.ImageTypeCutin.Checked)
				config.ImageType = 1;
			else if (this.ImageTypeBanner.Checked)
				config.ImageType = 2;

			config.OutputType = this.OutputToClipboard.Checked ? 1 : 0;
			config.OpenImageAfterOutput = this.OpenImageAfterOutput.Checked;
			config.DisableOverwritePrompt = this.DisableOverwritePrompt.Checked;
			config.AutoSetFileNameToDate = this.AutoSetFileNameToDate.Checked;
			config.SyncronizeTitleAndFileName = this.SyncronizeTitleAndFileName.Checked;
            config.FleetImageToJapanese = this.CheckBox_ToJapanese.Checked;

			config.LastOutputPath = this.OutputPath.Text;
		}



		private void ApplyToUI(FleetImageArgument args)
		{

			int[] fleetIDs = args.FleetIDs ?? new int[0];

            this.TargetFleet1.Checked = fleetIDs.Contains(1);
            this.TargetFleet2.Checked = fleetIDs.Contains(2);
            this.TargetFleet3.Checked = fleetIDs.Contains(3);
            this.TargetFleet4.Checked = fleetIDs.Contains(4);

			if (!this.SyncronizeTitleAndFileName.Checked)
                this.Title.Text = args.Title;
            this.Comment.Text = string.IsNullOrWhiteSpace(args.Comment) ? "" : this.LFtoCRLF.Replace(args.Comment, "\r\n");       // 保存データからのロード時に \n に変換されてしまっているため


            this.HorizontalFleetCount.Value = args.HorizontalFleetCount;
            this.HorizontalShipCount.Value = args.HorizontalShipCount;

            this.ReflectDamageGraphic.Checked = args.ReflectDamageGraphic;
            this.AvoidTwitterDeterioration.Checked = args.AvoidTwitterDeterioration;

            this.BackgroundImagePath.Text = args.BackgroundImagePath;

			for (int i = 0; i < this.TextFontList.Length; i++)
			{
                this.TextFontList[i].Text = SerializableFont.FontToString(args.Fonts[i], true);
			}
		}

		private FleetImageArgument ApplyToArgument(FleetImageArgument defaultValue = null)
		{

			var ret = defaultValue?.Clone() ?? new FleetImageArgument();

			ret.FleetIDs = new[]{
                this.TargetFleet1.Checked ? 1 : 0,
                this.TargetFleet2.Checked ? 2 : 0,
                this.TargetFleet3.Checked ? 3 : 0,
                this.TargetFleet4.Checked ? 4 : 0
			}.Where(i => i > 0).ToArray();

			ret.HorizontalFleetCount = (int)this.HorizontalFleetCount.Value;
			ret.HorizontalShipCount = (int)this.HorizontalShipCount.Value;

			ret.ReflectDamageGraphic = this.ReflectDamageGraphic.Checked;
			ret.AvoidTwitterDeterioration = this.AvoidTwitterDeterioration.Checked;

			var fonts = ret.Fonts;
			for (int i = 0; i < fonts.Length; i++)
			{
				if (fonts[i] != null)
					fonts[i].Dispose();
				fonts[i] = SerializableFont.StringToFont(this.TextFontList[i].Text, true);
			}
			ret.Fonts = fonts;

			ret.BackgroundImagePath = this.BackgroundImagePath.Text;

			ret.Title = this.Title.Text;
			ret.Comment = this.Comment.Text;

			return ret;
		}

        private int ImageType
        {
            get
            {
                if (this.ImageTypeCard.Checked)
                    return 0;
                if (this.ImageTypeCutin.Checked)
                    return 1;
                if (this.ImageTypeBanner.Checked)
                    return 2;
                if (this.ImageTypeBaseAirCorps.Checked)
                    return 3;
                return 0;
            }
        }

        private int[] ToFleetIDs()
		{
			return new[]{
                this.TargetFleet1.Checked ? 1 : 0,
                this.TargetFleet2.Checked ? 2 : 0,
                this.TargetFleet3.Checked ? 3 : 0,
                this.TargetFleet4.Checked ? 4 : 0
			}.Where(i => i > 0).ToArray();
		}


		private void ApplyGeneralFont_Click(object sender, EventArgs e)
		{

			if (this.GeneralFont != null)
			{
                this.GeneralFont.Dispose();
			}
            this.GeneralFont = SerializableFont.StringToFont(this.TextGeneralFont.Text, true);

			if (this.GeneralFont == null)
			{
				MessageBox.Show("폰트 이름이 올바르지 않습니다.", "폰트 변환 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.TextGeneralFont.Text = "";
				return;
			}


			for (int i = 0; i < this.TextFontList.Length; i++)
			{
				float size = FleetImageArgument.DefaultFontPixels[i];
				var unit = GraphicsUnit.Pixel;
				var style = FontStyle.Regular;

				var font = SerializableFont.StringToFont(this.TextFontList[i].Text, true);
				if (font != null)
				{
					size = font.Size;
					unit = font.Unit;
					style = font.Style;
					font.Dispose();
				}

				font = new Font(this.GeneralFont.FontFamily, size, style, unit);
                this.TextFontList[i].Text = SerializableFont.FontToString(font);
				font.Dispose();
			}

		}


		private void SelectGeneralFont_Click(object sender, EventArgs e)
		{
            this.fontDialog1.Font = this.GeneralFont;
            if (this.fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.GeneralFont = this.fontDialog1.Font;
                this.TextGeneralFont.Text = SerializableFont.FontToString(this.GeneralFont, true);
			}
		}

		private void SelectFont_Click(object sender, EventArgs e, int index)
		{
            this.fontDialog1.Font = SerializableFont.StringToFont(this.TextFontList[index].Text, true);
			if (this.fontDialog1.ShowDialog() == DialogResult.OK)
			{
                this.TextFontList[index].Text = SerializableFont.FontToString(this.fontDialog1.Font, true);
			}
		}


		private void SearchBackgroundImagePath_Click(object sender, EventArgs e)
		{
            this.OpenImageDialog.FileName = this.BackgroundImagePath.Text;
			if (this.OpenImageDialog.ShowDialog() ==DialogResult.OK)
			{
                this.BackgroundImagePath.Text = this.OpenImageDialog.FileName;
			}
		}

		private void ClearBackgroundPath_Click(object sender, EventArgs e)
		{
            this.BackgroundImagePath.Text = "";
		}




		private void ButtonOK_Click(object sender, EventArgs e)
		{

			var args = this.ApplyToArgument();

			// validation
			if (args.FleetIDs == null || args.FleetIDs.Length == 0)
			{
				MessageBox.Show("출력 대상 함대가 지정되어있지않습니다.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				args.DisposeResources();
				return;
			}

			if (args.HorizontalFleetCount <= 0 || args.HorizontalShipCount <= 0)
			{
				MessageBox.Show("함대 및 함선의 폭은 1 이상으로 설정해주세요.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				args.DisposeResources();
				return;
			}

			if (args.Fonts.Any(f => f == null))
			{
				MessageBox.Show("미입력되거나 잘못된 포트가 존재합니다.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				args.DisposeResources();
				return;
			}

			if (!this.OutputToClipboard.Checked)
			{
				if (string.IsNullOrWhiteSpace(this.OutputPath.Text))
				{
					MessageBox.Show("출력 파일 이름이 입력되어 있지 않습니다.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					args.DisposeResources();
					return;
				}

				if (this.OutputPath.Text.ToCharArray().Intersect(Path.GetInvalidPathChars()).Any())
				{
					MessageBox.Show("사용할 수 없는 문자가 포함되어 있습니다.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					args.DisposeResources();
					return;
				}

				if (!this.DisableOverwritePrompt.Checked && File.Exists(this.OutputPath.Text))
				{
					if (MessageBox.Show(Path.GetFileName(this.OutputPath.Text) + "\r\n은 이미 존재하는 파일입니다.\r\n덮어 쓰시겠습니까?", "덮어쓰기 확인",
						MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
						== System.Windows.Forms.DialogResult.No)
					{
						args.DisposeResources();
						return;
					}
				}
			}

            int mode = this.ImageType;

            try
			{

				if (!this.OutputToClipboard.Checked)
				{

					using (var image = this.GenerateFleetImage(args, mode))
					{

						if (!Directory.Exists(Path.GetDirectoryName(this.OutputPath.Text)))
						{
							Directory.CreateDirectory(Path.GetDirectoryName(this.OutputPath.Text));
						}

						switch (Path.GetExtension(this.OutputPath.Text).ToLower())
						{
							case ".png":
							default:
								image.Save(this.OutputPath.Text, System.Drawing.Imaging.ImageFormat.Png);
								break;

							case ".bmp":
							case ".dib":
								image.Save(this.OutputPath.Text, System.Drawing.Imaging.ImageFormat.Bmp);
								break;

							case ".gif":
								image.Save(this.OutputPath.Text, System.Drawing.Imaging.ImageFormat.Gif);
								break;

							case ".tif":
							case ".tiff":
								image.Save(this.OutputPath.Text, System.Drawing.Imaging.ImageFormat.Tiff);
								break;

							case ".jpg":
							case ".jpeg":
							case ".jpe":
							case ".jfif":
								{
									// jpeg quality settings
									var encoderParams = new System.Drawing.Imaging.EncoderParameters();
									encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);

									var codecInfo = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.MimeType == "image/jpeg");

									image.Save(this.OutputPath.Text, codecInfo, encoderParams);
								}
								break;
						}

						if (this.OpenImageAfterOutput.Checked)
							System.Diagnostics.Process.Start(this.OutputPath.Text);


					}


				}
				else
				{

					using (var image = this.GenerateFleetImage(args, mode))
					{

						Clipboard.SetImage(image);
					}
				}



				if (this.CurrentArgument != null)
                    this.CurrentArgument.DisposeResources();
                this.CurrentArgument = args;
                this.SaveConfiguration();

				Utility.Logger.Add(2, "편성 이미지를 출력했습니다.");

			}
			catch (Exception ex)
			{

				ErrorReporter.SendErrorReport(ex, "편성 이미지 출력에 실패했습니다.");
				MessageBox.Show("편성 이미지 출력에 실패했습니다.\r\n" + ex.GetType().Name + "\r\n" + ex.Message, "편성 이미지 출력 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);

			}
			finally
			{
				args.DisposeResources();
			}


            this.Close();

		}

		private Bitmap GenerateFleetImage(FleetImageArgument args, int mode)
		{
            if(this.CheckBox_ToJapanese.Checked)
            {
                switch (mode)
                {
                    case 0:
                    default:
                        return FleetImageGenerator.GenerateCardBitmap_JP(args);
                    case 1:
                        return FleetImageGenerator.GenerateCutinBitmap_JP(args);
                    case 2:
                        return FleetImageGenerator.GenerateBannerBitmap_JP(args);
                    case 3:
                        return FleetImageGenerator.GenerateBaseAirCorpsImage_JP(args);
                }
            }

			switch (mode)
			{
				case 0:
				default:
					return FleetImageGenerator.GenerateCardBitmap(args);
				case 1:
					return FleetImageGenerator.GenerateCutinBitmap(args);
				case 2:
					return FleetImageGenerator.GenerateBannerBitmap(args);
				case 3:
					return FleetImageGenerator.GenerateBaseAirCorpsImage(args);
			}
		}


		private void ButtonCancel_Click(object sender, EventArgs e)
		{
            this.Close();
		}


		private void ImageTypeCard_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ImageTypeCard.Checked)
                this.HorizontalShipCount.Value = 2;

            this.UpdateButtonAlert();
        }

		private void ImageTypeCutin_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ImageTypeCutin.Checked)
                this.HorizontalShipCount.Value = 1;

            this.UpdateButtonAlert();
        }

		private void ImageTypeBanner_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ImageTypeBanner.Checked)
                this.HorizontalShipCount.Value = 2;

            this.UpdateButtonAlert();
        }

        private bool HasShipImage()
        {
            switch (this.ImageType)
            {
                case 0:
                    return FleetImageGenerator.HasShipImageCard(this.ToFleetIDs(), this.ReflectDamageGraphic.Checked);
                case 1:
                    return FleetImageGenerator.HasShipImageCutin(this.ToFleetIDs(), this.ReflectDamageGraphic.Checked);
                case 2:
                    return FleetImageGenerator.HasShipImageBanner(this.ToFleetIDs(), this.ReflectDamageGraphic.Checked);
                default:
                    return true;
            }
        }

        private void UpdateButtonAlert()
		{

			bool visibility = false;

            if (!Utility.Configuration.Config.Connection.SaveReceivedData || !Utility.Configuration.Config.Connection.SaveOtherFile)
            {

                visibility = true;
                this.ButtonAlert.Text = "함선 이미지 저장 설정이 잘못되었습니다.(상세보기...)";

			}

            if (!this.HasShipImage())
            {

                visibility = true;
                this.ButtonAlert.Text = "함선 이미지가 충분하지 않습니다.(상세보기...)";

			}

            this.ButtonAlert.Visible = visibility;

		}


		private void ButtonAlert_Click(object sender, EventArgs e)
		{
            var config = Utility.Configuration.Config.Connection;

            if (!config.SaveReceivedData || !config.SaveOtherFile)
            {

                if (MessageBox.Show("편성 이미지를 출력하기 위해서는 함선 이미지 저장 설정을 활성화해야합니다.\r\n활성화 하시겠습니까?",
					"함선 이미지 저장 설정이 잘못되었습니다.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
					== System.Windows.Forms.DialogResult.Yes)
				{

                    if (!config.SaveReceivedData)
                    {
                        config.SaveReceivedData = true;
                        config.SaveResponse = false;            // もともと不要にしていたユーザーには res は邪魔なだけだと思うので
                    }

                    config.SaveOtherFile = true;

                    this.UpdateButtonAlert();
				}

			}

            if (!this.HasShipImage())
            {
                string needs;
                switch (this.ImageType)
                {
                    case 0:
                        needs = "칸코레 편성 화면에서 각 함선의 정보를 열면";
                        break;
                    case 1:
                        needs = "이 편성으로 전투를 시작하면";
                        break;
                    case 2:
                        needs = "칸코레 편성 화면을 열면";
                        break;
                    default:
                        needs = "칸코레에서 필요한 이미지가 표시될 때";
                        break;
                }

                MessageBox.Show("현재의 함대의 함선 이미지 데이터가 부족합니다\r\n\r\n캐시를 삭제한 후 다시 로드하시기 바랍니다.\r\n" + needs + "\r\n함선 이미지 데이터가 저장됩니다.",
					"함선 이미지 데이터 부족", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                this.UpdateButtonAlert();
			}

		}



		private void TargetFleet1_CheckedChanged(object sender, EventArgs e)
		{
            this.UpdateButtonAlert();
		}


		private void ButtonClearFont_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show("폰트를 기본 설정으로 되돌립니다.\r\n진행하시겠습니까?", "초기화 확인",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
				 == System.Windows.Forms.DialogResult.Yes)
			{

				if (this.GeneralFont != null)
                    this.GeneralFont.Dispose();
                this.GeneralFont = null;
                this.TextGeneralFont.Text = "";

				var defaultFonts = FleetImageArgument.GetDefaultFonts();
				for (int i = 0; i < this.TextFontList.Length; i++)
				{
                    this.TextFontList[i].Text = SerializableFont.FontToString(defaultFonts[i]);
					defaultFonts[i].Dispose();
				}
			}
		}




		private void Title_TextChanged(object sender, EventArgs e)
		{
			if (this.SyncronizeTitleAndFileName.Checked)
			{
				try
				{

					string replaceTo = Path.GetDirectoryName(this.OutputPath.Text) + "\\" + this.Title.Text + Path.GetExtension(this.OutputPath.Text);

					if (this.OutputPath.Text != replaceTo)
                        this.OutputPath.Text = replaceTo;

				}
				catch (Exception)
				{
				}
			}
		}

		private void OutputPath_TextChanged(object sender, EventArgs e)
		{

			if (this.SyncronizeTitleAndFileName.Checked)
			{
				try
				{
					string replaceTo = Path.GetFileNameWithoutExtension(this.OutputPath.Text);

					if (this.Title.Text != replaceTo)
                        this.Title.Text = replaceTo;

				}
				catch (Exception)
				{       // path contains invalid char.
				}
			}

			if (string.IsNullOrWhiteSpace(this.OutputPath.Text) || this.OutputPath.Text.ToCharArray().Intersect(Path.GetInvalidPathChars()).Any())
			{
                this.OutputPath.BackColor = Color.MistyRose;
			}
			else if (File.Exists(this.OutputPath.Text))
			{
                this.OutputPath.BackColor = Color.Moccasin;
			}
			else
			{
                this.OutputPath.BackColor = SystemColors.Window;
			}
		}



		private void AutoSetFileNameToDate_CheckedChanged(object sender, EventArgs e)
		{

			if (this.AutoSetFileNameToDate.Checked)
			{
				try
				{

                    this.OutputPath.Text = Path.GetDirectoryName(this.OutputPath.Text) + "\\" + Utility.Mathematics.DateTimeHelper.GetTimeStamp() + Path.GetExtension(this.OutputPath.Text);

				}
				catch (Exception)
				{
				}
			}

		}


		private void SyncronizeTitleAndFileName_CheckedChanged(object sender, EventArgs e)
		{

			if (this.SyncronizeTitleAndFileName.Checked)
			{

				if (string.IsNullOrWhiteSpace(this.OutputPath.Text))
				{
                    this.Title_TextChanged(sender, e);

				}
				else
				{
                    this.OutputPath_TextChanged(sender, e);
				}

			}

		}

		private void SearchOutputPath_Click(object sender, EventArgs e)
		{

			try
			{
                this.SaveImageDialog.FileName = System.IO.Path.GetFileName(this.OutputPath.Text);
                this.SaveImageDialog.InitialDirectory = string.IsNullOrWhiteSpace(this.OutputPath.Text) ? "" : System.IO.Path.GetDirectoryName(this.OutputPath.Text);
			}
			catch (Exception)
			{
			}
			if (this.SaveImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.OutputPath.Text = this.SaveImageDialog.FileName;
			}

		}



		private void DialogFleetImageGenerator_FormClosing(object sender, FormClosingEventArgs e)
		{
            this.CurrentArgument.DisposeResources();
		}



		private void OutputToClipboard_CheckedChanged(object sender, EventArgs e)
		{
            this.OutputPath.Enabled =
            this.SearchOutputPath.Enabled =
            this.OpenImageAfterOutput.Enabled =
            this.DisableOverwritePrompt.Enabled =
            this.AutoSetFileNameToDate.Enabled =
            this.SyncronizeTitleAndFileName.Enabled =
				!this.OutputToClipboard.Checked;

            this.ToolTipInfo.SetToolTip(this.GroupOutputPath, this.OutputToClipboard.Checked ? "클립보드에 출력됩니다.\r\n파일로 출력하고자 하는 경우 고급 탭의 '클립보드에 출력하기' 체크를 해제하면 됩니다." : null);
		}

		private void Comment_KeyDown(object sender, KeyEventArgs e)
		{

			// Multiline == true の TextBox では、 Ctrl-A ショートカットが無効化されるらしいので自家実装

			if (e.Control && e.KeyCode == Keys.A)
			{
				if (sender != null)
				{
					((TextBox)sender).SelectAll();
				}
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}



		private void DialogFleetImageGenerator_FormClosed(object sender, FormClosedEventArgs e)
		{
			ResourceManager.DestroyIcon(this.Icon);
		}


	}
}
