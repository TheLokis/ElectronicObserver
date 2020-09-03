using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Utility.Mathematics;

namespace ElectronicObserver.Window.Control
{
	public partial class ShipStatusHP : UserControl
	{


		private const TextFormatFlags TextFormatTime = TextFormatFlags.NoPadding | TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
		private const TextFormatFlags TextFormatText = TextFormatFlags.NoPadding | TextFormatFlags.Bottom | TextFormatFlags.Left;
		private const TextFormatFlags TextFormatHP = TextFormatFlags.NoPadding | TextFormatFlags.Bottom | TextFormatFlags.Right;

		private static readonly Size MaxSize = new Size(int.MaxValue, int.MaxValue);
		private const string SlashText = " / ";


		private StatusBarModule _HPBar;

		private bool _onMouse;

		#region Property

		[Browsable(true), Category("Data"), DefaultValue(66)]
		[Description("HPの現在値です。")]
		public int Value
		{
			get { return this._HPBar.Value; }
			set
			{
                this._HPBar.Value = value;
                this._valueSizeCache = this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}

		[Browsable(true), Category("Data"), DefaultValue(88)]
		[Description("以前のHPです。")]
		public int PrevValue
		{
			get { return this._HPBar.PrevValue; }
			set
			{
                this._HPBar.PrevValue = value;
                this.PropertyChanged();
			}
		}

		[Browsable(true), Category("Data"), DefaultValue(100)]
		[Description("HPの最大値です。")]
		public int MaximumValue
		{
			get { return this._HPBar.MaximumValue; }
			set
			{
                this._HPBar.MaximumValue = value;
                this._maximumValueSizeCache = this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}

		private DateTime _repairTime;
		[Browsable(true), Category("Data")]
		[Description("修復が完了する日時です。")]
		public DateTime RepairTime
		{
			get { return this._repairTime; }
			set
			{
                this._repairTime = value;
                this.PropertyChanged();
			}
		}

		private ShipStatusHPRepairTimeShowMode _repairTimeShowMode;
		[Browsable(true), Category("Data"), DefaultValue(null)]
		[Description("修復完了日時の表示方法を指定します。。")]
		public ShipStatusHPRepairTimeShowMode RepairTimeShowMode
		{
			get { return this._repairTimeShowMode; }
			set
			{
                this._repairTimeShowMode = value;
                this.PropertyChanged();
			}
		}


		private int _maximumDigit;
		[Browsable(true), Category("Data"), DefaultValue(999)]
		[Description("想定されるHPの最大値です。この値に応じてレイアウトされます。")]
		public int MaximumDigit
		{
			get { return this._maximumDigit; }
			set
			{
                this._maximumDigit = value;
                this._valueSizeCache =
                this._maximumValueSizeCache =
                this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}


		private Color _mainFontColor;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 0, 0")]
		[Description("主要テキストの色を指定します。")]
		public Color MainFontColor
		{
			get
			{
				return this._mainFontColor;
			}
			set
			{
                this._mainFontColor = value;
                this.PropertyChanged();
			}
		}

		private Color _subFontColor;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "136, 136, 136")]
		[Description("補助テキストの色を指定します。")]
		public Color SubFontColor
		{
			get
			{
				return this._subFontColor;
			}
			set
			{
                this._subFontColor = value;
                this.PropertyChanged();
			}
		}

		private Color _repairFontColor;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 0, 136")]
		[Description("修復時間テキストの色を指定します。")]
		public Color RepairFontColor
		{
			get
			{
				return this._repairFontColor;
			}
			set
			{
                this._repairFontColor = value;
                this.PropertyChanged();
			}
		}


		private Font _mainFont;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Font), "Meiryo UI, 12px")]
		[Description("主要テキストのフォントを指定します。")]
		public Font MainFont
		{
			get
			{
				return this._mainFont;
			}
			set
			{
                this._mainFont = value;
                this._valueSizeCache =
                this._repairTimeSizeCache =
                this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}

		private Font _subFont;
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Font), "Meiryo UI, 10px")]
		[Description("補助テキストのフォントを指定します。")]
		public Font SubFont
		{
			get
			{
				return this._subFont;
			}
			set
			{
                this._subFont = value;
                this._textSizeCache =
                this._maximumValueSizeCache =
                this._slashSizeCache =
                this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}


		private string _text;
		[Browsable(true), Category("Appearance"), DefaultValue("HP:")]
		[Description("説明文となるテキストを指定します。")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Bindable(BindableSupport.Default)]
		public override string Text
		{
			get { return this._text; }
			set
			{
                this._text = value;
                this._textSizeCache = this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}


		[Browsable(true), Category("Appearance"), DefaultValue(true)]
		[Description("以前の値との差分を表示するかを指定します。")]
		public bool UsePrevValue
		{
			get { return this.HPBar.UsePrevValue; }
			set
			{
                this.HPBar.UsePrevValue = value;
                this.PropertyChanged();
			}
		}


		private bool _showDifference;
		[Browsable(true), Category("Appearance"), DefaultValue(false)]
		[Description("最大値の代わりに以前の値を表示するかを指定します。")]
		public bool ShowDifference
		{
			get { return this._showDifference; }
			set
			{
                this._showDifference = value;
                this.PropertyChanged();
			}
		}

		private bool _showHPBar;
		[Browsable(true), Category("Appearance"), DefaultValue(true)]
		[Description("HPバーを表示するかを指定します。")]
		public bool ShowHPBar
		{
			get { return this._showHPBar; }
			set
			{
                this._showHPBar = value;
                this._preferredSizeCache = null;
                this.PropertyChanged();
			}
		}


		[Browsable(true), Category("Appearance")]
		[Description("HPバーへの参照です。")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public StatusBarModule HPBar
		{
			get { return this._HPBar; }
			set { this._HPBar = value; }
		}


		private bool IsRefreshSuspended { get; set; }

		public void SuspendUpdate()
		{
            this.IsRefreshSuspended = true;
		}
		public void ResumeUpdate()
		{
            this.IsRefreshSuspended = false;
            this.PropertyChanged();
		}


		// size cache

		private Size? _textSizeCache;
		private Size TextSizeCache
		{
			get
			{
				if (this._textSizeCache == null)
				{
                    this._textSizeCache = TextRenderer.MeasureText(this.Text, this.SubFont, MaxSize, TextFormatText) - new Size(!string.IsNullOrEmpty(this.Text) ? (int)(this.SubFont.Size / 2.0) : 0, 0);
                    this._preferredSizeCache = null;
				}
				return this._textSizeCache.Value;
			}
		}

		private Size? _valueSizeCache;
		private Size ValueSizeCache
		{
			get
			{
				if (this._valueSizeCache == null)
				{
                    this._valueSizeCache = TextRenderer.MeasureText(Math.Max(this.Value, this.MaximumDigit).ToString(), this.MainFont, MaxSize, TextFormatHP) - new Size((int)(this.MainFont.Size / 2.0), 0);
                    this._preferredSizeCache = null;
				}
				return this._valueSizeCache.Value;
			}
		}

		private Size? _maximumValueSizeCache;
		private Size MaximumValueSizeCache
		{
			get
			{
				if (this._maximumValueSizeCache == null)
				{
                    this._maximumValueSizeCache = TextRenderer.MeasureText(Math.Max(this.MaximumValue, this.MaximumDigit).ToString(), this.SubFont, MaxSize, TextFormatHP) - new Size((int)(this.SubFont.Size / 2.0), 0);
                    this._preferredSizeCache = null;
				}
				return this._maximumValueSizeCache.Value;
			}
		}

		private Size? _slashSizeCache;
		private Size SlashSizeCache
		{
			get
			{
				if (this._slashSizeCache == null)
				{
                    this._slashSizeCache = TextRenderer.MeasureText(SlashText, this.SubFont, MaxSize, TextFormatHP) - new Size((int)(this.SubFont.Size / 2.0), 0);
                    this._preferredSizeCache = null;
				}
				return this._slashSizeCache.Value;
			}
		}

		private Size? _repairTimeSizeCache;
		private Size RepairTimeSizeCache
		{
			get
			{
				if (this._repairTimeSizeCache == null)
				{
                    this._repairTimeSizeCache = TextRenderer.MeasureText(DateTimeHelper.ToTimeRemainString(TimeSpan.Zero), this.MainFont, MaxSize, TextFormatTime) - new Size((int)(this.MainFont.Size / 2.0), 0);
				}
				return this._repairTimeSizeCache.Value;
			}
		}

		private Size? _preferredSizeCache;

		#endregion




		public ShipStatusHP()
		{
            this.InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this._HPBar = new StatusBarModule
			{
				Value = 66,
				PrevValue = 88,
				MaximumValue = 100
			};
            this._repairTime = DateTime.Now;

            this._maximumDigit = 999;

            this._mainFont = new Font("Meiryo UI", 12, FontStyle.Regular, GraphicsUnit.Pixel);
            this._mainFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

            this._subFont = new Font("Meiryo UI", 10, FontStyle.Regular, GraphicsUnit.Pixel);
            this._subFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.SubFontColor);

            this._repairFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.RepairColor);
            this._text = "HP:";

            this._HPBar.UsePrevValue = true;
            this._showDifference = false;
            this._repairTimeShowMode = ShipStatusHPRepairTimeShowMode.Invisible;
            this._showHPBar = true;

		}


		private void ShipStatusHP_Paint(object sender, PaintEventArgs e)
		{

			Graphics g = e.Graphics;
			Rectangle basearea = new Rectangle(this.Padding.Left, this.Padding.Top, this.Width - this.Padding.Horizontal, this.Height - this.Padding.Vertical);
			Size barSize = this.ShowHPBar ? this._HPBar.GetPreferredSize(new Size(basearea.Width, 0)) : Size.Empty;



			if (this.RepairTimeShowMode == ShipStatusHPRepairTimeShowMode.Visible ||
				(this.RepairTimeShowMode == ShipStatusHPRepairTimeShowMode.MouseOver && this._onMouse))
			{
				string timestr = DateTimeHelper.ToTimeRemainString((DateTime)this.RepairTime);

				var rect = new Rectangle(basearea.X, basearea.Y, basearea.Width, basearea.Height - barSize.Height);
				Font font;
				if (rect.Width >= this.RepairTimeSizeCache.Width)
					font = this.MainFont;
				else
					font = this.SubFont;

				TextRenderer.DrawText(g, timestr, font, rect, this.RepairFontColor, TextFormatTime);

			}
			else
			{

				Point p = new Point(basearea.X, basearea.Bottom - barSize.Height - Math.Max(this.TextSizeCache.Height, this.MaximumValueSizeCache.Height) + 1);
				TextRenderer.DrawText(g, this.Text, this.SubFont, new Rectangle(p, this.TextSizeCache), this.SubFontColor, TextFormatText);
				//g.DrawRectangle( Pens.Orange, new Rectangle( p, TextSizeCache ) );

				p.X = basearea.Right - this.MaximumValueSizeCache.Width;
				TextRenderer.DrawText(g, !this.ShowDifference ? this.MaximumValue.ToString() : this.GetDifferenceString(), this.SubFont, new Rectangle(p, this.MaximumValueSizeCache), this.SubFontColor, TextFormatHP);
				//g.DrawRectangle( Pens.Orange, new Rectangle( p, MaximumValueSizeCache ) );

				p.X -= this.SlashSizeCache.Width;
				TextRenderer.DrawText(g, SlashText, this.SubFont, new Rectangle(p, this.SlashSizeCache), this.SubFontColor, TextFormatHP);
				//g.DrawRectangle( Pens.Orange, new Rectangle( p, SlashSizeCache ) );

				p.X -= this.ValueSizeCache.Width;
				p.Y = basearea.Bottom - barSize.Height - this.ValueSizeCache.Height + 1;
				TextRenderer.DrawText(g, Math.Max(this.Value, 0).ToString(), this.MainFont, new Rectangle(p, this.ValueSizeCache), this.MainFontColor, TextFormatHP);
				//g.DrawRectangle( Pens.Orange, new Rectangle( p, ValueSizeCache ) );

			}

			if (this.ShowHPBar)
                this._HPBar.Paint(g, new Rectangle(basearea.X, basearea.Bottom - barSize.Height, barSize.Width, barSize.Height));
		}



		public override Size GetPreferredSize(Size proposedSize)
		{

			if (this._preferredSizeCache == null)
			{

				Size barSize = this.ShowHPBar ? this._HPBar.GetPreferredSize() : Size.Empty;

                this._preferredSizeCache = new Size(
                    this.TextSizeCache.Width + this.ValueSizeCache.Width + this.SlashSizeCache.Width + this.MaximumValueSizeCache.Width + this.Padding.Horizontal,
					Math.Max(this.TextSizeCache.Height, this.ValueSizeCache.Height) + barSize.Height + this.Padding.Vertical);
			}

			return this._preferredSizeCache.Value;
		}



		private void PropertyChanged()
		{
			if (!this.IsRefreshSuspended)
			{
				if (this.AutoSize)
				{
					var size = this.GetPreferredSize(MaxSize);

					if (size != this.Size)
                        this.Size = size;
				}
                this.Refresh();
			}
		}


		private static Color FromArgb(uint color)
		{
			return Color.FromArgb(unchecked((int)color));
		}

		private string GetDifferenceString()
		{

			return (this.Value - this.PrevValue).ToString("+0;-0;-0");
		}

		private void ShipStatusHP_MouseEnter(object sender, EventArgs e)
		{
            this._onMouse = true;
			if (this.RepairTimeShowMode == ShipStatusHPRepairTimeShowMode.MouseOver)
                this.PropertyChanged();
		}

		private void ShipStatusHP_MouseLeave(object sender, EventArgs e)
		{
            this._onMouse = false;
			if (this.RepairTimeShowMode == ShipStatusHPRepairTimeShowMode.MouseOver)
                this.PropertyChanged();
		}

	}


	public enum ShipStatusHPRepairTimeShowMode
	{
		Invisible,
		MouseOver,
		Visible,
	}
}
