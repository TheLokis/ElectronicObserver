using ElectronicObserver.Data;
using ElectronicObserver.Data.Battle;
using ElectronicObserver.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	/// <summary>
	/// 大破進撃警告通知を扱います。
	/// </summary>
	public class NotifierDamage : NotifierBase
	{

		/// <summary>
		/// 事前通知(出撃前、戦闘結果判明直後)が有効かどうか
		/// </summary>
		public bool NotifiesBefore { get; set; }

		/// <summary>
		/// 事中通知(出撃前、戦績画面)が有効かどうか
		/// </summary>
		public bool NotifiesNow { get; set; }

		/// <summary>
		/// 事後通知(出撃直後、進撃中)が有効かどうか
		/// </summary>
		public bool NotifiesAfter { get; set; }

		/// <summary>
		/// 通知が有効な艦船のLv下限
		/// これよりLvが低い艦は除外されます
		/// </summary>
		public int LevelBorder { get; set; }

		/// <summary>
		/// 非ロック艦も含める
		/// </summary>
		public bool ContainsNotLockedShip { get; set; }

		/// <summary>
		/// ダメコン装備艦も含める
		/// </summary>
		public bool ContainsSafeShip { get; set; }

		/// <summary>
		/// 旗艦を含める
		/// </summary>
		public bool ContainsFlagship { get; set; }

		/// <summary>
		/// 終点でも通知する
		/// </summary>
		public bool NotifiesAtEndpoint { get; set; }


		public NotifierDamage()
			: base()
		{
            this.Initialize();
		}

		public NotifierDamage(Utility.Configuration.ConfigurationData.ConfigNotifierDamage config)
			: base(config)
		{
            this.Initialize();

            this.NotifiesBefore = config.NotifiesBefore;
            this.NotifiesNow = config.NotifiesNow;
            this.NotifiesAfter = config.NotifiesAfter;
            this.LevelBorder = config.LevelBorder;
            this.ContainsNotLockedShip = config.ContainsNotLockedShip;
            this.ContainsSafeShip = config.ContainsSafeShip;
            this.ContainsFlagship = config.ContainsFlagship;
            this.NotifiesAtEndpoint = config.NotifiesAtEndpoint;
		}


		private void Initialize()
		{
            this.DialogData.Title = "！대파경고！";

			APIObserver o = APIObserver.Instance;

			o["api_port/port"].ResponseReceived += this.CloseAll;

			o["api_req_map/start"].ResponseReceived += this.InSortie;
			o["api_req_map/next"].ResponseReceived += this.InSortie;

			o["api_get_member/mapinfo"].ResponseReceived += this.BeforeSortie;

			o["api_req_sortie/battleresult"].ResponseReceived += this.BattleFinished;
			o["api_req_combined_battle/battleresult"].ResponseReceived += this.BattleFinished;

			o["api_req_sortie/battle"].ResponseReceived += this.BattleStarted;
			o["api_req_battle_midnight/battle"].ResponseReceived += this.BattleStarted;
			o["api_req_battle_midnight/sp_midnight"].ResponseReceived += this.BattleStarted;
			o["api_req_sortie/airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_sortie/ld_airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_sortie/night_to_day"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/battle_water"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/midnight_battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/sp_midnight"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ld_airbattle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ec_battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ec_midnight_battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/ec_night_to_day"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/each_battle"].ResponseReceived += this.BattleStarted;
			o["api_req_combined_battle/each_battle_water"].ResponseReceived += this.BattleStarted;

		}

		void CloseAll(string apiname, dynamic data)
		{
            this.DialogData.OnCloseAll();
		}




		private void BeforeSortie(string apiname, dynamic data)
		{
			if (this.NotifiesNow || this.NotifiesBefore)
			{

				string[] array = this.GetDamagedShips(
					KCDatabase.Instance.Fleet.Fleets.Values
					.Where(f => f.ExpeditionState == 0)
					.SelectMany(f => f.MembersWithoutEscaped.Skip(!this.ContainsFlagship ? 1 : 0)));

				if (array != null && array.Length > 0)
				{
                    this.Notify(array);
				}
			}
		}


		private void InSortie(string apiname, dynamic data)
		{
			if (this.NotifiesAfter)
			{

				string[] array = this.GetDamagedShips(KCDatabase.Instance.Fleet.Fleets.Values
					.Where(f => f.IsInSortie)
					.SelectMany(f => f.MembersWithoutEscaped.Skip(!this.ContainsFlagship ? 1 : 0)));


				if (array != null && array.Length > 0)
				{
                    this.Notify(array);
				}
			}
		}


		private void BattleStarted(string apiname, dynamic data)
		{
			if (this.NotifiesBefore)
			{
                this.CheckBattle();
			}
		}


		private void BattleFinished(string apiname, dynamic data)
		{
			if (this.NotifiesNow)
			{
                this.CheckBattle();
			}
		}


		private void CheckBattle()
		{

			BattleManager bm = KCDatabase.Instance.Battle;

			if (bm.Compass.IsEndPoint && !this.NotifiesAtEndpoint)
				return;


			var list = new List<string>();

			var battle = bm.SecondBattle ?? bm.FirstBattle;
			list.AddRange(this.GetDamagedShips(battle.Initial.FriendFleet, battle.ResultHPs.ToArray()));

			if (bm.IsCombinedBattle)
				list.AddRange(this.GetDamagedShips(battle.Initial.FriendFleetEscort, battle.ResultHPs.Skip(6).ToArray()));


			if (list.Count > 0)
                this.Notify(list.ToArray());

		}


		// 注: 退避中かどうかまではチェックしない
		private bool IsShipDamaged(ShipData ship, int hp)
		{
			return ship != null &&
				hp > 0 &&
				(double)hp / ship.HPMax <= 0.25 &&
				ship.RepairingDockID == -1 &&
				ship.Level >= this.LevelBorder &&
				(this.ContainsNotLockedShip ? true : (ship.IsLocked || ship.SlotInstance.Count(q => q != null && q.IsLocked) > 0)) &&
				(this.ContainsSafeShip ? true : !ship.AllSlotInstanceMaster.Any(e => e?.CategoryType == EquipmentTypes.DamageControl));
		}

		private string[] GetDamagedShips(IEnumerable<ShipData> ships)
		{
			return ships.Where(s => this.IsShipDamaged(s, s?.HPCurrent ?? 0)).Select(s => $"{s.NameWithLevel} ({s.HPCurrent}/{s.HPMax})").ToArray();
		}

		private string[] GetDamagedShips(FleetData fleet, int[] hps)
		{

			LinkedList<string> list = new LinkedList<string>();

			for (int i = 0; i < fleet.Members.Count; i++)
			{
				if (i == 0 && !this.ContainsFlagship) continue;

				ShipData s = fleet.MembersInstance[i];

				if (s != null && !fleet.EscapedShipList.Contains(s.MasterID) && this.IsShipDamaged(s, hps[i]))
				{
					list.AddLast($"{s.NameWithLevel} ({hps[i]}/{s.HPMax})");
				}
			}

			return list.ToArray();
		}

		public void Notify(string[] messages)
		{

            this.DialogData.Message = string.Format("{0} 가 대파하였습니다!",
				string.Join(", ", messages));

			base.Notify();
		}


		public override void ApplyToConfiguration(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
		{
			base.ApplyToConfiguration(config);


			if (config is Utility.Configuration.ConfigurationData.ConfigNotifierDamage c)
			{
				c.NotifiesBefore = this.NotifiesBefore;
				c.NotifiesNow = this.NotifiesNow;
				c.NotifiesAfter = this.NotifiesAfter;
				c.LevelBorder = this.LevelBorder;
				c.ContainsNotLockedShip = this.ContainsNotLockedShip;
				c.ContainsSafeShip = this.ContainsSafeShip;
				c.ContainsFlagship = this.ContainsFlagship;
				c.NotifiesAtEndpoint = this.NotifiesAtEndpoint;
			}
		}

	}
}
