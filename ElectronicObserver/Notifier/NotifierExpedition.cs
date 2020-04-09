using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	/// <summary>
	/// 遠征帰投通知を扱います。
	/// </summary>
	public class NotifierExpedition : NotifierBase
	{

		private Dictionary<int, bool> processedFlags;


		public NotifierExpedition()
			: base()
		{
            this.Initialize();
		}

		public NotifierExpedition(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
			: base(config)
		{
            this.Initialize();
		}


		private void Initialize()
		{
            this.DialogData.Title = "원정귀환";
            this.processedFlags = new Dictionary<int, bool>();
		}


		protected override void UpdateTimerTick()
		{

			foreach (var fleet in KCDatabase.Instance.Fleet.Fleets.Values)
			{

				if (!this.processedFlags.ContainsKey(fleet.FleetID))
                    this.processedFlags.Add(fleet.FleetID, false);

				if (fleet.ExpeditionState != 0)
				{
					if (!this.processedFlags[fleet.FleetID] && (int)(fleet.ExpeditionTime - DateTime.Now).TotalMilliseconds <= this.AccelInterval)
					{

                        this.processedFlags[fleet.FleetID] = true;
                        this.Notify(fleet.FleetID, fleet.ExpeditionDestination);

					}

				}
				else
				{
                    this.processedFlags[fleet.FleetID] = false;
				}

			}

		}

		public void Notify(int fleetID, int destination)
		{

            this.DialogData.Message = string.Format("#{0} 「{1}」가「{2} : {3}」에서 귀환했습니다.",
				fleetID, KCDatabase.Instance.Fleet[fleetID].Name, destination, KCDatabase.Instance.Mission[destination].Name);

			base.Notify();
		}
	}
}
