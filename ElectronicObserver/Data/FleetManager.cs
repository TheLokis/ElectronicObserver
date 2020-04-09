using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 艦隊情報を統括して扱います。
	/// </summary>
	public class FleetManager : APIWrapper
	{

		public IDDictionary<FleetData> Fleets { get; private set; }


		/// <summary>
		/// 連合艦隊フラグ
		/// </summary>
		public int CombinedFlag { get; internal set; }

		/// <summary>
		/// 泊地修理タイマ
		/// </summary>
		public DateTime AnchorageRepairingTimer { get; private set; }



		/// <summary> 更新直前の艦船データ </summary>
		private IDDictionary<ShipData> PreviousShips;

		/// <summary> 更新直前に泊地修理が可能だったか </summary>
		private Dictionary<int, bool> IsAnchorageRepaired;

		/// <summary> 更新直前の入渠艦IDリスト </summary>
		private HashSet<int> PreviousDockingID;

		// conditions
		public static readonly TimeSpan ConditionHealingSpan = TimeSpan.FromSeconds(180);
		private double ConditionPredictMin;
		private double ConditionPredictMax;
		private DateTime LastConditionUpdated;

		/// <summary> コンディションが回復する秒オフセット </summary>
		public double ConditionBorderSeconds => this.ConditionPredictMax % ConditionHealingSpan.TotalSeconds;

		/// <summary> コンディションが回復する秒オフセット の精度[秒] </summary>
		public double ConditionBorderAccuracy => this.ConditionPredictMax - this.ConditionPredictMin;


		public FleetManager()
		{
            this.Fleets = new IDDictionary<FleetData>();
            this.AnchorageRepairingTimer = DateTime.MinValue;

            this.ConditionPredictMin = 0;
            this.ConditionPredictMax = ConditionHealingSpan.TotalSeconds * 2;
            this.LastConditionUpdated = DateTime.Now;
            this.PreviousShips = new IDDictionary<ShipData>();
            this.IsAnchorageRepaired = new Dictionary<int, bool>();
		}


		public FleetData this[int fleetID] => this.Fleets[fleetID];



		public override void LoadFromResponse(string apiname, dynamic data)
		{

			switch (apiname)
			{
				case "api_req_sortie/goback_port":
				case "api_req_combined_battle/goback_port":
					{
						var battle = KCDatabase.Instance.Battle;

						foreach (int ii in battle.Result.EscapingShipIndex)
						{
							int index = ii - 1;

							if (index < battle.FirstBattle.Initial.FriendFleet.Members.Count)
								battle.FirstBattle.Initial.FriendFleet.Escape(index);
							else
								battle.FirstBattle.Initial.FriendFleetEscort.Escape(index - 6);
						}
					}
					break;

				case "api_get_member/ndock":
					foreach (var fleet in this.Fleets.Values)
					{
						fleet.LoadFromResponse(apiname, data);
					}
					break;

				case "api_req_hensei/preset_select":
					{
						int id = (int)data.api_id;

						if (!this.Fleets.ContainsKey(id))
						{
							var a = new FleetData();
							a.LoadFromResponse(apiname, data);
                            this.Fleets.Add(a);

						}
						else
						{
                            this.Fleets[id].LoadFromResponse(apiname, data);
						}

					}
					break;

				default:
					base.LoadFromResponse(apiname, (object)data);

					//api_port/port, api_get_member/deck
					foreach (var elem in data)
					{

						int id = (int)elem.api_id;

						if (!this.Fleets.ContainsKey(id))
						{
							var a = new FleetData();
							a.LoadFromResponse(apiname, elem);
                            this.Fleets.Add(a);

						}
						else
						{
                            this.Fleets[id].LoadFromResponse(apiname, elem);
						}
					}
					break;
			}


			// 泊地修理・コンディションの処理
			if (apiname == "api_port/port")
			{

				if ((DateTime.Now - this.AnchorageRepairingTimer).TotalMinutes >= 20)
                    this.StartAnchorageRepairingTimer();
				else
                    this.CheckAnchorageRepairingHealing();

                this.UpdateConditionPrediction();
			}
		}



		public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
		{
			base.LoadFromRequest(apiname, data);

			switch (apiname)
			{
				case "api_req_hensei/change":
					{
						int memberID = int.Parse(data["api_ship_idx"]);     //変更スロット
						if (memberID != -1)
							data.Add("replaced_id", this.Fleets[int.Parse(data["api_id"])].Members[memberID].ToString());

						foreach (int i in this.Fleets.Keys)
                            this.Fleets[i].LoadFromRequest(apiname, data);

					}
					break;

				case "api_req_map/start":
					{
						int fleetID = int.Parse(data["api_deck_id"]);
						if (this.CombinedFlag != 0 && fleetID == 1)
						{
                            this.Fleets[2].IsInSortie = true;
						}
                        this.Fleets[fleetID].IsInSortie = true;
					}
					goto default;

				case "api_req_hensei/combined":
                    this.CombinedFlag = int.Parse(data["api_combined_type"]);
					break;

				case "api_req_practice/battle":
					{
						int fleetID = int.Parse(data["api_deck_id"]);
                        this.Fleets[fleetID].IsInSortie = true;
					}
					break;

				default:
					foreach (int i in this.Fleets.Keys)
                        this.Fleets[i].LoadFromRequest(apiname, data);
					break;

			}

		}


		/// <summary>
		/// 泊地修理タイマを現在時刻にセットします。
		/// </summary>
		public void StartAnchorageRepairingTimer()
		{
            this.AnchorageRepairingTimer = DateTime.Now;
		}

		/// <summary>
		/// 泊地修理による回復が発生していたかをチェックし、発生していた場合は泊地修理タイマをリセットします。
		/// </summary>
		public void CheckAnchorageRepairingHealing()
		{
			foreach (var f in this.Fleets.Values)
			{
				if (this.IsAnchorageRepaired.ContainsKey(f.FleetID) && !this.IsAnchorageRepaired[f.FleetID])
					continue;

				var prev = f.Members.Select(id => this.PreviousDockingID.Contains(id) ? null : this.PreviousShips[id]).ToArray();
				var now = f.MembersInstance.ToArray();

				for (int i = 0; i < prev.Length; i++)
				{
					if (prev[i] == null || now[i] == null)
						continue;

					// 回復検知
					if (prev[i].RepairingDockID == -1 && prev[i].HPCurrent < now[i].HPCurrent)
					{
                        this.StartAnchorageRepairingTimer();

						//debug
						if (Utility.Configuration.Config.Debug.EnableDebugMenu)
							Utility.Logger.Add(1, "아카시 수리: 타이머를 재설정합니다.");
						return;
					}

				}
			}
		}


		/// <summary>
		/// 更新直前の艦船データをコピーして退避します。
		/// </summary>
		public void EvacuatePreviousShips()
		{

			if (this.Fleets.Values.Any(f => f != null && f.IsInSortie))
				return;

            this.PreviousShips = new IDDictionary<ShipData>(KCDatabase.Instance.Ships.Values);
            this.IsAnchorageRepaired = this.Fleets.ToDictionary(f => f.Key, f => f.Value.CanAnchorageRepair);
            this.PreviousDockingID = new HashSet<int>(KCDatabase.Instance.Docks.Values.Select(d => d.ShipID));
		}


		/// <summary>
		/// コンディションの更新予測パラメータを更新します。
		/// </summary>
		public void UpdateConditionPrediction()
		{

			var now = DateTime.Now;

			var conditionDiff = this.PreviousShips.Where(s => s.Value.Condition < 49)
				.Join(KCDatabase.Instance.Ships.Values, pair => pair.Key, ship => ship.ID, (pair, ship) => ship.Condition - pair.Value.Condition);
			if (!conditionDiff.Any())
			{
				goto LabelFinally;
			}

			int healed = (int)Math.Ceiling(conditionDiff.Max() / 3.0);
			int predictedHealLow = (int)Math.Floor((now - this.LastConditionUpdated).TotalSeconds / ConditionHealingSpan.TotalSeconds);


			if (healed < predictedHealLow)
			{
				goto LabelFinally;
			}

			double newPredictMin, newPredictMax;

			if (healed <= predictedHealLow)
			{
				newPredictMin = TimeSpan.FromTicks(now.Ticks % ConditionHealingSpan.Ticks).TotalSeconds;
				newPredictMax = TimeSpan.FromTicks(this.LastConditionUpdated.Ticks % ConditionHealingSpan.Ticks).TotalSeconds;
			}
			else
			{
				newPredictMin = TimeSpan.FromTicks(this.LastConditionUpdated.Ticks % ConditionHealingSpan.Ticks).TotalSeconds;
				newPredictMax = TimeSpan.FromTicks(now.Ticks % ConditionHealingSpan.Ticks).TotalSeconds;
			}

			if (newPredictMax < newPredictMin)
				newPredictMax += ConditionHealingSpan.TotalSeconds;

			double amin, amax, apre, bmin, bmax, bpre;
			if (this.ConditionPredictMin < newPredictMin)
			{
				amin = this.ConditionPredictMin;
				amax = this.ConditionPredictMax;
				apre = this.ConditionPredictMax - ConditionHealingSpan.TotalSeconds;
				bmin = newPredictMin;
				bmax = newPredictMax;
				bpre = newPredictMax - ConditionHealingSpan.TotalSeconds;
			}
			else
			{
				bmin = this.ConditionPredictMin;
				bmax = this.ConditionPredictMax;
				bpre = this.ConditionPredictMax - ConditionHealingSpan.TotalSeconds;
				amin = newPredictMin;
				amax = newPredictMax;
				apre = newPredictMax - ConditionHealingSpan.TotalSeconds;
			}

			bool startsWithAmin = amin < bpre;
			bool startsWithBmin = bmin < amax;

			bool endsWithBpre = amin < bpre && bpre < amax;
			bool endsWithAmax = (bmin < amax || amax <= bpre) && amax < bmax;
			bool endsWidthBmax = bmax < amax;

			if ((startsWithAmin && startsWithBmin) || (endsWithBpre && endsWithAmax))
			{
				// 二重領域; どちらか小さいほう
				if (amax - amin < bmax - bmin)
				{
                    this.ConditionPredictMin = amin;
                    this.ConditionPredictMax = amax;
				}
				else
				{
                    this.ConditionPredictMin = bmin;
                    this.ConditionPredictMax = bmax;
				}
			}
			else
			{
				if (startsWithAmin)
                    this.ConditionPredictMin = amin;
				else if (startsWithBmin)
                    this.ConditionPredictMin = bmin;
				else
				{
                    this.ConditionPredictMin = newPredictMin;     // 空集合; 新しいほうを設定
				}

				if (endsWithBpre)
                    this.ConditionPredictMax = bpre;
				else if (endsWithAmax)
                    this.ConditionPredictMax = amax;
				else if (endsWidthBmax)
                    this.ConditionPredictMax = bmax;
				else
				{
                    this.ConditionPredictMax = newPredictMax;     // 空集合; 新しいほうを設定
				}
			}


			LabelFinally:
            this.LastConditionUpdated = now;

			foreach (var f in this.Fleets.Values)
				f.UpdateConditionTime();

		}


		/// <summary>
		/// 指定された疲労が少なくとも回復するはずの時刻を取得します。
		/// </summary>
		/// <param name="healAmount">回復する cond 値の量(現在値からの増分)。</param>
		/// <returns></returns>
		public DateTime CalculateConditionHealingEstimation(int healAmount)
		{
			healAmount = (int)Math.Ceiling(healAmount / 3.0);

			if (healAmount <= 0)
				return DateTime.Now;

			double last = TimeSpan.FromTicks(this.LastConditionUpdated.Ticks % ConditionHealingSpan.Ticks).TotalSeconds;

			var firstHeal = TimeSpan.FromSeconds(this.ConditionBorderSeconds - last);
			var afterHeal = TimeSpan.FromSeconds(ConditionHealingSpan.TotalSeconds * (healAmount - 1));

			if (this.ConditionPredictMin <= last && last <= this.ConditionPredictMax)
				firstHeal = ConditionHealingSpan;
			if (firstHeal.Ticks <= 0)
				firstHeal += ConditionHealingSpan;

			var offset = firstHeal + afterHeal;


			return this.LastConditionUpdated + offset;

		}

	}

}
