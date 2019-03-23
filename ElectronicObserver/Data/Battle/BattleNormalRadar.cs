﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Data.Battle.Phase;

namespace ElectronicObserver.Data.Battle
{
    /// <summary>
    /// 通常艦隊 vs 通常艦隊 レーダー射撃
    /// </summary>
    public class BattleNormalRadar : BattleDay
    {
        public override void LoadFromResponse(string apiname, dynamic data)
        {
            base.LoadFromResponse(apiname, (object)data);

            JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "기지항공대 분식 강습");
            BaseAirAttack = new PhaseBaseAirAttack(this, "기지 항공대 공격");

            Shelling1 = new PhaseRadar(this, "레이더사격");

            foreach (var phase in GetPhases())
                phase.EmulateBattle(_resultHPs, _attackDamages);
        }


        public override string APIName => "api_req_sortie/ld_shooting";

        public override string BattleName => "통상함대 레이더 사격";

        public override IEnumerable<PhaseBase> GetPhases()
        {
            yield return Initial;
            yield return Searching;     // ?
            yield return JetBaseAirAttack;
            yield return BaseAirAttack;
            yield return Shelling1;
        }
    }
}