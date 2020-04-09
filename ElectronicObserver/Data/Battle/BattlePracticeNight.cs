using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 演習 夜戦
	/// </summary>
	public class BattlePracticeNight : BattleNight
	{

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            this.NightInitial = new PhaseNightInitial(this, "야전개시", false);
            this.NightBattle = new PhaseNightBattle(this, "야전", 0);

            this.NightBattle.EmulateBattle(this._resultHPs, this._attackDamages);

		}


		public override string APIName => "api_req_practice/midnight_battle";

		public override string BattleName => "연습 야간전";

		public override bool IsPractice => true;


		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return this.Initial;
			yield return this.NightInitial;
			yield return this.NightBattle;
		}
	}
}
