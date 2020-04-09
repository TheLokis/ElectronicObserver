using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Window.Control
{

	/// <summary>
	/// 現在値と最大値を視覚的に表現するバー
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StatusBarModule
	{


		public event EventHandler PropertyChanged = delegate { };


		#region Property

		private int _value = 66;
		/// <summary>
		/// 現在値
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(66)]
		[Description("現在値を指定します。")]
		public int Value
		{
			get { return this._value; }
			set
			{
                this._value = value;
				if (!this._usePrevValue)
                    this._prevValue = this._value;
                this.Refresh();
			}
		}

		private int _prevValue = 88;
		/// <summary>
		/// 直前の現在値
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(88)]
		[Description("直前の値を指定します。")]
		public int PrevValue
		{
			get { return this._prevValue; }
			set
			{
				if (this._usePrevValue)
                    this._prevValue = value;
                this.Refresh();
			}
		}

		private int _maximumValue = 100;
		/// <summary>
		/// 最大値
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(100)]
		[Description("最大値を指定します。")]
		public int MaximumValue
		{
			get { return this._maximumValue; }
			set
			{
                this._maximumValue = value;
                this.Refresh();
			}
		}

		private bool _usePrevValue = true;
		/// <summary>
		/// 直前の値を利用するかどうか
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(true)]
		[Description("直前の値を利用するかを指定します。")]
		public bool UsePrevValue
		{
			get { return this._usePrevValue; }
			set
			{
                this._usePrevValue = value;
				if (!this._usePrevValue)
                    this._prevValue = this._value;
                this.Refresh();
			}
		}


		private Color _barColor0Begin = FromArgb(0xFFFF0000);
		/// <summary>
		/// バーの色(0%～25% - 始点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 0, 0")]
		[Description("0%～25%エリアの始点の色を指定します。")]
		public Color BarColor0Begin
		{
			get { return this._barColor0Begin; }
			set
			{
                this._barColor0Begin = value;
                this.Refresh();
			}
		}

		private Color _barColor0End = FromArgb(0xFFFF0000);
		/// <summary>
		/// バーの色(0%～25% - 終点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 0, 0")]
		[Description("0%～25%エリアの終点の色を指定します。")]
		public Color BarColor0End
		{
			get { return this._barColor0End; }
			set
			{
                this._barColor0End = value;
                this.Refresh();
			}
		}

		private Color _barColor1Begin = FromArgb(0xFFFF8800);
		/// <summary>
		/// バーの色(25%～50% - 始点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 136, 0")]
		[Description("25～50%エリアの始点の色を指定します。")]
		public Color BarColor1Begin
		{
			get { return this._barColor1Begin; }
			set
			{
                this._barColor1Begin = value;
                this.Refresh();
			}
		}

		private Color _barColor1End = FromArgb(0xFFFF8800);
		/// <summary>
		/// バーの色(25%～50% - 終点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 136, 0")]
		[Description("25～50%エリアの終点の色を指定します。")]
		public Color BarColor1End
		{
			get { return this._barColor1End; }
			set
			{
                this._barColor1End = value;
                this.Refresh();
			}
		}

		private Color _barColor2Begin = FromArgb(0xFFFFCC00);
		/// <summary>
		/// バーの色(50%～75% - 始点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 204, 0")]
		[Description("50～75%エリアの始点の色を指定します。")]
		public Color BarColor2Begin
		{
			get { return this._barColor2Begin; }
			set
			{
                this._barColor2Begin = value;
                this.Refresh();
			}
		}

		private Color _barColor2End = FromArgb(0xFFFFCC00);
		/// <summary>
		/// バーの色(50%～75% - 終点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 204, 0")]
		[Description("50～75%エリアの終点の色を指定します。")]
		public Color BarColor2End
		{
			get { return this._barColor2End; }
			set
			{
                this._barColor2End = value;
                this.Refresh();
			}
		}

		private Color _barColor3Begin = FromArgb(0xFF00CC00);
		/// <summary>
		/// バーの色(75%～100% - 始点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 204, 0")]
		[Description("75～100%エリアの始点の色を指定します。")]
		public Color BarColor3Begin
		{
			get { return this._barColor3Begin; }
			set
			{
                this._barColor3Begin = value;
                this.Refresh();
			}
		}

		private Color _barColor3End = FromArgb(0xFF00CC00);
		/// <summary>
		/// バーの色(75%～100% - 終点)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 204, 0")]
		[Description("75～100%エリアの終点の色を指定します。")]
		public Color BarColor3End
		{
			get { return this._barColor3End; }
			set
			{
                this._barColor3End = value;
                this.Refresh();
			}
		}

		private Color _barColor4 = FromArgb(0xFF0044CC);
		/// <summary>
		/// バーの色(100%)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 68, 204")]
		[Description("100%の時のバーの色を指定します。")]
		public Color BarColor4
		{
			get { return this._barColor4; }
			set
			{
                this._barColor4 = value;
                this.Refresh();
			}
		}

		private Color _barColorIncrement = FromArgb(0xFF44FF00);
		/// <summary>
		/// バーの色(増加分)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "68, 255, 0")]
		[Description("現在値が増加した時のバーの色を指定します。")]
		public Color BarColorIncrement
		{
			get { return this._barColorIncrement; }
			set
			{
                this._barColorIncrement = value;
                this.Refresh();
			}
		}

		private Color _barColorDecrement = FromArgb(0xFF882222);
		/// <summary>
		/// バーの色(減少分)
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "136, 34, 34")]
		[Description("現在値が減少した時のバーの色を指定します。")]
		public Color BarColorDecrement
		{
			get { return this._barColorDecrement; }
			set
			{
                this._barColorDecrement = value;
                this.Refresh();
			}
		}

		private Color _barColorBackground = FromArgb(0xFF888888);
		/// <summary>
		/// バーの背景色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "136, 136, 136")]
		[Description("バーの背景色を指定します。")]
		public Color BarColorBackground
		{
			get { return this._barColorBackground; }
			set
			{
                this._barColorBackground = value;
                this.Refresh();
			}
		}


		private int _barThickness = 4;
		/// <summary>
		/// バーの太さ
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(4)]
		[Description("バーの太さ(高さ)を指定します。")]
		public int BarThickness
		{
			get { return this._barThickness; }
			set
			{
                this._barThickness = value;
                this.Refresh();
			}
		}

		private int _barBackgroundOffset = 1;
		/// <summary>
		/// バーの前景と背景とのずれの大きさ
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(1)]
		[Description("バーの前景と背景のずれの大きさを指定します。影のような表現に利用します。")]
		public int BarBackgroundOffset
		{
			get { return this._barBackgroundOffset; }
			set
			{
                this._barBackgroundOffset = value;
                this.Refresh();
			}
		}

		private bool _colorMorphing = false;
		[Browsable(true), Category("Appearance"), DefaultValue(false)]
		[Description("バーの色を割合に応じて滑らかに変化させるかを指定します。")]
		/// <summary>
		/// 色を滑らかに変化させるか
		/// </summary>
		public bool ColorMorphing
		{
			get { return this._colorMorphing; }
			set
			{
                this._colorMorphing = value;
                this.Refresh();
			}
		}


		#endregion



		public StatusBarModule()
		{

		}



		public Color[] GetBarColorScheme()
		{
			return new Color[] {
                this._barColor0Begin,
                this._barColor0End,
                this._barColor1Begin,
                this._barColor1End,
                this._barColor2Begin,
                this._barColor2End,
                this._barColor3Begin,
                this._barColor3End,
                this._barColor4,
                this._barColorIncrement,
                this._barColorDecrement,
                this._barColorBackground,
			};
		}

		public void SetBarColorScheme(Color[] colors)
		{

			if (colors.Length < 12)
				throw new ArgumentOutOfRangeException("colors 의 배열 길이가 충분하지않습니다.");


            this._barColor0Begin = colors[0];
            this._barColor0End = colors[1];
            this._barColor1Begin = colors[2];
            this._barColor1End = colors[3];
            this._barColor2Begin = colors[4];
            this._barColor2End = colors[5];
            this._barColor3Begin = colors[6];
            this._barColor3End = colors[7];
            this._barColor4 = colors[8];
            this._barColorIncrement = colors[9];
            this._barColorDecrement = colors[10];
            this._barColorBackground = colors[11];


            this.Refresh();
		}


		private static double GetPercentage(int value, int max)
		{
			if (max <= 0 || value <= 0)
				return 0.0;
			else if (value > max)
				return 1.0;
			else
				return (double)value / max;
		}


		private static Color FromArgb(uint color)
		{
			return Color.FromArgb(unchecked((int)color));
		}

		private static Color BlendColor(Color a, Color b, double weight)
		{

			if (weight < 0.0 || 1.0 < weight)
				throw new ArgumentOutOfRangeException("weight は 0.0 - 1.0 の範囲内でなければなりません。指定値: " + weight);

			return Color.FromArgb(
				(int)(a.A * (1 - weight) + b.A * weight),
				(int)(a.R * (1 - weight) + b.R * weight),
				(int)(a.G * (1 - weight) + b.G * weight),
				(int)(a.B * (1 - weight) + b.B * weight));
		}


		/// <summary>
		/// バーを描画します。
		/// </summary>
		/// <param name="g">描画するための Graphics。</param>
		/// <param name="rect">描画する領域。</param>
		public void Paint(Graphics g, Rectangle rect)
		{

			using (var b = new SolidBrush(this.BarColorBackground))
			{
				g.FillRectangle(b, new Rectangle(rect.X + this.BarBackgroundOffset, rect.Bottom - this.BarThickness, rect.Width - this.BarBackgroundOffset, this.BarThickness));
			}
			using (var b = new SolidBrush(this.Value > this.PrevValue ? this.BarColorIncrement : this.BarColorDecrement))
			{
				g.FillRectangle(b, new Rectangle(rect.X, rect.Bottom - this.BarThickness - this.BarBackgroundOffset,
					(int)Math.Ceiling((rect.Width - this.BarBackgroundOffset) * GetPercentage(Math.Max(this.Value, this.PrevValue), this.MaximumValue)), this.BarThickness));
			}

			Color barColor;
			double p = GetPercentage(this.Value, this.MaximumValue);

			if (!this.ColorMorphing)
			{

				if (p <= 0.25)
					barColor = this.BarColor0Begin;
				else if (p <= 0.50)
					barColor = this.BarColor1Begin;
				else if (p <= 0.75)
					barColor = this.BarColor2Begin;
				else if (p < 1.00)
					barColor = this.BarColor3Begin;
				else
					barColor = this.BarColor4;

			}
			else
			{

				if (p <= 0.25)
					barColor = BlendColor(this.BarColor0Begin, this.BarColor0End, p * 4.0);
				else if (p <= 0.50)
					barColor = BlendColor(this.BarColor1Begin, this.BarColor1End, (p - 0.25) * 4.0);
				else if (p <= 0.75)
					barColor = BlendColor(this.BarColor2Begin, this.BarColor2End, (p - 0.50) * 4.0);
				else if (p < 1.00)
					barColor = BlendColor(this.BarColor3Begin, this.BarColor3End, (p - 0.75) * 4.0);
				else
					barColor = this.BarColor4;

			}

			using (var b = new SolidBrush(barColor))
			{
				g.FillRectangle(b, new Rectangle(rect.X, rect.Bottom - this.BarThickness - this.BarBackgroundOffset,
					(int)Math.Ceiling((rect.Width - this.BarBackgroundOffset) * GetPercentage(Math.Min(this.Value, this.PrevValue), this.MaximumValue)), this.BarThickness));
			}

		}



		/// <summary>
		/// このモジュールを描画するために適切なサイズを取得します。
		/// </summary>
		public Size GetPreferredSize(Size proposedSize)
		{
			return new Size(Math.Max(proposedSize.Width, this.BarThickness + this.BarBackgroundOffset),
				Math.Max(proposedSize.Height, this.BarThickness + this.BarBackgroundOffset));
		}

		/// <summary>
		/// このモジュールを描画するために適切なサイズを取得します。
		/// </summary>
		public Size GetPreferredSize()
		{
			return this.GetPreferredSize(Size.Empty);
		}


		private void Refresh()
		{
			PropertyChanged(this, new EventArgs());
		}

	}

}
