using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Control
{

	public partial class ShipStatusLevel : UserControl
	{

		private const TextFormatFlags TextFormatValue = TextFormatFlags.NoPadding | TextFormatFlags.Bottom | TextFormatFlags.Right;
		private const TextFormatFlags TextFormatText = TextFormatFlags.NoPadding | TextFormatFlags.Bottom | TextFormatFlags.Left;



		#region Property

		private int _value;
		[Browsable(true), Category("Data"), DefaultValue(0)]
		[Description("現在のレベルです。")]
		public int Value
		{
			get { return this._value; }
			set
			{
                this._value = value;
                this._valueSizeCache = null;
                this.PropertyChanged();
			}
		}

		private int _maximumValue;
		[Browsable(true), Category("Data"), DefaultValue(0)]
		[Description("レベルの最大値です。")]
		public int MaximumValue
		{
			get { return this._maximumValue; }
			set
			{
                this._maximumValue = value;
                this._valueSizeCache = null;
                this.PropertyChanged();
			}
		}

		private int _valueNext;
		[Browsable(true), Category("Data"), DefaultValue(0)]
		[Description("次のレベルアップに必要な経験値です。")]
		public int ValueNext
		{
			get { return this._valueNext; }
			set
			{
                this._valueNext = value;
                this._valueNextSizeCache = null;
                this.PropertyChanged();
			}
		}

		private int _maximumValueNext;
		[Browsable(true), Category("Data"), DefaultValue(0)]
		[Description("次のレベルアップに必要な経験値の最大値です。")]
		public int MaximumValueNext
		{
			get { return this._maximumValueNext; }
			set
			{
                this._maximumValueNext = value;
                this._valueNextSizeCache = null;
                this.PropertyChanged();
			}
		}

		private Color _mainFontColor;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 0, 0")]
		[Description("主要テキストの色を指定します。")]
		public Color MainFontColor
		{
			get { return this._mainFontColor; }
			set
			{
                this._mainFontColor = value;
                this.PropertyChanged();
			}
		}

		private Color _subFontColor;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "68, 68, 68")]
		[Description("補助テキストの色を指定します。")]
		public Color SubFontColor
		{
			get { return this._subFontColor; }
			set
			{
                this._subFontColor = value;
                this.PropertyChanged();
			}
		}

		private Font _mainFont;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Font), "Meiryo UI, 12px")]
		[Description("主要テキストのフォントを指定します。")]
		public Font MainFont
		{
			get { return this._mainFont; }
			set
			{
                this._mainFont = value;
                this._valueSizeCache = null;
                this.PropertyChanged();
			}
		}

		private Font _subFont;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Font), "Meiryo UI, 10px")]
		[Description("補助テキストの色を指定します。")]
		public Font SubFont
		{
			get { return this._subFont; }
			set
			{
                this._subFont = value;
                this._textSizeCache =
                this._valueNextSizeCache =
                this._textNextSizeCache = null;
                this.PropertyChanged();
			}
		}


		private string _text;
		[Browsable(true), Category("Appearance"), DefaultValue("Lv.")]
		[Description("レベルの説明文となるテキストを指定します。")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Bindable(BindableSupport.Default)]
		public override string Text
		{
			get { return this._text; }
			set
			{
                this._text = value;
                this._textSizeCache = null;
                this.PropertyChanged();
			}
		}

		private string _textNext;
		[Browsable(true), Category("Appearance"), DefaultValue("next:")]
		[Description("次のレベルアップに必要な経験値の説明文となるテキストを指定します。")]
		public string TextNext
		{
			get { return this._textNext; }
			set
			{
                this._textNext = value;
                this._textNextSizeCache = null;
                this._valueNextSizeCache = null;
                this.PropertyChanged();
			}
		}


		// size cache

		private static readonly Size MaxSize = new Size(int.MaxValue, int.MaxValue);

		private Size? _textSizeCache;
		private Size TextSizeCache
		{
			get
			{
				return this._textSizeCache ??
					(this._textSizeCache = TextRenderer.MeasureText(this.Text, this.SubFont, MaxSize, TextFormatText) - new Size(!string.IsNullOrEmpty(this.Text) ? (int)(this.SubFont.Size / 2.0) : 0, 0)).Value;
			}
		}

		private Size? _valueSizeCache;
		private Size ValueSizeCache
		{
			get
			{
				return this._valueSizeCache ??
					(this._valueSizeCache = TextRenderer.MeasureText(Math.Max(this.Value, this.MaximumValue).ToString(), this.MainFont, MaxSize, TextFormatValue) - new Size((int)(this.MainFont.Size / 2.0), 0)).Value;
			}
		}

		private Size? _textNextSizeCache;
		private Size TextNextSizeCache
		{
			get
			{
				return this._textNextSizeCache ??
					(this._textNextSizeCache = this.TextNext == null ?
						Size.Empty :
						(TextRenderer.MeasureText(this.TextNext, this.SubFont, MaxSize, TextFormatText) - new Size(!string.IsNullOrEmpty(this.TextNext) ? (int)(this.SubFont.Size / 2.0) : 0, 0))).Value;
			}
		}

		private Size? _valueNextSizeCache;
		private Size ValueNextSizeCache
		{
			get
			{
				return this._valueNextSizeCache ??
					(this._valueNextSizeCache = this.TextNext == null ?
						Size.Empty :
						(TextRenderer.MeasureText(Math.Max(this.ValueNext, this.MaximumValueNext).ToString(), this.SubFont, MaxSize, TextFormatText) - new Size((int)(this.MainFont.Size / 2.0), 0))).Value;
			}
		}


		private int InnerHorizontalMargin => 4;
		#endregion




		public ShipStatusLevel()
		{
            this.InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this._value = 0;
            this._maximumValue = 0;

            this._mainFontColor = Color.FromArgb(0x00, 0x00, 0x00);
            this._subFontColor = Color.FromArgb(0x44, 0x44, 0x44);
            this._mainFont = new Font("Meiryo UI", 12, FontStyle.Regular, GraphicsUnit.Pixel);
            this._subFont = new Font("Meiryo UI", 10, FontStyle.Regular, GraphicsUnit.Pixel);
            this._text = "Lv.";

            this._valueNext = 0;
            this._maximumValueNext = 0;
            this._textNext = "next:";

		}

		private void ShipStatusLevel_Paint(object sender, PaintEventArgs e)
		{

			Rectangle basearea = new Rectangle(this.Padding.Left, this.Padding.Top, this.Width - this.Padding.Horizontal, this.Height - this.Padding.Vertical);
			//e.Graphics.DrawRectangle( Pens.Magenta, new Rectangle( basearea.X, basearea.Y, basearea.Width - 1, basearea.Height - 1 ) );


			//*/

			//alignment.bottom 

			Point p = new Point(basearea.X, basearea.Bottom - this.TextSizeCache.Height);
			TextRenderer.DrawText(e.Graphics, this.Text, this.SubFont, new Rectangle(p, this.TextSizeCache), this.SubFontColor, TextFormatText);
			//e.Graphics.DrawRectangle( Pens.Orange, new Rectangle( p, TextSizeCache ) );

			p.X += this.TextSizeCache.Width;
			p.Y = basearea.Bottom - this.ValueSizeCache.Height;
			TextRenderer.DrawText(e.Graphics, this.Value.ToString(), this.MainFont, new Rectangle(p, this.ValueSizeCache), this.MainFontColor, TextFormatValue);
			//e.Graphics.DrawRectangle( Pens.Orange, new Rectangle( p, ValueSizeCache ) );


			p.X = basearea.Right - Math.Max(this.TextNextSizeCache.Width, this.ValueNextSizeCache.Width);
			p.Y = basearea.Bottom - this.TextNextSizeCache.Height - this.ValueNextSizeCache.Height + (int)(this.SubFont.Size / 2.0);
			if (this.TextNext != null)
			{
				TextRenderer.DrawText(e.Graphics, this.TextNext, this.SubFont, new Rectangle(p, this.TextNextSizeCache), this.SubFontColor, TextFormatText);
				//e.Graphics.DrawRectangle( Pens.Orange, new Rectangle( p, TextNextSizeCache ) );
			}

			p.Y = basearea.Bottom - this.ValueNextSizeCache.Height + 1;
			if (this.TextNext != null)
			{
				TextRenderer.DrawText(e.Graphics, this.ValueNext.ToString(), this.SubFont, new Rectangle(p, this.ValueNextSizeCache), this.SubFontColor, TextFormatText);
				//e.Graphics.DrawRectangle( Pens.Orange, new Rectangle( p, ValueNextSizeCache ) );
			}

		}



		public override Size GetPreferredSize(Size proposedSize)
		{

			return new Size(this.ValueSizeCache.Width + this.TextSizeCache.Width + (this.TextNext == null ? 0 : this.InnerHorizontalMargin) + Math.Max(this.TextNextSizeCache.Width, this.ValueNextSizeCache.Width) + this.Padding.Horizontal,
				Math.Max(this.ValueSizeCache.Height, this.TextNextSizeCache.Height + this.ValueNextSizeCache.Height - (int)(this.SubFont.Size / 2.0)) + this.Padding.Vertical - 1);

		}


		private void PropertyChanged()
		{
			if (this.AutoSize)
                this.Size = this.GetPreferredSize(this.Size);

            this.Refresh();
		}

        private void ShipStatusLevel_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText($"{this._text} {this._value} {this._textNext} {this._valueNext}");
        }
    }

}
