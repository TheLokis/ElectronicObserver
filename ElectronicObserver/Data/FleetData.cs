using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 艦隊の情報を保持します。
	/// </summary>
	public class FleetData : APIWrapper, IIdentifiable
	{

		/// <summary>
		/// 艦隊ID
		/// </summary>
		public int FleetID => (int)this.RawData.api_id;

		/// <summary>
		/// 艦隊名
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// 遠征状態
		/// 0=未出撃, 1=遠征中, 2=遠征帰投, 3=強制帰投中
		/// </summary>
		public int ExpeditionState { get; internal set; }

		/// <summary>
		/// 遠征先ID
		/// </summary>
		public int ExpeditionDestination { get; internal set; }

		/// <summary>
		/// 遠征帰投時間
		/// </summary>
		public DateTime ExpeditionTime { get; internal set; }


		private int[] _members;
		/// <summary>
		/// 艦隊メンバー(艦船ID)
		/// </summary>
		public ReadOnlyCollection<int> Members => Array.AsReadOnly(this._members);

		/// <summary>
		/// 艦隊メンバー(艦船データ)
		/// </summary>
		public ReadOnlyCollection<ShipData> MembersInstance
		{
			get
			{
				if (this._members == null) return null;

				ShipData[] ships = new ShipData[this._members.Length];
				for (int i = 0; i < ships.Length; i++)
				{
					ships[i] = KCDatabase.Instance.Ships[this._members[i]];
				}

				return Array.AsReadOnly(ships);
			}
		}

		/// <summary>
		/// 艦隊メンバー(艦船データ、退避艦を除く)
		/// </summary>
		public ReadOnlyCollection<ShipData> MembersWithoutEscaped
		{
			get
			{
				if (this._members == null) return null;

				ShipData[] ships = new ShipData[this._members.Length];
				for (int i = 0; i < ships.Length; i++)
				{
					ships[i] = this._escapedShipList.Contains(this._members[i]) ? null : KCDatabase.Instance.Ships[this._members[i]];
				}

				return Array.AsReadOnly(ships);
			}
		}


		public int this[int i] => this._members[i];



		private List<int> _escapedShipList = new List<int>();
		/// <summary>
		/// 退避艦のIDリスト
		/// </summary>
		public ReadOnlyCollection<int> EscapedShipList => this._escapedShipList.AsReadOnly();

		/// <summary>
		/// 出撃中かどうか
		/// </summary>
		public bool IsInSortie { get; internal set; }




		public int ID => this.FleetID;



		public FleetData()
			: base()
		{

		}


		public override void LoadFromResponse(string apiname, dynamic data)
		{
			switch (apiname)
			{

				case "api_port/port":
					base.LoadFromResponse(apiname, (object)data);

                    this.Name = (string)this.RawData.api_name;
                    this._members = (int[])this.RawData.api_ship;
                    this.ExpeditionState = (int)this.RawData.api_mission[0];
                    this.ExpeditionDestination = (int)this.RawData.api_mission[1];
                    this.ExpeditionTime = DateTimeHelper.FromAPITime((long)this.RawData.api_mission[2]);

                    this._escapedShipList.Clear();
					if (this.IsInSortie)
					{
						Utility.Logger.Add(2, string.Format("#{0}「{1}」이 귀환했습니다.", this.FleetID, this.Name));
					}
                    this.IsInSortie = false;

					break;

				case "api_get_member/ndock":
				case "api_req_kousyou/destroyship":
				case "api_get_member/ship3":
				case "api_req_kaisou/powerup":
					break;

				default:    //checkme
					base.LoadFromResponse(apiname, (object)data);

                    this.Name = (string)this.RawData.api_name;
                    this._members = (int[])this.RawData.api_ship;
                    this.ExpeditionState = (int)this.RawData.api_mission[0];
                    this.ExpeditionDestination = (int)this.RawData.api_mission[1];
                    this.ExpeditionTime = DateTimeHelper.FromAPITime((long)this.RawData.api_mission[2]);
					break;

			}

		}


		public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
		{
			base.LoadFromRequest(apiname, data);    //checkme


			switch (apiname)
			{
				case "api_req_hensei/change":
					{
						int fleetID = int.Parse(data["api_id"]);
						int index = int.Parse(data["api_ship_idx"]);
						int shipID = int.Parse(data["api_ship_id"]);
						int replacedID = data.ContainsKey("replaced_id") ? int.Parse(data["replaced_id"]) : -1;
						int flagshipID = this._members[0];

						if (this.FleetID == fleetID)
						{
							if (shipID == -2)
							{
								//旗艦以外全解除
								for (int i = 1; i < this._members.Length; i++)
                                    this._members[i] = -1;

							}
							else if (shipID == -1)
							{
                                //はずす
                                this.RemoveShip(index);

							}
							else
							{
								//入隊

								for (int y = index - 1; y >= 0; y--)
								{       // 変更位置よりも前に空欄があれば位置をずらす
									if (this._members[y] != -1)
									{
										index = y + 1;
										break;
									}
								}

                                this._members[index] = shipID;

								//入れ替え
								for (int i = 0; i < this._members.Length; i++)
								{
									if (i != index && this._members[i] == shipID)
									{

										if (replacedID != -1)
                                            this._members[i] = replacedID;
										else
                                            this.RemoveShip(i);

										break;
									}
								}

							}


							if (shipID != -2 && this.IsFlagshipRepairShip)        //随伴艦一括解除を除く
								KCDatabase.Instance.Fleet.StartAnchorageRepairingTimer();

						}
						else
						{

							if (index != -1 && shipID != -1)
							{
								//入れ替え
								for (int i = 0; i < this._members.Length; i++)
								{
									if (this._members[i] == shipID)
									{

										if (replacedID != -1)
                                            this._members[i] = replacedID;
										else
                                            this.RemoveShip(i);

										if (this.IsFlagshipRepairShip)
											KCDatabase.Instance.Fleet.StartAnchorageRepairingTimer();

										break;
									}
								}

							}

						}


					}
					break;


				case "api_req_kousyou/destroyship":
					{
						foreach (int id in data["api_ship_id"].Split(",".ToCharArray()).Select(s => int.Parse(s)))
						{
							for (int i = 0; i < this._members.Length; i++)
							{
								if (this._members[i] == id)
								{
                                    this.RemoveShip(i);
									break;
								}
							}
						}
					}
					break;

				case "api_req_kaisou/powerup":
					{
						foreach (int id in data["api_id_items"].Split(",".ToCharArray()).Select(s => int.Parse(s)))
						{
							for (int i = 0; i < this._members.Length; i++)
							{
								if (this._members[i] == id)
								{
                                    this.RemoveShip(i);
									break;
								}
							}
						}
					}
					break;

				case "api_req_mission/start":
                    this.ExpeditionState = 1;
                    this.ExpeditionDestination = int.Parse(data["api_mission_id"]);
                    this.ExpeditionTime = DateTime.Now;  //暫定処理。実際の更新はResponseで行う

					break;


				case "api_req_member/updatedeckname":
                    this.Name = data["api_name"];
					break;

			}

		}


		/// <summary>
		/// 指定した艦娘を艦隊からはずします。
		/// </summary>
		/// <param name="index">対象艦のインデックス。0-5</param>
		private void RemoveShip(int index)
		{

			for (int i = index + 1; i < this._members.Length; i++)
                this._members[i - 1] = this._members[i];

            this._members[this._members.Length - 1] = -1;

		}



		/// <summary>
		/// 護衛退避を実行します。
		/// </summary>
		/// <param name="index">対象艦の艦隊内でのインデックス。[0-6]</param>
		public void Escape(int index)
		{
            this._escapedShipList.Add(this._members[index]);
		}


		/// <summary>
		/// 制空戦力を取得します。
		/// </summary>
		/// <returns>制空戦力。</returns>
		public int GetAirSuperiority()
		{
			switch (Utility.Configuration.Config.FormFleet.AirSuperiorityMethod)
			{
				case 0:
				default:
					return Calculator.GetAirSuperiorityIgnoreLevel(this);
				case 1:
					return Calculator.GetAirSuperiority(this);
			}
		}

		/// <summary>
		/// 現在の設定に応じて、制空戦力を表す文字列を取得します。
		/// </summary>
		/// <returns></returns>
		public string GetAirSuperiorityString()
		{
			switch (Utility.Configuration.Config.FormFleet.AirSuperiorityMethod)
			{
				case 0:
				default:
					return Calculator.GetAirSuperiorityIgnoreLevel(this).ToString();
				case 1:
					{
						int min = Calculator.GetAirSuperiority(this, false);
						int max = Calculator.GetAirSuperiority(this, true);

						if (Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange && min < max)
							return string.Format("{0} ～ {1}", min, max);
						else
							return min.ToString();
					}
			}
		}

		/// <summary>
		/// 現在の設定に応じて、索敵能力を取得します。
		/// </summary>
		public double GetSearchingAbility()
		{
			return Calculator.GetSearchingAbility_New33(this, 1);
		}

		/// <summary>
		/// 新判定式(33)索敵能力を表す文字列を取得します。
		/// </summary>
		/// <param name="branchWeight">分岐点係数。1 / 4 / 3</param>
		public string GetSearchingAbilityString(int branchWeight = 1)
		{
			return String.Format(branchWeight > 1 ? "(" + branchWeight + ") {0:f2}" : "{0:f2}",
				Math.Floor(Calculator.GetSearchingAbility_New33(this, branchWeight) * 100) / 100);
		}

		/// <summary>
		/// 触接開始率を取得します。
		/// </summary>
		/// <returns></returns>
		public double GetContactProbability()
		{
			return Calculator.GetContactProbability(this);
		}

		public Dictionary<int, double> GetContactSelectionProbability()
		{
			return Calculator.GetContactSelectionProbability(this);
		}


		/// <summary>
		/// 支援艦隊種別
		/// 0=不発, 1=空撃, 2=砲撃, 3=雷撃
		/// </summary>
		public int SupportType
		{
			get
			{
				int destroyerCount = 0;
				int aircraftCarrierCount = 0;
				int aircraftAuxiliaryCount = 0;
				int aircraftShellingCount = 0;
				int shellingCount = 0;
				int battleshipCount = 0;
				int heavyCruiserCount = 0;
				int otherCount = 0;
                int lightcarriercount = 0;
                int escortcount = 0;
                int submarineattackcount = 0;

                foreach (var s in this.MembersInstance.Where(ss => ss != null))
				{
					switch (s.MasterShip.ShipType)
					{
                        case ShipTypes.Escort:
                            submarineattackcount += 2;
                            break;

						case ShipTypes.Destroyer:
							destroyerCount++;
							break;

                        case ShipTypes.LightCruiser:
                            submarineattackcount++;
                            break;

						case ShipTypes.LightAircraftCarrier:
                            aircraftCarrierCount++;
                            lightcarriercount++;
                            break;

                        case ShipTypes.AircraftCarrier:
                        case ShipTypes.ArmoredAircraftCarrier:
							aircraftCarrierCount++;
							break;

						case ShipTypes.SeaplaneTender:
						case ShipTypes.AmphibiousAssaultShip:
							aircraftAuxiliaryCount++;
                            submarineattackcount++;
							break;

						case ShipTypes.AviationBattleship:
							aircraftShellingCount++;
							battleshipCount++;
							break;

						case ShipTypes.AviationCruiser:
							aircraftShellingCount++;
							heavyCruiserCount++;
							break;

						case ShipTypes.Transport:
							aircraftShellingCount++;
                            submarineattackcount++;
                            break;
                        
						case ShipTypes.Battleship:
						case ShipTypes.Battlecruiser:
							shellingCount++;
							battleshipCount++;
							break;

						case ShipTypes.HeavyCruiser:
							shellingCount++;
							heavyCruiserCount++;
							break;

						default:
							otherCount++;
							break;
					}

				}


				if (destroyerCount < 2)
					return 0;       // 発生しない

				if (shellingCount == 0)
				{
                    if (aircraftCarrierCount >= 1 ||
                        aircraftAuxiliaryCount >= 2 ||
                        aircraftShellingCount >= 2)
                    {
                        if(lightcarriercount >= 1)
                        {
                            if (lightcarriercount >= 2 || submarineattackcount >= 1)
                                return 4;
                        }

                        return 1;   // 空撃
                    }
                }

				if (shellingCount >= 1)
				{
                    if (aircraftCarrierCount + aircraftAuxiliaryCount >= 2 || aircraftShellingCount >= 3)
                    {
                        if (lightcarriercount >= 1)
                        {
                            if (lightcarriercount >= 2 || submarineattackcount >= 1)
                                return 4;
                        }

                        return 1;   // 空撃
                    }
				}

				if (battleshipCount >= 2 ||
					(battleshipCount == 1 && heavyCruiserCount >= 3) ||
					heavyCruiserCount >= 4)
					return 2;       // 砲撃

				return 3;           // 雷撃
			}
		}


		/// <summary>
		/// 旗艦が工作艦か
		/// </summary>
		public bool IsFlagshipRepairShip
		{
			get
			{
				ShipData flagship = KCDatabase.Instance.Ships[this._members[0]];
				return flagship != null && flagship.MasterShip.ShipType == ShipTypes.RepairShip;
			}
		}

		/// <summary>
		/// 泊地修理が発動可能か
		/// </summary>
		public bool CanAnchorageRepair
		{
			get
			{
				// 流石に資源チェックまではしない
				var flagship = KCDatabase.Instance.Ships[this._members[0]];

				return this.IsFlagshipRepairShip &&
					flagship.HPRate > 0.5 &&
					flagship.RepairingDockID == -1 &&
                    this.ExpeditionState == 0 &&
                    this.MembersInstance.Take(2 + flagship.SlotInstance.Count(eq => eq != null && eq.MasterEquipment.CategoryType == EquipmentTypes.RepairFacility))
					.Any(ship => ship != null && 0.5 < ship.HPRate && ship.HPRate < 1.0 && ship.RepairingDockID == -1);
			}
		}


		/// <summary>
		/// 疲労が回復すると予測される日時 (疲労していない場合は null)
		/// </summary>
		public DateTime? ConditionTime { get; private set; }

		public void UpdateConditionTime()
		{
			var ships = this.MembersInstance.Where(ship => ship != null && ship.Condition < Utility.Configuration.Config.Control.ConditionBorder);
			if (!ships.Any())
			{
                this.ConditionTime = null;

			}
			else
			{
                this.ConditionTime = KCDatabase.Instance.Fleet.CalculateConditionHealingEstimation(Utility.Configuration.Config.Control.ConditionBorder - ships.Min(ship => ship.Condition));
			}
		}


		public override string ToString() => $"[{this.FleetID}] {this.Name}";

	}

}
