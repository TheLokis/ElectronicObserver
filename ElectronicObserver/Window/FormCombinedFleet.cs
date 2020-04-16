using Codeplex.Data;
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
        { // 함대 상태 정보(FormCombinedFleet으로 이관)
            public FleetState State;
            public ToolTip ToolTipInfo;
            public FormCombinedFleet Parent;

            public TableFleetControl(FormCombinedFleet parent)
            {
                this.Parent = parent;
                #region Initialize

                this.State = new FleetState
                {
                    Anchor = AnchorStyles.Left,
                    ForeColor = parent.MainFontColor,
                    Padding = new Padding(),
                    Margin = new Padding(),
                    AutoSize = true
                };

                this.ConfigurationChanged(parent);

                this.ToolTipInfo = parent.ToolTipInfo;

                #endregion
            }

            public TableFleetControl(FormCombinedFleet parent, TableLayoutPanel table)
                : this(parent)
            {
                this.Parent = parent;
                this.AddToTable(table);
            }

            public void AddToTable(TableLayoutPanel table)
            {
                table.SuspendLayout();
                table.Controls.Add(this.State, 1, 0);
                table.ResumeLayout();
            }

            public void Refresh()
            {
                this.State.RefreshFleetState();
            }

            public void ConfigurationChanged(FormCombinedFleet parent)
            {
                this.State.Font     = parent.MainFont;
                this.State.RefreshFleetState();

                ControlHelper.SetTableRowStyles(parent.TableFleet, ControlHelper.GetDefaultRowStyle());
            }

            public void Dispose()
            {
                this.State.Dispose();
            }
        }

        private class TableMemberControl : IDisposable
        {
            public ImageLabel Name;
            public ShipStatusLevel Level;
            public ShipStatusHP HP;
            public ImageLabel Condition;
            public ShipStatusResource ShipResource;
            public ShipStatusEquipment Equipments;
            public int SelectedItem = 0;

            private ToolTip     _toolTipInfo;
            private FormCombinedFleet   _parent;

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

                this.Level = new ShipStatusLevel();
                this.Level.SuspendLayout();
                this.Level.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                this.Level.Value = 0;
                this.Level.MaximumValue = ExpTable.ShipMaximumLevel;
                this.Level.ValueNext = 0;
                this.Level.MainFontColor = parent.MainFontColor;
                this.Level.SubFontColor = parent.SubFontColor;
                //Level.TextNext = "n.";
                this.Level.Padding = new Padding(0, 0, 0, 0);
                this.Level.Margin = new Padding(2, 0, 2, 1);
                this.Level.AutoSize = true;
                this.Level.Visible = false;
                this.Level.Cursor = Cursors.Help;
                this.Level.MouseDown += this.Level_MouseDown;
                this.Level.ResumeLayout();

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

                this.Equipments = new ShipStatusEquipment();
                this.Equipments.SuspendUpdate();
                this.Equipments.Anchor = AnchorStyles.Left;
                this.Equipments.Padding = new Padding(0, 1, 0, 1);
                this.Equipments.Margin = new Padding(2, 0, 2, 1);
                this.Equipments.Size = new Size(40, 20);
                this.Equipments.AutoSize = true;
                this.Equipments.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.Equipments.Visible = false;
                this.Equipments.ResumeUpdate();

                this.ConfigurationChanged(parent);

                this._toolTipInfo = parent.ToolTipInfo;
                this._parent = parent;
                #endregion

            }

            public TableMemberControl(FormCombinedFleet parent, TableLayoutPanel table, int row)
                : this(parent)
            {
                this.AddToTable(table, row);

                this.Equipments.Name = string.Format("{0}_{1}", parent.FleetID, row + 1);
            }

            public void AddToTable(TableLayoutPanel table, int row)
            {
                table.SuspendLayout();

                table.Controls.Add(this.Name, 0, row);
                table.Controls.Add(this.Level, 1, row);
                table.Controls.Add(this.HP, 2, row);
                table.Controls.Add(this.Condition, 3, row);
                table.Controls.Add(this.ShipResource, 4, row);
                table.Controls.Add(this.Equipments, 5, row);

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
                            this.Name.ForeColor = ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.ExtraFontColor);
                            this.Name.BackColor = colorscheme[Math.Min(ship.SallyArea, colorscheme.Count - 1)];
                        }
                        else
                        {
                            this.Name.ForeColor = ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
                            this.Name.BackColor = ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);
                        }
                    }

                    this.Level.Value = ship.Level;
                    this.Level.ValueNext = ship.ExpNext;
                    this.Level.Tag = ship.MasterID;

                    {
                        StringBuilder tip = new StringBuilder();
                        tip.AppendFormat("Total: {0} exp.\r\n", ship.ExpTotal);

                        if (!Utility.Configuration.Config.FormFleet.ShowNextExp)
                            tip.AppendFormat("다음 레벨까지: {0} exp.\r\n", ship.ExpNext);

                        if (ship.MasterShip.RemodelAfterShipID != 0 && ship.Level < ship.MasterShip.RemodelAfterLevel)
                        {
                            tip.AppendFormat("개장까지: Lv. {0} / {1} exp.\r\n", ship.MasterShip.RemodelAfterLevel - ship.Level, ship.ExpNextRemodel);
                        }
                        else if (ship.Level <= 99)
                        {
                            tip.AppendFormat("Lv99까지: {0} exp.\r\n", Math.Max(ExpTable.GetExpToLevelShip(ship.ExpTotal, 99), 0));
                        }
                        else
                        {
                            tip.AppendFormat("Lv{0}까지: {1} exp.\r\n", ExpTable.ShipMaximumLevel, Math.Max(ExpTable.GetExpToLevelShip(ship.ExpTotal, ExpTable.ShipMaximumLevel), 0));
                        }

                        tip.AppendLine("(우클릭으로 경험치계산기에)");

                        this._toolTipInfo.SetToolTip(this.Level, tip.ToString());
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
                        this.HP.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);
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
                    else
                    {
                        this._toolTipInfo.SetToolTip(this.Condition, string.Format("앞으로 {0} 회 대성공가능", (int)Math.Ceiling((ship.Condition - 49) / 3.0)));
                    }

                    this.ShipResource.SetResources(ship.Fuel, ship.FuelMax, ship.Ammo, ship.AmmoMax);


                    this.Equipments.SetSlotList(ship);
                    this._toolTipInfo.SetToolTip(this.Equipments, this.GetEquipmentString(ship));

                }
                else
                {
                    this.Name.Tag = -1;
                }

                this.Name.Visible =
                this.Level.Visible =
                this.HP.Visible =
                this.Condition.Visible =
                this.ShipResource.Visible =
                this.Equipments.Visible = shipMasterID != -1;
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

            private void Level_MouseDown(object sender, MouseEventArgs e)
            {
                if (this.Level.Tag is int id && id != -1)
                {
                    if ((e.Button & MouseButtons.Right) != 0)
                    {
                        new DialogExpChecker(id).Show(this._parent);
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
                        this.Condition.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.GreenHighlight);

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
                this.Level.MainFont = parent.MainFont;
                this.Level.SubFont = parent.SubFont;
                this.HP.MainFont = parent.MainFont;
                this.HP.SubFont = parent.SubFont;
                this.Condition.Font = parent.MainFont;
                this.SetConditionDesign((this.Condition.Tag as int?) ?? 49);
                this.Equipments.Font = parent.SubFont;
            }

            public void Dispose()
            {
                this.Name.Dispose();
                this.Level.Dispose();
                this.HP.Dispose();
                this.Condition.Dispose();
                this.ShipResource.Dispose();
                this.Equipments.Dispose();
            }
        }

        public Font MainFont { get; set; }
        public Font SubFont { get; set; }
        public Color MainFontColor { get; set; }
        public Color SubFontColor { get; set; }

        private TableFleetControl[] _controlFleet = new TableFleetControl[2];
        private TableMemberControl[][] _controlMember { get; set; } 

        private int _anchorageRepairBound;

        public FormCombinedFleet(FormMain parent)
        {
            this.InitializeComponent();

            Utility.SystemEvents.UpdateTimerTick += this.UpdateTimerTick;

            this.ConfigurationChanged();

            this.MainFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.SubFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.SubFontColor);

            this._anchorageRepairBound = 0;

            //ui init

            ControlHelper.SetDoubleBuffered(this.TableFleet);
            ControlHelper.SetDoubleBuffered(this.TableMember);

            this.TableFleet.Visible = false;
            this.TableFleet.SuspendLayout();
            this.TableFleet.BorderStyle = BorderStyle.FixedSingle;
            this._controlFleet[0] = new TableFleetControl(this, this.TableFleet);
            this._controlFleet[1] = new TableFleetControl(this, this.TableFleet);
            this.TableFleet.ResumeLayout();


            this.TableMember.SuspendLayout();
            for (int i = 0; i < this._controlFleet.Length; i++)
            {
                this._controlMember[i] = new TableMemberControl[7];
                for (int j = 0; j < this._controlMember.Length; j++)
                {
                    this._controlMember[i][j] = new TableMemberControl(this, this.TableMember, j);
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

            for (int j = 1; j < 2; j++)
            {
                FleetData fleet = db.Fleet.Fleets[j];

                if (fleet == null) return;

                this.TableFleet.SuspendLayout();
                this.TableFleet.Visible = true;
                this.TableFleet.ResumeLayout();

                this._anchorageRepairBound = fleet.CanAnchorageRepair ? 2 + fleet.MembersInstance[0].SlotInstance.Count(eq => eq != null && eq.MasterEquipment.CategoryType == EquipmentTypes.RepairFacility) : 0;

                this.TableMember.SuspendLayout();
                this.TableMember.RowCount = fleet.Members.Count(id => id > 0);
                for (int i = 0; i < this._controlMember.Length; i++)
                {
                    this._controlMember[fleet.FleetID - 1][i].Update(i < fleet.Members.Count ? fleet.Members[i] : -1);
                }
                this.TableMember.ResumeLayout();


                if (this.Icon != null) ResourceManager.DestroyIcon(this.Icon);
                this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[this._controlFleet[fleet.FleetID - 1].State.GetIconIndex()]);
                if (this.Parent != null) this.Parent.Refresh();       //アイコンを更新するため
            }
        }

        void UpdateTimerTick()
        {
            for (int j = 1; j < 2; j++)
            {
                FleetData fleet = KCDatabase.Instance.Fleet.Fleets[j];

                this.TableFleet.SuspendLayout();
                {
                    if (fleet != null)
                        this._controlFleet[fleet.FleetID - 1].Refresh();
                }

                this.TableFleet.ResumeLayout();

                this.TableMember.SuspendLayout();
                for (int i = 0; i < this._controlMember[fleet.FleetID - 1].Length; i++)
                {
                    this._controlMember[fleet.FleetID - 1][i].HP.Refresh();
                }
                this.TableMember.ResumeLayout();
            }
        }

        private void ContextMenuFleet_Opening(object sender, CancelEventArgs e)
        {
            this.ContextMenuFleet_Capture.Visible = Utility.Configuration.Config.Debug.EnableDebugMenu;
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

        private void ContextMenuFleet_AntiAirDetails_Click(object sender, EventArgs e)
        {
            var dialog = new DialogAntiAirDefense();

            dialog.SetFleetID(this.FleetID);
            dialog.Show(this);
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
            this.BackColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.MainFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.MainFontColor);
            this.SubFontColor = Utility.ThemeManager.GetColor(Utility.Configuration.Config.UI.Theme, Utility.ThemeColors.SubFontColor);

            this.AutoScroll = c.FormFleet.IsScrollable;

            var fleet = KCDatabase.Instance.Fleet[this.FleetID];

            this.TableFleet.SuspendLayout();
            if (this._controlFleet != null && fleet != null)
            {
                this._controlFleet.ConfigurationChanged(this);
                this._controlFleet.Update(fleet);
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

                for (int i = 0; i < this._controlMember.Length; i++)
                {
                    var member = this._controlMember[i];

                    member.Equipments.ShowAircraft = showAircraft;
                    if (fixShipNameWidth)
                    {
                        member.Name.AutoSize = false;
                        member.Name.Size = new Size(fixedShipNameWidth, 20);
                    }
                    else
                    {
                        member.Name.AutoSize = true;
                    }

                    member.HP.SuspendUpdate();
                    member.HP.Text = shortHPBar ? "" : "HP:";
                    member.HP.HPBar.ColorMorphing = colorMorphing;
                    member.HP.HPBar.SetBarColorScheme(colorScheme);
                    member.HP.MaximumSize = isLayoutFixed ? new Size(int.MaxValue, (int)ControlHelper.GetDefaultRowStyle().Height - member.HP.Margin.Vertical) : Size.Empty;
                    member.HP.ResumeUpdate();
                    member.Level.TextNext = showNext ? "next:" : null;
                    member.Condition.ImageAlign = showConditionIcon ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                    member.Equipments.LevelVisibility = levelVisibility;
                    member.Equipments.ShowAircraftLevelByNumber = showAircraftLevelByNumber;
                    member.Equipments.MaximumSize = isLayoutFixed ? new Size(int.MaxValue, (int)ControlHelper.GetDefaultRowStyle().Height - member.Equipments.Margin.Vertical) : Size.Empty;
                    member.ShipResource.BarFuel.ColorMorphing =
                    member.ShipResource.BarAmmo.ColorMorphing = colorMorphing;
                    member.ShipResource.BarFuel.SetBarColorScheme(colorScheme);
                    member.ShipResource.BarAmmo.SetBarColorScheme(colorScheme);

                    member.ConfigurationChanged(this);
                    if (fleet != null)
                        member.Update(i < fleet.Members.Count ? fleet.Members[i] : -1);
                }
            }

            ControlHelper.SetTableRowStyles(this.TableMember, ControlHelper.GetDefaultRowStyle());
            this.TableMember.ResumeLayout();

            this.TableMember.Location = new Point(this.TableMember.Location.X, this.TableFleet.Bottom /*+ Math.Max( TableFleet.Margin.Bottom, TableMember.Margin.Top )*/ );

            this.TableMember.PerformLayout();        //fixme:サイズ変更に親パネルが追随しない

        }

        private void TableMember_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
        }

        protected override string GetPersistString()
        {
            return "Fleet #" + this.FleetID.ToString();
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
                this._controlMember[i].Dispose();
                

            // --- auto generated ---
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }
    }

}
