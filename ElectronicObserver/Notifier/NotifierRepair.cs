using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	/// <summary>
	/// 入渠完了通知を扱います。
	/// </summary>
	public class NotifierRepair : NotifierBase
	{

		private Dictionary<int, bool> processedFlags;


		public NotifierRepair()
			: base()
		{
            this.Initialize();
		}

		public NotifierRepair(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
			: base(config)
		{
            this.Initialize();
		}


		private void Initialize()
		{
            this.DialogData.Title = "수리완료";
            this.processedFlags = new Dictionary<int, bool>();
		}


		protected override void UpdateTimerTick()
		{

			foreach (var dock in KCDatabase.Instance.Docks.Values)
			{

				if (!this.processedFlags.ContainsKey(dock.DockID))
                    this.processedFlags.Add(dock.DockID, false);

				if (dock.State > 0)
				{
					if (!this.processedFlags[dock.DockID] && (int)(dock.CompletionTime - DateTime.Now).TotalMilliseconds <= this.AccelInterval)
					{

                        this.processedFlags[dock.DockID] = true;
                        this.Notify(dock.DockID, dock.ShipID);

					}

				}
				else
				{
                    this.processedFlags[dock.DockID] = false;
				}

			}

		}

		public void Notify(int dockID, int shipID)
		{

            this.DialogData.Message = string.Format("입거독 #{0}「{1}」의 수리가 완료되었습니다.",
				dockID, KCDatabase.Instance.Ships[shipID].NameWithLevel);

			base.Notify();
		}

	}
}
