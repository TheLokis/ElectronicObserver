using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{
	using DSPair = KeyValuePair<double, string>;

	/// <summary>
	/// 任務「あ号作戦」の進捗を管理します。
	/// </summary>
	[DataContract(Name = "ProgressAGo")]
	public class ProgressAGo : ProgressData
	{

		/// <summary>
		/// 達成に必要な出撃回数
		/// </summary>
		[IgnoreDataMember]
		private int sortieMax => 36;

		/// <summary>
		/// 達成に必要なS勝利回数
		/// </summary>
		[IgnoreDataMember]
		private int sWinMax => 6;

		/// <summary>
		/// 達成に必要なボス戦闘回数
		/// </summary>
		[IgnoreDataMember]
		private int bossMax => 24;

		/// <summary>
		/// 達成に必要なボス勝利回数
		/// </summary>
		[IgnoreDataMember]
		private int bossWinMax => 12;


		/// <summary>
		/// 現在の出撃回数
		/// </summary>
		[IgnoreDataMember]
		private int sortieCount
		{
			get { return this.Progress & 0xFF; }
			set { this.Progress = (this.Progress & ~0xFF) | (Math.Min(value, this.sortieMax) & 0xFF); }
		}

		/// <summary>
		/// 現在のS勝利回数
		/// </summary>
		[IgnoreDataMember]
		private int sWinCount
		{
			get { return (this.Progress >> 8) & 0xFF; }
			set { this.Progress = (this.Progress & ~(0xFF << 8)) | ((Math.Min(value, this.sWinMax) & 0xFF) << 8); }
		}

		/// <summary>
		/// 現在のボス戦闘回数
		/// </summary>
		[IgnoreDataMember]
		private int bossCount
		{
			get { return (this.Progress >> 16) & 0xFF; }
			set { this.Progress = (this.Progress & ~(0xFF << 16)) | ((Math.Min(value, this.bossMax) & 0xFF) << 16); }
		}

		/// <summary>
		/// 現在のボス勝利回数
		/// </summary>
		[IgnoreDataMember]
		private int bossWinCount
		{
			get { return (this.Progress >> 24) & 0xFF; }
			set { this.Progress = (this.Progress & ~(0xFF << 24)) | ((Math.Min(value, this.bossWinMax) & 0xFF) << 24); }
		}


		#region tempシリーズ

		/// <summary>
		/// 現在の出撃回数(temp)
		/// </summary>
		[IgnoreDataMember]
		private int sortieCountTemp
		{
			get { return this.TemporaryProgress & 0xFF; }
			set { this.TemporaryProgress = (this.TemporaryProgress & ~0xFF) | (Math.Min(value, this.sortieMax) & 0xFF); }
		}

		/// <summary>
		/// 現在のS勝利回数(temp)
		/// </summary>
		[IgnoreDataMember]
		private int sWinCountTemp
		{
			get { return (this.TemporaryProgress >> 8) & 0xFF; }
			set { this.TemporaryProgress = (this.TemporaryProgress & ~(0xFF << 8)) | ((Math.Min(value, this.sWinMax) & 0xFF) << 8); }
		}

		/// <summary>
		/// 現在のボス戦闘回数(temp)
		/// </summary>
		[IgnoreDataMember]
		private int bossCountTemp
		{
			get { return (this.TemporaryProgress >> 16) & 0xFF; }
			set { this.TemporaryProgress = (this.TemporaryProgress & ~(0xFF << 16)) | ((Math.Min(value, this.bossMax) & 0xFF) << 16); }
		}

		/// <summary>
		/// 現在のボス勝利回数(temp)
		/// </summary>
		[IgnoreDataMember]
		private int bossWinCountTemp
		{
			get { return (this.TemporaryProgress >> 24) & 0xFF; }
			set { this.TemporaryProgress = (this.TemporaryProgress & ~(0xFF << 24)) | ((Math.Min(value, this.bossWinMax) & 0xFF) << 24); }
		}

		#endregion


		public ProgressAGo(QuestData quest)
			: base(quest, 0)
		{
		}


		public override double ProgressPercentage
		{
			get
			{
				double prog = 0;
				prog += Math.Min((double)this.sortieCount / this.sortieMax, 1.0) * 0.25;
				prog += Math.Min((double)this.sWinCount / this.sWinMax, 1.0) * 0.25;
				prog += Math.Min((double)this.bossCount / this.bossMax, 1.0) * 0.25;
				prog += Math.Min((double)this.bossWinCount / this.bossWinMax, 1.0) * 0.25;
				return prog;
			}
		}



		public override void Increment()
		{
			throw new NotSupportedException();
		}

		public override void Decrement()
		{
			throw new NotSupportedException();
		}


		public override void CheckProgress(QuestData q)
		{

			if (this.TemporaryProgress != 0)
			{
				if (q.State == 2)
				{

                    this.sortieCount = this.sortieCount + this.sortieCountTemp;
                    this.sWinCount = this.sWinCount + this.sWinCountTemp;
                    this.bossCount = this.bossCount + this.bossCountTemp;
                    this.bossWinCount = this.bossWinCount + this.bossWinCountTemp;

				}

                this.TemporaryProgress = 0;
			}

		}


		/// <summary>
		/// 出撃回数を増やします。
		/// </summary>
		public void IncrementSortie()
		{

			var q = KCDatabase.Instance.Quest[this.QuestID];

			if (q == null)
			{
                this.sortieCountTemp++;
				return;
			}

			if (q.State != 2)
				return;


            this.CheckProgress(q);

            this.sortieCount++;
		}

		/// <summary>
		/// 戦闘回数を増やします。
		/// </summary>
		public void IncrementBattle(string rank, bool isBoss)
		{

			var q = KCDatabase.Instance.Quest[this.QuestID];

			if (q != null)
			{
				if (q.State != 2)
					return;
				else
                    this.CheckProgress(q);
			}


			int irank = Constants.GetWinRank(rank);

			if (isBoss)
			{
				if (q != null) this.bossCount++; else this.bossCountTemp++;

				if (irank >= Constants.GetWinRank("B"))
					if (q != null) this.bossWinCount++; else this.bossWinCountTemp++;
			}

			if (irank >= Constants.GetWinRank("S"))
				if (q != null) this.sWinCount++; else this.sWinCountTemp++;

		}


		public override string ToString()
		{
			var list = new List<DSPair>
			{
				new DSPair(Math.Min((double)this.sortieCount / this.sortieMax, 1.0), string.Format("출격 {0}/{1}", this.sortieCount, this.sortieMax)),
				new DSPair(Math.Min((double)this.sWinCount / this.sWinMax, 1.0), string.Format(" S승리 {0}/{1}", this.sWinCount, this.sWinMax)),
				new DSPair(Math.Min((double)this.bossCount / this.bossMax, 1.0), string.Format(" 보스 {0}/{1}", this.bossCount, this.bossMax)),
				new DSPair(Math.Min((double)this.bossWinCount / this.bossWinMax, 1.0), string.Format(" 보스승리 {0}/{1}", this.bossWinCount, this.bossWinMax))
			};

			var slist = list.Where(elem => elem.Key < 1.0).OrderBy(elem => elem.Key).Select(elem => elem.Value);
            return string.Format("{0} ({1:p1})", slist.Count() > 0 ? string.Join(", ", slist) : "달성", this.ProgressPercentage);
        }

		public override string GetClearCondition()
		{
			return string.Format("출격 {0}, S승리 {1}, 보스 {2}, 보스승리 {3}", this.sortieMax, this.sWinMax, this.bossMax, this.bossWinMax);
		}
	}

}
