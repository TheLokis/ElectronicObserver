using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Data;
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
	public partial class DialogExpChecker : Form
	{
		private static readonly string DefaultTitle = "경험치 계산기";
		private DataGridViewCellStyle CellStyleModernized;

        public static Dictionary<string, int> SortieExpTable = new Dictionary<string, int>
        {
            {"1-1", 30}, {"1-2", 50}, {"1-3", 80}, {"1-4", 100}, {"1-5", 150},
            {"2-1", 120}, {"2-2", 150}, {"2-3", 200},{"2-4", 300}, {"2-5", 250},
            {"3-1", 310}, {"3-2", 320}, {"3-3", 330}, {"3-4", 350}, {"3-5", 400},
            {"4-1", 310}, {"4-2", 320}, {"4-3", 330}, {"4-4", 340},
            {"5-1", 360}, {"5-2", 380}, {"5-3", 400}, {"5-4", 420}, {"5-5", 450},
            {"6-1", 400}, {"6-2", 420},
        };

        private int DefaultShipID = -1;



		private class ComboShipData
		{
			public ShipData Ship;

			public ComboShipData(ShipData ship)
			{
                this.Ship = ship;
			}

			public override string ToString() => $"{this.Ship.MasterShip.ShipTypeName} {this.Ship.NameWithLevel}";
		}

		private class ASWEquipmentData
		{
			public int ID;
			public int ASW;
			public string Name;
			public bool IsSonar;
			public int Count;

			public override string ToString() => this.Name;
		}






		public DialogExpChecker()
		{
            this.InitializeComponent();

            this.CellStyleModernized = new DataGridViewCellStyle(this.ColumnLevel.DefaultCellStyle);
            this.CellStyleModernized.BackColor =
            this.CellStyleModernized.SelectionBackColor = Color.LightGreen;

		}

		public DialogExpChecker(int shipID) : this()
		{
            this.DefaultShipID = shipID;
            this.Text = DefaultTitle;
		}

		private void DialogExpChecker_Load(object sender, EventArgs e)
		{
			var ships = KCDatabase.Instance.Ships.Values;

			if (!ships.Any())
			{
				MessageBox.Show("함선 데이터가 존재하지 않습니다.\r\n모항화면으로 한번 이동해주세요.", "함선 데이터 없음", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
				return;
			}
            try
            {
                if (Utility.Configuration.Config.Control.MapSelect.Equals("") || Utility.Configuration.Config.Control.MapSelect == null)
                {
                    Utility.Configuration.Config.Control.MapSelect = "1-1";
                }
            } catch
            {
                Utility.Configuration.Config.Control.MapSelect = "1-1";
            }

            try
            {
                if (Utility.Configuration.Config.Control.Rank.Equals("") || Utility.Configuration.Config.Control.Rank == null)
                {
                    Utility.Configuration.Config.Control.Rank = "S";
                }
            } catch
            {
                Utility.Configuration.Config.Control.Rank = "S";
            }
            /*
            MapSelect.SelectedItem = MapSelect.Items.OfType<String>().FirstOrDefault(x => x.Equals(Utility.Configuration.Config.Control.MapSelect));
            ExpUnit.Value = Utility.Configuration.Config.Control.ExpCheckerExpUnit;
            MVP_Check.Checked = Utility.Configuration.Config.Control.MVPCheck;
            FlagShip_Check.Checked = Utility.Configuration.Config.Control.FlagShipCheck;
            Rank_List.SelectedItem = Rank_List.Items.OfType<String>().FirstOrDefault(x => x.Equals(Utility.Configuration.Config.Control.Rank));
            ExpControl.Checked = Utility.Configuration.Config.Control.ExpManual;
            ExpUnit.ReadOnly = !Utility.Configuration.Config.Control.ExpManual;
            MapSelect.Enabled = !ExpControl.Checked;
            MVP_Check.Enabled = !ExpControl.Checked;
            FlagShip_Check.Enabled = !ExpControl.Checked;
            Rank_List.Enabled = !ExpControl.Checked;
            label4.Enabled = !ExpControl.Checked;
            label5.Enabled = !ExpControl.Checked;
            */
            this.LabelAlert.Text = "";
            this.SearchInFleet_CheckedChanged(this, new EventArgs());
            this.ExpUnit.Value = Utility.Configuration.Config.Control.ExpCheckerExpUnit;

            if (this.DefaultShipID != -1)
                this.TextShip.SelectedItem = this.TextShip.Items.OfType<ComboShipData>().FirstOrDefault(f => f.Ship.MasterID == this.DefaultShipID);


			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormExpChecker]);
		}

		private void DialogExpChecker_FormClosed(object sender, FormClosedEventArgs e)
		{/*
			Utility.Configuration.Config.Control.ExpCheckerExpUnit = (int)ExpUnit.Value;
            Utility.Configuration.Config.Control.MapSelect = (string)MapSelect.SelectedItem;
            Utility.Configuration.Config.Control.MVPCheck = MVP_Check.Checked;
            Utility.Configuration.Config.Control.FlagShipCheck = FlagShip_Check.Checked;
            Utility.Configuration.Config.Control.Rank = (string)Rank_List.SelectedItem;
            Utility.Configuration.Config.Control.ExpManual = ExpControl.Checked;*/
            ResourceManager.DestroyIcon(this.Icon);
		}

        private void UpdateProperty()
        {
            float ExpValue = (float)this.ExpUnit.Value;
            /*
            ExpValue = SortieExpTable[(string)MapSelect.SelectedItem];
            if (MVP_Check.Checked) ExpValue *= 2;
            if (FlagShip_Check.Checked) { ExpValue *= 1.5f; }

            switch (Rank_List.Text)
            {
                case "S":
                    ExpValue *= 1.2f;
                    break;
                case "C":
                    ExpValue *= 0.8f;
                    break;
                case "D":
                    ExpValue *= 0.7f;
                    break;
                case "E":
                    ExpValue *= 0.5f;
                    break;
                default:
                    break;
            }
            */
            this.ExpUnit.Value = (decimal)ExpValue;
        }

		private void UpdateLevelView()
		{
            var selectedShip = (this.TextShip.SelectedItem as ComboShipData)?.Ship;

			if (selectedShip == null)
			{
				System.Media.SystemSounds.Asterisk.Play();
				return;
			}


            this.LevelView.SuspendLayout();

            this.LevelView.Rows.Clear();


			// 空母系は面倒なので省略
			int openingASWborder = selectedShip.MasterShip.ShipType == ShipTypes.Escort ? 60 : 100;

			var ASWEquipmentPairs = new Dictionary<int, string>();
			if (this.ShowAllASWEquipments.Checked)
			{

				var had = KCDatabase.Instance.Equipments.Values
					.Where(eq => eq.MasterEquipment.CategoryType == EquipmentTypes.Sonar || eq.MasterEquipment.CategoryType == EquipmentTypes.DepthCharge)
					.GroupBy(eq => eq.EquipmentID)
					.Select(g => new ASWEquipmentData { ID = g.Key, ASW = g.First().MasterEquipment.ASW, Name = g.First().MasterEquipment.Name, IsSonar = g.First().MasterEquipment.IsSonar, Count = g.Count() })
					.Concat(new[] { new ASWEquipmentData { ID = -1, ASW = 0, Name = "", Count = 99, IsSonar = false } })
					.OrderByDescending(a => a.ASW)
					.ToArray();

				var stack = Enumerable.Repeat(0, selectedShip.SlotSize).ToArray();

				var pair = new Dictionary<int, List<ASWEquipmentData[]>>();

				if (had.Length > 0 && stack.Length > 0)
				{

					while (stack[0] != -1)
					{
						var convert = stack.Select(i => had[i]).ToArray();


						if (convert.Any(c => c.IsSonar) && stack.GroupBy(s => s).All(s => had[s.Key].Count >= s.Count()))
						{
							int aswsum = convert.Sum(c => c.ASW);

							if (!pair.ContainsKey(aswsum))
								pair.Add(aswsum, new List<ASWEquipmentData[]>() { convert });
							else
								pair[aswsum].Add(convert);
						}

						for (int p = stack.Length - 1; p >= 0; p--)
						{
							stack[p]++;
							if (stack[p] < had.Length)
								break;
							stack[p] = -1;
						}
						for (int p = 1; p < stack.Length; p++)
						{
							if (stack[p] == -1)
								stack[p] = stack[p - 1];
						}
					}
				}

				foreach (var x in pair)
				{
					// 要するに下のようなフォーマットにする
					ASWEquipmentPairs.Add(openingASWborder - x.Key,
						string.Join(", ",
							x.Value.OrderBy(a => a.Count(b => b.ID > 0))
								.Select(a => $"[{string.Join(", ", a.Where(b => b.ID > 0).GroupBy(b => b.ID).Select(b => b.Count() == 1 ? b.First().Name : $"{b.First().Name}x{b.Count()}"))}]")));
				}
			}
			else
			{
				if (selectedShip.SlotSize >= 4)
				{
					ASWEquipmentPairs.Add(openingASWborder - 51, "[4식소나x3, 시제15cm9연장대잠분진포]");
					ASWEquipmentPairs.Add(openingASWborder - 48, "[4식소나x4]");
					ASWEquipmentPairs.Add(openingASWborder - 44, "[4식소나x3, 3식폭뢰투사기]");
				}
				if (selectedShip.SlotSize >= 3)
				{
                    ASWEquipmentPairs.Add(openingASWborder - 42, "[4식소나, HF/DF + Type144/147 ASDIC, 시제15cm9연장대잠분진포]");
                    ASWEquipmentPairs.Add(openingASWborder - 40, "[4식소나, Type144/147 ASDIC, 시제15cm9연장대잠분진포]");
                    ASWEquipmentPairs.Add(openingASWborder - 39, "[4식소나x2, 시제15cm9연장대잠분진포]");
                    ASWEquipmentPairs.Add(openingASWborder - 36, "[4식소나x3]");
					ASWEquipmentPairs.Add(openingASWborder - 32, "[4식소나x2, 3식폭뢰투사기]");
					ASWEquipmentPairs.Add(openingASWborder - 28, "[3식소나x2, 3식폭뢰투사기]");
					ASWEquipmentPairs.Add(openingASWborder - 27, "[4식소나, 3식폭뢰투사기, 2식폭뢰]");
                }
				if (selectedShip.SlotSize >= 2)
				{
                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 30))
                        ASWEquipmentPairs[openingASWborder - 30] += ", [HF/DF + Type144/147 ASDIC, 시제15cm9연장대잠분진포]";
                    else
                        ASWEquipmentPairs.Add(openingASWborder - 30, "[HF/DF + Type144/147 ASDIC, 시제15cm9연장대잠분진포]");

                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 28))
                    {
                        ASWEquipmentPairs[openingASWborder - 28] += ", [Type144/147 ASDIC, 시제15cm9연장대잠분진포]";
                        ASWEquipmentPairs[openingASWborder - 28] += ", [Type144/147 ASDIC, HF/DF + Type144/147 ASDIC]";
                    }
                    else
                    {
                        ASWEquipmentPairs.Add(openingASWborder - 28, "[Type144/147 ASDIC, 시제15cm9연장대잠분진포]");
                        ASWEquipmentPairs[openingASWborder - 28] += ", [Type144/147 ASDIC, HF/DF + Type144/147 ASDIC]";
                    }

                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 27))
                    {
                        ASWEquipmentPairs[openingASWborder - 27] += ", [4식소나, 시제15cm9연장대잠분진포]";
                        ASWEquipmentPairs[openingASWborder - 27] += ", [4식소나, HF/DF + Type144/147 ASDIC]";
                    }
                    else
                    {
                        ASWEquipmentPairs.Add(openingASWborder - 27, "[4식소나, 시제15cm9연장대잠분진포]");
                        ASWEquipmentPairs[openingASWborder - 27] += ", [4식소나, HF/DF + Type144/147 ASDIC]";
                    }

                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 26))
                    {
                        ASWEquipmentPairs[openingASWborder - 26] += ", [HF/DF + Type144/147 ASDIC, Type124 ASDIC]";
                        ASWEquipmentPairs[openingASWborder - 26] += ", [시제15cm9연장대잠분진포, Type124 ASDIC]";
                    }
                    else
                    {
                        ASWEquipmentPairs.Add(openingASWborder - 26, "[HF/DF + Type144/147 ASDIC, Type124 ASDIC]");
                        ASWEquipmentPairs[openingASWborder - 26] += ", [시제15cm9연장대잠분진포, Type124 ASDIC]";
                    }

                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 25))
                    {
                        ASWEquipmentPairs[openingASWborder - 25] += ", [Type144/147 ASDIC, 3식폭뢰투사기 집중배치]";
                        ASWEquipmentPairs[openingASWborder - 25] += ", [Type144/147 ASDIC, 4식소나]";
                    }
                    else
                    {
                        ASWEquipmentPairs.Add(openingASWborder - 25, "[Type144/147 ASDIC, 3식폭뢰투사기 집중배치]");
                        ASWEquipmentPairs[openingASWborder - 25] += ", [Type144/147 ASDIC, 4식소나]";
                    }

                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 24))
                    {
                        ASWEquipmentPairs[openingASWborder - 24] += ", [4식소나x2]";
                        ASWEquipmentPairs[openingASWborder - 24] += ", [4식소나x2]";
                    }
                    else
                    {
                        ASWEquipmentPairs.Add(openingASWborder - 24, "[4식소나x2]");
                        ASWEquipmentPairs[openingASWborder - 24] += ", [4식소나x2]";
                    }

                    if (ASWEquipmentPairs.ContainsKey(openingASWborder - 22))
                    {
                        ASWEquipmentPairs[openingASWborder - 22] += ", [Type124 ASDICx2]";
                        ASWEquipmentPairs[openingASWborder - 22] += ", [Type124 ASDICx2]";
                    }
                    else
                    {
                        ASWEquipmentPairs.Add(openingASWborder - 22, "[Type124 ASDICx2]");
                        ASWEquipmentPairs[openingASWborder - 22] += ", [Type124 ASDICx2]";
                    }

                    ASWEquipmentPairs.Add(openingASWborder - 20, "[4식소나, 3식폭뢰투사기]");
					ASWEquipmentPairs.Add(openingASWborder - 18, "[3식소나, 3식폭뢰투사기]");
				}
				ASWEquipmentPairs.Add(openingASWborder - 12, "[4식소나]");
			}


            var aswdata = selectedShip.MasterShip.ASW;
            int aswmin = aswdata.Minimum;
            int aswmax = aswdata.Maximum;
            int aswmod = (int)this.ASWModernization.Value;
			int currentlv = selectedShip.Level;
			int minlv = this.ShowAllLevel.Checked ? 1 : (currentlv + 1);
			int unitexp = Math.Max((int)this.ExpUnit.Value, 1);
			var remodelLevelTable = this.GetRemodelLevelTable(selectedShip.MasterShip);

            if (!aswdata.IsAvailable)
                this.LabelAlert.Text = "＊대잠 값을 알 수 없기 때문에 예측을 할 수 없습니다.";
            else if (!aswdata.IsDetermined)
                this.LabelAlert.Text = "＊대잠 값이 미확정이어서, 예측값이 정확하지 않습니다.";
            else
                this.LabelAlert.Text = "";

            var rows = new DataGridViewRow[ExpTable.ShipMaximumLevel - (minlv - 1)];

			for (int lv = minlv; lv <= ExpTable.ShipMaximumLevel; lv++)
			{
				int asw = aswmin + ((aswmax - aswmin) * lv / 99) + aswmod;

				int needexp = ExpTable.ShipExp[lv].Total - selectedShip.ExpTotal;

				var row = new DataGridViewRow();
				row.CreateCells(this.LevelView);
				row.SetValues(
					lv,
					Math.Max(needexp, 0),
					Math.Max((int)Math.Ceiling((double)needexp / unitexp), 0),
!aswdata.IsAvailable ? -1 : asw,
                    !aswdata.IsAvailable ? "-" : (ASWEquipmentPairs.Where(k => asw >= k.Key).OrderByDescending(p => p.Key).FirstOrDefault().Value ?? "-")

                    );


				if (remodelLevelTable.Contains(lv))
				{
					row.Cells[this.ColumnLevel.Index].Style = this.CellStyleModernized;
				}

				rows[lv - minlv] = row;
			}

            this.LevelView.Rows.AddRange(rows);

            this.LevelView.ResumeLayout();


            this.Text = DefaultTitle + " - " + selectedShip.NameWithLevel;
            this.GroupExp.Text = $"{selectedShip.NameWithLevel}: Exp. {selectedShip.ExpTotal}, 대잠 {selectedShip.ASWBase} (현재개수+{selectedShip.ASWModernized})";
		}

        private void LevelView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == this.ColumnASW.Index)
            {
                e.Value = (int)e.Value == -1 ? "???" : e.Value.ToString();
                e.FormattingApplied = true;
            }
        }

        private void SearchInFleet_CheckedChanged(object sender, EventArgs e)
		{
			var ships = KCDatabase.Instance.Ships.Values;

            this.TextShip.Items.Clear();

            //작업 체크용 1

			if (this.SearchInFleet.Checked)
			{
                this.TextShip.Items.AddRange(ships
					.Where(s => s.Fleet != -1)
					.OrderBy(s => s.FleetWithIndex)
					.Select(s => new ComboShipData(s))
					.ToArray());
			}
			else
			{
                if(Utility.Configuration.Config.FormFleet.ExpCheckerOption == 1)
                {
                    this.TextShip.Items.AddRange(ships
    .OrderBy(s => s.MasterShip.ShipType)
    .ThenByDescending(s => s.Level)
    .Select(s => new ComboShipData(s))
    .ToArray());
                } else
                {
                    this.TextShip.Items.AddRange(ships
    .OrderByDescending(s => s.Level)
    .Select(s => new ComboShipData(s))
    .ToArray());
                }


			}

            this.TextShip.SelectedIndex = 0;
		}

		private void TextShip_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedShip = (this.TextShip.SelectedItem as ComboShipData)?.Ship;

			if (selectedShip == null)
				return;

            this.ASWModernization.Value = selectedShip.ASWModernized;

            this.UpdateLevelView();
		}

		private void ShowAllLevel_CheckedChanged(object sender, EventArgs e)
		{
            this.UpdateLevelView();
		}

		private void ShowAllASWEquipments_CheckedChanged(object sender, EventArgs e)
		{
            this.UpdateLevelView();
		}

		private void ExpUnit_ValueChanged(object sender, EventArgs e)
		{
            this.UpdateLevelView();
		}

		private void ASWModernization_ValueChanged(object sender, EventArgs e)
		{
            this.UpdateLevelView();
		}

        // 작업 체크용 3
        private void FlagShip_CheckChanged(object sender, EventArgs e)
        {
            this.UpdateProperty();
        }

        private void MVP_CheckChanged(object sender, EventArgs e)
        {
            this.UpdateProperty();
        }

        private void Rank_IndexChanged(object sender, EventArgs e)
        {
            this.UpdateProperty();
        }

        private void ExpControl_CheckChanged(object sender, EventArgs e)
        {/*
            Utility.Configuration.Config.Control.ExpManual = ExpControl.Checked;
            ExpControl.Checked = ExpControl.Checked;
            ExpUnit.ReadOnly = !ExpControl.Checked;
            MapSelect.Enabled = !ExpControl.Checked;
            MVP_Check.Enabled = !ExpControl.Checked;
            FlagShip_Check.Enabled = !ExpControl.Checked;
            Rank_List.Enabled = !ExpControl.Checked;
            label4.Enabled = !ExpControl.Checked;
            label5.Enabled = !ExpControl.Checked;*/
        }

        private void MapSelect_IndexChanged(object sender, EventArgs e)
        {
            this.UpdateProperty();
        }

        private int[] GetRemodelLevelTable(ShipDataMaster ship)
		{
			while (ship.RemodelBeforeShip != null)
				ship = ship.RemodelBeforeShip;

			var list = new LinkedList<int>();

			while (ship != null)
			{
				list.AddLast(ship.RemodelAfterLevel);
				ship = ship.RemodelAfterShip;
				if (list.Last() >= ship.RemodelAfterLevel)
					break;
			}

			return list.ToArray();
		}


	}
}
