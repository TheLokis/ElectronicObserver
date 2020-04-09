using ElectronicObserver.Data.ShipGroup;
using ElectronicObserver.Utility.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ElectronicObserver.Data
{


	/// <summary>
	/// 艦船グループのデータを保持します。
	/// </summary>
	[DataContract(Name = "ShipGroupData")]
	[DebuggerDisplay("[{GroupID}] : {Name} ({Members.Count} ships)")]
	public sealed class ShipGroupData : DataStorage, IIdentifiable, ICloneable
	{


		/// <summary>
		/// 列のプロパティを保持します。
		/// </summary>
		[DataContract(Name = "ViewColumnData")]
		public class ViewColumnData : ICloneable
		{

			/// <summary>
			/// 列名
			/// </summary>
			[DataMember]
			public string Name { get; set; }

			/// <summary>
			/// 幅
			/// </summary>
			[DataMember]
			public int Width { get; set; }

			/// <summary>
			/// 表示される順番
			/// </summary>
			[DataMember]
			public int DisplayIndex { get; set; }

			/// <summary>
			/// 可視かどうか
			/// </summary>
			[DataMember]
			public bool Visible { get; set; }

			/// <summary>
			/// 自動幅調整を行うか
			/// </summary>
			[DataMember]
			public bool AutoSize { get; set; }



			public ViewColumnData(string name)
			{
                this.Name = name;
			}

			public ViewColumnData(string name, int width, int displayIndex, bool visible, bool autoSize)
			{
                this.Name = name;
                this.Width = width;
                this.DisplayIndex = displayIndex;
                this.Visible = visible;
                this.AutoSize = autoSize;
			}

			public ViewColumnData(DataGridViewColumn column)
			{
                this.FromColumn(column);
			}


			/// <summary>
			/// 現在の設定を、列に対して適用します。
			/// </summary>
			/// <param name="column">対象となる列。</param>
			public void ToColumn(DataGridViewColumn column)
			{
				if (column.Name != this.Name)
					throw new ArgumentException("설정 열과 이름이 다릅니다.");

				column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;      //width 変更のためいったん戻す
				column.Width = this.Width;
				column.DisplayIndex = this.DisplayIndex;
				column.Visible = this.Visible;
				column.AutoSizeMode = this.AutoSize ? DataGridViewAutoSizeColumnMode.AllCellsExceptHeader : DataGridViewAutoSizeColumnMode.NotSet;

			}

			/// <summary>
			/// 現在の列の状態から、設定を生成します。
			/// </summary>
			/// <param name="column">対象となる列。</param>
			/// <returns>このインスタンス自身を返します。</returns>
			public ViewColumnData FromColumn(DataGridViewColumn column)
			{
                this.Name = column.Name;
                this.Width = column.Width;
                this.DisplayIndex = column.DisplayIndex;
                this.Visible = column.Visible;
                this.AutoSize = column.AutoSizeMode == DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
				return this;
			}


			public ViewColumnData Clone()
			{
				return (ViewColumnData)this.MemberwiseClone();
			}

			object ICloneable.Clone()
			{
				return this.Clone();
			}
		}




		/// <summary>
		/// グループID
		/// </summary>
		[DataMember]
		public int GroupID { get; internal set; }


		/// <summary>
		/// グループ名
		/// </summary>
		[DataMember]
		public string Name { get; set; }


		/// <summary>
		/// 列の設定
		/// </summary>
		[IgnoreDataMember]
		public Dictionary<string, ViewColumnData> ViewColumns { get; set; }

		[DataMember]
		private IEnumerable<ViewColumnData> ViewColumnsSerializer
		{
			get { return this.ViewColumns.Values; }
			set { this.ViewColumns = value.ToDictionary(v => v.Name); }
		}


		/// <summary>
		/// ロックされる列数(左端から)
		/// </summary>
		[DataMember]
		public int ScrollLockColumnCount { get; set; }


		/// <summary>
		/// 自動ソートの順番
		/// </summary>
		[IgnoreDataMember]
		public List<KeyValuePair<string, ListSortDirection>> SortOrder { get; set; }

		[DataMember]
		private List<SerializableKeyValuePair<string, ListSortDirection>> SortOrderSerializer
		{
			get { return this.SortOrder?.Select(s => new SerializableKeyValuePair<string, ListSortDirection>(s)).ToList(); }
			set { this.SortOrder = value?.Select(s => new KeyValuePair<string, ListSortDirection>(s.Key, s.Value)).ToList(); }
		}


		/// <summary>
		/// 自動ソートを行うか
		/// </summary>
		[DataMember]
		public bool AutoSortEnabled { get; set; }


		/// <summary>
		/// フィルタデータ
		/// </summary>
		[DataMember]
		public ExpressionManager Expressions { get; set; }


		/// <summary>
		/// 包含フィルタ
		/// </summary>
		[IgnoreDataMember]
		public List<int> InclusionFilter { get; set; }

		[DataMember]
		private SerializableList<int> InclusionFilterSerializer
		{
			get { return this.InclusionFilter; }
			set { this.InclusionFilter = value; }
		}

		/// <summary>
		/// 除外フィルタ
		/// </summary>
		[IgnoreDataMember]
		public List<int> ExclusionFilter { get; set; }

		[DataMember]
		private SerializableList<int> ExclusionFilterSerializer
		{
			get { return this.ExclusionFilter; }
			set { this.ExclusionFilter = value; }
		}



		/// <summary>
		/// 艦船IDリスト
		/// </summary>
		[IgnoreDataMember]
		public List<int> Members { get; private set; }

		/// <summary>
		/// 艦船リスト
		/// </summary>
		[IgnoreDataMember]
		public IEnumerable<ShipData> MembersInstance => this.Members.Select(id => KCDatabase.Instance.Ships[id]);


		[DataMember]
		private SerializableList<int> MembersSerializer
		{
			get { return this.Members; }
			set { this.Members = value; }
		}



		public ShipGroupData(int groupID)
			: base()
		{
            this.Initialize();
            this.GroupID = groupID;
		}

		public override void Initialize()
		{
            this.GroupID = -1;
            this.ViewColumns = new Dictionary<string, ViewColumnData>();
            this.Name = "no title";
            this.ScrollLockColumnCount = 0;
            this.AutoSortEnabled = true;
            this.SortOrder = new List<KeyValuePair<string, ListSortDirection>>();
            this.Expressions = new ExpressionManager();
            this.InclusionFilter = new List<int>();
            this.ExclusionFilter = new List<int>();
            this.Members = new List<int>();
		}


		/// <summary>
		/// フィルタに基づいて検索を実行し、Members に結果をセットします。
		/// </summary>
		/// <param name="previousOrder">直前の並び替え順。なるべくこの順番を維持するように結果が生成されます。null もしくは 要素数 0 の場合は適当に生成されます。</param>
		public void UpdateMembers(IEnumerable<int> previousOrder = null)
		{

			if (this.Expressions == null)
                this.Expressions = new ExpressionManager();

			if (this.InclusionFilter == null)
                this.InclusionFilter = new List<int>();

			if (this.ExclusionFilter == null)
                this.ExclusionFilter = new List<int>();

            this.ValidateFilter();


			if (!this.Expressions.IsAvailable)
                this.Expressions.Compile();

			var newdata = this.Expressions.GetResult(KCDatabase.Instance.Ships.Values).Select(s => s.MasterID).Union(this.InclusionFilter).Except(this.ExclusionFilter);

			IEnumerable<int> prev = (previousOrder != null && previousOrder.Count() > 0) ? previousOrder : (this.Members ?? new List<int>());

            // ソート順序を維持するため
            this.Members = prev.Except(prev.Except(newdata)).Union(newdata).ToList();
		}


		public void AddInclusionFilter(IEnumerable<int> list)
		{
            this.InclusionFilter = this.InclusionFilter.Union(list).ToList();
            this.ExclusionFilter = this.ExclusionFilter.Except(list).ToList();
		}

		public void AddExclusionFilter(IEnumerable<int> list)
		{
            this.InclusionFilter = this.InclusionFilter.Except(list).ToList();
            this.ExclusionFilter = this.ExclusionFilter.Union(list).ToList();
		}

		public void ValidateFilter()
		{
			if (KCDatabase.Instance.Ships.Count > 0)
			{
				var ships = KCDatabase.Instance.Ships.Keys;
                this.InclusionFilter = this.InclusionFilter.Intersect(ships).Distinct().ToList();
                this.ExclusionFilter = this.ExclusionFilter.Intersect(ships).Distinct().ToList();
			}
		}


		public int ID => this.GroupID;


		public override string ToString() => this.Name;




		/// <summary>
		/// このオブジェクトの複製(ディープ コピー)を作成します。
		/// </summary>
		/// <remarks>複製したオブジェクトのIDは必ず -1 になります。適宜再設定してください。</remarks>
		public ShipGroupData Clone()
		{
			var clone = (ShipGroupData)this.MemberwiseClone();
			clone.GroupID = -1;
			clone.ViewColumns = this.ViewColumns.Select(p => p.Value.Clone()).ToDictionary(p => p.Name);
			clone.SortOrder = new List<KeyValuePair<string, ListSortDirection>>(this.SortOrder);
			clone.Expressions = this.Expressions.Clone();
			clone.InclusionFilter = new List<int>(this.InclusionFilter);
			clone.ExclusionFilter = new List<int>(this.ExclusionFilter);
			clone.Members = new List<int>(this.Members);

			return clone;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}



	}

}
