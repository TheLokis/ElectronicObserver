using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Data;
using System.Drawing.Design;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Window.Support;

namespace ElectronicObserver.Window.Control
{

	public partial class ShipStatusEquipment : UserControl
	{

		[System.Diagnostics.DebuggerDisplay("{Equipment}")]
		private class SlotItem
		{

			/// <summary>
			/// 装備ID
			/// </summary>
			public int EquipmentID { get; set; }

			/// <summary>
			/// 装備インスタンス
			/// </summary>
			public EquipmentDataMaster Equipment => KCDatabase.Instance.MasterEquipments[this.EquipmentID];


			/// <summary>
			/// 装備アイコンID
			/// </summary>
			public int EquipmentIconID
			{
				get
				{
					var eq = KCDatabase.Instance.MasterEquipments[this.EquipmentID];
					if (eq != null)
						return eq.IconType;
					else
						return -1;
				}
			}

			/// <summary>
			/// 搭載機数
			/// </summary>
			public int AircraftCurrent { get; set; }

			/// <summary>
			/// 最大搭載機数
			/// </summary>
			public int AircraftMax { get; set; }


			/// <summary>
			/// 改修レベル
			/// </summary>
			public int Level { get; set; }

			/// <summary>
			/// 艦載機熟練度
			/// </summary>
			public int AircraftLevel { get; set; }


			public SlotItem()
			{
                this.EquipmentID = -1;
                this.AircraftCurrent = this.AircraftMax = 0;
                this.Level = this.AircraftLevel = 0;
			}
		}



		#region Properties


		private SlotItem[] SlotList;
		private int _slotSize;
		private int SlotSize
		{
			get { return this._slotSize; }
			set
			{
                this._slotSize = value;
                this.PropertyChanged();
			}
		}

		private bool _onMouse;


		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Font), "Meiryo UI, 10px")]
		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				if (this.LayoutParam != null)
                    this.LayoutParam.ResetLayout();
                this.PropertyChanged();
			}
		}

		private Color _aircraftColorDisabled;
		/// <summary>
		/// 艦載機非搭載スロットの文字色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "170, 170, 170")]
		[Description("艦載機非搭載スロットの文字色を指定します。")]
		public Color AircraftColorDisabled
		{
			get { return this._aircraftColorDisabled; }
			set
			{
                this._aircraftColorDisabled = value;
                this.PropertyChanged();
			}
		}

		private Color _aircraftColorLost;
		/// <summary>
		/// 艦載機全滅スロットの文字色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 0, 255")]
		[Description("艦載機全滅スロットの文字色を指定します。")]
		public Color AircraftColorLost
		{
			get { return this._aircraftColorLost; }
			set
			{
                this._aircraftColorLost = value;
                this.PropertyChanged();
			}
		}

		private Color _aircraftColorDamaged;
		/// <summary>
		/// 艦載機被撃墜スロットの文字色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 0, 0")]
		[Description("艦載機被撃墜スロットの文字色を指定します。")]
		public Color AircraftColorDamaged
		{
			get { return this._aircraftColorDamaged; }
			set
			{
                this._aircraftColorDamaged = value;
                this.PropertyChanged();
			}
		}

		private Color _aircraftColorFull;
		/// <summary>
		/// 艦載機満載スロットの文字色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 0, 0")]
		[Description("艦載機満載スロットの文字色を指定します。")]
		public Color AircraftColorFull
		{
			get { return this._aircraftColorFull; }
			set
			{
                this._aircraftColorFull = value;
                this.PropertyChanged();
			}
		}


		private Color _equipmentLevelColor;
		/// <summary>
		/// 改修レベルの色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "0, 102, 102")]
		[Description("改修レベルの文字色を指定します。")]
		public Color EquipmentLevelColor
		{
			get { return this._equipmentLevelColor; }
			set
			{
                this._equipmentLevelColor = value;
                this.PropertyChanged();
			}
		}

		private Color _aircraftLevelColorLow;
		/// <summary>
		/// 艦載機熟練度の色 ( Lv. 1 ~ Lv. 3 )
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "102, 153, 238")]
		[Description("艦載機熟練度の文字色( Lv. 1 ~ 3 )を指定します。")]
		public Color AircraftLevelColorLow
		{
			get { return this._aircraftLevelColorLow; }
			set
			{
                this._aircraftLevelColorLow = value;
                this.PropertyChanged();
			}
		}

		private Color _aircraftLevelColorHigh;
		/// <summary>
		/// 艦載機熟練度の色 ( Lv. 4 ~ Lv. 7 )
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "255, 170, 0")]
		[Description("艦載機熟練度の文字色( Lv. 4 ~ 7 )を指定します。")]
		public Color AircraftLevelColorHigh
		{
			get { return this._aircraftLevelColorHigh; }
			set
			{
                this._aircraftLevelColorHigh = value;
                this.PropertyChanged();
			}
		}

		private Color _invalidSlotColor;
		private Brush _invalidSlotBrush;
		/// <summary>
		/// 不正スロットの背景色
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "64, 255, 0, 0")]
		[Description("不正スロットの文字色を指定します。")]
		public Color InvalidSlotColor
		{
			get { return this._invalidSlotColor; }
			set
			{
                this._invalidSlotColor = value;

				if (this._invalidSlotBrush != null)
                    this._invalidSlotBrush.Dispose();
                this._invalidSlotBrush = new SolidBrush(this._invalidSlotColor);

                this.PropertyChanged();
			}
		}


		private bool _showAircraft;
		/// <summary>
		/// 艦載機搭載数を表示するか
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(true)]
		[Description("艦載機搭載数を表示するかを指定します。")]
		public bool ShowAircraft
		{
			get { return this._showAircraft; }
			set
			{
                this._showAircraft = value;
                this.PropertyChanged();
			}
		}



		/// <summary>
		/// 装備改修レベル・艦載機熟練度の表示フラグ
		/// </summary>
		public enum LevelVisibilityFlag
		{

			/// <summary> 非表示 </summary>
			Invisible,

			/// <summary> 改修レベルのみ </summary>
			LevelOnly,

			/// <summary> 艦載機熟練度のみ </summary>
			AircraftLevelOnly,

			/// <summary> 改修レベル優先 </summary>
			LevelPriority,

			/// <summary> 艦載機熟練度優先 </summary>
			AircraftLevelPriority,

			/// <summary> 両方表示 </summary>
			Both,

			/// <summary> 両方表示(艦載機熟練度はアイコンにオーバーレイする) </summary>
			AircraftLevelOverlay,
		}

		private LevelVisibilityFlag _levelVisibility;
		/// <summary>
		/// 装備改修レベル・艦載機熟練度の表示フラグ
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(LevelVisibilityFlag.Both)]
		[Description("装備改修レベル・艦載機熟練度の表示方法を指定します。")]
		public LevelVisibilityFlag LevelVisibility
		{
			get { return this._levelVisibility; }
			set
			{
                this._levelVisibility = value;
                this.PropertyChanged();
			}
		}

		private bool _showAircraftLevelByNumber;
		/// <summary>
		/// 艦載機熟練度を数字で表示するか
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(false)]
		[Description("艦載機熟練度を記号ではなく数値で表示するかを指定します。")]
		public bool ShowAircraftLevelByNumber
		{
			get { return this._showAircraftLevelByNumber; }
			set
			{
                this._showAircraftLevelByNumber = value;
                this.PropertyChanged();
			}
		}

		private int _slotMargin;
		/// <summary>
		/// スロット間の空きスペース
		/// </summary>
		[Browsable(true), Category("Appearance"), DefaultValue(3)]
		[Description("スロット間の空きスペースを指定します。")]
		public int SlotMargin
		{
			get { return this._slotMargin; }
			set
			{
                this._slotMargin = value;
                this.PropertyChanged();
			}
		}



		private Brush _overlayBrush = new SolidBrush(Color.FromArgb(0xC0, 0xF0, 0xF0, 0xF0));


		[System.Diagnostics.DebuggerDisplay("[{PreferredSize.Width}, {PreferredSize.Height}]")]
		private class LayoutParameter
		{
			private ShipStatusEquipment Parent;

			/// <summary> 数字2桁分のサイズキャッシュ </summary>
			public Size Digit2Size { get; private set; }

			/// <summary> 装備画像のサイズキャッシュ </summary>
			public Size ImageSize { get; private set; }

			/// <summary> 艦載機数・改修レベル等の表示エリアのサイズキャッシュ </summary>
			public Size InfoAreaSize { get; private set; }

			/// <summary> 1スロットあたりのサイズキャッシュ (マージン含む) </summary>
			public Size SlotUnitSize { get; private set; }

			/// <summary> コントロールの最適サイズのキャッシュ </summary>
			public Size PreferredSize { get; private set; }


			public bool IsAvailable { get; private set; }


			public LayoutParameter(ShipStatusEquipment parent)
			{
                this.Parent = parent;
                this.ResetLayout();
			}


			public void ResetLayout()
			{
                this.Digit2Size = Size.Empty;
                this.ImageSize = Size.Empty;
                this.InfoAreaSize = Size.Empty;
                this.SlotUnitSize = Size.Empty;
                this.PreferredSize = Size.Empty;
                this.IsAvailable = false;
			}

			public void UpdateParameters(Graphics g, Size proposedSize, Font font)
			{

				bool isGraphicsSpecified = g != null;

				if (!this.IsAvailable)
				{
					if (!isGraphicsSpecified)
						g = this.Parent.CreateGraphics();
                    this.Digit2Size = TextRenderer.MeasureText(g, "88", font, new Size(int.MaxValue, int.MaxValue), GetBaseTextFormat() | TextFormatFlags.Top | TextFormatFlags.Right);
				}

				ImageList eqimages = ResourceManager.Instance.Equipments;


				int imageZoomRate = (int)Math.Max(Math.Ceiling(Math.Min(this.Digit2Size.Height * 2.0, (proposedSize.Height > 0 ? proposedSize.Height : int.MaxValue)) / eqimages.ImageSize.Height - 1), 1);
                this.ImageSize = new Size(eqimages.ImageSize.Width * imageZoomRate, eqimages.ImageSize.Height * imageZoomRate);

                // 情報エリア (機数とか熟練度とか) のサイズ
                this.InfoAreaSize = new Size(
					Math.Max(this.Digit2Size.Width, this.ImageSize.Width),
					Math.Min(this.Digit2Size.Height + Math.Max(this.Digit2Size.Height, this.ImageSize.Height / 2), proposedSize.Height));


                // スロット1つ当たりのサイズ(右の余白含む)
                this.SlotUnitSize = new Size(this.ImageSize.Width + this.Parent.SlotMargin, this.ImageSize.Height);
				if (this.Parent.ShowAircraft || this.Parent.LevelVisibility != LevelVisibilityFlag.Invisible)
				{
                    this.SlotUnitSize = new Size(this.SlotUnitSize.Width + this.InfoAreaSize.Width, Math.Max(this.SlotUnitSize.Height, this.InfoAreaSize.Height));
				}

				int slotSize = Math.Max(this.Parent.SlotSize, Array.FindLastIndex(this.Parent.SlotList, sl => sl.EquipmentID > 0) + 1);
                this.PreferredSize = new Size(this.SlotUnitSize.Width * slotSize, this.SlotUnitSize.Height);


				if (!this.IsAvailable && !isGraphicsSpecified)
					g.Dispose();

                this.IsAvailable = true;
			}
		}
		private LayoutParameter LayoutParam;

		#endregion


		private bool IsRefreshSuspended { get; set; }

		public void SuspendUpdate()
		{
            this.IsRefreshSuspended = true;
            this.SuspendLayout();
		}
		public void ResumeUpdate()
		{
            this.ResumeLayout();
			if (this.IsRefreshSuspended)
			{
                this.IsRefreshSuspended = false;
                this.PropertyChanged();
			}
		}



		public ShipStatusEquipment()
		{
            this.IsRefreshSuspended = true;
            this.InitializeComponent();

            this.SlotList = new SlotItem[6];
			for (int i = 0; i < this.SlotList.Length; i++)
			{
                this.SlotList[i] = new SlotItem();
			}
            this._slotSize = 0;

            this._onMouse = false;

			base.Font = new Font("Meiryo UI", 10, FontStyle.Regular, GraphicsUnit.Pixel);

            this._aircraftColorDisabled = Utility.ThemeManager.GetColor(Utility.ThemeColors.SubFontColor);
            this._aircraftColorLost = Color.FromArgb(0xFF, 0x00, 0xFF);
            this._aircraftColorDamaged = Utility.ThemeManager.GetColor(Utility.ThemeColors.RedHighlight);
            this._aircraftColorFull = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);

            this._equipmentLevelColor = Color.FromArgb(0x00, 0x66, 0x66);
            this._aircraftLevelColorLow = Color.FromArgb(0x66, 0x99, 0xEE);
            this._aircraftLevelColorHigh = Color.FromArgb(0xFF, 0xAA, 0x00);

            this._invalidSlotColor = Color.FromArgb(0x40, 0xFF, 0x00, 0x00);
            this._invalidSlotBrush = new SolidBrush(this._invalidSlotColor);

            this._showAircraft = true;

            this._levelVisibility = LevelVisibilityFlag.Both;
            this._showAircraftLevelByNumber = false;

            this._slotMargin = 3;

            this.LayoutParam = new LayoutParameter(this);


            this.IsRefreshSuspended = false;

		}

		/// <summary>
		/// スロット情報を設定します。主に味方艦用です。
		/// </summary>
		/// <param name="ship">当該艦船。</param>
		public void SetSlotList(ShipData ship)
		{

			int slotCount = Math.Max(ship.SlotSize + (ship.IsExpansionSlotAvailable ? 1 : 0), 4);


			if (this.SlotList.Length != slotCount)
			{
                this.SlotList = new SlotItem[slotCount];
				for (int i = 0; i < this.SlotList.Length; i++)
				{
                    this.SlotList[i] = new SlotItem();
				}
			}

			for (int i = 0; i < Math.Min(slotCount, 5); i++)
			{
				var eq = ship.SlotInstance[i];
                this.SlotList[i].EquipmentID = eq?.EquipmentID ?? -1;
                this.SlotList[i].AircraftCurrent = ship.Aircraft[i];
                this.SlotList[i].AircraftMax = ship.MasterShip.Aircraft[i];
                this.SlotList[i].Level = eq?.Level ?? 0;
                this.SlotList[i].AircraftLevel = eq?.AircraftLevel ?? 0;
			}

			if (ship.IsExpansionSlotAvailable)
			{
				var eq = ship.ExpansionSlotInstance;
                this.SlotList[ship.SlotSize].EquipmentID = eq?.EquipmentID ?? -1;
                this.SlotList[ship.SlotSize].AircraftCurrent =
                this.SlotList[ship.SlotSize].AircraftMax = 0;
                this.SlotList[ship.SlotSize].Level = eq?.Level ?? 0;
                this.SlotList[ship.SlotSize].AircraftLevel = eq?.AircraftLevel ?? 0;
			}


            this._slotSize = ship.SlotSize + (ship.IsExpansionSlotAvailable ? 1 : 0);

            this.PropertyChanged();
		}

		/// <summary>
		/// スロット情報を設定します。主に敵艦用です。
		/// </summary>
		/// <param name="ship">当該艦船。</param>
		public void SetSlotList(ShipDataMaster ship)
		{


			if (this.SlotList.Length != ship.Aircraft.Count)
			{
                this.SlotList = new SlotItem[ship.Aircraft.Count];
				for (int i = 0; i < this.SlotList.Length; i++)
				{
                    this.SlotList[i] = new SlotItem();
				}
			}

			for (int i = 0; i < this.SlotList.Length; i++)
			{
                this.SlotList[i].EquipmentID = ship.DefaultSlot == null ? -1 : (i < ship.DefaultSlot.Count ? ship.DefaultSlot[i] : -1);
                this.SlotList[i].AircraftCurrent =
                this.SlotList[i].AircraftMax = ship.Aircraft[i];
                this.SlotList[i].Level =
                this.SlotList[i].AircraftLevel = 0;
			}

            this._slotSize = ship.SlotSize;

            this.PropertyChanged();
		}

		/// <summary>
		/// スロット情報を設定します。主に演習の敵艦用です。
		/// </summary>
		/// <param name="shipID">艦船ID。</param>
		/// <param name="slot">装備スロット。</param>
		public void SetSlotList(int shipID, int[] slot)
		{

			ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];

			int slotLength = slot?.Length ?? 0;

			if (this.SlotList.Length != slotLength)
			{
                this.SlotList = new SlotItem[slotLength];
				for (int i = 0; i < this.SlotList.Length; i++)
				{
                    this.SlotList[i] = new SlotItem();
				}
			}

			for (int i = 0; i < this.SlotList.Length; i++)
			{
                this.SlotList[i].EquipmentID = slot[i];
                this.SlotList[i].AircraftCurrent = ship.Aircraft[i];
                this.SlotList[i].AircraftMax = ship.Aircraft[i];
                this.SlotList[i].Level =
                this.SlotList[i].AircraftLevel = 0;
			}

            this._slotSize = ship?.SlotSize ?? 0;

            this.PropertyChanged();
		}

		public void SetSlotList(BaseAirCorpsData corps)
		{

			int slotLength = corps?.Squadrons?.Count() ?? 0;

			if (this.SlotList.Length != slotLength)
			{
                this.SlotList = new SlotItem[slotLength];
				for (int i = 0; i < this.SlotList.Length; i++)
				{
                    this.SlotList[i] = new SlotItem();
				}
			}

			for (int i = 0; i < this.SlotList.Length; i++)
			{
				var squadron = corps[i + 1];
				var eq = squadron.EquipmentInstance;

				switch (squadron.State)
				{
					case 0:     // 未配属
					case 2:     // 配置転換中
					default:
                        this.SlotList[i].EquipmentID = -1;
                        this.SlotList[i].AircraftCurrent =
                        this.SlotList[i].AircraftMax =
                        this.SlotList[i].Level =
                        this.SlotList[i].AircraftLevel = 0;
						break;
					case 1:     // 配属済み
						if (eq == null)
							goto case 0;
                        this.SlotList[i].EquipmentID = eq.EquipmentID;
                        this.SlotList[i].AircraftCurrent = squadron.AircraftCurrent;
                        this.SlotList[i].AircraftMax = squadron.AircraftMax;
                        this.SlotList[i].Level = eq.Level;
                        this.SlotList[i].AircraftLevel = eq.AircraftLevel;
						break;
				}

			}

            this._slotSize = slotLength;

            this.PropertyChanged();
		}


		private void PropertyChanged()
		{

			if (this.IsRefreshSuspended)
				return;

            this.LayoutParam.ResetLayout();
            this.PerformLayout();

            this.Refresh();
		}


		private void ShipStatusEquipment_Paint(object sender, PaintEventArgs e)
		{

			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

			Rectangle basearea = new Rectangle(this.Padding.Left, this.Padding.Top, this.Width - this.Padding.Horizontal, this.Height - this.Padding.Vertical);
			//e.Graphics.DrawRectangle( Pens.Magenta, basearea.X, basearea.Y, basearea.Width - 1, basearea.Height - 1 );

			ImageList eqimages = ResourceManager.Instance.Equipments;

			TextFormatFlags textformatBottomRight = GetBaseTextFormat() | TextFormatFlags.Bottom | TextFormatFlags.Right;
			TextFormatFlags textformatTopLeft = GetBaseTextFormat() | TextFormatFlags.Top | TextFormatFlags.Left;
			TextFormatFlags textformatTopRight = GetBaseTextFormat() | TextFormatFlags.Top | TextFormatFlags.Right;


            this.LayoutParam.UpdateParameters(e.Graphics, basearea.Size, this.Font);


			for (int slotindex = 0; slotindex < this.SlotList.Length; slotindex++)
			{

				SlotItem slot = this.SlotList[slotindex];

				Image image = null;

				var origin = new Point(basearea.X + this.LayoutParam.SlotUnitSize.Width * slotindex, basearea.Y);


				if (slotindex >= this.SlotSize && slot.EquipmentID != -1)
				{
					//invalid!
					e.Graphics.FillRectangle(this._invalidSlotBrush, new Rectangle(origin, this.LayoutParam.SlotUnitSize));
				}


				if (slot.EquipmentID == -1)
				{
					if (slotindex < this.SlotSize)
					{
						//nothing
						image = eqimages.Images[(int)ResourceManager.EquipmentContent.Nothing];

					}
					else
					{
						//locked
						image = eqimages.Images[(int)ResourceManager.EquipmentContent.Locked];
					}

				}
				else
				{
					int imageID = slot.EquipmentIconID;
					if (imageID <= 0 || (int)ResourceManager.EquipmentContent.Locked <= imageID)
						imageID = (int)ResourceManager.EquipmentContent.Unknown;

					image = eqimages.Images[imageID];
				}


				Rectangle imagearea = new Rectangle(origin.X, origin.Y + (this.LayoutParam.SlotUnitSize.Height - this.LayoutParam.ImageSize.Height) / 2, this.LayoutParam.ImageSize.Width, this.LayoutParam.ImageSize.Height);
				if (image != null)
				{
					e.Graphics.DrawImage(image, imagearea);
				}

				Color aircraftColor		= this.AircraftColorDisabled;
				bool drawAircraftSlot	= this.ShowAircraft;

				if (slot.AircraftMax == 0)
				{
					if (slot.Equipment?.IsAircraft ?? false)
					{
						aircraftColor = this.AircraftColorDisabled;
					}
					else
					{
						drawAircraftSlot = false;
					}

				}
				else if (slot.AircraftCurrent == 0)
				{
					aircraftColor = this.AircraftColorLost;

				}
				else if (slot.AircraftCurrent < slot.AircraftMax)
				{
					aircraftColor = this.AircraftColorDamaged;

				}
				else if (!(slot.Equipment?.IsAircraft ?? false))
				{
					aircraftColor = this.AircraftColorDisabled;

				}
				else
				{
					aircraftColor = this.AircraftColorFull;
				}

				// 艦載機数描画
				if (drawAircraftSlot)
				{
					Rectangle textarea = new Rectangle(origin.X + this.LayoutParam.ImageSize.Width, origin.Y + this.LayoutParam.InfoAreaSize.Height * 3 / 4 - this.LayoutParam.Digit2Size.Height / 2,
                        this.LayoutParam.InfoAreaSize.Width, this.LayoutParam.Digit2Size.Height);
					//e.Graphics.DrawRectangle( Pens.Cyan, textarea );

					if (slot.AircraftCurrent < 10)
					{
						//1桁なら画像に近づける

						textarea.Width -= this.LayoutParam.Digit2Size.Width / 2;

					}
					else if (slot.AircraftCurrent >= 100)
					{
						//3桁以上ならオーバーレイを入れる

						Size sz_realstr = TextRenderer.MeasureText(e.Graphics, slot.AircraftCurrent.ToString(), this.Font, new Size(int.MaxValue, int.MaxValue), textformatBottomRight);

						textarea.X -= sz_realstr.Width - textarea.Width;
						textarea.Width = sz_realstr.Width;

						e.Graphics.FillRectangle(this._overlayBrush, textarea);
					}

					TextRenderer.DrawText(e.Graphics, slot.AircraftCurrent.ToString(), this.Font, textarea, aircraftColor, textformatBottomRight);
				}

				// 改修レベル描画
				if (slot.Level > 0)
				{

					if (this.LevelVisibility == LevelVisibilityFlag.LevelOnly ||
                        this.LevelVisibility == LevelVisibilityFlag.Both ||
                        this.LevelVisibility == LevelVisibilityFlag.AircraftLevelOverlay ||
						(this.LevelVisibility == LevelVisibilityFlag.LevelPriority && (!this._onMouse || slot.AircraftLevel == 0)) ||
						(this.LevelVisibility == LevelVisibilityFlag.AircraftLevelPriority && (this._onMouse || slot.AircraftLevel == 0)))
					{

						TextRenderer.DrawText(e.Graphics, slot.Level >= 10 ? "★" : "+" + slot.Level, this.Font,
							new Rectangle(origin.X + this.LayoutParam.ImageSize.Width, origin.Y + this.LayoutParam.InfoAreaSize.Height * 1 / 4 - this.LayoutParam.Digit2Size.Height / 2,
                                this.LayoutParam.InfoAreaSize.Width, this.LayoutParam.Digit2Size.Height), this.EquipmentLevelColor, textformatTopRight);

					}

				}


				// 艦載機熟練度描画
				if (slot.AircraftLevel > 0)
				{

					if (this.LevelVisibility == LevelVisibilityFlag.AircraftLevelOnly ||
                        this.LevelVisibility == LevelVisibilityFlag.Both ||
						(this.LevelVisibility == LevelVisibilityFlag.AircraftLevelPriority && (!this._onMouse || slot.Level == 0)) ||
						(this.LevelVisibility == LevelVisibilityFlag.LevelPriority && (this._onMouse || slot.Level == 0)))
					{
						// 右上に描画

						if (this.ShowAircraftLevelByNumber)
						{
							var area = new Rectangle(origin.X + this.LayoutParam.ImageSize.Width, origin.Y + this.LayoutParam.InfoAreaSize.Height * 1 / 4 - this.LayoutParam.Digit2Size.Height / 2,
                                this.LayoutParam.InfoAreaSize.Width, this.LayoutParam.Digit2Size.Height);
							TextRenderer.DrawText(e.Graphics, slot.AircraftLevel.ToString(), this.Font, area, this.GetAircraftLevelColor(slot.AircraftLevel), textformatTopRight);

						}
						else
						{
							var area = new Rectangle(origin.X + this.LayoutParam.ImageSize.Width, origin.Y,
                                this.LayoutParam.ImageSize.Width, this.LayoutParam.ImageSize.Height);
							e.Graphics.DrawImage(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.AircraftLevelTop0 + slot.AircraftLevel], area);
						}


					}
					else if (this.LevelVisibility == LevelVisibilityFlag.AircraftLevelOverlay)
					{
						// 左上に描画

						if (this.ShowAircraftLevelByNumber)
						{
							var area = new Rectangle(origin.X, origin.Y, this.LayoutParam.Digit2Size.Width / 2, this.LayoutParam.Digit2Size.Height);
							e.Graphics.FillRectangle(this._overlayBrush, area);
							TextRenderer.DrawText(e.Graphics, slot.AircraftLevel.ToString(), this.Font, area, this.GetAircraftLevelColor(slot.AircraftLevel), textformatTopLeft);
						}
						else
						{
							e.Graphics.FillRectangle(this._overlayBrush, new Rectangle(origin.X, origin.Y, this.LayoutParam.ImageSize.Width, this.LayoutParam.ImageSize.Height / 2));
							e.Graphics.DrawImage(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.AircraftLevelTop0 + slot.AircraftLevel], new Rectangle(origin, this.LayoutParam.ImageSize));
						}
					}
                }
            }
        }

		public override Size GetPreferredSize(Size proposedSize)
		{
            this.LayoutParam.UpdateParameters(null, proposedSize, this.Font);

			return this.LayoutParam.PreferredSize + this.Padding.Size;
		}

		private static TextFormatFlags GetBaseTextFormat()
		{
			return TextFormatFlags.NoPadding;
		}

		private Color GetAircraftLevelColor(int level)
		{
			if (level <= 3)
				return this.AircraftLevelColorLow;
			else
				return this.AircraftLevelColorHigh;
		}

		private void ShipStatusEquipment_MouseEnter(object sender, EventArgs e)
		{
            this._onMouse = true;
			if (this.LevelVisibility == LevelVisibilityFlag.LevelPriority || this.LevelVisibility == LevelVisibilityFlag.AircraftLevelPriority)
                this.PropertyChanged();
		}

		private void ShipStatusEquipment_MouseLeave(object sender, EventArgs e)
		{
            this._onMouse = false;
			if (this.LevelVisibility == LevelVisibilityFlag.LevelPriority || this.LevelVisibility == LevelVisibilityFlag.AircraftLevelPriority)
                this.PropertyChanged();
		}

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
            this._overlayBrush.Dispose();
            this._invalidSlotBrush.Dispose();

			// --- auto generated
			if (disposing && (this.components != null))
			{
                this.components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
