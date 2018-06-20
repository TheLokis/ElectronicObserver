using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 演習 昼戦
	/// </summary>
	public class BattlePracticeDay : BattleDay
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

			JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
			AirBattle = new PhaseAirBattle(this, "항공전");
			OpeningASW = new PhaseOpeningASW(this, "선제대잠");
			OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
			Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
			Shelling2 = new PhaseShelling(this, "제2차포격전", 2, "2");
			Shelling3 = new PhaseShelling(this, "제3차폭겨전", 3, "3");
			Torpedo = new PhaseTorpedo(this, "뇌격전", 4);


			foreach (var phase in GetPhases())
				phase.EmulateBattle(_resultHPs, _attackDamages);

		}


		public override string APIName => "api_req_practice/battle";

		public override string BattleName => "연습 주간전";

		public override bool IsPractice => true;


		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return Initial;
			yield return Searching;
			yield return JetAirBattle;
			yield return AirBattle;
			yield return OpeningASW;
			yield return OpeningTorpedo;
			yield return Shelling1;
			yield return Shelling2;
			yield return Shelling3;
			yield return Torpedo;
		}
	}

}
