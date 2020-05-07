using ElectronicObserver.Data.Battle.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

    /// <summary>
    /// 砲撃戦フェーズの処理を行います。
    /// </summary>
    public class PhaseShelling : PhaseBase
    {

        protected readonly int PhaseID;
        protected readonly string Suffix;


        public List<PhaseShellingAttack> Attacks { get; private set; }


        public class PhaseShellingAttack
        {
            public BattleIndex Attacker;
            public int AttackType;
            public List<PhaseShellingDefender> Defenders;
            public int[] EquipmentIDs;

            public PhaseShellingAttack()
            {
                this.Defenders = new List<PhaseShellingDefender>();
            }

            public override string ToString() => $"{this.Attacker}[{this.AttackType}] -> [{string.Join(", ", this.Defenders)}]";

        }
        public class PhaseShellingDefender
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



        public PhaseShelling(BattleData data, string title, int phaseID, string suffix)
            : base(data, title)
        {

            this.PhaseID = phaseID;
            this.Suffix = suffix;

            if (!this.IsAvailable)
                return;

            // "translate"

			int[] fleetflag = (int[])this.ShellingData.api_at_eflag;
			int[] attackers = (int[])this.ShellingData.api_at_list;
			int[] attackTypes = (int[])this.ShellingData.api_at_type;
			int[][] defenders = ((dynamic[])this.ShellingData.api_df_list).Select(elem => (int[])elem).ToArray();
			int[][] attackEquipments = ((dynamic[])this.ShellingData.api_si_list).Select(elem => ((dynamic[])elem).Select<dynamic, int>(ch => ch is string ? int.Parse(ch) : (int)ch).ToArray()).ToArray();
			int[][] criticalFlags = ((dynamic[])this.ShellingData.api_cl_list).Select(elem => (int[])elem).ToArray();
			double[][] rawDamages = ((dynamic[])this.ShellingData.api_damage).Select(elem => ((double[])elem).Select(p => Math.Max(p, 0)).ToArray()).ToArray();

            this.Attacks = new List<PhaseShellingAttack>();

            for (int i = 0; i < attackers.Length; i++)
            {
                var attack = new PhaseShellingAttack()
                {
                    Attacker = new BattleIndex(attackers[i] + (fleetflag[i] == 0 ? 0 : 12), this.Battle.IsFriendCombined, this.Battle.IsEnemyCombined),
                };


                for (int k = 0; k < defenders[i].Length; k++)
                {
                    var defender = new PhaseShellingDefender
                    {
                        Defender = new BattleIndex(defenders[i][k] + (fleetflag[i] == 0 ? 12 : 0), this.Battle.IsFriendCombined, this.Battle.IsEnemyCombined),
                        CriticalFlag = criticalFlags[i][k],
                        RawDamage = rawDamages[i][k],
                    };

                    attack.Defenders.Add(defender);
                }

                attack.AttackType = attackTypes[i];
                attack.EquipmentIDs = attackEquipments[i];

                this.Attacks.Add(attack);
            }

        }


        public override bool IsAvailable => (int)this.RawData.api_hourai_flag[this.PhaseID - 1] != 0;


        public virtual dynamic ShellingData => this.RawData["api_hougeki" + this.Suffix];


        public override void EmulateBattle(int[] hps, int[] damages)
        {

            if (!this.IsAvailable)
                return;


            foreach (var atk in this.Attacks)
            {
                switch (atk.AttackType)
                {
                    case 100:
                        // nelson touch
                        for (int i = 0; i < atk.Defenders.Count; i++)
                        {
                            var comboatk = new BattleIndex(atk.Attacker.Side, i * 2);       // #1, #3, #5
                            this.BattleDetails.Add(new BattleDayDetail(this.Battle, comboatk, atk.Defenders[i].Defender, new[] { atk.Defenders[i].RawDamage }, new[] { atk.Defenders[i].CriticalFlag }, atk.AttackType, atk.EquipmentIDs, hps[atk.Defenders[i].Defender]));
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
                            this.BattleDetails.Add(new BattleDayDetail(this.Battle, comboatk, atk.Defenders[i].Defender, new[] { atk.Defenders[i].RawDamage }, new[] { atk.Defenders[i].CriticalFlag }, atk.AttackType, atk.EquipmentIDs, hps[atk.Defenders[i].Defender]));
                            this.AddDamage(hps, atk.Defenders[i].Defender, atk.Defenders[i].Damage);
                            damages[comboatk] += atk.Defenders[i].Damage;
                        }
                        break;
                    case 103:
                    case 104:
                        // colorado touch / kongo-class night attack
                        for (int i = 0; i < atk.Defenders.Count; i++)
                        {
                            var comboatk = new BattleIndex(atk.Attacker.Side, i);       // #1, #2, #3
                            this.BattleDetails.Add(new BattleDayDetail(this.Battle, comboatk, atk.Defenders[i].Defender, new[] { atk.Defenders[i].RawDamage }, new[] { atk.Defenders[i].CriticalFlag }, atk.AttackType, atk.EquipmentIDs, hps[atk.Defenders[i].Defender]));
                            this.AddDamage(hps, atk.Defenders[i].Defender, atk.Defenders[i].Damage);
                            damages[comboatk] += atk.Defenders[i].Damage;
                        }
                        break;

                    default:
                        foreach (var defs in atk.Defenders.GroupBy(d => d.Defender))
                        {
                            this.BattleDetails.Add(new BattleDayDetail(this.Battle, atk.Attacker, defs.Key, defs.Select(d => d.RawDamage).ToArray(), defs.Select(d => d.CriticalFlag).ToArray(), atk.AttackType, atk.EquipmentIDs, hps[defs.Key]));
                            this.AddDamage(hps, defs.Key, defs.Sum(d => d.Damage));
                        }
                        damages[atk.Attacker] += atk.Defenders.Sum(d => d.Damage);
                        break;
                }
            }

        }

    }
}
