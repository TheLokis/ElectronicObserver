using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{

	/// <summary>
	/// 遠征の進捗を管理します。
	/// </summary>
	[DataContract(Name = "ProgressExpedition")]
	public class ProgressExpedition : ProgressData
	{

		/// <summary>
		/// 対象となる海域
		/// </summary>
		[DataMember]
		private HashSet<int> TargetArea { get; set; }


		public ProgressExpedition(QuestData quest, int maxCount, int[] targetArea)
			: base(quest, maxCount)
		{

            this.TargetArea = targetArea == null ? null : new HashSet<int>(targetArea);
		}


		public void Increment(int areaID)
		{

			if (this.TargetArea != null && !this.TargetArea.Contains(areaID))
				return;

            this.Increment();
		}


		public override string GetClearCondition()
		{
			StringBuilder sb = new StringBuilder();
			if (this.TargetArea != null)
			{
				sb.Append(string.Join("・", this.TargetArea.OrderBy(s => s).Select(s => KCDatabase.Instance.Mission[s].Name)));
			}
			else
			{
				sb.Append("원정 ");
			}
			sb.Append(this.ProgressMax);

			return sb.ToString();
		}
	}
}
