using DynaJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 基地空襲戦フェーズ
	/// </summary>
	public class PhaseBaseAirRaid : PhaseAirBattleBase
	{

		private BattleBaseAirCorpsSquadron[] _squadrons;
		/// <summary>
		/// 参加した航空中隊データ
		/// </summary>
		public ReadOnlyCollection<BattleBaseAirCorpsSquadron> Squadrons => Array.AsReadOnly(this._squadrons);

		private IEnumerable<BattleBaseAirCorpsSquadron> GetSquadrons()
		{
			if (this.AirBattleData.api_map_squadron_plane == null)
				yield break;

			foreach (KeyValuePair<string, dynamic> p in this.AirBattleData.api_map_squadron_plane)
			{
				if (!(p.Value is JsonObject))
					continue;
				if (!p.Value.IsArray)
					continue;

				foreach (dynamic e in p.Value)
					yield return new BattleBaseAirCorpsSquadron(e);
			}
		}


		public PhaseBaseAirRaid(BattleData data, string title)
			: base(data, title)
		{

            this.AirBattleData = data.RawData.api_air_base_attack;
            this.StageFlag = this.AirBattleData.api_stage_flag() ? (int[])this.AirBattleData.api_stage_flag : null;

            this.LaunchedShipIndexFriend = this.GetLaunchedShipIndex(0);
            this.LaunchedShipIndexEnemy = this.GetLaunchedShipIndex(1);

            this._squadrons = this.GetSquadrons().ToArray();

            this.TorpedoFlags = this.ConcatStage3Array<int>("api_frai_flag", "api_erai_flag");
            this.BomberFlags = this.ConcatStage3Array<int>("api_fbak_flag", "api_ebak_flag");
            this.Criticals = this.ConcatStage3Array<int>("api_fcl_flag", "api_ecl_flag");
            this.Damages = this.ConcatStage3Array<double>("api_fdam", "api_edam");
		}

		public override void EmulateBattle(int[] hps, int[] damages)
		{
			if (!this.IsAvailable) return;

            this.CalculateAttack(-1, hps);
		}

	}
}
