using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 基地航空隊の航空中隊データを扱います。
	/// </summary>
	public class BaseAirCorpsSquadron : APIWrapper, IIdentifiable
	{

		/// <summary>
		/// 中隊ID
		/// </summary>
		public int SquadronID => (int)this.RawData.api_squadron_id;


		/// <summary>
		/// 状態
		/// 0=未配属, 1=配属済み, 2=配置転換中
		/// </summary>
		public int State => this.RawData.api_state() ? (int)this.RawData.api_state : 0;


		/// <summary>
		/// 装備固有ID
		/// </summary>
		public int EquipmentMasterID => (int)this.RawData.api_slotid;


		/// <summary>
		/// 装備データ
		/// </summary>
		public EquipmentData EquipmentInstance => KCDatabase.Instance.Equipments[this.EquipmentMasterID];


		/// <summary>
		/// 装備ID
		/// </summary>
		public int EquipmentID => this.EquipmentInstance?.EquipmentID ?? -1;

		/// <summary>
		/// マスター装備データ
		/// </summary>
		public EquipmentDataMaster EquipmentInstanceMaster => this.EquipmentInstance?.MasterEquipment;

		/// <summary>
		/// 現在の稼働機数
		/// </summary>
		public int AircraftCurrent => this.RawData.api_count() ? (int)this.RawData.api_count : 0;


		/// <summary>
		/// 最大機数
		/// </summary>
		public int AircraftMax => this.RawData.api_max_count() ? (int)this.RawData.api_max_count : 0;


		/// <summary>
		/// コンディション
		/// 1=通常、2=橙疲労、3=赤疲労
		/// </summary>
		public int Condition => this.RawData.api_cond() ? (int)this.RawData.api_cond : 1;



		/// <summary>
		/// 配置転換を開始した時刻
		/// </summary>
		public DateTime RelocatedTime
		{
			get
			{
				if (this.State != 2)
					return DateTime.MinValue;

				var relocated = KCDatabase.Instance.RelocatedEquipments[this.EquipmentMasterID];
				if (relocated == null)
					return DateTime.MinValue;

				return relocated.RelocatedTime;
			}
		}


		public override void LoadFromResponse(string apiname, dynamic data)
		{

			int prevState = this.RawData != null ? this.State : 0;

			base.LoadFromResponse(apiname, (object)data);

		}


		public override string ToString() => $"{this.EquipmentInstance} {this.AircraftCurrent}/{this.AircraftMax}";


		public int ID => this.SquadronID;
	}

}
