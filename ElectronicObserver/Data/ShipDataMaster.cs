using ElectronicObserver.Resource.Record;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Window;
namespace ElectronicObserver.Data
{


	/// <summary>
	/// 艦船のマスターデータを保持します。
	/// </summary>
	public class ShipDataMaster : ResponseWrapper, IIdentifiable
	{

		/// <summary>
		/// 艦船ID
		/// </summary>
		public int ShipID => (int)this.RawData.api_id;

		/// <summary>
		/// 図鑑番号
		/// </summary>
		public int AlbumNo => !this.RawData.api_sortno() ? 0 : (int)this.RawData.api_sortno;

        /// <summary>
		/// 母港ソート順
		/// </summary>
		public int SortID => !this.RawData.api_sort_id() ? 0 : (int)this.RawData.api_sort_id;

        /// <summary>
        /// 名前 번역됨
        /// </summary>
        public string Name
        {
            get { return FormMain.Instance.Translator.GetTranslation(this.RawData.api_name, Utility.DataType.ShipName); }
        }

        public string Name_JP => this.RawData.api_name;


        /// <summary>
        /// 読み
        /// </summary>
        public string NameReading => this.RawData.api_yomi;

		/// <summary>
		/// 艦種
		/// </summary>
		public ShipTypes ShipType => (ShipTypes)(int)this.RawData.api_stype;

        /// <summary>
        /// 艦型
        /// </summary>
        public int ShipClass => (int)this.RawData.api_ctype;


		/// <summary>
		/// 改装Lv.
		/// </summary>
		public int RemodelAfterLevel => !this.RawData.api_afterlv() ? 0 : (int)this.RawData.api_afterlv;

		/// <summary>
		/// 改装後の艦船ID
		/// 0=なし
		/// </summary>
		public int RemodelAfterShipID => !this.RawData.api_aftershipid() ? 0 : int.Parse((string)this.RawData.api_aftershipid);

		/// <summary>
		/// 改装後の艦船
		/// </summary>
		public ShipDataMaster RemodelAfterShip => this.RemodelAfterShipID > 0 ? KCDatabase.Instance.MasterShips[this.RemodelAfterShipID] : null;


		/// <summary>
		/// 改装前の艦船ID
		/// 0=なし
		/// </summary>
		public int RemodelBeforeShipID { get; internal set; }

		/// <summary>
		/// 改装前の艦船
		/// </summary>
		public ShipDataMaster RemodelBeforeShip => this.RemodelBeforeShipID > 0 ? KCDatabase.Instance.MasterShips[this.RemodelBeforeShipID] : null;


		/// <summary>
		/// 改装に必要な弾薬
		/// </summary>
		public int RemodelAmmo => !this.RawData.api_afterbull() ? 0 : (int)this.RawData.api_afterbull;

		/// <summary>
		/// 改装に必要な鋼材
		/// </summary>
		public int RemodelSteel => !this.RawData.api_afterfuel() ? 0 : (int)this.RawData.api_afterfuel;

		/// <summary>
		/// 改装に改装設計図が必要かどうか
		/// </summary>
		public int NeedBlueprint { get; internal set; }

		/// <summary>
		/// 改装に試製甲板カタパルトが必要かどうか
		/// </summary>
		public int NeedCatapult { get; internal set; }

        /// <summary>
        /// 改装に戦闘詳報が必要かどうか
        /// </summary>
        public int NeedActionReport { get; internal set; }


		#region Parameters

		/// <summary>
		/// 耐久初期値
		/// </summary>
		public int HPMin
		{
			get
			{
				if (this.RawData.api_taik())
				{
					return (int)this.RawData.api_taik[0];
				}
				else
				{
					return this.GetParameterElement()?.HPMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 耐久最大値
		/// </summary>
		public int HPMax
		{
			get
			{
				if (this.RawData.api_taik())
				{
					return (int)this.RawData.api_taik[1];
				}
				else
				{
					return this.GetParameterElement()?.HPMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 装甲初期値
		/// </summary>
		public int ArmorMin
		{
			get
			{
				if (this.RawData.api_souk())
				{
					return (int)this.RawData.api_souk[0];
				}
				else
				{
					return this.GetParameterElement()?.ArmorMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 装甲最大値
		/// </summary>
		public int ArmorMax
		{
			get
			{
				if (this.RawData.api_souk())
				{
					return (int)this.RawData.api_souk[1];
				}
				else
				{
					return this.GetParameterElement()?.ArmorMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 火力初期値
		/// </summary>
		public int FirepowerMin
		{
			get
			{
				if (this.RawData.api_houg())
				{
					return (int)this.RawData.api_houg[0];
				}
				else
				{
					return this.GetParameterElement()?.FirepowerMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 火力最大値
		/// </summary>
		public int FirepowerMax
		{
			get
			{
				if (this.RawData.api_houg())
				{
					return (int)this.RawData.api_houg[1];
				}
				else
				{
					return this.GetParameterElement()?.FirepowerMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 雷装初期値
		/// </summary>
		public int TorpedoMin
		{
			get
			{
				if (this.RawData.api_raig())
				{
					return (int)this.RawData.api_raig[0];
				}
				else
				{
					return this.GetParameterElement()?.TorpedoMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 雷装最大値
		/// </summary>
		public int TorpedoMax
		{
			get
			{
				if (this.RawData.api_raig())
				{
					return (int)this.RawData.api_raig[1];
				}
				else
				{
					return this.GetParameterElement()?.TorpedoMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 対空初期値
		/// </summary>
		public int AAMin
		{
			get
			{
				if (this.RawData.api_tyku())
				{
					return (int)this.RawData.api_tyku[0];
				}
				else
				{
					return this.GetParameterElement()?.AAMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 対空最大値
		/// </summary>
		public int AAMax
		{
			get
			{
				if (this.RawData.api_tyku())
				{
					return (int)this.RawData.api_tyku[1];
				}
				else
				{
					return this.GetParameterElement()?.AAMax ?? 0;
				}
			}
		}


		/// <summary>
		/// 対潜
		/// </summary>
		public ShipParameterRecord.Parameter ASW => this.GetParameterElement()?.ASW;

		/// <summary>
		/// 回避
		/// </summary>
		public ShipParameterRecord.Parameter Evasion => this.GetParameterElement()?.Evasion;

		/// <summary>
		/// 索敵
		/// </summary>
		public ShipParameterRecord.Parameter LOS => this.GetParameterElement()?.LOS;


		/// <summary>
		/// 運初期値
		/// </summary>
		public int LuckMin
		{
			get
			{
				if (this.RawData.api_luck())
				{
					return (int)this.RawData.api_luck[0];
				}
				else
				{
					return this.GetParameterElement()?.LuckMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 運最大値
		/// </summary>
		public int LuckMax
		{
			get
			{
				if (this.RawData.api_luck())
				{
					return (int)this.RawData.api_luck[1];
				}
				else
				{
					return this.GetParameterElement()?.LuckMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 速力
		/// 0=陸上基地, 5=低速, 10=高速
		/// </summary>
		public int Speed => (int)this.RawData.api_soku;

		/// <summary>
		/// 射程
		/// </summary>
		public int Range
		{
			get
			{
				if (this.RawData.api_leng())
				{
					return (int)this.RawData.api_leng;
				}
				else
				{
					return this.GetParameterElement()?.Range ?? 0;
				}
			}
		}
		#endregion


		/// <summary>
		/// 装備スロットの数
		/// </summary>
		public int SlotSize => (int)this.RawData.api_slot_num;

		/// <summary>
		/// 各スロットの航空機搭載数
		/// </summary>
		public ReadOnlyCollection<int> Aircraft
		{
			get
			{
				if (this.RawData.api_maxeq())
				{
					return Array.AsReadOnly((int[])this.RawData.api_maxeq);
				}
				else
				{
					var p = this.GetParameterElement();
					if (p != null && p.Aircraft != null)
						return Array.AsReadOnly(p.Aircraft);
					else
						return Array.AsReadOnly(new[] { 0, 0, 0, 0, 0 });
				}
			}
		}

		/// <summary>
		/// 搭載
		/// </summary>
		public int AircraftTotal => this.Aircraft.Sum(a => Math.Max(a, 0));


		/// <summary>
		/// 初期装備のID
		/// </summary>
		public ReadOnlyCollection<int> DefaultSlot
		{
			get
			{
				var p = this.GetParameterElement();
				if (p != null && p.DefaultSlot != null)
					return Array.AsReadOnly(p.DefaultSlot);
				else
					return null;
			}
		}

        internal int[] specialEquippableCategory = null;
        /// <summary>
        /// 特殊装備カテゴリ　指定がない場合は null
        /// </summary>
        public IEnumerable<int> SpecialEquippableCategories => this.specialEquippableCategory;
        /// <summary>
        /// 装備可能なカテゴリ
        /// </summary>
        public IEnumerable<int> EquippableCategories
        {
            get
            {
                if (this.specialEquippableCategory != null)
                    return this.SpecialEquippableCategories;
                else
                    return KCDatabase.Instance.ShipTypes[(int)this.ShipType].EquippableCategories;
            }
        }


        /// <summary>
        /// 建造時間(分)
        /// </summary>
        public int BuildingTime => !this.RawData.api_buildtime() ? 0 : (int)this.RawData.api_buildtime;


		/// <summary>
		/// 解体資材
		/// </summary>
		public ReadOnlyCollection<int> Material => Array.AsReadOnly(!this.RawData.api_broken() ? new[] { 0, 0, 0, 0 } : (int[])this.RawData.api_broken);

		/// <summary>
		/// 近代化改修の素材にしたとき上昇するパラメータの量
		/// </summary>
		public ReadOnlyCollection<int> PowerUp => Array.AsReadOnly(!this.RawData.api_powup() ? new[] { 0, 0, 0, 0 } : (int[])this.RawData.api_powup);

		/// <summary>
		/// レアリティ
		/// </summary>
		public int Rarity => !this.RawData.api_backs() ? 0 : (int)this.RawData.api_backs;

		/// <summary>
		/// ドロップ/ログイン時のメッセージ
		/// </summary>
		public string MessageGet => this.GetParameterElement()?.MessageGet?.Replace("<br>", "\r\n") ?? "";

		/// <summary>
		/// 艦船名鑑でのメッセージ
		/// </summary>
		public string MessageAlbum => this.GetParameterElement()?.MessageAlbum?.Replace("<br>", "\r\n") ?? "";


		/// <summary>
		/// 搭載燃料
		/// </summary>
		public int Fuel => !this.RawData.api_fuel_max() ? 0 : (int)this.RawData.api_fuel_max;

		/// <summary>
		/// 搭載弾薬
		/// </summary>
		public int Ammo => !this.RawData.api_bull_max() ? 0 : (int)this.RawData.api_bull_max;


		/// <summary>
		/// ボイス再生フラグ
		/// </summary>
		public int VoiceFlag => !this.RawData.api_voicef() ? 0 : (int)this.RawData.api_voicef;


        /// <summary>
        /// グラフィック設定データへの参照
        /// </summary>
        public ShipGraphicData GraphicData => KCDatabase.Instance.ShipGraphics[this.ShipID];

        /// <summary>
        /// リソースのファイル/フォルダ名
        /// </summary>
        public string ResourceName => this.GraphicData?.ResourceName ?? "";

        /// <summary>
        /// 画像リソースのバージョン
        /// </summary>
        public string ResourceGraphicVersion => this.GraphicData?.GraphicVersion ?? "";

        /// <summary>
        /// ボイスリソースのバージョン
        /// </summary>
		public string ResourceVoiceVersion => this.GraphicData?.VoiceVersion ?? "";

        /// <summary>
        /// 母港ボイスリソースのバージョン
        /// </summary>
        public string ResourcePortVoiceVersion => this.GraphicData?.PortVoiceVersion ?? "";

        /// <summary>
        /// 衣替え艦：ベースとなる艦船ID
        /// </summary>
        public int OriginalCostumeShipID => this.GetParameterElement()?.OriginalCostumeShipID ?? -1;



		//以下、自作計算プロパティ群

		public static readonly int HPModernizableLimit = 2;
		public static readonly int ASWModernizableLimit = 9;


		/// <summary>
		/// ケッコンカッコカリ後のHP
		/// </summary>
		public int HPMaxMarried
		{
			get
			{
				int incr;
				if (this.HPMin < 30) incr = 4;
				else if (this.HPMin < 40) incr = 5;
				else if (this.HPMin < 50) incr = 6;
				else if (this.HPMin < 70) incr = 7;
				else if (this.HPMin < 90) incr = 8;
				else incr = 9;

				return Math.Min(this.HPMin + incr, this.HPMax);
			}
		}

		/// <summary>
		/// HP改修可能値(未婚時)
		/// </summary>
		public int HPMaxModernizable => Math.Min(this.HPMax - this.HPMin, HPModernizableLimit);

		/// <summary>
		/// HP改修可能値(既婚時)
		/// </summary>
		public int HPMaxMarriedModernizable => Math.Min(this.HPMax - this.HPMaxMarried, HPModernizableLimit);

		/// <summary>
		/// 近代化改修後のHP(未婚時)
		/// </summary>
		public int HPMaxModernized => Math.Min(this.HPMin + this.HPMaxModernizable, this.HPMax);


		/// <summary>
		/// 近代化改修後のHP(既婚時)
		/// </summary>
		public int HPMaxMarriedModernized => Math.Min(this.HPMaxMarried + this.HPMaxModernizable, this.HPMax);



		/// <summary>
		/// 対潜改修可能値
		/// </summary>
		public int ASWModernizable => this.ASW == null || this.ASW.Maximum == 0 ? 0 : ASWModernizableLimit;


		/// <summary>
		/// 深海棲艦かどうか
		/// </summary>
		public bool IsAbyssalShip => this.ShipID > 1500;


		/// <summary>
		/// クラスも含めた艦名
		/// </summary>
		public string NameWithClass
		{
			get
			{
				if (!this.IsAbyssalShip || this.NameReading == "" || this.NameReading == "-")
					return this.Name;
				else
					return $"{this.Name} {this.NameReading}";
			}
		}

		/// <summary>
		/// 陸上基地かどうか
		/// </summary>
		public bool IsLandBase => this.Speed == 0;

        public ShipType ShipTypeInstance => KCDatabase.Instance.ShipTypes[(int)this.ShipType];

        /// <summary>
        /// 図鑑に載っているか
        /// </summary>
        public bool IsListedInAlbum => 0 < this.AlbumNo && this.AlbumNo <= 420;


        /// <summary>
        /// 改装段階
        /// 初期 = 0, 改 = 1, 改二 = 2, ...
        /// </summary>
        public int RemodelTier
		{
			get
			{
				int tier = 0;
				var ship = this;
				while (ship.RemodelBeforeShip != null)
				{
					tier++;
					ship = ship.RemodelBeforeShip;
				}

				return tier;
			}
		}


        /// <summary>
        /// 艦種名 번역됨
        /// </summary>
        public string ShipTypeName
        {
            get { return FormMain.Instance.Translator.GetTranslation(KCDatabase.Instance.ShipTypes[(int)this.ShipType].Name, Utility.DataType.ShipType); }
        }

        /// <summary>
        /// 潜水艦系か (潜水艦/潜水空母)
        /// </summary>
        public bool IsSubmarine => this.ShipType == ShipTypes.Submarine || this.ShipType == ShipTypes.SubmarineAircraftCarrier;

		/// <summary>
		/// 空母系か (軽空母/正規空母/装甲空母)
		/// </summary>
		public bool IsAircraftCarrier => this.ShipType == ShipTypes.LightAircraftCarrier || this.ShipType == ShipTypes.AircraftCarrier || this.ShipType == ShipTypes.ArmoredAircraftCarrier;

        /// <summary>
        /// 護衛空母か
        /// </summary>
        public bool IsEscortAircraftCarrier => ShipType == ShipTypes.LightAircraftCarrier && ASW.Minimum > 0;

        /// <summary>
        /// 自身のパラメータレコードを取得します。
        /// </summary>
        /// <returns></returns>
        private ShipParameterRecord.ShipParameterElement GetParameterElement()
		{
			return RecordManager.Instance.ShipParameter[this.ShipID];
		}



		private static readonly Color[] ShipNameColors = new Color[] {
            Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor),
            Color.FromArgb( 0xFF, 0x00, 0x00 ),
			Color.FromArgb( 0xFF, 0x88, 0x00 ),
			Color.FromArgb( 0x00, 0x66, 0x00 ),
			Color.FromArgb( 0x88, 0x00, 0x00 ),
			Color.FromArgb( 0x00, 0x88, 0xFF ),
			Color.FromArgb( 0x00, 0x00, 0xFF ),
		};

		public Color GetShipNameColor()
		{

			if (!this.IsAbyssalShip)
			{
				return Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            }

			bool isLateModel = this.Name.Contains("후기형");
			bool isRemodeled = this.Name.Contains("개");
			bool isDestroyed = this.Name.Contains("-괴");
			bool isDemon = this.Name.Contains("귀");
			bool isPrincess = this.Name.Contains("희");
			bool isWaterDemon = this.Name.Contains("수귀");
			bool isWaterPrincess = this.Name.Contains("수희");
			bool isElite = this.NameReading == "elite";
			bool isFlagship = this.NameReading == "flagship";


			if (isDestroyed)
				return Color.FromArgb(0xFF, 0x00, 0xFF);

			else if (isWaterPrincess)
				return ShipNameColors[6];
			else if (isWaterDemon)
				return ShipNameColors[5];
			else if (isPrincess)
				return ShipNameColors[4];
			else if (isDemon)
				return ShipNameColors[3];
			else
			{

				int tier;

				if (isFlagship)
					tier = 2;
				else if (isElite)
					tier = 1;
				else
					tier = 0;

				if (isLateModel || isRemodeled)
					tier += 3;

				return ShipNameColors[tier];
			}
		}


		public ShipDataMaster()
		{
            this.RemodelBeforeShipID = 0;
		}

        public Color GetShipNameColor(bool uis)
        {

            if (!this.IsAbyssalShip)
            {
                return SystemColors.ControlText;
            }

            bool isLateModel = this.Name.Contains("후기형");
            bool isRemodeled = this.Name.Contains("개");
            bool isDestroyed = this.Name.EndsWith("-괴");
            bool isDemon = this.Name.EndsWith("귀");
            bool isPrincess = this.Name.EndsWith("희");
            bool isWaterDemon = this.Name.EndsWith("수귀");
            bool isWaterPrincess = this.Name.EndsWith("수희");
            bool isElite = this.NameReading == "elite";
            bool isFlagship = this.NameReading == "flagship";


            if (isDestroyed)
                return Color.FromArgb(0xFF, 0x00, 0xFF);

            else if (isWaterPrincess)
                return ShipNameColors[6];
            else if (isWaterDemon)
                return ShipNameColors[5];
            else if (isPrincess)
                return ShipNameColors[4];
            else if (isDemon)
                return ShipNameColors[3];
            else
            {

                int tier;

                if (isFlagship)
                    tier = 2;
                else if (isElite)
                    tier = 1;
                else
                    tier = 0;

                if (isLateModel || isRemodeled)
                    tier += 3;

                return ShipNameColors[tier];
            }
        }

        public int ID => this.ShipID;


		public override string ToString() => $"[{this.ShipID}] {this.NameWithClass}";


	}

}
