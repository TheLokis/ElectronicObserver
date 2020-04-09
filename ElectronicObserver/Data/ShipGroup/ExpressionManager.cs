using ElectronicObserver.Utility.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.ShipGroup
{


	[DataContract(Name = "ExpressionManager")]
	public sealed class ExpressionManager : DataStorage, ICloneable
	{

		[DataMember]
		public List<ExpressionList> Expressions { get; set; }

		[IgnoreDataMember]
		private Expression<Func<ShipData, bool>> predicate;

		[IgnoreDataMember]
		private Expression expression;


		public ExpressionManager() : base()
		{
            this.Initialize();
		}

		public override void Initialize()
		{
            this.Expressions = new List<ExpressionList>();
            this.predicate = null;
            this.expression = null;
		}


		public ExpressionList this[int index]
		{
			get { return this.Expressions[index]; }
			set { this.Expressions[index] = value; }
		}


		public void Compile()
		{
			Expression ex = null;
			var paramex = Expression.Parameter(typeof(ShipData), "ship");

			foreach (var exlist in this.Expressions)
			{
				if (!exlist.Enabled)
					continue;

				if (ex == null)
				{
					ex = exlist.Compile(paramex);

				}
				else
				{
					if (exlist.ExternalAnd)
					{
						ex = Expression.AndAlso(ex, exlist.Compile(paramex));
					}
					else
					{
						ex = Expression.OrElse(ex, exlist.Compile(paramex));
					}
				}
			}


			if (ex == null)
			{
				ex = Expression.Constant(true, typeof(bool));       //:-P
			}

            this.predicate = Expression.Lambda<Func<ShipData, bool>>(ex, paramex);
            this.expression = ex;

		}


		public IEnumerable<ShipData> GetResult(IEnumerable<ShipData> list)
		{

			if (this.predicate == null)
				throw new InvalidOperationException("식이 컴파일 되지 않았습니다.");

			return list.AsQueryable().Where(this.predicate).AsEnumerable();
		}

		public bool IsAvailable => this.predicate != null;



		public override string ToString()
		{

			if (this.Expressions == null)
				return "(없음)";

			StringBuilder sb = new StringBuilder();
			foreach (var ex in this.Expressions)
			{
				if (!ex.Enabled)
					continue;
				else if (sb.Length == 0)
					sb.Append(ex.ToString());
				else
					sb.AppendFormat(" {0} {1}", ex.ExternalAnd ? "하고" : "또는", ex.ToString());
			}

			if (sb.Length == 0)
				sb.Append("(없음)");
			return sb.ToString();
		}

		public string ToExpressionString()
		{
			return this.expression.ToString();
		}



		public ExpressionManager Clone()
		{
			var clone = (ExpressionManager)this.MemberwiseClone();
			clone.Expressions = this.Expressions?.Select(e => e.Clone()).ToList();
			clone.predicate = null;
			clone.expression = null;
			return clone;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}


	}

}
