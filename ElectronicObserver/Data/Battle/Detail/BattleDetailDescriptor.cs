using ElectronicObserver.Data.Battle.Phase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Detail
{

	public static class BattleDetailDescriptor
	{

		public static string GetBattleDetail(BattleManager bm)
		{
			var sb = new StringBuilder();

			if (bm.IsPractice)
			{
				sb.AppendLine("연습");

			}
			else
			{
				sb.AppendFormat("{0} ({1}-{2})", bm.Compass.MapInfo.Name, bm.Compass.MapAreaID, bm.Compass.MapInfoID);
				if (bm.Compass.MapInfo.EventDifficulty > 0)
					sb.AppendFormat(" [{0}]", Constants.GetDifficulty(bm.Compass.MapInfo.EventDifficulty));
				sb.Append(" 노드: ").Append(bm.Compass.Destination.ToString());
				if (bm.Compass.EventID == 5)
					sb.Append(" (보스)");
				sb.AppendLine();

				var mapinfo = bm.Compass.MapInfo;
				if (!mapinfo.IsCleared)
				{
					if (mapinfo.RequiredDefeatedCount != -1)
					{
						sb.AppendFormat("격파: {0} / {1} 회", mapinfo.CurrentDefeatedCount, mapinfo.RequiredDefeatedCount)
							.AppendLine();
					}
					else if (mapinfo.MapHPMax > 0)
					{
						int current = bm.Compass.MapHPCurrent > 0 ? bm.Compass.MapHPCurrent : mapinfo.MapHPCurrent;
						int max = bm.Compass.MapHPMax > 0 ? bm.Compass.MapHPMax : mapinfo.MapHPMax;
						sb.AppendFormat("{0}{1}: {2} / {3}", mapinfo.CurrentGaugeIndex > 0 ? "#" + mapinfo.CurrentGaugeIndex + " " : "", mapinfo.GaugeType == 3 ? "TP" : "HP", current, max)
							.AppendLine();
					}
				}
			}
			if (bm.Result != null)
			{
				sb.AppendLine(bm.Result.EnemyFleetName);
			}
			sb.AppendLine();


			sb.AppendFormat("◆ {0} ◆\r\n", bm.FirstBattle.BattleName).AppendLine(GetBattleDetail(bm.FirstBattle));
			if (bm.SecondBattle != null)
				sb.AppendFormat("◆ {0} ◆\r\n", bm.SecondBattle.BattleName).AppendLine(GetBattleDetail(bm.SecondBattle));


			if (bm.Result != null)
			{
				sb.AppendLine(GetBattleResult(bm));
			}

			return sb.ToString();
		}


		public static string GetBattleDetail(BattleData battle)
		{

			var sbmaster = new StringBuilder();
			bool isBaseAirRaid = battle.IsBaseAirRaid;


			foreach (var phase in battle.GetPhases())
			{

				var sb = new StringBuilder();

				switch (phase)
				{
					case PhaseBaseAirRaid p:

						sb.AppendLine("아군기지항공대 참여 부대:");
						sb.Append("　").AppendLine(string.Join(", ", p.Squadrons.Where(sq => sq.EquipmentInstance != null).Select(sq => sq.ToString()).DefaultIfEmpty("(なし)")));

						GetBattleDetailPhaseAirBattle(sb, p);

						break;

					case PhaseAirBattle p:

						GetBattleDetailPhaseAirBattle(sb, p);

						break;

					case PhaseBaseAirAttack p:

						foreach (var a in p.AirAttackUnits)
						{
							sb.AppendFormat("〈제{0}격〉\r\n", a.AirAttackIndex + 1);

							sb.AppendLine("아군기지항공대 참여 부대:");
							sb.Append("　").AppendLine(string.Join(", ", a.Squadrons.Where(sq => sq.EquipmentInstance != null).Select(sq => sq.ToString())));

							GetBattleDetailPhaseAirBattle(sb, a);
							sb.Append(a.GetBattleDetail());
						}

						break;

					case PhaseJetAirBattle p:
						GetBattleDetailPhaseAirBattle(sb, p);

						break;

					case PhaseJetBaseAirAttack p:

						foreach (var a in p.AirAttackUnits)
						{
							sb.AppendFormat("〈제{0}격〉\r\n", a.AirAttackIndex + 1);

							sb.AppendLine("아군기지항공대 참여 부대:");
							sb.Append("　").AppendLine(string.Join(", ", a.Squadrons.Where(sq => sq.EquipmentInstance != null).Select(sq => sq.ToString())));

							GetBattleDetailPhaseAirBattle(sb, a);
							sb.Append(a.GetBattleDetail());
						}

						break;

					case PhaseInitial p:


						if (p.FriendFleetEscort != null)
							sb.AppendLine("〈아군기함〉");
						else
							sb.AppendLine("〈아군함대〉");

						if (isBaseAirRaid)
							OutputFriendBase(sb, p.FriendInitialHPs, p.FriendMaxHPs);
						else
							OutputFriendData(sb, p.FriendFleet, p.FriendInitialHPs, p.FriendMaxHPs);

						if (p.FriendFleetEscort != null)
						{
							sb.AppendLine();
							sb.AppendLine("〈아군수반함〉");

							OutputFriendData(sb, p.FriendFleetEscort, p.FriendInitialHPsEscort, p.FriendMaxHPsEscort);
						}

						sb.AppendLine();

						if (p.EnemyMembersEscort != null)
							sb.Append("〈적기함〉");
						else
							sb.Append("〈적함대〉");

						if (p.IsBossDamaged)
							sb.Append(" : 장갑파괴");
						sb.AppendLine();

						OutputEnemyData(sb, p.EnemyMembersInstance, p.EnemyLevels, p.EnemyInitialHPs, p.EnemyMaxHPs, p.EnemySlotsInstance, p.EnemyParameters);


						if (p.EnemyMembersEscort != null)
						{
							sb.AppendLine();
							sb.AppendLine("〈적수반함〉");

							OutputEnemyData(sb, p.EnemyMembersEscortInstance, p.EnemyLevelsEscort, p.EnemyInitialHPsEscort, p.EnemyMaxHPsEscort, p.EnemySlotsEscortInstance, p.EnemyParametersEscort);
						}

						sb.AppendLine();

						if (battle.GetPhases().Where(ph => ph is PhaseBaseAirAttack || ph is PhaseBaseAirRaid).Any(ph => ph != null && ph.IsAvailable))
						{
							sb.AppendLine("〈기지항공대〉");
							GetBattleDetailBaseAirCorps(sb, KCDatabase.Instance.Battle.Compass.MapAreaID);      // :(
							sb.AppendLine();
						}

						if (p.RationIndexes.Length > 0)
						{
							sb.AppendLine("〈전투식량보급〉");
							foreach (var index in p.RationIndexes)
							{
								var ship = p.GetFriendShip(index);

								if (ship != null)
								{
									sb.AppendFormat("　{0} #{1}\r\n", ship.NameWithLevel, index + 1);
								}
							}
							sb.AppendLine();
						}

						break;

					case PhaseNightInitial p:

						{
							var eq = KCDatabase.Instance.MasterEquipments[p.TouchAircraftFriend];
							if (eq != null)
							{
								sb.Append("아군야간촉접: ").AppendLine(eq.Name);
							}
							eq = KCDatabase.Instance.MasterEquipments[p.TouchAircraftEnemy];
							if (eq != null)
							{
								sb.Append("적군야간촉접: ").AppendLine(eq.Name);
							}
						}

						{
							int searchlightIndex = p.SearchlightIndexFriend;
							if (searchlightIndex != -1)
							{
								sb.AppendFormat("아군탐조등: {0} #{1}\r\n", p.FriendFleet.MembersInstance[searchlightIndex].Name, searchlightIndex + 1);
							}
							searchlightIndex = p.SearchlightIndexEnemy;
							if (searchlightIndex != -1)
							{
								sb.AppendFormat("적탐조등: {0} #{1}\r\n", p.EnemyMembersInstance[searchlightIndex].NameWithClass, searchlightIndex + 1);
							}
						}

						if (p.FlareIndexFriend != -1)
						{
							sb.AppendFormat("아군조명탄사용: {0} #{1}\r\n", p.FlareFriendInstance.NameWithLevel, p.FlareIndexFriend + 1);
						}
						if (p.FlareIndexEnemy != -1)
						{
							sb.AppendFormat("적조명탄사용: {0} #{1}\r\n", p.FlareEnemyInstance.NameWithClass, p.FlareIndexEnemy + 1);
						}

						sb.AppendLine();
						break;


					case PhaseSearching p:
						sb.Append("아군진형: ").Append(Constants.GetFormation(p.FormationFriend));
						sb.Append(" / 적군진형: ").AppendLine(Constants.GetFormation(p.FormationEnemy));
						sb.Append("교전진형: ").AppendLine(Constants.GetEngagementForm(p.EngagementForm));
						sb.Append("아군색적: ").Append(Constants.GetSearchingResult(p.SearchingFriend));
						sb.Append(" / 적군색적: ").AppendLine(Constants.GetSearchingResult(p.SearchingEnemy));

						sb.AppendLine();

						break;

					case PhaseSupport p:
						if (p.IsAvailable)
						{
							sb.AppendLine("〈지원함대〉");
							OutputSupportData(sb, p.SupportFleet);
							sb.AppendLine();
						}
						break;

					case PhaseFriendlySupport p:
						if (p.IsAvailable)
						{
							sb.AppendLine("〈우군함대〉");
							OutputFriendlySupportData(sb, p);
							sb.AppendLine();

							{
								int searchlightIndex = p.SearchlightIndexFriend;
								if (searchlightIndex != -1)
								{
									sb.AppendFormat("아군탐조등: {0} #{1}\r\n", p.SearchlightFriendInstance.NameWithClass, searchlightIndex + 1);
								}
								searchlightIndex = p.SearchlightIndexEnemy;
								if (searchlightIndex != -1)
								{
									sb.AppendFormat("적탐조등: {0} #{1}\r\n", p.SearchlightEnemyInstance.NameWithClass, searchlightIndex + 1);
								}
							}

							{
								int flareIndex = p.FlareIndexFriend;
								if (flareIndex != -1)
								{
									sb.AppendFormat("아군조명탄사용: {0} #{1}\r\n", p.FlareFriendInstance.NameWithClass, flareIndex + 1);
								}
								flareIndex = p.FlareIndexEnemy;
								if (flareIndex != -1)
								{
									sb.AppendFormat("적조명탄사용: {0} #{1}\r\n", p.FlareEnemyInstance.NameWithClass, flareIndex + 1);
								}
							}

							sb.AppendLine();
						}
						break;

				}


				if (!(phase is PhaseBaseAirAttack || phase is PhaseJetBaseAirAttack))       // 通常出力と重複するため
					sb.Append(phase.GetBattleDetail());

				if (sb.Length > 0)
				{
					sbmaster.AppendFormat("《{0}》\r\n", phase.Title).Append(sb);
				}
			}


			{
				sbmaster.AppendLine("《전투종료》");

				var friend = battle.Initial.FriendFleet;
				var friendescort = battle.Initial.FriendFleetEscort;
				var enemy = battle.Initial.EnemyMembersInstance;
				var enemyescort = battle.Initial.EnemyMembersEscortInstance;

				if (friendescort != null)
					sbmaster.AppendLine("〈아군기함〉");
				else
					sbmaster.AppendLine("〈아군함대〉");

				if (isBaseAirRaid)
				{

					for (int i = 0; i < 6; i++)
					{
						if (battle.Initial.FriendMaxHPs[i] <= 0)
							continue;

						OutputResultData(sbmaster, i, string.Format("제{0}기지", i + 1),
							battle.Initial.FriendInitialHPs[i], battle.ResultHPs[i], battle.Initial.FriendMaxHPs[i]);
					}

				}
				else
				{
					for (int i = 0; i < friend.Members.Count(); i++)
					{
						var ship = friend.MembersInstance[i];
						if (ship == null)
							continue;

						OutputResultData(sbmaster, i, ship.Name,
							battle.Initial.FriendInitialHPs[i], battle.ResultHPs[i], battle.Initial.FriendMaxHPs[i]);
					}
				}

				if (friendescort != null)
				{
					sbmaster.AppendLine().AppendLine("〈아군수반함〉");

					for (int i = 0; i < friendescort.Members.Count(); i++)
					{
						var ship = friendescort.MembersInstance[i];
						if (ship == null)
							continue;

						OutputResultData(sbmaster, i + 6, ship.Name,
							battle.Initial.FriendInitialHPsEscort[i], battle.ResultHPs[i + 6], battle.Initial.FriendMaxHPsEscort[i]);
					}

				}


				sbmaster.AppendLine();
				if (enemyescort != null)
					sbmaster.AppendLine("〈적기함〉");
				else
					sbmaster.AppendLine("〈적함대〉");

				for (int i = 0; i < enemy.Length; i++)
				{
					var ship = enemy[i];
					if (ship == null)
						continue;

					OutputResultData(sbmaster, i,
						ship.NameWithClass,
						battle.Initial.EnemyInitialHPs[i], battle.ResultHPs[i + 12], battle.Initial.EnemyMaxHPs[i]);
				}

				if (enemyescort != null)
				{
					sbmaster.AppendLine().AppendLine("〈적수반함〉");

					for (int i = 0; i < enemyescort.Length; i++)
					{
						var ship = enemyescort[i];
						if (ship == null)
							continue;

						OutputResultData(sbmaster, i + 6, ship.NameWithClass,
							battle.Initial.EnemyInitialHPsEscort[i], battle.ResultHPs[i + 18], battle.Initial.EnemyMaxHPsEscort[i]);
					}
				}

				sbmaster.AppendLine();
			}

			return sbmaster.ToString();
		}


		private static void GetBattleDetailBaseAirCorps(StringBuilder sb, int mapAreaID)
		{
			foreach (var corps in KCDatabase.Instance.BaseAirCorps.Values.Where(corps => corps.MapAreaID == mapAreaID))
			{
				sb.AppendFormat("{0} [{1}]\r\n　{2}\r\n",
					corps.Name, Constants.GetBaseAirCorpsActionKind(corps.ActionKind),
					string.Join(", ", corps.Squadrons.Values
						.Where(sq => sq.State == 1 && sq.EquipmentInstance != null)
						.Select(sq => sq.EquipmentInstance.NameWithLevel)));
			}
		}

		private static void GetBattleDetailPhaseAirBattle(StringBuilder sb, PhaseAirBattleBase p)
		{

			if (p.IsStage1Available)
			{
				sb.Append("Stage1: ").AppendLine(Constants.GetAirSuperiority(p.AirSuperiority));
				sb.AppendFormat("　아군: -{0}/{1}\r\n　적 : -{2}/{3}\r\n",
					p.AircraftLostStage1Friend, p.AircraftTotalStage1Friend,
					p.AircraftLostStage1Enemy, p.AircraftTotalStage1Enemy);
				if (p.TouchAircraftFriend > 0)
					sb.AppendFormat("　아군촉접: {0}\r\n", KCDatabase.Instance.MasterEquipments[p.TouchAircraftFriend].Name);
				if (p.TouchAircraftEnemy > 0)
					sb.AppendFormat("　적 촉접: {0}\r\n", KCDatabase.Instance.MasterEquipments[p.TouchAircraftEnemy].Name);
			}
			if (p.IsStage2Available)
			{
				sb.Append("Stage2: ");
				if (p.IsAACutinAvailable)
				{
					sb.AppendFormat("대공컷인( {0}, {1}({2}) )", p.AACutInShip.NameWithLevel, Constants.GetAACutinKind(p.AACutInKind), p.AACutInKind);
				}
				sb.AppendLine();
				sb.AppendFormat("　아군: -{0}/{1}\r\n　적 : -{2}/{3}\r\n",
					p.AircraftLostStage2Friend, p.AircraftTotalStage2Friend,
					p.AircraftLostStage2Enemy, p.AircraftTotalStage2Enemy);
			}

			if (p.IsStage1Available || p.IsStage2Available)
				sb.AppendLine();
		}


		private static void OutputFriendData(StringBuilder sb, FleetData fleet, int[] initialHPs, int[] maxHPs)
		{

			for (int i = 0; i < fleet.MembersInstance.Count; i++)
			{
				var ship = fleet.MembersInstance[i];

				if (ship == null)
					continue;

				sb.AppendFormat("#{0}: {1} {2} HP: {3} / {4} - 화력{5}, 뇌장{6}, 대공{7}, 장갑{8}{9}\r\n",
					i + 1,
					ship.MasterShip.ShipTypeName, ship.NameWithLevel,
					initialHPs[i], maxHPs[i],
					ship.FirepowerBase, ship.TorpedoBase, ship.AABase, ship.ArmorBase,
					fleet.EscapedShipList.Contains(ship.MasterID) ? " (대피중)" : "");

				sb.Append("　");
				sb.AppendLine(string.Join(", ", ship.AllSlotInstance.Where(eq => eq != null)));
			}
		}

		private static void OutputFriendBase(StringBuilder sb, int[] initialHPs, int[] maxHPs)
		{

			for (int i = 0; i < initialHPs.Length; i++)
			{
				if (maxHPs[i] <= 0)
					continue;

				sb.AppendFormat("#{0}: 육상기지 제{1}기지 HP: {2} / {3}\r\n\r\n",
					i + 1,
					i + 1,
					initialHPs[i], maxHPs[i]);
			}

		}

		public static void OutputSupportData(StringBuilder sb, FleetData fleet)
		{

			for (int i = 0; i < fleet.MembersInstance.Count; i++)
			{
				var ship = fleet.MembersInstance[i];

				if (ship == null)
					continue;

				sb.AppendFormat("#{0}: {1} {2} - 화력{3}, 뇌장{4}, 대공{5}, 장갑{6}\r\n",
					i + 1,
					ship.MasterShip.ShipTypeName, ship.NameWithLevel,
					ship.FirepowerBase, ship.TorpedoBase, ship.AABase, ship.ArmorBase);

				sb.Append("　");
				sb.AppendLine(string.Join(", ", ship.AllSlotInstance.Where(eq => eq != null)));
			}

		}

		private static void OutputFriendlySupportData(StringBuilder sb, PhaseFriendlySupport p)
		{

			for (int i = 0; i < p.FriendlyMembersInstance.Length; i++)
			{
				var ship = p.FriendlyMembersInstance[i];

				if (ship == null)
					continue;

				sb.AppendFormat("#{0}: {1} {2} Lv. {3} HP: {4} / {5} - 화력{6}, 뇌장{7}, 대공{8}, 장갑{9}\r\n",
					i + 1,
					ship.ShipTypeName, p.FriendlyMembersInstance[i].NameWithClass, p.FriendlyLevels[i],
					p.FriendlyInitialHPs[i], p.FriendlyMaxHPs[i],
					p.FriendlyParameters[i][0], p.FriendlyParameters[i][1], p.FriendlyParameters[i][2], p.FriendlyParameters[i][3]);

				sb.Append("　");
				sb.AppendLine(string.Join(", ", p.FriendlySlots[i].Select(id => KCDatabase.Instance.MasterEquipments[id]).Where(eq => eq != null).Select(eq => eq.Name)));
			}
		}

		private static void OutputEnemyData(StringBuilder sb, ShipDataMaster[] members, int[] levels, int[] initialHPs, int[] maxHPs, EquipmentDataMaster[][] slots, int[][] parameters)
		{

			for (int i = 0; i < members.Length; i++)
			{
				if (members[i] == null)
					continue;

				sb.AppendFormat("#{0}: ID:{1} {2} {3} Lv. {4} HP: {5} / {6}",
					i + 1,
					members[i].ShipID,
					members[i].ShipTypeName, members[i].NameWithClass,
					levels[i],
					initialHPs[i], maxHPs[i]);

				if (parameters != null)
				{
					sb.AppendFormat(" - 화력{0}, 뇌장{1}, 대공{2}, 장갑{3}",
					parameters[i][0], parameters[i][1], parameters[i][2], parameters[i][3]);
				}

				sb.AppendLine().Append("　");
				sb.AppendLine(string.Join(", ", slots[i].Where(eq => eq != null)));
			}
		}


		private static void OutputResultData(StringBuilder sb, int index, string name, int initialHP, int resultHP, int maxHP)
		{
			sb.AppendFormat("#{0}: {1} HP: ({2} → {3})/{4} ({5})\r\n",
				index + 1, name,
				Math.Max(initialHP, 0),
				Math.Max(resultHP, 0),
				Math.Max(maxHP, 0),
				resultHP - initialHP);
		}


		private static string GetBattleResult(BattleManager bm)
		{
			var result = bm.Result;

			var sb = new StringBuilder();


			sb.AppendLine("◆ 전투결과 ◆");
			sb.AppendFormat("랭크: {0}\r\n", result.Rank);

			if (bm.IsCombinedBattle)
			{
				sb.AppendFormat("MVP(기함): {0}\r\n",
					result.MVPIndex == -1 ? "(없음)" : bm.FirstBattle.Initial.FriendFleet.MembersInstance[result.MVPIndex - 1].NameWithLevel);
				sb.AppendFormat("MVP(수반함): {0}\r\n",
					result.MVPIndexCombined == -1 ? "(없음)" : bm.FirstBattle.Initial.FriendFleetEscort.MembersInstance[result.MVPIndexCombined - 1].NameWithLevel);

			}
			else
			{
				sb.AppendFormat("MVP: {0}\r\n",
					result.MVPIndex == -1 ? "(없음)" : bm.FirstBattle.Initial.FriendFleet.MembersInstance[result.MVPIndex - 1].NameWithLevel);
			}

			sb.AppendFormat("제독경험치: +{0}\r\n함선기본경험치: +{1}\r\n",
				result.AdmiralExp, result.BaseExp);


			if (!bm.IsPractice)
			{
				sb.AppendLine().AppendLine("드롭：");


				int length = sb.Length;

				var ship = KCDatabase.Instance.MasterShips[result.DroppedShipID];
				if (ship != null)
				{
					sb.AppendFormat("　{0} {1}\r\n", ship.ShipTypeName, ship.NameWithClass);
				}

				var eq = KCDatabase.Instance.MasterEquipments[result.DroppedEquipmentID];
				if (eq != null)
				{
					sb.AppendFormat("　{0} {1}\r\n", eq.CategoryTypeInstance.Name, eq.Name);
				}

				var item = KCDatabase.Instance.MasterUseItems[result.DroppedItemID];
				if (item != null)
				{
					sb.Append("　").AppendLine(item.Name);
				}

				if (length == sb.Length)
				{
					sb.AppendLine("　(없음)");
				}
			}


			return sb.ToString();
		}

	}
}
