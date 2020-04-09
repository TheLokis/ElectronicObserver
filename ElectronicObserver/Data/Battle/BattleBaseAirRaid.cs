using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 基地防空戦
	/// </summary>
	public class BattleBaseAirRaid : BattleDay
	{

		public PhaseBaseAirRaid BaseAirRaid { get; protected set; }

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            this.BaseAirRaid = new PhaseBaseAirRaid(this, "방공전");

			foreach (var phase in this.GetPhases())
				phase.EmulateBattle(this._resultHPs, this._attackDamages);

		}


		public override string APIName => "api_req_map/next";

		public override string BattleName => "기지항공대";

		public override bool IsBaseAirRaid => true;

		public override IEnumerable<PhaseBase> GetPhases()
		{
			yield return this.Initial;
			yield return this.Searching;
			yield return this.BaseAirRaid;
		}
	}
}
