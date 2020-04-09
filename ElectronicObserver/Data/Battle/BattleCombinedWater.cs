using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 水上部隊 vs 通常艦隊 昼戦
	/// </summary>
	public class BattleCombinedWater : BattleDay
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);
            /*
			JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "噴式基地航空隊攻撃");
			JetAirBattle = new PhaseJetAirBattle(this, "噴式航空戦");
			BaseAirAttack = new PhaseBaseAirAttack(this, "基地航空隊攻撃");
			AirBattle = new PhaseAirBattle(this, "航空戦");
			Support = new PhaseSupport(this, "支援攻撃");
			OpeningASW = new PhaseOpeningASW(this, "先制対潜");
			OpeningTorpedo = new PhaseTorpedo(this, "先制雷撃", 0);
			Shelling1 = new PhaseShelling(this, "第一次砲撃戦", 1, "1");
			Shelling2 = new PhaseShelling(this, "第二次砲撃戦", 2, "2");
			Shelling3 = new PhaseShelling(this, "第三次砲撃戦", 3, "3");
			Torpedo = new PhaseTorpedo(this, "雷撃戦", 4);
            */
            this.JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
            this.JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
            this.BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
            this.AirBattle = new PhaseAirBattle(this, "항공전");
            this.Support = new PhaseSupport(this, "지원 공격");
            this.OpeningASW = new PhaseOpeningASW(this, "선제대잠");
            this.OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
            this.Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
            this.Shelling2 = new PhaseShelling(this, "제2차포격전", 2, "2");
            this.Shelling3 = new PhaseShelling(this, "제3차포격전", 3, "3");
            this.Torpedo = new PhaseTorpedo(this, "뇌격전", 4);


            foreach (var phase in this.GetPhases())
				phase.EmulateBattle(this._resultHPs, this._attackDamages);

		}


		public override string APIName => "api_req_combined_battle/battle_water";

		public override string BattleName => "연합함대-수상부대 주간전";


		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return this.Initial;
			yield return this.Searching;
			yield return this.JetBaseAirAttack;
			yield return this.JetAirBattle;
			yield return this.BaseAirAttack;
			yield return this.AirBattle;
			yield return this.Support;
			yield return this.OpeningASW;
			yield return this.OpeningTorpedo;
			yield return this.Shelling1;
			yield return this.Shelling2;
			yield return this.Shelling3;
			yield return this.Torpedo;
		}
	}

}
