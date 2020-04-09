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
	public class DevelopmentRecord : RecordBase
	{

		[DebuggerDisplay("[{EquipmentID}] : {EquipmentName}")]
		public sealed class DevelopmentElement : RecordElementBase
		{

			/// <summary>
			/// 開発した装備のID
			/// </summary>
			public int EquipmentID { get; set; }

			/// <summary>
			/// 開発した装備の名前
			/// </summary>
			public string EquipmentName { get; set; }

			/// <summary>
			/// 開発日時
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
			/// 旗艦の艦船ID
			/// </summary>
			public int FlagshipID { get; set; }

			/// <summary>
			/// 旗艦の艦名
			/// </summary>
			public string FlagshipName { get; set; }

			/// <summary>
			/// 旗艦の艦種
			/// </summary>
			public int FlagshipType { get; set; }

			/// <summary>
			/// 司令部Lv.
			/// </summary>
			public int HQLevel { get; set; }



			public DevelopmentElement()
			{
                this.EquipmentID = -1;
                this.Date = DateTime.Now;
			}

			public DevelopmentElement(string line)
				: this()
			{
                this.LoadLine(line);
			}

			public DevelopmentElement(int equipmentID, int fuel, int ammo, int steel, int bauxite, int flagshipID, int hqLevel)
			{
                this.EquipmentID = equipmentID;
                this.Fuel = fuel;
                this.Ammo = ammo;
                this.Steel = steel;
                this.Bauxite = bauxite;
                this.FlagshipID = flagshipID;
                this.HQLevel = hqLevel;

                this.SetSubParameters();
			}


			public override void LoadLine(string line)
			{

				string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
                if (elem.Length < 11) throw new ArgumentException("요소 수가 너무 적습니다.");

                this.EquipmentID = int.Parse(elem[0]);
                this.EquipmentName = elem[1];
                this.Date = DateTimeHelper.CSVStringToTime(elem[2]);
                this.Fuel = int.Parse(elem[3]);
                this.Ammo = int.Parse(elem[4]);
                this.Steel = int.Parse(elem[5]);
                this.Bauxite = int.Parse(elem[6]);
                this.FlagshipID = int.Parse(elem[7]);
                this.FlagshipName = elem[8];
                this.FlagshipType = int.Parse(elem[9]);
                this.HQLevel = int.Parse(elem[10]);

			}

			public override string SaveLine()
			{

				return string.Join(",",
                    this.EquipmentID,
                    CsvHelper.EscapeCsvCell(this.EquipmentName),
                    DateTimeHelper.TimeToCSVString(this.Date),
                    this.Fuel,
                    this.Ammo,
                    this.Steel,
                    this.Bauxite,
                    this.FlagshipID,
                    CsvHelper.EscapeCsvCell(this.FlagshipName),
                    this.FlagshipType,
                    this.HQLevel);
			}

			/// <summary>
			/// 艦名などのパラメータを現在のIDをもとに設定します。
			/// </summary>
			public void SetSubParameters()
			{
				var eq = KCDatabase.Instance.MasterEquipments[this.EquipmentID];
				var flagship = KCDatabase.Instance.MasterShips[this.FlagshipID];

                this.EquipmentName = this.EquipmentID == -1 ? "(실패)" :
					eq?.Name ?? "???";
                this.FlagshipName = flagship?.NameWithClass ?? "???";
                this.FlagshipType = (int?)flagship?.ShipType ?? -1;
			}
		}



		public List<DevelopmentElement> Record { get; private set; }
		private DevelopmentElement tempElement;
		private int LastSavedCount;


		public DevelopmentRecord()
		{
            this.Record = new List<DevelopmentElement>();
		}

		public override void RegisterEvents()
		{
            APIObserver.Instance["api_req_kousyou/createitem"].ResponseReceived += this.DevelopmentEnd;
        }


		public DevelopmentElement this[int i]
		{
			get { return this.Record[i]; }
			set { this.Record[i] = value; }
		}


		private void DevelopmentEnd(string apiname, dynamic data)
		{
            var dev = KCDatabase.Instance.Development;
            var flagshipID = KCDatabase.Instance.Fleet[1].MembersInstance[0].ShipID;
            var hqLevel = KCDatabase.Instance.Admiral.Level;

            foreach (var result in dev.Results)
            {
                var element = new DevelopmentElement
                {
                    Fuel = dev.Fuel,
                    Ammo = dev.Ammo,
                    Steel = dev.Steel,
                    Bauxite = dev.Bauxite,

                    EquipmentID = result.EquipmentID,
                    FlagshipID = flagshipID,
                    HQLevel = hqLevel,
                };

                element.SetSubParameters();
                this.Record.Add(element);
            }
        }



		protected override void LoadLine(string line)
		{
            this.Record.Add(new DevelopmentElement(line));
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


		public override string RecordHeader => "장비ID,장비명,개발날짜,연료,탄약,강재,보키,기함ID,기함,함종,사령부Lv";

		public override string FileName => "DevelopmentRecord.csv";
	}


}
