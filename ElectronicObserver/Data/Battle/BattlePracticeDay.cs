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

            this.JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
            this.AirBattle = new PhaseAirBattle(this, "항공전");
            this.OpeningASW = new PhaseOpeningASW(this, "선제대잠");
            this.OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
            this.Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
            this.Shelling2 = new PhaseShelling(this, "제2차포격전", 2, "2");
            this.Shelling3 = new PhaseShelling(this, "제3차포격전", 3, "3");
            this.Torpedo = new PhaseTorpedo(this, "뇌격전", 4);


			foreach (var phase in this.GetPhases())
				phase.EmulateBattle(this._resultHPs, this._attackDamages);

		}


		public override string APIName => "api_req_practice/battle";

		public override string BattleName => "연습 주간전";

		public override bool IsPractice => true;


		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return this.Initial;
			yield return this.Searching;
			yield return this.JetAirBattle;
			yield return this.AirBattle;
			yield return this.OpeningASW;
			yield return this.OpeningTorpedo;
			yield return this.Shelling1;
			yield return this.Shelling2;
			yield return this.Shelling3;
			yield return this.Torpedo;
		}
	}

}
