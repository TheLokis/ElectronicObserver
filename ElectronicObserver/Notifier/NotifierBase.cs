using ElectronicObserver.Utility;
using ElectronicObserver.Window;
using ElectronicObserver.Window.Dialog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	/// <summary>
	/// 通知を扱います。
	/// </summary>
	public abstract class NotifierBase
	{

		/// <summary>
		/// 通知ダイアログに渡す設定データ
		/// </summary>
		public NotifierDialogData DialogData { get; protected set; }

		/// <summary>
		/// 有効かどうか
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// ミュート状態かどうか
		/// </summary>
		public bool IsSilenced { get; set; }


		/// <summary>
		/// 通知音
		/// </summary>
		public MediaPlayer Sound { get; protected set; }

		/// <summary>
		/// 通知音のパス
		/// </summary>
		public string SoundPath { get; set; }

		/// <summary>
		/// 通知音を再生するか
		/// </summary>
		public bool PlaysSound { get; set; }


		private bool _loopsSound;
		/// <summary>
		/// 通知音をループさせるか
		/// </summary>
		public bool LoopsSound
		{
			get { return this._loopsSound; }
			set
			{
                this._loopsSound = value;
                this.SetIsLoop();
			}
		}

		private int _soundVolume;
		/// <summary>
		/// 通知音の音量 (0-100)
		/// </summary>
		public int SoundVolume
		{
			get { return this._soundVolume; }
			set
			{
                this._soundVolume = value;
				if (!Utility.Configuration.Config.Control.UseSystemVolume)
                    this.Sound.Volume = this._soundVolume;
			}
		}

		private bool _showsDialog;
		/// <summary>
		/// 通知ダイアログを表示するか
		/// </summary>
		public bool ShowsDialog
		{
			get { return this._showsDialog; }
			set
			{
                this._showsDialog = value;
                this.SetIsLoop();
			}
		}

		private void SetIsLoop()
		{
            this.Sound.IsLoop = this.LoopsSound && this.ShowsDialog;
		}


		/// <summary>
		/// 通知を早める時間(ミリ秒)
		/// </summary>
		public int AccelInterval { get; set; }




		public NotifierBase()
		{

            this.Initialize();
            this.DialogData = new NotifierDialogData();

		}

		public NotifierBase(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
		{

            this.Initialize();
            this.DialogData = new NotifierDialogData(config);
            if (config.PlaysSound && !string.IsNullOrEmpty(config.SoundPath))
                this.LoadSound(config.SoundPath);

            this.IsEnabled = config.IsEnabled;
            this.IsSilenced = config.IsSilenced;
            this.PlaysSound = config.PlaysSound;
            this.SoundVolume = config.SoundVolume;
            this.LoopsSound = config.LoopsSound;
            this.ShowsDialog = config.ShowsDialog;
            this.AccelInterval = config.AccelInterval;

		}

		private void Initialize()
		{

			SystemEvents.UpdateTimerTick += this.UpdateTimerTick;
            this.Sound = new MediaPlayer
			{
				IsShuffle = true
			};
            this.Sound.MediaEnded += this.Sound_MediaEnded;
            this.SoundPath = "";

		}


		public void SetInitialVolume(int volume)
		{
            this.Sound.Volume = volume;
		}


		protected virtual void UpdateTimerTick() { }


		#region 通知音

		/// <summary>
		/// 通知音を読み込みます。
		/// </summary>
		/// <param name="path">音声ファイルへのパス。</param>
		/// <returns>成功すれば true 、失敗すれば false を返します。</returns>
		public bool LoadSound(string path)
		{
			try
			{

                this.DisposeSound();

				if (File.Exists(path))
				{
                    this.Sound.SetPlaylist(null);
                    this.Sound.SourcePath = path;

				}
				else if (Directory.Exists(path))
				{
                    this.Sound.SetPlaylistFromDirectory(path);

				}
				else
				{
					throw new FileNotFoundException("지정된 파일 또는 디렉토리를 찾을 수 없습니다.");
				}

                this.SoundPath = path;

				return true;

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, string.Format("알림 시스템 : 알림 사운드 {0}의 로드에 실패했습니다.", path));
                this.DisposeSound();

			}

			return false;
		}

		/// <summary>
		/// 通知音を再生します。
		/// </summary>
		public void PlaySound()
		{
			try
			{

				if (this.Sound != null && this.PlaysSound)
				{
					if (this.Sound.PlayState == 3)
					{       //playing
						if (this.Sound.GetPlaylist().Any())
                            this.Sound.Next();

                        this.Sound.Stop();
					}

                    //音量の再設定(システム側の音量変更によって設定が変わることがあるので)
                    this.SoundVolume = this._soundVolume;

                    this.Sound.Play();
				}

			}
			catch (Exception ex)
			{

				Utility.Logger.Add(3, "알림 시스템 : 알림음 재생에 실패했습니다." + ex.Message);
			}
		}

		/// <summary>
		/// 通知音を破棄します。
		/// </summary>
		public void DisposeSound()
		{
            this.Sound.Close();
            this.Sound.SourcePath = this.SoundPath = "";
		}


		void Sound_MediaEnded()
		{
			if (this.Sound.GetPlaylist().Any() && !this.LoopsSound)
                this.Sound.Next();
		}


		#endregion



		/// <summary>
		/// 通知ダイアログを表示します。
		/// </summary>
		public void ShowDialog(System.Windows.Forms.FormClosingEventHandler customClosingHandler = null)
		{

			if (this.ShowsDialog)
			{
				var dialog = new DialogNotifier(this.DialogData);
				dialog.FormClosing += this.dialog_FormClosing;
				if (customClosingHandler != null)
				{
					dialog.FormClosing += customClosingHandler;
				}
				NotifierManager.Instance.ShowNotifier(dialog);
			}
		}

		void dialog_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			if (this.LoopsSound)
			{
                this.Sound.Stop();
                this.Sound.Next();
			}
		}

		/// <summary>
		/// 通知を行います。
		/// </summary>
		public virtual void Notify()
		{
            this.Notify(null);
		}

		/// <summary>
		/// 終了時のイベントハンドラを指定して通知を行います。
		/// </summary>
		public virtual void Notify(System.Windows.Forms.FormClosingEventHandler customClosingHandler)
		{

			if (!this.IsEnabled || this.IsSilenced) return;

            this.ShowDialog(customClosingHandler);
            this.PlaySound();

		}


		public virtual void ApplyToConfiguration(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
		{

            this.DialogData.ApplyToConfiguration(config);
			config.PlaysSound = this.PlaysSound;
			config.SoundPath = this.SoundPath;
			config.SoundVolume = this.SoundVolume;
			config.LoopsSound = this.LoopsSound;
			config.IsEnabled = this.IsEnabled;
			config.IsSilenced = this.IsSilenced;
			config.ShowsDialog = this.ShowsDialog;
			config.AccelInterval = this.AccelInterval;

		}

	}
}
