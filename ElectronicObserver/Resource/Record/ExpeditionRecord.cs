using ElectronicObserver.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Utility.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Resource.Record
{

	[DebuggerDisplay("{Record.Count} Records")]
	public class ExpeditionRecord : RecordBase
	{

		[DebuggerDisplay("[{Date}] : {MissionID}")]
		public sealed class ExpeditionRecordElement : RecordElementBase
		{
			/// <summary>
			/// 기함 ID
			/// </summary>
			public int ShipID { get; set; }

			/// <summary>
			/// 기함 명
			/// </summary>
			public string ShipName { get; set; }

			/// <summary>
			/// 기함 타입
			/// </summary>
			public string ShipType { get; set; }

			/// <summary>
			/// 기함 레벨
			/// </summary>
			public int ShipLevel { get; set; }

			/// <summary>
			/// 도착한 시간
			/// </summary>
			public DateTime Date { get; set; }

			/// <summary>
			/// 원정 ID
			/// </summary>
			public int MissionID { get; set; }

			/// <summary>
			/// 획득한 자원 - 연료
			/// </summary>
			public int Fuel;

            /// <summary>
            /// 획득한 자원 - 탄약
            /// </summary>
            public int Ammo;

            /// <summary>
            /// 획득한 자원 - 강재
            /// </summary>
            public int Steel;

            /// <summary>
            /// 획득한 자원 - 보키
            /// </summary>
            public int Baux;

			/// <summary>
			/// 대성공여부
			/// </summary>
			public bool IsGreat;

			public ExpeditionRecordElement()
			{
                this.ShipID = -1;
                this.Date = DateTime.Now;
			}

			public ExpeditionRecordElement(string line)
				: this()
			{
                this.LoadLine(line);
			}

			public ExpeditionRecordElement(int missionID, int flagShipID, int fuel, int ammo, int steel, int baux, bool isGreat)
			{
				this.MissionID = missionID;
				this.ShipID = flagShipID;

				if(flagShipID != -1)
				{
					var ship = KCDatabase.Instance.Ships[flagShipID];
					this.ShipLevel = ship.Level;
					this.ShipName = ship.MasterShip.Name;
					this.ShipType = ship.MasterShip.ShipTypeName;
				}

				this.Fuel = fuel;
				this.Ammo = ammo;
				this.Steel = steel;
				this.Baux = baux;
				this.IsGreat = isGreat;

                this.Date = DateTime.Now;
			}


			public override void LoadLine(string line)
			{
				string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
                if (elem.Length < 10) throw new ArgumentException("요소 수가 너무 적습니다.");

				this.MissionID = int.Parse(elem[0]);
				this.Date = DateTime.Parse(elem[1]);
				this.ShipName = elem[2];
				this.ShipType = elem[3];
				this.ShipLevel = int.Parse(elem[4]);
				this.Fuel = int.Parse(elem[5]);
				this.Ammo = int.Parse(elem[6]);
				this.Steel = int.Parse(elem[7]);
				this.Baux = int.Parse(elem[8]);
				this.IsGreat = Boolean.Parse(elem[9]);
            }

            public override string SaveLine()
            {

				return string.Join(",",
					this.MissionID,
					DateTimeHelper.TimeToCSVString(this.Date),
					CsvHelper.EscapeCsvCell(this.ShipName),
					CsvHelper.EscapeCsvCell(this.ShipType),
					this.ShipLevel,
					this.Fuel,
					this.Ammo,
					this.Steel,
					this.Baux,
					this.IsGreat
				);
            }
        }



		public List<ExpeditionRecordElement> Record { get; private set; }
		private int _lastSavedCount;

		public ExpeditionRecord()
			: base()
		{
            this.Record = new List<ExpeditionRecordElement>();
		}

		public override void RegisterEvents()
		{
			// nop
		}


		public ExpeditionRecordElement this[int i]
		{
			get { return this.Record[i]; }
			set { this.Record[i] = value; }
		}

		public void Add(int missionID, int flagShipID, int fuel, int ammo, int steel, int baux, bool isGreat)
		{
            this.Record.Add(new ExpeditionRecordElement(missionID, flagShipID, fuel, ammo, steel, baux, isGreat));
		}


		protected override void LoadLine(string line)
		{
            this.Record.Add(new ExpeditionRecordElement(line));
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
			foreach (var elem in this.Record.Skip(this._lastSavedCount).OrderBy(r => r.Date))
			{
				sb.AppendLine(elem.SaveLine());
			}
			return sb.ToString();
		}

		protected override void UpdateLastSavedIndex()
		{
            this._lastSavedCount = this.Record.Count;
		}

		public override bool NeedToSave => this._lastSavedCount < this.Record.Count;

		public override bool SupportsPartialSave => true;


		protected override void ClearRecord()
		{
            this.Record.Clear();
            this._lastSavedCount = 0;
        }

        public override string RecordHeader => "원정번호,날짜,기함명,기함종,레벨,연료,탄약,강재,보키,대성공";

		public override string FileName => "ExpeditionRecord.csv";
	}

}
