using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 通常艦隊 vs 通常艦隊 夜戦
	/// </summary>
	public class BattleNormalNight : BattleNight
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

			NightInitial = new PhaseNightInitial(this, "야전개시", false);
			FriendlySupport = new PhaseFriendlySupport(this, "우군함대원호");
			// 支援なし?
			NightBattle = new PhaseNightBattle(this, "야전", 0);

			foreach (var phase in GetPhases())
				phase.EmulateBattle(_resultHPs, _attackDamages);
		}


		public override string APIName => "api_req_battle_midnight/battle";

		public override string BattleName => "일반함대 야전";



		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return Initial;
			yield return NightInitial;
			yield return FriendlySupport;
			yield return NightBattle;
		}
	}
}
