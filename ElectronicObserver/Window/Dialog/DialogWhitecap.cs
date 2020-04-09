using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogWhitecap : Form
	{

		private int[,,] board;
		private Size boardSize;
		private int currentDim;

		private int birthRule;
		private int aliveRule;

		private int zoomrate;
		private int colortheme;
		private Bitmap imagebuf;

		private Random rand;

		private int clock;


		public DialogWhitecap()
		{
            this.InitializeComponent();

            this.birthRule = Utility.Configuration.Config.Whitecap.BirthRule;
            this.aliveRule = Utility.Configuration.Config.Whitecap.AliveRule;

            this.zoomrate = Utility.Configuration.Config.Whitecap.ZoomRate;

            this.colortheme = Utility.Configuration.Config.Whitecap.ColorTheme;
            this.imagebuf = null;

            this.rand = new Random();

            this.SetSize(Utility.Configuration.Config.Whitecap.BoardWidth, Utility.Configuration.Config.Whitecap.BoardHeight);
            this.ShowInTaskbar = Utility.Configuration.Config.Whitecap.ShowInTaskbar;
            this.TopMost = Utility.Configuration.Config.Whitecap.TopMost;
		}

		private void DialogWhitecap_Load(object sender, EventArgs e)
		{

            this.UpdateTimer.Interval = Utility.Configuration.Config.Whitecap.UpdateInterval;

            this.Start();
		}

		private void Start()
		{

            this.InitBoard();

            this.ClientSize = new Size(this.boardSize.Width * this.zoomrate, this.boardSize.Height * this.zoomrate);
            this.imagebuf?.Dispose();
            this.imagebuf = new Bitmap(this.boardSize.Width, this.boardSize.Height, PixelFormat.Format24bppRgb);

            this.clock = 0;
            this.UpdateTimer.Start();
		}

		private void SetSize(int width, int height)
		{
            this.boardSize = new Size(width, height);
            this.board = new int[2, height, width];
		}

		private void InitBoard(bool isRand = true)
		{

			for (int dim = 0; dim < 2; dim++)
			{
				for (int y = 0; y < this.boardSize.Height; y++)
				{
					for (int x = 0; x < this.boardSize.Width; x++)
					{
                        this.board[dim, y, x] = isRand ? this.rand.Next(2) : 0;
					}
				}
			}

            this.currentDim = 0;
		}

		private void SetCell(int dim, int x, int y, int value)
		{

			x = x % this.boardSize.Width;
			if (x < 0) x += this.boardSize.Width;

			y = y % this.boardSize.Height;
			if (y < 0) y += this.boardSize.Height;

            this.board[dim, y, x] = value;
		}

		private int GetCell(int dim, int x, int y)
		{

			x = x % this.boardSize.Width;
			if (x < 0) x += this.boardSize.Width;

			y = y % this.boardSize.Height;
			if (y < 0) y += this.boardSize.Height;

			return this.board[dim, y, x];
		}



		private void UpdateTimer_Tick(object sender, EventArgs e)
		{

			for (int y = 0; y < this.boardSize.Height; y++)
			{
				for (int x = 0; x < this.boardSize.Width; x++)
				{

					int alive = 0;

					for (int dy = -1; dy <= 1; dy++)
					{
						for (int dx = -1; dx <= 1; dx++)
						{
							if ((dx != 0 || dy != 0) && this.GetCell(this.currentDim, x + dx, y + dy) != 0)
								alive++;
						}
					}

					if (this.GetCell(this.currentDim, x, y) != 0)
					{
                        this.SetCell(1 - this.currentDim, x, y, ((1 << alive) & this.aliveRule) != 0 ? 1 : 0);

					}
					else
					{

                        this.SetCell(1 - this.currentDim, x, y, ((1 << alive) & this.birthRule) != 0 ? 1 : 0);

					}
				}
			}

            this.currentDim = 1 - this.currentDim;
            this.clock++;

            this.Refresh();
		}


		private void DialogWhitecap_Paint(object sender, PaintEventArgs e)
		{

			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

			e.Graphics.Clear(Color.Black);

			BitmapData bmpdata = this.imagebuf.LockBits(new Rectangle(0, 0, this.imagebuf.Width, this.imagebuf.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			byte[] canvas = new byte[this.imagebuf.Width * this.imagebuf.Height * 3];
			Marshal.Copy(bmpdata.Scan0, canvas, 0, canvas.Length);

			for (int y = 0; y < this.boardSize.Height; y++)
			{
				for (int x = 0; x < this.boardSize.Width; x++)
				{

					Color col;
					Color prev = Color.FromArgb(
						canvas[((y * this.boardSize.Width + x) * 3 + 2)],
						canvas[((y * this.boardSize.Width + x) * 3 + 1)],
						canvas[((y * this.boardSize.Width + x) * 3 + 0)]);
					int value = this.GetCell(this.currentDim, x, y);


					switch (this.colortheme)
					{

						case 1:
							col = value != 0 ?
                                this.BlendColor(this.FromRgb(0x000000), this.FromRgb(0xFF0000), (double)y / this.boardSize.Height) :
                                this.BlendColor(this.FromRgb(0xFF0000), this.FromRgb(0xFFFF00), (double)y / this.boardSize.Height);
							break;

						case 2:
							col = value != 0 ?
                                this.BlendColor(this.FromRgb(0xFFFFFF), this.FromRgb(0x00FFFF), (double)y / this.boardSize.Height) :
                                this.BlendColor(this.FromRgb(0x0044FF), this.FromRgb(0x000000), (double)y / this.boardSize.Height);
							break;

						case 3:
							/*
							col = value != 0 ?
								BlendColor( FromRgb( 0xFFFFFF ), FromRgb( 0xFFDDBB ), (double)y / boardSize.Height ) :
								BlendColor( FromRgb( 0x00FFFF ), FromRgb( 0xFFDDBB ), (double)y / boardSize.Height );
							*/
							col = this.BlendColor(this.GetCell(this.currentDim, x, y + (int)((Math.Sin(this.clock / 100.0 * 2.0 * Math.PI) + 1) * this.boardSize.Height / 8.0)) != 0 ?
                                this.BlendColor(this.FromRgb(0xFFFFFF), this.FromRgb(0xFFDDBB), Math.Max(Math.Min((y + ((Math.Sin(this.clock / 100.0 * 2.0 * Math.PI)) * this.boardSize.Height / 8.0)) / this.boardSize.Height, 1.0), 0.0)) :
                                this.BlendColor(this.FromRgb(0x00FFFF), this.FromRgb(0xFFDDBB), Math.Min((y + ((Math.Sin(this.clock / 100.0 * 2.0 * Math.PI) + 1) * this.boardSize.Height / 8.0)) / this.boardSize.Height, 1.0)),
								prev, 0.8);
							break;

						case 4:
							col = value != 0 ?
                                this.BlendColor(this.FromRgb(0xFFFFFF), this.FromRgb(0xCCCCFF), (double)y / this.boardSize.Height) :
                                this.BlendColor(this.FromRgb(0x000000), this.FromRgb(0x000088), (double)y / this.boardSize.Height);
							break;

						case 5:
							col = value != 0 ?
                                this.BlendColor(this.FromRgb(0xDDDDDD), this.FromRgb(0xFFFFFF), (double)(x + y) / (this.boardSize.Width + this.boardSize.Height)) :
                                this.BlendColor(this.FromRgb(0x778888), this.FromRgb(0x99AAAA), (double)(x + y) / (this.boardSize.Width + this.boardSize.Height));
							break;

						case 6:
							col = value != 0 ?
                                this.BlendColor(this.FromRgb(0xFF66FF), this.FromRgb(0xFFAAFF), Math.Pow(x + y, 2) / Math.Pow(this.boardSize.Width + this.boardSize.Height, 2)) :
                                this.BlendColor(this.FromRgb(0xFFCCCC), this.FromRgb(0xFFFFFF), Math.Pow(x + y, 2) / Math.Pow(this.boardSize.Width + this.boardSize.Height, 2));
							break;

						case 7:
							col = value != 0 ?
                                this.BlendColor(this.FromRgb(0x008800), this.FromRgb(0x44FF44), Math.Pow(x + y, 2) / Math.Pow(this.boardSize.Width + this.boardSize.Height, 2)) :
                                this.BlendColor(this.FromRgb(0x88FF88), this.FromRgb(0xCCFF88), Math.Pow(x + y, 2) / Math.Pow(this.boardSize.Width + this.boardSize.Height, 2));
							break;

						case 8:
							col = value != 0 ?
                                this.FromRgb(0xFFFFFF) :
                                this.BlendColor(this.FromRgb(0x000000), prev, 0.5);
							break;

						case 9:
							col = value != 0 ?
                                this.FromHsv(x + y + this.clock * 3, 1.0, 1.0) :
                                this.FromRgb(0x000000);
							break;

						case 10:
							col = this.GetCell(this.currentDim, x, y + this.clock) != 0 ?
                                this.BlendColor(this.FromRgb(0xFFFFFF), this.FromRgb(0x00FFFF), (double)y / this.boardSize.Height) :
                                this.BlendColor(prev, this.BlendColor(this.FromRgb(0x0044FF), this.FromRgb(0x000000), (double)y / this.boardSize.Height), 0.2);
							break;

						case 11:
							col = value != 0 ? this.FromRgb(0x00FF00) : this.FromRgb(0x111111);
							break;

						case 12:
							col = value != 0 ?
                                this.FromRgb(0x0044FF) :
                                this.BlendColor(this.FromRgb(0xFFFFFF), prev, 0.9);
							break;

						case 13:
							col = value != 0 ?
                                this.FromRgb(0xFF0000) :
                                this.AddColor(prev, this.FromRgb(0xFF4422), 0.1);
							break;

						case 14:
							col = this.GetCell(this.currentDim, x, y + this.clock) != 0 ?
                                this.BlendColor(this.FromRgb(0xFFFFFF), this.FromRgb(0xFFFFCC), (double)y / this.boardSize.Height) :
                                this.BlendColor(prev, this.BlendColor(this.FromRgb(0x88FFFF), this.FromRgb(0x0000FF), (double)y / this.boardSize.Height), 0.05);
							break;

						case 15:
							col = this.FromHsv(x * x + 2 * x * y + y * y + 98 * x + 168 * y, value != 0 ? 1.0 : 0.2, value != 0 ? 1.0 : 1.0);
							break;

						case 16:
							col = value != 0 ? this.FromRgb(0x000000) : this.FromRgb(0xFFFFFF);
							break;

						case 17:
							col = this.BlendColor(prev, this.GetCell(this.currentDim, x + this.clock / 4, y) != 0 ?
                                this.FromRgb(0xFFFFFF) : this.BlendColor(this.FromRgb(0x0088FF), this.FromRgb(0x88FFFF), (double)y / this.boardSize.Height),
								0.08);
							break;

						case 18:
							col = this.AddColor(value != 0 ? this.FromRgb(0xFF0000) : this.FromRgb(0x000000),
                                this.AddColor(this.GetCell(this.currentDim, x, this.boardSize.Height - 1 - y) != 0 ? this.FromRgb(0x00FF00) : this.FromRgb(0x000000),
                                this.GetCell(this.currentDim, this.boardSize.Width - 1 - x, y) != 0 ? this.FromRgb(0x0000FF) : this.FromRgb(0x000000)));
							break;

						case 19:
							//*/
							if (value != 0 ||
                                this.GetCell(this.currentDim, x + 1, y) != 0 ||
                                this.GetCell(this.currentDim, x, y + 1) != 0 ||
                                this.GetCell(this.currentDim, x + 1, y + 1) != 0)
								col = this.FromRgb(0xFFFFFF);
							else if (
                                this.GetCell(this.currentDim, x - 1, y) != 0 ||
                                this.GetCell(this.currentDim, x, y - 1) != 0 ||
                                this.GetCell(this.currentDim, x - 1, y - 1) != 0)
								col = this.FromRgb(0x888888);
							else
								col = this.FromRgb(0x000000);
							/*/
							if ( value != 0 )
								col = FromRgb( 0xFFFFFF );
							else if ( GetCell( currentDim, x - 1, y - 1 ) != 0 )
								col = FromRgb( 0x888888 );
							else
								col = FromRgb( 0x000000 );			
							//*/
							break;

						case 20:
							if (value != 0)
							{
								const int incr = 1;
								col = Color.FromArgb(Math.Min(prev.R + incr, 255), Math.Min(prev.G + incr, 255), Math.Min(prev.B + incr, 255));
							}
							else
							{
								const int decr = 0;
								col = Color.FromArgb(Math.Max(prev.R - decr, 0), Math.Max(prev.G - decr, 0), Math.Max(prev.B - decr, 0));
							}
							break;

						case 21:
							if (value != 0)
							{
								col = this.BlendColor(prev, this.FromRgb(0x000000), 0.2);
							}
							else
							{
								col = this.BlendColor(prev, this.BlendColor(this.FromRgb(0x111188), this.FromRgb(0x111111), (double)y / this.boardSize.Height), 0.2);
							}

							if (value != 0)
							{
								const int blocksize = 32;
								const int frequency = 16;

								int ux = (int)(x / blocksize) * blocksize;
								int uy = (int)(y / blocksize) * blocksize;

								int seedx = this.clock / frequency * 16829 + ux / blocksize * 81953 + uy / blocksize * 40123;
								int seedy = this.clock / frequency * 81041 + ux / blocksize * 11471 + uy / blocksize * 51419;
								int seedz = this.clock / frequency * 39503 + ux / blocksize * 46133 + uy / blocksize * 15241;


								int rx = (seedx >> 4) % blocksize;
								int ry = (seedy >> 4) % blocksize;
								int rz = (seedz >> 4) % 256;

								if (ux + rx == x && uy + ry == y)
								{
									Color eye;
									if (rz < 160)
										eye = this.FromRgb(0x00FFFF);
									else if (rz < 224)
										eye = this.FromRgb(0xFF0000);
									else
										eye = this.FromRgb(0xFFCC00);


									col = this.BlendColor(col, eye, (double)y / this.boardSize.Height);
								}

							}

							break;

						case 22:
							if (value == 0)
								col = this.FromRgb(0x000000);
							else
							{
								if (this.GetCell(1 - this.currentDim, x, y) != 0)
								{
									col = prev;
								}
								else
								{
									col = this.FromHsv((int)(this.rand.NextDouble() * 12) * 30, 0.75, 1);
								}
							}
							break;

						case 23:
							if (value != 0)
								col = this.FromHsv(0, 0, this.rand.NextDouble() * 0.5 + 0.5);
							else
								col = this.FromHsv(0, 0, this.rand.NextDouble() * 0.5);
							break;

						case 24:
							{
								int prevcell = this.GetCell(1 - this.currentDim, x, y);
								if (value != 0)
								{
									if (prevcell != 0)
										col = this.FromRgb(0xFFFFFF);
									else
										col = this.FromRgb(0x00FF00);
								}
								else
								{
									if (prevcell != 0)
										col = this.FromRgb(0xFF0000);
									else
										col = this.FromRgb(0x000000);
								}
							}
							break;

						case 25:
							{
								int prevcell = this.GetCell(1 - this.currentDim, x, y);
								if (value != 0)
								{
									if (prevcell != 0)
										col = this.FromRgb(0x222222);
									else
										col = this.FromRgb(0x00FF00);
								}
								else
								{
									if (prevcell != 0)
										col = this.FromRgb(0xFF0000);
									else
										col = this.FromRgb(0x000000);
								}
							}
							break;

						case 26:
							{
								if (value != 0)
								{
									col = this.FromRgb(0x334433);
								}
								else
								{
									if (this.GetCell(this.currentDim, x - 1, y) != 0 ||
                                        this.GetCell(this.currentDim, x + 1, y) != 0 ||
                                        this.GetCell(this.currentDim, x, y - 1) != 0 ||
                                        this.GetCell(this.currentDim, x, y + 1) != 0)
									{
										col = this.FromRgb(0xEEFFEE);
									}
									else
									{
										col = this.FromRgb(0x889988);
									}
								}
							}
							break;

						case 27:
							{
								int prevcell = this.GetCell(1 - this.currentDim, x, y);

								if (value != 0)
								{
									if (prevcell != 0)
										col = this.FromRgb(0xFFFF44);
									else
										col = this.FromRgb(0x444422);
								}
								else
								{
									col = this.FromRgb(0x000022);
								}
							}
							break;

						default:
							col = value != 0 ? this.FromRgb(0xFFFFFF) : this.FromRgb(0x000000);
							break;

					}


					canvas[((y * this.boardSize.Width + x) * 3 + 0)] = col.B;
					canvas[((y * this.boardSize.Width + x) * 3 + 1)] = col.G;
					canvas[((y * this.boardSize.Width + x) * 3 + 2)] = col.R;

				}
			}

			Marshal.Copy(canvas, 0, bmpdata.Scan0, canvas.Length);
            this.imagebuf.UnlockBits(bmpdata);

			e.Graphics.DrawImage(this.imagebuf, 0, 0, this.imagebuf.Width * this.zoomrate, this.imagebuf.Height * this.zoomrate);

		}


		private Color FromRgb(int rgb)
		{
			return Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
		}


		/// <summary>
		/// generate Color from hsv
		/// </summary>
		/// <param name="hue">0-360</param>
		/// <param name="saturation">0-1</param>
		/// <param name="brightness">0-1</param>
		/// <returns></returns>
		private Color FromHsv(double hue, double saturation, double brightness)
		{
			hue = hue % 360.0;
			if (hue < 0.0) hue += 360.0;
			if (saturation < 0.0) saturation = 0.0;

			double r = 255 * brightness, g = 255 * brightness, b = 255 * brightness;
			int mode = (int)(hue / 60.0);
			double weight = ((hue / 60.0) - (int)(hue / 60.0));

			switch (mode)
			{
				default:
				case 0:
					g *= 1.0 - saturation * (1.0 - weight);
					b *= 1.0 - saturation;
					break;
				case 1:
					r *= 1.0 - saturation * weight;
					b *= 1.0 - saturation;
					break;
				case 2:
					b *= 1.0 - saturation * (1.0 - weight);
					r *= 1.0 - saturation;
					break;
				case 3:
					g *= 1.0 - saturation * weight;
					r *= 1.0 - saturation;
					break;
				case 4:
					r *= 1.0 - saturation * (1.0 - weight);
					g *= 1.0 - saturation;
					break;
				case 5:
					b *= 1.0 - saturation * weight;
					g *= 1.0 - saturation;
					break;
			}

			return Color.FromArgb((int)r, (int)g, (int)b);
		}

		private Color AddColor(Color a, Color b, double weight = 1.0)
		{
			return Color.FromArgb(
				Math.Min(a.R + (int)(b.R * weight), 255),
				Math.Min(a.G + (int)(b.G * weight), 255),
				Math.Min(a.B + (int)(b.B * weight), 255));
		}

		private Color BlendColor(Color a, Color b, double weight = 0.5)
		{
			return Color.FromArgb(
				Math.Min((int)(a.R * (1 - weight) + b.R * weight), 255),
				Math.Min((int)(a.G * (1 - weight) + b.G * weight), 255),
				Math.Min((int)(a.B * (1 - weight) + b.B * weight), 255));
		}

		private void DialogWhitecap_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
                this.UpdateTimer.Stop();
                this.InitBoard();
                this.UpdateTimer.Start();

			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{

                this.UpdateTimer.Stop();

				try
				{
                    this.imagebuf.Save(string.Format("SS@{0}.png", DateTimeHelper.GetTimeStamp()), ImageFormat.Png);

				}
				catch (Exception)
				{
					System.Media.SystemSounds.Exclamation.Play();

				}
				finally
				{
                    this.UpdateTimer.Start();
				}
			}

		}

		private void DialogWhitecap_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
                this.UpdateTimer.Stop();
                this.colortheme = this.rand.Next(64);
                //colortheme = 27;
                this.Start();
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			// imagebuf?.Dispose() とするとコード分析に引っかかるので :(
			if (this.imagebuf != null)
                this.imagebuf.Dispose();


			// --- auto generated ---
			if (disposing && (this.components != null))
			{
                this.components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
