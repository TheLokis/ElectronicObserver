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
			InitializeComponent();

			TextFontList = new TextBox[]{
				TextTitleFont,
				TextLargeFont,
				TextMediumFont,
				TextSmallFont,
				TextMediumDigitFont,
				TextSmallDigitFont,
			};

			for (int i = 0; i < TextFontList.Length; i++)
			{
				int x = i;
				this.Controls.Find("Select" + TextFontList[i].Name.Remove(0, 4), true).First().Click += (sender, e) => SelectFont_Click(sender, e, x);
			}

			LoadConfiguration();

		}

		public DialogFleetImageGenerator(int fleetID)
			: this()
		{

			if (KCDatabase.Instance.Fleet.CombinedFlag > 0 && fleetID <= 2)
				CurrentArgument.FleetIDs = new int[] { 1, 2 };
			else
				CurrentArgument.FleetIDs = new int[] { fleetID };
		}



		private void DialogFleetImageGenerator_Load(object sender, EventArgs e)
		{

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleetImageGenerator]);

			ApplyToUI(CurrentArgument);

			UpdateButtonAlert();
		}



		private void LoadConfiguration()
		{
			var config = Utility.Configuration.Config.FleetImageGenerator;

			CurrentArgument = config.Argument.Clone();


			switch (config.ImageType)
			{
				case 0:
				default:
					ImageTypeCard.Checked = true;
					break;
				case 1:
					ImageTypeCutin.Checked = true;
					break;
				case 2:
					ImageTypeBanner.Checked = true;
					break;
			}

			OutputToClipboard.Checked = config.OutputType == 1;
			OpenImageAfterOutput.Checked = config.OpenImageAfterOutput;
			DisableOverwritePrompt.Checked = config.DisableOverwritePrompt;

			OutputPath.Text = config.LastOutputPath;
			try
			{
				SaveImageDialog.FileName = System.IO.Path.GetFileName(config.LastOutputPath);
				SaveImageDialog.InitialDirectory = string.IsNullOrWhiteSpace(config.LastOutputPath) ? "" : System.IO.Path.GetDirectoryName(config.LastOutputPath);
			}
			catch (Exception)
			{
			}

			SyncronizeTitleAndFileName.Checked = config.SyncronizeTitleAndFileName;
			AutoSetFileNameToDate.Checked = config.AutoSetFileNameToDate;

		}

		private void SaveConfiguration()
		{
			var config = Utility.Configuration.Config.FleetImageGenerator;

			if (config.Argument != null)
				config.Argument.DisposeResources();

			config.Argument = CurrentArgument.Clone();

			if (ImageTypeCard.Checked)
				config.ImageType = 0;
			else if (ImageTypeCutin.Checked)
				config.ImageType = 1;
			else if (ImageTypeBanner.Checked)
				config.ImageType = 2;

			config.OutputType = OutputToClipboard.Checked ? 1 : 0;
			config.OpenImageAfterOutput = OpenImageAfterOutput.Checked;
			config.DisableOverwritePrompt = DisableOverwritePrompt.Checked;
			config.AutoSetFileNameToDate = AutoSetFileNameToDate.Checked;
			config.SyncronizeTitleAndFileName = SyncronizeTitleAndFileName.Checked;

			config.LastOutputPath = OutputPath.Text;
		}



		private void ApplyToUI(FleetImageArgument args)
		{

			int[] fleetIDs = args.FleetIDs ?? new int[0];

			TargetFleet1.Checked = fleetIDs.Contains(1);
			TargetFleet2.Checked = fleetIDs.Contains(2);
			TargetFleet3.Checked = fleetIDs.Contains(3);
			TargetFleet4.Checked = fleetIDs.Contains(4);

			if (!SyncronizeTitleAndFileName.Checked)
				Title.Text = args.Title;
			Comment.Text = string.IsNullOrWhiteSpace(args.Comment) ? "" : LFtoCRLF.Replace(args.Comment, "\r\n");       // 保存データからのロード時に \n に変換されてしまっているため


			HorizontalFleetCount.Value = args.HorizontalFleetCount;
			HorizontalShipCount.Value = args.HorizontalShipCount;

			ReflectDamageGraphic.Checked = args.ReflectDamageGraphic;
			AvoidTwitterDeterioration.Checked = args.AvoidTwitterDeterioration;

			BackgroundImagePath.Text = args.BackgroundImagePath;

			for (int i = 0; i < TextFontList.Length; i++)
			{
				TextFontList[i].Text = SerializableFont.FontToString(args.Fonts[i], true);
			}
		}

		private FleetImageArgument ApplyToArgument(FleetImageArgument defaultValue = null)
		{

			var ret = defaultValue?.Clone() ?? new FleetImageArgument();

			ret.FleetIDs = new[]{
				TargetFleet1.Checked ? 1 : 0,
				TargetFleet2.Checked ? 2 : 0,
				TargetFleet3.Checked ? 3 : 0,
				TargetFleet4.Checked ? 4 : 0
			}.Where(i => i > 0).ToArray();

			ret.HorizontalFleetCount = (int)HorizontalFleetCount.Value;
			ret.HorizontalShipCount = (int)HorizontalShipCount.Value;

			ret.ReflectDamageGraphic = ReflectDamageGraphic.Checked;
			ret.AvoidTwitterDeterioration = AvoidTwitterDeterioration.Checked;

			var fonts = ret.Fonts;
			for (int i = 0; i < fonts.Length; i++)
			{
				if (fonts[i] != null)
					fonts[i].Dispose();
				fonts[i] = SerializableFont.StringToFont(TextFontList[i].Text, true);
			}
			ret.Fonts = fonts;

			ret.BackgroundImagePath = BackgroundImagePath.Text;

			ret.Title = Title.Text;
			ret.Comment = Comment.Text;

			return ret;
		}

		private int[] ToFleetIDs()
		{
			return new[]{
				TargetFleet1.Checked ? 1 : 0,
				TargetFleet2.Checked ? 2 : 0,
				TargetFleet3.Checked ? 3 : 0,
				TargetFleet4.Checked ? 4 : 0
			}.Where(i => i > 0).ToArray();
		}


		private void ApplyGeneralFont_Click(object sender, EventArgs e)
		{

			if (GeneralFont != null)
			{
				GeneralFont.Dispose();
			}
			GeneralFont = SerializableFont.StringToFont(TextGeneralFont.Text, true);

			if (GeneralFont == null)
			{
				MessageBox.Show("폰트 이름이 올바르지 않습니다.", "폰트 변환 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
				TextGeneralFont.Text = "";
				return;
			}


			for (int i = 0; i < TextFontList.Length; i++)
			{
				float size = FleetImageArgument.DefaultFontPixels[i];
				var unit = GraphicsUnit.Pixel;
				var style = FontStyle.Regular;

				var font = SerializableFont.StringToFont(TextFontList[i].Text, true);
				if (font != null)
				{
					size = font.Size;
					unit = font.Unit;
					style = font.Style;
					font.Dispose();
				}

				font = new Font(GeneralFont.FontFamily, size, style, unit);
				TextFontList[i].Text = SerializableFont.FontToString(font);
				font.Dispose();
			}

		}


		private void SelectGeneralFont_Click(object sender, EventArgs e)
		{
			fontDialog1.Font = GeneralFont;
			if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				GeneralFont = fontDialog1.Font;
				TextGeneralFont.Text = SerializableFont.FontToString(GeneralFont, true);
			}
		}

		private void SelectFont_Click(object sender, EventArgs e, int index)
		{
			fontDialog1.Font = SerializableFont.StringToFont(TextFontList[index].Text, true);
			if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TextFontList[index].Text = SerializableFont.FontToString(fontDialog1.Font, true);
			}
		}


		private void SearchBackgroundImagePath_Click(object sender, EventArgs e)
		{
			OpenImageDialog.FileName = BackgroundImagePath.Text;
			if (OpenImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				BackgroundImagePath.Text = OpenImageDialog.FileName;
			}
		}

		private void ClearBackgroundPath_Click(object sender, EventArgs e)
		{
			BackgroundImagePath.Text = "";
		}




		private void ButtonOK_Click(object sender, EventArgs e)
		{

			var args = ApplyToArgument();

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

			if (!OutputToClipboard.Checked)
			{
				if (string.IsNullOrWhiteSpace(OutputPath.Text))
				{
					MessageBox.Show("출력 파일 이름이 입력되어 있지 않습니다.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					args.DisposeResources();
					return;
				}

				if (OutputPath.Text.ToCharArray().Intersect(Path.GetInvalidPathChars()).Any())
				{
					MessageBox.Show("사용할 수 없는 문자가 포함되어 있습니다.", "값 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					args.DisposeResources();
					return;
				}

				if (!DisableOverwritePrompt.Checked && File.Exists(OutputPath.Text))
				{
					if (MessageBox.Show(Path.GetFileName(OutputPath.Text) + "\r\n은 이미 존재하는 파일입니다.\r\n덮어 쓰시겠습니까?", "덮어쓰기 확인",
						MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
						== System.Windows.Forms.DialogResult.No)
					{
						args.DisposeResources();
						return;
					}
				}
			}

			int mode;
			if (ImageTypeCard.Checked)
				mode = 0;
			else if (ImageTypeCutin.Checked)
				mode = 1;
			else if (ImageTypeBanner.Checked)
				mode = 2;
			else
				mode = 3;


			try
			{

				if (!OutputToClipboard.Checked)
				{

					using (var image = GenerateFleetImage(args, mode))
					{

						if (!Directory.Exists(Path.GetDirectoryName(OutputPath.Text)))
						{
							Directory.CreateDirectory(Path.GetDirectoryName(OutputPath.Text));
						}

						switch (Path.GetExtension(OutputPath.Text).ToLower())
						{
							case ".png":
							default:
								image.Save(OutputPath.Text, System.Drawing.Imaging.ImageFormat.Png);
								break;

							case ".bmp":
							case ".dib":
								image.Save(OutputPath.Text, System.Drawing.Imaging.ImageFormat.Bmp);
								break;

							case ".gif":
								image.Save(OutputPath.Text, System.Drawing.Imaging.ImageFormat.Gif);
								break;

							case ".tif":
							case ".tiff":
								image.Save(OutputPath.Text, System.Drawing.Imaging.ImageFormat.Tiff);
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

									image.Save(OutputPath.Text, codecInfo, encoderParams);
								}
								break;
						}

						if (OpenImageAfterOutput.Checked)
							System.Diagnostics.Process.Start(OutputPath.Text);


					}


				}
				else
				{

					using (var image = GenerateFleetImage(args, mode))
					{

						Clipboard.SetImage(image);
					}
				}



				if (CurrentArgument != null)
					CurrentArgument.DisposeResources();
				CurrentArgument = args;
				SaveConfiguration();

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


			Close();

		}

		private Bitmap GenerateFleetImage(FleetImageArgument args, int mode)
		{
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
			Close();
		}


		private void ImageTypeCard_CheckedChanged(object sender, EventArgs e)
		{
			if (ImageTypeCard.Checked)
				HorizontalShipCount.Value = 2;
		}

		private void ImageTypeCutin_CheckedChanged(object sender, EventArgs e)
		{
			if (ImageTypeCutin.Checked)
				HorizontalShipCount.Value = 1;
		}

		private void ImageTypeBanner_CheckedChanged(object sender, EventArgs e)
		{
			if (ImageTypeBanner.Checked)
				HorizontalShipCount.Value = 2;
		}



		private void UpdateButtonAlert()
		{

			bool visibility = false;

			if (!Utility.Configuration.Config.Connection.SaveReceivedData || !Utility.Configuration.Config.Connection.SaveSWF)
			{

				visibility = true;
				ButtonAlert.Text = "함선 이미지 저장 설정이 잘못되었습니다.(상세보기...)";

			}

			if (!FleetImageGenerator.HasShipSwfImage(ToFleetIDs()))
			{

				visibility = true;
				ButtonAlert.Text = "함선 이미지가 충분하지 않습니다.(상세보기...)";

			}

			ButtonAlert.Visible = visibility;

		}


		private void ButtonAlert_Click(object sender, EventArgs e)
		{

			if (!Utility.Configuration.Config.Connection.SaveReceivedData || !Utility.Configuration.Config.Connection.SaveSWF)
			{

				if (MessageBox.Show("편성 이미지를 출력하기 위해서는 함선 이미지 저장 설정을 활성화해야합니다.\r\n활성화 하시겠습니까?",
					"함선 이미지 저장 설정이 잘못되었습니다.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
					== System.Windows.Forms.DialogResult.Yes)
				{

					if (!Utility.Configuration.Config.Connection.SaveReceivedData)
					{
						Utility.Configuration.Config.Connection.SaveReceivedData = true;
						Utility.Configuration.Config.Connection.SaveResponse = false;       // もともと不要にしていたユーザーには res は邪魔なだけだと思うので
					}
					Utility.Configuration.Config.Connection.SaveSWF = true;

					UpdateButtonAlert();
				}

			}

			if (!FleetImageGenerator.HasShipSwfImage(ToFleetIDs()))
			{

				MessageBox.Show("현재의 함대의 함선 이미지 데이터가 부족합니다\r\n\r\n캐시를 삭제한 후 다시 로드하시기 바랍니다.\r\n칸코레 게임에서 출력 하고 싶은 함대의 편성화면을 열때\r\n함선 이미지 데이터가 저장됩니다.",
					"함선 이미지 데이터 부족", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				UpdateButtonAlert();
			}

		}



		private void TargetFleet1_CheckedChanged(object sender, EventArgs e)
		{
			UpdateButtonAlert();
		}


		private void ButtonClearFont_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show("폰트를 기본 설정으로 되돌립니다.\r\n진행하시겠습니까?", "초기화 확인",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
				 == System.Windows.Forms.DialogResult.Yes)
			{

				if (GeneralFont != null)
					GeneralFont.Dispose();
				GeneralFont = null;
				TextGeneralFont.Text = "";

				var defaultFonts = FleetImageArgument.GetDefaultFonts();
				for (int i = 0; i < TextFontList.Length; i++)
				{
					TextFontList[i].Text = SerializableFont.FontToString(defaultFonts[i]);
					defaultFonts[i].Dispose();
				}
			}
		}




		private void Title_TextChanged(object sender, EventArgs e)
		{
			if (SyncronizeTitleAndFileName.Checked)
			{
				try
				{

					string replaceTo = Path.GetDirectoryName(OutputPath.Text) + "\\" + Title.Text + Path.GetExtension(OutputPath.Text);

					if (OutputPath.Text != replaceTo)
						OutputPath.Text = replaceTo;

				}
				catch (Exception)
				{
				}
			}
		}

		private void OutputPath_TextChanged(object sender, EventArgs e)
		{

			if (SyncronizeTitleAndFileName.Checked)
			{
				try
				{
					string replaceTo = Path.GetFileNameWithoutExtension(OutputPath.Text);

					if (Title.Text != replaceTo)
						Title.Text = replaceTo;

				}
				catch (Exception)
				{       // path contains invalid char.
				}
			}

			if (string.IsNullOrWhiteSpace(OutputPath.Text) || OutputPath.Text.ToCharArray().Intersect(Path.GetInvalidPathChars()).Any())
			{
				OutputPath.BackColor = Color.MistyRose;
			}
			else if (File.Exists(OutputPath.Text))
			{
				OutputPath.BackColor = Color.Moccasin;
			}
			else
			{
				OutputPath.BackColor = SystemColors.Window;
			}
		}



		private void AutoSetFileNameToDate_CheckedChanged(object sender, EventArgs e)
		{

			if (AutoSetFileNameToDate.Checked)
			{
				try
				{

					OutputPath.Text = Path.GetDirectoryName(OutputPath.Text) + "\\" + Utility.Mathematics.DateTimeHelper.GetTimeStamp() + Path.GetExtension(OutputPath.Text);

				}
				catch (Exception)
				{
				}
			}

		}


		private void SyncronizeTitleAndFileName_CheckedChanged(object sender, EventArgs e)
		{

			if (SyncronizeTitleAndFileName.Checked)
			{

				if (string.IsNullOrWhiteSpace(OutputPath.Text))
				{
					Title_TextChanged(sender, e);

				}
				else
				{
					OutputPath_TextChanged(sender, e);
				}

			}

		}

		private void SearchOutputPath_Click(object sender, EventArgs e)
		{

			try
			{
				SaveImageDialog.FileName = System.IO.Path.GetFileName(OutputPath.Text);
				SaveImageDialog.InitialDirectory = string.IsNullOrWhiteSpace(OutputPath.Text) ? "" : System.IO.Path.GetDirectoryName(OutputPath.Text);
			}
			catch (Exception)
			{
			}
			if (SaveImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				OutputPath.Text = SaveImageDialog.FileName;
			}

		}



		private void DialogFleetImageGenerator_FormClosing(object sender, FormClosingEventArgs e)
		{
			CurrentArgument.DisposeResources();
		}



		private void OutputToClipboard_CheckedChanged(object sender, EventArgs e)
		{
			OutputPath.Enabled =
			SearchOutputPath.Enabled =
			OpenImageAfterOutput.Enabled =
			DisableOverwritePrompt.Enabled =
			AutoSetFileNameToDate.Enabled =
			SyncronizeTitleAndFileName.Enabled =
				!OutputToClipboard.Checked;

			ToolTipInfo.SetToolTip(GroupOutputPath, OutputToClipboard.Checked ? "클립보드에 출력됩니다.\r\n파일로 출력하고자 하는 경우 고급 탭의 '클립보드에 출력하기' 체크를 해제하면 됩니다." : null);
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
			ResourceManager.DestroyIcon(Icon);
		}


	}
}
