using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{

	/// <summary>
	/// 戦闘系の任務の進捗を管理します。
	/// </summary>
	[DataContract(Name = "ProgressBattle")]
	public class ProgressBattle : ProgressData
	{

		/// <summary>
		/// 条件を満たす最低ランク
		/// </summary>
		[DataMember]
		private int LowestRank { get; set; }

		/// <summary>
		/// 対象となる海域
		/// </summary>
		[DataMember]
		private HashSet<int> TargetArea { get; set; }

		/// <summary>
		/// ボス限定かどうか
		/// </summary>
		[DataMember]
		private bool IsBossOnly { get; set; }


		public ProgressBattle(QuestData quest, int maxCount, string lowestRank, int[] targetArea, bool isBossOnly)
			: base(quest, maxCount)
		{

            this.LowestRank = Constants.GetWinRank(lowestRank);
            this.TargetArea = targetArea == null ? null : new HashSet<int>(targetArea);
            this.IsBossOnly = isBossOnly;
		}



        public virtual void Increment(string rank, int areaID, bool isBoss)
        {

            if (this.TargetArea != null && !this.TargetArea.Contains(areaID))
				return;

			if (Constants.GetWinRank(rank) < this.LowestRank)
				return;

			if (this.IsBossOnly && !isBoss)
				return;


            this.Increment();
		}



		public override string GetClearCondition()
		{
			StringBuilder sb = new StringBuilder();
			if (this.TargetArea != null)
			{
				sb.Append(string.Join("・", this.TargetArea.OrderBy(s => s).Select(s => string.Format("{0}-{1}", s / 10, s % 10))));
			}
			if (this.IsBossOnly)
				sb.Append(" 보스 ");
			switch (this.LowestRank)
			{
                case 0:
                    sb.Append("전투 ");
                    break;
                case 1:
				default:
					sb.Append("전투 ");
					break;
				case 2:
				case 3:
					sb.Append(Constants.GetWinRank(this.LowestRank) + "이상 ");
					break;
				case 4:
					sb.Append("승리 ");
					break;
				case 5:
				case 6:
				case 7:
					sb.Append(Constants.GetWinRank(this.LowestRank) + "승리 ");
					break;
			}
			sb.Append(this.ProgressMax);
            sb.Append(" 회");

			return sb.ToString();
		}
	}

}
