using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Utility
{

	public sealed class SyncBGMPlayer
	{

		#region Singleton

		private static readonly SyncBGMPlayer instance = new SyncBGMPlayer();

		public static SyncBGMPlayer Instance => instance;

		#endregion


		[DataContract(Name = "SoundHandle")]
		public class SoundHandle : IIdentifiable, ICloneable
		{

			[DataMember]
			public SoundHandleID HandleID { get; set; }

			[DataMember]
			public bool Enabled { get; set; }

			[DataMember]
			public string Path { get; set; }

			[DataMember]
			public bool IsLoop { get; set; }

			[DataMember]
			public double LoopHeadPosition { get; set; }

			[DataMember]
			public int Volume { get; set; }

			public SoundHandle(SoundHandleID id)
			{
                this.HandleID = id;
                this.Enabled = true;
                this.Path = "";
                this.IsLoop = true;
                this.LoopHeadPosition = 0.0;
                this.Volume = 100;
			}

			[IgnoreDataMember]
			public int ID => (int)this.HandleID;

			public override string ToString() => Enum.GetName(typeof(SoundHandleID), this.HandleID) + " : " + this.Path;


			public SoundHandle Clone()
			{
				return (SoundHandle)this.MemberwiseClone();
			}

			object ICloneable.Clone()
			{
				return this.Clone();
			}
		}

		public enum SoundHandleID
		{
			Port = 1,
			Sortie = 101,
			BattleDay = 201,
			BattleNight,
			BattleAir,
			BattleBoss,
			BattlePracticeDay,
			BattlePracticeNight,
			ResultWin = 301,
			ResultLose,
			ResultBossWin,
			Record = 401,
			Item,
			Quest,
			Album,
			ImprovementArsenal,
		}

		public IDDictionary<SoundHandle> Handles { get; internal set; }
		public bool Enabled;
		public bool IsMute
		{
			get { return this._mp.IsMute; }
			set { this._mp.IsMute = value; }
		}

		private MediaPlayer _mp;
		private SoundHandleID _currentSoundHandleID;
		private bool _isBoss;


		public SyncBGMPlayer()
		{

            this._mp = new MediaPlayer();

			if (!this._mp.IsAvailable)
				Utility.Logger.Add(3, "Windows Media Player 의 로드에 실패했습니다. 음성 재생이 되지 않습니다.");

            this._mp.AutoPlay = false;
            this._mp.IsShuffle = true;

            this._currentSoundHandleID = (SoundHandleID)(-1);
            this._isBoss = false;


            this.Enabled = false;
            this.Handles = new IDDictionary<SoundHandle>();

			foreach (SoundHandleID id in Enum.GetValues(typeof(SoundHandleID)))
                this.Handles.Add(new SoundHandle(id));



			#region API register
			APIObserver o = APIObserver.Instance;

			o["api_port/port"].ResponseReceived += this.PlayPort;

			o["api_req_map/start"].ResponseReceived += this.PlaySortie;
			o["api_req_map/next"].ResponseReceived += this.PlaySortie;

			o["api_req_sortie/battle"].ResponseReceived += this.PlayBattleDay;
			o["api_req_combined_battle/battle"].ResponseReceived += this.PlayBattleDay;
			o["api_req_combined_battle/battle_water"].ResponseReceived += this.PlayBattleDay;
			o["api_req_combined_battle/ec_battle"].ResponseReceived += this.PlayBattleDay;
			o["api_req_combined_battle/each_battle"].ResponseReceived += this.PlayBattleDay;
			o["api_req_combined_battle/each_battle_water"].ResponseReceived += this.PlayBattleDay;

			o["api_req_battle_midnight/battle"].ResponseReceived += this.PlayBattleNight;
			o["api_req_battle_midnight/sp_midnight"].ResponseReceived += this.PlayBattleNight;
			o["api_req_sortie/night_to_day"].ResponseReceived += this.PlayBattleNight;
            o["api_req_sortie/ld_shooting"].ResponseReceived += this.PlayBattleNight;

            o["api_req_combined_battle/midnight_battle"].ResponseReceived += this.PlayBattleNight;
			o["api_req_combined_battle/sp_midnight"].ResponseReceived += this.PlayBattleNight;
			o["api_req_combined_battle/ec_midnight_battle"].ResponseReceived += this.PlayBattleNight;
			o["api_req_combined_battle/ec_night_to_day"].ResponseReceived += this.PlayBattleNight;
            o["api_req_combined_battle/ld_shooting"].ResponseReceived += this.PlayBattleNight;

            o["api_req_sortie/airbattle"].ResponseReceived += this.PlayBattleAir;
			o["api_req_combined_battle/airbattle"].ResponseReceived += this.PlayBattleAir;
			o["api_req_sortie/ld_airbattle"].ResponseReceived += this.PlayBattleAir;
			o["api_req_combined_battle/ld_airbattle"].ResponseReceived += this.PlayBattleAir;

			o["api_req_practice/battle"].ResponseReceived += this.PlayPracticeDay;

			o["api_req_practice/midnight_battle"].ResponseReceived += this.PlayPracticeNight;

			o["api_req_sortie/battleresult"].ResponseReceived += this.PlayBattleResult;
			o["api_req_combined_battle/battleresult"].ResponseReceived += this.PlayBattleResult;
			o["api_req_practice/battle_result"].ResponseReceived += this.PlayBattleResult;

			o["api_get_member/record"].ResponseReceived += this.PlayRecord;

			o["api_get_member/payitem"].ResponseReceived += this.PlayItem;

			o["api_get_member/questlist"].ResponseReceived += this.PlayQuest;

			o["api_get_member/picture_book"].ResponseReceived += this.PlayAlbum;

			o["api_req_kousyou/remodel_slotlist"].ResponseReceived += this.PlayImprovementArsenal;

			#endregion

			Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
			SystemEvents.SystemShuttingDown += this.SystemEvents_SystemShuttingDown;
		}

		public void ConfigurationChanged()
		{
			var c = Utility.Configuration.Config.BGMPlayer;

            this.Enabled = c.Enabled;

			if (c.Handles != null)
                this.Handles = new IDDictionary<SoundHandle>(c.Handles);

			if (!c.SyncBrowserMute)
                this.IsMute = false;

            // 設定変更を適用するためいったん閉じる
            this._mp.Close();
            this._currentSoundHandleID = (SoundHandleID)(-1);
		}

		void SystemEvents_SystemShuttingDown()
		{
			var c = Utility.Configuration.Config.BGMPlayer;

			c.Enabled = this.Enabled;
			c.Handles = this.Handles.Values.ToList();
		}


		public void SetInitialVolume(int volume)
		{
            this._mp.Volume = volume;
		}



		void PlayPort(string apiname, dynamic data)
		{
            this._isBoss = false;
            this.Play(this.Handles[(int)SoundHandleID.Port]);
		}

		void PlaySortie(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.Sortie]);
            this._isBoss = (int)data.api_event_id == 5;
		}

		void PlayBattleDay(string apiname, dynamic data)
		{
			if (this._isBoss)
                this.Play(this.Handles[(int)SoundHandleID.BattleBoss]);
			else
                this.Play(this.Handles[(int)SoundHandleID.BattleDay]);
		}

		void PlayBattleNight(string apiname, dynamic data)
		{
			if (this._isBoss)
                this.Play(this.Handles[(int)SoundHandleID.BattleBoss]);
			else
                this.Play(this.Handles[(int)SoundHandleID.BattleNight]);
		}

		void PlayBattleAir(string apiname, dynamic data)
		{
			if (this._isBoss)
                this.Play(this.Handles[(int)SoundHandleID.BattleBoss]);
			else
                this.Play(this.Handles[(int)SoundHandleID.BattleAir]);
		}

		void PlayPracticeDay(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.BattlePracticeDay]);
		}

		void PlayPracticeNight(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.BattlePracticeNight]);

		}

		void PlayBattleResult(string apiname, dynamic data)
		{
			switch ((string)data.api_win_rank)
			{
				case "S":
				case "A":
				case "B":
					if (this._isBoss)
                        this.Play(this.Handles[(int)SoundHandleID.ResultBossWin]);
					else
                        this.Play(this.Handles[(int)SoundHandleID.ResultWin]);
					break;
				default:
                    this.Play(this.Handles[(int)SoundHandleID.ResultLose]);
					break;
			}
		}


		void PlayRecord(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.Record]);
		}

		void PlayItem(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.Item]);
		}

		void PlayQuest(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.Quest]);
		}

		void PlayAlbum(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.Album]);
		}

		void PlayImprovementArsenal(string apiname, dynamic data)
		{
            this.Play(this.Handles[(int)SoundHandleID.ImprovementArsenal]);
		}


		private bool Play(SoundHandle sh)
		{
			if (this.Enabled &&
				sh != null &&
				sh.Enabled &&
				!string.IsNullOrWhiteSpace(sh.Path) &&
				sh.HandleID != this._currentSoundHandleID)
			{


				if (File.Exists(sh.Path))
				{
                    this._mp.Close();
                    this._mp.SetPlaylist(null);
                    this._mp.SourcePath = sh.Path;

				}
				else if (Directory.Exists(sh.Path))
				{
                    this._mp.Close();
                    this._mp.SetPlaylistFromDirectory(sh.Path);

				}
				else
				{
					return false;
				}

                this._currentSoundHandleID = sh.HandleID;

                this._mp.IsLoop = sh.IsLoop;
                this._mp.LoopHeadPosition = sh.LoopHeadPosition;
				if (!Utility.Configuration.Config.Control.UseSystemVolume)
                    this._mp.Volume = sh.Volume;
                this._mp.Play();

				return true;
			}

			return false;
		}



		public static string SoundHandleIDToString(SoundHandleID id)
		{
			switch (id)
			{
				case SoundHandleID.Port:
					return "모항";
				case SoundHandleID.Sortie:
					return "출격중";
				case SoundHandleID.BattleDay:
					return "주간전";
				case SoundHandleID.BattleNight:
					return "야간전";
				case SoundHandleID.BattleAir:
					return "항공전";
				case SoundHandleID.BattleBoss:
					return "보스전";
				case SoundHandleID.BattlePracticeDay:
					return "연전주간";
				case SoundHandleID.BattlePracticeNight:
					return "연전야간";
				case SoundHandleID.ResultWin:
					return "승리";
				case SoundHandleID.ResultLose:
					return "패배";
				case SoundHandleID.ResultBossWin:
					return "보스승리";
				case SoundHandleID.Record:
					return "전적";
				case SoundHandleID.Item:
					return "아이템";
				case SoundHandleID.Quest:
					return "임무";
				case SoundHandleID.Album:
					return "도감";
				case SoundHandleID.ImprovementArsenal:
					return "개수공창";
				default:
					return "불명";
			}
		}
	}
}
