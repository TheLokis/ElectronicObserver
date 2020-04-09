using ElectronicObserver.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle.Detail
{
	/// <summary>
	/// 戦闘詳細のデータを保持します。
	/// </summary>
	public abstract class BattleDetail
	{

		public double[] RawDamages { get; protected set; }
		public int[] Damages { get; protected set; }
		public bool[] GuardsFlagship { get; protected set; }
		public CriticalType[] CriticalTypes { get; protected set; }
		public int AttackType { get; protected set; }
		public int[] EquipmentIDs { get; protected set; }
		public int DefenderHP { get; protected set; }

		public ShipDataMaster Attacker { get; protected set; }
		public ShipDataMaster Defender { get; protected set; }


		/// <summary> 攻撃側インデックス </summary>
		public BattleIndex AttackerIndex { get; protected set; }

		/// <summary> 防御側インデックス </summary>
		public BattleIndex DefenderIndex { get; protected set; }


		protected readonly BattleData Battle;


		public enum CriticalType
		{
			Miss = 0,
			Hit = 1,
			Critical = 2,
			Invalid = -1
		}


		/// <param name="bd">戦闘情報。</param>
		/// <param name="attackerIndex">攻撃側のインデックス。</param>
		/// <param name="defenderIndex">防御側のインデックス。</param>
		/// <param name="damages">ダメージの配列。</param>
		/// <param name="criticalTypes">命中判定の配列。</param>
		/// <param name="attackType">攻撃種別。</param>
		/// <param name="defenderHP">防御側の攻撃を受ける直前のHP。</param>
		public BattleDetail(BattleData bd, BattleIndex attackerIndex, BattleIndex defenderIndex, double[] damages, int[] criticalTypes, int attackType, int[] equipmentIDs, int defenderHP)
		{
            this.Battle = bd;

            this.AttackerIndex = attackerIndex;
            this.DefenderIndex = defenderIndex;
            this.RawDamages = damages;
            this.Damages = damages.Select(dmg => (int)dmg).ToArray();
            this.GuardsFlagship = damages.Select(dmg => dmg != Math.Floor(dmg)).ToArray();
            this.CriticalTypes = criticalTypes.Select(i => (CriticalType)i).ToArray();
            this.AttackType = attackType;
            this.EquipmentIDs = equipmentIDs;
            this.DefenderHP = defenderHP;

		}


		protected int[] SetAttacker()
		{
			if (this.AttackerIndex < 0)
			{
                this.Attacker = null;
				return null;
			}
			else
			{
				switch (this.AttackerIndex.Side)
				{
					case BattleSides.FriendMain:
						{
							var atk = this.Battle.Initial.FriendFleet.MembersInstance[this.AttackerIndex.Index];
                            this.Attacker = atk.MasterShip;
							return atk.AllSlotMaster.ToArray();
						}

					case BattleSides.FriendEscort:
						{
							var atk = this.Battle.Initial.FriendFleetEscort.MembersInstance[this.AttackerIndex.Index];
                            this.Attacker = atk.MasterShip;
							return atk.AllSlotMaster.ToArray();
						}

					case BattleSides.EnemyMain:
                        this.Attacker = this.Battle.Initial.EnemyMembersInstance[this.AttackerIndex.Index];
						return this.Battle.Initial.EnemySlots[this.AttackerIndex.Index];

					case BattleSides.EnemyEscort:
                        this.Attacker = this.Battle.Initial.EnemyMembersEscortInstance[this.AttackerIndex.Index];
						return this.Battle.Initial.EnemySlotsEscort[this.AttackerIndex.Index];

					default:
						throw new NotSupportedException();
				}
			}
		}

		protected void SetDefender()
		{
			if (this.Battle.IsBaseAirRaid)
			{
                this.Defender = null;
			}
			else
			{
				switch (this.DefenderIndex.Side)
				{
					case BattleSides.FriendMain:
                        this.Defender = this.Battle.Initial.FriendFleet.MembersInstance[this.DefenderIndex.Index].MasterShip;
						break;

					case BattleSides.FriendEscort:
                        this.Defender = this.Battle.Initial.FriendFleetEscort.MembersInstance[this.DefenderIndex.Index].MasterShip;
						break;

					case BattleSides.EnemyMain:
                        this.Defender = this.Battle.Initial.EnemyMembersInstance[this.DefenderIndex.Index];
						break;

					case BattleSides.EnemyEscort:
                        this.Defender = this.Battle.Initial.EnemyMembersEscortInstance[this.DefenderIndex.Index];
						break;
				}
			}

		}



		/// <summary>
		/// 戦闘詳細の情報を出力します。
		/// </summary>
		public override string ToString()
		{

			StringBuilder builder = new StringBuilder();


			if (this.Battle.IsPractice)
				builder.AppendFormat("{0}{1} → {2}{3}",
                    this.Attacker == null ? "" : this.AttackerIndex.IsFriend ? "아군 " : "적 ", this.GetAttackerName(),
                    this.DefenderIndex.IsFriend ? "아군 " : "적  ", this.GetDefenderName()
					).AppendLine();
			else
				builder.AppendFormat("{0} → {1}", this.GetAttackerName(), this.GetDefenderName()).AppendLine();


			if (this.AttackType >= 0)
				builder.Append("[").Append(this.GetAttackKind()).Append("] ");

			/*// 
			if ( EquipmentIDs != null ) {
				var eqs = EquipmentIDs.Select( id => KCDatabase.Instance.MasterEquipments[id] ).Where( eq => eq != null ).Select( eq => eq.Name );
				if ( eqs.Any() )
					builder.Append( "(" ).Append( string.Join( ", ", eqs ) ).Append( ") " );
			}
			//*/

			for (int i = 0; i < this.Damages.Length; i++)
			{
				if (this.CriticalTypes[i] == CriticalType.Invalid)   // カットイン(主砲/主砲)、カットイン(主砲/副砲)時に発生する
					continue;

				if (i > 0)
					builder.Append(" , ");

				if (this.GuardsFlagship[i])
					builder.Append("<기함보호> ");

				switch (this.CriticalTypes[i])
				{
					case CriticalType.Miss:
						builder.Append("Miss");
						break;
					case CriticalType.Hit:
						builder.Append(this.Damages[i]).Append(" Dmg");
						break;
					case CriticalType.Critical:
						builder.Append(this.Damages[i]).Append(" Critical!");
						break;
				}

			}


			int beforeHP = Math.Max(this.DefenderHP, 0);
			int afterHP = Math.Max(this.DefenderHP - this.Damages.Sum(), 0);
			if (beforeHP != afterHP)
				builder.AppendFormat(" ( {0} → {1} )", beforeHP, afterHP);



			builder.AppendLine();


			// damage control
			if (beforeHP > 0 && afterHP <= 0 && this.DefenderIndex.IsFriend && !this.Battle.IsPractice && !this.Battle.IsBaseAirRaid)
			{
				// 友軍艦隊時は常に beforeHP == 0 になるので、ここには来ないはず
				// 暫定対策でしかないのでできればまともにしたい

				var defender = (this.DefenderIndex.Side == BattleSides.FriendEscort ? this.Battle.Initial.FriendFleetEscort : this.Battle.Initial.FriendFleet)
					?.MembersInstance?.ElementAtOrDefault(this.DefenderIndex.Index);
				if (defender != null)
				{
					int id = defender.DamageControlID;

					if (id == 42)
						builder.AppendFormat("　응급 수리 요원 발동　HP{0}", (int)(defender.HPMax * 0.2)).AppendLine();

					else if (id == 43)
						builder.AppendFormat("　응급 수리 여신 발동　HP{0}", defender.HPMax).AppendLine();

				}
			}
			return builder.ToString();
		}


		protected virtual string GetAttackerName()
		{
			int index = this.AttackerIndex.Index + 1 + (this.AttackerIndex.IsEscort ? 6 : 0);

			if (this.Attacker == null)
				return "#" + index;

			return this.Attacker.NameWithClass + " #" + index;
		}

		protected virtual string GetDefenderName()
		{
			int index = this.DefenderIndex.Index + 1 + (this.DefenderIndex.IsEscort ? 6 : 0);

			if (this.Defender == null)
				return "#" + index;
			return this.Defender.NameWithClass + " #" + index;
		}

		protected abstract int CaclulateAttackKind(int[] slots, int attackerShipID, int defenderShipID);
		protected abstract string GetAttackKind();

	}


	/// <summary>
	/// 昼戦の戦闘詳細データを保持します。
	/// </summary>
	public class BattleDayDetail : BattleDetail
	{

		public BattleDayDetail(BattleData bd, BattleIndex attackerId, BattleIndex defenderId, double[] damages, int[] criticalTypes, int attackType, int[] equipmentIDs, int defenderHP)
			: base(bd, attackerId, defenderId, damages, criticalTypes, attackType, equipmentIDs, defenderHP)
		{
			var attackerSlots = this.SetAttacker();
            this.SetDefender();

			if (this.AttackType == 0 && this.Attacker != null)
                this.AttackType = this.CaclulateAttackKind(attackerSlots, this.Attacker.ShipID, this.Defender.ShipID);

		}

		protected override int CaclulateAttackKind(int[] slots, int attackerShipID, int defenderShipID)
		{
			return (int)Calculator.GetDayAttackKind(slots, attackerShipID, defenderShipID, false);
		}

		protected override string GetAttackKind()
		{
			return Constants.GetDayAttackKind((DayAttackKind)this.AttackType);
		}
	}

	/// <summary>
	/// 支援攻撃の戦闘詳細データを保持します。
	/// </summary>
	public class BattleSupportDetail : BattleDetail
	{

		public BattleSupportDetail(BattleData bd, BattleIndex defenderId, double damage, int criticalType, int attackType, int defenderHP)
			: base(bd, BattleIndex.Invalid, defenderId, new double[] { damage }, new int[] { criticalType }, attackType, null, defenderHP)
		{
			var attackerSlots = this.SetAttacker();
            this.SetDefender();

			if (this.AttackType == 0 && this.Attacker != null)
                this.AttackType = this.CaclulateAttackKind(attackerSlots, this.Attacker.ShipID, this.Defender.ShipID);
		}

		protected override string GetAttackerName()
		{
			return "지원함대";
		}

		protected override int CaclulateAttackKind(int[] slots, int attackerShipID, int defenderShipID)
		{
			return -1;
		}

		protected override string GetAttackKind()
		{
			switch (this.AttackType)
			{
				case 1:
					return "공습";
				case 2:
					return "포격";
				case 3:
					return "뇌격";
				case 4:
					return "폭격";
				default:
					return "불명";
			}
		}

	}

	/// <summary>
	/// 夜戦における戦闘詳細データを保持します。
	/// </summary>
	public class BattleNightDetail : BattleDetail
	{

		public bool NightAirAttackFlag { get; protected set; }

		public BattleNightDetail(BattleData bd, BattleIndex attackerId, BattleIndex defenderId, double[] damages, int[] criticalTypes, int attackType, int[] equipmentIDs, bool nightAirAttackFlag, int defenderHP)
			: base(bd, attackerId, defenderId, damages, criticalTypes, attackType, equipmentIDs, defenderHP)
		{
            this.NightAirAttackFlag = nightAirAttackFlag;

			var attackerSlots = this.SetAttacker();
            this.SetDefender();

			if (this.AttackType == 0 && this.Attacker != null)
                this.AttackType = this.CaclulateAttackKind(attackerSlots, this.Attacker.ShipID, this.Defender.ShipID);
		}

		protected override int CaclulateAttackKind(int[] slots, int attackerShipID, int defenderShipID)
		{
			return (int)Calculator.GetNightAttackKind(slots, attackerShipID, defenderShipID, false, this.NightAirAttackFlag);
		}

		protected override string GetAttackKind()
		{
			return Constants.GetNightAttackKind((NightAttackKind)this.AttackType);
		}
	}

	/// <summary>
	/// 航空戦における戦闘詳細データを保持します。
	/// </summary>
	public class BattleAirDetail : BattleDayDetail
	{

		public int WaveIndex { get; protected set; }

		public BattleAirDetail(BattleData bd, int waveIndex, BattleIndex defenderId, double damage, int criticalType, int attackType, int defenderHP)
			: base(bd, BattleIndex.Invalid, defenderId, new double[] { damage }, new int[] { criticalType }, attackType, null, defenderHP)
		{
            this.WaveIndex = waveIndex;
		}

		protected override string GetAttackerName()
		{
			if (this.WaveIndex <= 0)
			{
				if (this.DefenderIndex.Side == BattleSides.FriendMain || this.DefenderIndex.Side == BattleSides.FriendEscort)
					return "적항공대";
				else
					return "아군항공대";

			}
			else
			{
				return string.Format("기지항공대 제{0}격", this.WaveIndex);

			}
		}

		protected override string GetDefenderName()
		{
			if (this.WaveIndex < 0 && this.DefenderIndex.Side == BattleSides.FriendMain)
				return string.Format("제{0}기지", this.DefenderIndex.Index + 1);

			return base.GetDefenderName();
		}

		protected override int CaclulateAttackKind(int[] slots, int attackerShipID, int defenderShipID)
		{
			return -1;
		}

		protected override string GetAttackKind()
		{
			switch (this.AttackType)
			{
				case 1:
					return "뇌격";
				case 2:
					return "폭격";
				case 3:
					return "뇌격+폭격";
				default:
					return "불명";
			}
		}

	}


	/// <summary>
	/// 友軍艦隊攻撃における戦闘詳細データを保持します。
	/// </summary>
	public class BattleFriendlySupportDetail : BattleDetail
	{

		public bool NightAirAttackFlag { get; protected set; }

		public BattleFriendlySupportDetail(BattleNight bd, BattleIndex attackerId, BattleIndex defenderId, double[] damages, int[] criticalTypes, int attackType, int[] equipmentIDs, bool nightAirAttackFlag, int defenderHP)
			: base(bd, attackerId, defenderId, damages, criticalTypes, attackType, equipmentIDs, defenderHP)
		{
            this.NightAirAttackFlag = nightAirAttackFlag;

			int[] attackerSlots;

			if (attackerId.IsFriend)
			{
                this.Attacker = bd.FriendlySupport.FriendlyMembersInstance[attackerId.Index];
				attackerSlots = bd.FriendlySupport.FriendlySlots[attackerId.Index];
			}
			else
			{
				attackerSlots = this.SetAttacker();
			}

			if (defenderId.IsFriend)
                this.Defender = bd.FriendlySupport.FriendlyMembersInstance[defenderId.Index];
			else
                this.SetDefender();


			if (this.AttackType == 0 && this.Attacker != null)
                this.AttackType = this.CaclulateAttackKind(attackerSlots, this.Attacker.ShipID, this.Defender.ShipID);
		}

		protected override int CaclulateAttackKind(int[] slots, int attackerShipID, int defenderShipID)
		{
			return (int)Calculator.GetNightAttackKind(slots, attackerShipID, defenderShipID, false, this.NightAirAttackFlag);
		}

		protected override string GetAttackKind()
		{
			return Constants.GetNightAttackKind((NightAttackKind)this.AttackType);
		}
	}

}
