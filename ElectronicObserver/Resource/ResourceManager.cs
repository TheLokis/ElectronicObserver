using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Resource
{


	public sealed class ResourceManager
	{


		#region Singleton

		private static readonly ResourceManager instance = new ResourceManager();

		public static ResourceManager Instance => instance;

		#endregion


		#region Properties

		public ImageList Icons { get; private set; }

		public ImageList Equipments { get; private set; }

		public Icon AppIcon { get; private set; }

		#endregion


		#region Constants

		public static string AssetFilePath => "Assets.zip";

		#endregion


		public enum IconContent
		{
            Nothing = -1,
            AppIcon,
            ResourceFuel,
            ResourceAmmo,
            ResourceSteel,
            ResourceBauxite,
            ConditionSparkle,
            ConditionNormal,
            ConditionLittleTired,
            ConditionTired,
            ConditionVeryTired,
            ItemInstantRepair,
            ItemInstantConstruction,
            ItemDevelopmentMaterial,
            ItemModdingMaterial,
            ItemFurnitureCoin,
            ItemBlueprint,
            ItemCatapult,
            ItemPresentBox,
            ItemActionReport,
            FormArsenal,
            FormBattle,
            FormCompass,
            FormDock,
            FormFleet,
            FormHeadQuarters,
            FormInformation,
            FormLog,
            FormMain,
            FormQuest,
            FormShipGroup,
            FormBrowser,
            FormAlbumShip,
            FormAlbumEquipment,
            FormConfiguration,
            FormEquipmentList,
            FormWindowCapture,
            FormDropRecord,
            FormDevelopmentRecord,
            FormConstructionRecord,
            FormResourceChart,
            FormBaseAirCorps,
            FormJson,
            FormAntiAirDefense,
            FormFleetImageGenerator,
            FormExpChecker,
            FormExpeditionCheck,
            FormFleetPreset,
            FleetNoShip,
            FleetDocking,
            FleetSortieDamaged,
            FleetSortie,
            FleetExpedition,
            FleetDamaged,
            FleetNotReplenished,
            FleetAnchorageRepairing,
            FleetReady,
            FleetCombined,
            HeadQuartersShip,
            HeadQuartersEquipment,
            BrowserScreenShot,
            BrowserZoom,
            BrowserZoomIn,
            BrowserZoomOut,
            BrowserUnmute,
            BrowserMute,
            BrowserRefresh,
            BrowserNavigate,
            BrowserOther,
            RarityBlack,
            RarityRed,
            RarityBlueC,
            RarityBlueB,
            RarityBlueA,
            RaritySilver,
            RarityGold,
            RarityHoloB,
            RarityHoloA,
            RarityCherry,
            ParameterHP,
            ParameterFirepower,
            ParameterTorpedo,
            ParameterAA,
            ParameterArmor,
            ParameterASW,
            ParameterEvasion,
            ParameterLOS,
            ParameterLuck,
            ParameterBomber,
            ParameterAccuracy,
            ParameterAircraft,
            ParameterSpeed,
            ParameterRange,
            ParameterInterception,
            ParameterAntiBomber,
            ParameterAircraftCost,
            ParameterAircraftDistance,
            BattleFormationEnemyLineAhead,
            BattleFormationEnemyDoubleLine,
            BattleFormationEnemyDiamond,
            BattleFormationEnemyEchelon,
            BattleFormationEnemyLineAbreast,
            AircraftLevel0,
            AircraftLevel1,
            AircraftLevel2,
            AircraftLevel3,
            AircraftLevel4,
            AircraftLevel5,
            AircraftLevel6,
            AircraftLevel7,
            AircraftLevelTop0,
            AircraftLevelTop1,
            AircraftLevelTop2,
            AircraftLevelTop3,
            AircraftLevelTop4,
            AircraftLevelTop5,
            AircraftLevelTop6,
            AircraftLevelTop7,
        }

		public enum EquipmentContent
		{
			Nothing,            //0
			MainGunS,
			MainGunM,
			MainGunL,
			SecondaryGun,
			Torpedo,
			CarrierBasedFighter,
			CarrierBasedBomber,
			CarrierBasedTorpedo,
			CarrierBasedRecon,
			Seaplane,
			Radar,
			AAShell,
			APShell,
			DamageControl,
			AAGun,
			HighAngleGun,
			DepthCharge,
			Sonar,
			Engine,
			LandingCraft,
			Autogyro,
			ASPatrol,
			Bulge,
			Searchlight,
			DrumCanister,
			RepairFacility,
			Flare,
			CommandFacility,
			MaintenanceTeam,
			AADirector,
			RocketArtillery,
			PicketCrew,
			FlyingBoat,
			Ration,
			Supplies,
			AmphibiousVehicle,
			LandAttacker,
			Interceptor,
			JetFightingBomberKeiun,
			JetFightingBomberKikka,
			TransportMaterials,
			SubmarineEquipment,
			SeaplaneFighter,
			ArmyInterceptor,
			NightFighter,
			NightAttacker,
			LandASPatrol,
			Locked,
			Unknown,
		}


		private ResourceManager()
		{

            this.Icons = new ImageList
			{
				ColorDepth = ColorDepth.Depth32Bit,
				ImageSize = new Size(16, 16)
			};

            this.Equipments = new ImageList
			{
				ColorDepth = ColorDepth.Depth32Bit,
				ImageSize = new Size(16, 16)
			};
		}


		public bool Load()
		{

			try
			{

                this.LoadFromArchive(AssetFilePath);
				return true;

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "리소스 파일 로드에 실패했습니다.");
				MessageBox.Show("리소스 파일 로드에 실패했습니다.\r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);

				FillWithBlankImage(this.Icons, Enum.GetValues(typeof(IconContent)).Length);
				FillWithBlankImage(this.Equipments, Enum.GetValues(typeof(EquipmentContent)).Length);

			}

			return false;
		}



		private void LoadFromArchive(string path)
		{


			using (var archive = new ZipArchive(File.OpenRead(path), ZipArchiveMode.Read))
			{

				const string mstpath = @"Assets/";


                this.AppIcon = LoadIconFromArchive(archive, mstpath + @"AppIcon.ico");

				// ------------------------ icons ------------------------

				LoadImageFromArchive(this.Icons, archive, mstpath + @"AppIcon_16.png", "AppIcon");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Resource/Fuel.png", "Resource_Fuel");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Resource/Ammo.png", "Resource_Ammo");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Resource/Steel.png", "Resource_Steel");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Resource/Bauxite.png", "Resource_Bauxite");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Condition/Sparkle.png", "Condition_Sparkle");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Condition/Normal.png", "Condition_Normal");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Condition/LittleTired.png", "Condition_LittleTired");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Condition/Tired.png", "Condition_Tired");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Condition/VeryTired.png", "Condition_VeryTired");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/InstantRepair.png", "Item_InstantRepair");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/InstantConstruction.png", "Item_InstantConstruction");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/DevelopmentMaterial.png", "Item_DevelopmentMaterial");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/ModdingMaterial.png", "Item_ModdingMaterial");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/FurnitureCoin.png", "Item_FurnitureCoin");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/Blueprint.png", "Item_Blueprint");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/Catapult.png", "Item_Catapult");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/PresentBox.png", "Item_PresentBox");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Item/ActionReport.png", "Item_ActionReport");

                LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Arsenal.png", "Form_Arsenal");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Battle.png", "Form_Battle");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Compass.png", "Form_Compass");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Dock.png", "Form_Dock");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Fleet.png", "Form_Fleet");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Headquarters.png", "Form_Headquarters");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Information.png", "Form_Information");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Log.png", "Form_Log");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Main.png", "Form_Main");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Quest.png", "Form_Quest");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/ShipGroup.png", "Form_ShipGroup");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Browser.png", "Form_Browser");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/AlbumShip.png", "Form_AlbumShip");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/AlbumEquipment.png", "Form_AlbumEquipment");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Configuration.png", "Form_Configuration");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/EquipmentList.png", "Form_EquipmentList");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/WindowCapture.png", "Form_WindowCapture");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/DropRecord.png", "Form_DropRecord");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/DevelopmentRecord.png", "Form_DevelopmentRecord");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/ConstructionRecord.png", "Form_ConstructionRecord");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/ResourceChart.png", "Form_ResourceChart");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/BaseAirCorps.png", "Form_BaseAirCorps");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/Json.png", "Form_Json");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/AntiAirDefense.png", "Form_AntiAirDefense");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/FleetImageGenerator.png", "Form_FleetImageGenerator");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/ExpChecker.png", "Form_ExpChecker");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/ExpeditionCheck.png", "Form_ExpeditionCheck");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Form/FleetPreset.png", "Form_FleetPreset");

                LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/NoShip.png", "Fleet_NoShip");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/Docking.png", "Fleet_Docking");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/SortieDamaged.png", "Fleet_SortieDamaged");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/Sortie.png", "Fleet_Sortie");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/Expedition.png", "Fleet_Expedition");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/Damaged.png", "Fleet_Damaged");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/NotReplenished.png", "Fleet_NotReplenished");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/AnchorageRepairing.png", "Fleet_AnchorageRepairing");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/Ready.png", "Fleet_Ready");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Fleet/Combined.png", "Fleet_Combined");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Headquarters/Ship.png", "HeadQuarters_Ship");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Headquarters/Equipment.png", "HeadQuarters_Equipment");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/ScreenShot.png", "Browser_ScreenShot");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/Zoom.png", "Browser_Zoom");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/ZoomIn.png", "Browser_ZoomIn");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/ZoomOut.png", "Browser_ZoomOut");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/Unmute.png", "Browser_Unmute");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/Mute.png", "Browser_Mute");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/Refresh.png", "Browser_Refresh");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/Navigate.png", "Browser_Navigate");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Browser/Other.png", "Browser_Other");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/Black.png", "Rarity_Black");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/Red.png", "Rarity_Red");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/BlueC.png", "Rarity_BlueC");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/BlueB.png", "Rarity_BlueB");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/BlueA.png", "Rarity_BlueA");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/Silver.png", "Rarity_Silver");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/Gold.png", "Rarity_Gold");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/HoloB.png", "Rarity_HoloB");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/HoloA.png", "Rarity_HoloA");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Rarity/Cherry.png", "Rarity_Cherry");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/HP.png", "Parameter_HP");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Firepower.png", "Parameter_Firepower");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Torpedo.png", "Parameter_Torpedo");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/AA.png", "Parameter_AA");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Armor.png", "Parameter_Armor");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/ASW.png", "Parameter_ASW");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Evasion.png", "Parameter_Evasion");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/LOS.png", "Parameter_LOS");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Luck.png", "Parameter_Luck");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Bomber.png", "Parameter_Bomber");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Accuracy.png", "Parameter_Accuracy");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Aircraft.png", "Parameter_Aircraft");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Speed.png", "Parameter_Speed");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Range.png", "Parameter_Range");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/Interception.png", "Parameter_Interception");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/AntiBomber.png", "Parameter_AntiBomber");
                LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/AircraftCost.png", "Parameter_AircraftCost");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Parameter/AircraftDistance.png", "Parameter_AircraftDistance");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Battle/FormationEnemy01.png", "Battle_FormationEnemy_LineAhead");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Battle/FormationEnemy02.png", "Battle_FormationEnemy_DoubleLine");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Battle/FormationEnemy03.png", "Battle_FormationEnemy_Diamond");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Battle/FormationEnemy04.png", "Battle_FormationEnemy_Echelon");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Battle/FormationEnemy05.png", "Battle_FormationEnemy_LineAbreast");

				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel0.png", "Level_AircraftLevel_0");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel1.png", "Level_AircraftLevel_1");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel2.png", "Level_AircraftLevel_2");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel3.png", "Level_AircraftLevel_3");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel4.png", "Level_AircraftLevel_4");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel5.png", "Level_AircraftLevel_5");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel6.png", "Level_AircraftLevel_6");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevel7.png", "Level_AircraftLevel_7");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop0.png", "Level_AircraftLevelTop_0");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop1.png", "Level_AircraftLevelTop_1");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop2.png", "Level_AircraftLevelTop_2");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop3.png", "Level_AircraftLevelTop_3");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop4.png", "Level_AircraftLevelTop_4");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop5.png", "Level_AircraftLevelTop_5");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop6.png", "Level_AircraftLevelTop_6");
				LoadImageFromArchive(this.Icons, archive, mstpath + @"Level/AircraftLevelTop7.png", "Level_AircraftLevelTop_7");


				// ------------------------ equipments ------------------------

				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Nothing.png", "Equipment_Nothing");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/MainGunS.png", "Equipment_MainGunS");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/MainGunM.png", "Equipment_MainGunM");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/MainGunL.png", "Equipment_MainGunL");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/SecondaryGun.png", "Equipment_SecondaryGun");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Torpedo.png", "Equipment_Torpedo");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/CarrierBasedFighter.png", "Equipment_CarrierBasedFighter");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/CarrierBasedBomber.png", "Equipment_CarrierBasedBomber");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/CarrierBasedTorpedo.png", "Equipment_CarrierBasedTorpedo");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/CarrierBasedRecon.png", "Equipment_CarrierBasedRecon");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Seaplane.png", "Equipment_Seaplane");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/RADAR.png", "Equipment_RADAR");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/AAShell.png", "Equipment_AAShell");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/APShell.png", "Equipment_APShell");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/DamageControl.png", "Equipment_DamageControl");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/AAGun.png", "Equipment_AAGun");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/HighAngleGun.png", "Equipment_HighAngleGun");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/DepthCharge.png", "Equipment_DepthCharge");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/SONAR.png", "Equipment_SONAR");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Engine.png", "Equipment_Engine");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/LandingCraft.png", "Equipment_LandingCraft");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Autogyro.png", "Equipment_Autogyro");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/ASPatrol.png", "Equipment_ASPatrol");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Bulge.png", "Equipment_Bulge");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Searchlight.png", "Equipment_Searchlight");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/DrumCanister.png", "Equipment_DrumCanister");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/RepairFacility.png", "Equipment_RepairFacility");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Flare.png", "Equipment_Flare");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/CommandFacility.png", "Equipment_CommandFacility");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/MaintenanceTeam.png", "Equipment_MaintenanceTeam");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/AADirector.png", "Equipment_AADirector");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/RocketArtillery.png", "Equipment_RocketArtillery");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/PicketCrew.png", "Equipment_PicketCrew");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/FlyingBoat.png", "Equipment_FlyingBoat");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Ration.png", "Equipment_Ration");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Supplies.png", "Equipment_Supplies");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/AmphibiousVehicle.png", "Equipment_AmphibiousVehicle");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/LandAttacker.png", "Equipment_LandAttacker");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Interceptor.png", "Equipment_Interceptor");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/JetFightingBomberKeiun.png", "Equipment_JetFightingBomberKeiun");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/JetFightingBomberKikka.png", "Equipment_JetFightingBomberKikka");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/TransportMaterials.png", "Equipment_TransportMaterials");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/SubmarineEquipment.png", "Equipment_SubmarineEquipment");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/SeaplaneFighter.png", "Equipment_SeaplaneFighter");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/ArmyInterceptor.png", "Equipment_ArmyInterceptor");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/NightFighter.png", "Equipment_NightFighter");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/NightAttacker.png", "Equipment_NightAttacker");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/LandASPatrol.png", "Equipment_LandASPatrol");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Locked.png", "Equipment_Locked");
				LoadImageFromArchive(this.Equipments, archive, mstpath + @"Equipment/Unknown.png", "Equipment_Unknown");

			}

		}

		private static void LoadImageFromArchive(ImageList imglist, ZipArchive arc, string path, string name)
		{

			var entry = arc.GetEntry(path);

			if (entry == null)
			{
				Utility.Logger.Add(3, string.Format("이미지 파일 {0} 은 존재하지 않습니다.", path));
				imglist.Images.Add(name, new Bitmap(imglist.ImageSize.Width, imglist.ImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
				return;
			}


			try
			{

				Bitmap bmp = new Bitmap(entry.Open());

				if (bmp.Size != imglist.ImageSize)
				{

					bmp.Dispose();
					bmp = CreateBlankImage();

				}

				imglist.Images.Add(name, bmp);


			}
			catch (Exception)
			{
				Utility.Logger.Add(3, string.Format("이미지 파일 {0} 의 로드에 실패했습니다.", path));
				imglist.Images.Add(name, CreateBlankImage());
				return;
			}

		}

		private static Icon LoadIconFromArchive(ZipArchive arc, string path)
		{

			var entry = arc.GetEntry(path);

			if (entry == null)
			{
				Utility.Logger.Add(3, string.Format("이미지 파일 {0} 은 존재하지 않습니다.", path));
				return null;
			}

			try
			{
				/*//ストリームから直接読み込むと不思議なチカラによってかき消される
				return new Icon( entry.Open() );
				/*/
				byte[] bytes;
				using (MemoryStream ms = new MemoryStream())
				{
					entry.Open().CopyTo(ms);
					bytes = ms.ToArray();
				}
				using (MemoryStream ms = new MemoryStream(bytes))
				{
					return new Icon(ms);
				}
			}
			catch (Exception)
			{

				Utility.Logger.Add(3, string.Format("이미지 파일 {0} 의 로드에 실패했습니다.", path));
			}

			return null;
		}


		/// <summary>
		/// アーカイブの中からファイルをコピーします。
		/// </summary>
		/// <param name="archivePath">アーカイブの場所。</param>
		/// <param name="source">アーカイブ内のファイルのパス。</param>
		/// <param name="destination">出力するファイルのパス。</param>
		/// <param name="checkexist">true の場合、ファイルが既に存在するときコピーを中止します。</param>
		/// <param name="convertEncoding">エンコーディングを shift-jis から現在設定に合わせて変換するか。</param>
		/// <returns>コピーに成功すれば true 。それ以外は false 。</returns>
		public static bool CopyFromArchive(string archivePath, string source, string destination, bool checkexist = true, bool convertEncoding = false)
		{

			if (checkexist && File.Exists(destination))
			{
				return false;
			}


			using (var archive = new ZipArchive(File.OpenRead(archivePath), ZipArchiveMode.Read))
			{

				string entrypath = @"Assets/" + source;

				var entry = archive.GetEntry(entrypath);

				if (entry == null)
				{
					Utility.Logger.Add(3, string.Format("{0} 은 존재하지 않습니다.", entrypath));
					return false;
				}


				try
				{

					if (convertEncoding && Utility.Configuration.Config.Log.FileEncodingID != 4)
					{
						using (var filetoconvert = GetStreamFromArchive(source))
						{
							filetoconvert.Position = 0;
							using (var convertStream = new StreamReader(filetoconvert, Encoding.GetEncoding(932)))
							{
								string fileread = convertStream.ReadToEnd();
								File.WriteAllText(destination, fileread, Utility.Configuration.Config.Log.FileEncoding);
							}
						}
					}
					else
					{
						entry.ExtractToFile(destination);
					}
					Utility.Logger.Add(2, string.Format("{0} 을 복사했습니다.", entrypath));

				}
				catch (Exception ex)
				{

					Utility.Logger.Add(3, string.Format("{0} 의 복사에 실패했습니다. {1}", entrypath, ex.Message));
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// アーカイブの中からファイルをコピーします。
		/// </summary>
		/// <param name="source">アーカイブ内のファイルのパス。</param>
		/// <param name="destination">出力するファイルのパス。</param>
		/// <param name="checkexist">true の場合、ファイルが既に存在するときコピーを中止します。</param>
		/// <param name="convertEncoding">エンコーディングを shift-jis から現在設定に合わせて変換するか。</param>
		/// <returns>コピーに成功すれば true 。それ以外は false 。</returns>
		public static bool CopyFromArchive(string source, string destination, bool checkexist = true, bool convertEncoding = false)
		{
            return CopyFromArchive(AssetFilePath, source, destination, checkexist, convertEncoding);
        }


		/// <summary>
		/// アーカイブの中から文書ファイルをコピーします。エンコーディングは現在設定に合わせて自動で変更されます。
		/// </summary>
		/// <param name="source">アーカイブ内のファイルのパス。</param>
		/// <param name="destination">出力するファイルのパス。</param>
		/// <returns>コピーに成功すれば true 。それ以外は false 。</returns>
		public static bool CopyDocumentFromArchive(string source, string destination)
		{
			return CopyFromArchive(AssetFilePath, source, destination, true, true);
		}


		/// <summary>
		/// アーカイブからファイルを選択し、ストリームを開きます。
		/// </summary>
		/// <param name="archivePath">アーカイブの場所。</param>
		/// <param name="source">アーカイブ内のファイルのパス。</param>
		/// <returns>ファイルのストリーム。オープンに失敗した場合は null を返します。</returns>
		public static MemoryStream GetStreamFromArchive(string archivePath, string source)
		{
			using (var archive = new ZipArchive(File.OpenRead(archivePath), ZipArchiveMode.Read))
			{

				string entrypath = @"Assets/" + source;

				var entry = archive.GetEntry(entrypath);

				if (entry == null)
				{
					Utility.Logger.Add(3, string.Format("{0} 은 존재하지 않습니다.", entrypath));
					return null;
				}


				try
				{

					byte[] bytes;
					using (MemoryStream ms = new MemoryStream())
					{
						var st = entry.Open();
						st.CopyTo(ms);
						bytes = ms.ToArray();
						st.Close();
					}

					return new MemoryStream(bytes);

				}
				catch (Exception ex)
				{

					Utility.Logger.Add(3, string.Format("{0} 의 배포에 실패했습니다.{1}", entrypath, ex.Message));
					return null;
				}
			}

		}


		/// <summary>
		/// アーカイブからファイルを選択し、ストリームを開きます。
		/// </summary>
		/// <param name="source">アーカイブ内のファイルのパス。</param>
		/// <returns>ファイルのストリーム。オープンに失敗した場合は null を返します。</returns>
		public static MemoryStream GetStreamFromArchive(string source)
		{
			return GetStreamFromArchive(AssetFilePath, source);
		}


		/// <summary>
		/// エラーが発生しないよう、ダミーの画像で領域を埋めます。
		/// </summary>
		private static void FillWithBlankImage(ImageList list, int length)
		{

			for (int i = list.Images.Count; i < length; i++)
			{
				list.Images.Add(CreateBlankImage());
			}
		}

		/// <summary>
		/// 空白画像を作成します。
		/// </summary>
		private static Bitmap CreateBlankImage()
		{
			return new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		}



		/// <summary>
		/// 装備アイコンを取得します。一般的用途(ロック/未装備等でない、マスターとしてのアイコン)に向いています。
		/// </summary>
		public static Image GetEquipmentImage(int imageID)
		{

			if (imageID < 0)
				return Instance.Equipments.Images[(int)EquipmentContent.Locked];
			if (imageID >= (int)EquipmentContent.Locked)
				return Instance.Equipments.Images[(int)EquipmentContent.Unknown];

			return Instance.Equipments.Images[imageID];
		}



		/// <summary>
		/// BitmapをIconに変換します。
		/// </summary>
		public static Icon BitmapToIcon(Bitmap image)
		{
			return Icon.FromHandle(image.GetHicon());
		}

		/// <summary>
		/// ImageをIconに変換します。
		/// </summary>
		public static Icon ImageToIcon(Image image)
		{
			return BitmapToIcon((Bitmap)image);
		}


		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public extern static bool DestroyIcon(IntPtr handle);

		/// <summary>
		/// ImageToIcon を利用して生成したアイコンを破棄する場合、必ずこのメソッドを呼んで破棄してください。
		/// </summary>
		public static bool DestroyIcon(Icon icon)
		{
			return DestroyIcon(icon.Handle);
		}

	}


}
