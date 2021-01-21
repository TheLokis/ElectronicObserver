using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogBaseAirCorpsSimulation : Form
	{

		/// <summary> 基地航空隊に配備可能な航空機リスト </summary>
		private static readonly HashSet<EquipmentTypes> SquadronAircraftCategories = new HashSet<EquipmentTypes> {

			EquipmentTypes.CarrierBasedFighter,
			EquipmentTypes.CarrierBasedBomber,
			EquipmentTypes.CarrierBasedTorpedo,
			EquipmentTypes.CarrierBasedRecon,
			EquipmentTypes.SeaplaneRecon,
			EquipmentTypes.SeaplaneBomber,
			EquipmentTypes.FlyingBoat,
			EquipmentTypes.SeaplaneFighter,
			EquipmentTypes.LandBasedAttacker,
			EquipmentTypes.Interceptor,
            EquipmentTypes.LandBasedRecon,
			EquipmentTypes.HeavyBomber,
			EquipmentTypes.JetFighter,
			EquipmentTypes.JetBomber,
			EquipmentTypes.JetTorpedo,
			EquipmentTypes.JetRecon,
		};

		/// <summary> 基地航空隊に配備可能な攻撃系航空機リスト </summary>
		private static readonly HashSet<EquipmentTypes> SquadronAttackerCategories = new HashSet<EquipmentTypes> {

			EquipmentTypes.CarrierBasedBomber,
			EquipmentTypes.CarrierBasedTorpedo,
			EquipmentTypes.SeaplaneBomber,
			EquipmentTypes.LandBasedAttacker,
			EquipmentTypes.HeavyBomber,
			EquipmentTypes.JetBomber,
			EquipmentTypes.JetTorpedo,
		};

		/// <summary> 基地航空隊に配備可能な戦闘機リスト </summary>
		private static readonly HashSet<EquipmentTypes> SquadronFighterCategories = new HashSet<EquipmentTypes> {

			EquipmentTypes.CarrierBasedFighter,
			EquipmentTypes.SeaplaneFighter,
			EquipmentTypes.Interceptor,
			EquipmentTypes.JetFighter,
		};

		/// <summary> 基地航空隊に配備可能な偵察機リスト </summary>
		private static readonly HashSet<EquipmentTypes> SquadronReconCategories = new HashSet<EquipmentTypes> {

			EquipmentTypes.CarrierBasedRecon,
			EquipmentTypes.SeaplaneRecon,
			EquipmentTypes.FlyingBoat,
            EquipmentTypes.LandBasedRecon,
            EquipmentTypes.JetRecon,
		};

        public static string RangeString(int min, int max) => min == max ? $"{min}" : $"{min} ～ {max}";

        private class SquadronUI : IDisposable
		{

			public readonly int BaseAirCorpsID;
			public readonly int SquadronID;

			public ComboBox AircraftCategory;
			public ComboBox Aircraft;

			public NumericUpDown AircraftCount;

			public Label AirSuperioritySortie;
			public Label AirSuperiorityAirDefense;
			public Label Distance;
			public Label Bomber;
			public Label Torpedo;
			public Label OrganizationCost;

			public DialogBaseAirCorpsSimulation Parent;
			public ToolTip ToolTipInternal;

            public event EventHandler Updated = delegate { };


			public SquadronUI(int baseAirCorpsID, int squadronID, DialogBaseAirCorpsSimulation parent)
			{

                this.BaseAirCorpsID = baseAirCorpsID;
                this.SquadronID = squadronID;

                this.AircraftCategory = new ComboBox();
                this.AircraftCategory.Size = new Size(160, this.AircraftCategory.Height);
                this.AircraftCategory.Anchor = AnchorStyles.None;
                this.AircraftCategory.Margin = new Padding(2, 0, 2, 0);
                this.AircraftCategory.DropDownStyle = ComboBoxStyle.DropDownList;
                this.AircraftCategory.Items.AddRange(ComboBoxCategory.GetAllCategories().ToArray());
                this.AircraftCategory.SelectedValueChanged += this.AircraftCategory_SelectedValueChanged;

                this.Aircraft = new ComboBox();
                this.Aircraft.Size = new Size(240, this.Aircraft.Height);
                this.Aircraft.Anchor = AnchorStyles.None;
                this.Aircraft.Margin = new Padding(2, 0, 2, 0);
                this.Aircraft.DropDownStyle = ComboBoxStyle.DropDownList;
                this.Aircraft.SelectedValueChanged += this.Aircraft_SelectedValueChanged;

                this.AircraftCount = new NumericUpDown();
                this.AircraftCount.Size = new Size(60, this.AircraftCount.Height);
                this.AircraftCount.Anchor = AnchorStyles.None;
                this.AircraftCount.Maximum = this.AircraftCount.Minimum = 0;
                this.AircraftCount.TextAlign = HorizontalAlignment.Right;
                this.AircraftCount.Margin = new Padding(2, 0, 2, 0);
                this.AircraftCount.ValueChanged += this.AircraftCount_ValueChanged;

                this.AirSuperioritySortie = this.NewLabel();
                this.AirSuperiorityAirDefense = this.NewLabel();
                this.Distance = this.NewLabel();
                this.Bomber = this.NewLabel();
                this.Torpedo = this.NewLabel();
                this.OrganizationCost = this.NewLabel();

                this.Parent = parent;
                this.ToolTipInternal = parent.ToolTipInfo;

                this.Update();
			}


			private Label NewLabel()
			{
				var label = new Label
				{
					Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					TextAlign = ContentAlignment.MiddleRight
				};

				return label;
			}

			public void AddToTable(TableLayoutPanel table, int row)
			{
				table.Controls.Add(this.AircraftCategory, 0, row);
				table.Controls.Add(this.Aircraft, 1, row);
				table.Controls.Add(this.AircraftCount, 2, row);
				table.Controls.Add(this.AirSuperioritySortie, 3, row);
				table.Controls.Add(this.AirSuperiorityAirDefense, 4, row);
				table.Controls.Add(this.Distance, 5, row);
				table.Controls.Add(this.Bomber, 6, row);
				table.Controls.Add(this.Torpedo, 7, row);
				table.Controls.Add(this.OrganizationCost, 8, row);
			}

			void AircraftCategory_SelectedValueChanged(object sender, EventArgs e)
			{

				// 指定されたカテゴリにおいて、利用可能な装備を列挙する

				var category = this.AircraftCategory.SelectedItem as ComboBoxCategory;

				IEnumerable<ComboBoxEquipment> list = new[] { new ComboBoxEquipment() };

				if (category != null)
				{
					list = list.Concat(KCDatabase.Instance.Equipments.Values
						.Where(eq => eq != null && (int)eq.MasterEquipment.CategoryType == category.EquipmentType.TypeID)
						.OrderBy(eq => eq.EquipmentID)
						.ThenBy(eq => eq.Level)
						.ThenBy(eq => eq.AircraftLevel)
						.Select(eq => new ComboBoxEquipment(eq)));
				}

                this.Aircraft.Items.Clear();
                this.Aircraft.Items.AddRange(list.ToArray());
                this.Aircraft.SelectedIndex = 0;

			}

			void Aircraft_SelectedValueChanged(object sender, EventArgs e)
			{

				var equipment = this.Aircraft.SelectedItem as ComboBoxEquipment;

				if (equipment == null || equipment.EquipmentID == -1)
				{
                    this.AircraftCount.Maximum = 0;

                    this.ToolTipInternal.SetToolTip(this.Aircraft, null);
				}
				else
				{
                    int aircraftCount =
                        equipment.EquipmentInstance.CategoryType == EquipmentTypes.HeavyBomber ? 9 :
                        equipment.EquipmentInstance.IsCombatAircraft ? 18 : 4;

                    this.AircraftCount.Value = this.AircraftCount.Maximum = aircraftCount;

                    this.ToolTipInternal.SetToolTip(this.Aircraft, GetAircraftParameters(equipment.EquipmentInstance));
				}

                this.Update();

			}

			void AircraftCount_ValueChanged(object sender, EventArgs e)
			{
                this.Update();
			}

			private static string GetAircraftParameters(EquipmentDataMaster eq)
			{

				if (eq == null)
					return "";

				var sb = new StringBuilder();

				void Add(string name, int value)
				{
					if (value != 0)
						sb.Append(name).Append(": ").AppendLine(value.ToString("+0;-0;0"));
				}

				void AddNoSign(string name, int value)
				{
					if (value != 0)
						sb.Append(name).Append(": ").AppendLine(value.ToString());
				}

				bool isLand = eq.CategoryType == EquipmentTypes.Interceptor;

				Add("화력", eq.Firepower);
				Add("뇌장", eq.Torpedo);
				Add("폭장", eq.Bomber);
				Add("대공", eq.AA);
				Add("장갑", eq.Armor);
				Add("대잠", eq.ASW);
				Add(isLand ? "영격" : "회피", eq.Evasion);
				Add("색적", eq.LOS);
				Add(isLand ? "대폭" : "명중", eq.Accuracy);
				AddNoSign("배치비용", eq.AircraftCost);
				AddNoSign("행동반경", eq.AircraftDistance);

				return sb.ToString();
			}

			private void Update()
			{
                var equipment = this.Aircraft.SelectedItem as ComboBoxEquipment;

                if (equipment == null || equipment.EquipmentID == -1)
                {
                    this.AirSuperioritySortie.Text = "0";
                    this.AirSuperioritySortie.Tag = 0;
                    this.AirSuperiorityAirDefense.Text = "0";
                    this.AirSuperiorityAirDefense.Tag = 0;
                    this.Distance.Text = "0";
                    this.Bomber.Text = "0";
                    this.Torpedo.Text = "0";
                    this.OrganizationCost.Text = "0";
                    this.OrganizationCost.Tag = 0;

                }
                else
                {

                    var eq = equipment.EquipmentInstance;
                    int aircraftCount = (int)this.AircraftCount.Value;

                    var isranged = Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange;

                    int airSuperioritySortie = Calculator.GetAirSuperiority(equipment.EquipmentID, aircraftCount, equipment.AircraftLevel, equipment.Level, 1);
                    int airSuperioritySortieMax = Calculator.GetAirSuperiority(equipment.EquipmentID, aircraftCount, equipment.AircraftLevel, equipment.Level, 1, true);
                    this.AirSuperioritySortie.Text = isranged ? RangeString(airSuperioritySortie, airSuperioritySortieMax) : airSuperioritySortie.ToString();
                    this.AirSuperioritySortie.Tag = airSuperioritySortie;

                    int airSuperiorityAirDefense = Calculator.GetAirSuperiority(equipment.EquipmentID, aircraftCount, equipment.AircraftLevel, equipment.Level, 2);
                    int airSuperiorityAirDefenseMax = Calculator.GetAirSuperiority(equipment.EquipmentID, aircraftCount, equipment.AircraftLevel, equipment.Level, 2, true);
                    this.AirSuperiorityAirDefense.Text = isranged ? RangeString(airSuperiorityAirDefense, airSuperiorityAirDefenseMax) : airSuperiorityAirDefense.ToString();
                    this.AirSuperiorityAirDefense.Tag = airSuperiorityAirDefense;

                    this.Distance.Text = eq.AircraftDistance.ToString();

                    this.Torpedo.Text = eq.Torpedo.ToString();
                    this.Bomber.Text = eq.Bomber.ToString();

                    int organizationCost = aircraftCount * eq.AircraftCost;
                    this.OrganizationCost.Text = organizationCost.ToString();
                    this.OrganizationCost.Tag = organizationCost;

                }

                Updated(this, new EventArgs());

            }

			public void Dispose()
			{
                this.AircraftCategory.Dispose();
                this.Aircraft.Dispose();

                this.AircraftCount.Dispose();

                this.AirSuperioritySortie.Dispose();
                this.AirSuperiorityAirDefense.Dispose();
                this.Distance.Dispose();
                this.Bomber.Dispose();
                this.Torpedo.Dispose();
                this.OrganizationCost.Dispose();
			}
		}


		private class BaseAirCorpsUI : IDisposable
		{

			public readonly int BaseAirCorpsID;

			public Label TitleAircraftCategory;
			public Label TitleAircraft;
			public Label TitleAircraftCount;
			public Label TitleAirSuperioritySortie;
			public Label TitleAirSuperiorityAirDefense;
			public Label TitleDistance;
			public Label TitleBomber;
			public Label TitleTorpedo;
			public Label TitleOrganizationCost;

			public SquadronUI[] Squadrons;

			public Label TitleTotal;
			public Label DuplicateCheck;
			public Label TotalAirSuperioritySortie;
			public Label TotalAirSuperiorityAirDefense;
			public Label TotalDistance;
			public Label TotalOrganizationCost;

			public Label TitleAutoAirSuperiority;
			public Label TitleAutoDistance;
			public ComboBox AutoAirSuperiorityMode;
			public NumericUpDown AutoAirSuperiority;
			public NumericUpDown AutoDistance;
			public Button AutoOrganizeSortie;
			public Button AutoOrganizeAirDefense;

			public DialogBaseAirCorpsSimulation Parent;
			public ToolTip ToolTipInternal;
            public bool IsHighAltitude => this.Parent.TopMenu_Settings_HighAltitude.Checked;

            public event EventHandler Updated = delegate { };


			public BaseAirCorpsUI(int baseAirCorpsID, DialogBaseAirCorpsSimulation parent)
			{

                this.BaseAirCorpsID = baseAirCorpsID;

                this.TitleAircraftCategory = this.NewTitleLabel();
                this.TitleAircraft = this.NewTitleLabel();
                this.TitleAircraftCount = this.NewTitleLabel();
                this.TitleAirSuperioritySortie = this.NewTitleLabel();
                this.TitleAirSuperiorityAirDefense = this.NewTitleLabel();
                this.TitleDistance = this.NewTitleLabel();
                this.TitleBomber = this.NewTitleLabel();
                this.TitleTorpedo = this.NewTitleLabel();
                this.TitleOrganizationCost = this.NewTitleLabel();
                this.TitleAutoAirSuperiority = this.NewTitleLabel();
                this.TitleAutoDistance = this.NewTitleLabel();

                this.TitleAircraftCategory.Text = "카테고리";
                this.TitleAircraft.Text = "배치기";
                this.TitleAircraftCount.Text = "수";
                this.TitleAirSuperioritySortie.Text = "출격제공";
                this.TitleAirSuperiorityAirDefense.Text = "방공제공";
                this.TitleDistance.Text = "행동반경";
                this.TitleBomber.Text = "폭장";
                this.TitleTorpedo.Text = "뇌장";
                this.TitleOrganizationCost.Text = "배치코스트";
                this.TitleAutoAirSuperiority.Text = "목표제공";
                this.TitleAutoDistance.Text = "목표반경";

                this.AutoAirSuperiority = new NumericUpDown();
                this.AutoAirSuperiority.Size = new Size(60, this.AutoAirSuperiority.Height);
                this.AutoAirSuperiority.Anchor = AnchorStyles.None;
                this.AutoAirSuperiority.Maximum = 9999;
                this.AutoAirSuperiority.TextAlign = HorizontalAlignment.Right;
                this.AutoAirSuperiority.Margin = new Padding(2, 0, 2, 0);

                this.AutoDistance = new NumericUpDown();
                this.AutoDistance.Size = new Size(60, this.AutoDistance.Height);
                this.AutoDistance.Anchor = AnchorStyles.None;
                this.AutoDistance.Maximum = 20;
                this.AutoDistance.TextAlign = HorizontalAlignment.Right;
                this.AutoDistance.Margin = new Padding(2, 0, 2, 0);

                this.AutoAirSuperiorityMode = new ComboBox();
                this.AutoAirSuperiorityMode.Size = new Size(160, this.AutoAirSuperiorityMode.Height);
                this.AutoAirSuperiorityMode.Anchor = AnchorStyles.None;
                this.AutoAirSuperiorityMode.Margin = new Padding(2, 0, 2, 0);
                this.AutoAirSuperiorityMode.DropDownStyle = ComboBoxStyle.DropDownList;
                this.AutoAirSuperiorityMode.Items.Add(-1);
                this.AutoAirSuperiorityMode.Items.Add(1);
                this.AutoAirSuperiorityMode.Items.Add(2);
                this.AutoAirSuperiorityMode.Items.Add(0);
                this.AutoAirSuperiorityMode.Items.Add(3);
                this.AutoAirSuperiorityMode.Items.Add(4);
                this.AutoAirSuperiorityMode.FormattingEnabled = true;
                this.AutoAirSuperiorityMode.Format += this.AutoAirSuperiorityMode_Format;
                this.AutoAirSuperiorityMode.SelectedIndex = 0;

                this.AutoOrganizeSortie = new Button();
                this.AutoOrganizeSortie.Size = new Size(60, this.AutoOrganizeSortie.Height);
                this.AutoOrganizeSortie.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                this.AutoOrganizeSortie.Margin = new Padding(2, 0, 2, 0);
                this.AutoOrganizeSortie.Text = "출격편성";
                this.AutoOrganizeSortie.Click += this.AutoOrganize_Click;

                this.AutoOrganizeAirDefense = new Button
				{
					Size = new Size(60, this.AutoOrganizeSortie.Height),
					Anchor = AnchorStyles.Left | AnchorStyles.Right,
					Margin = new Padding(2, 0, 2, 0),
					Text = "방공편성"
				};
                this.AutoOrganizeAirDefense.Click += this.AutoOrganize_Click;

                this.Squadrons = new SquadronUI[4];
				for (int i = 0; i < this.Squadrons.Length; i++)
				{
                    this.Squadrons[i] = new SquadronUI(baseAirCorpsID, i + 1, parent);
                    this.Squadrons[i].Updated += this.BaseAirCorpsUI_Updated;
				}

                this.TitleTotal = this.NewTitleLabel();
                this.DuplicateCheck = this.NewTitleLabel();
                this.TotalAirSuperioritySortie = this.NewTotalLabel();
                this.TotalAirSuperiorityAirDefense = this.NewTotalLabel();
                this.TotalDistance = this.NewTotalLabel();
                this.TotalOrganizationCost = this.NewTotalLabel();

                this.TitleTotal.Text = "합계";
                this.DuplicateCheck.TextAlign = ContentAlignment.MiddleLeft;
                this.DuplicateCheck.ForeColor = Color.Red;

                this.Parent = parent;
                this.ToolTipInternal = parent.ToolTipInfo;
                this.Parent.TopMenu_Settings_HighAltitude.CheckedChanged += this.BaseAirCorpsUI_Updated;

                this.BaseAirCorpsUI_Updated(null, new EventArgs());
			}

			private Label NewTitleLabel()
			{
				var label = new Label
				{
					Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					TextAlign = ContentAlignment.MiddleCenter
				};

				return label;
			}

			private Label NewTotalLabel()
			{
				var label = new Label
				{
					Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
					Padding = new Padding(0, 1, 0, 1),
					Margin = new Padding(2, 1, 2, 1),
					TextAlign = ContentAlignment.MiddleRight
				};

				return label;
			}

			public void AddToTable(TableLayoutPanel table)
			{

				table.Controls.Add(this.TitleAircraftCategory, 0, 0);
				table.Controls.Add(this.TitleAircraft, 1, 0);
				table.Controls.Add(this.TitleAircraftCount, 2, 0);
				table.Controls.Add(this.TitleAirSuperioritySortie, 3, 0);
				table.Controls.Add(this.TitleAirSuperiorityAirDefense, 4, 0);
				table.Controls.Add(this.TitleDistance, 5, 0);
				table.Controls.Add(this.TitleBomber, 6, 0);
				table.Controls.Add(this.TitleTorpedo, 7, 0);
				table.Controls.Add(this.TitleOrganizationCost, 8, 0);

				for (int i = 0; i < this.Squadrons.Length; i++)
				{
                    this.Squadrons[i].AddToTable(table, i + 1);
				}

				table.Controls.Add(this.TitleTotal, 0, this.Squadrons.Length + 1);
				table.Controls.Add(this.DuplicateCheck, 1, this.Squadrons.Length + 1);
				table.Controls.Add(this.TotalAirSuperioritySortie, 3, this.Squadrons.Length + 1);
				table.Controls.Add(this.TotalAirSuperiorityAirDefense, 4, this.Squadrons.Length + 1);
				table.Controls.Add(this.TotalDistance, 5, this.Squadrons.Length + 1);
				table.Controls.Add(this.TotalOrganizationCost, 8, this.Squadrons.Length + 1);

				int autocolumn = 9;
				table.Controls.Add(this.TitleAutoAirSuperiority, autocolumn + 0, 0);
				table.Controls.Add(this.TitleAutoDistance, autocolumn + 1, 0);
				table.Controls.Add(this.AutoAirSuperiority, autocolumn + 0, 1);
				table.Controls.Add(this.AutoDistance, autocolumn + 1, 1);
				table.Controls.Add(this.AutoAirSuperiorityMode, autocolumn + 0, 2);
				table.Controls.Add(this.AutoOrganizeSortie, autocolumn + 0, 5);
				table.Controls.Add(this.AutoOrganizeAirDefense, autocolumn + 1, 5);

				table.SetColumnSpan(this.AutoAirSuperiorityMode, 2);
			}


			void BaseAirCorpsUI_Updated(object sender, EventArgs e)
			{

				var squadrons = this.Squadrons.Select(sq => sq.Aircraft.SelectedItem as ComboBoxEquipment)
                    .Where(eq => eq?.EquipmentInstance != null);


                int airSortie = this.Squadrons.Select(sq => sq.AirSuperioritySortie.Tag as int? ?? 0).Sum();
                airSortie = (int)(airSortie * squadrons.Select(eq => Calculator.GetAirSuperioritySortieReconBonus(eq.EquipmentID)).DefaultIfEmpty(1).Max());

                this.TotalAirSuperioritySortie.Text = airSortie.ToString();
                this.ToolTipInternal.SetToolTip(this.TotalAirSuperioritySortie,
					string.Format("확보: {0}\r\n우세: {1}\r\n균등: {2}\r\n열세: {3}\r\n",
						(int)(airSortie / 3.0),
						(int)(airSortie / 1.5),
						Math.Max((int)(airSortie * 1.5 - 1), 0),
						Math.Max((int)(airSortie * 3.0 - 1), 0)));


				int airDefense = this.Squadrons.Select(sq => sq.AirSuperiorityAirDefense.Tag as int? ?? 0).Sum();

                airDefense = (int)(airDefense * squadrons.Select(eq => Calculator.GetAirSuperiorityAirDefenseReconBonus(eq.EquipmentID)).DefaultIfEmpty(1).Max());

                double highAltitude = this.IsHighAltitude ? Math.Min(0.5 + squadrons.Count(eq => eq.EquipmentInstance.IsHightAltitudeFighter) * 0.3, 1.2) : 1.0;
                airDefense = (int)(airDefense * squadrons.Select(eq => Calculator.GetAirSuperiorityAirDefenseReconBonus(eq.EquipmentID)).DefaultIfEmpty(1).Max() * highAltitude);


                this.TotalAirSuperiorityAirDefense.Text = airDefense.ToString();
                this.ToolTipInternal.SetToolTip(this.TotalAirSuperiorityAirDefense,
                    string.Format("확보: {0}\r\n우세: {1}\r\n균등: {2}\r\n열세: {3}\r\n",
                        (int)(airDefense / 3.0),
						(int)(airDefense / 1.5),
						Math.Max((int)(airDefense * 1.5 - 1), 0),
						Math.Max((int)(airDefense * 3.0 - 1), 0)));


				// distance
				{
					int minDistance = squadrons
						.Select(eq => eq.EquipmentInstance.AircraftDistance)
						.DefaultIfEmpty()
						.Min();

					int maxReconDistance =
						squadrons.Where(sq => sq.EquipmentInstance.IsReconAircraft)
						.Select(sq => sq.EquipmentInstance.AircraftDistance)
						.DefaultIfEmpty()
						.Max();

					int distance = minDistance;
					if (maxReconDistance > minDistance)
						distance += Math.Min((int)Math.Round(Math.Sqrt(maxReconDistance - minDistance)), 3);

                    this.TotalDistance.Text = distance.ToString();
				}

                this.TotalOrganizationCost.Text = this.Squadrons.Select(sq => sq.OrganizationCost.Tag as int? ?? 0).Sum().ToString();


				Updated(this, new EventArgs());
			}


			void AutoAirSuperiorityMode_Format(object sender, ListControlConvertEventArgs e)
			{
				if (e.DesiredType == typeof(string))
				{
					int val = (int)e.Value;

					if (val == -1)
						e.Value = "설정안함";
					else
						e.Value = Constants.GetAirSuperiority(val);
				}
			}

			void AutoOrganize_Click(object sender, EventArgs e)
			{

				bool isAirDefense = sender == this.AutoOrganizeAirDefense;
				int airSuperiority = (int)this.AutoAirSuperiority.Value;
				switch (this.AutoAirSuperiorityMode.SelectedItem as int? ?? 0)
				{
					case -1:
					default:
						break;
					case 1:
						airSuperiority = airSuperiority * 3;
						break;
					case 2:
						airSuperiority = (int)Math.Ceiling(airSuperiority * 1.5);
						break;
					case 0:
						airSuperiority = (int)Math.Ceiling(airSuperiority / 1.5);
						break;
					case 3:
						airSuperiority = (int)Math.Ceiling(airSuperiority / 3.0);
						break;
					case 4:
						airSuperiority = 0;
						break;
				}
				int distance = (int)this.AutoDistance.Value;


				// 装備済み・ほかの航空隊に配備されている機体以外で編成
				var orgs = AutoOrganize(isAirDefense, airSuperiority, distance,
                    this.Parent.GetUsingEquipments(new int[] { this.BaseAirCorpsID - 1 }).Concat(KCDatabase.Instance.Ships.Values.SelectMany(s => s.AllSlot)));

				if (orgs == null || orgs.All(o => o == null))
				{
					MessageBox.Show("자동편성에 실패했습니다. \r\n조건이 너무 높거나, 항공기가 부족합니다.\r\n",
						"자동편성실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				for (int i = 0; i < this.Squadrons.Length; i++)
				{
					var squi = this.Squadrons[i];

					squi.AircraftCategory.SelectedItem = squi.AircraftCategory.Items.OfType<ComboBoxCategory>().FirstOrDefault(c => c == (orgs[i]?.MasterEquipment?.CategoryType ?? (EquipmentTypes)(-1)));
					squi.Aircraft.SelectedItem = squi.Aircraft.Items.OfType<ComboBoxEquipment>().FirstOrDefault(q => q.UniqueID == (orgs[i]?.MasterID ?? -1));
				}

				System.Media.SystemSounds.Asterisk.Play();
			}

			public void Dispose()
			{
                this.TitleAircraftCategory.Dispose();
                this.TitleAircraft.Dispose();
                this.TitleAircraftCount.Dispose();
                this.TitleAirSuperioritySortie.Dispose();
                this.TitleAirSuperiorityAirDefense.Dispose();
                this.TitleDistance.Dispose();
                this.TitleBomber.Dispose();
                this.TitleTorpedo.Dispose();
                this.TitleOrganizationCost.Dispose();

				foreach (var sq in this.Squadrons)
					sq.Dispose();

                this.TitleTotal.Dispose();
                this.DuplicateCheck.Dispose();
                this.TotalAirSuperioritySortie.Dispose();
                this.TotalAirSuperiorityAirDefense.Dispose();
                this.TotalDistance.Dispose();
                this.TotalOrganizationCost.Dispose();

                this.TitleAutoAirSuperiority.Dispose();
                this.TitleAutoDistance.Dispose();
                this.AutoAirSuperiorityMode.Dispose();
                this.AutoAirSuperiority.Dispose();
                this.AutoDistance.Dispose();
                this.AutoOrganizeSortie.Dispose();
                this.AutoOrganizeAirDefense.Dispose();
			}
		}


		private class ComboBoxCategory
		{

			public readonly EquipmentTypes ID;
			public readonly EquipmentType EquipmentType;

			public ComboBoxCategory(EquipmentTypes id)
			{
                this.ID = id;
                this.EquipmentType = KCDatabase.Instance.EquipmentTypes[(int)id];
			}

			public override string ToString()
			{
				if (this.EquipmentType == null)
					return "(불명)";
				else
					return this.EquipmentType.Name;
			}


			public static implicit operator EquipmentTypes(ComboBoxCategory from)
			{
				return from.ID;
			}

			public static implicit operator EquipmentType(ComboBoxCategory from)
			{
				return from.EquipmentType;
			}


			public static IEnumerable<ComboBoxCategory> GetAllCategories()
			{
				foreach (var category in KCDatabase.Instance.EquipmentTypes.Values)
				{

					// オートジャイロ / 対潜哨戒機 は除外
					if (category.TypeID == (int)EquipmentTypes.Autogyro || category.TypeID == (int)EquipmentTypes.ASPatrol)
						continue;

					var first = KCDatabase.Instance.MasterEquipments.Values
						.Where(eq => !eq.IsAbyssalEquipment)
						.FirstOrDefault(eq => (int)eq.CategoryType == category.TypeID);

					if (first != null && first.IsAircraft)
						yield return new ComboBoxCategory(first.CategoryType);
				}
			}
		}

		private class ComboBoxEquipment
		{

			public readonly int EquipmentID;
			public readonly int Level;
			public readonly int AircraftLevel;
			public readonly EquipmentDataMaster EquipmentInstance;
			public readonly int UniqueID;

			public ComboBoxEquipment()
				: this(-1, 0, 0) { }

			public ComboBoxEquipment(int equipmentID, int level, int aircraftLevel)
			{
                this.EquipmentID = equipmentID;
                this.Level = level;
                this.AircraftLevel = aircraftLevel;
                this.EquipmentInstance = KCDatabase.Instance.MasterEquipments[equipmentID];
                this.UniqueID = -1;
			}

			public ComboBoxEquipment(EquipmentData equipment)
			{
				if (equipment == null)
				{
                    this.EquipmentID = -1;
                    this.Level = 0;
                    this.AircraftLevel = 0;
                    this.EquipmentInstance = null;
                    this.UniqueID = -1;

				}
				else
				{
                    this.EquipmentID = equipment.EquipmentID;
                    this.Level = equipment.Level;
                    this.AircraftLevel = equipment.AircraftLevel;
                    this.EquipmentInstance = KCDatabase.Instance.MasterEquipments[equipment.EquipmentID];
                    this.UniqueID = equipment.MasterID;
				}
			}

			public override string ToString()
			{
				if (this.EquipmentInstance != null)
				{

					var sb = new StringBuilder(this.EquipmentInstance.Name);

					if (this.Level > 0)
						sb.Append("+").Append(this.Level);
					if (this.AircraftLevel > 0)
						sb.Append(" ").Append(EquipmentData.AircraftLevelString[this.AircraftLevel]);

					sb.Append(" :").Append(this.EquipmentInstance.AircraftDistance);
					return sb.ToString();

				}
				else return "(없음)";
			}
		}




		private BaseAirCorpsUI[] BaseAirCorpsUIList;
		private TableLayoutPanel[] TableBaseAirCorpsList;

		public DialogBaseAirCorpsSimulation()
		{
            this.InitializeComponent();

            this.TableBaseAirCorpsList = new[] {
                this.TableBaseAirCorps1,
                this.TableBaseAirCorps2,
                this.TableBaseAirCorps3,
			};


            this.BaseAirCorpsUIList = new BaseAirCorpsUI[this.TableBaseAirCorpsList.Length];
			for (int i = 0; i < this.BaseAirCorpsUIList.Length; i++)
			{
                this.BaseAirCorpsUIList[i] = new BaseAirCorpsUI(i + 1, this);

                this.TableBaseAirCorpsList[i].SuspendLayout();

                this.BaseAirCorpsUIList[i].AddToTable(this.TableBaseAirCorpsList[i]);
                this.BaseAirCorpsUIList[i].Updated += this.BaseAirCorpsUIList_Updated;

                this.TableBaseAirCorpsList[i].CellPaint += this.TableBaseAirCorps_CellPaint;
				ControlHelper.SetTableRowStyles(this.TableBaseAirCorpsList[i], new RowStyle(SizeType.Absolute, 32));
				ControlHelper.SetTableColumnStyles(this.TableBaseAirCorpsList[i], new ColumnStyle(SizeType.Absolute, 72));

				ControlHelper.SetTableColumnStyle(this.TableBaseAirCorpsList[i], 0, new ColumnStyle(SizeType.Absolute, 164));
				ControlHelper.SetTableColumnStyle(this.TableBaseAirCorpsList[i], 1, new ColumnStyle(SizeType.Absolute, 244));

				ControlHelper.SetDoubleBuffered(this.TableBaseAirCorpsList[i]);

                this.TableBaseAirCorpsList[i].ResumeLayout();
			}

		}

		private void DialogBaseAirCorpsSimulation_Load(object sender, EventArgs e)
		{

			if (!KCDatabase.Instance.BaseAirCorps.Any())
			{
				MessageBox.Show("기지항공대 데이터가 없습니다.\r\n출격화면으로 한번 이동해주세요.", "기지 항공대 데이터 없음",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
			}


			// 基地航空隊からのインポート; メニュー設定
			{
				var maps = KCDatabase.Instance.BaseAirCorps.Values
					.Select(b => b.MapAreaID)
					.Distinct()
					.OrderBy(i => i)
					.Select(i => KCDatabase.Instance.MapArea[i])
					.Where(m => m != null);

				foreach (var map in maps)
				{
					int mapAreaID = map.MapAreaID;
					string name = map.Name;

					if (string.IsNullOrWhiteSpace(map.Name) || map.Name == "※")
						name = "이벤트해역";

					var tool = new ToolStripMenuItem(string.Format("#{0} {1}", mapAreaID, name), null,
						new EventHandler((ssender, ee) => this.TopMenu_Edit_MapArea_Click(mapAreaID)));

                    this.TopMenu_Edit_ImportOrganization.DropDownItems.Add(tool);
				}
			}

			// 表示部初期化
			for (int i = 0; i < this.BaseAirCorpsUIList.Length; i++)
			{
				var ui = this.BaseAirCorpsUIList[i];
				var table = this.TableBaseAirCorpsList[i];

				table.SuspendLayout();
				foreach (var squi in ui.Squadrons)
				{
					squi.AircraftCategory.SelectedItem = null;
				}
				table.ResumeLayout();
			}

            this.ClientSize = this.tableLayoutPanel2.PreferredSize;
			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormBaseAirCorps]);

		}

		void BaseAirCorpsUIList_Updated(object sender, EventArgs e)
		{
			// 重複check	

			var sqs = this.BaseAirCorpsUIList.SelectMany(ui => ui.Squadrons.Select(squi => squi.Aircraft.SelectedItem).OfType<ComboBoxEquipment>());

			var sqids = sqs.Where(sq => sq != null && sq.UniqueID > 0);
			var dupes = sqids.GroupBy(sq => sq.UniqueID).Where(g => g.Count() > 1).Select(g => g.Key);

			for (int i = 0; i < this.BaseAirCorpsUIList.Length; i++)
			{
				var ui = this.BaseAirCorpsUIList[i];
				var dupelist = new List<int>();

				for (int x = 0; x < ui.Squadrons.Length; x++)
				{
					var squi = ui.Squadrons[x];

					if (squi.Aircraft.SelectedItem is ComboBoxEquipment selected && dupes.Contains(selected.UniqueID))
						dupelist.Add(x);
				}

				if (dupelist.Any())
				{
					ui.DuplicateCheck.Text = "중복가능 " + string.Join(", ", dupelist.Select(d => "#" + (d + 1)));
				}
				else
				{
					ui.DuplicateCheck.Text = "";
				}
			}
		}



		void TableBaseAirCorps_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
			if (!(e.Column == 9 && e.Row == 2))
				e.Graphics.DrawLine(Pens.Silver, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		private void TopMenu_Edit_MapArea_Click(int mapAreaID)
		{

			for (int i = 0; i < this.BaseAirCorpsUIList.Length; i++)
			{

				var ui = this.BaseAirCorpsUIList[i];

				int id = mapAreaID * 10 + i + 1;
				var baseAirCorps = KCDatabase.Instance.BaseAirCorps[id];

				if (baseAirCorps == null)
				{
					for (int x = 0; x < ui.Squadrons.Length; x++)
					{
						ui.Squadrons[x].AircraftCategory.SelectedItem = null;
						ui.Squadrons[x].Aircraft.SelectedItem = null;
					}
					continue;
				}

				for (int x = 0; x < ui.Squadrons.Length; x++)
				{
					var sq = baseAirCorps[x + 1];

					if (sq.State != 1)
					{
						ui.Squadrons[x].AircraftCategory.SelectedItem = null;
						ui.Squadrons[x].Aircraft.SelectedItem = null;
					}
					else
					{
						ui.Squadrons[x].AircraftCategory.SelectedItem = ui.Squadrons[x].AircraftCategory.Items.OfType<ComboBoxCategory>().FirstOrDefault(cat => cat == sq.EquipmentInstanceMaster.CategoryType);
						ui.Squadrons[x].Aircraft.SelectedItem = ui.Squadrons[x].Aircraft.Items.OfType<ComboBoxEquipment>().FirstOrDefault(eq => eq.UniqueID == sq.EquipmentMasterID);
						ui.Squadrons[x].AircraftCount.Value = sq.AircraftCurrent;
					}
				}
			}

		}

		private void TopMenu_Edit_Clear_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("편성을 모두 지웁니다. \r\n지우시겠습니까?", "편성 초기화", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
				== System.Windows.Forms.DialogResult.Yes)
			{

				for (int i = 0; i < this.BaseAirCorpsUIList.Length; i++)
				{
					var ui = this.BaseAirCorpsUIList[i];
					var table = this.TableBaseAirCorpsList[i];

					table.SuspendLayout();
					foreach (var squi in ui.Squadrons)
					{
						squi.AircraftCategory.SelectedItem = null;
						squi.Aircraft.SelectedItem = null;
					}
					table.ResumeLayout();
				}
			}
		}

		private void DialogBaseAirCorpsSimulation_FormClosed(object sender, FormClosedEventArgs e)
		{
			ResourceManager.DestroyIcon(this.Icon);
		}



		/// <summary>
		/// 自動編成を行います。
		/// </summary>
		/// <param name="isAirDefense">防空かどうか。false なら出撃</param>
		/// <param name="minimumFigherPower">目標制空値。</param>
		/// <param name="minimumDistance">目標戦闘行動半径。</param>
		/// <param name="excludeEquipments">使用しない装備IDのリスト。</param>
		/// <returns>編成結果のリスト[4]。要素に null を含む可能性があります。編成不可能だった場合は null を返します。</returns>
		public static List<EquipmentData> AutoOrganize(bool isAirDefense, int minimumFigherPower, int minimumDistance, IEnumerable<int> excludeEquipments)
		{

			var ret = new List<EquipmentData>(4);

			var available = KCDatabase.Instance.Equipments.Values
				.Where(eq => !excludeEquipments.Contains(eq.MasterID))
				.Select(eq => new { eq, master = eq.MasterEquipment })
				.Where(eqp => SquadronAircraftCategories.Contains(eqp.master.CategoryType));

			var fighter = available
					.Where(eqp => SquadronFighterCategories.Contains(eqp.master.CategoryType));


			if (!isAirDefense)
			{

				// 戦闘機に割くスロット数
				int fighterSlot = -1;

				// 射程拡張が必要か、必要ならいくつ伸ばすか
				int extendedDistance;


				// 攻撃力(仮想的に 雷装+爆装)の高いのを詰め込む
				// 射程拡張も考慮して、 min - 3 まで確保しておく
				var attackerfp = available
					.Where(eq => SquadronAttackerCategories.Contains(eq.master.CategoryType) && eq.master.AircraftDistance >= minimumDistance - 3)
					.Select(eqp => new { eqp.eq, eqp.master, fp = Calculator.GetAirSuperiority(eqp.master.EquipmentID, 18, eqp.eq.AircraftLevel, eqp.eq.Level, 1) })
					.OrderByDescending(eq => eq.master.Torpedo + eq.master.Bomber)
					.ThenBy(f => f.master.AircraftCost)
					.AsEnumerable();


				var fighterfp = fighter.Select(eqp => new { eqp.eq, eqp.master, fp = Calculator.GetAirSuperiority(eqp.master.EquipmentID, 18, eqp.eq.AircraftLevel, eqp.eq.Level, 1) })
					.OrderByDescending(f => f.fp)
					.ThenBy(f => f.master.AircraftCost);

				// 最強の戦闘機を編成すると仮定して、最低何スロット必要かを調べる
				for (extendedDistance = 0; extendedDistance <= 3; extendedDistance++)
				{

					var availfighterfp = fighterfp
						.Where(f => f.master.AircraftDistance + extendedDistance >= minimumDistance);

					for (int i = 0; i <= (extendedDistance > 0 ? 3 : 4); i++)
					{
						if (availfighterfp.Take(i).Sum(f => f.fp) + attackerfp.Take(4 - i - (extendedDistance > 0 ? 1 : 0)).Sum(f => f.fp) >= minimumFigherPower)
						{
							fighterSlot = i;
							break;
						}
					}

					if (fighterSlot != -1)
						break;
				}

				if (fighterSlot == -1)
					return null;        // 編成不可能


				// 攻撃隊の射程調整
				while (attackerfp.Count(f => f.master.AircraftDistance + extendedDistance >= minimumDistance) < (4 - (extendedDistance > 0 ? 1 : 0) - fighterSlot) &&
					extendedDistance < 3)
					extendedDistance++;


				// 射程拡張が必要なら適切な偵察機を載せる
				if (extendedDistance > 0)
				{
					// 延長距離 = sqrt( ( 偵察機距離 - その他距離 ) )
					// 偵察機距離 = 延長距離^2 + その他距離

					int reconDistance = extendedDistance * extendedDistance + (minimumDistance - extendedDistance);

					var recon = available.Where(eqp => SquadronReconCategories.Contains(eqp.master.CategoryType) &&
					   eqp.master.AircraftDistance >= reconDistance)
						.OrderBy(eqp => eqp.master.AircraftCost)
						.FirstOrDefault();

					if (recon == null)
						return null;    // 編成不可能

					ret.Add(recon.eq);
				}


				attackerfp = attackerfp
					.Where(f => f.master.AircraftDistance + extendedDistance >= minimumDistance)
					.Take(4 - ret.Count - fighterSlot);
				minimumFigherPower -= attackerfp.Sum(f => f.fp);


				if (fighterSlot > 0)
				{
					// 射程が足りている戦闘機
					var fighterfpdist = fighterfp.Where(f => f.master.AircraftDistance + extendedDistance >= minimumDistance);
					int estimatedIndex = fighterfpdist.TakeWhile(f => f.fp >= minimumFigherPower / fighterSlot).Count();

					// fighterfpdist は 制空値が高い順 に並んでいるので、
					// 下から窓をずらしていけばいい感じのが出る（はず）
					// 少なくとも先頭(制空値最高)が 目標 / スロット 以下だと絶対に満たせないので、そこから始める
					for (int i = Math.Min(estimatedIndex, fighterfpdist.Count() - fighterSlot); i >= 0; i--)
					{

						var org = fighterfpdist.Skip(i).Take(fighterSlot);
						if (org.Sum(f => f.fp) >= minimumFigherPower)
						{
							ret.AddRange(org.Select(f => f.eq));
							break;
						}
					}
				}

				ret.AddRange(attackerfp.Select(f => f.eq));


			}
			else
			{
				// 防空

				// とりあえず最大補正の偵察機を突っ込む
				var recons = available
					.Where(eq => SquadronReconCategories.Contains(eq.master.CategoryType))
					.Select(eq => new { eq.eq, eq.master, bonus = Calculator.GetAirSuperiorityAirDefenseReconBonus(eq.master.EquipmentID) })
					.OrderByDescending(f => f.bonus)
					.ThenBy(eq => eq.master.AircraftCost);

				if (recons.Any())
				{
					ret.Add(recons.First().eq);
					minimumFigherPower = (int)Math.Ceiling(minimumFigherPower / recons.First().bonus);
				}

				var fighterfp = fighter
					.Select(eqp => new { eqp.eq, eqp.master, fp = Calculator.GetAirSuperiority(eqp.master.EquipmentID, 18, eqp.eq.AircraftLevel, eqp.eq.Level, 2) })
					.OrderByDescending(f => f.fp)
					.ThenBy(f => f.master.AircraftCost);

				int estimatedIndex = fighterfp.TakeWhile(f => f.fp >= minimumFigherPower / (4 - ret.Count)).Count();

				// fighterfp は 制空値が高い順 に並んでいるので、
				// 下から窓をずらしていけばいい感じのが出る（はず）
				for (int i = Math.Min(estimatedIndex, fighterfp.Count() - (4 - ret.Count)); i >= 0; i--)
				{

					var org = fighterfp.Skip(i).Take(4 - ret.Count);
					if (org.Sum(f => f.fp) >= minimumFigherPower)
					{
						ret.AddRange(org.Select(f => f.eq));
						break;
					}
				}

				if (ret.Count == (recons.Any() ? 1 : 0))        // 戦闘機の配備に失敗
					return null;
			}

			while (ret.Count < 4)
				ret.Add(null);
			return ret;
		}


		/// <summary>
		/// 現在UI上に配備されている装備ID群を求めます。
		/// </summary>
		/// <param name="except">除外する航空隊のインデックス。</param>
		private IEnumerable<int> GetUsingEquipments(IEnumerable<int> except)
		{

			foreach (var corpsui in this.BaseAirCorpsUIList.Where((b, i) => !except.Contains(i)))
			{
				foreach (var squi in corpsui.Squadrons)
				{


					if (squi.Aircraft.SelectedItem is ComboBoxEquipment eq && eq.UniqueID != -1)
					{
						yield return eq.UniqueID;
					}
				}
			}
		}

        private void TopMenu_Settings_HighAltitude_Click(object sender, EventArgs e)
        {

        }

        private void 대고고도폭격ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
