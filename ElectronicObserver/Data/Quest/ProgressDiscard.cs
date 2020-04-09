using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest
{

	/// <summary>
	/// 装備廃棄任務の進捗を管理します。
	/// </summary>
	[DataContract(Name = "ProgressDiscard")]
	public class ProgressDiscard : ProgressData
	{

		/// <summary>
		/// 廃棄した個数をベースにカウントするか
		/// false = 回数、true = 個数
		/// </summary>
		[DataMember]
		private bool CountsAmount { get; set; }

		/// <summary>
		/// 対象となる装備カテゴリ
		/// null ならすべての装備を対象とする
		/// </summary>
		[DataMember]
		private HashSet<int> Categories { get; set; }

        /// <summary>
        /// Categories の扱い
        /// -1=装備ID, 1=図鑑分類, 2=通常の装備カテゴリ, 3=アイコン
        /// </summary>
        [DataMember]
        protected int CategoryIndex { get; set; }

        public ProgressDiscard(QuestData quest, int maxCount, bool countsAmount, int[] categories)
            : this(quest, maxCount, countsAmount, categories, 2) 
        { 

        }

        public ProgressDiscard(QuestData quest, int maxCount, bool countsAmount, int[] categories, int categoryIndex)
            : base(quest, maxCount)
		{
            this.CountsAmount = countsAmount;
            this.Categories = categories == null ? null : new HashSet<int>(categories);
            this.CategoryIndex = categoryIndex;
        }

		public void Increment(IEnumerable<int> equipments)
		{
			if (!this.CountsAmount)
			{
                this.Increment();
				return;
			}

			if (this.Categories == null)
			{
				foreach (var i in equipments)
                    this.Increment();
				return;
			}

			foreach (var i in equipments)
			{
				var eq = KCDatabase.Instance.Equipments[i];

                switch (this.CategoryIndex)
                {
                    case -1:
                        if (this.Categories.Contains(eq.EquipmentID))
                            this.Increment();
                        break;
                    case 1:
                        if (this.Categories.Contains(eq.MasterEquipment.CardType))
                            this.Increment();
                        break;
                    case 2:
                        if (this.Categories.Contains((int)eq.MasterEquipment.CategoryType))
                            this.Increment();
                        break;
                    case 3:
                        if (this.Categories.Contains(eq.MasterEquipment.IconType))
                            this.Increment();
                        break;
                }
            }
		}



		public override string GetClearCondition()
		{
            return (this.Categories == null ? "" : string.Join("・", this.Categories.OrderBy(s => s).Select(s =>
            {
                switch (this.CategoryIndex)
                {
                    case -1:
                        return KCDatabase.Instance.MasterEquipments[s].Name;
                    case 1:
                        return $"도감 [{s}]";
                    case 2:
                        return KCDatabase.Instance.EquipmentTypes[s].Name;
                    case 3:
                        return $"아이콘 [{s}]";
                    default:
                        return $"???[{s}]";
                }
            }))) + "폐기 " + this.ProgressMax + (this.CountsAmount ? "개" : "회");
        }



        /// <summary>
        /// 互換性維持：デフォルト値の設定
        /// </summary>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.CategoryIndex = 2;
        }

	}
}
