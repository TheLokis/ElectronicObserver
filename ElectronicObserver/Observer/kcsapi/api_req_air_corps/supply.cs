using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_req_air_corps
{
	public class supply : APIBase
	{

		private int _aircorpsID;


		public override bool IsRequestSupported => true;

		public override void OnRequestReceived(Dictionary<string, string> data)
		{

            this._aircorpsID = BaseAirCorpsData.GetID(data);

			base.OnRequestReceived(data);
		}


		public override void OnResponseReceived(dynamic data)
		{

			var corps = KCDatabase.Instance.BaseAirCorps;

			if (corps.ContainsKey(this._aircorpsID))
				corps[this._aircorpsID].LoadFromResponse(this.APIName, data);


			int fuel = KCDatabase.Instance.Material.Fuel;
			int baux = KCDatabase.Instance.Material.Bauxite;

			KCDatabase.Instance.Material.LoadFromResponse(this.APIName, data);

			fuel -= KCDatabase.Instance.Material.Fuel;
			baux -= KCDatabase.Instance.Material.Bauxite;

			if (corps.ContainsKey(this._aircorpsID))
				Utility.Logger.Add(2, string.Format("#{0}「{1}」에 보급을 실시했습니다. 소비: 연료x{2}, 보크사이트x{3}",
					corps[this._aircorpsID].MapAreaID, corps[this._aircorpsID].Name, fuel, baux));

			base.OnResponseReceived((object)data);
		}

		public override string APIName => "api_req_air_corps/supply";
	}

}
