using ElectronicObserver.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogHalloween : Form
	{

		private Bitmap canvas;
		private Bitmap[] fairies;
		private Graphics paintbrush;
		private int zoomscale;
		private Rectangle trickButton;
		private Rectangle treatButton;
		private Random rand;
		private Point cursorPosition;
		private RectangleF[] fairiesVector;
		private int state;
		private int tick;

		public DialogHalloween()
		{
            this.InitializeComponent();
		}

		private void DialogHalloween_Load(object sender, EventArgs e)
		{

            this.canvas = new Bitmap(320, 240, PixelFormat.Format32bppArgb);
            this.paintbrush = Graphics.FromImage(this.canvas);
            this.rand = new Random();
            this.fairies = new Bitmap[4];
            this.fairiesVector = new RectangleF[4];

			for (int i = 0; i < 4; i++)
			{
				try
				{
                    this.fairies[i] = new Bitmap(ResourceManager.GetStreamFromArchive("Fairy/Fairy" + (i + 1) + ".png"));
				}
				catch (Exception ex)
				{
					Utility.Logger.Add(1, "[PumpkinHead] Failed to gather fairies. Boo... " + ex.Message);
                    this.Close();
					return;
				}
			}

            this.zoomscale = 2;
            this.trickButton = new Rectangle(this.canvas.Width * 1 / 4 - 80 / 2, this.canvas.Height * 3 / 4 - 32 / 2, 80, 32);
            this.treatButton = new Rectangle(this.canvas.Width * 3 / 4 - 80 / 2, this.canvas.Height * 3 / 4 - 32 / 2, 80, 32);

            this.cursorPosition = new Point(-1, -1);
            this.state = 1;
            this.tick = 0;

            this.ClientSize = new Size(this.canvas.Width * this.zoomscale, this.canvas.Height * this.zoomscale);
            this.Icon = ResourceManager.Instance.AppIcon;
            this.Updater.Start();

		}

		private void Updater_Tick(object sender, EventArgs e)
		{

            this.paintbrush.ResetTransform();

			switch (this.state)
			{
				case 1:
                    // initial phase

                    this.DrawBackground();

					//
					{
						string mes = "T r i c k   o r   T r e a t !";
                        this.paintbrush.DrawString(mes, this.Font, Brushes.White, (int)(this.canvas.Width / 2 - mes.Length * this.Font.Size / 4) + 1, this.canvas.Height * 1 / 8 + 1);
                        this.paintbrush.DrawString(mes, this.Font, Brushes.Red, (int)(this.canvas.Width / 2 - mes.Length * this.Font.Size / 4), this.canvas.Height * 1 / 8);
					}

                    this.paintbrush.DrawImage(this.fairies[3],
                        this.canvas.Width / 2 - this.fairies[3].Width / 2 + (this.cursorPosition.X >= this.canvas.Width / 2 ? this.fairies[3].Width : 0),
                        this.canvas.Height * 4 / 8 - this.fairies[3].Height / 2 + (int)(Math.Sin(1.0 / 4.0 * this.tick / this.GetFPS() * 2 * Math.PI) * this.fairies[3].Height / 4),
                        this.fairies[3].Width * (this.cursorPosition.X >= this.canvas.Width / 2 ? -1 : 1),
                        this.fairies[3].Height);

                    this.DrawButton("Trick!", this.trickButton, Brushes.Orange, Pens.White, this.trickButton.Contains(this.cursorPosition) ? Brushes.Red : Brushes.Maroon);
                    this.DrawButton("Treat!", this.treatButton, Brushes.Orange, Pens.White, this.treatButton.Contains(this.cursorPosition) ? Brushes.Red : Brushes.Maroon);

					break;

				case 2:
                    // Tricked

                    this.DrawBackground();

					//

					{
						string mes = "B o o o o o o o o o o o o ! !";
                        this.paintbrush.DrawString(mes, this.Font, Brushes.White, (int)(this.canvas.Width / 2 - mes.Length * this.Font.Size / 4) + 1, this.canvas.Height * 1 / 8 + 1);
                        this.paintbrush.DrawString(mes, this.Font, Brushes.Red, (int)(this.canvas.Width / 2 - mes.Length * this.Font.Size / 4), this.canvas.Height * 1 / 8);
					}

					{
						string mes = "* Open config and press OK to restore  ";
                        this.paintbrush.DrawString(mes, this.Font, Brushes.White, (int)(this.canvas.Width - mes.Length * this.Font.Size / 2) + 1, this.canvas.Height * 15 / 16 + 1);
                        this.paintbrush.DrawString(mes, this.Font, Brushes.Brown, (int)(this.canvas.Width - mes.Length * this.Font.Size / 2), this.canvas.Height * 15 / 16);
					}

					for (int i = 0; i < this.fairiesVector.Length; i++)
					{
                        this.fairiesVector[i].X += this.fairiesVector[i].Width;
                        this.fairiesVector[i].Y += this.fairiesVector[i].Height;

						if ((this.fairiesVector[i].X < 0 && this.fairiesVector[i].Width < 0) ||
							 (this.fairiesVector[i].X >= this.canvas.Width - this.fairies[i].Width && this.fairiesVector[i].Width > 0))
                            this.fairiesVector[i].Width *= -1;
						if ((this.fairiesVector[i].Y < 0 && this.fairiesVector[i].Height < 0) ||
							 (this.fairiesVector[i].Y >= this.canvas.Height - this.fairies[i].Height && this.fairiesVector[i].Height > 0))
                            this.fairiesVector[i].Height *= -1;

                        this.paintbrush.DrawImage(this.fairies[i], new Rectangle(
							(this.fairiesVector[i].Width <= 0 ? (int)this.fairiesVector[i].X : ((int)this.fairiesVector[i].X + this.fairies[i].Width)) + (int)Math.Round((this.rand.NextDouble() * 2.0 - 1.0) * 4.0),
							(int)this.fairiesVector[i].Y + (int)Math.Round((this.rand.NextDouble() * 2.0 - 1.0) * 4.0),
							(this.fairiesVector[i].Width > 0 ? -1 : 1) * this.fairies[i].Width,
                            this.fairies[i].Height));
					}

					break;


				case 3:
                    // Treated

                    this.DrawBackground();

					//
					{
						string mes = "T h a n k   y o u ! !";
                        this.paintbrush.DrawString(mes, this.Font, Brushes.White, (int)(this.canvas.Width / 2 - mes.Length * this.Font.Size / 4) + 1, this.canvas.Height * 1 / 8 + 1);
                        this.paintbrush.DrawString(mes, this.Font, Brushes.Red, (int)(this.canvas.Width / 2 - mes.Length * this.Font.Size / 4), this.canvas.Height * 1 / 8);
					}

					{
						string mes = "* Set comment \"jackolantern\"  ";
                        this.paintbrush.DrawString(mes, this.Font, Brushes.White, (int)(this.canvas.Width - mes.Length * this.Font.Size / 2) + 1, this.canvas.Height * 15 / 16 + 1);
                        this.paintbrush.DrawString(mes, this.Font, Brushes.Brown, (int)(this.canvas.Width - mes.Length * this.Font.Size / 2), this.canvas.Height * 15 / 16);
					}


					// green girl
					{
						int w = this.fairies[0].Width;
						int h = this.fairies[0].Height;
						Point org = new Point(8 + w / 2, this.canvas.Height / 2 - h / 2);
						switch (this.tick * 2 / this.GetFPS() % 4)
						{
							case 0:
                                this.paintbrush.DrawImage(this.fairies[0], new Point[] {
									new Point( org.X, org.Y ),
									new Point( org.X + w, org.Y ),
									new Point( org.X, org.Y + h ),
								});
								break;
							case 1:
                                this.paintbrush.DrawImage(this.fairies[0], new Point[] {
									new Point( org.X + w, org.Y ),
									new Point( org.X + w, org.Y + h ),
									new Point( org.X, org.Y ),
								});
								break;
							case 2:
                                this.paintbrush.DrawImage(this.fairies[0], new Point[] {
									new Point( org.X + w, org.Y + h ),
									new Point( org.X, org.Y + h ),
									new Point( org.X + w, org.Y ),
								});
								break;
							case 3:
                                this.paintbrush.DrawImage(this.fairies[0], new Point[] {
									new Point( org.X, org.Y + h ),
									new Point( org.X, org.Y ),
									new Point( org.X + w, org.Y + h ),
								});
								break;
						}
					}


					//peach girl
					{
						int beattick = 8;
						int phase = (int)(this.tick / 0.4 / this.GetFPS()) % beattick;
						bool isInverted = phase >= beattick / 2;
                        this.paintbrush.ResetTransform();
                        this.paintbrush.DrawImage(this.fairies[1], new Rectangle(
							64 + 8 + 32 + (isInverted ? this.fairies[1].Width : 0),
                            this.canvas.Height / 2 - this.fairies[1].Height / 2 + (phase % 4 == 0 ? ((int)(Math.Sin(this.tick % 4 / 2.0 * Math.PI) * 8)) : 0),
                            this.fairies[1].Width * (isInverted ? -1 : 1),
                            this.fairies[1].Height));
					}

					//bird girl
					{
						int beattick = 1 * this.GetFPS();
						int phase = this.tick / beattick % 4;
						bool horizontalInverted = phase == 1 || phase == 2;
						bool verticalInverted = phase == 2 || phase == 3;
                        this.paintbrush.DrawImage(this.fairies[2], new Rectangle(
							128 + 8 + 32 + (horizontalInverted ? this.fairies[2].Width : 0) + (int)Math.Round((this.rand.NextDouble() * 2.0 - 1.0) * 8),
                            this.canvas.Height / 2 - this.fairies[2].Height / 2 + (verticalInverted ? this.fairies[2].Height : 0) + (int)Math.Round((this.rand.NextDouble() * 2.0 - 1.0) * 8),
                            this.fairies[2].Width * (horizontalInverted ? -1 : 1),
                            this.fairies[2].Height * (verticalInverted ? -1 : 1)));
					}

					//witch girl
					{
						double rad = 16;
						double angle = (double)this.tick / this.GetFPS() * Math.PI % (2 * Math.PI);
                        this.paintbrush.DrawImage(this.fairies[3], new Rectangle(
							192 + 8 + 32 + (int)(Math.Cos(angle) * rad) + (Math.Cos(angle) >= 0 ? this.fairies[3].Width : 0),
                            this.canvas.Height / 2 - this.fairies[3].Height / 2 + (int)(Math.Sin(angle) * rad),
                            this.fairies[3].Width * (Math.Cos(angle) >= 0 ? -1 : 1),
                            this.fairies[3].Height));
					}

					break;
			}

            this.paintbrush.ResetTransform();
            //paintbrush.DrawString( tick.ToString(), Font, Brushes.Red, 0, 0 );

            this.tick++;
            this.Refresh();
		}

		private void DialogHalloween_MouseClick(object sender, MouseEventArgs e)
		{
            this.cursorPosition = new Point(e.X / this.zoomscale, e.Y / this.zoomscale);

			switch (this.state)
			{
				case 1:

					if (this.trickButton.Contains(this.cursorPosition))
					{

                        this.state = 2;


						// initialize movement
						for (int i = 0; i < 4; i++)
						{
                            this.fairiesVector[i] = new RectangleF(this.rand.Next(this.canvas.Width - this.fairies[i].Width), this.rand.Next(this.canvas.Height - this.fairies[i].Height),
								(float)((this.rand.NextDouble() * 2.0 - 1.0) * 8.0), (float)((this.rand.NextDouble() * 2.0 - 1.0) * 8.0));
						}


						// system font override
						var c = Utility.Configuration.Config;
						Font preservedfont_main = c.UI.MainFont;
						Font preservedfont_sub = c.UI.SubFont;

						string[] candidates = {
							"HGP創英角ﾎﾟｯﾌﾟ体",
							"ふい字Ｐ",
							"Segoe Script",
							"MS UI Gothic",
						  };
						string fontname = null;

						var fonts = new System.Drawing.Text.InstalledFontCollection();
						for (int i = 0; i < candidates.Length; i++)
						{
							if (fonts.Families.Count(f => f.Name == candidates[i]) > 0)
							{
								fontname = candidates[i];
								break;
							}

						}
						if (fontname == null)
							break;

						c.UI.MainFont = new Font(fontname, 12, FontStyle.Regular, GraphicsUnit.Pixel);
						c.UI.SubFont = new Font(fontname, 10, FontStyle.Regular, GraphicsUnit.Pixel);

						Utility.Configuration.Instance.OnConfigurationChanged();

						c.UI.MainFont = preservedfont_main;
						c.UI.SubFont = preservedfont_sub;

					}
					else if (this.treatButton.Contains(this.cursorPosition))
					{
                        this.state = 3;
					}

					break;

			}
		}

		private void DialogHalloween_MouseMove(object sender, MouseEventArgs e)
		{
            this.cursorPosition = new Point(e.X / this.zoomscale, e.Y / this.zoomscale);
		}

		private void DialogHalloween_Paint(object sender, PaintEventArgs e)
		{

			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

			e.Graphics.DrawImage(this.canvas, 0, 0, this.canvas.Width * this.zoomscale, this.canvas.Height * this.zoomscale);

		}



		private int GetFPS()
		{
			return 1000 / this.Updater.Interval;
		}

		private void DrawBackground()
		{

			const int squaresize = 32;
			const int framerate = 4;

			BitmapData bitmapdata = this.canvas.LockBits(new Rectangle(0, 0, this.canvas.Width, this.canvas.Height), ImageLockMode.WriteOnly, this.canvas.PixelFormat);
			byte[] buffer = new byte[this.canvas.Width * this.canvas.Height * 4];

			int shift = (this.tick * framerate / this.GetFPS()) % squaresize;

			for (int i = 0; i < buffer.Length; i += 4)
			{
				int colflag = (((i / 4 % this.canvas.Width + shift) / squaresize) & 1) ^ (((i / 4 / this.canvas.Width + shift) / squaresize) & 1);

				if (colflag == 0)
				{
					//orange
					buffer[i + 3] = 0xFF;
					buffer[i + 2] = 0xFF;
					buffer[i + 1] = 0xCC;
					buffer[i + 0] = 0x88;

				}
				else
				{
					//black
					buffer[i + 3] = 0xFF;
					buffer[i + 2] = 0x88;
					buffer[i + 1] = 0x88;
					buffer[i + 0] = 0x88;

				}
			}

			Marshal.Copy(buffer, 0, bitmapdata.Scan0, buffer.Length);
            this.canvas.UnlockBits(bitmapdata);
		}

		private void DrawButton(string message, Rectangle rect, Brush foregroundBrush, Pen backgroundPen, Brush backgroundBrush)
		{
            this.paintbrush.FillRectangle(backgroundBrush, rect);
            this.paintbrush.DrawRectangle(backgroundPen, rect);
            this.paintbrush.DrawRectangle(backgroundPen, new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4));
            this.paintbrush.DrawString(message, this.Font, Brushes.White, rect.X + rect.Width / 2 - (int)(message.Length * this.Font.Size / 2) / 2 + 1, rect.Y + rect.Height / 2 - (int)(this.Font.Size / 2) + 1);
            this.paintbrush.DrawString(message, this.Font, foregroundBrush, rect.X + rect.Width / 2 - (int)(message.Length * this.Font.Size / 2) / 2, rect.Y + rect.Height / 2 - (int)(this.Font.Size / 2));

		}


		private void DialogHalloween_FormClosed(object sender, FormClosedEventArgs e)
		{

            this.Updater.Stop();

            this.paintbrush.Dispose();
            this.canvas.Dispose();
			for (int i = 0; i < this.fairies.Length; i++)
				if (this.fairies[i] != null)
                    this.fairies[i].Dispose();

		}




	}
}
