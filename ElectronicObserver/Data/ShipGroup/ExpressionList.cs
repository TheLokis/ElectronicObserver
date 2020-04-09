using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.ShipGroup
{

	[DataContract(Name = "ExpressionList")]
	public class ExpressionList : ICloneable
	{

		[DataMember]
		public List<ExpressionData> Expressions { get; set; }


		[DataMember]
		public bool InternalAnd { get; set; }

		[IgnoreDataMember]
		public bool InternalOr
		{
			get { return !this.InternalAnd; }
			set { this.InternalAnd = !value; }
		}


		[DataMember]
		public bool ExternalAnd { get; set; }

		[IgnoreDataMember]
		public bool ExternalOr
		{
			get { return !this.ExternalAnd; }
			set { this.ExternalAnd = !value; }
		}


		[DataMember]
		public bool Inverse { get; set; }

		[DataMember]
		public bool Enabled { get; set; }


		public ExpressionList()
		{
            this.Expressions = new List<ExpressionData>();
            this.InternalAnd = true;
            this.ExternalOr = true;
            this.Inverse = false;
            this.Enabled = true;
		}

		public ExpressionList(bool isInternalAnd, bool isExternalAnd, bool inverse)
		{
            this.Expressions = new List<ExpressionData>();
            this.InternalAnd = isInternalAnd;
            this.ExternalAnd = isExternalAnd;
            this.Inverse = inverse;
            this.Enabled = true;
		}


		public ExpressionData this[int index]
		{
			get { return this.Expressions[index]; }
			set { this.Expressions[index] = value; }
		}


		public Expression Compile(ParameterExpression paramex)
		{
			Expression ex = null;

			foreach (var exdata in this.Expressions)
			{
				if (!exdata.Enabled)
					continue;

				if (ex == null)
				{
					ex = exdata.Compile(paramex);

				}
				else
				{
					if (this.InternalAnd)
					{
						ex = Expression.AndAlso(ex, exdata.Compile(paramex));
					}
					else
					{
						ex = Expression.OrElse(ex, exdata.Compile(paramex));
					}
				}
			}

			if (ex == null)
				ex = Expression.Constant(true);

			if (this.Inverse)
				ex = Expression.Not(ex);

			return ex;
		}


		public override string ToString()
		{
			var exp = this.Expressions.Where(p => p.Enabled);
			return string.Format("({0}){1}", exp.Count() == 0 ? "없음" : string.Join(this.InternalAnd ? " 하고 " : " 또는 ", exp), this.Inverse ? " 을 충족하지 않음" : "");
		}



		public ExpressionList Clone()
		{
			var clone = (ExpressionList)this.MemberwiseClone();
			clone.Expressions = this.Expressions?.Select(e => e.Clone()).ToList();
			return clone;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}
