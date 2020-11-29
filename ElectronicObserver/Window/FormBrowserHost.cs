using BrowserLib;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Mathematics;
using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window
{

	/// <summary>
	/// ブラウザのホスト側フォーム
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public partial class FormBrowserHost : DockContent, IBrowserHost
	{


		public static readonly string BrowserExeName = "EOBrowser.exe";


		/// <summary>
		/// FormBrowserHostの通信サーバ
		/// </summary>
		private string ServerUri = "net.pipe://localhost/" + Process.GetCurrentProcess().Id + "/ElectronicObserver";

		/// <summary>
		/// FormBrowserとの通信インターフェース
		/// </summary>
		private PipeCommunicator<IBrowser> Browser;

		private Process BrowserProcess;

		private IntPtr BrowserWnd = IntPtr.Zero;



		[Flags]
		private enum InitializationStageFlag
		{
			InitialAPILoaded = 1,
			BrowserConnected = 2,
			SetProxyCompleted = 4,
			Completed = 7,
		}

		/// <summary>
		/// 初期化ステージカウント
		/// 完了したらログインページを開く (各処理が終わらないと正常にロードできないため)
		/// </summary>
		private InitializationStageFlag _initializationStage = 0;
		private InitializationStageFlag InitializationStage
		{
			get { return this._initializationStage; }
			set
			{
				//AddLog( 1, _initializationStage + " -> " + value );
				if (this._initializationStage != InitializationStageFlag.Completed && value == InitializationStageFlag.Completed)
				{
					if (Utility.Configuration.Config.FormBrowser.IsEnabled)
					{
                        this.NavigateToLogInPage();
					}
				}

                this._initializationStage = value;
			}
		}



		public FormBrowserHost(FormMain parent)
		{
            this.InitializeComponent();

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBrowser]);
		}

		public void InitializeApiCompleted()
		{
            this.InitializationStage |= InitializationStageFlag.InitialAPILoaded;
		}

		private void FormBrowser_Load(object sender, EventArgs e)
		{
            this.LaunchBrowserProcess();
		}


		private void LaunchBrowserProcess()
		{
            // 通信サーバ起動
            this.Browser = new PipeCommunicator<IBrowser>(
				this, typeof(IBrowserHost), this.ServerUri, "BrowserHost");

			try
			{
				// プロセス起動

				if (System.IO.File.Exists(BrowserExeName))
                    this.BrowserProcess = Process.Start(BrowserExeName, this.ServerUri);

				else    //デバッグ環境用 作業フォルダにかかわらず自分と同じフォルダのを参照する
                    this.BrowserProcess = Process.Start(
						System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + BrowserExeName,
                        this.ServerUri);

				// 残りはサーバに接続してきたブラウザプロセスがドライブする

			}
			catch (Exception ex)
			{
				Utility.ErrorReporter.SendErrorReport(ex, "브라우저를 시작하지 못했습니다.");
				MessageBox.Show("브라우저를 시작하지 못했습니다.\r\n" + ex.Message,
					"에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		internal void ConfigurationChanged()
		{
            this.Font = Utility.Configuration.Config.UI.MainFont;
            this.Configuration.Theme = (uint)Utility.Configuration.Config.UI.Theme;
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.ConfigurationChanged(this.Configuration));
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.ConfigurationChanged(this.Configuration));
        }


		//ロード直後の適用ではレイアウトがなぜか崩れるのでこのタイミングでも適用
		void InitialAPIReceived(string apiname, dynamic data)
		{
			if (this.InitializationStage != InitializationStageFlag.Completed)       // 初期化が終わってから
				return;

            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.InitialAPIReceived());
		}


		/// <summary>
		/// 指定した URL のページを開きます。
		/// </summary>
		public void Navigate(string url)
		{
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.Navigate(url));
		}

		/// <summary>
		/// 艦これのログインページを開きます。
		/// </summary>
		public void NavigateToLogInPage()
		{
            this.Navigate(Utility.Configuration.Config.FormBrowser.LogInPageURL);
		}

		/// <summary>
		/// ブラウザを再読み込みします。
		/// </summary>
		public void RefreshBrowser()
		{
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.RefreshBrowser());
		}

		/// <summary>
		/// ズームを適用します。
		/// </summary>
		public void ApplyZoom()
		{
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.ApplyZoom());
		}

		/// <summary>
		/// スタイルシートを適用します。
		/// </summary>
		public void ApplyStyleSheet()
		{
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.ApplyStyleSheet());
		}

		/// <summary>
		/// DMMによるページ更新ダイアログを非表示にします。
		/// </summary>
		public void DestroyDMMreloadDialog()
		{
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.DestroyDMMreloadDialog());
		}


		/// <summary>
		/// スクリーンショットを保存します。
		/// </summary>
		public void SaveScreenShot()
		{
            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.SaveScreenShot());
		}


		public void SendErrorReport(string exceptionName, string message)
		{
			Utility.ErrorReporter.SendErrorReport(new Exception(exceptionName), message);
		}

		public void AddLog(int priority, string message)
		{
			Utility.Logger.Add(priority, message);
		}


		public BrowserLib.BrowserConfiguration Configuration
		{
			get
			{
				BrowserLib.BrowserConfiguration config = new BrowserLib.BrowserConfiguration();
				var c = Utility.Configuration.Config.FormBrowser;

				config.ZoomRate = c.ZoomRate;
				config.ZoomFit = c.ZoomFit;
				config.LogInPageURL = c.LogInPageURL;
				config.IsEnabled = c.IsEnabled;
				config.ScreenShotPath = c.ScreenShotPath;
				config.ScreenShotFormat = c.ScreenShotFormat;
				config.ScreenShotSaveMode = c.ScreenShotSaveMode;
				config.StyleSheet = c.StyleSheet;
				config.IsScrollable = c.IsScrollable;
				config.AppliesStyleSheet = c.AppliesStyleSheet;
				config.IsDMMreloadDialogDestroyable = c.IsDMMreloadDialogDestroyable;
				config.AvoidTwitterDeterioration = c.AvoidTwitterDeterioration;
				config.ToolMenuDockStyle = (int)c.ToolMenuDockStyle;
				config.IsToolMenuVisible = c.IsToolMenuVisible;
				config.ConfirmAtRefresh = c.ConfirmAtRefresh;
                config.HardwareAccelerationEnabled = c.HardwareAccelerationEnabled;
                config.PreserveDrawingBuffer = c.PreserveDrawingBuffer;
                config.Theme = (uint)Utility.Configuration.Config.UI.Theme;
                config.ForceColorProfile = c.ForceColorProfile;
                config.SavesBrowserLog = c.SavesBrowserLog;

                config.EnableDebugMenu = Utility.Configuration.Config.Debug.EnableDebugMenu;
                return config;
			}
		}

		public void ConfigurationUpdated(BrowserLib.BrowserConfiguration config)
		{

			var c = Utility.Configuration.Config.FormBrowser;

			c.ZoomRate = config.ZoomRate;
			c.ZoomFit = config.ZoomFit;
			c.LogInPageURL = config.LogInPageURL;
			c.IsEnabled = config.IsEnabled;
			c.ScreenShotPath = config.ScreenShotPath;
			c.ScreenShotFormat = config.ScreenShotFormat;
			c.ScreenShotSaveMode = config.ScreenShotSaveMode;
			c.StyleSheet = config.StyleSheet;
			c.IsScrollable = config.IsScrollable;
			c.AppliesStyleSheet = config.AppliesStyleSheet;
			c.IsDMMreloadDialogDestroyable = config.IsDMMreloadDialogDestroyable;
			c.AvoidTwitterDeterioration = config.AvoidTwitterDeterioration;
			c.ToolMenuDockStyle = (DockStyle)config.ToolMenuDockStyle;
			c.IsToolMenuVisible = config.IsToolMenuVisible;
			c.ConfirmAtRefresh = config.ConfirmAtRefresh;
            c.HardwareAccelerationEnabled = config.HardwareAccelerationEnabled;
            c.ForceColorProfile = config.ForceColorProfile;
            c.PreserveDrawingBuffer = config.PreserveDrawingBuffer;
            //Utility.Configuration.Config.Debug.EnableDebugMenu = config.EnableDebugMenu;

            // volume
            if (Utility.Configuration.Config.BGMPlayer.SyncBrowserMute)
			{
				Utility.SyncBGMPlayer.Instance.IsMute = config.IsMute;
			}
		}

		public void GetIconResource()
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

			byte[] canvas = new byte[unitsize * keys.Length];

			for (int i = 0; i < keys.Length; i++)
			{
				Image img = ResourceManager.Instance.Icons.Images[keys[i]];
				if (img != null)
				{
					using (Bitmap bmp = new Bitmap(img))
					{

						BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
						Marshal.Copy(bmpdata.Scan0, canvas, unitsize * i, unitsize);
						bmp.UnlockBits(bmpdata);

					}
				}
			}

            this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.SetIconResource(canvas));

		}


		public void RequestNavigation(string baseurl)
		{

			using (var dialog = new Window.Dialog.DialogTextInput("대상 주소 입력", "대상 URL을 입력하십시오."))
			{
				dialog.InputtedText = baseurl;

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

                    this.Navigate(dialog.InputtedText);
				}
			}

		}

        public async void ClearCache()
        {
            Utility.Logger.Add(2, "캐시 삭제를 위해 브라우저를 종료합니다.");

            try
            {
                if (!this.Browser?.Closed ?? false)
                {
                    this.Browser.Proxy?.CloseBrowser();

                    await this.Browser.CloseAsync(this);

                    this.TerminateBrowserProcess();
                }
            }
            catch (Exception) { }


            await this.ClearCacheAsync().ContinueWith(task =>
            {
                Utility.Logger.Add(2, "캐시 삭제가 완료되었습니다. 브라우저를 재시작합니다.");

                this._initializationStage = InitializationStageFlag.InitialAPILoaded;
                try
                {
                    this.LaunchBrowserProcess();
                }
                catch (Exception ex)
                {
                    Utility.ErrorReporter.SendErrorReport(ex, "브라우저 재시작에 실패했습니다.");
                    MessageBox.Show("브라우저 프로세스 재시작에 실패했습니다. \r\n 74식 전자관측의를 재시작해주세요.", ":(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task ClearCacheAsync()
        {
            int trial;
            Exception lastException = null;
            var dir = new DirectoryInfo("BrowserCache");

            for (trial = 0; trial < 4; trial++)
            {
                try
                {
                    dir.Refresh();

                    if (dir.Exists)
                        dir.Delete(true);
                    else
                        break;

                    for (int i = 0; i < 10; i++)
                    {
                        dir.Refresh();
                        if (dir.Exists)
                        {
                            await Task.Delay(50);
                        }
                        else break;
                    }
                    if (!dir.Exists)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    await Task.Delay(500);
                }
            }
            if (trial == 4)
            {
                Utility.ErrorReporter.SendErrorReport(lastException, "캐시 삭제를 실패했습니다.");
            }
        }

        public void ConnectToBrowser(IntPtr hwnd)
		{
            this.BrowserWnd = hwnd;

			// 子ウィンドウに設定
			SetParent(this.BrowserWnd, this.Handle);
			MoveWindow(this.BrowserWnd, 0, 0, this.Width, this.Height, true);

			//キー入力をブラウザに投げる
			Application.AddMessageFilter(new KeyMessageGrabber(this.BrowserWnd));

            // デッドロックするので非同期で処理
            this.BeginInvoke((Action)(() =>
			{
                // ブラウザプロセスに接続
                this.Browser.Connect(this.ServerUri + "Browser/Browser");
                this.Browser.Faulted += this.Browser_Faulted;

                this.ConfigurationChanged();

				Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

				APIObserver.Instance.APIList["api_start2/getData"].ResponseReceived +=
					(string apiname, dynamic data) => InitialAPIReceived(apiname, data);

                // プロキシをセット
                this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.SetProxy(this.BuildDownstreamProxy()));
				APIObserver.Instance.ProxyStarted += () =>
				{
                    this.Browser.AsyncRemoteRun(() => this.Browser.Proxy.SetProxy(this.BuildDownstreamProxy()));
				};

                this.InitializationStage |= InitializationStageFlag.BrowserConnected;

			}));
		}

		private string BuildDownstreamProxy()
		{
			var config = Utility.Configuration.Config.Connection;

			if (!string.IsNullOrEmpty(config.DownstreamProxy))
			{
				return config.DownstreamProxy;

			}
			else if (config.UseSystemProxy)
			{
				return APIObserver.Instance.ProxyPort.ToString();

			}
			else if (config.UseUpstreamProxy)
			{
				return string.Format(
					"http=127.0.0.1:{0};https={1}:{2}",
					APIObserver.Instance.ProxyPort,
					config.UpstreamProxyAddress,
					config.UpstreamProxyPort);
			}
			else
			{
				return string.Format("http=127.0.0.1:{0}", APIObserver.Instance.ProxyPort);
			}
		}


		public void SetProxyCompleted()
		{
            this.InitializationStage |= InitializationStageFlag.SetProxyCompleted;
		}


		void Browser_Faulted(Exception e)
		{
			if (this.Browser.Proxy == null)
			{
				Utility.ErrorReporter.SendErrorReport(e, "브라우저가 예기치 않게 종료되었습니다.");
			}
			else
			{
				Utility.ErrorReporter.SendErrorReport(e, "브라우저에 통신 에러가 발생했습니다.");
			}
		}


		private void TerminateBrowserProcess()
		{
			if (!this.BrowserProcess.WaitForExit(2000))
			{
				try
				{
                    // 2秒待って終了しなかったらKill
                    this.BrowserProcess.Kill();
				}
				catch (Exception)
				{
					// プロセスが既に終了してた場合などに例外が出る
				}
			}
            this.BrowserWnd = IntPtr.Zero;
		}

		public void CloseBrowser()
		{

			try
			{

				if (this.Browser == null)
				{
					// ブラウザを開いていない場合はnullなので
					return;
				}
				if (!this.Browser.Closed)
				{
					// ブラウザプロセスが異常終了した場合などはnullになる
					if (this.Browser.Proxy != null)
					{
                        this.Browser.Proxy.CloseBrowser();
					}
                    this.Browser.Close();
                    this.TerminateBrowserProcess();
				}

			}
			catch (Exception ex)
			{       //ブラウザプロセスが既に終了していた場合など

				Utility.ErrorReporter.SendErrorReport(ex, "브라우저 종료중 오류가 발생했습니다.");
			}

		}

		private void FormBrowserHost_Resize(object sender, EventArgs e)
		{
			if (this.BrowserWnd != IntPtr.Zero)
			{
				MoveWindow(this.BrowserWnd, 0, 0, this.Width, this.Height, true);
			}
		}

		/// <summary>
		/// ハートビート用
		/// </summary>
		public IntPtr HWND => this.Handle;

		protected override string GetPersistString()
		{
			return "Browser";
		}


		#region 呪文

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);


		#endregion

	}


	/// <summary>
	/// 別プロセスのウィンドウにフォーカスがあるとキーボードショートカットが効かなくなるため、
	/// キー関連のメッセージのコピーを別のウィンドウに送る
	/// </summary>
	internal class KeyMessageGrabber : IMessageFilter
	{
		private IntPtr TargetWnd;

		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYDOWN = 0x0104;
		private const int WM_SYSKEYUP = 0x0105;

		public KeyMessageGrabber(IntPtr targetWnd)
		{
            this.TargetWnd = targetWnd;
		}

		public bool PreFilterMessage(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_KEYDOWN:
				case WM_KEYUP:
				case WM_SYSKEYDOWN:
				case WM_SYSKEYUP:
					PostMessage(this.TargetWnd, m.Msg, m.WParam, m.LParam);
					break;
			}
			return false;
		}
	}

}
