using ElectronicObserver.Notifier;
using ElectronicObserver.Observer;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogConfiguration : Form
	{

		public const string RegistryPathMaster = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\";
		public const string RegistryPathBrowserVersion = @"FEATURE_BROWSER_EMULATION\";
		public const string RegistryPathGPURendering = @"FEATURE_GPU_RENDERING\";

		public const int DefaultBrowserVersion = 11001;
		public const bool DefaultGPURendering = true;

		/// <summary> 司令部「任意アイテム表示」から除外するアイテムのIDリスト </summary>
		private readonly HashSet<int> IgnoredItems = new HashSet<int>() { 1, 2, 3, 4, 50, 51, 66, 67, 69 };

		private System.Windows.Forms.Control _UIControl;

		private Dictionary<SyncBGMPlayer.SoundHandleID, SyncBGMPlayer.SoundHandle> BGMHandles;


		private DateTime _shownTime;
		private double _playTimeCache;



		public DialogConfiguration()
		{
            this.InitializeComponent();

            this._shownTime = DateTime.Now;
		}

		public DialogConfiguration(Configuration.ConfigurationData config)
			: this()
		{

            this.FromConfiguration(config);
		}


		private void Connection_SaveReceivedData_CheckedChanged(object sender, EventArgs e)
		{

            this.Connection_PanelSaveData.Enabled = this.Connection_SaveReceivedData.Checked;

		}

        private void NodeToAlphabetBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Config.FormCompass.ToAlphabet = this.NodeToAlphabetBox.Checked;
        }


        private void Connection_SaveDataPath_TextChanged(object sender, EventArgs e)
		{

			if (Directory.Exists(this.Connection_SaveDataPath.Text))
			{
                this.Connection_SaveDataPath.BackColor = SystemColors.Window;
                this.ToolTipInfo.SetToolTip(this.Connection_SaveDataPath, null);
			}
			else
			{
                this.Connection_SaveDataPath.BackColor = Color.MistyRose;
                this.ToolTipInfo.SetToolTip(this.Connection_SaveDataPath, "지정된 폴더가 존재하지 않습니다.");
			}
		}


		/// <summary>
		/// パラメータの更新をUIに適用します。
		/// </summary>
		internal void UpdateParameter()
		{

            this.Connection_SaveReceivedData_CheckedChanged(null, new EventArgs());
            this.Connection_SaveDataPath_TextChanged(null, new EventArgs());
            this.Debug_EnableDebugMenu_CheckedChanged(null, new EventArgs());
            this.FormFleet_FixShipNameWidth_CheckedChanged(null, new EventArgs());
		}



		private void Connection_SaveDataPathSearch_Click(object sender, EventArgs e)
		{

            this.Connection_SaveDataPath.Text = PathHelper.ProcessFolderBrowserDialog(this.Connection_SaveDataPath.Text, this.FolderBrowser);

		}


		private void UI_MainFontSelect_Click(object sender, EventArgs e)
		{

            this.FontSelector.Font = this.UI_MainFont.Font;

			if (this.FontSelector.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{

				SerializableFont font = new SerializableFont(this.FontSelector.Font);

                this.UI_MainFont.Text = font.SerializeFontAttribute;
                this.UI_MainFont.BackColor = SystemColors.Window;
                this.UI_RenderingTest.MainFont = font.FontData;
			}

		}


		private void UI_SubFontSelect_Click(object sender, EventArgs e)
		{

            this.FontSelector.Font = this.UI_SubFont.Font;

			if (this.FontSelector.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{

				SerializableFont font = new SerializableFont(this.FontSelector.Font);

                this.UI_SubFont.Text = font.SerializeFontAttribute;
                this.UI_SubFont.BackColor = SystemColors.Window;
                this.UI_RenderingTest.SubFont = font.FontData;
			}

		}


		private void DialogConfiguration_Load(object sender, EventArgs e)
		{

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormConfiguration]);

            this._UIControl = this.Owner;

		}

		private void DialogConfiguration_FormClosed(object sender, FormClosedEventArgs e)
		{

			ResourceManager.DestroyIcon(this.Icon);

		}


		private void UI_MainFontApply_Click(object sender, EventArgs e)
		{

            this.UI_MainFont.Font = SerializableFont.StringToFont(this.UI_MainFont.Text) ?? this.UI_MainFont.Font;
		}

		private void UI_SubFontApply_Click(object sender, EventArgs e)
		{

            this.UI_SubFont.Font = SerializableFont.StringToFont(this.UI_SubFont.Text) ?? this.UI_SubFont.Font;
		}




		//ui
		private void Connection_OutputConnectionScript_Click(object sender, EventArgs e)
		{

			string serverAddress = APIObserver.Instance.ServerAddress;
			if (serverAddress == null)
			{
				MessageBox.Show("칸코레 실행후 작업하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			using (var dialog = new SaveFileDialog())
			{
				dialog.Filter = "Proxy Script|*.pac|File|*";
				dialog.Title = "자동 프록시 설정 스크립트를 저장";
				dialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
				dialog.FileName = System.IO.Directory.GetCurrentDirectory() + "\\proxy.pac";

				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					try
					{

						using (StreamWriter sw = new StreamWriter(dialog.FileName))
						{

							sw.WriteLine("function FindProxyForURL(url, host) {");
							sw.WriteLine("  if (/^" + serverAddress.Replace(".", @"\.") + "/.test(host)) {");
							sw.WriteLine("    return \"PROXY localhost:{0}; DIRECT\";", (int)this.Connection_Port.Value);
							sw.WriteLine("  }");
							sw.WriteLine("  return \"DIRECT\";");
							sw.WriteLine("}");

						}

						Clipboard.SetData(DataFormats.StringFormat, "file:///" + dialog.FileName.Replace('\\', '/'));

						MessageBox.Show("자동 프록시 설정 스크립트를 저장하고 URL구성을 클립보드에 복사했습니다. \r\n붙여넣기 하세요.",
							"작성완료", MessageBoxButtons.OK, MessageBoxIcon.Information);


					}
					catch (Exception ex)
					{

						Utility.ErrorReporter.SendErrorReport(ex, "자동 프록시 구성 스크립트를 저장하는데 실패했습니다.");
						MessageBox.Show("자동 프록시 구성 스크립트를 저장하는데 실패했습니다.\r\n" + ex.Message, "에러",
							MessageBoxButtons.OK, MessageBoxIcon.Error);

					}

				}
			}

		}



		private void Notification_Expedition_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Expedition))
			{
				dialog.ShowDialog(this);
			}
		}

		private void Notification_Construction_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Construction))
			{
				dialog.ShowDialog(this);
			}
		}

		private void Notification_Repair_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Repair))
			{
				dialog.ShowDialog(this);
			}
		}

		private void Notification_Condition_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Condition))
			{
				dialog.ShowDialog(this);
			}
		}

		private void Notification_Damage_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Damage))
			{
				dialog.ShowDialog(this);
			}
		}

		private void Notification_AnchorageRepair_Click(object sender, EventArgs e)
		{

			using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.AnchorageRepair))
			{
				dialog.ShowDialog(this);
			}
		}

        private void Notification_BaseAirCorps_Click(object sender, EventArgs e)
        {
            using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.BaseAirCorps))
            {
                dialog.ShowDialog(this);
            }
        }

        private void Life_LayoutFilePathSearch_Click(object sender, EventArgs e)
		{

            this.Life_LayoutFilePath.Text = PathHelper.ProcessOpenFileDialog(this.Life_LayoutFilePath.Text, this.LayoutFileBrowser);

		}


		private void Debug_APIListPathSearch_Click(object sender, EventArgs e)
		{

            this.Debug_APIListPath.Text = PathHelper.ProcessOpenFileDialog(this.Debug_APIListPath.Text, this.APIListBrowser);

		}


		private void Debug_EnableDebugMenu_CheckedChanged(object sender, EventArgs e)
		{

            this.Debug_SealingPanel.Visible =
            this.Connection_UpstreamProxyAddress.Visible =
            this.Connection_DownstreamProxy.Visible =
            this.Connection_DownstreamProxyLabel.Visible =
            this.SubWindow_Json_SealingPanel.Visible =
                this.Debug_EnableDebugMenu.Checked;

		}


		private void FormBrowser_ScreenShotPathSearch_Click(object sender, EventArgs e)
		{

            this.FormBrowser_ScreenShotPath.Text = PathHelper.ProcessFolderBrowserDialog(this.FormBrowser_ScreenShotPath.Text, this.FolderBrowser);
		}





		/// <summary>
		/// 設定からUIを初期化します。
		/// </summary>
		public void FromConfiguration(Configuration.ConfigurationData config)
		{

            //[通信]
            this.Connection_Port.Value = config.Connection.Port;
            this.Connection_SaveReceivedData.Checked = config.Connection.SaveReceivedData;
            this.Connection_SaveDataPath.Text = config.Connection.SaveDataPath;
            this.Connection_SaveRequest.Checked = config.Connection.SaveRequest;
            this.Connection_SaveResponse.Checked = config.Connection.SaveResponse;
            this.Connection_SaveOtherFile.Checked = config.Connection.SaveOtherFile;
            this.Connection_ApplyVersion.Checked = config.Connection.ApplyVersion;
            this.Connection_RegisterAsSystemProxy.Checked = config.Connection.RegisterAsSystemProxy;
            this.Connection_UseUpstreamProxy.Checked = config.Connection.UseUpstreamProxy;
            this.Connection_UpstreamProxyPort.Value = config.Connection.UpstreamProxyPort;
            this.Connection_UpstreamProxyAddress.Text = config.Connection.UpstreamProxyAddress;
            this.Connection_UseSystemProxy.Checked = config.Connection.UseSystemProxy;
            this.Connection_DownstreamProxy.Text = config.Connection.DownstreamProxy;

            //[UI]
            this.UI_MainFont.Text = config.UI.MainFont.SerializeFontAttribute;
            this.UI_SubFont.Text = config.UI.SubFont.SerializeFontAttribute;
            this.selectTheme.DataSource = Enum.GetValues(typeof(Theme));
            this.selectTheme.SelectedItem = config.UI.Theme;
            this.UI_BarColorMorphing.Checked = config.UI.BarColorMorphing;
            this.UI_IsLayoutFixed.Checked = config.UI.IsLayoutFixed;
			{
                this.UI_RenderingTest.MainFont = config.UI.MainFont.FontData;
                this.UI_RenderingTest.SubFont = config.UI.SubFont.FontData;
                this.UI_RenderingTest.HPBar.ColorMorphing = config.UI.BarColorMorphing;
                this.UI_RenderingTest.HPBar.SetBarColorScheme(config.UI.BarColorScheme.Select(c => c.ColorData).ToArray());
                this.UI_RenderingTestChanger.Maximum = this.UI_RenderingTest.MaximumValue;
                this.UI_RenderingTestChanger.Value = this.UI_RenderingTest.Value;
			}

            //[ログ]
            this.Log_LogLevel.Value = config.Log.LogLevel;
            this.Log_SaveLogFlag.Checked = config.Log.SaveLogFlag;
            this.Log_SaveErrorReport.Checked = config.Log.SaveErrorReport;
            this.Log_FileEncodingID.SelectedIndex = config.Log.FileEncodingID;
            this.Log_ShowSpoiler.Checked = config.Log.ShowSpoiler;
            this._playTimeCache = config.Log.PlayTime;
            this.UpdatePlayTime();
            this.Log_SaveBattleLog.Checked = config.Log.SaveBattleLog;
            this.Log_SaveLogImmediately.Checked = config.Log.SaveLogImmediately;

            //[動作]
            this.Control_ConditionBorder.Value = config.Control.ConditionBorder;
            this.Control_RecordAutoSaving.SelectedIndex = config.Control.RecordAutoSaving;
            this.Control_UseSystemVolume.Checked = config.Control.UseSystemVolume;
            this.Control_PowerEngagementForm.SelectedIndex = config.Control.PowerEngagementForm - 1;
            this.Control_ShowSallyAreaAlertDialog.Checked = config.Control.ShowSallyAreaAlertDialog;
            this.Control_ShowExpeditionAlertDialog.Checked = config.Control.ShowExpeditionAlertDialog;

            //[デバッグ]
            this.Debug_EnableDebugMenu.Checked = config.Debug.EnableDebugMenu;
            this.Debug_LoadAPIListOnLoad.Checked = config.Debug.LoadAPIListOnLoad;
            this.Debug_APIListPath.Text = config.Debug.APIListPath;
            this.Debug_AlertOnError.Checked = config.Debug.AlertOnError;

            //[起動と終了]
            this.Life_ConfirmOnClosing.Checked = config.Life.ConfirmOnClosing;
            this.Life_TopMost.Checked = this.TopMost = config.Life.TopMost;      //メインウィンドウに隠れないように
            this.Life_LayoutFilePath.Text = config.Life.LayoutFilePath;
            this.Life_CheckUpdateInformation.Checked = config.Life.CheckUpdateInformation;
            this.Life_ShowStatusBar.Checked = config.Life.ShowStatusBar;
            this.Life_ClockFormat.SelectedIndex = config.Life.ClockFormat;
            this.Life_LockLayout.Checked = config.Life.LockLayout;
            this.Life_CanCloseFloatWindowInLock.Checked = config.Life.CanCloseFloatWindowInLock;
            this.Life_CanSizableFloatWindowInLock.Checked = config.Life.CanSizableFloatWindowInLock;

            //[サブウィンドウ]
            this.FormArsenal_ShowShipName.Checked = config.FormArsenal.ShowShipName;
            this.FormArsenal_BlinkAtCompletion.Checked = config.FormArsenal.BlinkAtCompletion;
            this.FormArsenal_MaxShipNameWidth.Value = config.FormArsenal.MaxShipNameWidth;

            this.FormDock_BlinkAtCompletion.Checked = config.FormDock.BlinkAtCompletion;
            this.FormDock_MaxShipNameWidth.Value = config.FormDock.MaxShipNameWidth;

            this.FormFleet_ShowAircraft.Checked = config.FormFleet.ShowAircraft;
            this.FormFleet_SearchingAbilityMethod.SelectedIndex = config.FormFleet.SearchingAbilityMethod;
            this.FormFleet_IsScrollable.Checked = config.FormFleet.IsScrollable;
            this.FormFleet_FixShipNameWidth.Checked = config.FormFleet.FixShipNameWidth;
            this.FormFleet_ShortenHPBar.Checked = config.FormFleet.ShortenHPBar;
            this.FormFleet_ShowNextExp.Checked = config.FormFleet.ShowNextExp;
            this.FormFleet_EquipmentLevelVisibility.SelectedIndex = (int)config.FormFleet.EquipmentLevelVisibility;
            this.FormFleet_ShowAircraftLevelByNumber.Checked = config.FormFleet.ShowAircraftLevelByNumber;
            this.FormFleet_AirSuperiorityMethod.SelectedIndex = config.FormFleet.AirSuperiorityMethod;
            this.ExpCheckerOpList.SelectedIndex = config.FormFleet.ExpCheckerOption;
            this.FormFleet_ShowAnchorageRepairingTimer.Checked = config.FormFleet.ShowAnchorageRepairingTimer;
            this.FormFleet_BlinkAtCompletion.Checked = config.FormFleet.BlinkAtCompletion;
            this.FormFleet_ShowConditionIcon.Checked = config.FormFleet.ShowConditionIcon;
            this.FormFleet_FixedShipNameWidth.Value = config.FormFleet.FixedShipNameWidth;
            this.FormFleet_ShowAirSuperiorityRange.Checked = config.FormFleet.ShowAirSuperiorityRange;
            this.FormFleet_ReflectAnchorageRepairHealing.Checked = config.FormFleet.ReflectAnchorageRepairHealing;
            this.FormFleet_BlinkAtDamaged.Checked = config.FormFleet.BlinkAtDamaged;
            this.FormFleet_EmphasizesSubFleetInPort.Checked = config.FormFleet.EmphasizesSubFleetInPort;
            this.FormFleet_FleetStateDisplayMode.SelectedIndex = config.FormFleet.FleetStateDisplayMode;
            this.FormFleet_AppliesSallyAreaColor.Checked = config.FormFleet.AppliesSallyAreaColor;
            this.FormFleet_FocusModifiedFleet.Checked = config.FormFleet.FocusModifiedFleet;
            this.FormHeadquarters_BlinkAtMaximum.Checked = config.FormHeadquarters.BlinkAtMaximum;
            this.FormHeadquarters_Visibility.Items.Clear();
            this.FormHeadquarters_Visibility.Items.AddRange(FormHeadquarters.GetItemNames().ToArray());
			FormHeadquarters.CheckVisibilityConfiguration();
			for (int i = 0; i < this.FormHeadquarters_Visibility.Items.Count; i++)
			{
                this.FormHeadquarters_Visibility.SetItemChecked(i, config.FormHeadquarters.Visibility.List[i]);
			}

			{
                this.FormHeadquarters_DisplayUseItemID.Items.AddRange(
					ElectronicObserver.Data.KCDatabase.Instance.MasterUseItems.Values
						.Where(i => i.Name.Length > 0 && i.Description.Length > 0 && !this.IgnoredItems.Contains(i.ItemID))
						.Select(i => i.Name).ToArray());
				var item = ElectronicObserver.Data.KCDatabase.Instance.MasterUseItems[config.FormHeadquarters.DisplayUseItemID];

				if (item != null)
				{
                    this.FormHeadquarters_DisplayUseItemID.Text = item.Name;
				}
				else
				{
                    this.FormHeadquarters_DisplayUseItemID.Text = config.FormHeadquarters.DisplayUseItemID.ToString();
				}
			}

            this.FormQuest_ShowRunningOnly.Checked = config.FormQuest.ShowRunningOnly;
            this.FormQuest_ShowOnce.Checked = config.FormQuest.ShowOnce;
            this.FormQuest_ShowDaily.Checked = config.FormQuest.ShowDaily;
            this.FormQuest_ShowWeekly.Checked = config.FormQuest.ShowWeekly;
            this.FormQuest_ShowMonthly.Checked = config.FormQuest.ShowMonthly;
            this.FormQuest_ShowOther.Checked = config.FormQuest.ShowOther;
            this.FormQuest_ProgressAutoSaving.SelectedIndex = config.FormQuest.ProgressAutoSaving;
            this.FormQuest_AllowUserToSortRows.Checked = config.FormQuest.AllowUserToSortRows;

            this.FormShipGroup_AutoUpdate.Checked = config.FormShipGroup.AutoUpdate;
            this.FormShipGroup_ShowStatusBar.Checked = config.FormShipGroup.ShowStatusBar;
            this.FormShipGroup_ShipNameSortMethod.SelectedIndex = config.FormShipGroup.ShipNameSortMethod;

            this.FormBattle_IsScrollable.Checked = config.FormBattle.IsScrollable;
            this.FormBattle_HideDuringBattle.Checked = config.FormBattle.HideDuringBattle;
            this.FormBattle_ShowHPBar.Checked = config.FormBattle.ShowHPBar;
            this.FormBattle_ShowShipTypeInHPBar.Checked = config.FormBattle.ShowShipTypeInHPBar;
            this.FormBattle_Display7thAsSingleLine.Checked = config.FormBattle.Display7thAsSingleLine;

            this.FormBrowser_IsEnabled.Checked = config.FormBrowser.IsEnabled;
            this.FormBrowser_ZoomRate.Value = (decimal)Math.Min(Math.Max(config.FormBrowser.ZoomRate * 100, 10), 1000);
            this.FormBrowser_ZoomFit.Checked = config.FormBrowser.ZoomFit;
            this.FormBrowser_LogInPageURL.Text = config.FormBrowser.LogInPageURL;
            this.FormBrowser_ScreenShotFormat_JPEG.Checked = config.FormBrowser.ScreenShotFormat == 1;
            this.FormBrowser_ScreenShotFormat_PNG.Checked = config.FormBrowser.ScreenShotFormat == 2;
            this.FormBrowser_ScreenShotPath.Text = config.FormBrowser.ScreenShotPath;
            this.FormBrowser_ConfirmAtRefresh.Checked = config.FormBrowser.ConfirmAtRefresh;
            this.FormBrowser_AppliesStyleSheet.Checked = config.FormBrowser.AppliesStyleSheet;
            this.FormBrowser_IsDMMreloadDialogDestroyable.Checked = config.FormBrowser.IsDMMreloadDialogDestroyable;
            this.FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Checked = config.FormBrowser.AvoidTwitterDeterioration;
            this.FormBrowser_ScreenShotSaveMode.SelectedIndex = config.FormBrowser.ScreenShotSaveMode - 1;
            this.FormBrowser_HardwareAccelerationEnabled.Checked = config.FormBrowser.HardwareAccelerationEnabled;
            this.FormBrowser_PreserveDrawingBuffer.Checked = config.FormBrowser.PreserveDrawingBuffer;
            this.FormBrowser_ForceColorProfile.Checked = config.FormBrowser.ForceColorProfile;
            this.FormBrowser_SavesBrowserLog.Checked = config.FormBrowser.SavesBrowserLog;

            if (!config.FormBrowser.IsToolMenuVisible)
                this.FormBrowser_ToolMenuDockStyle.SelectedIndex = 4;
			else
                this.FormBrowser_ToolMenuDockStyle.SelectedIndex = (int)config.FormBrowser.ToolMenuDockStyle - 1;

            this.FormCompass_CandidateDisplayCount.Value = config.FormCompass.CandidateDisplayCount;
            this.FormCompass_IsScrollable.Checked = config.FormCompass.IsScrollable;
            this.NodeToAlphabetBox.Checked = config.FormCompass.ToAlphabet;
            this.FormCompass_MaxShipNameWidth.Value = config.FormCompass.MaxShipNameWidth;

            this.FormJson_AutoUpdate.Checked = config.FormJson.AutoUpdate;
            this.FormJson_UpdatesTree.Checked = config.FormJson.UpdatesTree;
            this.FormJson_AutoUpdateFilter.Text = config.FormJson.AutoUpdateFilter;

            this.FormBaseAirCorps_ShowEventMapOnly.Checked = config.FormBaseAirCorps.ShowEventMapOnly;


			//[通知]
			{
				bool issilenced = NotifierManager.Instance.GetNotifiers().All(no => no.IsSilenced);
                this.Notification_Silencio.Checked = issilenced;
                this.setSilencioConfig(issilenced);
			}

            //[BGM]
            this.BGMPlayer_Enabled.Checked = config.BGMPlayer.Enabled;
            this.BGMHandles = config.BGMPlayer.Handles.ToDictionary(h => h.HandleID);
            this.BGMPlayer_SyncBrowserMute.Checked = config.BGMPlayer.SyncBrowserMute;
            this.UpdateBGMPlayerUI();

            //finalize
            this.UpdateParameter();
		}



		/// <summary>
		/// UIをもとに設定を適用します。
		/// </summary>
		public void ToConfiguration(Configuration.ConfigurationData config)
		{

			//[通信]
			{
				bool changed = false;

				changed |= config.Connection.Port != (ushort)this.Connection_Port.Value;
				config.Connection.Port = (ushort)this.Connection_Port.Value;

				config.Connection.SaveReceivedData = this.Connection_SaveReceivedData.Checked;
				config.Connection.SaveDataPath = this.Connection_SaveDataPath.Text.Trim(@"\ """.ToCharArray());
				config.Connection.SaveRequest = this.Connection_SaveRequest.Checked;
				config.Connection.SaveResponse = this.Connection_SaveResponse.Checked;
				config.Connection.SaveOtherFile = this.Connection_SaveOtherFile.Checked;
				config.Connection.ApplyVersion = this.Connection_ApplyVersion.Checked;

				changed |= config.Connection.RegisterAsSystemProxy != this.Connection_RegisterAsSystemProxy.Checked;
				config.Connection.RegisterAsSystemProxy = this.Connection_RegisterAsSystemProxy.Checked;

				changed |= config.Connection.UseUpstreamProxy != this.Connection_UseUpstreamProxy.Checked;
				config.Connection.UseUpstreamProxy = this.Connection_UseUpstreamProxy.Checked;
				changed |= config.Connection.UpstreamProxyPort != (ushort)this.Connection_UpstreamProxyPort.Value;
				config.Connection.UpstreamProxyPort = (ushort)this.Connection_UpstreamProxyPort.Value;
				changed |= config.Connection.UpstreamProxyAddress != this.Connection_UpstreamProxyAddress.Text;
				config.Connection.UpstreamProxyAddress = this.Connection_UpstreamProxyAddress.Text;

				changed |= config.Connection.UseSystemProxy != this.Connection_UseSystemProxy.Checked;
				config.Connection.UseSystemProxy = this.Connection_UseSystemProxy.Checked;

				changed |= config.Connection.DownstreamProxy != this.Connection_DownstreamProxy.Text;
				config.Connection.DownstreamProxy = this.Connection_DownstreamProxy.Text;

				if (changed)
				{
					APIObserver.Instance.Start(config.Connection.Port, this._UIControl);
				}

			}

			//[UI]
			{
				var newfont = SerializableFont.StringToFont(this.UI_MainFont.Text, true);
				if (newfont != null)
					config.UI.MainFont = newfont;
			}
			{
				var newfont = SerializableFont.StringToFont(this.UI_SubFont.Text, true);
				if (newfont != null)
					config.UI.SubFont = newfont;
			}
			config.UI.BarColorMorphing = this.UI_BarColorMorphing.Checked;
			config.UI.IsLayoutFixed = this.UI_IsLayoutFixed.Checked;

            Theme theme;
            Enum.TryParse(this.selectTheme.SelectedValue.ToString(), out theme);
            config.UI.Theme = theme;

            //[ログ]
            config.Log.LogLevel = (int)this.Log_LogLevel.Value;
			config.Log.SaveLogFlag = this.Log_SaveLogFlag.Checked;
			config.Log.SaveErrorReport = this.Log_SaveErrorReport.Checked;
			config.Log.FileEncodingID = this.Log_FileEncodingID.SelectedIndex;
			config.Log.ShowSpoiler = this.Log_ShowSpoiler.Checked;
			config.Log.SaveBattleLog = this.Log_SaveBattleLog.Checked;
			config.Log.SaveLogImmediately = this.Log_SaveLogImmediately.Checked;

			//[動作]
			config.Control.ConditionBorder = (int)this.Control_ConditionBorder.Value;
			config.Control.RecordAutoSaving = this.Control_RecordAutoSaving.SelectedIndex;
			config.Control.UseSystemVolume = this.Control_UseSystemVolume.Checked;
			config.Control.PowerEngagementForm = this.Control_PowerEngagementForm.SelectedIndex + 1;
			config.Control.ShowSallyAreaAlertDialog = this.Control_ShowSallyAreaAlertDialog.Checked;
            config.Control.ShowExpeditionAlertDialog = this.Control_ShowExpeditionAlertDialog.Checked;

            //[デバッグ]
            config.Debug.EnableDebugMenu = this.Debug_EnableDebugMenu.Checked;
			config.Debug.LoadAPIListOnLoad = this.Debug_LoadAPIListOnLoad.Checked;
			config.Debug.APIListPath = this.Debug_APIListPath.Text;
			config.Debug.AlertOnError = this.Debug_AlertOnError.Checked;

			//[起動と終了]
			config.Life.ConfirmOnClosing = this.Life_ConfirmOnClosing.Checked;
			config.Life.TopMost = this.Life_TopMost.Checked;
			config.Life.LayoutFilePath = this.Life_LayoutFilePath.Text;
			config.Life.CheckUpdateInformation = this.Life_CheckUpdateInformation.Checked;
			config.Life.ShowStatusBar = this.Life_ShowStatusBar.Checked;
			config.Life.ClockFormat = this.Life_ClockFormat.SelectedIndex;
			config.Life.LockLayout = this.Life_LockLayout.Checked;
			config.Life.CanCloseFloatWindowInLock = this.Life_CanCloseFloatWindowInLock.Checked;
            config.Life.CanSizableFloatWindowInLock = this.Life_CanSizableFloatWindowInLock.Checked;

            //[サブウィンドウ]
            config.FormArsenal.ShowShipName = this.FormArsenal_ShowShipName.Checked;
			config.FormArsenal.BlinkAtCompletion = this.FormArsenal_BlinkAtCompletion.Checked;
			config.FormArsenal.MaxShipNameWidth = (int)this.FormArsenal_MaxShipNameWidth.Value;

			config.FormDock.BlinkAtCompletion = this.FormDock_BlinkAtCompletion.Checked;
			config.FormDock.MaxShipNameWidth = (int)this.FormDock_MaxShipNameWidth.Value;

			config.FormFleet.ShowAircraft = this.FormFleet_ShowAircraft.Checked;
			config.FormFleet.SearchingAbilityMethod = this.FormFleet_SearchingAbilityMethod.SelectedIndex;
			config.FormFleet.IsScrollable = this.FormFleet_IsScrollable.Checked;
			config.FormFleet.FixShipNameWidth = this.FormFleet_FixShipNameWidth.Checked;
			config.FormFleet.ShortenHPBar = this.FormFleet_ShortenHPBar.Checked;
			config.FormFleet.ShowNextExp = this.FormFleet_ShowNextExp.Checked;
			config.FormFleet.EquipmentLevelVisibility = (Window.Control.ShipStatusEquipment.LevelVisibilityFlag)this.FormFleet_EquipmentLevelVisibility.SelectedIndex;
			config.FormFleet.ShowAircraftLevelByNumber = this.FormFleet_ShowAircraftLevelByNumber.Checked;
			config.FormFleet.AirSuperiorityMethod = this.FormFleet_AirSuperiorityMethod.SelectedIndex;
            config.FormFleet.ExpCheckerOption = this.ExpCheckerOpList.SelectedIndex;
            config.FormFleet.ShowAnchorageRepairingTimer = this.FormFleet_ShowAnchorageRepairingTimer.Checked;
			config.FormFleet.BlinkAtCompletion = this.FormFleet_BlinkAtCompletion.Checked;
			config.FormFleet.ShowConditionIcon = this.FormFleet_ShowConditionIcon.Checked;
			config.FormFleet.FixedShipNameWidth = (int)this.FormFleet_FixedShipNameWidth.Value;
			config.FormFleet.ShowAirSuperiorityRange = this.FormFleet_ShowAirSuperiorityRange.Checked;
			config.FormFleet.ReflectAnchorageRepairHealing = this.FormFleet_ReflectAnchorageRepairHealing.Checked;
			config.FormFleet.BlinkAtDamaged = this.FormFleet_BlinkAtDamaged.Checked;
			config.FormFleet.EmphasizesSubFleetInPort = this.FormFleet_EmphasizesSubFleetInPort.Checked;
			config.FormFleet.FleetStateDisplayMode = this.FormFleet_FleetStateDisplayMode.SelectedIndex;
            config.FormFleet.AppliesSallyAreaColor = this.FormFleet_AppliesSallyAreaColor.Checked;
            config.FormFleet.FocusModifiedFleet = this.FormFleet_FocusModifiedFleet.Checked;
            config.FormHeadquarters.BlinkAtMaximum = this.FormHeadquarters_BlinkAtMaximum.Checked;
			{
				var list = new List<bool>();
				for (int i = 0; i < this.FormHeadquarters_Visibility.Items.Count; i++)
					list.Add(this.FormHeadquarters_Visibility.GetItemChecked(i));
				config.FormHeadquarters.Visibility.List = list;
			}
			{
				string name = this.FormHeadquarters_DisplayUseItemID.Text;
				if (string.IsNullOrEmpty(name))
				{
					config.FormHeadquarters.DisplayUseItemID = -1;

				}
				else
				{
					var item = ElectronicObserver.Data.KCDatabase.Instance.MasterUseItems.Values.FirstOrDefault(p => p.Name == name);

					if (item != null)
					{
						config.FormHeadquarters.DisplayUseItemID = item.ItemID;

					}
					else
					{
						if (int.TryParse(name, out int val))
							config.FormHeadquarters.DisplayUseItemID = val;
						else
							config.FormHeadquarters.DisplayUseItemID = -1;
					}
				}
			}

			config.FormQuest.ShowRunningOnly = this.FormQuest_ShowRunningOnly.Checked;
			config.FormQuest.ShowOnce = this.FormQuest_ShowOnce.Checked;
			config.FormQuest.ShowDaily = this.FormQuest_ShowDaily.Checked;
			config.FormQuest.ShowWeekly = this.FormQuest_ShowWeekly.Checked;
			config.FormQuest.ShowMonthly = this.FormQuest_ShowMonthly.Checked;
			config.FormQuest.ShowOther = this.FormQuest_ShowOther.Checked;
			config.FormQuest.ProgressAutoSaving = this.FormQuest_ProgressAutoSaving.SelectedIndex;
			config.FormQuest.AllowUserToSortRows = this.FormQuest_AllowUserToSortRows.Checked;

			config.FormShipGroup.AutoUpdate = this.FormShipGroup_AutoUpdate.Checked;
			config.FormShipGroup.ShowStatusBar = this.FormShipGroup_ShowStatusBar.Checked;
			config.FormShipGroup.ShipNameSortMethod = this.FormShipGroup_ShipNameSortMethod.SelectedIndex;

			config.FormBattle.IsScrollable = this.FormBattle_IsScrollable.Checked;
			config.FormBattle.HideDuringBattle = this.FormBattle_HideDuringBattle.Checked;
			config.FormBattle.ShowHPBar = this.FormBattle_ShowHPBar.Checked;
			config.FormBattle.ShowShipTypeInHPBar = this.FormBattle_ShowShipTypeInHPBar.Checked;
			config.FormBattle.Display7thAsSingleLine = this.FormBattle_Display7thAsSingleLine.Checked;

			config.FormBrowser.IsEnabled = this.FormBrowser_IsEnabled.Checked;
            config.FormBrowser.ZoomRate = (double)this.FormBrowser_ZoomRate.Value / 100;
            config.FormBrowser.ZoomFit = this.FormBrowser_ZoomFit.Checked;
			config.FormBrowser.LogInPageURL = this.FormBrowser_LogInPageURL.Text;
			if (this.FormBrowser_ScreenShotFormat_JPEG.Checked)
				config.FormBrowser.ScreenShotFormat = 1;
			else
				config.FormBrowser.ScreenShotFormat = 2;
			config.FormBrowser.ScreenShotPath = this.FormBrowser_ScreenShotPath.Text;
			config.FormBrowser.ConfirmAtRefresh = this.FormBrowser_ConfirmAtRefresh.Checked;
			config.FormBrowser.AppliesStyleSheet = this.FormBrowser_AppliesStyleSheet.Checked;
			config.FormBrowser.IsDMMreloadDialogDestroyable = this.FormBrowser_IsDMMreloadDialogDestroyable.Checked;
            config.FormBrowser.HardwareAccelerationEnabled = this.FormBrowser_HardwareAccelerationEnabled.Checked;
            config.FormBrowser.AvoidTwitterDeterioration = this.FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Checked;
            config.FormBrowser.PreserveDrawingBuffer = this.FormBrowser_PreserveDrawingBuffer.Checked;
            config.FormBrowser.ForceColorProfile = this.FormBrowser_ForceColorProfile.Checked;
            config.FormBrowser.SavesBrowserLog = this.FormBrowser_SavesBrowserLog.Checked;

            if (this.FormBrowser_ToolMenuDockStyle.SelectedIndex == 4)
			{
				config.FormBrowser.IsToolMenuVisible = false;
			}
			else
			{
				config.FormBrowser.IsToolMenuVisible = true;
				config.FormBrowser.ToolMenuDockStyle = (DockStyle)(this.FormBrowser_ToolMenuDockStyle.SelectedIndex + 1);
			}
			config.FormBrowser.ScreenShotSaveMode = this.FormBrowser_ScreenShotSaveMode.SelectedIndex + 1;

			config.FormCompass.CandidateDisplayCount = (int)this.FormCompass_CandidateDisplayCount.Value;
			config.FormCompass.IsScrollable = this.FormCompass_IsScrollable.Checked;
            config.FormCompass.ToAlphabet = this.NodeToAlphabetBox.Checked;
			config.FormCompass.MaxShipNameWidth = (int)this.FormCompass_MaxShipNameWidth.Value;

			config.FormJson.AutoUpdate = this.FormJson_AutoUpdate.Checked;
			config.FormJson.UpdatesTree = this.FormJson_UpdatesTree.Checked;
			config.FormJson.AutoUpdateFilter = this.FormJson_AutoUpdateFilter.Text;

			config.FormBaseAirCorps.ShowEventMapOnly = this.FormBaseAirCorps_ShowEventMapOnly.Checked;


            //[通知]
            this.setSilencioConfig(this.Notification_Silencio.Checked);

			//[BGM]
			config.BGMPlayer.Enabled = this.BGMPlayer_Enabled.Checked;
			for (int i = 0; i < this.BGMPlayer_ControlGrid.Rows.Count; i++)
			{
                this.BGMHandles[(SyncBGMPlayer.SoundHandleID)this.BGMPlayer_ControlGrid[this.BGMPlayer_ColumnContent.Index, i].Value].Enabled = (bool)this.BGMPlayer_ControlGrid[this.BGMPlayer_ColumnEnabled.Index, i].Value;
			}
			config.BGMPlayer.Handles = new List<SyncBGMPlayer.SoundHandle>(this.BGMHandles.Values.ToList());
			config.BGMPlayer.SyncBrowserMute = this.BGMPlayer_SyncBrowserMute.Checked;
		}


		private void UpdateBGMPlayerUI()
		{

            this.BGMPlayer_ControlGrid.Rows.Clear();

			var rows = new DataGridViewRow[this.BGMHandles.Count];

			int i = 0;
			foreach (var h in this.BGMHandles.Values)
			{
				var row = new DataGridViewRow();
				row.CreateCells(this.BGMPlayer_ControlGrid);
				row.SetValues(h.Enabled, h.HandleID, h.Path);
				rows[i] = row;
				i++;
			}

            this.BGMPlayer_ControlGrid.Rows.AddRange(rows);

            this.BGMPlayer_VolumeAll.Value = (int)this.BGMHandles.Values.Average(h => h.Volume);
		}

		private void Database_LinkKCDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://kancolle-db.net/");
		}



		// BGMPlayer
		private void BGMPlayer_ControlGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == this.BGMPlayer_ColumnSetting.Index)
			{

				var handleID = (SyncBGMPlayer.SoundHandleID)this.BGMPlayer_ControlGrid[this.BGMPlayer_ColumnContent.Index, e.RowIndex].Value;

				using (var dialog = new DialogConfigurationBGMPlayer(this.BGMHandles[handleID]))
				{
					if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
                        this.BGMHandles[handleID] = dialog.ResultHandle;
					}
				}

                this.UpdateBGMPlayerUI();
			}
		}

		private void BGMPlayer_ControlGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == this.BGMPlayer_ColumnContent.Index)
			{
				e.Value = SyncBGMPlayer.SoundHandleIDToString((SyncBGMPlayer.SoundHandleID)e.Value);
				e.FormattingApplied = true;
			}

		}

		//for checkbox
		private void BGMPlayer_ControlGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (this.BGMPlayer_ControlGrid.Columns[this.BGMPlayer_ControlGrid.CurrentCellAddress.X] is DataGridViewCheckBoxColumn)
			{
				if (this.BGMPlayer_ControlGrid.IsCurrentCellDirty)
				{
                    this.BGMPlayer_ControlGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
				}
			}
		}

		private void BGMPlayer_SetVolumeAll_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show("모든 BGM의 볼륨을 " + (int)this.BGMPlayer_VolumeAll.Value + " 로 적용합니다. \r\n괜찮으십니까?\r\n", "볼륨 일괄 설정 확인",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
			{

				foreach (var h in this.BGMHandles.Values)
				{
					h.Volume = (int)this.BGMPlayer_VolumeAll.Value;
				}

                this.UpdateBGMPlayerUI();
			}

		}


		private void setSilencioConfig(bool silenced)
		{
			foreach (NotifierBase no in NotifierManager.Instance.GetNotifiers())
			{
				no.IsSilenced = silenced;
			}
		}


		private void UpdatePlayTime()
		{
			double elapsed = (DateTime.Now - this._shownTime).TotalSeconds;
            this.Log_PlayTime.Text = "플레이시간: " + ElectronicObserver.Utility.Mathematics.DateTimeHelper.ToTimeElapsedString(TimeSpan.FromSeconds(this._playTimeCache + elapsed));
		}

		private void PlayTimeTimer_Tick(object sender, EventArgs e)
		{
            this.UpdatePlayTime();
		}

		private void FormFleet_FixShipNameWidth_CheckedChanged(object sender, EventArgs e)
		{
            this.FormFleet_FixedShipNameWidth.Enabled = this.FormFleet_FixShipNameWidth.Checked;
		}

		private void FormBrowser_ScreenShotFormat_PNG_CheckedChanged(object sender, EventArgs e)
		{
            this.FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Enabled = true;
		}

		private void FormBrowser_ScreenShotFormat_JPEG_CheckedChanged(object sender, EventArgs e)
		{
            this.FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Enabled = false;
		}


		private void UI_MainFont_Validating(object sender, CancelEventArgs e)
		{

			var newfont = SerializableFont.StringToFont(this.UI_MainFont.Text, true);

			if (newfont != null)
			{
                this.UI_RenderingTest.MainFont = newfont;
                this.UI_MainFont.BackColor = SystemColors.Window;
			}
			else
			{
                this.UI_MainFont.BackColor = Color.MistyRose;
			}

		}

		private void UI_SubFont_Validating(object sender, CancelEventArgs e)
		{

			var newfont = SerializableFont.StringToFont(this.UI_SubFont.Text, true);

			if (newfont != null)
			{
                this.UI_RenderingTest.SubFont = newfont;
                this.UI_SubFont.BackColor = SystemColors.Window;
			}
			else
			{
                this.UI_SubFont.BackColor = Color.MistyRose;
			}
		}

		private void UI_BarColorMorphing_CheckedChanged(object sender, EventArgs e)
		{
            this.UI_RenderingTest.HPBar.ColorMorphing = this.UI_BarColorMorphing.Checked;
            this.UI_RenderingTest.Refresh();
		}

		private void UI_MainFont_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.SuppressKeyPress = true;
				e.Handled = true;
                this.UI_MainFont_Validating(sender, new CancelEventArgs());
			}
		}

		private void UI_MainFont_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.IsInputKey = true;        // AcceptButton の影響を回避する
			}
		}

		private void UI_SubFont_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.SuppressKeyPress = true;
				e.Handled = true;
                this.UI_SubFont_Validating(sender, new CancelEventArgs());
			}
		}


		private void UI_SubFont_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.IsInputKey = true;        // AcceptButton の影響を回避する
			}
		}

		private void UI_RenderingTestChanger_Scroll(object sender, EventArgs e)
		{
            this.UI_RenderingTest.Value = this.UI_RenderingTestChanger.Value;
		}


	}
}
