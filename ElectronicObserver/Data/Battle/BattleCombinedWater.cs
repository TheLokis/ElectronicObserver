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
            JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
            JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
            BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
            AirBattle = new PhaseAirBattle(this, "항공전");
            Support = new PhaseSupport(this, "지원 공격");
            OpeningASW = new PhaseOpeningASW(this, "선제대잠");
            OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
            Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
            Shelling2 = new PhaseShelling(this, "제2차포격전", 2, "2");
            Shelling3 = new PhaseShelling(this, "제3차포격전", 3, "3");
            Torpedo = new PhaseTorpedo(this, "뇌격전", 4);


            foreach (var phase in GetPhases())
				phase.EmulateBattle(_resultHPs, _attackDamages);

		}


		public override string APIName => "api_req_combined_battle/battle_water";

		public override string BattleName => "연합함대-수상부대 주간전";


		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return Initial;
			yield return Searching;
			yield return JetBaseAirAttack;
			yield return JetAirBattle;
			yield return BaseAirAttack;
			yield return AirBattle;
			yield return Support;
			yield return OpeningASW;
			yield return OpeningTorpedo;
			yield return Shelling1;
			yield return Shelling2;
			yield return Shelling3;
			yield return Torpedo;
		}
	}

}
