﻿using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_req_mission
{

	public class start : APIBase
	{

		//private int FleetID;


		public override void OnRequestReceived(Dictionary<string, string> data)
		{

			/*	//checkme: どちらにせよあとで deck が呼ばれるので不要？
			FleetID = int.Parse( data["api_deck_id"] );
			KCDatabase.Instance.Fleet.Fleets[FleetID].LoadFromRequest( APIName, data );
			*/

			int deckID = int.Parse(data["api_deck_id"]);
			int destination = int.Parse(data["api_mission_id"]);

			Utility.Logger.Add(2, string.Format("#{0}「{1}」가 원정「{2}: {3}」에 출발했습니다.", deckID, KCDatabase.Instance.Fleet[deckID].Name, destination, KCDatabase.Instance.Mission[destination].Name));

			base.OnRequestReceived(data);
		}

		public override void OnResponseReceived(dynamic data)
		{

			/*
			KCDatabase.Instance.Fleet.Fleets[FleetID].LoadFromResponse( APIName, data );
			*/

			base.OnResponseReceived((object)data);

		}


		public override bool IsRequestSupported => true;
		public override bool IsResponseSupported => true;

		public override string APIName => "api_req_mission/start";
	}


}
