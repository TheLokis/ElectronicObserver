using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Design;

namespace ElectronicObserver.Window.Control
{

	/// <summary>
	/// 画像付きの Label です。
	/// </summary>
	public partial class ImageLabel : Label
	{

		private Size _imageSize = new Size(16, 16);
		[Browsable(true), DefaultValue(typeof(Size), "16, 16"), Category("Appearance")]
		[Description("イメージのサイズを指定します。")]
		public Size ImageSize
		{
			get
			{
				if (this.Image != null)
					return this.Image.Size;
				else
					return this._imageSize;
			}
			set
			{
                this._imageSize = value;
                this.AdjustSize();
                this.Invalidate();
			}
		}

		private int _imageMargin = 3;
		[Browsable(true), DefaultValue(3), Category("Appearance")]
		[Description("イメージとラベルの間のスペースを指定します。")]
		public int ImageMargin
		{
			get { return this._imageMargin; }
			set
			{
                this._imageMargin = value;
                this.AdjustSize();
                this.Invalidate();
			}
		}

		private bool _autoWrap = false;
		[Browsable(true), DefaultValue(false), Category("Behavior")]
		[Description("幅を超えるテキストを折り返すかを指定します。")]
		public bool AutoWrap
		{
			get { return this._autoWrap; }
			set
			{
                this._autoWrap = value;
                this._measureTextCache = null;
                this.Invalidate();
			}
		}

		[DefaultValue(false)]
		public new bool AutoEllipsis
		{
			get { return base.AutoEllipsis; }
			set
			{
                this._measureTextCache = null;
				base.AutoEllipsis = value;
			}
		}

		[DefaultValue(ContentAlignment.MiddleLeft)]
		public override ContentAlignment TextAlign
		{
			get { return base.TextAlign; }
			set
			{
                this._measureTextCache = null;
				base.TextAlign = value;
			}
		}

		[DefaultValue(ContentAlignment.MiddleLeft)]
		public new ContentAlignment ImageAlign
		{
			get { return base.ImageAlign; }
			set
			{
				base.ImageAlign = value;
                this.AdjustSize();
			}
		}

		[DefaultValue(false)]
		public new bool UseMnemonic
		{
			get { return base.UseMnemonic; }
			set
			{
                this._measureTextCache = null;
				base.UseMnemonic = value;
                this.AdjustSize();
			}
		}

		[DefaultValue(null)]
		public new Image Image
		{
			get { return base.Image; }
			set
			{
				base.Image = value;
                this.AdjustSize();
			}
		}

		[DefaultValue(true)]
		public new bool AutoSize
		{
			get { return base.AutoSize; }
			set
			{
				bool checkon = !base.AutoSize && value;
				base.AutoSize = value;
				if (checkon)
                    this.AdjustSize();
			}
		}

		[DefaultValue(typeof(Padding), "3, 3, 3, 3")]
		public new Padding Margin
		{
			get { return base.Margin; }
			set { base.Margin = value; }
		}

		public new BorderStyle BorderStyle
		{
			get { return base.BorderStyle; }
			set
			{
				base.BorderStyle = value;
                this.AdjustSize();
			}
		}

		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set
			{
				base.MinimumSize = value;
                this.AdjustSize();
			}
		}

		public override Size MaximumSize
		{
			get { return base.MaximumSize; }
			set
			{
				base.MaximumSize = value;
                this.AdjustSize();
			}
		}



		private Size? _measureTextCache = null;
		private Size MeasureTextCache => this._measureTextCache ?? (Size)(this._measureTextCache = TextRenderer.MeasureText(this.Text, this.Font, new Size(int.MaxValue, int.MaxValue), this.GetTextFormat()));


		private Size? _preferredSizeCache = null;
		private Size PreferredSizeCache => this._preferredSizeCache ?? (Size)(this._preferredSizeCache = this.GetPreferredSize(this.Size));



		public ImageLabel()
			: base()
		{
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.UseMnemonic = false;
            this.AutoSize = true;
            this.Margin = new Padding(3);
		}


		protected override void OnTextChanged(EventArgs e)
		{
            this._measureTextCache = null;
            this.AdjustSize();
			base.OnTextChanged(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
            this._measureTextCache = null;
            this.AdjustSize();
			base.OnFontChanged(e);
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
            this.AdjustSize();
			base.OnPaddingChanged(e);
		}



		public void AdjustSize()
		{
			if (this.AutoSize)
			{
                this._preferredSizeCache = null;
                this.Size = this.PreferredSizeCache;
			}
		}

		public Size GetPreferredSize()
		{

			if (this._preferredSizeCache != null)
				return (Size)this.PreferredSizeCache;

			// size - clientsize は border の調整用
			Size ret = new Size(this.Padding.Horizontal, this.Padding.Vertical) + (this.Size - this.ClientSize);

			Size sz_text = this.MeasureTextCache;

			if (!string.IsNullOrEmpty(this.Text))
				sz_text.Width -= (int)(this.Font.Size / 2);

			switch (this.ImageAlign)
			{
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft:
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					ret.Width += this.ImageSize.Width + this.ImageMargin + sz_text.Width;
					ret.Height += Math.Max(this.ImageSize.Height, sz_text.Height);
					break;

				case ContentAlignment.TopCenter:
				case ContentAlignment.BottomCenter:
					ret.Width += Math.Max(this.ImageSize.Width, sz_text.Width);
					ret.Height += this.ImageSize.Height + this.ImageMargin + sz_text.Height;
					break;

				case ContentAlignment.MiddleCenter:
					ret.Width += Math.Max(this.ImageSize.Width, sz_text.Width);
					ret.Height += Math.Max(this.ImageSize.Height, sz_text.Height);
					break;

			}

			return ret;
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			var size = this.GetPreferredSize();
			if (!this.MaximumSize.IsEmpty)
			{
				size.Width = Math.Min(this.MaximumSize.Width, size.Width);
				size.Height = Math.Min(this.MaximumSize.Height, size.Height);
			}
			if (!this.MinimumSize.IsEmpty)
			{
				size.Width = Math.Max(this.MinimumSize.Width, size.Width);
				size.Height = Math.Max(this.MinimumSize.Height, size.Height);
			}
			return size;
		}


		private TextFormatFlags GetTextFormat()
		{

			TextFormatFlags textformat = TextFormatFlags.NoPadding;

			if (this.AutoWrap)
				textformat |= TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;
			if (this.AutoEllipsis)
				textformat |= TextFormatFlags.EndEllipsis;
			if (!this.UseMnemonic)
				textformat |= TextFormatFlags.NoPrefix;

			switch (this.TextAlign)
			{
				case ContentAlignment.TopLeft:
					textformat |= TextFormatFlags.Top | TextFormatFlags.Left; break;
				case ContentAlignment.TopCenter:
					textformat |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter; break;
				case ContentAlignment.TopRight:
					textformat |= TextFormatFlags.Top | TextFormatFlags.Right; break;
				case ContentAlignment.MiddleLeft:
					textformat |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left; break;
				case ContentAlignment.MiddleCenter:
					textformat |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter; break;
				case ContentAlignment.MiddleRight:
					textformat |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right; break;
				case ContentAlignment.BottomLeft:
					textformat |= TextFormatFlags.Bottom | TextFormatFlags.Left; break;
				case ContentAlignment.BottomCenter:
					textformat |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter; break;
				case ContentAlignment.BottomRight:
					textformat |= TextFormatFlags.Bottom | TextFormatFlags.Right; break;
			}

			return textformat;
		}


		private Rectangle GetTextArea(Rectangle area)
		{

			switch (this.ImageAlign)
			{
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft:
					area.X += this.ImageSize.Width + this.ImageMargin;
					area.Width -= this.ImageSize.Width + this.ImageMargin;
					break;

				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					area.Width -= this.ImageSize.Width + this.ImageMargin;
					break;

				case ContentAlignment.TopCenter:
					area.Y += this.ImageSize.Height + this.ImageMargin;
					area.Height -= this.ImageSize.Height + this.ImageMargin;
					break;
				case ContentAlignment.BottomCenter:
					area.Height -= this.ImageSize.Height + this.ImageMargin;
					break;

				case ContentAlignment.MiddleCenter:
					break;
			}
			return area;
		}


		protected override void OnPaint(PaintEventArgs e)
		{

			Rectangle basearea = new Rectangle(this.Padding.Left, this.Padding.Top, this.ClientSize.Width - this.Padding.Horizontal, this.ClientSize.Height - this.Padding.Vertical);
			//e.Graphics.DrawRectangle( Pens.Magenta, basearea.X, basearea.Y, basearea.Width - 1, basearea.Height - 1 );

			Rectangle imagearea = new Rectangle(basearea.X, basearea.Y, this.ImageSize.Width, this.ImageSize.Height);

			switch (this.ImageAlign)
			{
				case ContentAlignment.TopLeft:
					break;
				case ContentAlignment.MiddleLeft:
					imagearea.Y += (basearea.Height - imagearea.Height) / 2;
					break;
				case ContentAlignment.BottomLeft:
					imagearea.Y = basearea.Bottom - imagearea.Height;
					break;

				case ContentAlignment.TopCenter:
					imagearea.X += (basearea.Width - imagearea.Width) / 2;
					break;
				case ContentAlignment.MiddleCenter:
					imagearea.X += (basearea.Width - imagearea.Width) / 2;
					imagearea.Y += (basearea.Height - imagearea.Height) / 2;
					break;
				case ContentAlignment.BottomCenter:
					imagearea.X += (basearea.Width - imagearea.Width) / 2;
					imagearea.Y = basearea.Bottom - imagearea.Height;
					break;

				case ContentAlignment.TopRight:
					imagearea.X = basearea.Right - imagearea.Width;
					break;
				case ContentAlignment.MiddleRight:
					imagearea.X = basearea.Right - imagearea.Width;
					imagearea.Y += (basearea.Height - imagearea.Height) / 2;
					break;
				case ContentAlignment.BottomRight:
					imagearea.X = basearea.Right - imagearea.Width;
					imagearea.Y = basearea.Bottom - imagearea.Height;
					break;
			}


			if (this.Image != null)
			{
				if (this.Enabled)
					e.Graphics.DrawImage(this.Image, imagearea);
				else
					ControlPaint.DrawImageDisabled(e.Graphics, this.Image, imagearea.X, imagearea.Y, this.BackColor);

				//e.Graphics.DrawRectangle( Pens.Orange, imagearea.X, imagearea.Y, imagearea.Width - 1, imagearea.Height - 1 );
			}


			Color textcolor;
			if (this.Enabled)
			{
				textcolor = this.ForeColor;
			}
			else
			{
				if (this.BackColor.GetBrightness() < SystemColors.Control.GetBrightness())
					textcolor = ControlPaint.Dark(this.BackColor);
				else
					textcolor = SystemColors.ControlDark;
			}

			var textarea = this.GetTextArea(basearea);
			//e.Graphics.DrawRectangle( Pens.Orange, textrect.X, textrect.Y, textrect.Width - 1, textrect.Height - 1 );
			TextRenderer.DrawText(e.Graphics, this.Text, this.Font, textarea, textcolor, this.GetTextFormat());

			//base.OnPaint( e );
		}

	}
}
