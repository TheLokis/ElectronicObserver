using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	/// <summary>
	/// 疲労回復通知を扱います。
	/// </summary>
	public class NotifierCondition : NotifierBase
	{

		private Dictionary<int, bool> _processedFlags;


		public NotifierCondition()
			: base()
		{
            this.Initialize();
		}

		public NotifierCondition(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
			: base(config)
		{
            this.Initialize();
		}


		private void Initialize()
		{
            this.DialogData.Title = "피로회복";
            this._processedFlags = new Dictionary<int, bool>();

			for (int i = 1; i <= 4; i++)
                this._processedFlags.Add(i, false);


			APIObserver o = APIObserver.Instance;

			o["api_port/port"].ResponseReceived += this.ClearFlags;

		}


		private void ClearFlags(string apiname, dynamic data)
		{

			foreach (int key in this._processedFlags.Keys.ToArray())
			{       //列挙中の変更によるエラーを防ぐため
                this._processedFlags[key] = false;
			}
		}


		protected override void UpdateTimerTick()
		{

			foreach (var fleet in KCDatabase.Instance.Fleet.Fleets.Values)
			{

				if (fleet.ExpeditionState > 0 || fleet.IsInSortie) continue;

				if (this._processedFlags[fleet.FleetID])
					continue;


				if (fleet.ConditionTime != null && !fleet.IsInSortie)
				{

					if (((DateTime)fleet.ConditionTime - DateTime.Now).TotalMilliseconds <= this.AccelInterval)
					{

                        this.Notify(fleet.FleetID);
                        this._processedFlags[fleet.FleetID] = true;
					}
				}
			}

		}

		public void Notify(int fleetID)
		{

            this.DialogData.Message = string.Format("#{0} 「{1}」의 피로도 회복이 완료되었습니다!",
				fleetID, KCDatabase.Instance.Fleet[fleetID].Name);

			base.Notify();
		}

	}
}
