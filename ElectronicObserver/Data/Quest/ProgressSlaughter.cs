using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{

	/// <summary>
	/// 特定艦種撃沈任務の進捗を管理します。
	/// </summary>
	[DataContract(Name = "ProgressSlaughter")]
	public class ProgressSlaughter : ProgressData
	{

		/// <summary>
		/// 対象となる艦種リスト
		/// 互換性維持のため enum ではなく int で管理する
		/// </summary>
		[DataMember]
		private HashSet<int> TargetShipType { get; set; }

		public ProgressSlaughter(QuestData quest, int maxCount, int[] targetShipType)
			: base(quest, maxCount)
		{

            this.TargetShipType = targetShipType == null ? null : new HashSet<int>(targetShipType);

		}


		public void Increment(ShipTypes shipType)
		{
			if (this.TargetShipType.Contains((int)shipType))
                this.Increment();
		}


		public override string GetClearCondition()
		{
			StringBuilder sb = new StringBuilder();
			if (this.TargetShipType != null)
			{
				sb.Append(string.Join("・", this.TargetShipType.OrderBy(s => s).Select(s => KCDatabase.Instance.ShipTypes[s].Name)));
			}

			sb.Append("격침 ");
			sb.Append(this.ProgressMax);
            sb.Append("회");

			return sb.ToString();
		}
	}
}
