﻿using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 機動部隊 vs 通常艦隊 昼戦
	/// </summary>
	public class BattleCombinedNormalDay : BattleDay
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
            JetAirBattle = new PhaseJetAirBattle(this, "분식 항공전");
            BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");
            AirBattle = new PhaseAirBattle(this, "항공전");
            Support = new PhaseSupport(this, "지원 공격");
            OpeningASW = new PhaseOpeningASW(this, "선제대잠");
            OpeningTorpedo = new PhaseTorpedo(this, "선제뇌격", 0);
            Shelling1 = new PhaseShelling(this, "제1차포격전", 1, "1");
            Torpedo = new PhaseTorpedo(this, "뇌격전", 2);
            Shelling2 = new PhaseShelling(this, "제2차포격전", 3, "2");
            Shelling3 = new PhaseShelling(this, "제3차포격전", 4, "3");

            foreach (var phase in GetPhases())
				phase.EmulateBattle(_resultHPs, _attackDamages);

		}


		public override string APIName => "api_req_combined_battle/battle";

		public override string BattleName => "連合艦隊-機動部隊 昼戦";



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
			yield return Torpedo;
			yield return Shelling2;
			yield return Shelling3;
		}

	}

}
