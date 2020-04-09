using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Utility.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Resource.Record
{

	/// <summary>
	/// 資源のレコードを保持します。
	/// </summary>
	public class ResourceRecord : RecordBase
	{

		public sealed class ResourceElement : RecordElementBase
		{

			/// <summary>
			/// 記録日時
			/// </summary>
			public DateTime Date { get; set; }

			/// <summary>
			/// 燃料
			/// </summary>
			public int Fuel { get; set; }

			/// <summary>
			/// 弾薬
			/// </summary>
			public int Ammo { get; set; }

			/// <summary>
			/// 鋼材
			/// </summary>
			public int Steel { get; set; }

			/// <summary>
			/// ボーキサイト
			/// </summary>
			public int Bauxite { get; set; }


			/// <summary>
			/// 高速建造材
			/// </summary>
			public int InstantConstruction { get; set; }

			/// <summary>
			/// 高速修復材
			/// </summary>
			public int InstantRepair { get; set; }

			/// <summary>
			/// 開発資材
			/// </summary>
			public int DevelopmentMaterial { get; set; }

			/// <summary>
			/// 改修資材
			/// </summary>
			public int ModdingMaterial { get; set; }

			/// <summary>
			/// 艦隊司令部Lv.
			/// </summary>
			public int HQLevel { get; set; }

			/// <summary>
			/// 提督経験値
			/// </summary>
			public int HQExp { get; set; }


			public ResourceElement()
			{
                this.Date = DateTime.Now;
			}

			public ResourceElement(string line)
				: this()
			{
                this.LoadLine(line);
			}

			public ResourceElement(int fuel, int ammo, int steel, int bauxite, int instantConstruction, int instantRepair, int developmentMaterial, int moddingMaterial, int hqLevel, int hqExp)
				: this()
			{
                this.Fuel = fuel;
                this.Ammo = ammo;
                this.Steel = steel;
                this.Bauxite = bauxite;
                this.InstantConstruction = instantConstruction;
                this.InstantRepair = instantRepair;
                this.DevelopmentMaterial = developmentMaterial;
                this.ModdingMaterial = moddingMaterial;
                this.HQLevel = hqLevel;
                this.HQExp = hqExp;
			}

			public override void LoadLine(string line)
			{

				string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
                if (elem.Length < 11) throw new ArgumentException("요소 수가 너무 적습니다.");

                this.Date = DateTimeHelper.CSVStringToTime(elem[0]);
                this.Fuel = int.Parse(elem[1]);
                this.Ammo = int.Parse(elem[2]);
                this.Steel = int.Parse(elem[3]);
                this.Bauxite = int.Parse(elem[4]);
                this.InstantConstruction = int.Parse(elem[5]);
                this.InstantRepair = int.Parse(elem[6]);
                this.DevelopmentMaterial = int.Parse(elem[7]);
                this.ModdingMaterial = int.Parse(elem[8]);
                this.HQLevel = int.Parse(elem[9]);
                this.HQExp = int.Parse(elem[10]);

			}

			public override string SaveLine()
			{
				return string.Join(",",
                    DateTimeHelper.TimeToCSVString(this.Date),
                    this.Fuel,
                    this.Ammo,
                    this.Steel,
                    this.Bauxite,
                    this.InstantConstruction,
                    this.InstantRepair,
                    this.DevelopmentMaterial,
                    this.ModdingMaterial,
                    this.HQLevel,
                    this.HQExp);
			}

		}


		public List<ResourceElement> Record { get; private set; }
		private DateTime _prevTime;
		private bool _initialFlag;
		private int LastSavedCount;


		public ResourceRecord()
			: base()
		{

            this.Record = new List<ResourceElement>();
            this._prevTime = DateTime.Now;
            this._initialFlag = false;
		}

		public override void RegisterEvents()
		{
			var ao = APIObserver.Instance;

			ao["api_start2/getData"].ResponseReceived += this.ResourceRecord_Started;
			ao["api_port/port"].ResponseReceived += this.ResourceRecord_Updated;
		}


		private void ResourceRecord_Started(string apiname, dynamic data)
		{
            this._initialFlag = true;
		}


		void ResourceRecord_Updated(string apiname, dynamic data)
		{

			if (this._initialFlag || DateTimeHelper.IsCrossedHour(this._prevTime))
			{
                this._prevTime = DateTime.Now;
                this._initialFlag = false;

				var material = KCDatabase.Instance.Material;
				var admiral = KCDatabase.Instance.Admiral;
                this.Record.Add(new ResourceElement(
					material.Fuel,
					material.Ammo,
					material.Steel,
					material.Bauxite,
					material.InstantConstruction,
					material.InstantRepair,
					material.DevelopmentMaterial,
					material.ModdingMaterial,
					admiral.Level,
					admiral.Exp));
			}
		}


		public ResourceElement this[int i]
		{
			get { return this.Record[i]; }
			set { this.Record[i] = value; }
		}


		/// <summary>
		/// 指定した日時以降の最も古い記録を返します。
		/// </summary>
		public ResourceElement GetRecord(DateTime target)
		{

			int i;
			for (i = this.Record.Count - 1; i >= 0; i--)
			{
				if (this.Record[i].Date < target)
				{
					i++;
					break;
				}
			}
			// Record内の全ての記録がtarget以降だった
			if (i < 0)
				i = 0;

			if (0 <= i && i < this.Record.Count)
			{
				return this.Record[i];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 前回の戦果更新以降の最も古い記録を返します。
		/// </summary>
		public ResourceElement GetRecordPrevious()
		{

			DateTime now = DateTime.Now;
			DateTime target;
			if (now.TimeOfDay.Hours < 2)
			{
				target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0).Subtract(TimeSpan.FromDays(1));
			}
			else if (now.TimeOfDay.Hours < 14)
			{
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
			}
			else
			{
				target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
			}

			return this.GetRecord(target);
		}

		/// <summary>
		/// 今日の戦果更新以降の最も古い記録を返します。
		/// </summary>
		public ResourceElement GetRecordDay()
		{

			DateTime now = DateTime.Now;
			DateTime target;
			if (now.TimeOfDay.Hours < 2)
			{
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0).Subtract(TimeSpan.FromDays(1));
			}
			else
			{
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
			}

			return this.GetRecord(target);
		}

		/// <summary>
		/// 今月の戦果更新以降の最も古い記録を返します。
		/// </summary>
		public ResourceElement GetRecordMonth()
		{
			DateTime now = DateTime.Now;

			return this.GetRecord(new DateTime(now.Year, now.Month, 1));
		}




		protected override void LoadLine(string line)
		{
            this.Record.Add(new ResourceElement(line));
		}

		protected override string SaveLinesAll()
		{
			var sb = new StringBuilder();
			foreach (var elem in this.Record.OrderBy(r => r.Date))
			{
				sb.AppendLine(elem.SaveLine());
			}
			return sb.ToString();
		}

		protected override string SaveLinesPartial()
		{
			var sb = new StringBuilder();
			foreach (var elem in this.Record.Skip(this.LastSavedCount).OrderBy(r => r.Date))
			{
				sb.AppendLine(elem.SaveLine());
			}
			return sb.ToString();
		}

		protected override void UpdateLastSavedIndex()
		{
            this.LastSavedCount = this.Record.Count;
		}

		public override bool NeedToSave => this.LastSavedCount < this.Record.Count;

		public override bool SupportsPartialSave => true;

		protected override void ClearRecord()
		{
            this.Record.Clear();
            this.LastSavedCount = 0;
		}



		public override string RecordHeader => "날짜,연료,탄약,강재,보키,고속건조재,고속수복재,개발자재,개수자재,사령부Lv,제독Exp";

		public override string FileName => "ResourceRecord.csv";
	}

}
