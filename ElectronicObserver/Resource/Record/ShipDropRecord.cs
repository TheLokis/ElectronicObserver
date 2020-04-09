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
	public class ShipDropRecord : RecordBase
	{

		[DebuggerDisplay("[{Date}] : {ShipName} / {ItemName}")]
		public sealed class ShipDropElement : RecordElementBase
		{

			/// <summary>
			/// ドロップした艦のID　-1=なし
			/// </summary>
			public int ShipID { get; set; }

			/// <summary>
			/// ドロップした艦の名前
			/// </summary>
			public string ShipName { get; set; }

			/// <summary>
			/// ドロップしたアイテムのID　-1=なし
			/// </summary>
			public int ItemID { get; set; }

			/// <summary>
			/// ドロップしたアイテムの名前
			/// </summary>
			public string ItemName { get; set; }

			/// <summary>
			/// ドロップした装備のID　-1=なし
			/// </summary>
			public int EquipmentID { get; set; }

			/// <summary>
			/// ドロップした装備の名前
			/// </summary>
			public string EquipmentName { get; set; }

			/// <summary>
			/// ドロップした日時
			/// </summary>
			public DateTime Date { get; set; }

			/// <summary>
			/// 海域カテゴリID
			/// </summary>
			public int MapAreaID { get; set; }

			/// <summary>
			/// 海域カテゴリ内番号
			/// </summary>
			public int MapInfoID { get; set; }

			/// <summary>
			/// 海域セルID
			/// </summary>
			public int CellID { get; set; }

            /// <summary>
            /// 海域セルID List
            /// </summary>
            public int[] CellIDs { get; set; }

            /// <summary>
            /// 難易度(甲乙丙)
            /// </summary>
            public int Difficulty { get; set; }

			/// <summary>
			/// ボスかどうか
			/// </summary>
			public bool IsBossNode { get; set; }

			/// <summary>
			/// 敵編成ID
			/// </summary>
			public ulong EnemyFleetID { get; set; }

			/// <summary>
			/// 勝利ランク
			/// </summary>
			public string Rank { get; set; }

			/// <summary>
			/// 司令部Lv.
			/// </summary>
			public int HQLevel { get; set; }


			public ShipDropElement()
			{
                this.ShipID = -1;
                this.Date = DateTime.Now;
			}

			public ShipDropElement(string line)
				: this()
			{
                this.LoadLine(line);
			}

			public ShipDropElement(int shipID, int itemID, int equipmentID, int mapAreaID, int mapInfoID, int cellID, int difficulty, bool isBossNode, ulong enemyFleetID, string rank, int hqLevel)
			{
                this.ShipID = shipID;
				if (shipID == -1)
                    this.ShipName = "(없음)";
				else if (shipID == -2)
                    this.ShipName = "(여유공간X)";
				else
				{
					var ship = KCDatabase.Instance.MasterShips[shipID];
					if (ship != null)
                        this.ShipName = ship.NameWithClass;
					else
                        this.ShipName = "???";
				}

                this.ItemID = itemID;
				if (itemID == -1)
                    this.ItemName = "(없음)";
				else
				{
					var item = KCDatabase.Instance.MasterUseItems[itemID];
					if (item != null)
                        this.ItemName = item.Name;
					else
                        this.ItemName = "???";
				}

                this.EquipmentID = equipmentID;
				if (equipmentID == -1)
                    this.EquipmentName = "(없음)";
				else
				{
					var eq = KCDatabase.Instance.MasterEquipments[equipmentID];
					if (eq != null)
                        this.EquipmentName = eq.Name;
					else
                        this.EquipmentName = "???";
				}

                this.Date = DateTime.Now;
                this.MapAreaID = mapAreaID;
                this.MapInfoID = mapInfoID;
                this.CellID = cellID;
                this.Difficulty = difficulty;
                this.IsBossNode = isBossNode;
                this.EnemyFleetID = enemyFleetID;
                this.Rank = rank;
                this.HQLevel = hqLevel;
			}


			public override void LoadLine(string line)
			{

				string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
                if (elem.Length < 15) throw new ArgumentException("요소 수가 너무 적습니다.");

                this.ShipID = int.Parse(elem[0]);
                this.ShipName = elem[1];
                this.ItemID = int.Parse(elem[2]);
                this.ItemName = elem[3];
                this.EquipmentID = int.Parse(elem[4]);
                this.EquipmentName = elem[5];
                this.Date = DateTimeHelper.CSVStringToTime(elem[6]);
                this.MapAreaID = int.Parse(elem[7]);
                this.MapInfoID = int.Parse(elem[8]);
                this.CellID = int.Parse(elem[9]);
                this.Difficulty = Constants.GetDifficulty(elem[10]);
                this.IsBossNode = string.Compare(elem[11], "보스") == 0;
                this.EnemyFleetID = Convert.ToUInt64(elem[12], 16);
                this.Rank = elem[13];
                this.HQLevel = int.Parse(elem[14]);

			}

            public override string SaveLine()
            {

                return string.Join(",",
                    this.ShipID,
                    CsvHelper.EscapeCsvCell(this.ShipName),
                    this.ItemID,
                    CsvHelper.EscapeCsvCell(this.ItemName),
                    this.EquipmentID,
                    CsvHelper.EscapeCsvCell(this.EquipmentName),
                    DateTimeHelper.TimeToCSVString(this.Date),
                    this.MapAreaID,
                    this.MapInfoID,
                    this.CellID,
                    Constants.GetDifficulty(this.Difficulty),
                    this.IsBossNode ? "보스" : "-",
                    this.EnemyFleetID.ToString("x16"),
                    this.Rank,
                    this.HQLevel);

            }
        }



		public List<ShipDropElement> Record { get; private set; }
		private int LastSavedCount;


		public ShipDropRecord()
			: base()
		{
            this.Record = new List<ShipDropElement>();
		}

		public override void RegisterEvents()
		{
			// nop
		}


		public ShipDropElement this[int i]
		{
			get { return this.Record[i]; }
			set { this.Record[i] = value; }
		}

		public void Add(int shipID, int itemID, int equipmentID, int mapAreaID, int mapInfoID, int cellID, int difficulty, bool isBossNode, ulong enemyFleetID, string rank, int hqLevel)
		{

            this.Record.Add(new ShipDropElement(shipID, itemID, equipmentID, mapAreaID, mapInfoID, cellID, difficulty, isBossNode, enemyFleetID, rank, hqLevel));
		}


		protected override void LoadLine(string line)
		{
            this.Record.Add(new ShipDropElement(line));
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


		public override string RecordHeader => "함선ID,함명,아이템ID,아이템이름,장비ID,장비명,시간,해역,해역,노드,난이도,보스,적편성ID,랭크,사령부Lv";

		public override string FileName => "ShipDropRecord.csv";
	}

}
