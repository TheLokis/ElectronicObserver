using ElectronicObserver.Data.Battle.Detail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 噴式航空機による基地航空隊攻撃フェーズの処理を行います。
	/// </summary>
	public class PhaseJetBaseAirAttack : PhaseBase
	{

		/// <summary>
		/// 噴式航空機による基地航空隊攻撃フェーズの、個々の攻撃フェーズの処理を行います。
		/// </summary>
		public class PhaseJetBaseAirAttackUnit : PhaseAirBattleBase
		{

			public PhaseJetBaseAirAttackUnit(BattleData data, string title, int index)
				: base(data, title)
			{

				if (index == -1)
				{
                    this.AirAttackIndex = 0;
                    this.AirBattleData = data.RawData.api_air_base_injection;
				}
				else
				{
                    this.AirAttackIndex = index;
                    this.AirBattleData = data.RawData.api_air_base_injection[index];
				}

				if (this.AirBattleData != null)
				{
                    this.StageFlag = new int[] {
                        this.AirBattleData.api_stage1() ? 1 : 0,
                        this.AirBattleData.api_stage2() ? 1 : 0,
                        this.AirBattleData.api_stage3() ? 1 : 0,
					};
				}

                this._squadrons = this.GetSquadrons().ToArray();

                this.TorpedoFlags = this.ConcatStage3Array<int>("api_frai_flag", "api_erai_flag");
                this.BomberFlags = this.ConcatStage3Array<int>("api_fbak_flag", "api_ebak_flag");
                this.Criticals = this.ConcatStage3Array<int>("api_fcl_flag", "api_ecl_flag");
                this.Damages = this.ConcatStage3Array<double>("api_fdam", "api_edam");
			}


			public override void EmulateBattle(int[] hps, int[] damages)
			{

				if (!this.IsAvailable) return;

                this.CalculateAttack(this.AirAttackIndex + 1, hps);
			}


			/// <summary>
			/// 攻撃ID (第n波, 0から始まる)
			/// </summary>
			public int AirAttackIndex { get; private set; }


			private BattleBaseAirCorpsSquadron[] _squadrons;
			/// <summary>
			/// 参加した航空中隊データ
			/// </summary>
			public ReadOnlyCollection<BattleBaseAirCorpsSquadron> Squadrons => Array.AsReadOnly(this._squadrons);

			private IEnumerable<BattleBaseAirCorpsSquadron> GetSquadrons()
			{
				foreach (dynamic d in this.AirBattleData.api_air_base_data)
					yield return new BattleBaseAirCorpsSquadron(d);
			}

		}



		public PhaseJetBaseAirAttack(BattleData data, string title)
			: base(data, title)
		{

            this.AirAttackUnits = new List<PhaseJetBaseAirAttackUnit>();

			if (!this.IsAvailable)
				return;


			dynamic attackData = this.RawData.api_air_base_injection;
			if (attackData.IsArray)
			{
				int i = 0;
				foreach (var unit in this.RawData.api_air_base_injection)
				{
                    this.AirAttackUnits.Add(new PhaseJetBaseAirAttackUnit(data, title, i));
					i++;
				}

			}
			else if (attackData.IsObject)
			{
                this.AirAttackUnits.Add(new PhaseJetBaseAirAttackUnit(data, title, -1));
			}
		}


		/// <summary>
		/// 個々の攻撃フェーズのデータ
		/// </summary>
		public List<PhaseJetBaseAirAttackUnit> AirAttackUnits { get; private set; }



		public override bool IsAvailable => this.RawData.api_air_base_injection();


		public override void EmulateBattle(int[] hps, int[] damages)
		{

			if (!this.IsAvailable)
				return;

			foreach (var a in this.AirAttackUnits)
			{

				a.EmulateBattle(hps, damages);
			}
		}


		protected override IEnumerable<BattleDetail> SearchBattleDetails(int index)
		{
			return this.AirAttackUnits.SelectMany(p => p.BattleDetails).Where(d => d.DefenderIndex == index);
		}
	}
}
