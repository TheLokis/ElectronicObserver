using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.ShipGroup
{

	/// <summary>
	/// 艦船フィルタの式データ
	/// </summary>
	[DataContract(Name = "ExpressionData")]
	public class ExpressionData : ICloneable
	{

		public enum ExpressionOperator
		{
			Equal,
			NotEqual,
			LessThan,
			LessEqual,
			GreaterThan,
			GreaterEqual,
			Contains,
			NotContains,
			BeginWith,
			NotBeginWith,
			EndWith,
			NotEndWith,
			ArrayContains,
			ArrayNotContains,
		}


		[DataMember]
		public string LeftOperand { get; set; }

		[DataMember]
		public ExpressionOperator Operator { get; set; }

		[DataMember]
		public object RightOperand { get; set; }


		[DataMember]
		public bool Enabled { get; set; }


		[IgnoreDataMember]
		private static readonly Regex regex_index = new Regex(@"\.(?<name>\w+)(\[(?<index>\d+?)\])?", RegexOptions.Compiled);

		[IgnoreDataMember]
		public static readonly Dictionary<string, string> LeftOperandNameTable = new Dictionary<string, string>() {
			{ ".MasterID", "고유ID" },
			{ ".ShipID", "함선ID" },
			{ ".MasterShip.NameWithClass", "함명" },
			{ ".MasterShip.ShipType", "함종" },
			{ ".Level", "레벨" },
			{ ".ExpTotal", "경험치" },
			{ ".ExpNext", "다음레벨" },
			{ ".ExpNextRemodel", "개장까지" },
			{ ".HPCurrent", "현재HP" },
			{ ".HPMax", "최대HP" },
			{ ".HPRate", "HP비율" },
			{ ".Condition", "피로도" },
			{ ".AllSlotMaster", "슬롯" },
			{ ".SlotMaster[0]", "슬롯 #1" },	//checkme: 要る?
			{ ".SlotMaster[1]", "슬롯 #2" },
			{ ".SlotMaster[2]", "슬롯 #3" },
			{ ".SlotMaster[3]", "슬롯 #4" },
			{ ".SlotMaster[4]", "슬롯 #5" },
			{ ".ExpansionSlotMaster", "보강증설" },
			{ ".Aircraft[0]", "탑재 #1" },
			{ ".Aircraft[1]", "탑재 #2" },
			{ ".Aircraft[2]", "탑재 #3" },
			{ ".Aircraft[3]", "탑재 #4" },
			{ ".Aircraft[4]", "탑재 #5" },
			{ ".AircraftTotal", "탑재총합" },
			{ ".MasterShip.Aircraft[0]", "최대탑재 #1" },
			{ ".MasterShip.Aircraft[1]", "최대탑재 #2" },
			{ ".MasterShip.Aircraft[2]", "최대탑재 #3" },
			{ ".MasterShip.Aircraft[3]", "최대탑재 #4" },
			{ ".MasterShip.Aircraft[4]", "최대탑재 #5" },
			{ ".MasterShip.AircraftTotal", "최대탑재총합" },		//要る？
			{ ".AircraftRate[0]", "탑재비율 #1" },
			{ ".AircraftRate[1]", "탑재비율 #2" },
			{ ".AircraftRate[2]", "탑재비율 #3" },
			{ ".AircraftRate[3]", "탑재비율 #4" },
			{ ".AircraftRate[4]", "탑재비율 #5" },
			{ ".AircraftTotalRate", "탑재비율총합" },
			{ ".Fuel", "소비연료" },
			{ ".Ammo", "소비탄약" },
			{ ".FuelMax", "최대소비연료" },
			{ ".AmmoMax", "최대소비탄약" },
			{ ".FuelRate", "현재연료" },
			{ ".AmmoRate", "현재탄약" },
			{ ".SlotSize", "슬롯수" },
			{ ".RepairingDockID", "입거독번호" },
			{ ".RepairTime", "입거시간" },
			{ ".RepairSteel", "입거피료강재" },
			{ ".RepairFuel", "입거필요연료" },
			//強化値シリーズは省略
			{ ".FirepowerBase", "기본화력" },
			{ ".TorpedoBase", "기본뇌장" },
			{ ".AABase", "기본대공" },
			{ ".ArmorBase", "기본장갑" },
			{ ".EvasionBase", "기본회피" },
			{ ".ASWBase", "기본대잠" },
			{ ".LOSBase", "기본색적" },
			{ ".LuckBase", "기본운" },
			{ ".FirepowerTotal", "화력" },
			{ ".TorpedoTotal", "뇌장" },
			{ ".AATotal", "대공" },
			{ ".ArmorTotal", "장갑" },
			{ ".EvasionTotal", "회피" },
			{ ".ASWTotal", "대잠" },
			{ ".LOSTotal", "색적" },
			{ ".LuckTotal", "운" },
			{ ".BomberTotal", "폭장" },
			{ ".FirepowerRemain", "화력개수" },
			{ ".TorpedoRemain", "뇌장개수" },
			{ ".AARemain", "대공개수" },
			{ ".ArmorRemain", "장갑개수" },
			{ ".LuckRemain", "운개수" },
			{ ".Range", "사정" },		//現在の射程
			{ ".Speed", "속력" },
			{ ".MasterShip.Speed", "기본속력" },
			{ ".MasterShip.Rarity", "레어리티" },
			{ ".IsLocked", "잠금" },
			{ ".IsLockedByEquipment", "장비잠금" },
			{ ".SallyArea", "출격해역" },
			{ ".FleetWithIndex", "소속함대" },
			{ ".IsMarried", "결혼" },
			{ ".AirBattlePower", "제공치" },
			{ ".ShellingPower", "포격화력" },
			{ ".AircraftPower", "항공화력" },
			{ ".AntiSubmarinePower", "대잠공격" },
			{ ".TorpedoPower", "뇌격" },
			{ ".NightBattlePower", "야간화력" },
			{ ".MasterShip.AlbumNo", "도감번호" },
			{ ".MasterShip.NameReading", "함명" },
			{ ".MasterShip.RemodelBeforeShipID", "개장전함선ID" },
			{ ".MasterShip.RemodelAfterShipID", "개장후함선ID" },
			//マスターのパラメータ系もおそらく意味がないので省略		
		};

		private static Dictionary<string, Type> ExpressionTypeTable = new Dictionary<string, Type>();


		[IgnoreDataMember]
		public static readonly Dictionary<ExpressionOperator, string> OperatorNameTable = new Dictionary<ExpressionOperator, string>() {
			{ ExpressionOperator.Equal, "임" },
			{ ExpressionOperator.NotEqual, "이 아님" },
			{ ExpressionOperator.LessThan, "미만" },
			{ ExpressionOperator.LessEqual, "이하" },
			{ ExpressionOperator.GreaterThan, "초과" },
			{ ExpressionOperator.GreaterEqual, "이상" },
			{ ExpressionOperator.Contains, "포함" },
			{ ExpressionOperator.NotContains, "미포함" },
			{ ExpressionOperator.BeginWith, "에서시작" },
			{ ExpressionOperator.NotBeginWith, "에서시작하지않는" },
			{ ExpressionOperator.EndWith, "로끝나는" },
			{ ExpressionOperator.NotEndWith, "로끝나지않는" },
			{ ExpressionOperator.ArrayContains, "포함" },
			{ ExpressionOperator.ArrayNotContains, "미포함" },

		};



		public ExpressionData()
		{
            this.Enabled = true;
		}

		public ExpressionData(string left, ExpressionOperator ope, object right)
			: this()
		{
            this.LeftOperand = left;
            this.Operator = ope;
            this.RightOperand = right;
		}


		public Expression Compile(ParameterExpression paramex)
		{

			Expression memberex = null;
			Expression constex = Expression.Constant(this.RightOperand, this.RightOperand.GetType());

			{
				Match match = regex_index.Match(this.LeftOperand);
				if (match.Success)
				{

					do
					{

						if (memberex == null)
						{
							memberex = Expression.PropertyOrField(paramex, match.Groups["name"].Value);
						}
						else
						{
							memberex = Expression.PropertyOrField(memberex, match.Groups["name"].Value);
						}

						if (int.TryParse(match.Groups["index"].Value, out int index))
						{
							memberex = Expression.Property(memberex, "Item", Expression.Constant(index, typeof(int)));
						}

					} while ((match = match.NextMatch()).Success);

				}
				else
				{
					memberex = Expression.PropertyOrField(paramex, this.LeftOperand);
				}
			}

			if (memberex.Type.IsEnum)
				memberex = Expression.Convert(memberex, typeof(int));

			Expression condex;
			switch (this.Operator)
			{
				case ExpressionOperator.Equal:
					condex = Expression.Equal(memberex, constex);
					break;
				case ExpressionOperator.NotEqual:
					condex = Expression.NotEqual(memberex, constex);
					break;
				case ExpressionOperator.LessThan:
					condex = Expression.LessThan(memberex, constex);
					break;
				case ExpressionOperator.LessEqual:
					condex = Expression.LessThanOrEqual(memberex, constex);
					break;
				case ExpressionOperator.GreaterThan:
					condex = Expression.GreaterThan(memberex, constex);
					break;
				case ExpressionOperator.GreaterEqual:
					condex = Expression.GreaterThanOrEqual(memberex, constex);
					break;
				case ExpressionOperator.Contains:
					condex = Expression.Call(memberex, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), constex);
					break;
				case ExpressionOperator.NotContains:
					condex = Expression.Not(Expression.Call(memberex, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), constex));
					break;
				case ExpressionOperator.BeginWith:
					condex = Expression.Call(memberex, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), constex);
					break;
				case ExpressionOperator.NotBeginWith:
					condex = Expression.Not(Expression.Call(memberex, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), constex));
					break;
				case ExpressionOperator.EndWith:
					condex = Expression.Call(memberex, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), constex);
					break;
				case ExpressionOperator.NotEndWith:
					condex = Expression.Not(Expression.Call(memberex, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), constex));
					break;
				case ExpressionOperator.ArrayContains:  // returns Enumerable.Contains<>( memberex )
					condex = Expression.Call(typeof(Enumerable), "Contains", new Type[] { memberex.Type.GetElementType() ?? memberex.Type.GetGenericArguments().First() }, memberex, constex);
					break;
				case ExpressionOperator.ArrayNotContains:   // returns !Enumerable.Contains<>( memberex )
					condex = Expression.Not(Expression.Call(typeof(Enumerable), "Contains", new Type[] { memberex.Type.GetElementType() ?? memberex.Type.GetGenericArguments().First() }, memberex, constex));
					break;

				default:
					throw new NotImplementedException();
			}

			return condex;
		}



		public static Type GetLeftOperandType(string left)
		{

			if (ExpressionTypeTable.ContainsKey(left))
			{
				return ExpressionTypeTable[left];

			}
			else if (KCDatabase.Instance.Ships.Count > 0)
			{

				object obj = KCDatabase.Instance.Ships.Values.First();

				Match match = regex_index.Match(left);
				if (match.Success)
				{

					do
					{

						if (int.TryParse(match.Groups["index"].Value, out int index))
						{
							obj = ((dynamic)obj.GetType().InvokeMember(match.Groups["name"].Value, System.Reflection.BindingFlags.GetProperty, null, obj, null))[index];
						}
						else
						{
							object obj2 = obj.GetType().InvokeMember(match.Groups["name"].Value, System.Reflection.BindingFlags.GetProperty, null, obj, null);
							if (obj2 == null)
							{   //プロパティはあるけどnull
								var type = obj.GetType().GetProperty(match.Groups["name"].Value).GetType();
								ExpressionTypeTable.Add(left, type);
								return type;
							}
							else
							{
								obj = obj2;
							}
						}

					} while (obj != null && (match = match.NextMatch()).Success);


					if (obj != null)
					{
						ExpressionTypeTable.Add(left, obj.GetType());
						return obj.GetType();
					}
				}

			}

			return null;
		}

		public Type GetLeftOperandType()
		{
			return GetLeftOperandType(this.LeftOperand);
		}



		public override string ToString() => $"{this.LeftOperandToString()} 값이 {this.RightOperandToString()} {this.OperatorToString()}";



		/// <summary>
		/// 左辺値の文字列表現を求めます。
		/// </summary>
		public string LeftOperandToString()
		{
			if (LeftOperandNameTable.ContainsKey(this.LeftOperand))
				return LeftOperandNameTable[this.LeftOperand];
			else
				return this.LeftOperand;
		}

		/// <summary>
		/// 演算子の文字列表現を求めます。
		/// </summary>
		public string OperatorToString()
		{
			return OperatorNameTable[this.Operator];
		}

		/// <summary>
		/// 右辺値の文字列表現を求めます。
		/// </summary>
		public string RightOperandToString()
		{

			if (this.LeftOperand == ".MasterID")
			{
				var ship = KCDatabase.Instance.Ships[(int)this.RightOperand];
				if (ship != null)
					return $"{ship.MasterID} ({ship.NameWithLevel})";
				else
					return $"{(int)this.RightOperand} (미등록)";

			}
			else if (this.LeftOperand == ".ShipID")
			{
				var ship = KCDatabase.Instance.MasterShips[(int)this.RightOperand];
				if (ship != null)
					return $"{ship.ShipID} ({ship.NameWithClass})";
				else
					return $"{(int)this.RightOperand} (불명)";

			}
			else if (this.LeftOperand == ".MasterShip.ShipType")
			{
				var shiptype = KCDatabase.Instance.ShipTypes[(int)this.RightOperand];
				if (shiptype != null)
                    return FormMain.Instance.Translator.GetTranslation(shiptype.Name, Utility.DataType.ShipType);
				else
					return $"{(int)this.RightOperand} (미정의)";

			}
			else if (this.LeftOperand.Contains("SlotMaster"))
			{
				if ((int)this.RightOperand == -1)
				{
					return "(없음)";
				}
				else
				{
					var eq = KCDatabase.Instance.MasterEquipments[(int)this.RightOperand];
					if (eq != null)
						return eq.Name;
					else
						return $"{(int)this.RightOperand} (미정의)";
				}
			}
			else if (this.LeftOperand.Contains("Rate") && this.RightOperand is double)
			{
				return ((double)this.RightOperand).ToString("P0");

			}
			else if (this.LeftOperand == ".RepairTime")
			{
				return DateTimeHelper.ToTimeRemainString(DateTimeHelper.FromAPITimeSpan((int)this.RightOperand));

			}
			else if (this.LeftOperand == ".Range")
			{
				return Constants.GetRange((int)this.RightOperand);

			}
			else if (this.LeftOperand == ".Speed" || this.LeftOperand == ".MasterShip.Speed")
			{
				return Constants.GetSpeed((int)this.RightOperand);

			}
			else if (this.LeftOperand == ".MasterShip.Rarity")
			{
				return Constants.GetShipRarity((int)this.RightOperand);

			}
			else if (this.LeftOperand == ".MasterShip.AlbumNo")
			{
				var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.AlbumNo == (int)this.RightOperand);
				if (ship != null)
					return $"{(int)this.RightOperand} ({ship.NameWithClass})";
				else
					return $"{(int)this.RightOperand} (불명)";

			}
			else if (this.LeftOperand == ".MasterShip.RemodelAfterShipID")
			{

				if (((int)this.RightOperand) == 0)
					return "최종개장";

				var ship = KCDatabase.Instance.MasterShips[(int)this.RightOperand];
				if (ship != null)
					return $"{ship.ShipID} ({ship.NameWithClass})";
				else
					return $"{(int)this.RightOperand} (불명)";

			}
			else if (this.LeftOperand == ".MasterShip.RemodelBeforeShipID")
			{

				if (((int)this.RightOperand) == 0)
					return "미개장";

				var ship = KCDatabase.Instance.MasterShips[(int)this.RightOperand];
				if (ship != null)
					return $"{ship.ShipID} ({ship.NameWithClass})";
				else
					return $"{(int)this.RightOperand} (불명)";

			}
			else if (this.RightOperand is bool)
			{
				return ((bool)this.RightOperand) ? "○" : "×";

			}
			else
			{
				return this.RightOperand.ToString();

			}

		}


		public ExpressionData Clone()
		{
			var clone = this.MemberwiseClone();      //checkme: 右辺値に参照型を含む場合死ぬ
			return (ExpressionData)clone;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}




}
