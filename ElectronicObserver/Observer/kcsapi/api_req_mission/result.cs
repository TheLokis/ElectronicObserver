using ElectronicObserver.Data;
using ElectronicObserver.Resource.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_req_mission
{

	public class result : APIBase
	{

		private int _fleetID;


		public override bool IsRequestSupported => true;

		public override void OnRequestReceived(Dictionary<string, string> data)
		{

            this._fleetID = int.Parse(data["api_deck_id"]);

			base.OnRequestReceived(data);
		}

		public override void OnResponseReceived(dynamic data)
		{

			var fleet = KCDatabase.Instance.Fleet[this._fleetID];
			int[] materials = new int[4];

			Utility.Logger.Add(2, string.Format("#{0}「{1}」가 원정「{2}: {3}」에서 귀환했습니다.",
				fleet.FleetID, fleet.Name, fleet.ExpeditionDestination, 
				Window.FormMain.Instance.Translator.GetTranslation(data.api_quest_name, Utility.DataType.ExpeditionTitle, fleet.ExpeditionDestination)));


			// 獲得資源表示
			if (Utility.Configuration.Config.Log.ShowSpoiler)
			{
				var sb = new LinkedList<string>();

				//materials
				if (data.api_get_material is double == false)
				{       // 원정실패시 -1로 리턴
					materials = (int[])data.api_get_material;
					for (int i = 0; i < 4; i++)
					{
						if (materials[i] > 0)
						{
							sb.AddLast(Constants.GetMaterialName(i + 1) + "x" + materials[i]);
						}
					}
				}

				//items
				{
					for (int i = 0; i < 2; i++)
					{

						int kind = (int)data.api_useitem_flag[i];

						if (kind > 0)
						{

							int id = (int)data["api_get_item" + (i + 1)].api_useitem_id;
							int count = (int)data["api_get_item" + (i + 1)].api_useitem_count;

							switch (kind)
							{
								case 1:
									sb.AddLast("고속수복재x" + count);
									break;
								case 2:
									sb.AddLast("고속건조재x" + count);
									break;
								case 3:
									sb.AddLast("개발자재x" + count);
									break;
								case 4:
									sb.AddLast(KCDatabase.Instance.MasterUseItems[id].Name + "x" + count);
									break;
								case 5:
									sb.AddLast("가구코인x" + count);
									break;
							}

						}
					}
				}

				//exp
				{
					int admiralExp = (int)data.api_get_exp;
					if (admiralExp > 0)
					{
						sb.AddLast("제독 경험치+" + admiralExp);
					}

					int shipExp = ((int[])data.api_get_ship_exp).Min();
					if (shipExp > 0)
					{
						sb.AddLast("함선 경험치+" + shipExp);
					}
				}

				Utility.Logger.Add(2, "원정 결과 - " + Constants.GetExpeditionResult((int)data.api_clear_result) + ": " + (sb.Count == 0 ? "획득자원없음" : string.Join(", ", sb)));
			}


			// レベルアップ表示
				int[] exps = new int[6];
                int[] items = { -1, -1 };
                int[] itemscount = { -1, -1 };

                var src = (int[])data.api_get_ship_exp;
				Array.Copy(src, exps, src.Length);

				var lvup = new List<int[]>();
				foreach (var elem in data.api_get_exp_lvup)
				{
					lvup.Add((int[])elem);
				}

                for (int i = 0; i < 2; i++)
                {
                    int kind = (int)data.api_useitem_flag[i];

                if (kind > 0)
                {
                    items[i] = (int)data["api_get_item" + (i + 1)].api_useitem_id;
                    itemscount[i] = (int)data["api_get_item" + (i + 1)].api_useitem_count;
                    switch (kind)
                    {
                        case 1:
                        case 2:
                        case 3:
							items[i] = (int)data.api_useitem_flag[i];
							break;
						case 4:
						case 5:
                            items[i] = (int)data["api_get_item" + (i + 1)].api_useitem_id;
							break;
                    }
                }
            }

                for (int i = 0; i < lvup.Count; i++)
                {
                    if (lvup[i].Length >= 2 && lvup[i][1] > 0 && lvup[i][0] + exps[i] >= lvup[i][1])
                    {
                        var ship = fleet.MembersInstance[i];
                        int increment = Math.Max(lvup[i].Length - 2, 1);

                        Utility.Logger.Add(2, string.Format("{0} 가 레벨 {1} 이 되었습니다.", ship.Name, ship.Level + increment));
                    }
                }


            RecordManager.Instance.Expedition.Add(
                fleet.ExpeditionDestination,
                fleet[0],
                materials[0],
                materials[1],
                materials[2],
                materials[3],
                (int)data.api_clear_result,
				items[0],
				items[1],
				itemscount[0],
				itemscount[1]);

            base.OnResponseReceived((object)data);
		}

		public override string APIName => "api_req_mission/result";
	}

}
