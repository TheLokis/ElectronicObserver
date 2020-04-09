using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{
	/// <summary>
	/// 夜戦開始フェーズの処理を行います。
	/// </summary>
	public class PhaseNightInitial : PhaseBase
	{

		private readonly bool IsEscort;

		public PhaseNightInitial(BattleData battle, string title, bool isEscort)
			: base(battle, title)
		{
            this.IsEscort = isEscort;
		}

		public override bool IsAvailable => this.RawData != null;

		public override void EmulateBattle(int[] hps, int[] damages)
		{
			// nop
		}



		/// <summary>
		/// 戦闘する自軍艦隊
		/// 1=主力艦隊, 2=随伴艦隊
		/// </summary>
		public int ActiveFriendFleet => !this.RawData.api_active_deck() ? 1 : (int)this.RawData.api_active_deck[0];

		/// <summary>
		/// 自軍艦隊ID
		/// </summary>
		public int FriendFleetID
		{
			get
			{
				if (this.IsFriendEscort)
					return 2;
				else
					return this.Battle.Initial.FriendFleetID;
			}
		}

		/// <summary>
		/// 自軍艦隊
		/// </summary>
		public FleetData FriendFleet => KCDatabase.Instance.Fleet[this.FriendFleetID];

		/// <summary>
		/// 自軍が随伴艦隊かどうか
		/// </summary>
		public bool IsFriendEscort => this.IsEscort || this.ActiveFriendFleet != 1;


		/// <summary>
		/// 敵軍艦隊ID
		/// </summary>
		public int EnemyFleetID => !this.RawData.api_active_deck() ? 1 : (int)this.RawData.api_active_deck[1];

		/// <summary>
		/// 敵軍艦隊
		/// </summary>
		public int[] EnemyMembers => !this.IsEnemyEscort ? this.Battle.Initial.EnemyMembers : this.Battle.Initial.EnemyMembersEscort;

		/// <summary>
		/// 敵軍艦隊
		/// </summary>
		public ShipDataMaster[] EnemyMembersInstance => !this.IsEnemyEscort ? this.Battle.Initial.EnemyMembersInstance : this.Battle.Initial.EnemyMembersEscortInstance;

		/// <summary>
		/// 敵軍が随伴艦隊かどうか
		/// </summary>
		public bool IsEnemyEscort => this.EnemyFleetID != 1;


		/// <summary>
		/// 自軍触接機ID
		/// </summary>
		public int TouchAircraftFriend => (this.RawData.api_touch_plane[0] is string) ? int.Parse(this.RawData.api_touch_plane[0]) : (int)this.RawData.api_touch_plane[0];


		/// <summary>
		/// 敵軍触接機ID
		/// </summary>
		public int TouchAircraftEnemy => (this.RawData.api_touch_plane[1] is string) ? int.Parse(this.RawData.api_touch_plane[1]) : (int)this.RawData.api_touch_plane[1];


		/// <summary>
		/// 自軍照明弾投射艦インデックス(0-11, -1=発動せず)
		/// </summary>
		public int FlareIndexFriend => (int)this.RawData.api_flare_pos[0];

		/// <summary>
		/// 敵軍照明弾投射艦インデックス(0-11, -1=発動せず)
		/// </summary>
		public int FlareIndexEnemy => (int)this.RawData.api_flare_pos[1];


		/// <summary>
		/// 自軍照明弾投射艦
		/// </summary>
		public ShipData FlareFriendInstance
		{
			get
			{
				int index = this.FlareIndexFriend;

				if (index < 0)
					return null;

				if (this.IsFriendEscort)
					return this.FriendFleet.MembersInstance[index - 6];
				else
					return this.FriendFleet.MembersInstance[index];

			}
		}

		/// <summary>
		/// 敵軍照明弾投射艦
		/// </summary>
		public ShipDataMaster FlareEnemyInstance
		{
			get
			{
				int index = this.FlareIndexEnemy;

				if (index < 0)
					return null;

				if (this.IsEnemyEscort)
					return this.EnemyMembersInstance[index - 6];
				else
					return this.EnemyMembersInstance[index];
			}
		}


		/// <summary>
		/// 自軍探照灯照射艦番号
		/// </summary>
		public int SearchlightIndexFriend
		{
			get
			{
				var ships = this.FriendFleet.MembersWithoutEscaped;
				var hps = this.IsFriendEscort ? this.Battle.Initial.FriendInitialHPsEscort : this.Battle.Initial.FriendInitialHPs;
				int index = -1;

				for (int i = 0; i < ships.Count; i++)
				{
					var ship = ships[i];
					if (ship != null && hps[i] > 1)
					{

						if (ship.SlotInstanceMaster.Any(e => e?.CategoryType == EquipmentTypes.SearchlightLarge))
							return i;
						else if (ship.SlotInstanceMaster.Any(e => e?.CategoryType == EquipmentTypes.Searchlight) && index == -1)
							index = i;
					}
				}

				return index;
			}
		}

		/// <summary>
		/// 敵軍探照灯照射艦番号(0-5)
		/// </summary>
		public int SearchlightIndexEnemy
		{
			get
			{
				var ships = this.EnemyMembersInstance;
				var eqs = this.Battle.Initial.EnemySlotsInstance;
				var hps = this.IsEnemyEscort ? this.Battle.Initial.EnemyInitialHPsEscort : this.Battle.Initial.EnemyInitialHPs;
				int index = -1;

				for (int i = 0; i < ships.Length; i++)
				{
					if (ships[i] != null && hps[i] > 1)
					{

						if (eqs[i].Any(e => e?.CategoryType == EquipmentTypes.SearchlightLarge))
							return i;
						else if (eqs[i].Any(e => e?.CategoryType == EquipmentTypes.Searchlight) && index == -1)
							index = i;

					}
				}

				return index;
			}
		}

		/// <summary>
		/// 敵軍探照灯照射艦
		/// </summary>
		public ShipDataMaster SearchlightEnemyInstance
		{
			get
			{
				int index = this.SearchlightIndexEnemy;
				return index == -1 ? null : this.EnemyMembersInstance[index];
			}
		}


	}
}
