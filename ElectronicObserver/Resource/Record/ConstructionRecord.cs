using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Utility.Storage;
using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Resource.Record
{

	/// <summary>
	/// 建造のレコードです。
	/// </summary>
	[DebuggerDisplay("{Record.Count} Records")]
	public class ConstructionRecord : RecordBase
	{

		[DebuggerDisplay("[{ShipID}] : {ShipName}")]
		public sealed class ConstructionElement : RecordElementBase
		{

			/// <summary>
			/// 建造した艦のID
			/// </summary>
			public int ShipID { get; set; }

			/// <summary>
			/// 建造した艦の名前
			/// </summary>
			public string ShipName { get; set; }

			/// <summary>
			/// 建造日時
			/// </summary>
			public DateTime Date { get; set; }

			/// <summary>
			/// 投入燃料
			/// </summary>
			public int Fuel { get; set; }

			/// <summary>
			/// 投入弾薬
			/// </summary>
			public int Ammo { get; set; }

			/// <summary>
			/// 投入鋼材
			/// </summary>
			public int Steel { get; set; }

			/// <summary>
			/// 投入ボーキサイト
			/// </summary>
			public int Bauxite { get; set; }

			/// <summary>
			/// 投入開発資材
			/// </summary>
			public int DevelopmentMaterial { get; set; }

			/// <summary>
			/// 大型艦建造かのフラグ
			/// </summary>
			public bool IsLargeDock => this.Fuel >= 1000;

			/// <summary>
			/// 空きドック数
			/// </summary>
			public int EmptyDockAmount { get; set; }

			/// <summary>
			/// 旗艦の艦船ID
			/// </summary>
			public int FlagshipID { get; set; }

			/// <summary>
			/// 旗艦の艦名
			/// </summary>
			public string FlagshipName { get; set; }

			/// <summary>
			/// 司令部Lv.
			/// </summary>
			public int HQLevel { get; set; }



			public ConstructionElement()
			{
                this.ShipID = -1;
                this.Date = DateTime.Now;
			}

			public ConstructionElement(string line)
				: this()
			{
                this.LoadLine(line);
			}

			public ConstructionElement(int shipID, int fuel, int ammo, int steel, int bauxite, int developmentMaterial, int emptyDock, int flagshipID, int hqLevel)
			{
				var ship = KCDatabase.Instance.MasterShips[shipID];
				var flagship = KCDatabase.Instance.MasterShips[flagshipID];
                this.ShipID = shipID;
                this.ShipName = ship?.NameWithClass ?? "???";
                this.Date = DateTime.Now;
                this.Fuel = fuel;
                this.Ammo = ammo;
                this.Steel = steel;
                this.Bauxite = bauxite;
                this.DevelopmentMaterial = developmentMaterial;
                this.EmptyDockAmount = emptyDock;
                this.FlagshipID = flagshipID;
                this.FlagshipName = flagship?.NameWithClass ?? "???";
                this.HQLevel = hqLevel;
			}


			public override void LoadLine(string line)
			{

				string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
                if (elem.Length < 13)
					throw new ArgumentException("요소 수가 너무 적습니다.");

                this.ShipID = int.Parse(elem[0]);
                this.ShipName = elem[1];
                this.Date = DateTimeHelper.CSVStringToTime(elem[2]);
                this.Fuel = int.Parse(elem[3]);
                this.Ammo = int.Parse(elem[4]);
                this.Steel = int.Parse(elem[5]);
                this.Bauxite = int.Parse(elem[6]);
                this.DevelopmentMaterial = int.Parse(elem[7]);
                //IsLargeDock=elem[8]は読み飛ばす
                this.EmptyDockAmount = int.Parse(elem[9]);
                this.FlagshipID = int.Parse(elem[10]);
                this.FlagshipName = elem[11];
                this.HQLevel = int.Parse(elem[12]);

			}

			public override string SaveLine()
			{
				return string.Join(",",
                    this.ShipID,
                    CsvHelper.EscapeCsvCell(this.ShipName),
                    DateTimeHelper.TimeToCSVString(this.Date),
                    this.Fuel,
                    this.Ammo,
                    this.Steel,
                    this.Bauxite,
                    this.DevelopmentMaterial,
                    this.IsLargeDock ? 1 : 0,
                    this.EmptyDockAmount,
                    this.FlagshipID,
                    CsvHelper.EscapeCsvCell(this.FlagshipName),
                    this.HQLevel);
			}
		}



		public List<ConstructionElement> Record { get; private set; }
		private int ConstructingDockID;

		private int LastSavedCount;


		public ConstructionRecord()
			: base()
		{
            this.Record = new List<ConstructionElement>();
            this.ConstructingDockID = -1;
		}

		public override void RegisterEvents()
		{
			APIObserver ao = APIObserver.Instance;

			ao["api_req_kousyou/createship"].RequestReceived += this.ConstructionStart;
			ao["api_get_member/kdock"].ResponseReceived += this.ConstructionEnd;
		}


		public ConstructionElement this[int i]
		{
			get { return this.Record[i]; }
			set { this.Record[i] = value; }
		}



		void ConstructionStart(string apiname, dynamic data)
		{

            this.ConstructingDockID = int.Parse(data["api_kdock_id"]);

		}

		void ConstructionEnd(string apiname, dynamic data)
		{

			if (this.ConstructingDockID == -1) return;

			ArsenalData a = KCDatabase.Instance.Arsenals[this.ConstructingDockID];
			int emptyDock = KCDatabase.Instance.Arsenals.Values.Count(c => c.State == 0);
			ShipData flagship = KCDatabase.Instance.Fleet[1].MembersInstance[0];

            this.Record.Add(new ConstructionElement(a.ShipID, a.Fuel, a.Ammo, a.Steel, a.Bauxite, a.DevelopmentMaterial,
				emptyDock, flagship.ShipID, KCDatabase.Instance.Admiral.Level));

            this.ConstructingDockID = -1;
		}



		protected override void LoadLine(string line)
		{
            this.Record.Add(new ConstructionElement(line));
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


		public override string RecordHeader => "함선ID,함선명,날짜,연료,탄약,강재,보키,개발자재,대형건조,남은도크,기함ID,기함,사령부Lv";

		public override string FileName => "ConstructionRecord.csv";
	}


}
