using ElectronicObserver.Data.Battle.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Phase
{

	/// <summary>
	/// 雷撃戦フェーズの処理を行います。
	/// </summary>
	public class PhaseTorpedo : PhaseBase
	{

		/// <summary>
		/// フェーズID 0=開幕雷撃, 1-4=雷撃戦
		/// </summary>
		private readonly int phaseID;

		public PhaseTorpedo(BattleData data, string title, int phaseID)
			: base(data, title)
		{

			this.phaseID = phaseID;

			if (!this.IsAvailable)
				return;

            this.Damages = this.GetConcatArray("api_fdam", "api_edam", 0.0);
            this.AttackDamages = this.GetConcatArray("api_fydam", "api_eydam", 0);
            this.Targets = this.GetConcatArray("api_frai", "api_erai", -1);
            this.CriticalFlags = this.GetConcatArray("api_fcl", "api_ecl", 0);

		}


		public override bool IsAvailable
		{
			get
			{
				if (this.phaseID == 0)
				{
					return this.RawData.api_opening_flag() ? (int)this.RawData.api_opening_flag != 0 : false;

				}
				else
				{
					return (int)this.RawData.api_hourai_flag[this.phaseID - 1] != 0;
				}
			}
		}


		public override void EmulateBattle(int[] hps, int[] damages)
		{

			if (!this.IsAvailable)
				return;

			// 表示上は逐次ダメージ反映のほうが都合がいいが、AddDamage を逐次的にやるとダメコン判定を誤るため
			int[] currentHP = new int[hps.Length];
			Array.Copy(hps, currentHP, currentHP.Length);

			for (int i = 0; i < this.Targets.Length; i++)
			{
				if (this.Targets[i] >= 0)
				{
					BattleIndex attacker = new BattleIndex(i, this.Battle.IsFriendCombined, this.Battle.IsEnemyCombined);
					BattleIndex defender = new BattleIndex(this.Targets[i] + (i < 12 ? 12 : 0), this.Battle.IsFriendCombined, this.Battle.IsEnemyCombined);

                    this.BattleDetails.Add(new BattleDayDetail(this.Battle, attacker, defender, new double[] { this.AttackDamages[i] + this.Damages[defender] - Math.Floor(this.Damages[defender]) },    //propagates "guards flagship" flag
						new int[] { this.CriticalFlags[i] }, -1, null, currentHP[defender]));
					currentHP[defender] -= Math.Max(this.AttackDamages[i], 0);
				}
			}

			for (int i = 0; i < hps.Length; i++)
			{
                this.AddDamage(hps, i, (int)this.Damages[i]);
				damages[i] += this.AttackDamages[i];
			}

		}


		public dynamic TorpedoData => this.phaseID == 0 ? this.RawData.api_opening_atack : this.RawData.api_raigeki;


		/// <summary>
		/// 各艦の被ダメージ
		/// </summary>
		public double[] Damages { get; private set; }

		/// <summary>
		/// 各艦の与ダメージ
		/// </summary>
		public int[] AttackDamages { get; private set; }

		/// <summary>
		/// 各艦のターゲットインデックス
		/// </summary>
		public int[] Targets { get; private set; }

		/// <summary>
		/// クリティカルフラグ(攻撃側)
		/// </summary>
		public int[] CriticalFlags { get; private set; }




		private T[] GetConcatArray<T>(string friendName, string enemyName, T defaultValue)
		{
			var friend = (T[])this.TorpedoData[friendName];
			var enemy = (T[])this.TorpedoData[enemyName];

			var ret = new T[24];

			for (int i = 0; i < 12; i++)
			{
				if (i < friend.Length)
					ret[i] = friend[i];
				else
					ret[i] = defaultValue;

				if (i < enemy.Length)
					ret[i + 12] = enemy[i];
				else
					ret[i + 12] = defaultValue;
			}

			return ret;
		}
	}
}
