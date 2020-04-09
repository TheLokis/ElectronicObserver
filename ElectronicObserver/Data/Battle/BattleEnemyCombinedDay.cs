using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{
	/// <summary>
	/// 通常艦隊 vs 連合艦隊 昼戦
	/// </summary>
	public class BattleEnemyCombinedDay : BattleDay
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
			Torpedo = new PhaseTorpedo(this, "雷撃戦", 2);
			Shelling2 = new PhaseShelling(this, "第二次砲撃戦", 3, "2");
			Shelling3 = new PhaseShelling(this, "第三次砲撃戦", 4, "3");
            */
            this.JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
            this.JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
            this.BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
            this.AirBattle = new PhaseAirBattle(this, "1차 항공전");
            this.Support = new PhaseSupport(this, "지원 공격");
            this.OpeningASW = new PhaseOpeningASW(this, "선제대잠");
            this.OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
            this.Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
            this.Torpedo = new PhaseTorpedo(this, "뇌격전", 2);
            this.Shelling2 = new PhaseShelling(this, "제2차포격전", 3, "2");
            this.Shelling3 = new PhaseShelling(this, "제3차포격전", 4, "3");

            foreach (var phase in this.GetPhases())
				phase.EmulateBattle(this._resultHPs, this._attackDamages);

		}


		public override string APIName => "api_req_combined_battle/ec_battle";

		public override string BattleName => "통상함대 대 연합함대 주간전";



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
			yield return this.Torpedo;
			yield return this.Shelling2;
			yield return this.Shelling3;
		}
	}
}
