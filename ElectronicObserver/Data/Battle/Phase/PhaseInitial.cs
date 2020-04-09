using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 戦闘開始フェーズの処理を行います。
	/// </summary>
	public class PhaseInitial : PhaseBase
	{

		/// <summary>
		/// 自軍艦隊ID
		/// </summary>
		public int FriendFleetID { get; private set; }

		/// <summary>
		/// 自軍艦隊
		/// </summary>
		public FleetData FriendFleet => KCDatabase.Instance.Fleet[this.FriendFleetID];

		/// <summary>
		/// 自軍随伴艦隊
		/// </summary>
		public FleetData FriendFleetEscort => this.IsFriendCombined ? KCDatabase.Instance.Fleet[2] : null;


		/// <summary>
		/// 敵艦隊メンバ
		/// </summary>
		public int[] EnemyMembers { get; private set; }

		/// <summary>
		/// 敵艦隊メンバ
		/// </summary>
		public ShipDataMaster[] EnemyMembersInstance { get; private set; }


		/// <summary>
		/// 敵艦隊メンバ(随伴艦隊)
		/// </summary>
		public int[] EnemyMembersEscort { get; private set; }

		/// <summary>
		/// 敵艦隊メンバ(随伴艦隊)
		/// </summary>
		public ShipDataMaster[] EnemyMembersEscortInstance { get; private set; }


		/// <summary>
		/// 敵艦のレベル
		/// </summary>
		public int[] EnemyLevels { get; private set; }

		/// <summary>
		/// 敵艦のレベル(随伴艦隊)
		/// </summary>
		public int[] EnemyLevelsEscort { get; private set; }


		public int[] FriendInitialHPs { get; private set; }
		public int[] FriendInitialHPsEscort { get; private set; }
		public int[] EnemyInitialHPs { get; private set; }
		public int[] EnemyInitialHPsEscort { get; private set; }

		public int[] FriendMaxHPs { get; private set; }
		public int[] FriendMaxHPsEscort { get; private set; }
		public int[] EnemyMaxHPs { get; private set; }
		public int[] EnemyMaxHPsEscort { get; private set; }



		/// <summary>
		/// 敵艦のスロット
		/// </summary>
		public int[][] EnemySlots { get; private set; }

		/// <summary>
		/// 敵艦のスロット
		/// </summary>
		public EquipmentDataMaster[][] EnemySlotsInstance { get; private set; }


		/// <summary>
		/// 敵艦のスロット(随伴艦隊)
		/// </summary>
		public int[][] EnemySlotsEscort { get; private set; }

		/// <summary>
		/// 敵艦のスロット(随伴艦隊)
		/// </summary>
		public EquipmentDataMaster[][] EnemySlotsEscortInstance { get; private set; }


		/// <summary>
		/// 敵艦のパラメータ
		/// </summary>
		public int[][] EnemyParameters { get; private set; }

		/// <summary>
		/// 敵艦のパラメータ(随伴艦隊)
		/// </summary>
		public int[][] EnemyParametersEscort { get; private set; }


		/// <summary>
		/// 装甲破壊されているか
		/// </summary>
		public bool IsBossDamaged => this.RawData.api_xal01() && (int)this.RawData.api_xal01 > 0;


		/// <summary>
		/// 戦闘糧食を食べた艦娘のインデックス [0-11]
		/// </summary>
		public int[] RationIndexes { get; private set; }


		public bool IsFriendCombined => this.FriendInitialHPsEscort != null;
		public bool IsEnemyCombined => this.EnemyInitialHPsEscort != null;



		public PhaseInitial(BattleData data, string title)
			: base(data, title)
		{
			{
				dynamic id = this.RawData.api_dock_id() ? this.RawData.api_dock_id :
                    this.RawData.api_deck_id() ? this.RawData.api_deck_id : 1;
                this.FriendFleetID = id is string ? int.Parse((string)id) : (int)id;
			}
			if (this.FriendFleetID <= 0)
                this.FriendFleetID = 1;


			int[] GetArrayOrDefault(string objectName, int length) => !this.RawData.IsDefined(objectName) ? null : FixedArray((int[])this.RawData[objectName], length);
			int[][] GetArraysOrDefault(string objectName, int topLength, int bottomLength)
			{
				if (!this.RawData.IsDefined(objectName))
					return null;

				int[][] ret = new int[topLength][];
				dynamic[] raw = (dynamic[])this.RawData[objectName];
				for (int i = 0; i < ret.Length; i++)
				{
					if (i < raw.Length)
						ret[i] = FixedArray((int[])raw[i], bottomLength);
					else
						ret[i] = Enumerable.Repeat(-1, bottomLength).ToArray();
				}
				return ret;
			}

			int mainMemberCount = 7;
			int escortMemberCount = 6;

            this.EnemyMembers = GetArrayOrDefault("api_ship_ke", mainMemberCount);
            this.EnemyMembersInstance = this.EnemyMembers.Select(id => KCDatabase.Instance.MasterShips[id]).ToArray();

            this.EnemyMembersEscort = GetArrayOrDefault("api_ship_ke_combined", escortMemberCount);
            this.EnemyMembersEscortInstance = this.EnemyMembersEscort?.Select(id => KCDatabase.Instance.MasterShips[id]).ToArray();

            this.EnemyLevels = GetArrayOrDefault("api_ship_lv", mainMemberCount);
            this.EnemyLevelsEscort = GetArrayOrDefault("api_ship_lv_combined", escortMemberCount);

            this.FriendInitialHPs = GetArrayOrDefault("api_f_nowhps", mainMemberCount);
            this.FriendInitialHPsEscort = GetArrayOrDefault("api_f_nowhps_combined", escortMemberCount);
            this.EnemyInitialHPs = GetArrayOrDefault("api_e_nowhps", mainMemberCount);
            this.EnemyInitialHPsEscort = GetArrayOrDefault("api_e_nowhps_combined", escortMemberCount);

            this.FriendMaxHPs = GetArrayOrDefault("api_f_maxhps", mainMemberCount);
            this.FriendMaxHPsEscort = GetArrayOrDefault("api_f_maxhps_combined", escortMemberCount);
            this.EnemyMaxHPs = GetArrayOrDefault("api_e_maxhps", mainMemberCount);
            this.EnemyMaxHPsEscort = GetArrayOrDefault("api_e_maxhps_combined",escortMemberCount);


            this.EnemySlots = GetArraysOrDefault("api_eSlot", mainMemberCount, 5);
            this.EnemySlotsInstance = this.EnemySlots.Select(part => part.Select(id => KCDatabase.Instance.MasterEquipments[id]).ToArray()).ToArray();

            this.EnemySlotsEscort = GetArraysOrDefault("api_eSlot_combined", escortMemberCount, 5);
            this.EnemySlotsEscortInstance = this.EnemySlotsEscort?.Select(part => part.Select(id => KCDatabase.Instance.MasterEquipments[id]).ToArray()).ToArray();

            this.EnemyParameters = GetArraysOrDefault("api_eParam", mainMemberCount, 4);
            this.EnemyParametersEscort = GetArraysOrDefault("api_eParam_combined", escortMemberCount, 4);

			{
				var rations = new List<int>();
				if (this.RawData.api_combat_ration())
				{
					rations.AddRange(((int[])this.RawData.api_combat_ration).Select(i => this.FriendFleet.Members.IndexOf(i)));
				}
				if (this.RawData.api_combat_ration_combined())
				{
					rations.AddRange(((int[])this.RawData.api_combat_ration_combined).Select(i => this.FriendFleetEscort.Members.IndexOf(i) + 6));
				}
                this.RationIndexes = rations.ToArray();
			}
		}



		public ShipData GetFriendShip(int index)
		{
			if (index < 0 || index >= 12)
				return null;

			if (index < this.FriendFleet.Members.Count)
				return this.FriendFleet.MembersInstance[index];
			else if (index >= 6 && this.FriendFleetEscort != null)
				return this.FriendFleetEscort.MembersInstance[index - 6];
			else
				return null;
		}

		protected static int[] FixedArray(int[] array, int length, int defaultValue = -1)
		{
			var ret = new int[length];
			int l = Math.Min(length, array.Length);
			Array.Copy(array, ret, l);
			if (l < length)
			{
				for (int i = l; i < length; i++)
					ret[i] = defaultValue;
			}

			return ret;
		}



		public override bool IsAvailable => this.RawData != null;

		public override void EmulateBattle(int[] hps, int[] damages)
		{
		}

	}
}
