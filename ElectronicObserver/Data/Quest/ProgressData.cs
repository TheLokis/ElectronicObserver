using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{

	/// <summary>
	/// 任務の進捗を管理する基底クラスです。
	/// </summary>
	[DataContract(Name = "ProgressData")]
	public abstract class ProgressData : IIdentifiable
	{

		/// <summary>
		/// 任務ID
		/// </summary>
		[DataMember]
		public int QuestID { get; protected set; }


		/// <summary>
		/// 進捗現在値
		/// </summary>
		[DataMember]
		public int Progress { get; protected set; }

		/// <summary>
		/// 進捗最大値
		/// </summary>
		[DataMember]
		public int ProgressMax { get; protected set; }

		/// <summary>
		/// 任務出現タイプ
		/// </summary>
		[DataMember]
		public int QuestType { get; protected set; }

		/// <summary>
		/// 未ロード時の進捗
		/// </summary>
		[DataMember]
		public int TemporaryProgress { get; protected set; }

		/// <summary>
		/// 共有カウンタの進捗ずれ
		/// 開発任務など、カウンタが共用になっている任務のずれ補正用です
		/// </summary>
		[DataMember]
		public int SharedCounterShift { get; set; }

		/// <summary>
		/// 加算/減算時に進捗カウンタ修正を行うか
		/// </summary>
		[DataMember]
		public bool IgnoreCheckProgress { get; set; }


		/// <summary>
		/// 進捗率
		/// </summary>
		[IgnoreDataMember]
		public virtual double ProgressPercentage => (double)this.Progress / this.ProgressMax;

		/// <summary>
		/// クリア済みかどうか
		/// </summary>
		[IgnoreDataMember]
		public bool IsCleared => this.ProgressPercentage >= 1.0;


		public ProgressData(QuestData quest, int maxCount)
		{
            this.QuestID = quest.QuestID;
            this.ProgressMax = maxCount;
            this.QuestType = quest.Type;
            this.TemporaryProgress = 0;
            this.SharedCounterShift = 0;
            this.IgnoreCheckProgress = false;
		}



		/// <summary>
		/// 進捗を1増やします。
		/// </summary>
		public virtual void Increment()
		{

			var q = KCDatabase.Instance.Quest[this.QuestID];

			if (q == null)
			{
                this.TemporaryProgress++;
				return;
			}

			if (q.State != 2)
				return;


			if (!this.IgnoreCheckProgress)
                this.CheckProgress(q);

            this.Progress = Math.Min(this.Progress + 1, this.ProgressMax);
		}

		/// <summary>
		/// 進捗を1減らします。
		/// </summary>
		public virtual void Decrement()
		{

			var q = KCDatabase.Instance.Quest[this.QuestID];

			if (q != null && q.State == 3)      //達成済なら無視
				return;


            this.Progress = Math.Max(this.Progress - 1, 0);

			if (!this.IgnoreCheckProgress)
                this.CheckProgress(q);
		}


		public override string ToString() => $"{this.Progress}/{this.ProgressMax}";



		/// <summary>
		/// 実際の進捗データから、進捗度を補正します。
		/// </summary>
		/// <param name="q">任務データ。</param>
		public virtual void CheckProgress(QuestData q)
		{

			if (this.TemporaryProgress > 0)
			{
				if (q.State == 2)
                    this.Progress = Math.Min(this.Progress + this.TemporaryProgress, this.ProgressMax);
                this.TemporaryProgress = 0;
			}

			if (this.QuestType == 0)     // ver. 1.6.6 以前のデータとの互換性維持
                this.QuestType = q.Type;

			switch (q.Progress)
			{
				case 1:     //50%
                    this.Progress = (int)Math.Max(this.Progress, Math.Ceiling((this.ProgressMax + this.SharedCounterShift) * 0.5) - this.SharedCounterShift);
					break;
				case 2:     //80%
                    this.Progress = (int)Math.Max(this.Progress, Math.Ceiling((this.ProgressMax + this.SharedCounterShift) * 0.8) - this.SharedCounterShift);
					break;
			}

		}


		/// <summary>
		/// この任務の達成に必要な条件を表す文字列を返します。
		/// </summary>
		/// <returns></returns>
		public abstract string GetClearCondition();

		[IgnoreDataMember]
		public int ID => this.QuestID;
	}

}
