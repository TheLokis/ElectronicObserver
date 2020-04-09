using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 入渠ドックの情報を保持します。
	/// </summary>
	public class DockData : ResponseWrapper, IIdentifiable
	{

		/// <summary>
		/// ドックID
		/// </summary>
		public int DockID => (int)this.RawData.api_id;

		/// <summary>
		/// 入渠状態
		/// -1=ロック, 0=空き, 1=入渠中
		/// </summary>
		public int State { get; internal set; }

		/// <summary>
		/// 入渠中の艦船のID
		/// </summary>
		public int ShipID { get; internal set; }

		/// <summary>
		/// 入渠完了日時
		/// </summary>
		public DateTime CompletionTime { get; internal set; }


		public int ID => this.DockID;


		public override void LoadFromResponse(string apiname, dynamic data)
		{

			switch (apiname)
			{
				case "api_req_nyukyo/speedchange":
					if (this.State == 1 && this.ShipID != 0)
					{
						KCDatabase.Instance.Ships[this.ShipID].Repair();
                        this.State = 0;
                        this.ShipID = 0;
					}
					break;

				default:
					{
						base.LoadFromResponse(apiname, (object)data);

						int newstate = (int)this.RawData.api_state;

						if (this.State == 1 && newstate == 0 && this.ShipID != 0)
						{
							KCDatabase.Instance.Ships[this.ShipID].Repair();
						}

                        this.State = newstate;
                        this.ShipID = (int)this.RawData.api_ship_id;
                        this.CompletionTime = DateTimeHelper.FromAPITime((long)this.RawData.api_complete_time);
					}
					break;
			}


		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[" + this.ID.ToString() + "] : ");
			switch (this.State)
			{
				case -1:
					sb.Append("<Locked>"); break;
				case 0:
					sb.Append("<Empty>"); break;
				case 1:
					sb.Append(KCDatabase.Instance.Ships[this.ShipID].MasterShip.Name + ", at " + this.CompletionTime.ToString()); break;
			}

			return sb.ToString();
		}
	}

}
