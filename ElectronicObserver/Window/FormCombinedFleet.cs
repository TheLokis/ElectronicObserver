using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Dialog;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window
{
    public partial class FormCombinedFleet : DockContent
    {
        private bool _isRemodeling = false;

        private class TableFleetControl : IDisposable
        {
            public Label Name;
            public FleetState State;
            public ToolTip ToolTipInfo;
            public FormCombinedFleet Parent;
            public ImageLabel TotalAirSuperiority;
            public ImageLabel Fleet1AirSuperiority;
            public ImageLabel Fleet2AirSuperiority;
            public ImageLabel SearchingAbility;
            public ImageLabel AntiAirPower;

            public int BranchWeight { get; private set; } = 1;

            public TableFleetControl(FormCombinedFleet parent)
            {
                this.Parent = parent;
                #region Initialize

                this.Name = new Label
                {
                    Text = "[연합함대]",
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    UseMnemonic = false,
                    Padding = new Padding(0, 1, 0, 1),
                    Margin = new Padding(2, 0, 2, 0),
                    AutoSize = true,
                    //Name.Visible = false;
                    Cursor = Cursors.Help
                };

                this.State = new FleetState
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    Padding = new Padding(),
                    Margin = new Padding(),
                    AutoSize = true
                };

                this.TotalAirSuperiority = new ImageLabel
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    ImageList = ResourceManager.Instance.Equipments,
                    ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter,
                    Padding = new Padding(2, 2, 2, 2),
                    Margin = new Padding(2, 0, 2, 0),
                    AutoSize = true
                };

                this.Fleet1AirSuperiority = new ImageLabel
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    ImageList = ResourceManager.Instance.Equipments,
                    ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter,
                    Padding = new Padding(2, 2, 2, 2),
                    Margin = new Padding(2, 0, 2, 0),
                    AutoSize = true
                };

                this.Fleet2AirSuperiority = new ImageLabel
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    ImageList = ResourceManager.Instance.Equipments,
                    ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter,
                    Padding = new Padding(2, 2, 2, 2),
                    Margin = new Padding(2, 0, 2, 0),
                    AutoSize = true
                };

                this.SearchingAbility = new ImageLabel
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    ImageList = ResourceManager.Instance.Equipments,
                    ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedRecon,
                    Padding = new Padding(2, 2, 2, 2),
                    Margin = new Padding(2, 0, 2, 0),
                    AutoSize = true
                };
                this.SearchingAbility.Click += (sender, e) => this.SearchingAbility_Click(sender, e);

                this.AntiAirPower = new ImageLabel
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    ImageList = ResourceManager.Instance.Equipments,
                    ImageIndex = (int)ResourceManager.EquipmentContent.HighAngleGun,
                    Padding = new Padding(2, 2, 2, 2),
                    Margin = new Padding(2, 0, 2, 0),
                    AutoSize = true
                };

                this.ConfigurationChanged(parent);

                this.ToolTipInfo = parent.ToolTipInfo;

                #endregion
            }

            private void SearchingAbility_Click(object sender, EventArgs e)
            {
                this.BranchWeight--;
                if (this.BranchWeight <= 0)
                    this.BranchWeight = 4;

                this.Update();
            }

            public TableFleetControl(FormCombinedFleet parent, TableLayoutPanel table, int row)
                : this(parent)
            {
                this.Parent = parent;
                this.AddToTable(table, row);
            }

            public string GetAirSuperiorityString()
            {
                switch (Utility.Configuration.Config.FormFleet.AirSuperiorityMethod)
                {
                    case 0:
                    default:
                        return (Calculator.GetAirSuperiorityIgnoreLevel(KCDatabase.Instance.Fleet[1]) + Calculator.GetAirSuperiorityIgnoreLevel(KCDatabase.Instance.Fleet[2])).ToString();
                    case 1:
                        {
                            int min = Calculator.GetAirSuperiority(KCDatabase.Instance.Fleet[1], false) + Calculator.GetAirSuperiority(KCDatabase.Instance.Fleet[2], false);
                            int max = Calculator.GetAirSuperiority(KCDatabase.Instance.Fleet[1], true) + Calculator.GetAirSuperiority(KCDatabase.Instance.Fleet[2], true);

                            if (Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange && min < max)
                                return string.Format("{0} ～ {1}", min, max);
                            else
                                return min.ToString();
                        }
                }
            }

            public void Update()
            {
                KCDatabase db = KCDatabase.Instance;

                if (db.Fleet[1] == null) return;

                FleetData fleet1 = db.Fleet[1];
                FleetData fleet2 = db.Fleet[2];

                {
                    var members = db.Fleet[1].MembersInstance.Where(s => s != null).Union(db.Fleet[2].MembersInstance.Where(s => s != null));

                    int levelSum = members.Sum(s => s.Level);

                    int fueltotal   = members.Sum(s => Math.Max((int)Math.Floor(s.FuelMax * (s.IsMarried ? 0.85 : 1.00)), 1));
                    int ammototal   = members.Sum(s => Math.Max((int)Math.Floor(s.AmmoMax * (s.IsMarried ? 0.85 : 1.00)), 1));
                    int fuelunit    = members.Sum(s => Math.Max((int)Math.Floor(s.FuelMax * 0.2 * (s.IsMarried ? 0.85 : 1.00)), 1));
                    int ammounit    = members.Sum(s => Math.Max((int)Math.Floor(s.AmmoMax * 0.2 * (s.IsMarried ? 0.85 : 1.00)), 1));
                    int speed       = members.Select(s => s.Speed).DefaultIfEmpty(20).Min();

                    int tp = Calculator.GetTPDamage(fleet1) + Calculator.GetTPDamage(fleet2);

                    var transport   = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.CategoryType == EquipmentTypes.TransportContainer));
                    var landing     = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.CategoryType == EquipmentTypes.LandingCraft || eq?.CategoryType == EquipmentTypes.SpecialAmphibiousTank));

                    this.ToolTipInfo.SetToolTip(this.Name, string.Format(
                        "Lv합계: {0} / 평균: {1:0.00}\r\n" +
                        "{2}함대\r\n" +
                        "총 화력 {3} / 대공 {4} / 대잠 {5} / 색적 {6}\r\n" +
                        "드럼통: {7}개 ({8}척)\r\n" +
                        "수송량(TP): S {9} / A {10}\r\n" +
                        "소비자원: 연료 {11} / 탄약 {12}\r\n" +
                        "(1전투당 연료 {13} / 탄약 {14})",
                        levelSum, 
                        (double)levelSum / Math.Max(fleet1.Members.Count(id => id != -1) + fleet2.Members.Count(id => id != -1), 1),
                        Constants.GetSpeed(speed),
                        members.Sum(s => s.FirepowerTotal),
                        members.Sum(s => s.AATotal),
                        members.Sum(s => s.ASWTotal),
                        members.Sum(s => s.LOSTotal),
                        transport.Sum(),
                        transport.Count(i => i > 0),
                        tp,
                        (int)(tp * 0.7),
                        fueltotal,
                        ammototal,
                        fuelunit,
                        ammounit
                        ));
                }

                this.State.UpdateFleetState(fleet1, this.ToolTipInfo);

                //制空戦力計算	
                {
                    int airSuperiority = fleet1.GetAirSuperiority() + fleet2.GetAirSuperiority();
                    bool includeLevel = Utility.Configuration.Config.FormFleet.AirSuperiorityMethod == 1;
                    this.TotalAirSuperiority.Text   = this.GetAirSuperiorityString();
                    this.Fleet1AirSuperiority.Text  = fleet1.GetAirSuperiorityString();
                    this.Fleet2AirSuperiority.Text  = fleet2.GetAirSuperiorityString();
                    this.ToolTipInfo.SetToolTip(this.TotalAirSuperiority,
                        string.Format("확보: {0}\r\n우세: {1}\r\n균등: {2}\r\n열세: {3}\r\n({4}: {5})\r\n",
                        (int)(airSuperiority / 3.0),
                        (int)(airSuperiority / 1.5),
                        Math.Max((int)(airSuperiority * 1.5 - 1), 0),
                        Math.Max((int)(airSuperiority * 3.0 - 1), 0),
                        includeLevel ? "숙련도없음" : "숙련도있음",
                        includeLevel ? Calculator.GetAirSuperiorityIgnoreLevel(fleet1) + Calculator.GetAirSuperiorityIgnoreLevel(fleet2) : 
                                        Calculator.GetAirSuperiority(fleet1) + Calculator.GetAirSuperiority(fleet2)));
                }


                //索敵能力計算
                this.SearchingAbility.Text = String.Format(this.BranchWeight > 1 ? "(" + this.BranchWeight + ") {0:f2}" : "{0:f2}",
                                            Math.Floor((fleet1.GetSearchingAbility(this.BranchWeight) + fleet2.GetSearchingAbility(this.BranchWeight)) * 100) / 100);
                {
                    StringBuilder sb = new StringBuilder();
                    double probStart    = fleet1.GetContactProbability() + fleet2.GetContactProbability();
                    var probSelect      = fleet1.GetContactSelectionProbability().Union(fleet2.GetContactSelectionProbability());

                    sb.AppendFormat("신판정식(33) 분기점계수: {0}\r\n　(클릭으로전환)\r\n\r\n촉접률: \r\n　확보 {1:p1} / 우세 {2:p1}\r\n",
                        this.BranchWeight,
                        probStart,
                        probStart * 0.6);

                    if (probSelect.Count > 0)
                    {
                        sb.AppendLine("촉접선택율: ");

                        foreach (var p in probSelect.OrderBy(p => p.Key))
                        {
                            sb.AppendFormat("　명중{0} : {1:p1}\r\n", p.Key, p.Value);
                        }
                    }

                    this.ToolTipInfo.SetToolTip(this.SearchingAbility, sb.ToString());
                }

                // 対空能力計算
                {
                    var sb = new StringBuilder();
                    double lineahead    = (Calculator.GetAdjustedFleetAAValue(fleet1, 1) * Calculator.GetAirDefenseCombinedFleetCoefficient(1)) 
                                        + (Calculator.GetAdjustedFleetAAValue(fleet2, 1) * Calculator.GetAirDefenseCombinedFleetCoefficient(2));

                    this.AntiAirPower.Text = lineahead.ToString("0.0");

                    sb.AppendFormat("함대방공\r\n단종진: {0:0.0} / 복종진: {1:0.0} / 윤형진: {2:0.0}\r\n",
                        lineahead,
                        (Calculator.GetAdjustedFleetAAValue(fleet1, 2) * Calculator.GetAirDefenseCombinedFleetCoefficient(1) + 
                        Calculator.GetAdjustedFleetAAValue(fleet2, 2) * Calculator.GetAirDefenseCombinedFleetCoefficient(2)),
                        Calculator.GetAdjustedFleetAAValue(fleet1, 3) * Calculator.GetAirDefenseCombinedFleetCoefficient(1) +
                        Calculator.GetAdjustedFleetAAValue(fleet2, 3) * Calculator.GetAirDefenseCombinedFleetCoefficient(2));

                    this.ToolTipInfo.SetToolTip(this.AntiAirPower, sb.ToString());
                }
            }

            public void AddToTable(TableLayoutPanel table, int row)
            {
                table.SuspendLayout();
                if (row == 0)
                {
                    table.Controls.Add(this.Name, 0, 0);
                    table.Controls.Add(this.State, 1, 0);
                    table.Controls.Add(this.TotalAirSuperiority, 2, 0);
                    table.Controls.Add(this.Fleet1AirSuperiority, 3, 0);
                    table.Controls.Add(this.Fleet2AirSuperiority, 4, 0);
                    table.Controls.Add(this.SearchingAbility, 5, 0);
                    table.Controls.Add(this.AntiAirPower, 6, 0);
                }
                table.ResumeLayout();
            }

            public void Refresh()
            {
                this.State.RefreshFleetState();
            }

            public void ConfigurationChanged(FormCombinedFleet parent)
            {
                this.State.Font = parent.MainFont;
                this.State.RefreshFleetState();

                this.Name.Font = parent.MainFont;
                this.TotalAirSuperiority.Font = parent.MainFont;
                this.Fleet1AirSuperiority.Font = parent.MainFont;
                this.Fleet2AirSuperiority.Font = parent.MainFont;
                this.SearchingAbility.Font = parent.MainFont;
                this.AntiAirPower.Font = parent.MainFont;

                ControlHelper.SetTableRowStyles(parent.TableFleet, ControlHelper.GetDefaultRowStyle());
            }

            public void Dispose()
            {
                this.Name.Dispose();
                this.State.Dispose();
                this.TotalAirSuperiority.Dispose();
                this.Fleet1AirSuperiority.Dispose();
                this.Fleet2AirSuperiority.Dispose();
                this.SearchingAbility.Dispose();
                this.AntiAirPower.Dispose();
            }
        }

        private class TableMemberControl : IDisposable
        {
            public ImageLabel Name;
            public ShipStatusHP HP;
            public ImageLabel Condition;
            public ShipStatusResource ShipResource;
            public int SelectedItem = 0;
            public int Fleet = 0;

            private ToolTip _toolTipInfo;
            private FormCombinedFleet _parent;

            public TableMemberControl(FormCombinedFleet parent)
            {
                #region Initialize

                this.Name = new ImageLabel();
                this.Name.SuspendLayout();
                this.Name.Text = "*nothing*";
                this.Name.Anchor = AnchorStyles.Left;
                this.Name.TextAlign = ContentAlignment.MiddleLeft;
                this.Name.ImageAlign = ContentAlignment.MiddleCenter;
                this.Name.ForeColor = parent.MainFontColor;
                this.Name.Padding = new Padding(2, 1, 2, 1);
                this.Name.Margin = new Padding(2, 1, 2, 1);
                this.Name.AutoSize = true;
                //Name.AutoEllipsis = true;
                this.Name.Visible = false;
                this.Name.Cursor = Cursors.Help;
                this.Name.MouseDown += this.Name_MouseDown;
                this.Name.ResumeLayout();

                this.HP = new ShipStatusHP();
                this.HP.SuspendUpdate();
                this.HP.Anchor = AnchorStyles.Left;
                this.HP.Value = 0;
                this.HP.MaximumValue = 0;
                this.HP.MaximumDigit = 999;
                this.HP.UsePrevValue = false;
                this.HP.MainFontColor = parent.MainFontColor;
                this.HP.SubFontColor = parent.SubFontColor;
                this.HP.Padding = new Padding(0, 0, 0, 0);
                this.HP.Margin = new Padding(2, 1, 2, 2);
                this.HP.AutoSize = true;
                this.HP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.HP.Visible = false;
                this.HP.ResumeUpdate();

                this.Condition = new ImageLabel();
                this.Condition.SuspendLayout();
                this.Condition.Text = "*";
                this.Condition.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                this.Condition.ForeColor = parent.MainFontColor;
                this.Condition.TextAlign = ContentAlignment.BottomRight;
                this.Condition.ImageAlign = ContentAlignment.MiddleLeft;
                this.Condition.ImageList = ResourceManager.Instance.Icons;
                this.Condition.Padding = new Padding(2, 1, 2, 1);
                this.Condition.Margin = new Padding(2, 1, 2, 1);
                this.Condition.Size = new Size(40, 20);
                this.Condition.AutoSize = true;
                this.Condition.Visible = false;
                this.Condition.ResumeLayout();

                this.ShipResource = new ShipStatusResource(parent.ToolTipInfo);
                this.ShipResource.SuspendLayout();
                this.ShipResource.FuelCurrent = 0;
                this.ShipResource.FuelMax = 0;
                this.ShipResource.AmmoCurrent = 0;
                this.ShipResource.AmmoMax = 0;
                this.ShipResource.Anchor = AnchorStyles.Left;
                this.ShipResource.Padding = new Padding(0, 2, 0, 0);
                this.ShipResource.Margin = new Padding(2, 0, 2, 1);
                this.ShipResource.Size = new Size(30, 20);
                this.ShipResource.AutoSize = false;
                this.ShipResource.Visible = false;
                this.ShipResource.ResumeLayout();

                this.ConfigurationChanged(parent);

                this._toolTipInfo = parent.ToolTipInfo;
                this._parent = parent;
                #endregion

            }

            public TableMemberControl(FormCombinedFleet parent, TableLayoutPanel table, int row, int column)
                : this(parent)
            {
                this.Fleet = row;

                this.AddToTable(table, row, column);
            }

            public void AddToTable(TableLayoutPanel table, int row, int column)
            {
                table.SuspendLayout();

                table.Controls.Add(this.Name, 0 + (column * 4), row);
                table.Controls.Add(this.HP, 1 + (column * 4), row);
                table.Controls.Add(this.Condition, 2 + (column * 4), row);
                table.Controls.Add(this.ShipResource, 3 + (column * 4), row);

                table.ResumeLayout();
            }

            public void Update(int shipMasterID)
            {
                KCDatabase db = KCDatabase.Instance;
                ShipData ship = db.Ships[shipMasterID];

                if (ship != null)
                {
                    bool isEscaped = KCDatabase.Instance.Fleet[ship.Fleet].EscapedShipList.Contains(shipMasterID);

                    this.Name.Text = ship.MasterShip.NameWithClass;
                    this.Name.Tag = ship.ShipID;
                    this._toolTipInfo.SetToolTip(this.Name,
                        string.Format(
                            "{0} {1}\r\n화력: {2}/{3}\r\n뇌장: {4}/{5}\r\n대공: {6}/{7}\r\n장갑: {8}/{9}\r\n대잠: {10}/{11}\r\n회피: {12}/{13}\r\n색적: {14}/{15}\r\n운: {16}\r\n사정: {17} / 속력: {18}\r\n(우클릭으로 도감에)\n",
                            ship.MasterShip.ShipTypeName, ship.NameWithLevel,
                            ship.FirepowerBase, ship.FirepowerTotal,
                            ship.TorpedoBase, ship.TorpedoTotal,
                            ship.AABase, ship.AATotal,
                            ship.ArmorBase, ship.ArmorTotal,
                            ship.ASWBase, ship.ASWTotal,
                            ship.EvasionBase, ship.EvasionTotal,
                            ship.LOSBase, ship.LOSTotal,
                            ship.LuckTotal,
                            Constants.GetRange(ship.Range),
                            Constants.GetSpeed(ship.Speed)
                            ));
                    {
                        var colorscheme = Utility.Configuration.Config.FormFleet.SallyAreaColorScheme;

                        if (Utility.Configuration.Config.FormFleet.AppliesSallyAreaColor &&
                            (colorscheme?.Count ?? 0) > 0 &&
                            ship.SallyArea >= 0)
                        {
                            this.Name.ForeColor = ThemeManager.GetColor(Utility.ThemeColors.ExtraFontColor);
                            this.Name.BackColor = colorscheme[Math.Min(ship.SallyArea, colorscheme.Count - 1)];
                        }
                        else
                        {
                            this.Name.ForeColor = ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
                            this.Name.BackColor = ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
                        }
                    }


                    this.HP.SuspendUpdate();
                    this.HP.Value = this.HP.PrevValue = ship.HPCurrent;
                    this.HP.MaximumValue = ship.HPMax;
                    this.HP.UsePrevValue = false;
                    this.HP.ShowDifference = false;
                    {
                        int dockID = ship.RepairingDockID;

                        if (dockID != -1)
                        {
                            this.HP.RepairTime = db.Docks[dockID].CompletionTime;
                            this.HP.RepairTimeShowMode = ShipStatusHPRepairTimeShowMode.Visible;
                        }
                        else
                        {
                            this.HP.RepairTimeShowMode = ShipStatusHPRepairTimeShowMode.Invisible;
                        }
                    }
                    this.HP.Tag = (ship.RepairingDockID == -1 && 0.5 < ship.HPRate && ship.HPRate < 1.0) ? DateTimeHelper.FromAPITimeSpan(ship.RepairTime).TotalSeconds : 0.0;
                    if (isEscaped)
                    {
                        this.HP.BackColor = Color.Silver;
                    }
                    else
                    {
                        this.HP.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
                    }
                    {
                        StringBuilder sb = new StringBuilder();
                        double hprate = (double)ship.HPCurrent / ship.HPMax;

                        sb.AppendFormat("HP: {0:0.0}% [{1}]\n", hprate * 100, Constants.GetDamageState(hprate));
                        if (isEscaped)
                        {
                            sb.AppendLine("대피중");
                        }
                        else if (hprate > 0.50)
                        {
                            sb.AppendFormat("중파까지: {0} / 대파까지: {1}\n", ship.HPCurrent - ship.HPMax / 2, ship.HPCurrent - ship.HPMax / 4);
                        }
                        else if (hprate > 0.25)
                        {
                            sb.AppendFormat("대파까지: {0}\n", ship.HPCurrent - ship.HPMax / 4);
                        }
                        else
                        {
                            sb.AppendLine("대파했습니다!");
                        }

                        if (ship.RepairTime > 0)
                        {
                            var span = DateTimeHelper.FromAPITimeSpan(ship.RepairTime);
                            sb.AppendFormat("수리시간: {0} @ {1}",
                                DateTimeHelper.ToTimeRemainString(span),
                                DateTimeHelper.ToTimeRemainString(Calculator.CalculateDockingUnitTime(ship)));
                        }

                        this._toolTipInfo.SetToolTip(this.HP, sb.ToString());
                    }
                    this.HP.ResumeUpdate();


                    this.Condition.Text = ship.Condition.ToString();
                    this.Condition.Tag = ship.Condition;
                    this.SetConditionDesign(ship.Condition);

                    if (ship.Condition < 49)
                    {
                        TimeSpan ts = new TimeSpan(0, (int)Math.Ceiling((49 - ship.Condition) / 3.0) * 3, 0);
                        this._toolTipInfo.SetToolTip(this.Condition, string.Format("완전회복까지 약 {0:D2}:{1:D2}", (int)ts.TotalMinutes, (int)ts.Seconds));
                    }

                    this.ShipResource.SetResources(ship.Fuel, ship.FuelMax, ship.Ammo, ship.AmmoMax);
                }
                else
                {
                    this.Name.Tag = -1;
                }

                this.Name.Visible =
                this.HP.Visible =
                this.Condition.Visible =
                this.ShipResource.Visible = shipMasterID != -1;
            }

            void Name_MouseDown(object sender, MouseEventArgs e)
            {
                if (this.Name.Tag is int id && id != -1)
                {
                    if ((e.Button & MouseButtons.Right) != 0)
                    {
                        new DialogAlbumMasterShip(id).Show(this._parent);
                    }
                }
            }

            private string GetEquipmentString(ShipData ship)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < ship.Slot.Count; i++)
                {
                    var eq = ship.SlotInstance[i];
                    if (eq != null)
                        sb.AppendFormat("[{0}/{1}] {2}\r\n", ship.Aircraft[i], ship.MasterShip.Aircraft[i], eq.NameWithLevel);
                }

                {
                    var exslot = ship.ExpansionSlotInstance;
                    if (exslot != null)
                        sb.AppendFormat("보강: {0}\r\n", exslot.NameWithLevel);
                }

                int[] slotmaster = ship.AllSlotMaster.ToArray();

                sb.AppendFormat("\r\n주간전: {0}", Constants.GetDayAttackKind(Calculator.GetDayAttackKind(slotmaster, ship.ShipID, -1)));
                {
                    int shelling = ship.ShellingPower;
                    int aircraft = ship.AircraftPower;
                    if (shelling > 0)
                    {
                        if (aircraft > 0)
                            sb.AppendFormat(" - 포격: {0} / 공습: {1}", shelling, aircraft);
                        else
                            sb.AppendFormat(" - 위력: {0}", shelling);
                    }
                    else if (aircraft > 0)
                        sb.AppendFormat(" - 위력: {0}", aircraft);
                }
                sb.AppendLine();

                if (ship.CanAttackAtNight)
                {
                    sb.AppendFormat("야전: {0}", Constants.GetNightAttackKind(Calculator.GetNightAttackKind(slotmaster, ship.ShipID, -1)));
                    {
                        int night = ship.NightBattlePower;
                        if (night > 0)
                        {
                            sb.AppendFormat(" - 위력: {0}", night);
                        }
                    }
                    sb.AppendLine();
                }

                {
                    int torpedo = ship.TorpedoPower;
                    int asw = ship.AntiSubmarinePower;

                    if (torpedo > 0)
                    {
                        sb.AppendFormat("뇌격: {0}", torpedo);
                    }
                    if (asw > 0)
                    {
                        if (torpedo > 0)
                            sb.Append(" / ");

                        sb.AppendFormat("대잠: {0}", asw);

                        if (ship.CanOpeningASW)
                            sb.Append(" (선제대잠가능)");
                    }
                    if (torpedo > 0 || asw > 0)
                        sb.AppendLine();
                }

                {
                    int aacutin = Calculator.GetAACutinKind(ship.ShipID, slotmaster);
                    if (aacutin != 0)
                    {
                        sb.AppendFormat("대공: {0}\r\n", Constants.GetAACutinKind(aacutin));
                    }
                    double adjustedaa = Calculator.GetAdjustedAAValue(ship);
                    sb.AppendFormat("가중대공: {0} (비율격추: {1:p2})\r\n",
                        adjustedaa,
                        Calculator.GetProportionalAirDefense(adjustedaa)
                        );

                }

                {
                    int airsup_min;
                    int airsup_max;
                    if (Utility.Configuration.Config.FormFleet.AirSuperiorityMethod == 1)
                    {
                        airsup_min = Calculator.GetAirSuperiority(ship, false);
                        airsup_max = Calculator.GetAirSuperiority(ship, true);
                    }
                    else
                    {
                        airsup_min = airsup_max = Calculator.GetAirSuperiorityIgnoreLevel(ship);
                    }

                    int airbattle = ship.AirBattlePower;
                    if (airsup_min > 0)
                    {

                        string airsup_str;
                        if (Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange && airsup_min < airsup_max)
                        {
                            airsup_str = string.Format("{0} ～ {1}", airsup_min, airsup_max);
                        }
                        else
                        {
                            airsup_str = airsup_min.ToString();
                        }

                        if (airbattle > 0)
                            sb.AppendFormat("제공: {0} / 항공화력: {1}\r\n", airsup_str, airbattle);
                        else
                            sb.AppendFormat("제공: {0}\r\n", airsup_str);
                    }
                    else if (airbattle > 0)
                        sb.AppendFormat("항공화력: {0}\r\n", airbattle);
                }

                return sb.ToString();
            }

            private void SetConditionDesign(int cond)
            {

                if (this.Condition.ImageAlign == ContentAlignment.MiddleCenter)
                {
                    // icon invisible
                    this.Condition.ImageIndex = -1;

                    if (cond < 20)
                        this.Condition.BackColor = Color.LightCoral;
                    else if (cond < 30)
                        this.Condition.BackColor = Color.LightSalmon;
                    else if (cond < 40)
                        this.Condition.BackColor = Color.Moccasin;
                    else if (cond < 50)
                        this.Condition.BackColor = Color.Transparent;
                    else
                        this.Condition.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.GreenHighlight);

                }
                else
                {
                    this.Condition.BackColor = Color.Transparent;

                    if (cond < 20)
                        this.Condition.ImageIndex = (int)ResourceManager.IconContent.ConditionVeryTired;
                    else if (cond < 30)
                        this.Condition.ImageIndex = (int)ResourceManager.IconContent.ConditionTired;
                    else if (cond < 40)
                        this.Condition.ImageIndex = (int)ResourceManager.IconContent.ConditionLittleTired;
                    else if (cond < 50)
                        this.Condition.ImageIndex = (int)ResourceManager.IconContent.ConditionNormal;
                    else
                        this.Condition.ImageIndex = (int)ResourceManager.IconContent.ConditionSparkle;

                }
            }

            public void ConfigurationChanged(FormCombinedFleet parent)
            {
                this.Name.Font = parent.MainFont;
                this.HP.MainFont = parent.MainFont;
                this.HP.SubFont = parent.SubFont;
                this.Condition.Font = parent.MainFont;
                this.SetConditionDesign((this.Condition.Tag as int?) ?? 49);
            }

            public void Dispose()
            {
                this.Name.Dispose();
                this.HP.Dispose();
                this.Condition.Dispose();
                this.ShipResource.Dispose();
            }
        }

        public Font MainFont { get; set; }
        public Font SubFont { get; set; }
        public Color MainFontColor { get; set; }
        public Color SubFontColor { get; set; }

        private TableFleetControl[] _controlFleet = new TableFleetControl[2];
        private TableMemberControl[][] _controlMember = new TableMemberControl[2][];

        private int _anchorageRepairBound;

        public FormCombinedFleet(FormMain parent)
        {
            this.InitializeComponent();

            Utility.SystemEvents.UpdateTimerTick += this.UpdateTimerTick;

            this.ConfigurationChanged();

            this.MainFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.SubFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.SubFontColor);

            this._anchorageRepairBound = 0;

            //ui init

            ControlHelper.SetDoubleBuffered(this.TableFleet);
            ControlHelper.SetDoubleBuffered(this.TableMember);

            this.TableFleet.Visible = false;
            this.TableFleet.SuspendLayout();
            this.TableFleet.BorderStyle = BorderStyle.FixedSingle;
            this.TableFleet.RowCount = 1;
            this._controlFleet[0] = new TableFleetControl(this, this.TableFleet, 0);
            this._controlFleet[1] = new TableFleetControl(this, this.TableFleet, 1);
            this.TableFleet.ResumeLayout();
            this.TableMember.SuspendLayout();
            this._controlMember = new TableMemberControl[2][];

            for (int i = 0; i < this._controlFleet.Length; i++)
            {
                this._controlMember[i] = new TableMemberControl[7];
                for (int j = 0; j < this._controlMember[i].Length; j++)
                {
                    this._controlMember[i][j] = new TableMemberControl(this, this.TableMember, j, i);
                }
            }

            this.TableMember.ResumeLayout();

            this.ConfigurationChanged();     //fixme: 苦渋の決断

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormFleet]);

        }

        private void FormCombinedFleet_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("#{0}", Constants.GetCombinedFleet(KCDatabase.Instance.Fleet.CombinedFlag));

            APIObserver o = APIObserver.Instance;

            // 작업 보류체크
            //o["api_req_nyukyo/start"].RequestReceived += Updated;
            o["api_req_nyukyo/speedchange"].RequestReceived += this.Updated;
            o["api_req_hensei/change"].RequestReceived += this.Updated;
            o["api_req_kousyou/destroyship"].RequestReceived += this.Updated;
            o["api_req_member/updatedeckname"].RequestReceived += this.Updated;
            o["api_req_kaisou/remodeling"].RequestReceived += this.Updated;
            o["api_req_map/start"].RequestReceived += this.Updated;
            o["api_req_hensei/combined"].RequestReceived += this.Updated;

            o["api_port/port"].ResponseReceived += this.Updated;
            o["api_get_member/ship2"].ResponseReceived += this.Updated;
            o["api_get_member/ndock"].ResponseReceived += this.Updated;
            o["api_req_kousyou/getship"].ResponseReceived += this.Updated;
            o["api_req_hokyu/charge"].ResponseReceived += this.Updated;
            o["api_req_kousyou/destroyship"].ResponseReceived += this.Updated;
            o["api_get_member/ship3"].ResponseReceived += this.Updated;
            o["api_req_kaisou/powerup"].ResponseReceived += this.Updated;        //requestのほうは面倒なのでこちらでまとめてやる
            o["api_get_member/deck"].ResponseReceived += this.Updated;
            o["api_get_member/slot_item"].ResponseReceived += this.Updated;
            o["api_req_map/start"].ResponseReceived += this.Updated;
            o["api_req_map/next"].ResponseReceived += this.Updated;
            o["api_get_member/ship_deck"].ResponseReceived += this.Updated;
            o["api_req_hensei/preset_select"].ResponseReceived += this.Updated;
            o["api_req_kaisou/slot_exchange_index"].ResponseReceived += this.Updated;
            o["api_get_member/require_info"].ResponseReceived += this.Updated;
            o["api_req_kaisou/slot_deprive"].ResponseReceived += this.Updated;
            o["api_req_kaisou/marriage"].ResponseReceived += this.Updated;

            Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;
        }

        void Updated(string apiname, dynamic data)
        {
            if (this._isRemodeling)
            {
                if (apiname == "api_get_member/slot_item")
                    this._isRemodeling = false;
                else
                    return;
            }
            if (apiname == "api_req_kaisou/remodeling")
            {
                this._isRemodeling = true;
                return;
            }

            KCDatabase db = KCDatabase.Instance;

            if (db.Ships.Count == 0) return;
            this.TableFleet.Visible = true;

            this.TableFleet.SuspendLayout();

            this.TableFleet.Update();

            for (int j = 0; j < 2; j++)
            {
                FleetData fleet = db.Fleet.Fleets[j + 1];

                if (fleet == null) return;

                this._controlFleet[j].Update();

                this._anchorageRepairBound = fleet.CanAnchorageRepair ? 2 + fleet.MembersInstance[0].SlotInstance.Count(eq => eq != null && eq.MasterEquipment.CategoryType == EquipmentTypes.RepairFacility) : 0;
                this.TableMember.SuspendLayout();
                this.TableMember.RowCount = fleet.Members.Count(id => id > 0);

                for (int i = 0; i < this._controlMember[j].Length; i++)
                {
                    this._controlMember[j][i].Update(i < fleet.Members.Count ? fleet.Members[i] : -1);
                }

                this.TableMember.ResumeLayout();

                if (this.Icon != null) ResourceManager.DestroyIcon(this.Icon);
                this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[this._controlFleet[fleet.FleetID - 1].State.GetIconIndex()]);
                if (this.Parent != null) this.Parent.Refresh();       //アイコンを更新するため
            }

            this.TableFleet.ResumeLayout();
        }

        void UpdateTimerTick()
        {
            for (int j = 1; j < 2; j++)
            {
                FleetData fleet = KCDatabase.Instance.Fleet.Fleets[j];

                this.TableFleet.SuspendLayout();
                this.TableFleet.ResumeLayout();
                this.TableMember.SuspendLayout();

                if (fleet != null)
                {
                    this._controlFleet[fleet.FleetID - 1].Refresh();

                    for (int i = 0; i < this._controlMember[fleet.FleetID - 1].Length; i++)
                    {
                        this._controlMember[fleet.FleetID - 1][i].HP.Refresh();
                    }
                }

                this.TableMember.ResumeLayout();
            }
        }

        /// <summary>
		/// 「艦隊分析 -艦これ-」の艦隊情報反映用フォーマットでコピー
		/// https://kancolle-fleetanalysis.firebaseapp.com/#/
		/// </summary>
		private void ContextMenuFleet_CopyToFleetAnalysis_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            sb.Append("[");
            foreach (var ship in KCDatabase.Instance.Ships.Values.Where(s => s.IsLocked))
            {
                sb.AppendFormat(@"{{""api_ship_id"":{0},""api_lv"":{1},""api_kyouka"":[{2}]}},",
                    ship.ShipID, ship.Level, string.Join(",", (int[])ship.RawData.api_kyouka));
            }
            sb.Remove(sb.Length - 1, 1);        // remove ","
            sb.Append("]");

            Clipboard.SetData(DataFormats.StringFormat, sb.ToString());
        }

        private void ContextMenuFleet_Capture_Click(object sender, EventArgs e)
        {
            using (Bitmap bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
            {
                this.DrawToBitmap(bitmap, this.ClientRectangle);

                Clipboard.SetData(DataFormats.Bitmap, bitmap);
            }
        }

        private void ContextMenuFleet_OutputFleetImage_Click(object sender, EventArgs e)
        {
            using (var dialog = new DialogFleetImageGenerator(1))
            {
                dialog.ShowDialog(this);
            }
        }

        void ConfigurationChanged()
        {
            var c = Utility.Configuration.Config;

            this.MainFont = this.Font = c.UI.MainFont;
            this.SubFont = c.UI.SubFont;
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.MainFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.SubFontColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.SubFontColor);

            this.AutoScroll = c.FormFleet.IsScrollable;
            for (int i = 0; i < this._controlFleet.Length; i++)
            {
                var fleet = KCDatabase.Instance.Fleet[i + 1];

                this.TableFleet.SuspendLayout();
                if (this._controlFleet != null && fleet != null)
                {
                    this._controlFleet[i].ConfigurationChanged(this);
                    this._controlFleet[0].Update();
                }

                this.TableFleet.ResumeLayout();
                this.TableMember.SuspendLayout();
                if (this._controlMember != null)
                {
                    bool showAircraft = c.FormFleet.ShowAircraft;
                    bool fixShipNameWidth = c.FormFleet.FixShipNameWidth;
                    bool shortHPBar = c.FormFleet.ShortenHPBar;
                    bool colorMorphing = c.UI.BarColorMorphing;
                    Color[] colorScheme = c.UI.BarColorScheme.Select(col => col.ColorData).ToArray();
                    bool showNext = c.FormFleet.ShowNextExp;
                    bool showConditionIcon = c.FormFleet.ShowConditionIcon;
                    var levelVisibility = c.FormFleet.EquipmentLevelVisibility;
                    bool showAircraftLevelByNumber = c.FormFleet.ShowAircraftLevelByNumber;
                    int fixedShipNameWidth = c.FormFleet.FixedShipNameWidth;
                    bool isLayoutFixed = c.UI.IsLayoutFixed;
                    // bool IsDarkSkinUse = c.UI.IsDarkSkinUse;
                    if (this._controlMember[i] != null)
                    {
                        for (int j = 0; j < this._controlMember[i].Length; j++)
                        {
                            var member = this._controlMember[i][j];

                            if (fixShipNameWidth)
                            {
                                member.Name.AutoSize = false;
                                member.Name.Size = new Size(fixedShipNameWidth, 20);
                            }
                            else
                            {
                                Console.WriteLine(member.Name);
                                member.Name.AutoSize = true;
                            }

                            member.HP.SuspendUpdate();
                            member.HP.Text = shortHPBar ? "" : "HP:";
                            member.HP.HPBar.ColorMorphing = colorMorphing;
                            member.HP.HPBar.SetBarColorScheme(colorScheme);
                            member.HP.MaximumSize = isLayoutFixed ? new Size(int.MaxValue, (int)ControlHelper.GetDefaultRowStyle().Height - member.HP.Margin.Vertical) : Size.Empty;
                            member.HP.ResumeUpdate();
                            member.Condition.ImageAlign = showConditionIcon ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                            member.ShipResource.BarFuel.ColorMorphing =
                            member.ShipResource.BarAmmo.ColorMorphing = colorMorphing;
                            member.ShipResource.BarFuel.SetBarColorScheme(colorScheme);
                            member.ShipResource.BarAmmo.SetBarColorScheme(colorScheme);

                            member.ConfigurationChanged(this);
                            if (fleet != null)
                                member.Update(i < fleet.Members.Count ? fleet.Members[i] : -1);
                        }
                    }
                }

                ControlHelper.SetTableRowStyles(this.TableMember, ControlHelper.GetDefaultRowStyle());
                this.TableMember.ResumeLayout();

                this.TableMember.Location = new Point(this.TableMember.Location.X, this.TableFleet.Bottom /*+ Math.Max( TableFleet.Margin.Bottom, TableMember.Margin.Top )*/ );

                this.TableMember.PerformLayout();        //fixme:サイズ変更に親パネルが追随しない
            }
        }

        protected override string GetPersistString()
        {
            return "CombinedFleet";
        }

        private void TableMember_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            /*
            ControlFleet.Dispose();
            */
                for (int i = 0; i < this._controlMember.Length; i++)
                {
                    for (int j = 0; j < this._controlMember[i].Length; j++)
                    {
                        this._controlMember[i][j].Dispose();
                    }
                }


            // --- auto generated ---
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }
    }

}
