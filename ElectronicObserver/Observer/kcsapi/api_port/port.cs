using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_port
{

	public class port : APIBase
	{


		public override void OnResponseReceived(dynamic data)
		{

			KCDatabase db = KCDatabase.Instance;

			db.Fleet.EvacuatePreviousShips();


			//api_material
			db.Material.LoadFromResponse(this.APIName, data.api_material);

			//api_basic
			db.Admiral.LoadFromResponse(this.APIName, data.api_basic);

			//api_ship
			db.Ships.Clear();
			foreach (var elem in data.api_ship)
			{

				var a = new ShipData();
				a.LoadFromResponse(this.APIName, elem);
				db.Ships.Add(a);

			}


			//api_ndock
			foreach (var elem in data.api_ndock)
			{

				int id = (int)elem.api_id;

				if (!db.Docks.ContainsKey(id))
				{
					var a = new DockData();
					a.LoadFromResponse(this.APIName, elem);
					db.Docks.Add(a);

				}
				else
				{
					db.Docks[id].LoadFromResponse(this.APIName, elem);
				}
			}

			//api_deck_port
			db.Fleet.LoadFromResponse(this.APIName, data.api_deck_port);
			db.Fleet.CombinedFlag = data.api_combined_flag() ? (int)data.api_combined_flag : 0;


			// 基地航空隊　配置転換系の処理
			if (data.api_plane_info() && data.api_plane_info.api_base_convert_slot())
			{

				var prev = db.RelocatedEquipments.Keys.ToArray();
				var current = (int[])data.api_plane_info.api_base_convert_slot;

				foreach (int deleted in prev.Except(current))
				{
					db.RelocatedEquipments.Remove(deleted);
				}

				foreach (int added in current.Except(prev))
				{
					db.RelocatedEquipments.Add(new RelocationData(added, DateTime.Now));
				}

			}
			else
			{

				db.RelocatedEquipments.Clear();
			}

			db.Battle.LoadFromResponse(this.APIName, data);

			base.OnResponseReceived((object)data);
		}

		public override string APIName => "api_port/port";
	}


}
