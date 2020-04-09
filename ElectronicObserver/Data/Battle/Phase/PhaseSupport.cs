using ElectronicObserver.Data.Battle.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 支援攻撃フェーズの処理を行います。
	/// </summary>
	public class PhaseSupport : PhaseBase
	{

		public readonly bool IsNight;

		public PhaseSupport(BattleData data, string title, bool isNight = false)
			: base(data, title)
		{
            this.IsNight = isNight;

			switch (this.SupportFlag)
			{
				case 1:     // 空撃
				case 4:     // 対潜
					{
						if ((int)this.SupportData.api_support_airatack.api_stage_flag[2] != 0)
						{
                            // 敵連合でも api_stage3_combined は存在しない？

                            this.Damages = ((double[])this.SupportData.api_support_airatack.api_stage3.api_edam).ToArray();
                            this.Criticals = ((int[])this.SupportData.api_support_airatack.api_stage3.api_ecl_flag).ToArray();

							// 航空戦なので crit フラグが違う
							for (int i = 0; i < this.Criticals.Length; i++)
                                this.Criticals[i]++;
						}
						else
						{
                            this.Damages = new double[12];
                            this.Criticals = new int[12];
						}
					}
					break;
				case 2:     // 砲撃
				case 3:     // 雷撃
					{
						var dmg = (double[])this.SupportData.api_support_hourai.api_damage;
						var cl = (int[])this.SupportData.api_support_hourai.api_cl_list;

                        this.Damages = new double[12];
						Array.Copy(dmg, this.Damages, dmg.Length);

                        this.Criticals = new int[12];
						Array.Copy(cl, this.Criticals, cl.Length);
					}
					break;
				default:
                    this.Damages = new double[12];
                    this.Criticals = new int[12];
					break;
			}
		}


		public override bool IsAvailable => this.SupportFlag != 0;

		public override void EmulateBattle(int[] hps, int[] damages)
		{

			if (!this.IsAvailable) return;

			for (int i = 0; i < this.Battle.Initial.EnemyMembers.Length; i++)
			{
				if (this.Battle.Initial.EnemyMembers[i] > 0)
				{
					var index = new BattleIndex(BattleSides.EnemyMain, i);
                    this.BattleDetails.Add(new BattleSupportDetail(this.Battle, index, this.Damages[i], this.Criticals[i], this.SupportFlag, hps[index]));
                    this.AddDamage(hps, index, (int)this.Damages[i]);
				}
			}
			if (this.Battle.IsEnemyCombined)
			{
				for (int i = 0; i < this.Battle.Initial.EnemyMembersEscort.Length; i++)
				{
					if (this.Battle.Initial.EnemyMembersEscort[i] > 0)
					{
						var index = new BattleIndex(BattleSides.EnemyEscort, i);
                        this.BattleDetails.Add(new BattleSupportDetail(this.Battle, index, this.Damages[i + 6], this.Criticals[i + 6], this.SupportFlag, hps[index]));
                        this.AddDamage(hps, index, (int)this.Damages[i + 6]);
					}
				}
			}
		}

		protected override IEnumerable<BattleDetail> SearchBattleDetails(int index)
		{
			return this.BattleDetails.Where(d => d.DefenderIndex == index);
		}


		/// <summary>
		/// 支援艦隊フラグ
		/// </summary>
		public int SupportFlag
		{
			get
			{
				if (this.IsNight)
					return this.RawData.api_n_support_flag() ? (int)this.RawData.api_n_support_flag : 0;
				else
					return this.RawData.api_support_flag() ? (int)this.RawData.api_support_flag : 0;
			}
		}

		public dynamic SupportData => this.IsNight ? this.RawData.api_n_support_info : this.RawData.api_support_info;

		/// <summary>
		/// 支援艦隊ID
		/// </summary>
		public int SupportFleetID
		{
			get
			{
				switch (this.SupportFlag)
				{
					case 1:
					case 4:
						return (int)this.SupportData.api_support_airatack.api_deck_id;

					case 2:
					case 3:
						return (int)this.SupportData.api_support_hourai.api_deck_id;

					default:
						return -1;

				}
			}
		}

		/// <summary>
		/// 支援艦隊
		/// </summary>
		public FleetData SupportFleet
		{
			get
			{
				int id = this.SupportFleetID;
				if (id != -1)
					return KCDatabase.Instance.Fleet[id];
				else
					return null;
			}
		}


		/// <summary>
		/// 与ダメージ [12]
		/// </summary>
		public double[] Damages { get; private set; }

		/// <summary>
		/// クリティカルフラグ [12]
		/// </summary>
		public int[] Criticals { get; private set; }


	}
}
