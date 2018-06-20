﻿using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_req_kaisou
{

	public class marriage : APIBase
	{

		public override void OnResponseReceived(dynamic data)
		{

			Utility.Logger.Add(2, string.Format("{0} 와 결혼 했습니다. 축하드립니다!", KCDatabase.Instance.Ships[(int)data.api_id].Name));

			base.OnResponseReceived((object)data);
		}

		public override string APIName => "api_req_kaisou/marriage";
	}

}
