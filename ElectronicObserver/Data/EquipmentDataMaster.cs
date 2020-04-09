using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Window;

namespace ElectronicObserver.Data
{


	/// <summary>
	/// 装備のマスターデータを保持します。
	/// </summary>
	public class EquipmentDataMaster : ResponseWrapper, IIdentifiable
	{

		/// <summary>
		/// 装備ID
		/// </summary>
		public int EquipmentID => (int)this.RawData.api_id;

		/// <summary>
		/// 図鑑番号
		/// </summary>
		public int AlbumNo => (int)this.RawData.api_sortno;

        /// <summary>
        /// 名前 번역됨
        /// </summary>
        public string Name
        {
            get {
                return FormMain.Instance.Translator.GetTranslation(this.RawData.api_name, Utility.DataType.Equipment); }
        }

        public string Name_JP => this.RawData.api_name;


        /// <summary>
        /// 装備種別
        /// </summary>
        public ReadOnlyCollection<int> EquipmentType => Array.AsReadOnly((int[])this.RawData.api_type);



		#region Parameters

		/// <summary>
		/// 装甲
		/// </summary>
		public int Armor => (int)this.RawData.api_souk;

		/// <summary>
		/// 火力
		/// </summary>
		public int Firepower => (int)this.RawData.api_houg;

		/// <summary>
		/// 雷装
		/// </summary>
		public int Torpedo => (int)this.RawData.api_raig;

		/// <summary>
		/// 爆装
		/// </summary>
		public int Bomber => (int)this.RawData.api_baku;

		/// <summary>
		/// 対空
		/// </summary>
		public int AA => (int)this.RawData.api_tyku;

		/// <summary>
		/// 対潜
		/// </summary>
		public int ASW => (int)this.RawData.api_tais;

		/// <summary>
		/// 命中 / 対爆
		/// </summary>
		public int Accuracy => (int)this.RawData.api_houm;

		/// <summary>
		/// 回避 / 迎撃
		/// </summary>
		public int Evasion => (int)this.RawData.api_houk;

		/// <summary>
		/// 索敵
		/// </summary>
		public int LOS => (int)this.RawData.api_saku;

		/// <summary>
		/// 運
		/// </summary>
		public int Luck => (int)this.RawData.api_luck;

		/// <summary>
		/// 射程
		/// </summary>
		public int Range => (int)this.RawData.api_leng;

		#endregion


		/// <summary>
		/// レアリティ
		/// </summary>
		public int Rarity => (int)this.RawData.api_rare;

		/// <summary>
		/// 廃棄資材
		/// </summary>
		public ReadOnlyCollection<int> Material => Array.AsReadOnly((int[])this.RawData.api_broken);

        /// <summary>
        /// 図鑑説明
        /// </summary>
        public string Message => this.RawData.api_info() ? ((string)this.RawData.api_info).Replace("<br>", "\r\n") : "";
        /// <summary>
        /// 基地航空隊：配置コスト
        /// </summary>
        public int AircraftCost => this.RawData.api_cost() ? (int)this.RawData.api_cost : 0;


		/// <summary>
		/// 基地航空隊：戦闘行動半径
		/// </summary>
		public int AircraftDistance => this.RawData.api_distance() ? (int)this.RawData.api_distance : 0;



		/// <summary>
		/// 深海棲艦専用装備かどうか
		/// </summary>
		public bool IsAbyssalEquipment => this.EquipmentID > 500;


		/// <summary>
		/// 図鑑に載っているか
		/// </summary>
		public bool IsListedInAlbum => this.AlbumNo > 0;


		/// <summary>
		/// 装備種別：小分類
		/// </summary>
		public int CardType => (int)this.RawData.api_type[1];

		/// <summary>
		/// 装備種別：カテゴリ
		/// </summary>
		public EquipmentTypes CategoryType => (EquipmentTypes)(int)this.RawData.api_type[2];

		/// <summary>
		/// 装備種別：カテゴリ
		/// </summary>
		public EquipmentType CategoryTypeInstance => KCDatabase.Instance.EquipmentTypes[(int)this.CategoryType];

		/// <summary>
		/// 装備種別：アイコン
		/// </summary>
		public int IconType => (int)this.RawData.api_type[3];

        internal int[] equippableShipsAtExpansion = new int[0];
        /// <summary>
        /// 拡張スロットに装備可能な艦船IDのリスト
        /// </summary>
        public IEnumerable<int> EquippableShipsAtExpansion => this.equippableShipsAtExpansion;


        // 以降自作判定
        // note: icontype の扱いについては再考の余地あり

        /// <summary> 砲系かどうか </summary>
        public bool IsGun =>
            this.CategoryType == EquipmentTypes.MainGunSmall ||
            this.CategoryType == EquipmentTypes.MainGunMedium ||
            this.CategoryType == EquipmentTypes.MainGunLarge ||
            this.CategoryType == EquipmentTypes.MainGunLarge2 ||
            this.CategoryType == EquipmentTypes.SecondaryGun;

		/// <summary> 主砲系かどうか </summary>
		public bool IsMainGun =>
            this.CategoryType == EquipmentTypes.MainGunSmall ||
            this.CategoryType == EquipmentTypes.MainGunMedium ||
            this.CategoryType == EquipmentTypes.MainGunLarge ||
            this.CategoryType == EquipmentTypes.MainGunLarge2;

        public bool IsLargeGun =>
            this.CategoryType == EquipmentTypes.MainGunLarge ||
            this.CategoryType == EquipmentTypes.MainGunLarge2;

        /// <summary> 副砲系かどうか </summary>
        public bool IsSecondaryGun => this.CategoryType == EquipmentTypes.SecondaryGun;

		/// <summary> 魚雷系かどうか </summary>
		public bool IsTorpedo => this.CategoryType == EquipmentTypes.Torpedo || this.CategoryType == EquipmentTypes.SubmarineTorpedo;

		/// <summary> 後期型魚雷かどうか </summary>
		public bool IsLateModelTorpedo =>
            this.EquipmentID == 213 ||   // 後期型艦首魚雷(6門)
            this.EquipmentID == 214;     // 熟練聴音員+後期型艦首魚雷(6門)


		/// <summary> 高角砲かどうか </summary>
		public bool IsHighAngleGun => this.IconType == 16;

		/// <summary> 高角砲+高射装置かどうか </summary>
		public bool IsHighAngleGunWithAADirector => this.IsHighAngleGun && this.AA >= 8;

		/// <summary> 集中配備機銃かどうか </summary>
		public bool IsConcentratedAAGun => this.CategoryType == EquipmentTypes.AAGun && this.AA >= 9;


		/// <summary> 航空機かどうか </summary>
		public bool IsAircraft
		{
			get
			{
				switch (this.CategoryType)
				{
					case EquipmentTypes.CarrierBasedFighter:
					case EquipmentTypes.CarrierBasedBomber:
					case EquipmentTypes.CarrierBasedTorpedo:
					case EquipmentTypes.SeaplaneBomber:
					case EquipmentTypes.Autogyro:
					case EquipmentTypes.ASPatrol:
					case EquipmentTypes.SeaplaneFighter:
					case EquipmentTypes.LandBasedAttacker:
					case EquipmentTypes.Interceptor:
					case EquipmentTypes.JetFighter:
					case EquipmentTypes.JetBomber:
					case EquipmentTypes.JetTorpedo:

					case EquipmentTypes.CarrierBasedRecon:
					case EquipmentTypes.SeaplaneRecon:
					case EquipmentTypes.FlyingBoat:
                    case EquipmentTypes.LandBasedRecon:
                    case EquipmentTypes.JetRecon:
						return true;

					default:
						return false;
				}
			}
		}

		/// <summary> 戦闘に参加する航空機かどうか </summary>
		public bool IsCombatAircraft
		{
			get
			{
				switch (this.CategoryType)
				{
					case EquipmentTypes.CarrierBasedFighter:
					case EquipmentTypes.CarrierBasedBomber:
					case EquipmentTypes.CarrierBasedTorpedo:
					case EquipmentTypes.SeaplaneBomber:
					case EquipmentTypes.Autogyro:
					case EquipmentTypes.ASPatrol:
					case EquipmentTypes.SeaplaneFighter:
					case EquipmentTypes.LandBasedAttacker:
					case EquipmentTypes.Interceptor:
					case EquipmentTypes.JetFighter:
					case EquipmentTypes.JetBomber:
					case EquipmentTypes.JetTorpedo:
						return true;

					default:
						return false;
				}
			}
		}

		/// <summary> 偵察機かどうか </summary>
		public bool IsReconAircraft
		{
			get
			{
				switch (this.CategoryType)
				{
					case EquipmentTypes.CarrierBasedRecon:
					case EquipmentTypes.SeaplaneRecon:
					case EquipmentTypes.FlyingBoat:
                    case EquipmentTypes.LandBasedRecon:
                    case EquipmentTypes.JetRecon:
						return true;

					default:
						return false;
				}
			}
		}

		/// <summary> 対潜攻撃可能な航空機かどうか </summary>
		public bool IsAntiSubmarineAircraft
		{
			get
			{
				switch (this.CategoryType)
				{
					case EquipmentTypes.CarrierBasedBomber:
					case EquipmentTypes.CarrierBasedTorpedo:
					case EquipmentTypes.SeaplaneBomber:
					case EquipmentTypes.Autogyro:
					case EquipmentTypes.ASPatrol:
					case EquipmentTypes.FlyingBoat:
					case EquipmentTypes.LandBasedAttacker:
					case EquipmentTypes.JetBomber:
					case EquipmentTypes.JetTorpedo:
						return this.ASW > 0;

					default:
						return false;
				}
			}
		}

		/// <summary> 夜間行動可能な航空機かどうか </summary>
		public bool IsNightAircraft => this.IsNightFighter || this.IsNightAttacker;

		/// <summary> 夜間戦闘機かどうか </summary>
		public bool IsNightFighter => this.IconType == 45;

		/// <summary> 夜間攻撃機かどうか </summary>
		public bool IsNightAttacker => this.IconType == 46;

		/// <summary> Swordfish 系艦上攻撃機かどうか </summary>
		public bool IsSwordfish => this.CategoryType == EquipmentTypes.CarrierBasedTorpedo && this.Name.Contains("Swordfish");


		/// <summary> 電探かどうか </summary>
		public bool IsRadar => this.CategoryType == EquipmentTypes.RadarSmall || this.CategoryType == EquipmentTypes.RadarLarge || this.CategoryType == EquipmentTypes.RadarLarge2;

		/// <summary> 対空電探かどうか </summary>
		public bool IsAirRadar => this.IsRadar && this.AA >= 2;

		/// <summary> 水上電探かどうか </summary>
		public bool IsSurfaceRadar => this.IsRadar && this.LOS >= 5;

        /// <summary> ソナーかどうか </summary>
        public bool IsSonar => this.CategoryType == EquipmentTypes.Sonar || this.CategoryType == EquipmentTypes.SonarLarge;

		/// <summary> 爆雷かどうか(投射機は含まない) </summary>
		public bool IsDepthCharge =>
            this.EquipmentID == 226 ||       // 九五式爆雷 
            this.EquipmentID == 227;         // 二式爆雷

		/// <summary> 爆雷投射機かどうか(爆雷は含まない) </summary>
		public bool IsDepthChargeProjector => this.CategoryType == EquipmentTypes.DepthCharge && !this.IsDepthCharge;


        public bool IsHightAltitudeFighter =>
    this.EquipmentID == 350 ||   // Me163B
    this.EquipmentID == 351 ||   // 試製 秋水
    this.EquipmentID == 352;     // 秋水

        /// <summary> 夜間作戦航空要員かどうか </summary>
        public bool IsNightAviationPersonnel =>
            this.EquipmentID == 258 ||       // 夜間作戦航空要員
            this.EquipmentID == 259;         // 夜間作戦航空要員+熟練甲板員

		public int ID => this.EquipmentID;

		public override string ToString() => $"[{this.EquipmentID}] {this.Name}";

	}

}
