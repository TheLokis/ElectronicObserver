using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{

	/// <summary>
	/// 演習任務の進捗を管理します。
	/// </summary>
	[DataContract(Name = "ProgressPractice")]
	public class ProgressPractice : ProgressData
	{

		/// <summary>
		/// 勝利のみカウントする
		/// </summary>
		[DataMember]
		private bool WinOnly { get; set; }


		public ProgressPractice(QuestData quest, int maxCount, bool winOnly)
			: base(quest, maxCount)
		{

            this.WinOnly = winOnly;
		}


		public void Increment(string rank)
		{

			if (this.WinOnly && Constants.GetWinRank(rank) < Constants.GetWinRank("B"))
				return;

            this.Increment();
		}


		public override string GetClearCondition()
		{
			return "연습" + (this.WinOnly ? "승리" : "") + this.ProgressMax + " 회";
		}
	}
}
