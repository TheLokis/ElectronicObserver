using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ElectronicObserver.Utility
{

	/// <summary>
	/// Windows Media Player コントロールを利用して、音楽を再生するためのクラスです。
	/// </summary>
	public class MediaPlayer
	{

		private dynamic _wmp;

		public event Action<int> PlayStateChange = delegate { };
		public event Action MediaEnded = delegate { };

		private List<string> _playlist;
		private List<string> _realPlaylist;

		private Random _rand;


		/// <summary>
		/// 対応している拡張子リスト
		/// </summary>
		public static readonly ReadOnlyCollection<string> SupportedExtensions =
			new ReadOnlyCollection<string>(new List<string>() {
			"asf",
			"wma",
			"mp2",
			"mp3",
			"mid",
			"midi",
			"rmi",
			"aif",
			"aifc",
			"aiff",
			"au",
			"snd",
			"wav",
			"m4a",
			"aac",
			"flac",
			"mka",
		});

		private static readonly Regex SupportedFileName = new Regex(".*\\.(" + string.Join("|", SupportedExtensions) + ")", RegexOptions.Compiled);


		public MediaPlayer()
		{
			try
			{
				var type = Type.GetTypeFromProgID("WMPlayer.OCX.7");
				if (type != null)
				{
                    this._wmp = Activator.CreateInstance(type);
                    this._wmp.uiMode = "none";
                    this._wmp.settings.autoStart = false;
                    this._wmp.PlayStateChange += new Action<int>(this.wmp_PlayStateChange);
				}
				else
				{
                    this._wmp = null;
				}
			}
			catch
			{
                this._wmp = null;
			}

            this.IsLoop = false;
            this._isShuffle = false;
            this.IsMute = false;
            this.LoopHeadPosition = 0.0;
            this.AutoPlay = false;
            this._playlist = new List<string>();
            this._realPlaylist = new List<string>();
            this._rand = new Random();

			MediaEnded += this.MediaPlayer_MediaEnded;
		}


		/// <summary>
		/// 利用可能かどうか
		/// false の場合全機能が使用不可能
		/// </summary>
		public bool IsAvailable => this._wmp != null;

		/// <summary>
		/// メディアファイルのパス。
		/// 再生中に変更された場合停止します。
		/// </summary>
		public string SourcePath
		{
			get { return !this.IsAvailable ? string.Empty : this._wmp.URL; }
			set
			{
				if (this.IsAvailable && this._wmp.URL != value)
                    this._wmp.URL = value;
			}
		}

		/// <summary>
		/// 音量
		/// 0-100
		/// 注: システムの音量設定と連動しているようなので注意が必要
		/// </summary>
		public int Volume
		{
			get { return !this.IsAvailable ? 0 : this._wmp.settings.volume; }
			set { if (this.IsAvailable) this._wmp.settings.volume = value; }
		}

		/// <summary>
		/// ミュート
		/// </summary>
		public bool IsMute
		{
			get { return !this.IsAvailable ? false : this._wmp.settings.mute; }
			set { if (this.IsAvailable) this._wmp.settings.mute = value; }
		}


		private bool _isLoop;
		/// <summary>
		/// ループするか
		/// </summary>
		public bool IsLoop
		{
			get { return this._isLoop; }
			set
			{
                this._isLoop = value;
				if (this.IsAvailable)
                    this._wmp.settings.setMode("loop", this._isLoop);
			}
		}

		/// <summary>
		/// ループ時の先頭 (秒単位)
		/// </summary>
		public double LoopHeadPosition { get; set; }


		/// <summary>
		/// 現在の再生地点 (秒単位)
		/// </summary>
		public double CurrentPosition
		{
			get { return !this.IsAvailable ? 0.0 : this._wmp.controls.currentPosition; }
			set { if (this.IsAvailable) this._wmp.controls.currentPosition = value; }
		}

		/// <summary>
		/// 再生状態
		/// </summary>
		public int PlayState => !this.IsAvailable ? 0 : this._wmp.playState;

		/// <summary>
		/// 現在のメディアの名前
		/// </summary>
		public string MediaName => !this.IsAvailable ? string.Empty : this._wmp.currentMedia?.name;

		/// <summary>
		/// 現在のメディアの長さ(秒単位)
		/// なければ 0
		/// </summary>
		public double Duration => !this.IsAvailable ? 0.0 : this._wmp.currentMedia?.duration ?? 0;



		/// <summary>
		/// プレイリストのコピーを取得します。
		/// </summary>
		/// <returns></returns>
		public List<string> GetPlaylist()
		{
			return new List<string>(this._playlist);
		}

		/// <summary>
		/// プレイリストを設定します。
		/// </summary>
		/// <param name="list"></param>
		public void SetPlaylist(IEnumerable<string> list)
		{
			if (list == null)
                this._playlist = new List<string>();
			else
                this._playlist = list.Distinct().ToList();

            this.UpdateRealPlaylist();
		}


		public IEnumerable<string> SearchSupportedFiles(string path, System.IO.SearchOption option = System.IO.SearchOption.TopDirectoryOnly)
		{
			return System.IO.Directory.EnumerateFiles(path, "*", option).Where(s => SupportedFileName.IsMatch(s));
		}

		/// <summary>
		/// フォルダを検索し、音楽ファイルをプレイリストに設定します。
		/// </summary>
		/// <param name="path">フォルダへのパス。</param>
		/// <param name="option">検索オプション。既定ではサブディレクトリは検索されません。</param>
		public void SetPlaylistFromDirectory(string path, System.IO.SearchOption option = System.IO.SearchOption.TopDirectoryOnly)
		{
            this.SetPlaylist(this.SearchSupportedFiles(path, option));
		}



		private int _playingIndex;
		/// <summary>
		/// 現在再生中の曲のプレイリスト中インデックス
		/// </summary>
		private int PlayingIndex
		{
			get { return this._playingIndex; }
			set
			{
				if (this._playingIndex != value)
				{

					if (value < 0 || this._realPlaylist.Count <= value)
						return;

                    this._playingIndex = value;
                    this.SourcePath = this._realPlaylist[this._playingIndex];
					if (this.AutoPlay)
                        this.Play();
				}
			}
		}

		private bool _isShuffle;
		/// <summary>
		/// シャッフル再生するか
		/// </summary>
		public bool IsShuffle
		{
			get { return this._isShuffle; }
			set
			{
				bool changed = this._isShuffle != value;

                this._isShuffle = value;

				if (changed)
				{
                    this.UpdateRealPlaylist();
				}
			}
		}

		/// <summary>
		/// 曲が終了したとき自動で次の曲を再生するか
		/// </summary>
		public bool AutoPlay { get; set; }





		/// <summary>
		/// 再生
		/// </summary>
		public void Play()
		{
			if (!this.IsAvailable) return;

			if (this._realPlaylist.Count > 0 && this.SourcePath != this._realPlaylist[this._playingIndex])
                this.SourcePath = this._realPlaylist[this._playingIndex];

            this._wmp.controls.play();
		}

		/// <summary>
		/// ポーズ
		/// </summary>
		public void Pause()
		{
			if (!this.IsAvailable) return;

            this._wmp.controls.pause();
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void Stop()
		{
			if (!this.IsAvailable) return;

            this._wmp.controls.stop();
		}

		/// <summary>
		/// ファイルを閉じる
		/// </summary>
		public void Close()
		{
			if (!this.IsAvailable) return;

            this._wmp.close();
		}


		/// <summary>
		/// 次の曲へ
		/// </summary>
		public void Next()
		{
			if (!this.IsAvailable) return;

			int prevState = this.PlayState;

			if (this.PlayingIndex >= this._realPlaylist.Count - 1)
			{
				if (this.IsShuffle)
                    this.UpdateRealPlaylist();
                this.PlayingIndex = 0;
			}
			else
			{
                this.PlayingIndex++;
			}

			if (prevState == 3 || this.AutoPlay)     // Playing
                this.Play();
		}

		/// <summary>
		/// 前の曲へ
		/// </summary>
		public void Prev()
		{
			if (!this.IsAvailable) return;

			if (this.IsShuffle)
				return;

			int prevState = this.PlayState;

			if (this.PlayingIndex == 0)
                this.PlayingIndex = this._realPlaylist.Count - 1;
			else
                this.PlayingIndex--;

			if (prevState == 3 || this.AutoPlay)     // Playing
                this.Play();
		}

		private void UpdateRealPlaylist()
		{
			if (!this.IsAvailable) return;

			if (!this.IsShuffle)
			{
                this._realPlaylist = new List<string>(this._playlist);

			}
			else
			{
                // shuffle
                this._realPlaylist = this._playlist.OrderBy(s => Guid.NewGuid()).ToList();

				// 同じ曲が連続で流れるのを防ぐ
				if (this._realPlaylist.Count > 1 && this.SourcePath == this._realPlaylist[0])
				{
                    this._realPlaylist = this._realPlaylist.Skip(1).ToList();
                    this._realPlaylist.Insert(this._rand.Next(1, this._realPlaylist.Count + 1), this.SourcePath);
				}
			}

			int index = this._realPlaylist.IndexOf(this.SourcePath);
            this.PlayingIndex = index != -1 ? index : 0;
		}


		private bool _loopflag = false;
		void wmp_PlayStateChange(int NewState)
		{

			// ループ用処理
			if (this.IsLoop && this.LoopHeadPosition > 0.0)
			{
				switch (NewState)
				{
					case 8:     //MediaEnded
                        this._loopflag = true;
						break;

					case 3:     //playing
						if (this._loopflag)
						{
                            this.CurrentPosition = this.LoopHeadPosition;
                            this._loopflag = false;
						}
						break;
				}
			}

			if (NewState == 8)  //MediaEnded
                this.OnMediaEnded();

			PlayStateChange(NewState);
		}



		void MediaPlayer_MediaEnded()
		{
			// プレイリストの処理
			if (!this.IsLoop && this.AutoPlay)
                this.Next();
		}


		// 即時変化させるとイベント終了直後に書き換えられて next が無視されるので苦肉の策
		private async void OnMediaEnded()
		{
			await Task.Run(() => Task.WaitAll(Task.Delay(10)));
			MediaEnded();
		}
	}
}
