using ElectronicObserver.Data.Battle.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 航空戦フェーズの処理を行います。
	/// </summary>
	public class PhaseAirBattle : PhaseAirBattleBase
	{

		public PhaseAirBattle(BattleData data, string title, string suffix = "")
			: base(data, title)
		{

            this.AirBattleData = this.RawData.IsDefined("api_kouku" + suffix) ? this.RawData["api_kouku" + suffix] : null;
            this.StageFlag = this.RawData.IsDefined("api_stage_flag" + suffix) ? (int[])this.RawData["api_stage_flag" + suffix] : null;

            this.LaunchedShipIndexFriend = this.GetLaunchedShipIndex(0);
            this.LaunchedShipIndexEnemy = this.GetLaunchedShipIndex(1);

            this.TorpedoFlags = this.ConcatStage3Array<int>("api_frai_flag", "api_erai_flag");
            this.BomberFlags = this.ConcatStage3Array<int>("api_fbak_flag", "api_ebak_flag");
            this.Criticals = this.ConcatStage3Array<int>("api_fcl_flag", "api_ecl_flag");
            this.Damages = this.ConcatStage3Array<double>("api_fdam", "api_edam");
		}


		public override void EmulateBattle(int[] hps, int[] damages)
		{

			if (!this.IsAvailable) return;

            this.CalculateAttack(0, hps);
            this.CalculateAttackDamage(damages);
		}



		/// <summary>
		/// 航空戦での与ダメージを推測します。
		/// </summary>
		/// <param name="damages">与ダメージリスト。</param>
		private void CalculateAttackDamage(int[] damages)
		{
			// 敵はめんどくさすぎるので省略
			// 仮想火力を求め、それに従って合計ダメージを分配

			var firepower = new int[12];
			var launchedIndex = this.LaunchedShipIndexFriend;
			
			foreach (int i in launchedIndex)
			{
				var ship = this.Battle.Initial.GetFriendShip(i);

				if (ship == null)
					continue;

				var slots = ship.SlotInstanceMaster;
				var aircrafts = ship.Aircraft;
				for (int s = 0; s < slots.Count; s++)
				{

					if (slots[s] == null)
						continue;

					switch (slots[s].CategoryType)
					{
						// 通常の爆撃機系
						case EquipmentTypes.CarrierBasedBomber:
						case EquipmentTypes.SeaplaneBomber:
							firepower[i] += (int)(1.0 * (slots[s].Bomber * Math.Sqrt(aircrafts[s]) + 25));
							break;
						
						// 噴式爆撃機
						case EquipmentTypes.JetBomber:
							firepower[i] += (int)(1.0 / Math.Sqrt(2) * (slots[s].Bomber * Math.Sqrt(aircrafts[s]) + 25));
							break;

						// 通常の攻撃機系 (80%と150%はランダムのため係数は平均値)
						case EquipmentTypes.CarrierBasedTorpedo:
							firepower[i] += (int)(1.15 * (slots[s].Torpedo * Math.Sqrt(aircrafts[s]) + 25));
							break;

						// 噴式攻撃機(いる？)
						case EquipmentTypes.JetTorpedo:
							firepower[i] += (int)(1.15 / Math.Sqrt(2) * (slots[s].Torpedo * Math.Sqrt(aircrafts[s]) + 25));
							break;
					}
				}
			}

			int totalFirepower = firepower.Sum();
			int totalDamage = this.Damages.Select(dmg => (int)dmg).Skip(12).Take(12).Sum();

			for (int i = 0; i < firepower.Length; i++)
			{
				damages[i] += (int)Math.Round((double)totalDamage * firepower[i] / Math.Max(totalFirepower, 1));
			}
		}

		protected override IEnumerable<BattleDetail> SearchBattleDetails(int index)
		{
			return this.BattleDetails.Where(d => d.DefenderIndex == index);
		}

	}
}
