using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 通常艦隊 vs 通常艦隊 長距離空襲戦
	/// </summary>
	public class BattleAirRaid : BattleDay
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            this.JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
            this.JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
            this.BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
            this.AirBattle = new PhaseAirBattle(this, "공습전");
			// 支援は出ないものとする

			foreach (var phase in this.GetPhases())
				phase.EmulateBattle(this._resultHPs, this._attackDamages);

		}

		public override string APIName => "api_req_sortie/ld_airbattle";

		public override string BattleName => "통상 장거리 공습전";

		
		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return this.Initial;
			yield return this.Searching;
			yield return this.JetBaseAirAttack;
			yield return this.JetAirBattle;
			yield return this.BaseAirAttack;
			yield return this.AirBattle;
		}
	}
}
