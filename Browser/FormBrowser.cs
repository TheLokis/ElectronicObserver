using Browser.CefOp;
using BrowserLib;
using CefSharp;
using CefSharp.WinForms;
using Nekoxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
	/// <summary>
	/// ブラウザを表示するフォームです。
	/// </summary>
	/// <remarks>thx KanColleViewer!</remarks>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single/*, IncludeExceptionDetailInFaults = true*/)]
	public partial class FormBrowser : Form, BrowserLib.IBrowser
	{

		private readonly Size KanColleSize = new Size(1200, 720);
        private readonly string BrowserCachePath = "BrowserCache";

        private readonly string StyleClassID = Guid.NewGuid().ToString().Substring(0, 8);
		private bool RestoreStyleSheet = false;

		// FormBrowserHostの通信サーバ
		private string ServerUri;

		// FormBrowserの通信サーバ
		private PipeCommunicator<BrowserLib.IBrowserHost> BrowserHost;

		private BrowserLib.BrowserConfiguration Configuration;

		// 親プロセスが生きているか定期的に確認するためのタイマー
		private Timer HeartbeatTimer = new Timer();
		private IntPtr HostWindow;


		private ChromiumWebBrowser Browser = null;

		private string ProxySettings = null;


		private bool _styleSheetApplied;
		/// <summary>
		/// スタイルシートの変更が適用されているか
		/// </summary>
		private bool StyleSheetApplied
		{
			get { return this._styleSheetApplied; }
			set
			{
				if (value)
				{
                    //Browser.Anchor = AnchorStyles.None;
                    this.ApplyZoom();
                    this.SizeAdjuster_SizeChanged(null, new EventArgs());
				}
				else
				{
                    this.SizeAdjuster.SuspendLayout();
					if (this.IsBrowserInitialized)
					{
                        //Browser.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                        this.Browser.Location = new Point(0, 0);
                        this.Browser.MinimumSize = new Size(0, 0);
                        this.Browser.Size = this.SizeAdjuster.Size;
					}
                    this.SizeAdjuster.ResumeLayout();
				}

                this._styleSheetApplied = value;
			}
		}

		/// <summary>
		/// 艦これが読み込まれているかどうか
		/// </summary>
		private bool IsKanColleLoaded { get; set; }

        private VolumeManager _volumeManager = null;

        private string _lastScreenShotPath;


		private NumericUpDown ToolMenu_Other_Volume_VolumeControl
		{
			get { return (NumericUpDown)((ToolStripControlHost)this.ToolMenu_Other_Volume.DropDownItems["ToolMenu_Other_Volume_VolumeControlHost"]).Control; }
		}

		private PictureBox ToolMenu_Other_LastScreenShot_Control
		{
			get { return (PictureBox)((ToolStripControlHost)this.ToolMenu_Other_LastScreenShot.DropDownItems["ToolMenu_Other_LastScreenShot_ImageHost"]).Control; }
		}



		/// <summary>
		/// </summary>
		/// <param name="serverUri">ホストプロセスとの通信用URL</param>
		public FormBrowser(string serverUri)
		{
            this.InitializeComponent();

            this.ServerUri = serverUri;
            this.StyleSheetApplied = false;

			// 音量設定用コントロールの追加
			{
				var control = new NumericUpDown();
				control.Name = "ToolMenu_Other_Volume_VolumeControl";
				control.Maximum = 100;
				control.TextAlign = HorizontalAlignment.Right;
				control.Font = this.ToolMenu_Other_Volume.Font;

				control.ValueChanged += this.ToolMenu_Other_Volume_ValueChanged;
				control.Tag = false;

				var host = new ToolStripControlHost(control, "ToolMenu_Other_Volume_VolumeControlHost");

				control.Size = new Size(host.Width - control.Margin.Horizontal, host.Height - control.Margin.Vertical);
				control.Location = new Point(control.Margin.Left, control.Margin.Top);


                this.ToolMenu_Other_Volume.DropDownItems.Add(host);
			}

			// スクリーンショットプレビューコントロールの追加
			{
				double zoomrate = 0.5;
				var control = new PictureBox();
				control.Name = "ToolMenu_Other_LastScreenShot_Image";
				control.SizeMode = PictureBoxSizeMode.Zoom;
				control.Size = new Size((int)(this.KanColleSize.Width * zoomrate), (int)(this.KanColleSize.Height * zoomrate));
				control.Margin = new Padding();
				control.Image = new Bitmap((int)(this.KanColleSize.Width * zoomrate), (int)(this.KanColleSize.Height * zoomrate), PixelFormat.Format24bppRgb);
				using (var g = Graphics.FromImage(control.Image))
				{
					g.Clear(SystemColors.Control);
					g.DrawString("스크린 샷을 아직 촬영하지 않았습니다.\r\n", this.Font, Brushes.Black, new Point(4, 4));
				}

				var host = new ToolStripControlHost(control, "ToolMenu_Other_LastScreenShot_ImageHost");

				host.Size = new Size(control.Width + control.Margin.Horizontal, control.Height + control.Margin.Vertical);
				host.AutoSize = false;
				control.Location = new Point(control.Margin.Left, control.Margin.Top);

				host.Click += this.ToolMenu_Other_LastScreenShot_ImageHost_Click;

                this.ToolMenu_Other_LastScreenShot.DropDownItems.Insert(0, host);
			}

		}


		private void FormBrowser_Load(object sender, EventArgs e)
		{
			SetWindowLong(this.Handle, GWL_STYLE, WS_CHILD);

            // ホストプロセスに接続
            this.BrowserHost = new PipeCommunicator<BrowserLib.IBrowserHost>(
				this, typeof(BrowserLib.IBrowser), this.ServerUri + "Browser", "Browser");
            this.BrowserHost.Connect(this.ServerUri + "/BrowserHost");
            this.BrowserHost.Faulted += this.BrowserHostChannel_Faulted;


            this.ConfigurationChanged(this.BrowserHost.Proxy.Configuration);


            // ウィンドウの親子設定＆ホストプロセスから接続してもらう
            this.BrowserHost.Proxy.ConnectToBrowser(this.Handle);

            // 親ウィンドウが生きているか確認 
            this.HeartbeatTimer.Tick += (EventHandler)((sender2, e2) =>
			{
                this.BrowserHost.AsyncRemoteRun(() => { this.HostWindow = this.BrowserHost.Proxy.HWND; });
			});
            this.HeartbeatTimer.Interval = 2000; // 2秒ごと　
            this.HeartbeatTimer.Start();

            this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.GetIconResource());

            this.InitializeBrowser();
		}


		/// <summary>
		/// ブラウザを初期化します。
		/// 最初の呼び出しのみ有効です。二回目以降は何もしません。
		/// </summary>
		void InitializeBrowser()
		{
			if (this.Browser != null)
				return;

			if (this.ProxySettings == null)
				return;


			var settings = new CefSettings()
			{
				BrowserSubprocessPath = Path.Combine(
						AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
						Environment.Is64BitProcess ? "x64" : "x86",
						"CefSharp.BrowserSubprocess.exe"),
				CachePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.BrowserCachePath),
				Locale = "ja",
				AcceptLanguageList = "ja,en-US,en,kr",        // todo: いる？
                LogSeverity = this.Configuration.SavesBrowserLog ? LogSeverity.Error : LogSeverity.Disable,
                LogFile = "BrowserLog.log",
            };

            if (!this.Configuration.HardwareAccelerationEnabled)
				settings.DisableGpuAcceleration();

			settings.CefCommandLineArgs.Add("proxy-server", this.ProxySettings);
            if (this.Configuration.ForceColorProfile)
                settings.CefCommandLineArgs.Add("force-color-profile", "srgb");
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            Cef.Initialize(settings, false, (IBrowserProcessHandler)null);

            var requestHandler = new CustomRequestHandler(pixiSettingEnabled: this.Configuration.PreserveDrawingBuffer);
            requestHandler.RenderProcessTerminated += (mes) => this.AddLog(3, mes);

            this.Browser = new ChromiumWebBrowser(@"about:blank")
			{
				Dock = DockStyle.None,
				Size = this.SizeAdjuster.Size,
                RequestHandler = requestHandler,
                MenuHandler = new MenuHandler(),
				KeyboardHandler = new KeyboardHandler(),
				DragHandler = new DragHandler(),
			};

            this.Browser.AddressChanged += this.Browser_AddressChanged;

            this.Browser.LoadingStateChanged += this.Browser_LoadingStateChanged;
            this.Browser.IsBrowserInitializedChanged += this.Browser_IsBrowserInitializedChanged;

            this.SizeAdjuster.Controls.Add(this.Browser);

            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CefEOBrowser"))) {
                Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CefEOBrowser"), true);
                this.AddLog(3, "해당 버전에서 이전버전의 브라우저 관련 쓸모없는 파일들을 삭제하였습니다.");
            }
		}

        private void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            
            if (e.Address.Equals("http://games.dmm.com/detail/kancolle/"))
            {
                Uri uri;
                string SauceCookie = "javascript:void(eval(\"document.cookie = 'cklg=ja;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=osapi.dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=203.104.209.7;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\"));location.href=\"http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/\";";
                if (Uri.TryCreate(SauceCookie, UriKind.Absolute, out uri))
                {
                    this.Browser.Load(uri.ToString());
                }
            }

            if (e.Address.Contains("foreign") && e.Address.Contains("dmm.com"))
            {
                Uri uri;
                string SauceCookie = "javascript:void(eval(\"document.cookie = 'cklg=ja;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=osapi.dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=203.104.209.7;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\"));location.href=\"http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/\";";
                if (Uri.TryCreate(SauceCookie, UriKind.Absolute, out uri))
                {
                    this.Browser.Load(uri.ToString());
                }
            }

            if (e.Address.Contains("foreign") && e.Address.Contains("dmm.com"))
            {
                Uri uri;
                string SauceCookie = "javascript:void(eval(\"document.cookie = 'cklg=ja;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=osapi.dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=203.104.209.7;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\"));location.href=\"http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/\";";
                if (Uri.TryCreate(SauceCookie, UriKind.Absolute, out uri))
                {
                    this.Browser.Load(uri.ToString());
                }
            }

            if (e.Address.Contains("error/area") && e.Address.Contains("dmm.com"))
            {
                Uri uri;
                string SauceCookie = "javascript:void(eval(\"document.cookie = 'cklg=ja;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=osapi.dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=203.104.209.7;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2021 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\"));location.href=\"http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/\";";
                if (Uri.TryCreate(SauceCookie, UriKind.Absolute, out uri))
                {
                    this.Browser.Load(uri.ToString());
                }
            }
            
        }

        void Exit()
		{
			if (!this.BrowserHost.Closed)
			{
                this.BrowserHost.Close();
                this.HeartbeatTimer.Stop();
				Cef.Shutdown();
				Application.Exit();
			}
		}

		void BrowserHostChannel_Faulted(Exception e)
		{
            // 親と通信できなくなったら終了する
            this.Exit();
		}

		public void CloseBrowser()
		{
            this.HeartbeatTimer.Stop();
            // リモートコールでClose()呼ぶのばヤバそうなので非同期にしておく
            this.BeginInvoke((Action)(() => this.Exit()));
		}

		public void ConfigurationChanged(BrowserLib.BrowserConfiguration conf)
		{
            this.Configuration = conf;

            this.SizeAdjuster.AutoScroll = this.Configuration.IsScrollable;
            this.ToolMenu_Other_Zoom_Fit.Checked = this.Configuration.ZoomFit;
            this.ApplyZoom();
            this.ToolMenu_Other_AppliesStyleSheet.Checked = this.Configuration.AppliesStyleSheet;
            this.ToolMenu.Dock = (DockStyle)this.Configuration.ToolMenuDockStyle;
            this.ToolMenu.Visible = this.Configuration.IsToolMenuVisible;

            this.ToolMenu_CacheClear.Visible = conf.EnableDebugMenu;

            switch (conf.Theme)
            {
                default:
                case 0:
                    this.BackColor = SystemColors.Control;
                    this.ForeColor = SystemColors.ControlText;
                    this.ToolMenu.BackColor = SystemColors.Control;
                    this.ToolMenu.ForeColor = SystemColors.ControlText;
                    break;
                case 1:
                    var charcoal = Color.FromArgb(0x22, 0x22, 0x22);
                    this.BackColor = charcoal;
                    this.ForeColor = SystemColors.Control;
                    this.ToolMenu.BackColor = charcoal;
                    this.ToolMenu.ForeColor = SystemColors.Control;
                    break;
            }


        }

        private void ConfigurationUpdated()
		{
            this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.ConfigurationUpdated(this.Configuration));
		}

		private void AddLog(int priority, string message)
		{
            this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.AddLog(priority, message));
		}

		private void SendErrorReport(string exceptionName, string message)
		{
            this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.SendErrorReport(exceptionName, message));
		}


		public void InitialAPIReceived()
		{
            this.IsKanColleLoaded = true;

            //ロード直後の適用ではレイアウトがなぜか崩れるのでこのタイミングでも適用
            this.ApplyStyleSheet();
            this.ApplyZoom();
            this.DestroyDMMreloadDialog();

            //起動直後はまだ音声が鳴っていないのでミュートできないため、この時点で有効化
            this.SetVolumeState();
		}


		private void SizeAdjuster_SizeChanged(object sender, EventArgs e)
		{
			if (!this.StyleSheetApplied)
			{
				if (this.Browser != null)
				{
                    this.Browser.Location = new Point(0, 0);
                    this.Browser.Size = this.SizeAdjuster.Size;
				}
				return;
			}

            this.ApplyZoom();
		}

		private void CenteringBrowser()
		{
			if (this.SizeAdjuster.Width == 0 || this.SizeAdjuster.Height == 0) return;
			int x = this.Browser.Location.X, y = this.Browser.Location.Y;
			bool isScrollable = this.Configuration.IsScrollable;

			if (!isScrollable || this.Browser.Width <= this.SizeAdjuster.Width)
			{
				x = (this.SizeAdjuster.Width - this.Browser.Width) / 2;
			}
			if (!isScrollable || this.Browser.Height <= this.SizeAdjuster.Height)
			{
				y = (this.SizeAdjuster.Height - this.Browser.Height) / 2;
			}

            //if ( x != Browser.Location.X || y != Browser.Location.Y )
            this.Browser.Location = new Point(x, y);
		}


		private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
		{
			// DocumentCompleted に相当?
			// note: 非 UI thread からコールされるので、何かしら UI に触る場合は適切な処置が必要

			if (e.IsLoading)
				return;

            this.BeginInvoke((Action)(() =>
			{
                this.ApplyStyleSheet();

                this.ApplyZoom();
                this.DestroyDMMreloadDialog();
			}));
		}


		private bool IsBrowserInitialized =>
            this.Browser != null &&
            this.Browser.IsBrowserInitialized;

		private IFrame GetMainFrame()
		{
			if (!this.IsBrowserInitialized)
				return null;

			var browser = this.Browser.GetBrowser();
			var frame = browser.MainFrame;

			if (frame?.Url?.Contains(@"http://www.dmm.com/netgame/social/") ?? false)
				return frame;

			return null;
		}

		private IFrame GetGameFrame()
		{
			if (!this.IsBrowserInitialized)
				return null;

			var browser = this.Browser.GetBrowser();
			var frames = browser.GetFrameIdentifiers()
						.Select(id => browser.GetFrame(id));

			return frames.FirstOrDefault(f => f?.Url?.Contains(@"http://osapi.dmm.com/gadgets/") ?? false);
		}

		private IFrame GetKanColleFrame()
		{
			if (!this.IsBrowserInitialized)
				return null;

			var browser = this.Browser.GetBrowser();
			var frames = browser.GetFrameIdentifiers()
					.Select(id => browser.GetFrame(id));

			return frames.FirstOrDefault(f => f?.Url?.Contains(@"/kcs2/index.php") ?? false);
		}

		/// <summary>
		/// スタイルシートを適用します。
		/// </summary>
		public void ApplyStyleSheet()
		{
			if (!this.IsBrowserInitialized)
				return;

			if (!this.Configuration.AppliesStyleSheet && !this.RestoreStyleSheet)
				return;

			try
			{
				var mainframe = this.GetMainFrame();
				var gameframe = this.GetGameFrame();
				if (mainframe == null || gameframe == null)
					return;

				if (this.RestoreStyleSheet)
				{
					mainframe.EvaluateScriptAsync(string.Format(Properties.Resources.RestoreScript, this.StyleClassID));
					gameframe.EvaluateScriptAsync(string.Format(Properties.Resources.RestoreScript, this.StyleClassID));
                    this.StyleSheetApplied = false;
                    this.RestoreStyleSheet = false;
				}
				else
				{
					mainframe.EvaluateScriptAsync(string.Format(Properties.Resources.PageScript, this.StyleClassID));
					gameframe.EvaluateScriptAsync(string.Format(Properties.Resources.FrameScript, this.StyleClassID));
				}

                this.StyleSheetApplied = true;

			}
			catch (Exception ex)
			{
                this.SendErrorReport(ex.ToString(), "스타일 시트의 적용에 실패했습니다.");
			}

		}

		/// <summary>
		/// DMMによるページ更新ダイアログを非表示にします。
		/// </summary>
		public void DestroyDMMreloadDialog()
		{
			if (!this.IsBrowserInitialized)
				return;

			if (!this.Configuration.IsDMMreloadDialogDestroyable)
				return;

			try
			{
				var mainframe = this.GetMainFrame();
				if (mainframe == null)
					return;

				mainframe.EvaluateScriptAsync(Properties.Resources.DMMScript);
			}
			catch (Exception ex)
			{
                this.SendErrorReport(ex.ToString(), "DMM의 페이지 새로고침 안내창 숨기기에 실패했습니다.");
			}

		}

        private string navigateCache = null;
        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            if (this.IsBrowserInitialized && this.navigateCache != null)
            {
                // ロードが完了したので再試行
                string url = this.navigateCache;            // 非同期コールするのでコピーを取っておく必要がある
                this.BeginInvoke((Action)(() => this.Navigate(url)));
                this.navigateCache = null;
            }
        }

        /// <summary>
        /// 指定した URL のページを開きます。
        /// </summary>
        public void Navigate(string url)
		{
            if (url != this.Configuration.LogInPageURL || !this.Configuration.AppliesStyleSheet)
                this.StyleSheetApplied = false;
            this.Browser.Load(url);

            if (!this.IsBrowserInitialized)
            {
                // 大方ロードできないのであとで再試行する
                this.navigateCache = url;
            }
        }

		/// <summary>
		/// ブラウザを再読み込みします。
		/// </summary>
		public void RefreshBrowser() => this.RefreshBrowser(false);

		/// <summary>
		/// ブラウザを再読み込みします。
		/// </summary>
		/// <param name="ignoreCache">キャッシュを無視するか。</param>
		public void RefreshBrowser(bool ignoreCache)
		{
			if (!this.Configuration.AppliesStyleSheet)
                this.StyleSheetApplied = false;

            this.Browser.Reload(ignoreCache);
		}

		/// <summary>
		/// ズームを適用します。
		/// </summary>
		public void ApplyZoom()
		{
			if (!this.IsBrowserInitialized)
				return;


			double zoomRate = this.Configuration.ZoomRate;
			bool fit = this.Configuration.ZoomFit && this.StyleSheetApplied;


			double zoomFactor;

			if (fit)
			{
				double rateX = (double)this.SizeAdjuster.Width / this.KanColleSize.Width;
				double rateY = (double)this.SizeAdjuster.Height / this.KanColleSize.Height;
				zoomFactor = Math.Min(rateX, rateY);
			}
			else
			{
				if (zoomRate < 0.1)
					zoomRate = 0.1;
				if (zoomRate > 10)
					zoomRate = 10;

				zoomFactor = zoomRate;
			}


            this.Browser.SetZoomLevel(Math.Log(zoomFactor, 1.2));


			if (this.StyleSheetApplied)
			{
                this.Browser.Size = this.Browser.MinimumSize = new Size(
					(int)(this.KanColleSize.Width * zoomFactor),
					(int)(this.KanColleSize.Height * zoomFactor)
					);

                this.CenteringBrowser();
			}

			if (fit)
			{
                this.ToolMenu_Other_Zoom_Current.Text = "현재: 화면에맞춤";
			}
			else
			{
                this.ToolMenu_Other_Zoom_Current.Text = $"현재: {zoomRate:p1}";
			}

		}



        /// <summary>
        /// スクリーンショットを撮影します。
        /// </summary>
        private async Task<Bitmap> TakeScreenShot()
        {
            var kancolleFrame = this.GetKanColleFrame();

            if (kancolleFrame == null)
            {
                this.AddLog(3, string.Format("칸코레가 실행되지 않아 스크린 샷을 찍을 수 없습니다."));
				System.Media.SystemSounds.Beep.Play();
                return null;
            }


            Task<ScreenShotPacket> InternalTakeScreenShot()
            {
                var request = new ScreenShotPacket();

                if (this.Browser == null || !this.Browser.IsBrowserInitialized)
                    return request.TaskSource.Task;


                string script = $@"
(async function() 
{{
	await CefSharp.BindObjectAsync('{request.ID}');
	let canvas = document.querySelector('canvas');
	requestAnimationFrame(() =>
	{{
		let dataurl = canvas.toDataURL('image/png');
		{request.ID}.complete(dataurl);
	}});
}})();
";

                this.Browser.JavascriptObjectRepository.Register(request.ID, request, true);
                kancolleFrame.ExecuteJavaScriptAsync(script);

                return request.TaskSource.Task;
            }

            var result = await InternalTakeScreenShot();

            // ごみ掃除
            this.Browser.JavascriptObjectRepository.UnRegister(result.ID);
            kancolleFrame.ExecuteJavaScriptAsync($@"delete {result.ID}");

            return result.GetImage();
        }

		/// <summary>
		/// スクリーンショットを撮影し、設定で指定された保存先に保存します。
		/// </summary>
		public async Task SaveScreenShot()
		{

			int savemode = this.Configuration.ScreenShotSaveMode;
			int format = this.Configuration.ScreenShotFormat;
			string folderPath = this.Configuration.ScreenShotPath;
			bool is32bpp = format != 1 && this.Configuration.AvoidTwitterDeterioration;

			Bitmap image = null;
            try
            {
                image = await this.TakeScreenShot();


                if (image == null)
                    return;

                if (is32bpp)
                {
                    if (image.PixelFormat != PixelFormat.Format32bppArgb)
                    {
                        var imgalt = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                        using (var g = Graphics.FromImage(imgalt))
                        {
                            g.DrawImage(image, new Rectangle(0, 0, imgalt.Width, imgalt.Height));
                        }

                        image.Dispose();
                        image = imgalt;
                    }

                    // 不透明ピクセルのみだと jpeg 化されてしまうため、1px だけわずかに透明にする
                    Color temp = image.GetPixel(image.Width - 1, image.Height - 1);
                    image.SetPixel(image.Width - 1, image.Height - 1, Color.FromArgb(252, temp.R, temp.G, temp.B));
                }
                else
                {
                    if (image.PixelFormat != PixelFormat.Format24bppRgb)
                    {
                        var imgalt = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                        using (var g = Graphics.FromImage(imgalt))
                        {
                            g.DrawImage(image, new Rectangle(0, 0, imgalt.Width, imgalt.Height));
                        }

                        image.Dispose();
                        image = imgalt;
                    }
                }


                // to file
                if ((savemode & 1) != 0)
				{
					try
					{
						if (!Directory.Exists(folderPath))
							Directory.CreateDirectory(folderPath);

						string ext;
						ImageFormat imgFormat;

						switch (format)
						{
							case 1:
								ext = "jpg";
								imgFormat = ImageFormat.Jpeg;
								break;
							case 2:
							default:
								ext = "png";
								imgFormat = ImageFormat.Png;
								break;
						}

						string path = $"{folderPath}\\{DateTime.Now:yyyyMMdd_HHmmssff}.{ext}";
						image.Save(path, imgFormat);
                        this._lastScreenShotPath = path;

                        this.AddLog(2, $"스크린 샷을 {path} 에 저장 했습니다.");
					}
					catch (Exception ex)
					{
                        this.SendErrorReport(ex.ToString(), "스크린 샷 저장에 실패했습니다.");
					}
				}


				// to clipboard
				if ((savemode & 2) != 0)
				{
					try
					{
						Clipboard.SetImage(image);

						if ((savemode & 3) != 3)
                            this.AddLog(2, "스크린 샷을 클립 보드에 복사했습니다.");
					}
					catch (Exception ex)
					{
                        this.SendErrorReport(ex.ToString(), "스크린 샷 클립 보드 복사에 실패했습니다.");
					}
				}

			}
			catch (Exception ex)
			{
                this.SendErrorReport(ex.ToString(), "스크린 샷 촬영에 실패했습니다.");
			}
			finally
			{
				image?.Dispose();
			}

		}


		public void SetProxy(string proxy)
		{
			ushort port;
			if (ushort.TryParse(proxy, out port))
			{
				WinInetUtil.SetProxyInProcessForNekoxy(port);
                this.ProxySettings = "http=127.0.0.1:" + port;           // todo: 動くには動くが正しいかわからない
			}
			else
			{
				WinInetUtil.SetProxyInProcess(proxy, "local");
                this.ProxySettings = proxy;
			}

            this.InitializeBrowser();

            this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.SetProxyCompleted());
		}


		/// <summary>
		/// キャッシュを削除します。
		/// </summary>
		private bool ClearCache(long timeoutMilliseconds = 5000)
		{
			// note: Cef が起動している状態では削除できない X(
			// 今のところ手動でやってもらうことにする

			return true;
		}


		public void SetIconResource(byte[] canvas)
		{

			string[] keys = new string[] {
				"Browser_ScreenShot",
				"Browser_Zoom",
				"Browser_ZoomIn",
				"Browser_ZoomOut",
				"Browser_Unmute",
				"Browser_Mute",
				"Browser_Refresh",
				"Browser_Navigate",
				"Browser_Other",
			};
			int unitsize = 16 * 16 * 4;

			for (int i = 0; i < keys.Length; i++)
			{
				Bitmap bmp = new Bitmap(16, 16, PixelFormat.Format32bppArgb);

				if (canvas != null)
				{
					BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					Marshal.Copy(canvas, unitsize * i, bmpdata.Scan0, unitsize);
					bmp.UnlockBits(bmpdata);
				}

                this.Icons.Images.Add(keys[i], bmp);
			}


            this.ToolMenu_ScreenShot.Image = this.ToolMenu_Other_ScreenShot.Image =
                this.Icons.Images["Browser_ScreenShot"];
            this.ToolMenu_Zoom.Image = this.ToolMenu_Other_Zoom.Image =
                this.Icons.Images["Browser_Zoom"];
            this.ToolMenu_Other_Zoom_Increment.Image =
                this.Icons.Images["Browser_ZoomIn"];
            this.ToolMenu_Other_Zoom_Decrement.Image =
                this.Icons.Images["Browser_ZoomOut"];
            this.ToolMenu_Refresh.Image = this.ToolMenu_Other_Refresh.Image =
                this.Icons.Images["Browser_Refresh"];
            this.ToolMenu_NavigateToLogInPage.Image = this.ToolMenu_Other_NavigateToLogInPage.Image =
                this.Icons.Images["Browser_Navigate"];
            this.ToolMenu_Other.Image =
                this.Icons.Images["Browser_Other"];

            this.SetVolumeState();
		}

        private void TryGetVolumeManager()
        {
            this._volumeManager = VolumeManager.CreateInstanceByProcessName("CefSharp.BrowserSubprocess");
        }


        private void SetVolumeState()
		{

			bool mute;
			float volume;

			try
			{
                if (this._volumeManager == null)
                {
                    this.TryGetVolumeManager();
                }

                mute = this._volumeManager.IsMute;
				volume = this._volumeManager.Volume * 100;

			}
			catch (Exception)
			{
				this._volumeManager = null;
				// 音量データ取得不能時
				mute = false;
				volume = 100;
			}

            this.ToolMenu_Mute.Image = this.ToolMenu_Other_Mute.Image =
                this.Icons.Images[mute ? "Browser_Mute" : "Browser_Unmute"];

			{
				var control = this.ToolMenu_Other_Volume_VolumeControl;
				control.Tag = false;
				control.Value = (decimal)volume;
				control.Tag = true;
			}

            this.Configuration.Volume = volume;
            this.Configuration.IsMute = mute;
            this.ConfigurationUpdated();
		}


		private async void ToolMenu_Other_ScreenShot_Click(object sender, EventArgs e)
		{
			await this.SaveScreenShot();
		}

		private void ToolMenu_Other_Zoom_Decrement_Click(object sender, EventArgs e)
		{
            this.Configuration.ZoomRate = Math.Max(this.Configuration.ZoomRate - 0.2, 0.1);
            this.Configuration.ZoomFit = this.ToolMenu_Other_Zoom_Fit.Checked = false;
            this.ApplyZoom();
            this.ConfigurationUpdated();
		}

		private void ToolMenu_Other_Zoom_Increment_Click(object sender, EventArgs e)
		{
            this.Configuration.ZoomRate = Math.Min(this.Configuration.ZoomRate + 0.2, 10);
            this.Configuration.ZoomFit = this.ToolMenu_Other_Zoom_Fit.Checked = false;
            this.ApplyZoom();
            this.ConfigurationUpdated();
		}

		private void ToolMenu_Other_Zoom_Click(object sender, EventArgs e)
		{

			double zoom;

			if (sender == this.ToolMenu_Other_Zoom_25)
				zoom = 0.25;
			else if (sender == this.ToolMenu_Other_Zoom_50)
				zoom = 0.50;
			else if (sender == this.ToolMenu_Other_Zoom_Classic)
				zoom = 0.667;       // 2/3 ジャストだと 799x479 になる
			else if (sender == this.ToolMenu_Other_Zoom_75)
				zoom = 0.75;
			else if (sender == this.ToolMenu_Other_Zoom_100)
				zoom = 1;
			else if (sender == this.ToolMenu_Other_Zoom_150)
				zoom = 1.5;
			else if (sender == this.ToolMenu_Other_Zoom_200)
				zoom = 2;
			else if (sender == this.ToolMenu_Other_Zoom_250)
				zoom = 2.5;
			else if (sender == this.ToolMenu_Other_Zoom_300)
				zoom = 3;
			else if (sender == this.ToolMenu_Other_Zoom_400)
				zoom = 4;
			else
				zoom = 1;

            this.Configuration.ZoomRate = zoom;
            this.Configuration.ZoomFit = this.ToolMenu_Other_Zoom_Fit.Checked = false;
            this.ApplyZoom();
            this.ConfigurationUpdated();
		}

		private void ToolMenu_Other_Zoom_Fit_Click(object sender, EventArgs e)
		{
            this.Configuration.ZoomFit = this.ToolMenu_Other_Zoom_Fit.Checked;
            this.ApplyZoom();
            this.ConfigurationUpdated();
		}


		//ズームUIの使いまわし
		private void ToolMenu_Other_DropDownOpening(object sender, EventArgs e)
		{
			var list = this.ToolMenu_Zoom.DropDownItems.Cast<ToolStripItem>().ToArray();
            this.ToolMenu_Other_Zoom.DropDownItems.AddRange(list);
		}

		private void ToolMenu_Zoom_DropDownOpening(object sender, EventArgs e)
		{

			var list = this.ToolMenu_Other_Zoom.DropDownItems.Cast<ToolStripItem>().ToArray();
            this.ToolMenu_Zoom.DropDownItems.AddRange(list);
		}


		private void ToolMenu_Other_Mute_Click(object sender, EventArgs e)
		{
            if (this._volumeManager == null)
            {
                this.TryGetVolumeManager();
            }

            try
			{
                this._volumeManager.ToggleMute();

			}
			catch (Exception)
			{
				System.Media.SystemSounds.Beep.Play();
			}

            this.SetVolumeState();
		}

		void ToolMenu_Other_Volume_ValueChanged(object sender, EventArgs e)
		{
			var control = this.ToolMenu_Other_Volume_VolumeControl;

            if (this._volumeManager == null)
            {
                this.TryGetVolumeManager();
            }

            try
            {
				if ((bool)control.Tag)
                    this._volumeManager.Volume = (float)(control.Value / 100);
				control.BackColor = SystemColors.Window;

			}
			catch (Exception)
			{
				control.BackColor = Color.MistyRose;

			}

		}


		private void ToolMenu_Other_Refresh_Click(object sender, EventArgs e)
		{

			if (!this.Configuration.ConfirmAtRefresh ||
				MessageBox.Show("새로고침합니다.\r\n진행하시겠습니까?", "확인",
				MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
				== DialogResult.OK)
			{
                this.RefreshBrowser();
			}
		}

		private void ToolMenu_Other_RefreshIgnoreCache_Click(object sender, EventArgs e)
		{
			if (!this.Configuration.ConfirmAtRefresh ||
				MessageBox.Show("캐시를 무시하고 새로고침합니다.\r\n진행하시겠습니까?", "확인",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
				== DialogResult.OK)
			{
                this.RefreshBrowser(true);
			}
		}

		private void ToolMenu_Other_NavigateToLogInPage_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show("로그인 페이지로 이동합니다.\r\n진행하시겠습니까?", "확인",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
				== DialogResult.OK)
			{

                this.Navigate(this.Configuration.LogInPageURL);
			}

		}

		private void ToolMenu_Other_Navigate_Click(object sender, EventArgs e)
		{
            this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.RequestNavigation(this.Browser.GetMainFrame()?.Url ?? ""));
		}

		private void ToolMenu_Other_AppliesStyleSheet_Click(object sender, EventArgs e)
		{
            this.Configuration.AppliesStyleSheet = this.ToolMenu_Other_AppliesStyleSheet.Checked;
			if (!this.Configuration.AppliesStyleSheet)
                this.RestoreStyleSheet = true;
            this.ApplyStyleSheet();
            this.ApplyZoom();
            this.ConfigurationUpdated();
		}

		private void ToolMenu_Other_Alignment_Click(object sender, EventArgs e)
		{

			if (sender == this.ToolMenu_Other_Alignment_Top)
                this.ToolMenu.Dock = DockStyle.Top;
			else if (sender == this.ToolMenu_Other_Alignment_Bottom)
                this.ToolMenu.Dock = DockStyle.Bottom;
			else if (sender == this.ToolMenu_Other_Alignment_Left)
                this.ToolMenu.Dock = DockStyle.Left;
			else
                this.ToolMenu.Dock = DockStyle.Right;

            this.Configuration.ToolMenuDockStyle = (int)this.ToolMenu.Dock;

            this.ConfigurationUpdated();
		}

		private void ToolMenu_Other_Alignment_Invisible_Click(object sender, EventArgs e)
		{
            this.ToolMenu.Visible =
            this.Configuration.IsToolMenuVisible = false;
            this.ConfigurationUpdated();
		}



		private void SizeAdjuster_DoubleClick(object sender, EventArgs e)
		{
            this.ToolMenu.Visible =
            this.Configuration.IsToolMenuVisible = true;
            this.ConfigurationUpdated();
		}

		private void ContextMenuTool_ShowToolMenu_Click(object sender, EventArgs e)
		{
            this.ToolMenu.Visible =
            this.Configuration.IsToolMenuVisible = true;
            this.ConfigurationUpdated();
		}

		private void ContextMenuTool_Opening(object sender, CancelEventArgs e)
		{

			if (this.IsKanColleLoaded || this.ToolMenu.Visible)
				e.Cancel = true;
		}


		private void ToolMenu_ScreenShot_Click(object sender, EventArgs e)
		{
            this.ToolMenu_Other_ScreenShot_Click(sender, e);
		}

		private void ToolMenu_Mute_Click(object sender, EventArgs e)
		{
            this.ToolMenu_Other_Mute_Click(sender, e);
		}

		private void ToolMenu_Refresh_Click(object sender, EventArgs e)
		{
            this.ToolMenu_Other_Refresh_Click(sender, e);
		}

		private void ToolMenu_NavigateToLogInPage_Click(object sender, EventArgs e)
		{
            this.ToolMenu_Other_NavigateToLogInPage_Click(sender, e);
		}




		private void FormBrowser_Activated(object sender, EventArgs e)
		{
            this.Browser.Focus();
		}

		private void ToolMenu_Other_Alignment_DropDownOpening(object sender, EventArgs e)
		{

			foreach (var item in this.ToolMenu_Other_Alignment.DropDownItems)
			{
				var menu = item as ToolStripMenuItem;
				if (menu != null)
				{
					menu.Checked = false;
				}
			}

			switch ((DockStyle)this.Configuration.ToolMenuDockStyle)
			{
				case DockStyle.Top:
                    this.ToolMenu_Other_Alignment_Top.Checked = true;
					break;
				case DockStyle.Bottom:
                    this.ToolMenu_Other_Alignment_Bottom.Checked = true;
					break;
				case DockStyle.Left:
                    this.ToolMenu_Other_Alignment_Left.Checked = true;
					break;
				case DockStyle.Right:
                    this.ToolMenu_Other_Alignment_Right.Checked = true;
					break;
			}

            this.ToolMenu_Other_Alignment_Invisible.Checked = !this.Configuration.IsToolMenuVisible;
		}


		private void ToolMenu_Other_LastScreenShot_DropDownOpening(object sender, EventArgs e)
		{

			try
			{

				using (var fs = new FileStream(this._lastScreenShotPath, FileMode.Open, FileAccess.Read))
				{
                    this.ToolMenu_Other_LastScreenShot_Control.Image?.Dispose();

                    this.ToolMenu_Other_LastScreenShot_Control.Image = Image.FromStream(fs);
				}

			}
			catch (Exception)
			{
				// *ぷちっ*
			}

		}

		void ToolMenu_Other_LastScreenShot_ImageHost_Click(object sender, EventArgs e)
		{
			if (this._lastScreenShotPath != null && File.Exists(this._lastScreenShotPath))
				Process.Start(this._lastScreenShotPath);
		}

		private void ToolMenu_Other_LastScreenShot_OpenScreenShotFolder_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(this.Configuration.ScreenShotPath))
				Process.Start(this.Configuration.ScreenShotPath);
		}

		private void ToolMenu_Other_LastScreenShot_CopyToClipboard_Click(object sender, EventArgs e)
		{

			if (this._lastScreenShotPath != null && File.Exists(this._lastScreenShotPath))
			{
				try
				{
					using (var img = new Bitmap(this._lastScreenShotPath))
					{
						Clipboard.SetImage(img);
                        this.AddLog(2, string.Format("스크린 샷 {0}을 클립 보드에 복사했습니다.", this._lastScreenShotPath));
					}
				}
				catch (Exception ex)
				{
                    this.SendErrorReport(ex.Message, "스크린 샷 클립 보드 복사에 실패했습니다.");
				}
			}
		}

		private void ToolMenu_Other_OpenDevTool_Click(object sender, EventArgs e)
		{
			if (!this.IsBrowserInitialized)
				return;

            this.Browser.GetBrowser().ShowDevTools();
		}

        private void ToolMenu_CacheClear_Click(object sender, EventArgs e)
        {
            if (!this.IsBrowserInitialized)
                return;

            if (MessageBox.Show("브라우저의 캐시를 삭제합니다. 브라우저의 재 시작이 필요하며, \r\n일부 환경에 따라 본 프로그램이 종료될 수 있습니다.\r\n진행하시겠습니까?", "확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.BrowserHost.AsyncRemoteRun(() => this.BrowserHost.Proxy.ClearCache());
            }
        }

        private void Cache_Clear()
        {
            
        }

        protected override void WndProc(ref Message m)
		{

			if (m.Msg == WM_ERASEBKGND)
				// ignore this message
				return;

			base.WndProc(ref m);
		}


		#region 呪文


		[DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
		private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

		private const int GWL_STYLE = (-16);
		private const uint WS_CHILD = 0x40000000;
		private const uint WS_VISIBLE = 0x10000000;
		private const int WM_ERASEBKGND = 0x14;




		#endregion


	}



	/// <summary>
	/// ウィンドウが非アクティブ状態から1回のクリックでボタンが押せる ToolStrip です。
	/// </summary>
	internal class ExtraToolStrip : ToolStrip
	{
		public ExtraToolStrip() : base() { }

		private const uint WM_MOUSEACTIVATE = 0x21;
		private const uint MA_ACTIVATE = 1;
		private const uint MA_ACTIVATEANDEAT = 2;
		private const uint MA_NOACTIVATE = 3;
		private const uint MA_NOACTIVATEANDEAT = 4;

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == WM_MOUSEACTIVATE && m.Result == (IntPtr)MA_ACTIVATEANDEAT)
				m.Result = (IntPtr)MA_ACTIVATE;
		}
	}

}
