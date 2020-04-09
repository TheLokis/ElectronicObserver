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
				yield return this.EnemyFleet;
				yield return this.ShipParameter;
				yield return this.Construction;
				yield return this.ShipDrop;
				yield return this.Development;
				yield return this.Resource;
			}
		}


		private DateTime _prevTime;

		private RecordManager()
		{

            this.MasterPath = @"Record";
            this.EnemyFleet = new EnemyFleetRecord();
            this.ShipParameter = new ShipParameterRecord();
            this.Construction = new ConstructionRecord();
            this.ShipDrop = new ShipDropRecord();
            this.Development = new DevelopmentRecord();
            this.Resource = new ResourceRecord();

			foreach (var r in this.Records)
				r.RegisterEvents();


			if (!Directory.Exists(this.MasterPath))
			{
				Directory.CreateDirectory(this.MasterPath);
			}

            this._prevTime = DateTime.Now;
			Observer.APIObserver.Instance["api_port/port"].ResponseReceived += this.TimerSave;
		}

		public bool Load(bool logging = true)
		{

			bool succeeded = true;

			ResourceManager.CopyDocumentFromArchive("Record/" + this.ShipParameter.FileName, this.MasterPath + "\\" + this.ShipParameter.FileName);

			foreach (var r in this.Records)
				succeeded &= r.Load(this.MasterPath);

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

			foreach (var r in this.Records)
				succeeded &= r.SaveAll(this.MasterPath);

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


			foreach (var r in this.Records)
			{
				if (!r.NeedToSave)
				{
					continue;
				}

				if (r.SupportsPartialSave)
					succeeded &= r.SavePartial(this.MasterPath);
				else
					succeeded &= r.SaveAll(this.MasterPath);

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
					iscleared = DateTimeHelper.IsCrossedHour(this._prevTime);
					break;
				case 2:
					iscleared = DateTimeHelper.IsCrossedDay(this._prevTime, 0, 0, 0);
					break;
				case 3:
					iscleared = true;
					break;
			}


			if (iscleared)
			{
                this._prevTime = DateTime.Now;

				if (this.Records.Any(r => r.NeedToSave))
				{

					if (this.SavePartial(false))
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
