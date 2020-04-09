using ElectronicObserver.Data;
using ElectronicObserver.Data.Battle;
using ElectronicObserver.Utility.Storage;
using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Resource.Record
{

	/// <summary>
	/// 敵艦隊編成のレコードです。
	/// </summary>
	[DebuggerDisplay("{Record.Count} Records")]
	public class EnemyFleetRecord : RecordBase
	{

		public sealed class EnemyFleetElement : RecordElementBase
		{

			/// <summary>
			/// 艦隊ID
			/// </summary>
			public ulong FleetID { get; private set; }

			/// <summary>
			/// 艦隊名
			/// </summary>
			public string FleetName { get; private set; }

			/// <summary>
			/// 海域カテゴリID
			/// </summary>
			public int MapAreaID { get; private set; }

			/// <summary>
			/// 海域カテゴリ内番号
			/// </summary>
			public int MapInfoID { get; private set; }

			/// <summary>
			/// 海域セルID
			/// </summary>
			public int CellID { get; private set; }

			/// <summary>
			/// 海域難易度(甲乙丙)
			/// </summary>
			public int Difficulty { get; private set; }

			/// <summary>
			/// 陣形
			/// </summary>
			public int Formation { get; private set; }

			/// <summary>
			/// 敵艦船リスト [12]
			/// </summary>
			public int[] FleetMember { get; private set; }

			/// <summary>
			/// 敵艦船レベル [12]
			/// </summary>
			public int[] FleetMemberLevel { get; private set; }


			/// <summary>
			/// 艦娘の獲得経験値
			/// </summary>
			public int ExpShip { get; private set; }


			/// <summary>
			/// 連合艦隊かどうか
			/// </summary>
			public bool IsCombined => this.Formation >= 10;



			public EnemyFleetElement()
				: base() { }

			public EnemyFleetElement(string line)
				: this()
			{
                this.LoadLine(line);
			}

			public EnemyFleetElement(string fleetName, int mapAreaID, int mapInfoID, int cellID, int difficulty, int formation, int[] fleetMember, int[] fleetMemberLevel, int expShip)
				: base()
			{
                this.FleetName = FormMain.Instance.Translator.GetTranslation(fleetName, Utility.DataType.OperationSortie);
                this.MapAreaID = mapAreaID;
                this.MapInfoID = mapInfoID;
                this.CellID = cellID;
                this.Difficulty = difficulty;
                this.Formation = formation;

				int[] To12Array(int[] a) => a.Length < 12 ? a.Concat(Enumerable.Repeat(-1, 12 - a.Length)).ToArray() : a.Take(12).ToArray();

                this.FleetMember = To12Array(fleetMember);
                this.FleetMemberLevel = To12Array(fleetMemberLevel);
                this.ExpShip = expShip;


                this.FleetID = this.ComputeHash();
			}


			public override void LoadLine(string line)
			{

				string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
                if (elem.Length < 44)
					throw new ArgumentException("요소 수가 너무 적습니다.");

				ulong id = Convert.ToUInt64(elem[0], 16);
                this.FleetName = FormMain.Instance.Translator.GetTranslation(elem[1], Utility.DataType.OperationSortie);
                //elem[1];
                this.MapAreaID = int.Parse(elem[2]);
                this.MapInfoID = int.Parse(elem[3]);
                this.CellID = int.Parse(elem[4]);
                this.Difficulty = Constants.GetDifficulty(elem[5]);
                this.Formation = Constants.GetFormation(elem[6]);
                this.ExpShip = int.Parse(elem[7]);

                this.FleetMember = new int[12];
				for (int i = 0; i < this.FleetMember.Length; i++)
                    this.FleetMember[i] = int.Parse(elem[8 + i]);

                this.FleetMemberLevel = new int[12];
				for (int i = 0; i < this.FleetMember.Length; i++)
                    this.FleetMemberLevel[i] = int.Parse(elem[32 + i]);


                this.FleetID = this.ComputeHash();

				if (this.FleetID != id)
					Utility.Logger.Add(1, $"EnemyFleetRecord: 적 편성 ID에 오류가 있습니다. (기록된 ID {id:x16} -> 현재 ID {this.FleetID:x16})");
			}

			public override string SaveLine()
			{
				return string.Join(",",
                    this.FleetID.ToString("x16"),
                    CsvHelper.EscapeCsvCell(this.FleetName),
                    this.MapAreaID,
                    this.MapInfoID,
                    this.CellID,
					Constants.GetDifficulty(this.Difficulty),
					Constants.GetFormation(this.Formation),
                    this.ExpShip,
					string.Join(",", this.FleetMember),
                    string.Join(",", this.FleetMember.Select(id => CsvHelper.EscapeCsvCell(KCDatabase.Instance.MasterShips[id]?.NameWithClass ?? "-"))),
                    string.Join(",", this.FleetMemberLevel)
					);
			}


			/// <summary>
			/// 現在のインスタンスのIDとなるハッシュ値を求めます。
			/// </summary>
			/// <returns></returns>
			private ulong ComputeHash()
			{
				string key = string.Join(",", this.MapAreaID, this.MapInfoID, this.CellID, this.Difficulty, this.Formation, string.Join(",", this.FleetMember), string.Join(",", this.FleetMemberLevel));
				return BitConverter.ToUInt64(Utility.Data.RecordHash.ComputeHash(key), 0);
			}


			/// <summary>
			/// 現在の状態からインスタンスを生成します。
			/// </summary>
			public static EnemyFleetElement CreateFromCurrentState()
			{

				var battle = KCDatabase.Instance.Battle;
				string fleetName = battle.IsBaseAirRaid ? "적기지공습" : battle.Result?.EnemyFleetName ?? "";
				int baseExp = battle.Result?.BaseExp ?? 0;
				var initial = battle.FirstBattle.Initial;

				if (battle.IsPractice)
					return null;


				return new EnemyFleetElement(
					fleetName,
					battle.Compass.MapAreaID,
					battle.Compass.MapInfoID,
					battle.IsBaseAirRaid ? -1 : battle.Compass.Destination,
					battle.Compass.MapInfo.EventDifficulty,
					battle.FirstBattle.Searching.FormationEnemy,
					battle.IsEnemyCombined ? initial.EnemyMembers.Take(6).Concat(initial.EnemyMembersEscort).ToArray() : initial.EnemyMembers,
					battle.IsEnemyCombined ? initial.EnemyLevels.Take(6).Concat(initial.EnemyLevelsEscort).ToArray() : initial.EnemyLevels,
					baseExp);

			}

			public override string ToString() => $"[{this.FleetID:x16}] {this.MapAreaID}-{this.MapInfoID}-{this.CellID} {this.FleetName}";
		}



		public Dictionary<ulong, EnemyFleetElement> Record { get; private set; }
		private bool _changed;


		public EnemyFleetRecord()
			: base()
		{
            this.Record = new Dictionary<ulong, EnemyFleetElement>();
            this._changed = false;
		}

		public override void RegisterEvents()
		{
			// nop
		}


		public EnemyFleetElement this[ulong i]
		{
			get
			{
				return this.Record.ContainsKey(i) ? this.Record[i] : null;
			}
			set
			{
				if (!this.Record.ContainsKey(i))
				{
                    this.Record.Add(i, value);
				}
				else
				{
                    this.Record[i] = value;
				}
                this._changed = true;
			}
		}


		public void Update(EnemyFleetElement elem)
		{
			this[elem.FleetID] = elem;
		}


		protected override void LoadLine(string line)
		{
            this.Update(new EnemyFleetElement(line));
		}

		protected override string SaveLinesAll()
		{
			var sb = new StringBuilder();

			var rs = this.Record.Values
				.OrderBy(r => r.MapAreaID)
				.ThenBy(r => r.MapInfoID)
				.ThenBy(r => r.CellID)
				.ThenBy(r => r.Difficulty);

			for (int i = 0; i < 12; i++)
			{
				int ii = i;
				rs = rs.ThenBy(r => r.FleetMember[ii]);
			}

			rs = rs
				.ThenBy(r => r.Formation)
				.ThenBy(r => r.ExpShip);

			foreach (var elem in rs)
				sb.AppendLine(elem.SaveLine());


			return sb.ToString();
		}

		protected override string SaveLinesPartial()
		{
			throw new NotSupportedException();
		}

		protected override void UpdateLastSavedIndex()
		{
            this._changed = false;
		}

		public override bool NeedToSave => this._changed;

		public override bool SupportsPartialSave => false;

		protected override void ClearRecord()
		{
            this.Record.Clear();
		}


		public override string RecordHeader => "적편성ID,적함대명,해역,해역,노드,난이도,진형,함선경험치,ID#01,ID#02,ID#03,ID#04,ID#05,ID#06,ID#07,ID#08,ID#09,ID#10,ID#11,ID#12,함명#01,함명#02,함명#03,함명#04,함명#05,함명#06,함명,함명#08,함명#09,함명#10,함명#11,함명#12,Lv#01,Lv#02,Lv#03,Lv#04,Lv#05,Lv#06,Lv#07,Lv#08,Lv#09,Lv#10,Lv#11,Lv#12";

		public override string FileName => "EnemyFleetRecord.csv";
	}


}
