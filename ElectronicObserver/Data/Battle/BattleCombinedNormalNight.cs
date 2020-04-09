using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 連合艦隊 vs 通常艦隊 夜戦
	/// </summary>
	public class BattleCombinedNormalNight : BattleNight
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            this.NightInitial = new PhaseNightInitial(this, "야전개시", true);
            this.FriendlySupport = new PhaseFriendlySupport(this, "우군함대공격");
            // 支援なし?
            this.NightBattle = new PhaseNightBattle(this, "야전", 0);

            foreach (var phase in this.GetPhases())
				phase.EmulateBattle(this._resultHPs, this._attackDamages);
		}


		public override string APIName => "api_req_combined_battle/midnight_battle";

		public override string BattleName => "연합함대 야전";


		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return this.Initial;
			yield return this.NightInitial;
			yield return this.FriendlySupport;
			yield return this.NightBattle;
		}
	}

}
