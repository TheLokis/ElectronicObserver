using ElectronicObserver.Notifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{

	/// <summary>
	/// 通知ダイアログ
	/// </summary>
	public partial class DialogNotifier : Form
	{


		public NotifierDialogData DialogData { get; set; }


		private bool IsLayeredWindow => this.DialogData != null ? !this.DialogData.HasFormBorder && this.DialogData.DrawsImage : false;

		protected override bool ShowWithoutActivation => !this.DialogData.ShowWithActivation;



		public DialogNotifier(NotifierDialogData data)
		{

            this.InitializeComponent();

            this.DialogData = data.Clone();

            this.Text = this.DialogData.Title;
            this.Font = Utility.Configuration.Config.UI.MainFont;
            this.Icon = Resource.ResourceManager.Instance.AppIcon;
            this.Padding = new Padding(4);

            //SetStyle( ControlStyles.UserPaint, true );
            //SetStyle( ControlStyles.SupportsTransparentBackColor, true );
            this.ForeColor = this.DialogData.ForeColor;
            this.BackColor = this.DialogData.BackColor;

			if (this.DialogData.DrawsImage && this.DialogData.Image != null)
			{
                this.ClientSize = this.DialogData.Image.Size;
			}

			if (!this.DialogData.HasFormBorder)
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

			data.CloseAll += this.data_CloseAll;

		}




		private void DialogNotifier_Load(object sender, EventArgs e)
		{


			Rectangle screen = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
			switch (this.DialogData.Alignment)
			{

				case NotifierDialogAlignment.TopLeft:
                    this.Location = new Point(screen.X, screen.Y);
					break;
				case NotifierDialogAlignment.TopCenter:
                    this.Location = new Point(screen.X + (screen.Width - this.Width) / 2, screen.Y);
					break;
				case NotifierDialogAlignment.TopRight:
                    this.Location = new Point(screen.Right - this.Width, screen.Y);
					break;
				case NotifierDialogAlignment.MiddleLeft:
                    this.Location = new Point(screen.X, screen.Y + (screen.Height - this.Height) / 2);
					break;
				case NotifierDialogAlignment.MiddleCenter:
                    this.Location = new Point(screen.X + (screen.Width - this.Width) / 2, screen.Y + (screen.Height - this.Height) / 2);
					break;
				case NotifierDialogAlignment.MiddleRight:
                    this.Location = new Point(screen.Right - this.Width, screen.Y + (screen.Height - this.Height) / 2);
					break;
				case NotifierDialogAlignment.BottomLeft:
                    this.Location = new Point(screen.X, screen.Bottom - this.Height);
					break;
				case NotifierDialogAlignment.BottomCenter:
                    this.Location = new Point(screen.X + (screen.Width - this.Width) / 2, screen.Bottom - this.Height);
					break;
				case NotifierDialogAlignment.BottomRight:
                    this.Location = new Point(screen.Right - this.Width, screen.Bottom - this.Height);
					break;
				case NotifierDialogAlignment.Custom:
				case NotifierDialogAlignment.CustomRelative:
                    this.Location = new Point(this.DialogData.Location.X, this.DialogData.Location.Y);
					break;

			}

			if (this.IsLayeredWindow)
			{

				Size size = this.DialogData.Image?.Size ?? new Size(300, 100);

				// メッセージを書き込んだうえでレイヤードウィンドウ化する
				using (var bmp = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
				{

					using (var g = Graphics.FromImage(bmp))
					{

						g.Clear(Color.FromArgb(0, 0, 0, 0));
						g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
						g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

						if (this.DialogData.Image != null)
							g.DrawImage(this.DialogData.Image, new Rectangle(0, 0, bmp.Width, bmp.Height));
						else
							g.Clear(this.DialogData.BackColor);
						//DrawMessage( g );

						//*/
						if (this.DialogData.DrawsMessage)
						{

							// fixme: どうしても滑らかにフォントが描画できなかったので超絶苦肉の策

							using (var path = new GraphicsPath())
							{

								path.AddString(this.DialogData.Message, this.Font.FontFamily, (int)this.Font.Style, this.Font.Size, new RectangleF(this.Padding.Left, this.Padding.Top, this.ClientSize.Width - this.Padding.Horizontal, this.ClientSize.Height - this.Padding.Vertical), StringFormat.GenericDefault);

								using (var brush = new SolidBrush(this.ForeColor))
								{
									g.FillPath(brush, path);
								}
							}
						}
						//*/
					}

                    this.SetLayeredWindow(bmp);

				}
			}

			if (this.DialogData.ClosingInterval > 0)
			{
                this.CloseTimer.Interval = this.DialogData.ClosingInterval;
                this.CloseTimer.Start();
			}
		}


		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				if (this.DialogData != null && this.DialogData.TopMost)
					cp.ExStyle |= 0x8;      //set topmost flag
				if (this.IsLayeredWindow)
					cp.ExStyle |= 0x80000;  //set layered window flag
				return cp;
			}
		}


		private void DialogNotifier_Paint(object sender, PaintEventArgs e)
		{

			if (this.IsLayeredWindow) return;

			Graphics g = e.Graphics;
			g.Clear(this.BackColor);

			try
			{

				if (this.DialogData.DrawsImage && this.DialogData.Image != null)
				{

					g.DrawImage(this.DialogData.Image, new Rectangle(0, 0, this.DialogData.Image.Width, this.DialogData.Image.Height));
				}

                this.DrawMessage(g);

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "알림 시스템: 대화 상자에서 이미지 출력에 실패했습니다.");
			}
		}


		private void DrawMessage(Graphics g)
		{
			if (this.DialogData.DrawsMessage)
			{

				TextRenderer.DrawText(g, this.DialogData.Message, this.Font, new Rectangle(this.Padding.Left, this.Padding.Top, this.ClientSize.Width - this.Padding.Horizontal, this.ClientSize.Height - this.Padding.Vertical), this.ForeColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
			}
		}

		private void DialogNotifier_MouseClick(object sender, MouseEventArgs e)
		{

			var flag = this.DialogData.ClickFlag;

			if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
			{
				if ((flag & NotifierDialogClickFlags.Left) != 0 ||
				   ((flag & NotifierDialogClickFlags.LeftDouble) != 0 && e.Clicks > 1))
				{
                    this.Close();
					return;
				}
			}

			if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
			{
				if ((flag & NotifierDialogClickFlags.Right) != 0 ||
				   ((flag & NotifierDialogClickFlags.RightDouble) != 0 && e.Clicks > 1))
				{
                    this.Close();
					return;
				}
			}

			if ((e.Button & System.Windows.Forms.MouseButtons.Middle) != 0)
			{
				if ((flag & NotifierDialogClickFlags.Middle) != 0 ||
				   ((flag & NotifierDialogClickFlags.MiddleDouble) != 0 && e.Clicks > 1))
				{
                    this.Close();
					return;
				}
			}

		}

		private void DialogNotifier_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                this.Close();
		}

		private void DialogNotifier_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.DialogData.CloseOnMouseMove)
			{
                this.Close();
			}
		}

		private void CloseTimer_Tick(object sender, EventArgs e)
		{
            this.Close();
		}

		void data_CloseAll(object sender, EventArgs e)
		{
            this.Close();
		}


		// 以下 レイヤードウィンドウ用の呪文

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DeleteObject(IntPtr hobject);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DeleteDC(IntPtr hdc);

		public const byte AC_SRC_OVER = 0;
		public const byte AC_SRC_ALPHA = 1;
		public const int ULW_ALPHA = 2;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int UpdateLayeredWindow(
			IntPtr hwnd,
			IntPtr hdcDst,
			[System.Runtime.InteropServices.In()]
			ref Point pptDst,
			[System.Runtime.InteropServices.In()]
			ref Size psize,
			IntPtr hdcSrc,
			[System.Runtime.InteropServices.In()]
			ref Point pptSrc,
			int crKey,
			[System.Runtime.InteropServices.In()]
			ref BLENDFUNCTION pblend,
			int dwFlags);

		/// <summary>
		/// レイヤードウィンドウを作成します。
		/// </summary>
		/// <param name="src">元になる画像。</param>
		public void SetLayeredWindow(Bitmap src)
		{
			// GetDeviceContext
			IntPtr screenDc = IntPtr.Zero;
			IntPtr memDc = IntPtr.Zero;
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr hOldBitmap = IntPtr.Zero;
			try
			{
				screenDc = GetDC(IntPtr.Zero);
				memDc = CreateCompatibleDC(screenDc);
				hBitmap = src.GetHbitmap(Color.FromArgb(0));
				hOldBitmap = SelectObject(memDc, hBitmap);

				BLENDFUNCTION blend = new BLENDFUNCTION
				{
					BlendOp = AC_SRC_OVER,
					BlendFlags = 0,
					SourceConstantAlpha = 255,
					AlphaFormat = AC_SRC_ALPHA
				};

				//Size = new Size( src.Width, src.Height );
				Point pptDst = new Point(this.Left, this.Top);
				Size psize = new Size(this.Width, this.Height);
				Point pptSrc = new Point(0, 0);
				UpdateLayeredWindow(this.Handle, screenDc, ref pptDst, ref psize, memDc,
				  ref pptSrc, 0, ref blend, ULW_ALPHA);

			}
			finally
			{
				if (screenDc != IntPtr.Zero)
				{
					ReleaseDC(IntPtr.Zero, screenDc);
				}
				if (hBitmap != IntPtr.Zero)
				{
					SelectObject(memDc, hOldBitmap);
					DeleteObject(hBitmap);
				}
				if (memDc != IntPtr.Zero)
				{
					DeleteDC(memDc);
				}
			}
		}


	}
}
