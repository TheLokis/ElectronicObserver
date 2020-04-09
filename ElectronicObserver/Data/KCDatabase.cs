using ElectronicObserver.Data.Battle;
using ElectronicObserver.Data.Quest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{


	/// <summary>
	/// 艦これのデータを扱う中核です。
	/// </summary>
	public sealed class KCDatabase
	{


		#region Singleton

		private static readonly KCDatabase instance = new KCDatabase();

		public static KCDatabase Instance => instance;

		#endregion



		/// <summary>
		/// 艦船のマスターデータ
		/// </summary>
		public IDDictionary<ShipDataMaster> MasterShips { get; private set; }

		/// <summary>
		/// 艦種データ
		/// </summary>
		public IDDictionary<ShipType> ShipTypes { get; private set; }
        /// <summary>
		/// 艦船グラフィックデータ
		/// </summary>
		public IDDictionary<ShipGraphicData> ShipGraphics { get; private set; }
        /// <summary>
        /// 装備のマスターデータ
        /// </summary>
        public IDDictionary<EquipmentDataMaster> MasterEquipments { get; private set; }

		/// <summary>
		/// 装備種別
		/// </summary>
		public IDDictionary<EquipmentType> EquipmentTypes { get; private set; }


		/// <summary>
		/// 保有艦娘のデータ
		/// </summary>
		public IDDictionary<ShipData> Ships { get; private set; }

		/// <summary>
		/// 保有装備のデータ
		/// </summary>
		public IDDictionary<EquipmentData> Equipments { get; private set; }


		/// <summary>
		/// 提督・司令部データ
		/// </summary>
		public AdmiralData Admiral { get; private set; }


		/// <summary>
		/// アイテムのマスターデータ
		/// </summary>
		public IDDictionary<UseItemMaster> MasterUseItems { get; private set; }

		/// <summary>
		/// アイテムデータ
		/// </summary>
		public IDDictionary<UseItem> UseItems { get; private set; }


		/// <summary>
		/// 工廠ドックデータ
		/// </summary>
		public IDDictionary<ArsenalData> Arsenals { get; private set; }

		/// <summary>
		/// 入渠ドックデータ
		/// </summary>
		public IDDictionary<DockData> Docks { get; private set; }

        /// <summary>
        /// 開発データ
        /// </summary>
        public DevelopmentData Development { get; private set; }


        /// <summary>
        /// 艦隊データ
        /// </summary>
        public FleetManager Fleet { get; private set; }


		/// <summary>
		/// 資源データ
		/// </summary>
		public MaterialData Material { get; private set; }


		/// <summary>
		/// 任務データ
		/// </summary>
		public QuestManager Quest { get; private set; }

		/// <summary>
		/// 任務進捗データ
		/// </summary>
		public QuestProgressManager QuestProgress { get; private set; }


		/// <summary>
		/// 戦闘データ
		/// </summary>
		public BattleManager Battle { get; private set; }


		/// <summary>
		/// 海域カテゴリデータ
		/// </summary>
		public IDDictionary<MapAreaData> MapArea { get; private set; }

		/// <summary>
		/// 海域データ
		/// </summary>
		public IDDictionary<MapInfoData> MapInfo { get; private set; }


		/// <summary>
		/// 遠征データ
		/// </summary>
		public IDDictionary<MissionData> Mission { get; private set; }


		/// <summary>
		/// 艦船グループデータ
		/// </summary>
		public ShipGroupManager ShipGroup { get; private set; }


		/// <summary>
		/// 基地航空隊データ
		/// </summary>
		public IDDictionary<BaseAirCorpsData> BaseAirCorps { get; private set; }

		/// <summary>
		/// 配置転換中装備データ
		/// </summary>
		public IDDictionary<RelocationData> RelocatedEquipments { get; private set; }

		private KCDatabase()
		{

            this.MasterShips = new IDDictionary<ShipDataMaster>();
            this.ShipTypes = new IDDictionary<ShipType>();
            this.ShipGraphics = new IDDictionary<ShipGraphicData>();
            this.MasterEquipments = new IDDictionary<EquipmentDataMaster>();
            this.EquipmentTypes = new IDDictionary<EquipmentType>();
            this.Ships = new IDDictionary<ShipData>();
            this.Equipments = new IDDictionary<EquipmentData>();
            this.Admiral = new AdmiralData();
            this.MasterUseItems = new IDDictionary<UseItemMaster>();
            this.UseItems = new IDDictionary<UseItem>();
            this.Arsenals = new IDDictionary<ArsenalData>();
            this.Docks = new IDDictionary<DockData>();
            this.Development = new DevelopmentData();
            this.Fleet = new FleetManager();
            this.Material = new MaterialData();
            this.Quest = new QuestManager();
            this.QuestProgress = new QuestProgressManager();
            this.Battle = new BattleManager();
            this.MapArea = new IDDictionary<MapAreaData>();
            this.MapInfo = new IDDictionary<MapInfoData>();
            this.Mission = new IDDictionary<MissionData>();
            this.ShipGroup = new ShipGroupManager();
            this.BaseAirCorps = new IDDictionary<BaseAirCorpsData>();
            this.RelocatedEquipments = new IDDictionary<RelocationData>();
		}


		public void Load()
		{

			{
				var temp = (ShipGroupManager)this.ShipGroup.Load();
				if (temp != null)
                    this.ShipGroup = temp;
			}
			{
				var temp = this.QuestProgress.Load();
				if (temp != null)
				{
					if (this.QuestProgress != null)
                        this.QuestProgress.RemoveEvents();
                    this.QuestProgress = temp;
				}
			}

		}

		public void Save()
		{
            this.ShipGroup.Save();
            this.QuestProgress.Save();
		}

	}


}
