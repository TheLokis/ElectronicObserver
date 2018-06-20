using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 通常艦隊 vs 通常艦隊 航空戦
	/// </summary>
	public class BattleAirBattle : BattleDay
	{

		public PhaseAirBattle AirBattle2 { get; protected set; }

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

			JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
			JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
			BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
			AirBattle = new PhaseAirBattle(this, "1차 항공전");
			Support = new PhaseSupport(this, "지원 공격");
			AirBattle2 = new PhaseAirBattle(this, "2차 항공전", "2");

			foreach (var phase in GetPhases())
				phase.EmulateBattle(_resultHPs, _attackDamages);

		}

		public override string APIName => "api_req_sortie/airbattle";

		public override string BattleName => "통상 항공전";


		

		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return Initial;
			yield return Searching;
			yield return JetBaseAirAttack;
			yield return JetAirBattle;
			yield return BaseAirAttack;
			yield return AirBattle;
			yield return Support;
			yield return AirBattle2;
		}

	}
}
