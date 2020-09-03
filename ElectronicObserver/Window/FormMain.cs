using ElectronicObserver.Data;
using ElectronicObserver.Notifier;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility;
using ElectronicObserver.Window.Dialog;
using ElectronicObserver.Window.Integrate;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using DynaJson;

namespace ElectronicObserver.Window
{
	public partial class FormMain : Form
	{

		#region Properties

		public DockPanel MainPanel => this.MainDockPanel;
		public FormWindowCapture WindowCapture => this.fWindowCapture;

		private int ClockFormat;

		/// <summary>
		/// 音量設定用フラグ
		/// -1 = 無効, そうでなければ現在の試行回数
		/// </summary>
		private int _volumeUpdateState = 0;

		private DateTime _prevPlayTimeRecorded = DateTime.MinValue;

		#endregion


		#region Forms

		public List<DockContent> SubForms { get; private set; }

		public FormFleet[] fFleet;
		public FormCombinedFleet fCombinedFleet;
		public FormDock fDock;
		public FormArsenal fArsenal;
		public FormHeadquarters fHeadquarters;
		public FormInformation fInformation;
		public FormCompass fCompass;
		public FormLog fLog;
		public FormQuest fQuest;
		public FormBattle fBattle;
		public FormFleetOverview fFleetOverview;
		public FormShipGroup fShipGroup;
		public FormBrowserHost fBrowser;
		public FormWindowCapture fWindowCapture;
		public FormBaseAirCorps fBaseAirCorps;
		public FormJson fJson;
		public FormFleetPreset fFleetPreset;

		#endregion

		public static FormMain Instance;

        public ExternalDataReader Translator { get; private set; }


        public FormMain()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            CultureInfo c = CultureInfo.CurrentCulture;
            CultureInfo ui = CultureInfo.CurrentUICulture;
            if (c.Name != "en-US" && c.Name != "ja-JP")
            {
                c = new CultureInfo("ko-KR");
            }
            if (ui.Name != "en-US" && ui.Name != "ja-JP")
            {
                ui = new CultureInfo("ko-KR");
            }

            Thread.CurrentThread.CurrentCulture = c;
            Thread.CurrentThread.CurrentUICulture = ui;

			this.Translator = ExternalDataReader.Instance;

            Instance = this;
            this.InitializeComponent();
            //this.Text = SoftwareInformation.VersionJapanese;
            Utility.Configuration.Instance.Load(this);

            ThemeBase thm;
            switch (Configuration.Config.UI.Theme)
            {
                case Theme.Light:
                    thm = new VS2005Theme();
                    break;
                case Theme.Dark:
                    thm = new VS2013BlueTheme();
                    break;
                default:
                    thm = new VS2005Theme();
                    break;
            }

            this.MainDockPanel.Theme = thm;
            thm.Apply(this.MainDockPanel);

            this.Text = SoftwareInformation.SoftwareNameKorean;
		}

		private async void FormMain_Load(object sender, EventArgs e)
		{

			if (!Directory.Exists("Settings"))
				Directory.CreateDirectory("Settings");
			//Utility.Configuration.Instance.Load(this);


			Utility.Logger.Instance.LogAdded += new Utility.LogAddedEventHandler((Utility.Logger.LogData data) =>
			{
				if (this.InvokeRequired)
				{
                    // Invokeはメッセージキューにジョブを投げて待つので、別のBeginInvokeされたジョブが既にキューにあると、
                    // それを実行してしまい、BeginInvokeされたジョブの順番が保てなくなる
                    // GUIスレッドによる処理は、順番が重要なことがあるので、GUIスレッドからInvokeを呼び出してはいけない
                    this.Invoke(new Utility.LogAddedEventHandler(this.Logger_LogAdded), data);
				}
				else
				{
                    this.Logger_LogAdded(data);
				}
			});

			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
			Utility.Logger.Add(2, SoftwareInformation.SoftwareNameKorean + " 를 시작합니다.");


			ResourceManager.Instance.Load();
			RecordManager.Instance.Load();
			KCDatabase.Instance.Load();
			NotifierManager.Instance.Initialize(this);
			SyncBGMPlayer.Instance.ConfigurationChanged();

            #region Icon settings
            this.Icon = ResourceManager.Instance.AppIcon;

            this.StripMenu_File_Configuration.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormConfiguration];

            this.StripMenu_View_Fleet.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleet];
            this.StripMenu_View_FleetOverview.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleet];
            this.StripMenu_View_ShipGroup.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormShipGroup];
            this.StripMenu_View_Dock.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormDock];
            this.StripMenu_View_Arsenal.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormArsenal];
            this.StripMenu_View_Headquarters.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormHeadQuarters];
            this.StripMenu_View_Quest.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormQuest];
            this.StripMenu_View_Information.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormInformation];
            this.StripMenu_View_Compass.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormCompass];
            this.StripMenu_View_Battle.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBattle];
            this.StripMenu_View_Browser.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBrowser];
            this.StripMenu_View_Log.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormLog];
            this.StripMenu_WindowCapture.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormWindowCapture];
            this.StripMenu_View_BaseAirCorps.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBaseAirCorps];
            this.StripMenu_View_Json.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormJson];
            this.StripMenu_View_FleetPreset.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleetPreset];

            this.StripMenu_Tool_EquipmentList.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormEquipmentList];
            this.StripMenu_Tool_DropRecord.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormDropRecord];
            this.StripMenu_Tool_DevelopmentRecord.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormDevelopmentRecord];
            this.StripMenu_Tool_ConstructionRecord.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormConstructionRecord];
            this.StripMenu_Tool_ExpeditionRecord.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormResourceChart];
			this.StripMenu_Tool_ResourceChart.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormResourceChart];
			this.StripMenu_Tool_AlbumMasterShip.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumShip];
            this.StripMenu_Tool_AlbumMasterEquipment.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumEquipment];
            this.ToolStripMenuItem_AkashiList.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumEquipment];
            this.StripMenu_Tool_MasterExpedition.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumEquipment];
            this.StripMenu_Tool_AntiAirDefense.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAntiAirDefense];
            this.StripMenu_Tool_FleetImageGenerator.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleetImageGenerator];
            this.StripMenu_Tool_BaseAirCorpsSimulation.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBaseAirCorps];
            this.StripMenu_Tool_ExpChecker.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormExpChecker];
            this.StripMenu_Tool_ExpeditionCheck.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormExpeditionCheck];

            this.StripMenu_Help_Version.Image = ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.AppIcon];
			#endregion


			APIObserver.Instance.Start(Utility.Configuration.Config.Connection.Port, this);


            this.MainDockPanel.Extender.FloatWindowFactory = new CustomFloatWindowFactory();
            this.SubForms = new List<DockContent>();

            //form init
            //注：一度全てshowしないとイベントを受け取れないので注意	
            this.fFleet = new FormFleet[4];
			for (int i = 0; i < this.fFleet.Length; i++)
			{
                this.SubForms.Add(this.fFleet[i] = new FormFleet(this, i + 1));
			}

            this.SubForms.Add(this.fDock = new FormDock(this));
            this.SubForms.Add(this.fArsenal = new FormArsenal(this));
            this.SubForms.Add(this.fHeadquarters = new FormHeadquarters(this));
            this.SubForms.Add(this.fInformation = new FormInformation(this));
            this.SubForms.Add(this.fCompass = new FormCompass(this));
            this.SubForms.Add(this.fLog = new FormLog(this));
            this.SubForms.Add(this.fQuest = new FormQuest(this));
            this.SubForms.Add(this.fBattle = new FormBattle(this));
            this.SubForms.Add(this.fFleetOverview = new FormFleetOverview(this));
            this.SubForms.Add(this.fShipGroup = new FormShipGroup(this));
            this.SubForms.Add(this.fBrowser = new FormBrowserHost(this));
            this.SubForms.Add(this.fWindowCapture = new FormWindowCapture(this));
            this.SubForms.Add(this.fBaseAirCorps = new FormBaseAirCorps(this));
            this.SubForms.Add(this.fJson = new FormJson(this));
			this.SubForms.Add(this.fCombinedFleet = new FormCombinedFleet(this));
            this.SubForms.Add(this.fFleetPreset = new FormFleetPreset(this));

            this.ConfigurationChanged();     //設定から初期化

            this.LoadLayout(Configuration.Config.Life.LayoutFilePath);

			SoftwareInformation.CheckUpdate();
			// デバッグ: 開始時にAPIリストを読み込む
			if (Configuration.Config.Debug.LoadAPIListOnLoad)
			{
				try
				{
					await Task.Factory.StartNew(() => this.LoadAPIList(Configuration.Config.Debug.APIListPath));

                    this.Activate();     // 上記ロードに時間がかかるとウィンドウが表示されなくなることがあるので
				}
				catch (Exception ex)
				{

					Utility.Logger.Add(3, "API 로드를 실패했습니다." + ex.Message);
				}
			}

			APIObserver.Instance.ResponseReceived += (a, b) => this.UpdatePlayTime();


			// 🎃
			if (DateTime.Now.Month == 10 && DateTime.Now.Day == 31)
			{
				APIObserver.Instance.APIList["api_port/port"].ResponseReceived += this.CallPumpkinHead;
			}

            // 完了通知（ログインページを開く）
            this.fBrowser.InitializeApiCompleted();

            this.UIUpdateTimer.Start();

            SoftwareInformation.CheckMaintenance();

            Utility.Logger.Add(3, "기동이 완료되었습니다.");
        }


		private void FormMain_Shown(object sender, EventArgs e)
		{
            // Load で設定すると無視されるかバグる(タスクバーに出なくなる)のでここで設定
            this.TopMost = Utility.Configuration.Config.Life.TopMost;

            // HACK: タスクバーに表示されなくなる不具合への応急処置　効くかは知らない
            this.ShowInTaskbar = true;
		}

		// Toggle TopMost of Main Form back and forth to workaround a .Net Bug: KB2756203 (~win7) / KB2769674 (win8~)
		private void FormMain_RefreshTopMost()
		{
            this.TopMost = !this.TopMost;
            this.TopMost = !this.TopMost;
		}

		private void ConfigurationChanged()
		{
			var c = Utility.Configuration.Config;

            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.StripMenu.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.StripMenu.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.StripStatus.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.StripStatus.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

            this.StripMenu_Debug.Enabled = this.StripMenu_Debug.Visible =
            this.StripMenu_View_Json.Enabled = this.StripMenu_View_Json.Visible =
				c.Debug.EnableDebugMenu;

            this.StripStatus.Visible = c.Life.ShowStatusBar;

			// Load で TopMost を変更するとバグるため(前述)
			if (this.UIUpdateTimer.Enabled)
                this.TopMost = c.Life.TopMost;

            this.ClockFormat = c.Life.ClockFormat;

            this.Font = c.UI.MainFont;
            //StripMenu.Font = Font;
            this.StripStatus.Font = this.Font;

            this.MainDockPanel.Skin.AutoHideStripSkin.TextFont = this.Font;
            this.MainDockPanel.Skin.DockPaneStripSkin.TextFont = this.Font;

            if (c.Life.LockLayout)
			{
                this.MainDockPanel.AllowChangeLayout = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            }
			else
			{
                this.MainDockPanel.AllowChangeLayout = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			}

            this.StripMenu_File_Layout_LockLayout.Checked = c.Life.LockLayout;
            this.MainDockPanel.CanCloseFloatWindowInLock = c.Life.CanCloseFloatWindowInLock;
            this.MainDockPanel.CanSizableFloatWindowInLock = c.Life.CanSizableFloatWindowInLock;
            this.StripMenu_File_Layout_TopMost.Checked = c.Life.TopMost;

            this.StripMenu_File_Notification_MuteAll.Checked = Notifier.NotifierManager.Instance.GetNotifiers().All(n => n.IsSilenced);

			if (!c.Control.UseSystemVolume)
                this._volumeUpdateState = -1;
		}

		private void StripMenu_Debug_LoadAPIFromFile_Click(object sender, EventArgs e)
		{

			/*/
			using ( var dialog = new DialogLocalAPILoader() ) {

				if ( dialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK ) {
					if ( APIObserver.Instance.APIList.ContainsKey( dialog.APIName ) ) {

						if ( dialog.IsResponse ) {
							APIObserver.Instance.LoadResponse( dialog.APIPath, dialog.FileData );
						}
						if ( dialog.IsRequest ) {
							APIObserver.Instance.LoadRequest( dialog.APIPath, dialog.FileData );
						}

					}
				}
			}
			/*/
			new DialogLocalAPILoader2().Show(this);
			//*/
		}

		private void UIUpdateTimer_Tick(object sender, EventArgs e)
		{
			SystemEvents.OnUpdateTimerTick();

			// 東京標準時
			DateTime now = DateTime.UtcNow + new TimeSpan(9, 0, 0);

			switch (this.ClockFormat)
			{
				case 0: //時計表示
                    this.StripStatus_Clock.Text = now.ToString("HH\\:mm\\:ss");
                    this.StripStatus_Clock.ToolTipText = now.ToString("yyyy\\/MM\\/dd (ddd)") + "\r\n" + SoftwareInformation.GetMaintenanceTime();
					break;

				case 1: //演習更新まで
					{
						DateTime border = now.Date.AddHours(3);
						while (border < now)
							border = border.AddHours(12);

						TimeSpan ts = border - now;
                        this.StripStatus_Clock.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
                        this.StripStatus_Clock.ToolTipText = now.ToString("yyyy\\/MM\\/dd (ddd) HH\\:mm\\:ss") + "\r\n" + SoftwareInformation.GetMaintenanceTime();
					}
					break;

				case 2: //任務更新まで
					{
						DateTime border = now.Date.AddHours(5);
						if (border < now)
							border = border.AddHours(24);

						TimeSpan ts = border - now;
                        this.StripStatus_Clock.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
                        this.StripStatus_Clock.ToolTipText = now.ToString("yyyy\\/MM\\/dd (ddd) HH\\:mm\\:ss") + "\r\n" + SoftwareInformation.GetMaintenanceTime();
					}
					break;
			}


			// WMP コントロールによって音量が勝手に変えられてしまうため、前回終了時の音量の再設定を試みる。
			// 10回試行してダメなら諦める(例外によるラグを防ぐため)
			// 起動直後にやらないのはちょっと待たないと音量設定が有効にならないから
			if (this._volumeUpdateState != -1 && this._volumeUpdateState < 10 && Utility.Configuration.Config.Control.UseSystemVolume)
			{

				try
				{
					uint id = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
					float volume = Utility.Configuration.Config.Control.LastVolume;
					bool mute = Utility.Configuration.Config.Control.LastIsMute;

					BrowserLib.VolumeManager.SetApplicationVolume(id, volume);
					BrowserLib.VolumeManager.SetApplicationMute(id, mute);

					SyncBGMPlayer.Instance.SetInitialVolume((int)(volume * 100));
					foreach (var not in NotifierManager.Instance.GetNotifiers())
						not.SetInitialVolume((int)(volume * 100));

                    this._volumeUpdateState = -1;

				}
				catch (Exception)
				{

                    this._volumeUpdateState++;
				}
			}

		}




		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{

			if (Utility.Configuration.Config.Life.ConfirmOnClosing)
			{
				if (MessageBox.Show(SoftwareInformation.SoftwareNameKorean + "를 종료하시겠습니까？", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
					== System.Windows.Forms.DialogResult.No)
				{
					e.Cancel = true;
					return;
				}
			}


			Utility.Logger.Add(2, SoftwareInformation.SoftwareNameKorean + "를 종료합니다…");

            this.UIUpdateTimer.Stop();

            this.fBrowser.CloseBrowser();

            this.UpdatePlayTime();

			SystemEvents.OnSystemShuttingDown();

            this.SaveLayout(Configuration.Config.Life.LayoutFilePath);


			// 音量の保存
			{
				try
				{
					uint id = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
					Utility.Configuration.Config.Control.LastVolume = BrowserLib.VolumeManager.GetApplicationVolume(id);
					Utility.Configuration.Config.Control.LastIsMute = BrowserLib.VolumeManager.GetApplicationMute(id);

				}
				catch (Exception)
				{
					/* ぷちっ */
				}

			}
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			NotifierManager.Instance.ApplyToConfiguration();
			Utility.Configuration.Instance.Save();
			RecordManager.Instance.SavePartial();
			KCDatabase.Instance.Save();
			APIObserver.Instance.Stop();

			Utility.Logger.Add(2, "종료가 완료되었습니다.");

			if (Utility.Configuration.Config.Log.SaveLogFlag)
				Utility.Logger.Save();
		}



		private IDockContent GetDockContentFromPersistString(string persistString)
		{

			switch (persistString)
			{
				case "Fleet #1":
					return this.fFleet[0];
				case "Fleet #2":
					return this.fFleet[1];
				case "Fleet #3":
					return this.fFleet[2];
				case "Fleet #4":
					return this.fFleet[3];
				case "Dock":
					return this.fDock;
				case "Arsenal":
					return this.fArsenal;
				case "HeadQuarters":
					return this.fHeadquarters;
				case "Information":
					return this.fInformation;
				case "Compass":
					return this.fCompass;
				case "Log":
					return this.fLog;
				case "Quest":
					return this.fQuest;
				case "Battle":
					return this.fBattle;
				case "FleetOverview":
					return this.fFleetOverview;
				//case "ShipGroup":
				//	return fShipGroup;
				case "Browser":
					return this.fBrowser;
				case "WindowCapture":
					return this.fWindowCapture;
				case "BaseAirCorps":
					return this.fBaseAirCorps;
				case "Json":
					return this.fJson;
				case "CombinedFleet":
					return this.fCombinedFleet;
                case "FleetPreset":
                    return fFleetPreset;
                default:
					if (persistString.StartsWith("ShipGroup"))
					{
                        this.fShipGroup.ConfigureFromPersistString(persistString);
						return this.fShipGroup;
					}
					if (persistString.StartsWith(FormIntegrate.PREFIX))
					{
						return FormIntegrate.FromPersistString(this, persistString);
					}
					return null;
			}

		}



		private void LoadSubWindowsLayout(Stream stream)
		{

			try
			{

				if (stream != null)
				{

                    // 取り込んだウィンドウは一旦デタッチして閉じる
                    this.fWindowCapture.CloseAll();

					foreach (var f in this.SubForms)
					{
						f.Show(this.MainDockPanel, DockState.Document);
						f.DockPanel = null;
					}

                    this.MainDockPanel.LoadFromXml(stream, new DeserializeDockContent(this.GetDockContentFromPersistString));


                    this.fWindowCapture.AttachAll();

				}
				else
				{

					foreach (var f in this.SubForms)
						f.Show(this.MainDockPanel);


					foreach (var x in this.MainDockPanel.Contents)
					{
						x.DockHandler.Hide();
					}
				}

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "서브윈도우 레이아웃 복원에 실패했습니다.");
			}

		}


		private void SaveSubWindowsLayout(Stream stream)
		{

			try
			{

                this.MainDockPanel.SaveAsXml(stream, Encoding.UTF8);

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "서브 윈도우 레이아웃 저장에 실패했습니다.");
			}

		}



		private void LoadLayout(string path)
		{

			try
			{
				using (var archive = new ZipArchive(File.OpenRead(path), ZipArchiveMode.Read))
				{
                    this.MainDockPanel.SuspendLayout(true);

					WindowPlacementManager.LoadWindowPlacement(this, archive.GetEntry("WindowPlacement.xml").Open());
                    this.LoadSubWindowsLayout(archive.GetEntry("SubWindowLayout.xml").Open());
				}

				Utility.Logger.Add(2, path + " 에서 레이아웃을 로드했습니다.");

			}
			catch (FileNotFoundException)
			{

				Utility.Logger.Add(3, string.Format("레이아웃 파일이 잘못되었습니다."));
				MessageBox.Show("레이아웃이 초기화 되었습니다.\r\n'보기'메뉴에서 원하는 창을 추가하십시오.", "레이아웃 파일이 잘못되었습니다.",
					MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.fBrowser.Show(this.MainDockPanel);

			}
			catch (DirectoryNotFoundException)
			{

				Utility.Logger.Add(3, string.Format("레이아웃 파일이 존재하지 않습니다."));
				MessageBox.Show("레이아웃이 초기화 되었습니다.\r\n'보기'메뉴에서 원하는 창을 추가하십시오.", "레이아웃 파일이 존재하지 않습니다.",
					MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.fBrowser.Show(this.MainDockPanel);

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "레이아웃 복원에 실패했습니다.");

			}
			finally
			{

                this.MainDockPanel.ResumeLayout(true, true);
			}

		}

		private void SaveLayout(string path)
		{

			try
			{

                this.CreateParentDirectories(path);

				using (var archive = new ZipArchive(File.Open(path, FileMode.Create), ZipArchiveMode.Create))
				{

					using (var layoutstream = archive.CreateEntry("SubWindowLayout.xml").Open())
					{
                        this.SaveSubWindowsLayout(layoutstream);
					}
					using (var placementstream = archive.CreateEntry("WindowPlacement.xml").Open())
					{
						WindowPlacementManager.SaveWindowPlacement(this, placementstream);
					}
				}


				Utility.Logger.Add(2, path + " 에 레이아웃을 저장했습니다.");

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "레이아웃 저장에 실패했습니다.");
			}

		}

		private void CreateParentDirectories(string path)
		{
			var parents = Path.GetDirectoryName(path);

			if (!String.IsNullOrEmpty(parents))
			{
				Directory.CreateDirectory(parents);
			}
		}

		void Logger_LogAdded(Utility.Logger.LogData data)
		{
            this.StripStatus_Information.Text = data.Message.Replace("\r", " ").Replace("\n", " ");
		}


		private void StripMenu_Help_Version_Click(object sender, EventArgs e)
		{
			using (var dialog = new DialogVersion())
			{
				dialog.ShowDialog(this);
			}
		}

		private void StripMenu_File_Configuration_Click(object sender, EventArgs e)
		{
            this.UpdatePlayTime();

			using (var dialog = new DialogConfiguration(Utility.Configuration.Config))
			{
				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{

					dialog.ToConfiguration(Utility.Configuration.Config);
					Utility.Configuration.Instance.OnConfigurationChanged();

				}
			}
		}

		private void StripMenu_File_Close_Click(object sender, EventArgs e)
		{
            this.Close();
		}


		private void StripMenu_File_SaveData_Save_Click(object sender, EventArgs e)
		{
			RecordManager.Instance.SaveAll();
		}

		private void StripMenu_File_SaveData_Load_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("저장하지 않은 기록이 소실될 수 있습니다.\r\n로드하시겠습니까?", "확인",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
				== System.Windows.Forms.DialogResult.Yes)
			{

				RecordManager.Instance.Load();
			}
		}



		private async void StripMenu_Debug_LoadInitialAPI_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{

				ofd.Title = "API리스트를 로드";
				ofd.Filter = "API List|*.txt|File|*";
				ofd.InitialDirectory = Utility.Configuration.Config.Connection.SaveDataPath;
                if (!string.IsNullOrWhiteSpace(Utility.Configuration.Config.Debug.APIListPath))
                    ofd.FileName = Utility.Configuration.Config.Debug.APIListPath;

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

					try
					{

						await Task.Factory.StartNew(() => this.LoadAPIList(ofd.FileName));

					}
					catch (Exception ex)
					{

						MessageBox.Show("API 로드를 실패했습니다.\r\n" + ex.Message, "에러",
							MessageBoxButtons.OK, MessageBoxIcon.Error);

					}
				}
			}
		}



		private void LoadAPIList(string path)
		{
			string parent = Path.GetDirectoryName(path);

			using (StreamReader sr = new StreamReader(path))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{

					bool isRequest = false;
					{
						int slashindex = line.IndexOf('/');
						if (slashindex != -1)
						{
							switch (line.Substring(0, slashindex).ToLower())
							{
								case "q":
								case "request":
									isRequest = true;
									goto case "s";
								case "":
								case "s":
								case "response":
									line = line.Substring(Math.Min(slashindex + 1, line.Length));
									break;
							}
						}
					}

					if (APIObserver.Instance.APIList.ContainsKey(line))
					{
						APIBase api = APIObserver.Instance.APIList[line];

						if (isRequest ? api.IsRequestSupported : api.IsResponseSupported)
						{

							string[] files = Directory.GetFiles(parent, string.Format("*{0}@{1}.json", isRequest ? "Q" : "S", line.Replace('/', '@')), SearchOption.TopDirectoryOnly);

							if (files.Length == 0)
								continue;

							Array.Sort(files);

							using (StreamReader sr2 = new StreamReader(files[files.Length - 1]))
							{
								if (isRequest)
								{
                                    this.Invoke((Action)(() =>
									{
										APIObserver.Instance.LoadRequest("/kcsapi/" + line, sr2.ReadToEnd());
									}));
								}
								else
								{
                                    this.Invoke((Action)(() =>
									{
										APIObserver.Instance.LoadResponse("/kcsapi/" + line, sr2.ReadToEnd());
									}));
								}
							}

							//System.Diagnostics.Debug.WriteLine( "APIList Loader: API " + line + " File " + files[files.Length-1] + " Loaded." );
						}
					}
				}
			}
		}

		private void StripMenu_Debug_LoadRecordFromOld_Click(object sender, EventArgs e)
		{
			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
				MessageBox.Show("먼저 일반 api_start2 를 로드합니다.", "불편을 끼쳐드려 죄송합니다.", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using (OpenFileDialog ofd = new OpenFileDialog())
			{

				ofd.Title = "이전 api_start2 에서 기록";
				ofd.Filter = "api_start2|*api_start2*.json|JSON|*.json|File|*";

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

					try
					{

						using (StreamReader sr = new StreamReader(ofd.FileName))
						{

							dynamic json = JsonObject.Parse(sr.ReadToEnd().Remove(0, 7));

							foreach (dynamic elem in json.api_data.api_mst_ship)
							{
								if (elem.api_name != "なし" && KCDatabase.Instance.MasterShips.ContainsKey((int)elem.api_id) && KCDatabase.Instance.MasterShips[(int)elem.api_id].Name == elem.api_name)
								{
									RecordManager.Instance.ShipParameter.UpdateParameter((int)elem.api_id, 1, (int)elem.api_tais[0], (int)elem.api_tais[1], (int)elem.api_kaih[0], (int)elem.api_kaih[1], (int)elem.api_saku[0], (int)elem.api_saku[1]);

									int[] defaultslot = Enumerable.Repeat(-1, 5).ToArray();
									((int[])elem.api_defeq).CopyTo(defaultslot, 0);
									RecordManager.Instance.ShipParameter.UpdateDefaultSlot((int)elem.api_id, defaultslot);
								}
							}
						}

					}
					catch (Exception ex)
					{

						MessageBox.Show("API 로드에 실패했습니다.\r\n" + ex.Message, "에러",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}


		private void StripMenu_Debug_LoadDataFromOld_Click(object sender, EventArgs e)
		{
			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
                MessageBox.Show("먼저 일반 api_start2 를 로드합니다.", "불편을 끼쳐드려 죄송합니다.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

			using (OpenFileDialog ofd = new OpenFileDialog())
			{

                ofd.Title = "이전 api_start2 에서 기록";
                ofd.Filter = "api_start2|*api_start2*.json|JSON|*.json|File|*";
                ofd.InitialDirectory = Utility.Configuration.Config.Connection.SaveDataPath;

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

					try
					{

						using (StreamReader sr = new StreamReader(ofd.FileName))
						{

							dynamic json = JsonObject.Parse(sr.ReadToEnd().Remove(0, 7));

							foreach (dynamic elem in json.api_data.api_mst_ship)
							{

								var ship = KCDatabase.Instance.MasterShips[(int)elem.api_id];

								if (elem.api_name != "なし" && ship != null && ship.IsAbyssalShip)
								{

									KCDatabase.Instance.MasterShips[(int)elem.api_id].LoadFromResponse("api_start2", elem);
								}
							}
						}

						Utility.Logger.Add(1, "이전 api_start2 에서 데이터를 복원했습니다.");

					}
					catch (Exception ex)
					{


                        MessageBox.Show("API 로드에 실패했습니다.\r\n" + ex.Message, "에러",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}


		private void StripMenu_Tool_AlbumMasterShip_Click(object sender, EventArgs e)
		{
			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
				MessageBox.Show("함선 데이터가 로드되지 않았습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);

			}
			else
			{
				var dialogAlbumMasterShip = new DialogAlbumMasterShip();
                this.FormMain_RefreshTopMost();
				dialogAlbumMasterShip.Show(this);
			}
		}

		private void StripMenu_Tool_AlbumMasterEquipment_Click(object sender, EventArgs e)
		{
			if (KCDatabase.Instance.MasterEquipments.Count == 0)
			{
                MessageBox.Show("장비 데이터가 로드되지 않았습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
			else
			{
				var dialogAlbumMasterEquipment = new DialogAlbumMasterEquipment();
                this.FormMain_RefreshTopMost();
				dialogAlbumMasterEquipment.Show(this);
			}
		}

        private void StripMenu_Tool_MasterExpeditionClick(object sender, EventArgs e)
        {
            if (KCDatabase.Instance.Mission.Count == 0)
            {
                MessageBox.Show("원정 데이터가 로드되지 않았습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                var dialogAlbumMasterExpedition = new DialogAlbumMasterExpedition();
                this.FormMain_RefreshTopMost();
                dialogAlbumMasterExpedition.Show(this);
            }
        }

        private async void StripMenu_Debug_DeleteOldAPI_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("이전 API 데이터를 삭제합니다.\r\n실행하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
				== System.Windows.Forms.DialogResult.Yes)
			{

				try
				{

					int count = await Task.Factory.StartNew(() => this.DeleteOldAPI());

					MessageBox.Show("삭제가 완료되었습니다.\r\n" + count + " 개 파일을 삭제했습니다.", "삭제 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);

				}
				catch (Exception ex)
				{

					MessageBox.Show("삭제에 실패했습니다.\r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private int DeleteOldAPI()
		{
			//適当極まりない
			int count = 0;

			var apilist = new Dictionary<string, List<KeyValuePair<string, string>>>();

			foreach (string s in Directory.EnumerateFiles(Utility.Configuration.Config.Connection.SaveDataPath, "*.json", SearchOption.TopDirectoryOnly))
			{

				int start = s.IndexOf('@');
				int end = s.LastIndexOf('.');

				start--;
				string key = s.Substring(start, end - start + 1);
				string date = s.Substring(0, start);


				if (!apilist.ContainsKey(key))
				{
					apilist.Add(key, new List<KeyValuePair<string, string>>());
				}
				apilist[key].Add(new KeyValuePair<string, string>(date, s));
			}

			foreach (var l in apilist.Values)
			{
				var l2 = l.OrderBy(el => el.Key).ToList();
				for (int i = 0; i < l2.Count - 1; i++)
				{
					File.Delete(l2[i].Value);
					count++;
				}
			}

			return count;
		}



		private void StripMenu_Tool_EquipmentList_Click(object sender, EventArgs e)
		{
			var dialogEquipmentList = new DialogEquipmentList();
            this.FormMain_RefreshTopMost();
			dialogEquipmentList.Show(this);
		}


		private async void StripMenu_Debug_RenameShipResource_Click(object sender, EventArgs e)
		{
			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
				MessageBox.Show("함선 데이터가 로드 되지 않았습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (MessageBox.Show("저장된 함선 이름을 가진 파일 및 폴더를 다시 로드합니다.\r\n" +
				"대상은 지정된 폴더 아래의 모든 파일 및 폴더입니다.\r\n" +
				"계속 하시겠습니까?", "함선 파일의 이름을 변경", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
				== System.Windows.Forms.DialogResult.Yes)
			{

				string path = null;

				using (FolderBrowserDialog dialog = new FolderBrowserDialog())
				{
					dialog.SelectedPath = Configuration.Config.Connection.SaveDataPath;
					if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						path = dialog.SelectedPath;
					}
				}

				if (path == null) return;

				try
				{

					int count = await Task.Factory.StartNew(() => this.RenameShipResource(path));

					MessageBox.Show(string.Format("작업이 완료되었습니다.\r\n{0} 개 항목의 이름을 변경했습니다.", count), "처리 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);


				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "함선 파일 이름 바꾸기에 실패했습니다.");
					MessageBox.Show("함선 파일 이름 바꾸기에 실패했습니다.\r\n" + ex.Message, "애러", MessageBoxButtons.OK, MessageBoxIcon.Error);

				}
			}
		}


		private int RenameShipResource(string path)
		{
			int count = 0;

			foreach (var p in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
			{
				string name = Path.GetFileName(p);

				foreach (var ship in KCDatabase.Instance.MasterShips.Values)
				{
					if (name.Contains(ship.ResourceName))
					{
						name = name.Replace(ship.ResourceName, string.Format("{0}({1})", ship.NameWithClass, ship.ShipID)).Replace(' ', '_');

						try
						{
							File.Move(p, Path.Combine(Path.GetDirectoryName(p), name));
							count++;
							break;
						}
						catch (IOException)
						{
							//ファイルが既に存在する：＊にぎりつぶす＊
						}
					}
				}
			}

			foreach (var p in Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories))
			{
				string name = Path.GetFileName(p);      //GetDirectoryName だと親フォルダへのパスになってしまうため

				foreach (var ship in KCDatabase.Instance.MasterShips.Values)
				{
					if (name.Contains(ship.ResourceName))
					{
						name = name.Replace(ship.ResourceName, string.Format("{0}({1})", ship.NameWithClass, ship.ShipID)).Replace(' ', '_');

						try
						{

							Directory.Move(p, Path.Combine(Path.GetDirectoryName(p), name));
							count++;
							break;

						}
						catch (IOException)
						{
							//フォルダが既に存在する：＊にぎりつぶす＊
						}
					}
				}
			}
			return count;
		}


		private void StripMenu_Help_Help_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("외부 브라우저에서 온라인 메뉴얼을 엽니다.\r\n계속 하시겠습니까?", "도움말",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
				== System.Windows.Forms.DialogResult.Yes)
			{

				System.Diagnostics.Process.Start("https://github.com/TheLokis/ElectronicObserver/wiki");
			}
		}

		private void SeparatorWhitecap_Click(object sender, EventArgs e)
		{
			new DialogWhitecap().Show(this);
		}

		private void StripMenu_File_Layout_Load_Click(object sender, EventArgs e)
        {
            this.LoadLayout(Utility.Configuration.Config.Life.LayoutFilePath);
		}

		private void StripMenu_File_Layout_Save_Click(object sender, EventArgs e)
		{
            this.SaveLayout(Utility.Configuration.Config.Life.LayoutFilePath);
		}

		private void StripMenu_File_Layout_Open_Click(object sender, EventArgs e)
		{
			using (var dialog = new OpenFileDialog())
			{

				dialog.Filter = "Layout Archive|*.zip|File|*";
				dialog.Title = "레이아웃 파일 열기";


				PathHelper.InitOpenFileDialog(Utility.Configuration.Config.Life.LayoutFilePath, dialog);

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

					Utility.Configuration.Config.Life.LayoutFilePath = PathHelper.GetPathFromOpenFileDialog(dialog);
                    this.LoadLayout(Utility.Configuration.Config.Life.LayoutFilePath);

				}
			}
		}

		private void StripMenu_File_Layout_Change_Click(object sender, EventArgs e)
		{
			using (var dialog = new SaveFileDialog())
			{
				dialog.Filter = "Layout Archive|*.zip|File|*";
				dialog.Title = "레이아웃 파일 저장";

				PathHelper.InitSaveFileDialog(Utility.Configuration.Config.Life.LayoutFilePath, dialog);

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Utility.Configuration.Config.Life.LayoutFilePath = PathHelper.GetPathFromSaveFileDialog(dialog);
                    this.SaveLayout(Utility.Configuration.Config.Life.LayoutFilePath);
				}
			}
		}


		private void StripMenu_Tool_ResourceChart_Click(object sender, EventArgs e)
		{
			var dialogResourceChart = new DialogResourceChart();
            this.FormMain_RefreshTopMost();
			dialogResourceChart.Show(this);
		}

		private void StripMenu_Tool_DropRecord_Click(object sender, EventArgs e)
		{
			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
				MessageBox.Show("칸코레를 한번 실행하고 열어주세요.", "마스터 데이터가 없습니다.", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (RecordManager.Instance.ShipDrop.Record.Count == 0)
			{
				MessageBox.Show("드롭 기록이 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			new Dialog.DialogDropRecordViewer().Show(this);
		}


		private void StripMenu_Tool_DevelopmentRecord_Click(object sender, EventArgs e)
		{

			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
                MessageBox.Show("칸코레를 한번 실행하고 열어주세요.", "마스터 데이터가 없습니다.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
			}

			if (RecordManager.Instance.Development.Record.Count == 0)
			{
                MessageBox.Show("개발 기록이 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
			}

			new Dialog.DialogDevelopmentRecordViewer().Show(this);

		}

		private void StripMenu_Tool_ConstructionRecord_Click(object sender, EventArgs e)
		{

			if (KCDatabase.Instance.MasterShips.Count == 0)
			{
                MessageBox.Show("칸코레를 한번 실행하고 열어주세요.", "마스터 데이터가 없습니다.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
			}

			if (RecordManager.Instance.Construction.Record.Count == 0)
			{
                MessageBox.Show("건조 기록이 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
			}

			new Dialog.DialogConstructionRecordViewer().Show(this);
		}

        private void StripMenu_Tool_ExpeditionRecord_Click(object sender, EventArgs e)
        {
            if (KCDatabase.Instance.Mission.Count == 0)
            {
				// Error Message Point #1
                return;
            }

            if (RecordManager.Instance.Expedition.Record.Count == 0)
            {
                MessageBox.Show("원정 기록이 없습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            new Dialog.DialogExpeditionRecordViewer().Show(this);
        }


        private void StripMenu_Tool_AntiAirDefense_Click(object sender, EventArgs e)
		{
			new Dialog.DialogAntiAirDefense().Show(this);
		}

		private void StripMenu_Tool_FleetImageGenerator_Click(object sender, EventArgs e)
		{
			new Dialog.DialogFleetImageGenerator(1).Show(this);
		}

		private void StripMenu_Tool_BaseAirCorpsSimulation_Click(object sender, EventArgs e)
		{
			new Dialog.DialogBaseAirCorpsSimulation().Show(this);
		}

        private void StripMenu_Tool_ExpeditionCheck_Click(object sender, EventArgs e)
        {
            new Dialog.DialogExpeditionCheck().Show(this);
        }

        private void StripMenu_File_Layout_LockLayout_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.Life.LockLayout = this.StripMenu_File_Layout_LockLayout.Checked;
            this.ConfigurationChanged();
		}

		private void StripMenu_File_Layout_TopMost_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.Life.TopMost = this.StripMenu_File_Layout_TopMost.Checked;
            this.ConfigurationChanged();
		}


		private void StripMenu_File_Notification_MuteAll_Click(object sender, EventArgs e)
		{
			bool isSilenced = this.StripMenu_File_Notification_MuteAll.Checked;

			foreach (var n in NotifierManager.Instance.GetNotifiers())
				n.IsSilenced = isSilenced;
		}

		private void StripMenu_Tool_ExpChecker_Click(object sender, EventArgs e)
		{
			new Dialog.DialogExpChecker().Show(this);
		}

		private void CallPumpkinHead(string apiname, dynamic data)
		{
			new DialogHalloween().Show(this);
			APIObserver.Instance.APIList["api_port/port"].ResponseReceived -= this.CallPumpkinHead;
		}

		private void StripMenu_WindowCapture_AttachAll_Click(object sender, EventArgs e)
		{
            this.fWindowCapture.AttachAll();
		}

		private void StripMenu_WindowCapture_DetachAll_Click(object sender, EventArgs e)
		{
            this.fWindowCapture.DetachAll();
		}

		private void UpdatePlayTime()
		{
			var c = Utility.Configuration.Config.Log;
			DateTime now = DateTime.Now;

			double span = (now - this._prevPlayTimeRecorded).TotalSeconds;
			if (span < c.PlayTimeIgnoreInterval)
			{
				c.PlayTime += span;
			}

            this._prevPlayTimeRecorded = now;
		}

        private void StripMenu_Tool_AkashiList_Click(object sender, EventArgs e)
        {
            if (KCDatabase.Instance.MasterEquipments.Count == 0)
            {
                MessageBox.Show("장비 데이터가 로드되지 않았습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            new Dialog.DialogAkashilist().Show(this);
        }

        private void StripMenu_Tool_akashi_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://akashi-list.me/");
        }

        private void StripMenu_Tool_jwiki_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://wikiwiki.jp/kancolle/");
        }

        private void StripMenu_Tool_wikia_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ja.kancolle.wikia.com/wiki/%E3%83%9C%E3%83%BC%E3%83%89:%E3%80%90%E6%A4%9C%E8%A8%BC%E3%80%91");
        }

        private void StripMenu_Tool_enwikia_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://kancolle.fandom.com/");
        }

        private void StripMenu_Tool_namu_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://namu.wiki/w/%ED%95%A8%EB%8C%80%20%EC%BB%AC%EB%A0%89%EC%85%98");
        }

        private void StripMenu_Tool_poidb_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://db.kcwiki.moe/drop/");
        }

        private void StripMenu_Tool_Oyodo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ooyodo-quest.net/latest");
        }

        private void StripMenu_Tool_DeckB_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://kancolle-calc.net/deckbuilder.html");
        }
        private void StripMenu_View_FleetPreset_Click(object sender, EventArgs e)
        {
            this.ShowForm(this.fFleetPreset);
        }

        #region フォーム表示

        /// <summary>
        /// 子フォームを表示します。既に表示されている場合はフォームをある点に移動します。（失踪対策）
        /// </summary>
        /// <param name="form"></param>
        private void ShowForm(DockContent form)
		{
			if (form.IsFloat && form.Visible)
			{
				form.FloatPane.FloatWindow.Location = new Point(128, 128);
			}

			form.Show(this.MainDockPanel);
		}

		private void StripMenu_View_Fleet_1_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fFleet[0]);
		}

		private void StripMenu_View_Fleet_2_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fFleet[1]);
		}

		private void StripMenu_View_Fleet_3_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fFleet[2]);
		}

		private void StripMenu_View_Fleet_4_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fFleet[3]);
		}

        private void StripMenu_CombinedFleet_Click(object sender, EventArgs e)
        {
            this.ShowForm(this.fCombinedFleet);
        }

        private void StripMenu_View_Dock_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fDock);
		}

		private void StripMenu_View_Arsenal_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fArsenal);
		}

		private void StripMenu_View_Headquarters_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fHeadquarters);
		}

		private void StripMenu_View_Information_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fInformation);
		}

		private void StripMenu_View_Compass_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fCompass);
		}

		private void StripMenu_View_Log_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fLog);
		}

		private void StripMenu_View_Quest_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fQuest);
		}

		private void StripMenu_View_Battle_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fBattle);
		}

		private void StripMenu_View_FleetOverview_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fFleetOverview);
		}

		private void StripMenu_View_ShipGroup_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fShipGroup);
		}

		private void StripMenu_View_Browser_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fBrowser);
		}

		private void StripMenu_WindowCapture_SubWindow_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fWindowCapture);
		}

		private void StripMenu_View_BaseAirCorps_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fBaseAirCorps);
		}

		private void StripMenu_View_Json_Click(object sender, EventArgs e)
		{
            this.ShowForm(this.fJson);
		}
        
        #endregion

        private void StripMenu_Help_Click(object sender, EventArgs e)
        {

        }
    }
}
