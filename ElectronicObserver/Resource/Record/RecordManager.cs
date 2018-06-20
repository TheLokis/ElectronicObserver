using ElectronicObserver.Data;
using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Resource.Record
{

	public sealed class RecordManager
	{

		#region Singleton

		private static readonly RecordManager instance = new RecordManager();

		public static RecordManager Instance => instance;

		#endregion

		public string MasterPath { get; private set; }

		public EnemyFleetRecord EnemyFleet { get; private set; }
		public ShipParameterRecord ShipParameter { get; private set; }
		public ConstructionRecord Construction { get; private set; }
		public ShipDropRecord ShipDrop { get; private set; }
		public DevelopmentRecord Development { get; private set; }
		public ResourceRecord Resource { get; private set; }

		private IEnumerable<RecordBase> Records
		{
			get
			{
				yield return EnemyFleet;
				yield return ShipParameter;
				yield return Construction;
				yield return ShipDrop;
				yield return Development;
				yield return Resource;
			}
		}


		private DateTime _prevTime;

		private RecordManager()
		{

			MasterPath = @"Record";
			EnemyFleet = new EnemyFleetRecord();
			ShipParameter = new ShipParameterRecord();
			Construction = new ConstructionRecord();
			ShipDrop = new ShipDropRecord();
			Development = new DevelopmentRecord();
			Resource = new ResourceRecord();

			foreach (var r in Records)
				r.RegisterEvents();


			if (!Directory.Exists(MasterPath))
			{
				Directory.CreateDirectory(MasterPath);
			}

			_prevTime = DateTime.Now;
			Observer.APIObserver.Instance["api_port/port"].ResponseReceived += TimerSave;
		}

		public bool Load(bool logging = true)
		{

			bool succeeded = true;

			ResourceManager.CopyDocumentFromArchive("Record/" + ShipParameter.FileName, MasterPath + "\\" + ShipParameter.FileName);

			foreach (var r in Records)
				succeeded &= r.Load(MasterPath);

			if (logging)
			{
				if (succeeded)
					Utility.Logger.Add(2, "기록을 로드했습니다.");
				else
					Utility.Logger.Add(3, "기록 로드에 실패했습니다.");
			}

			return succeeded;
		}


		public bool SaveAll(bool logging = true)
		{

			//api_start2がロード済みのときのみ
			if (KCDatabase.Instance.MasterShips.Count == 0) return false;

			bool succeeded = true;

			foreach (var r in Records)
				succeeded &= r.SaveAll(MasterPath);

			if (logging)
			{
				if (succeeded)
					Utility.Logger.Add(2, "기록을 저장했습니다.");
				else
					Utility.Logger.Add(2, "기록 저장에 실패했습니다.");
			}

			return succeeded;
		}

		public bool SavePartial(bool logging = true)
		{

			//api_start2がロード済みのときのみ
			if (KCDatabase.Instance.MasterShips.Count == 0) return false;

			bool succeeded = true;


			foreach (var r in Records)
			{
				if (!r.NeedToSave)
				{
					continue;
				}

				if (r.SupportsPartialSave)
					succeeded &= r.SavePartial(MasterPath);
				else
					succeeded &= r.SaveAll(MasterPath);

			}

			if (logging)
			{
				if (succeeded)
					Utility.Logger.Add(2, "기록을 저장했습니다.");
				else
					Utility.Logger.Add(2, "기록 저장에 실패했습니다.");
			}

			return succeeded;
		}


		void TimerSave(string apiname, dynamic data)
		{

			bool iscleared;

			switch (Utility.Configuration.Config.Control.RecordAutoSaving)
			{
				case 0:
				default:
					iscleared = false;
					break;
				case 1:
					iscleared = DateTimeHelper.IsCrossedHour(_prevTime);
					break;
				case 2:
					iscleared = DateTimeHelper.IsCrossedDay(_prevTime, 0, 0, 0);
					break;
				case 3:
					iscleared = true;
					break;
			}


			if (iscleared)
			{
				_prevTime = DateTime.Now;

				if (Records.Any(r => r.NeedToSave))
				{

					if (SavePartial(false))
					{
						Utility.Logger.Add(1, "기록 자동 저장 하였습니다.");
					}
					else
					{
						Utility.Logger.Add(3, "기록 자동 저장 실패하였습니다.");
					}
				}
			}
		}

	}
}
