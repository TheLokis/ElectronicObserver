using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

	/// <summary>
	/// 戦闘情報を保持するデータの基底です。
	/// </summary>
	public abstract class BattleData : ResponseWrapper
	{

		protected int[] _resultHPs;
		/// <summary>
		/// 戦闘終了時の各艦のHP
		/// </summary>
		public ReadOnlyCollection<int> ResultHPs => Array.AsReadOnly(this._resultHPs);

		protected int[] _attackDamages;
		/// <summary>
		/// 各艦の与ダメージ
		/// </summary>
		public ReadOnlyCollection<int> AttackDamages => Array.AsReadOnly(this._attackDamages);


		public PhaseInitial Initial { get; protected set; }
		public PhaseSearching Searching { get; protected set; }
		public PhaseSupport Support { get; protected set; }


		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            this.Initial = new PhaseInitial(this, "전력");
            this.Searching = new PhaseSearching(this, "색적");

            this._resultHPs = new int[24];
			Array.Copy(this.Initial.FriendInitialHPs, 0, this._resultHPs, 0, this.Initial.FriendInitialHPs.Length);
			Array.Copy(this.Initial.EnemyInitialHPs, 0, this._resultHPs, 12, this.Initial.EnemyInitialHPs.Length);
			if (this.Initial.FriendInitialHPsEscort != null)
				Array.Copy(this.Initial.FriendInitialHPsEscort, 0, this._resultHPs, 6, 6);
			if (this.Initial.EnemyInitialHPsEscort != null)
				Array.Copy(this.Initial.EnemyInitialHPsEscort, 0, this._resultHPs, 18, 6);



			if (this._attackDamages == null)
                this._attackDamages = new int[this._resultHPs.Length];
		}


		/// <summary>
		/// MVP 取得候補艦のインデックス [0-6]
		/// </summary>
		public IEnumerable<int> MVPShipIndexes
		{
			get
			{
				int memberCount = this.Initial.FriendFleet.Members.Count;
				int max = this._attackDamages.Take(memberCount).Max();
				if (max == 0)
				{       // 全員ノーダメージなら旗艦MVP
					yield return 0;

				}
				else
				{
					for (int i = 0; i < memberCount; i++)
					{
						if (this._attackDamages[i] == max)
							yield return i;
					}
				}
			}
		}


		/// <summary>
		/// 連合艦隊随伴艦隊の MVP 取得候補艦のインデックス [0-5]
		/// </summary>
		public IEnumerable<int> MVPShipCombinedIndexes
		{
			get
			{
				int max = this._attackDamages.Skip(6).Take(6).Max();
				if (max == 0)
				{       // 全員ノーダメージなら旗艦MVP
					yield return 0;

				}
				else
				{
					for (int i = 0; i < 6; i++)
					{
						if (this._attackDamages[i + 6] == max)
							yield return i;
					}
				}
			}
		}


		/// <summary>
		/// 前回の戦闘データからパラメータを引き継ぎます。
		/// </summary>
		internal void TakeOverParameters(BattleData prev)
		{
            this._attackDamages = (int[])prev._attackDamages.Clone();
		}



		/// <summary>
		/// 対応しているAPIの名前を取得します。
		/// </summary>
		public abstract string APIName { get; }

		/// <summary>
		/// 戦闘形態の名称
		/// </summary>
		public abstract string BattleName { get; }


		public virtual bool IsPractice => false;
		public virtual bool IsFriendCombined => this.Initial.IsFriendCombined;
		public virtual bool IsEnemyCombined => this.Initial.IsEnemyCombined;
		public virtual bool IsBaseAirRaid => false;



		/// <summary>
		/// すべての戦闘詳細データを取得します。
		/// </summary>
		public string GetBattleDetail()
		{
			return this.GetBattleDetail(-1);
		}

		/// <summary>
		/// 指定したインデックスの艦の戦闘詳細データを取得します。
		/// </summary>
		/// <param name="index">インデックス。[0-23]</param>
		public string GetBattleDetail(int index)
		{
			var sb = new StringBuilder();

			foreach (var phase in this.GetPhases())
			{
				string bd = phase.GetBattleDetail(index);

				if (!string.IsNullOrEmpty(bd))
				{
					sb.AppendLine("《" + phase.Title + "》").Append(bd);
				}
			}
			return sb.ToString();
		}


		public abstract IEnumerable<PhaseBase> GetPhases();

	}

}
