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
	/// 基地航空隊攻撃フェーズの処理を行います。
	/// </summary>
	public class PhaseBaseAirAttack : PhaseBase
	{


		/// <summary>
		/// 基地航空隊攻撃フェーズの、個々の攻撃フェーズの処理を行います。
		/// </summary>
		public class PhaseBaseAirAttackUnit : PhaseAirBattleBase
		{


			public PhaseBaseAirAttackUnit(BattleData data, string title, int index)
				: base(data, title)
			{

                this.AirAttackIndex = index;
                this.AirBattleData = data.RawData.api_air_base_attack[index];
                this.StageFlag = this.AirBattleData.api_stage_flag() ? (int[])this.AirBattleData.api_stage_flag : null;

                this.LaunchedShipIndexEnemy = this.GetLaunchedShipIndex(0);

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

			/// <summary>
			/// 航空隊ID
			/// </summary>
			public int AirUnitID => (int)this.AirBattleData.api_base_id;



			private BattleBaseAirCorpsSquadron[] _squadrons;
			/// <summary>
			/// 参加した航空中隊データ
			/// </summary>
			public ReadOnlyCollection<BattleBaseAirCorpsSquadron> Squadrons => Array.AsReadOnly(this._squadrons);

			private IEnumerable<BattleBaseAirCorpsSquadron> GetSquadrons()
			{
				foreach (dynamic d in this.AirBattleData.api_squadron_plane)
					yield return new BattleBaseAirCorpsSquadron(d);
			}

		}



		public PhaseBaseAirAttack(BattleData data, string title)
			: base(data, title)
		{

            this.AirAttackUnits = new List<PhaseBaseAirAttackUnit>();

			if (!this.IsAvailable)
				return;


			int i = 0;
			foreach (var unit in this.RawData.api_air_base_attack)
			{
                this.AirAttackUnits.Add(new PhaseBaseAirAttackUnit(data, title, i));
				i++;
			}

		}


		/// <summary>
		/// 個々の攻撃フェーズのデータ
		/// </summary>
		public List<PhaseBaseAirAttackUnit> AirAttackUnits { get; private set; }



		public override bool IsAvailable => this.RawData.api_air_base_attack();


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


	/// <summary>
	/// 戦闘に参加した基地航空隊中隊のデータ
	/// </summary>
	public class BattleBaseAirCorpsSquadron
	{
		public int EquipmentID { get; private set; }
		public int AircraftCount { get; private set; }
		public EquipmentDataMaster EquipmentInstance => KCDatabase.Instance.MasterEquipments[this.EquipmentID];

		public BattleBaseAirCorpsSquadron(dynamic data)
		{
            this.EquipmentID = (int)data.api_mst_id;
            this.AircraftCount = (int)data.api_count;
		}

		public override string ToString() => $"{this.EquipmentInstance?.Name ?? "미확인장비"} x {this.AircraftCount}";

	}

}
