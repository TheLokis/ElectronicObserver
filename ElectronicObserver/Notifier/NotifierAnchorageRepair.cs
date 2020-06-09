using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	public class NotifierAnchorageRepair : NotifierBase
	{

		/// <summary>
		/// 通知レベル
		/// 0 = いつでも、1 = 明石旗艦の時、2 = 修理艦もいる時、3 = 2 + プリセット編成時
		/// </summary>
		public int NotificationLevel { get; set; }



		// いったん母港に行くまでは通知しない
		private bool processedFlag = true;

		public NotifierAnchorageRepair()
			: base()
		{
            this.Initialize();
		}

		public NotifierAnchorageRepair(Utility.Configuration.ConfigurationData.ConfigNotifierAnchorageRepair config)
			: base(config)
		{
            this.Initialize();

            this.NotificationLevel = config.NotificationLevel;
		}

		private void Initialize()
		{
            this.DialogData.Title = "아카시수리발동";


			APIObserver o = APIObserver.Instance;

			o["api_port/port"].ResponseReceived += this.ClearFlag;
		}

		void ClearFlag(string apiname, dynamic data)
		{

            this.processedFlag = false;
		}


		protected override void UpdateTimerTick()
		{

			var fleets = KCDatabase.Instance.Fleet;

			if (!this.processedFlag)
			{

				if ((DateTime.Now - fleets.AnchorageRepairingTimer).TotalMilliseconds + this.AccelInterval >= 20 * 60 * 1000)
				{

					bool clear;
					switch (this.NotificationLevel)
					{

						case 0:     //いつでも
						default:
							clear = true;
							break;

						case 1:     //明石旗艦の時
							clear = fleets.Fleets.Values.Any(f => f.IsFlagshipRepairShip);
							break;

						case 2:     //修理艦もいる時
							clear = fleets.Fleets.Values.Any(f => f.CanAnchorageRepair);
							break;

                        case 3:     // プリセット込み
                            clear = fleets.Fleets.Values.Any(f => f.CanAnchorageRepair) ||
                                KCDatabase.Instance.FleetPreset.Presets.Values.Any(p => FleetData.CanAnchorageRepairWithMember(p.MembersInstance));
                            break;
                    }

					if (clear)
					{

                        this.Notify();
                        this.processedFlag = true;
					}
				}
			}

		}


		public override void Notify()
		{

            this.DialogData.Message = "아카시배치후 20분이 경과했습니다.";

			base.Notify();
		}


		public override void ApplyToConfiguration(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
		{
			base.ApplyToConfiguration(config);

			var c = config as Utility.Configuration.ConfigurationData.ConfigNotifierAnchorageRepair;

			if (c != null)
			{
				c.NotificationLevel = this.NotificationLevel;
			}
		}

	}
}
