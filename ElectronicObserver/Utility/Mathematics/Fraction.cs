using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Utility.Mathematics
{

	public class Fraction
	{
		public int Current { get; set; }
		public int Max { get; set; }

		public double Rate => (double)this.Current / Math.Max(this.Max, 1);


		public Fraction()
		{
            this.Current = this.Max = 0;
		}

		public Fraction(int current, int max)
		{
            this.Current = current;
            this.Max = max;
		}

		public override string ToString() => $"{this.Current}/{this.Max}";

	}

}
