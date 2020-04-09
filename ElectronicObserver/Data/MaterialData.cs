using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 各種資源量を保持します。
	/// </summary>
	public class MaterialData : APIWrapper
	{

		/// <summary>
		/// 燃料
		/// </summary>
		public int Fuel { get; internal set; }

		/// <summary>
		/// 弾薬
		/// </summary>
		public int Ammo { get; internal set; }

		/// <summary>
		/// 鋼材
		/// </summary>
		public int Steel { get; internal set; }

		/// <summary>
		/// ボーキサイト
		/// </summary>
		public int Bauxite { get; internal set; }


		/// <summary>
		/// 高速建造材
		/// </summary>
		public int InstantConstruction { get; internal set; }

		/// <summary>
		/// 高速修復材
		/// </summary>
		public int InstantRepair { get; internal set; }

		/// <summary>
		/// 開発資材
		/// </summary>
		public int DevelopmentMaterial { get; internal set; }

		/// <summary>
		/// 改修資材
		/// </summary>
		public int ModdingMaterial { get; internal set; }




		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);           //何か基幹とするデータ構造があった場合、switch文のなかに移動すること

			switch (apiname)
			{
				case "api_port/port":
				case "api_get_member/material":
                    this.Fuel = (int)data[0].api_value;
                    this.Ammo = (int)data[1].api_value;
                    this.Steel = (int)data[2].api_value;
                    this.Bauxite = (int)data[3].api_value;
                    this.InstantConstruction = (int)data[4].api_value;
                    this.InstantRepair = (int)data[5].api_value;
                    this.DevelopmentMaterial = (int)data[6].api_value;
                    this.ModdingMaterial = (int)data[7].api_value;
					break;

				case "api_req_hokyu/charge":
				case "api_req_kousyou/destroyship":
                    this.Fuel = (int)data[0];
                    this.Ammo = (int)data[1];
                    this.Steel = (int)data[2];
                    this.Bauxite = (int)data[3];
					break;

				case "api_req_kousyou/destroyitem2":
                    this.Fuel += (int)data.api_get_material[0];
                    this.Ammo += (int)data.api_get_material[1];
                    this.Steel += (int)data.api_get_material[2];
                    this.Bauxite += (int)data.api_get_material[3];
					break;

				case "api_req_kousyou/createitem":
				case "api_req_kousyou/remodel_slot":
                    this.Fuel = (int)data[0];
                    this.Ammo = (int)data[1];
                    this.Steel = (int)data[2];
                    this.Bauxite = (int)data[3];
                    this.InstantConstruction = (int)data[4];
                    this.InstantRepair = (int)data[5];
                    this.DevelopmentMaterial = (int)data[6];
                    this.ModdingMaterial = (int)data[7];
					break;

				case "api_req_air_corps/supply":
                    this.Fuel = (int)data.api_after_fuel;
                    this.Bauxite = (int)data.api_after_bauxite;
					break;

				case "api_req_air_corps/set_plane":
					if (data.api_after_bauxite())
                        this.Bauxite = (int)data.api_after_bauxite;
					break;
			}
		}


		public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
		{
			base.LoadFromRequest(apiname, data);

			switch (apiname)
			{
				case "api_req_kousyou/createship":
                    this.Fuel -= int.Parse(data["api_item1"]);
                    this.Ammo -= int.Parse(data["api_item2"]);
                    this.Steel -= int.Parse(data["api_item3"]);
                    this.Bauxite -= int.Parse(data["api_item4"]);
                    this.DevelopmentMaterial -= int.Parse(data["api_item5"]);
					break;

			}
		}


	}

}
