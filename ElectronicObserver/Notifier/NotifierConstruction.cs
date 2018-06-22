using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	/// <summary>
	/// 建造完了通知を扱います。
	/// </summary>
	public class NotifierConstruction : NotifierBase
	{

		private Dictionary<int, bool> processedFlags;


		public NotifierConstruction()
			: base()
		{
			Initialize();
		}

		public NotifierConstruction(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
			: base(config)
		{
			Initialize();
		}


		private void Initialize()
		{
			DialogData.Title = "건조완료";
			processedFlags = new Dictionary<int, bool>();
		}


		protected override void UpdateTimerTick()
		{

			foreach (var arsenal in KCDatabase.Instance.Arsenals.Values)
			{

				if (!processedFlags.ContainsKey(arsenal.ArsenalID))
					processedFlags.Add(arsenal.ArsenalID, false);

				if (arsenal.State > 0)
				{
					if (!processedFlags[arsenal.ArsenalID] && (
						(int)(arsenal.CompletionTime - DateTime.Now).TotalMilliseconds <= AccelInterval ||
						arsenal.State == 3))
					{

						processedFlags[arsenal.ArsenalID] = true;
						Notify(arsenal.ArsenalID, arsenal.ShipID);
					}

				}
				else
				{
					processedFlags[arsenal.ArsenalID] = false;
				}

			}

		}

		public void Notify(int arsenalID, int shipID)
		{

			DialogData.Message = string.Format("공창 도크 #{0} 에서「{1}」의 건조가 완료되었습니다.",
				arsenalID, Utility.Configuration.Config.FormArsenal.ShowShipName ? KCDatabase.Instance.MasterShips[shipID].NameWithClass : "칸무스");

			base.Notify();
		}
	}
}
