using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElectronicObserver.Window.Dialog
{
    public partial class DialogAkashilist : Form
    {

        private int current_day;

        public DialogAkashilist()
        {
            this.InitializeComponent();

            List<Button> Btns = new List<Button>() { this.Btn_Day0, this.Btn_Day1, this.Btn_Day2, this.Btn_Day3, this.Btn_Day4, this.Btn_Day5, this.Btn_Day6 };
            List<string> Days = new List<string>() { "일", "월", "화", "수", "목", "금", "토" };

            ControlHelper.SetDoubleBuffered(this.AkashiListView);


            //ShipView Initialize
            //AkashiListView.SuspendLayout();

            this.Btn_BackColor_Reset();
            Btns[(int)DateTime.Today.DayOfWeek].BackColor = SystemColors.ActiveCaption;

            this.Update_DataList((int)DateTime.Today.DayOfWeek, false, false);
        }

        public void Btn_BackColor_Reset()
        {
            List<Button> Btns = new List<Button>() { this.Btn_Day0, this.Btn_Day1, this.Btn_Day2, this.Btn_Day3, this.Btn_Day4, this.Btn_Day5, this.Btn_Day6 };
            foreach (var btn in Btns)
            {
                btn.BackColor = SystemColors.Control;
            }
        }

        private void EquipmentView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.ColumnIndex == this.EqType.Index)
            {
                e.Value = ResourceManager.GetEquipmentImage((int)e.Value);
                e.FormattingApplied = true;
            }

        }

        private void ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected) e.Item.Selected = false;
        }

        public void Update_DataList(int day, bool Checked, bool KaisuChecked)
        {
            this.current_day = day;

            this.AkashiListView.Rows.Clear();

            JObject Data = DynamicDataReader.Instance.GetData(DataType.AkashiData);

            KCDatabase db = KCDatabase.Instance;

            List<DataGridViewRow> rows = new List<DataGridViewRow>(Data.Count);
            var ships = KCDatabase.Instance.Ships.Values;
            var equipments = KCDatabase.Instance.Equipments.Values;
            var allCount = equipments.GroupBy(equipment => equipment.EquipmentID).ToDictionary(group => group.Key, group => group.Count());

            var remainCount = new Dictionary<int, int>(allCount);

            List<EquipmentDataMaster> Equiped_Item = new List<EquipmentDataMaster>();

            foreach (var eq in ships
                .SelectMany(s => s.AllSlotInstanceMaster)
                .Where(eq => eq != null))
            {
                Equiped_Item.Add(eq);
            }

            foreach (var eq in KCDatabase.Instance.BaseAirCorps.Values
                .SelectMany(corps => corps.Squadrons.Values.Select(sq => sq.EquipmentInstance))
                .Where(eq => eq != null))
            {

                remainCount[eq.EquipmentID]--;
            }

            foreach (var eq in KCDatabase.Instance.RelocatedEquipments.Values
                .Where(eq => eq.EquipmentInstance != null))
            {

                remainCount[eq.EquipmentInstance.EquipmentID]--;
            }

            foreach (var eq in equipments)
            {
                if (Equiped_Item.Find(x => x.EquipmentID == eq.EquipmentID) != null)
                {
                    Equiped_Item.Remove(Equiped_Item.Find(x => x.EquipmentID == eq.EquipmentID));
                    remainCount[eq.EquipmentID]--;
                }
                else
                {
                    if (eq.IsLocked)
                        remainCount[eq.EquipmentID]--;
                }
            }


            if (!Checked)
                allCount = remainCount;



            DataGridViewCellStyle Default = new DataGridViewCellStyle();
            DataGridViewCellStyle CanKaisu = new DataGridViewCellStyle();
            CanKaisu.ForeColor = Color.Green;
            this.EqType.AutoSizeMode =
            this.Resource_Fuel.AutoSizeMode =
            this.Resource_Ammo.AutoSizeMode =
            this.Resource_Steel.AutoSizeMode =
            this.Resource_Baux.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            foreach (var eq in KCDatabase.Instance.MasterEquipments.Values)
            {
                if (eq.Name == "なし") continue;

                if (!Data.ContainsKey(eq.ID.ToString())) continue;

                var EquipmentDatas = Data[eq.ID.ToString()]["improvement"];

                foreach (var EquipmentData in EquipmentDatas)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.AkashiListView);
                    int[] resources = EquipmentData["resource"][0].ToObject<int[]>();
                    List<int> kanmusus = new List<int>();
                    bool can_kaisu_day = false;

                    foreach (var data in EquipmentData["req"])
                    {
                        if (Convert.ToBoolean(data[0][day]))
                        {
                            can_kaisu_day = true;

                            if (can_kaisu_day)
                                foreach (int kanmusu in data[1])
                                    kanmusus.Add(kanmusu);
                        }
                    }

                    int[] m_before5 = EquipmentData["resource"][1].ToObject<int[]>();
                    int[] m_after6 = EquipmentData["resource"][2].ToObject<int[]>();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < kanmusus.Count; i++)
                    {
                        sb.Append(KCDatabase.Instance.MasterShips[kanmusus[i]].Name);

                        if (i != kanmusus.Count - 1) sb.Append(" , ");
                    }

                    string kanmusu_list = sb.ToString();

                    string materials_1 = m_before5[0] + "/" + m_before5[2] + " (" + m_before5[1] + "/" + m_before5[3] + ")";
                    string materials_2 = m_after6[0] + "/" + m_after6[2] + " (" + m_after6[1] + "/" + m_after6[3] + ")";

                    bool[] can_kaisu_equipment = { false, false, false };

                    bool[] can_kaisu_material = { false, false, false };

                    if (m_before5[0] <= db.Material.DevelopmentMaterial && m_before5[2] <= db.Material.ModdingMaterial)
                    {
                        can_kaisu_material[0] = true;
                        row.Cells[8].Style = CanKaisu;
                    }
                    else
                        can_kaisu_material[0] = false;


                    if (m_after6[0] <= db.Material.DevelopmentMaterial && m_after6[2] <= db.Material.ModdingMaterial)
                    {
                        can_kaisu_material[1] = true;
                        row.Cells[10].Style = CanKaisu;
                    }
                    else
                        can_kaisu_material[1] = false;

                    string materials_eq1 = "";
                    if (m_before5[4] != 0)
                    {
                        materials_eq1 = KCDatabase.Instance.MasterEquipments[m_before5[4]].Name + " x " + m_before5[5];
                        if (allCount.ContainsKey(m_before5[4]))
                            if (allCount[m_before5[4]] >= m_before5[5])
                            {
                                can_kaisu_equipment[0] = true;
                                row.Cells[9].Style = CanKaisu;
                            }
                            else
                                can_kaisu_equipment[0] = false;
                    }

                    string materials_eq2 = "";
                    if (m_after6[4] != 0)
                    {
                        materials_eq2 = KCDatabase.Instance.MasterEquipments[m_after6[4]].Name + " x " + m_after6[5];
                        if (allCount.ContainsKey(m_after6[4]))
                            if (allCount[m_after6[4]] >= m_after6[5])
                            {
                                can_kaisu_equipment[1] = true;
                                row.Cells[11].Style = CanKaisu;
                            }
                            else
                                can_kaisu_equipment[1] = false;

                    }

                    int[] m_change = new int[0];
                    string materials_3 = "";
                    string materials_eq3 = "";
                    string Can_Upgrade = EquipmentData["upgrade"].ToString();

                    if (!Can_Upgrade.Contains("False"))
                    {
                        int Upgrade_To = EquipmentData["upgrade"][0].ToObject<int>();
                        int Upgrade_Grade = EquipmentData["upgrade"][1].ToObject<int>();

                        m_change = EquipmentData["resource"][3].ToObject<int[]>();
                        materials_3 = m_change[0] + "/" + m_change[2] + " (" + m_change[1] + "/" + m_change[3] + ")";

                        if (m_change[4] != 0)
                        {
                            materials_eq3 = KCDatabase.Instance.MasterEquipments[m_change[4]].Name + " x " + m_change[5];
                            if (allCount.ContainsKey(m_change[4]))
                                if (allCount[m_change[4]] >= m_change[5])
                                {
                                    can_kaisu_equipment[2] = true;
                                    row.Cells[13].Style = CanKaisu;
                                }
                                else
                                    can_kaisu_equipment[2] = false;

                        }

                        if (m_change[0] <= db.Material.DevelopmentMaterial && m_change[2] <= db.Material.ModdingMaterial)
                        {
                            sb.Clear();
                            sb.Append("변환 후 : " + KCDatabase.Instance.MasterEquipments[Upgrade_To].Name);
                            sb.Append(" ★" + Upgrade_Grade);
                            row.Cells[12].Style = CanKaisu;
                            row.Cells[12].ToolTipText = sb.ToString();
                            can_kaisu_material[2] = true;
                        }
                        else
                            can_kaisu_material[2] = false;

                    }

                    bool cond_resource = true;

                    if (db.Material.Fuel >= resources[0])
                        row.Cells[4].Style = CanKaisu;
                    else
                        cond_resource = false;

                    if (db.Material.Ammo >= resources[1])
                        row.Cells[5].Style = CanKaisu;
                    else
                        cond_resource = false;

                    if (db.Material.Steel >= resources[2])
                        row.Cells[6].Style = CanKaisu;
                    else
                        cond_resource = false;

                    if (db.Material.Bauxite >= resources[3])
                        row.Cells[7].Style = CanKaisu;
                    else
                        cond_resource = false;

                    if (can_kaisu_day)
                    {
                        if (KaisuChecked)
                        {
                            if (allCount.ContainsKey(eq.ID) && (cond_resource) && ((can_kaisu_equipment[0] && can_kaisu_material[0]) || (can_kaisu_equipment[1] && can_kaisu_material[1]) || (can_kaisu_equipment[2] && can_kaisu_material[2])))
                            {
                                row.SetValues(eq.ID, eq.IconType, eq.Name, kanmusu_list, resources[0], resources[1], resources[2], resources[3], materials_1, materials_eq1, materials_2, materials_eq2, materials_3, materials_eq3);
                                rows.Add(row);
                            }
                        }
                        else
                        {
                            row.SetValues(eq.ID, eq.IconType, eq.Name, kanmusu_list, resources[0], resources[1], resources[2], resources[3], materials_1, materials_eq1, materials_2, materials_eq2, materials_3, materials_eq3);
                            rows.Add(row);
                        }
                    }
                }
            }

            this.EqType.AutoSizeMode =
            this.Resource_Fuel.AutoSizeMode =
            this.Resource_Ammo.AutoSizeMode =
            this.Resource_Steel.AutoSizeMode =
            this.Resource_Baux.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            this.AkashiListView.Rows.AddRange(rows.ToArray());
            this.AkashiListView.Sort(this.EqType, ListSortDirection.Ascending);

        }


        public DialogAkashilist(int shipID)
            : this()
        {



        }

        private void TextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.TextSearch_TextChanged(sender, e);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }


        private void DialogAlbumMasterShip_Load(object sender, EventArgs e)
        {

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumShip]);

        }

        private void TextSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TextSearch.Text))
                return;


            bool Search(string searchWord)
            {
                var target =
                    this.AkashiListView.Rows.OfType<DataGridViewRow>()
                    .Select(r => KCDatabase.Instance.MasterEquipments[(int)r.Cells[this.EqID.Index].Value])
                    .FirstOrDefault(
                        eq => Calculator.ToHiragana(eq.Name.ToLower()).Contains(searchWord));

                if (target != null)
                {
                    this.AkashiListView.FirstDisplayedScrollingRowIndex = this.AkashiListView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[this.EqID.Index].Value == target.EquipmentID).Index;
                    return true;
                }
                return false;
            }

            if (!Search(Calculator.ToHiragana(this.TextSearch.Text.ToLower())))
                Search(Calculator.RomaToHira(this.TextSearch.Text));
        }


        private void DialogAlbumMasterShip_FormClosed(object sender, FormClosedEventArgs e)
        {

            ResourceManager.DestroyIcon(this.Icon);

        }

        private void ShipView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BasePanelShipGirl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void ShipView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }



        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Btn_Day0_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day0.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(0, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Btn_Day1_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day1.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(1, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Btn_Day2_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day2.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(2, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Btn_Day3_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day3.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(3, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Btn_Day4_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day4.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(4, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Btn_Day5_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day5.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(5, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Btn_Day6_Click(object sender, EventArgs e)
        {
            this.Btn_BackColor_Reset();
            this.Btn_Day6.BackColor = SystemColors.ActiveCaption;
            this.Update_DataList(6, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Material_Check_Click(object sender, EventArgs e)
        {
            this.Update_DataList(this.current_day, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void Can_KaisuCheck_Click(object sender, EventArgs e)
        {
            this.Update_DataList(this.current_day, this.CanMaterial_CheckBox.Checked, this.Can_KaisuCheck.Checked);
        }

        private void EqListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void AkashiListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SelectionChanged(Object sender, EventArgs e)
        {
            this.AkashiListView.ClearSelection();
        }
    }
}
