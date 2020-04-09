using ElectronicObserver.Data.Battle.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 夜戦フェーズの処理を行います。
	/// </summary>
	public class PhaseNightBattle : PhaseBase
	{

		private readonly int PhaseID;


		public PhaseNightBattle(BattleData data, string title, int phaseID)
			: base(data, title)
		{

            this.PhaseID = phaseID;


			if (!this.IsAvailable)
				return;


			int[] fleetflag = (int[])this.ShellingData.api_at_eflag;
			int[] attackers = (int[])this.ShellingData.api_at_list;
			int[] nightAirAttackFlags = (int[])this.ShellingData.api_n_mother_list;
			int[] attackTypes = (int[])this.ShellingData.api_sp_list;
			int[][] defenders = ((dynamic[])this.ShellingData.api_df_list).Select(elem => ((int[])elem).Where(e => e != -1).ToArray()).ToArray();
			int[][] attackEquipments = ((dynamic[])this.ShellingData.api_si_list).Select(elem => ((dynamic[])elem).Select<dynamic, int>(ch => ch is string ? int.Parse(ch) : (int)ch).ToArray()).ToArray();
			int[][] criticals = ((dynamic[])this.ShellingData.api_cl_list).Select(elem => ((int[])elem).Where(e => e != -1).ToArray()).ToArray();
			double[][] rawDamages = ((dynamic[])this.ShellingData.api_damage).Select(elem => ((double[])elem).Where(e => e != -1).ToArray()).ToArray();

            this.Attacks = new List<PhaseNightBattleAttack>();



			for (int i = 0; i < attackers.Length; i++)
			{
				var attack = new PhaseNightBattleAttack
				{
					Attacker = new BattleIndex(attackers[i] + (fleetflag[i] == 0 ? 0 : 12), this.Battle.IsFriendCombined, this.Battle.IsEnemyCombined),
					NightAirAttackFlag = nightAirAttackFlags[i] == -1,
					AttackType = attackTypes[i],
					EquipmentIDs = attackEquipments[i],
				};
				for (int k = 0; k < defenders[i].Length; k++)
				{
					var defender = new PhaseNightBattleDefender
					{
						Defender = new BattleIndex(defenders[i][k] + (fleetflag[i] == 0 ? 12 : 0), this.Battle.IsFriendCombined, this.Battle.IsEnemyCombined),
						CriticalFlag = criticals[i][k],
						RawDamage = rawDamages[i][k]
					};
					attack.Defenders.Add(defender);
				}

                this.Attacks.Add(attack);
			}

		}

		public override bool IsAvailable =>
            this.RawData.IsDefined(this.ShellingDataName) &&
            this.RawData[this.ShellingDataName].api_at_list() &&
            this.RawData[this.ShellingDataName].api_at_list != null;


		public dynamic ShellingData => this.RawData[this.ShellingDataName];

		private string ShellingDataName => this.PhaseID == 0 ? "api_hougeki" : ("api_n_hougeki" + this.PhaseID);


		public override void EmulateBattle(int[] hps, int[] damages)
		{

            if (!this.IsAvailable) return;


            foreach (var atk in this.Attacks)
            {
                switch (atk.AttackType)
                {
                    case 100:
                        // nelson touch
                        for (int i = 0; i < atk.Defenders.Count; i++)
                        {
                            var comboatk = new BattleIndex(atk.Attacker.Side, i * 2);       // #1, #3, #5
                            this.BattleDetails.Add(new BattleNightDetail(this.Battle, comboatk, atk.Defenders[i].Defender, new[] { atk.Defenders[i].RawDamage }, new[] { atk.Defenders[i].CriticalFlag }, atk.AttackType, atk.EquipmentIDs, atk.NightAirAttackFlag, hps[atk.Defenders[i].Defender]));
                            this.AddDamage(hps, atk.Defenders[i].Defender, atk.Defenders[i].Damage);
                            damages[comboatk] += atk.Defenders[i].Damage;
                        }
                        break;

                    case 101:
                    case 102:
                        // nagato/mutsu touch
                        for (int i = 0; i < atk.Defenders.Count; i++)
                        {
                            var comboatk = new BattleIndex(atk.Attacker.Side, i / 2);       // #1, #1, #2
                            this.BattleDetails.Add(new BattleNightDetail(this.Battle, comboatk, atk.Defenders[i].Defender, new[] { atk.Defenders[i].RawDamage }, new[] { atk.Defenders[i].CriticalFlag }, atk.AttackType, atk.EquipmentIDs, atk.NightAirAttackFlag, hps[atk.Defenders[i].Defender]));
                            this.AddDamage(hps, atk.Defenders[i].Defender, atk.Defenders[i].Damage);
                            damages[comboatk] += atk.Defenders[i].Damage;
                        }
                        break;
                    case 103:
                        // colorado touch
                        for (int i = 0; i < atk.Defenders.Count; i++)
                        {
                            var comboatk = new BattleIndex(atk.Attacker.Side, i);       // #1, #2, #3
                            this.BattleDetails.Add(new BattleNightDetail(this.Battle, comboatk, atk.Defenders[i].Defender, new[] { atk.Defenders[i].RawDamage }, new[] { atk.Defenders[i].CriticalFlag }, atk.AttackType, atk.EquipmentIDs, atk.NightAirAttackFlag, hps[atk.Defenders[i].Defender]));
                            this.AddDamage(hps, atk.Defenders[i].Defender, atk.Defenders[i].Damage);
                            damages[comboatk] += atk.Defenders[i].Damage;
                        }
                        break;

                    default:
                        foreach (var defs in atk.Defenders.GroupBy(d => d.Defender))
                        {
                            this.BattleDetails.Add(new BattleNightDetail(this.Battle, atk.Attacker, defs.Key, defs.Select(d => d.RawDamage).ToArray(), defs.Select(d => d.CriticalFlag).ToArray(), atk.AttackType, atk.EquipmentIDs, atk.NightAirAttackFlag, hps[defs.Key]));
                            this.AddDamage(hps, defs.Key, defs.Sum(d => d.Damage));
                        }
                        damages[atk.Attacker] += atk.Defenders.Sum(d => d.Damage);
                        break;
                }
            }

        }


        public List<PhaseNightBattleAttack> Attacks { get; private set; }
		public class PhaseNightBattleAttack
		{
			public BattleIndex Attacker;
			public int AttackType;
			public bool NightAirAttackFlag;
			public List<PhaseNightBattleDefender> Defenders;
			public int[] EquipmentIDs;

			public PhaseNightBattleAttack()
			{
                this.Defenders = new List<PhaseNightBattleDefender>();
			}

			public override string ToString() => $"{this.Attacker}[{this.AttackType}] -> [{string.Join(", ", this.Defenders)}]";

		}
		public class PhaseNightBattleDefender
		{
			public BattleIndex Defender;
			public int CriticalFlag;
			public double RawDamage;
			public bool GuardsFlagship => this.RawDamage != Math.Floor(this.RawDamage);
			public int Damage => (int)this.RawDamage;

			public override string ToString()
			{
				return string.Format("{0};{1}-{2}{3}", this.Defender, this.Damage,
                    this.CriticalFlag == 0 ? "miss" : this.CriticalFlag == 1 ? "dmg" : this.CriticalFlag == 2 ? "crit" : "INVALID",
                    this.GuardsFlagship ? " (guard)" : "");
			}
		}

	}
}
