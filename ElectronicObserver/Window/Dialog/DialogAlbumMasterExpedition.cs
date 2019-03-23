using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility;
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
using System.Xml.Linq;

namespace ElectronicObserver.Window.Dialog
{
    public partial class DialogAlbumMasterExpedition : Form
    {


        public DialogAlbumMasterExpedition()
        {
            InitializeComponent();

            ExpeditionView.SuspendLayout();

            EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            ExpeditionView.Rows.Clear();

            List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.Mission.Values.Count(s => s.Name != "없음"));
            // 원정 리스트 만들기
            foreach (var ex in KCDatabase.Instance.Mission.Values)
            {

                if (ex.Name == "없음") continue;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(ExpeditionView);
                row.SetValues(ex.MissionID, FormMain.Instance.Translator.GetTranslation(ex.Name, Utility.TranslationType.ExpeditionTitle));
                rows.Add(row);

            }

            ExpeditionView.Rows.AddRange(rows.ToArray());

            EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            ExpeditionView.Sort(EquipmentView_ID, ListSortDirection.Ascending);
            ExpeditionView.ResumeLayout();

            BasePanelExpedition.Visible = false;

        }

        public DialogAlbumMasterExpedition(int equipmentID)
            : this()
        {

            UpdateAlbumPage(equipmentID);


            if (KCDatabase.Instance.MasterEquipments.ContainsKey(equipmentID))
            {
                var row = ExpeditionView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[EquipmentView_ID.Index].Value == equipmentID);
                if (row != null)
                    ExpeditionView.FirstDisplayedScrollingRowIndex = row.Index;
            }
        }



        private void DialogAlbumMasterEquipment_Load(object sender, EventArgs e)
        {

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumEquipment]);

        }




        private void EquipmentView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {


        }

        private void EquipmentView_Sorted(object sender, EventArgs e)
        {


        }


        private void EquipmentView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {


        }



        private void EquipmentView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                int Expeditionid = (int)ExpeditionView.Rows[e.RowIndex].Cells[0].Value;

                if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
                {
                    Cursor = Cursors.AppStarting;
                    new DialogAlbumMasterExpedition(Expeditionid).Show(Owner);
                    Cursor = Cursors.Default;

                }
                else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
                {
                    UpdateAlbumPage(Expeditionid);
                }
            }

        }




        private void UpdateAlbumPage(int _ExpeditionID)
        {
            // 원정 디테일 표시
            KCDatabase db = KCDatabase.Instance;
            MissionData mis = db.Mission[_ExpeditionID];

            if (mis == null) return;



            BasePanelExpedition.SuspendLayout();

            ExpeditionName.Text = mis.Name;
            //header
            ExpeditionID.Tag = _ExpeditionID;
            ExpeditionID.Text = mis.ID.ToString();

            string DifficultyText = "E";

            switch (mis.Difficulty)
            {
                case 1:
                    DifficultyText = "E";
                    break;
                case 2:
                    DifficultyText = "E";
                    break;
                case 3:
                    DifficultyText = "C";
                    break;
                case 4:
                    DifficultyText = "B";
                    break;
                case 5:
                    DifficultyText = "A";
                    break;
                case 6:
                    DifficultyText = "S";
                    break;
            }

            Difficulty.Text = DifficultyText;

            TitleFuel.ImageList =
            TitleAmmo.ImageList =
            TitleBaux.ImageList =
            TitleSteel.ImageList =
            TitleUseFuel.ImageList =
            TitleUseAmmo.ImageList =
                ResourceManager.Instance.Icons;

            TitleFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            TitleAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            TitleSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
            TitleBaux.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;

            TitleUseFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            TitleUseAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;


            int time = mis.Time / 60;
            int minutes = (mis.Time % 60);

            string timestring = "";
            if (time != 0) { timestring += time + "시간 "; }
            if (minutes != 0 ) { timestring += minutes + "분"; }

            ExpeditionTime.Text = timestring;

            IEnumerable<XElement> DataList = FormMain.Instance.Translator.GetExpeditionData();
            IEnumerable<XElement> FoundData = DataList.Where(el =>
            {
                try
                {
                    if (el.Element("ID").Value.Equals(mis.ID.ToString())) return true;
                }
                catch
                {
                    return false;
                }
                return false;
            });

            if(FoundData.Count() == 0)
            {
                Utility.Logger.Add(3, "해당 원정의 데이터 등록이 되지 않았습니다.");
            }

            foreach (XElement el in FoundData)
            {
                float FuelAmount = Convert.ToInt32(el.Element("Fuel").Value);
                float AmmoAmount = Convert.ToInt32(el.Element("Ammo").Value);
                float SteelAmount = Convert.ToInt32(el.Element("Steel").Value);
                float BauxAmount = Convert.ToInt32(el.Element("Baux").Value);
                int AdmiralExpAmount = Convert.ToInt32(el.Element("AdmiralExp").Value);
                int KanmusuExp1 = Convert.ToInt32(el.Element("KanmusuExp1").Value);
                int KanmusuExp2 = Convert.ToInt32(el.Element("KanmusuExp2").Value);
                int FlagShipLevel = Convert.ToInt32(el.Element("FlagShipLevel").Value);
                int ItemAid = Convert.ToInt32(el.Element("ItemA").Value);
                int ItemBid = Convert.ToInt32(el.Element("ItemB").Value);
                int ItemAAmount = Convert.ToInt32(el.Element("ItemAAmount").Value);
                int ItemBAmount = Convert.ToInt32(el.Element("ItemBAmount").Value);
                int FleetLevel = Convert.ToInt32(el.Element("FleetLevel").Value);
                string Desc = el.Element("Desc").Value;
                string DrunNeed = el.Element("DrunNeed").Value;

                int DrumBonus = 0;
                if (el.Element("DrumBonus") != null)
                    if (el.Element("DrumBonus").Value != "")
                        DrumBonus = int.Parse(el.Element("DrumBonus").Value);

                string DrunKanmusu = el.Element("DrunKanmusu").Value;

                UseFuel.Text = (mis.Fuel * 100) + "%";
                UseAmmo.Text = (mis.Ammo * 100) + "%";

                if (FuelAmount != 0) BaseFuel.Text = FuelAmount.ToString();
                else BaseFuel.Text = "";

                if (AmmoAmount != 0) BaseAmmo.Text = AmmoAmount.ToString();
                else BaseAmmo.Text = "";

                if (SteelAmount != 0) BaseSteel.Text = SteelAmount.ToString();
                else BaseSteel.Text = "";

                if (BauxAmount != 0) BaseBaux.Text = BauxAmount.ToString();
                else BaseBaux.Text = "";

                string ExpString = KanmusuExp1.ToString();
                if (KanmusuExp2 != 0)
                    ExpString += " , " + KanmusuExp2;

                KanmusuExp.Text = ExpString;

                if (ItemAid != 0)
                {
                    var Item = KCDatabase.Instance.MasterUseItems[ItemAid];
                    ItemList1.Text = Item.Name + " X " + ItemAAmount;
                    imageLabel19.Text = "아이템1";
                }
                else
                {
                    imageLabel19.Text = "";
                    ItemList1.Text = "";
                }

                if (ItemBid != 0)
                {
                    var Item = KCDatabase.Instance.MasterUseItems[ItemBid];
                    ItemList2.Text = Item.Name + " X " + ItemBAmount;
                    imageLabel20.Text = "아이템2";
                }
                else
                {
                    imageLabel20.Text = "";
                    ItemList2.Text = "";
                }

                float TimeFuel = ((60f / mis.Time) * FuelAmount);
                float TimeAmmo = (60f / mis.Time) * AmmoAmount;
                float TimeSteel = (60f / mis.Time) * SteelAmount;
                float TimeBaux = (60f / mis.Time) * BauxAmount;

                if (TimeFuel != 0) FuelByTime.Text = TimeFuel.ToString("N1");
                else FuelByTime.Text = "";

                if (TimeAmmo != 0) AmmoByTime.Text = TimeAmmo.ToString("N1");
                else AmmoByTime.Text = "";

                if (TimeSteel != 0) SteelByTime.Text = TimeSteel.ToString("N1");
                else SteelByTime.Text = "";

                if (TimeBaux != 0) BauxByTime.Text = TimeBaux.ToString("N1");
                else BauxByTime.Text = "";

                if (KanmusuExp2 != 0)
                    KanmusuExp.Text = KanmusuExp1 + "," + KanmusuExp2;
                else
                    KanmusuExp.Text = KanmusuExp1.ToString();

                ToolTipInfo.SetToolTip(KanmusuExp, "경험치가 2개로 표시된 경우, 양쪽의 확률이 각각 50%입니다.");

                ToolTipInfo.SetToolTip(ItemList1, "아이템 리스트의 왼쪽 아이템은 대성공 여부와 관계없이 50%, 오른쪽 아이템은 대성공할시 100%로 얻을 수 있습니다. \r\n단일 아이템을 여러개 획득하는 원정이라면, 각 개수를 얻을 확률은 모두 같습니다.");
                ToolTipInfo.SetToolTip(ItemList2, "아이템 리스트의 왼쪽 아이템은 대성공 여부와 관계없이 50%, 오른쪽 아이템은 대성공할시 100%로 얻을 수 있습니다. \r\n단일 아이템을 여러개 획득하는 원정이라면, 각 개수를 얻을 확률은 모두 같습니다.");

                ToolTipInfo.SetToolTip(DrunCount, "괄호안에 있는 수만큼 드럼통을 장착하면 대성공 확률 보너스를 받을 수 있습니다.");

                AdmiralExp.Text = AdmiralExpAmount.ToString();

                FlagshipLv.Text = FlagShipLevel.ToString();
                if (FlagShipLevel.Equals(0)) FlagshipLv.Text = "제한 없음";

                FleetLv.Text = FleetLevel.ToString();
                if (FleetLevel.Equals(0)) FleetLv.Text = "제한 없음";
                if (FleetLevel.Equals(-1)) FleetLv.Text = "요검증";

                DrunCount.Text = DrunNeed.ToString() + "(" + DrumBonus + ")";
                KanmusuDrum.Text = DrunKanmusu.ToString();

                Description.Text = Desc;
            }

            BasePanelExpedition.Visible = true;

        }


        private void SetParameterText(ImageLabel label, int value)
        {

            if (value > 0)
            {
                label.ForeColor = SystemColors.ControlText;
                label.Text = "+" + value.ToString();
            }
            else if (value == 0)
            {
                label.ForeColor = Color.Silver;
                label.Text = "0";
            }
            else
            {
                label.ForeColor = Color.Red;
                label.Text = value.ToString();
            }

        }


        private void DefaultSlots_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {/*
				int index = DefaultSlots.IndexFromPoint(e.Location);
				if (index >= 0)
				{
					Cursor = Cursors.AppStarting;
					new DialogAlbumMasterShip(((ShipDataMaster)DefaultSlots.Items[index]).ShipID).Show(Owner);
					Cursor = Cursors.Default;
				}*/
            }
        }



        private void TableParameterMain_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
            /*/
			if ( e.Column == 0 )
				e.Graphics.DrawLine( Pens.Silver, e.CellBounds.Right - 1, e.CellBounds.Y, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
			//*/
        }

        private void TableParameterSub_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
        }



        private void TableArsenal_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
        }

        private void TableAircraft_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
        }

        private void TextSearch_TextChanged(object sender, EventArgs e)
        {/*
			if (string.IsNullOrWhiteSpace(TextSearch.Text))
				return;


			bool Search(string searchWord)
			{
				var target =
					EquipmentView.Rows.OfType<DataGridViewRow>()
					.Select(r => KCDatabase.Instance.MasterEquipments[(int)r.Cells[EquipmentView_ID.Index].Value])
					.FirstOrDefault(
						eq => Calculator.ToHiragana(eq.Name.ToLower()).Contains(searchWord));

				if (target != null)
				{
					EquipmentView.FirstDisplayedScrollingRowIndex = EquipmentView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[EquipmentView_ID.Index].Value == target.EquipmentID).Index;
					return true;
				}
				return false;
			}

			if (!Search(Calculator.ToHiragana(TextSearch.Text.ToLower())))
				Search(Calculator.RomaToHira(TextSearch.Text));*/
        }

        private void TextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextSearch_TextChanged(sender, e);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }




        private void DialogAlbumMasterEquipment_FormClosed(object sender, FormClosedEventArgs e)
        {

            ResourceManager.DestroyIcon(Icon);

        }

        private void StripMenu_Edit_CopyEquipmentName_Click(object sender, EventArgs e)
        {
            var eq = KCDatabase.Instance.MasterEquipments[ExpeditionID.Tag as int? ?? -1];
            if (eq != null)
                Clipboard.SetText(eq.Name);
            else
                System.Media.SystemSounds.Exclamation.Play();
        }

        private void EquipmentName_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var eq = KCDatabase.Instance.MasterEquipments[ExpeditionID.Tag as int? ?? -1];
                if (eq != null)
                    Clipboard.SetText(eq.Name);
                else
                    System.Media.SystemSounds.Exclamation.Play();
            }
        }

        private string GetAppearingArea(int equipmentID)
        {
            var sb = new StringBuilder();

            foreach (var ship in KCDatabase.Instance.MasterShips.Values
                .Where(s => s.DefaultSlot != null && s.DefaultSlot.Contains(equipmentID)))
            {
                sb.AppendLine(ship.NameWithClass);
            }

            foreach (var record in RecordManager.Instance.Development.Record
                .Where(r => r.EquipmentID == equipmentID)
                .Select(r => new
                {
                    r.Fuel,
                    r.Ammo,
                    r.Steel,
                    r.Bauxite
                })
                .Distinct()
                .OrderBy(r => r.Fuel)
                .ThenBy(r => r.Ammo)
                .ThenBy(r => r.Steel)
                .ThenBy(r => r.Bauxite)
                )
            {
                sb.AppendFormat("개발 {0} / {1} / {2} / {3}\r\n",
                    record.Fuel, record.Ammo, record.Steel, record.Bauxite);
            }

            return sb.ToString();
        }

        private void StripMenu_View_ShowAppearingArea_Click(object sender, EventArgs e)
        {

            int eqID = ExpeditionID.Tag as int? ?? -1;
            var eq = KCDatabase.Instance.MasterEquipments[eqID];

            if (eq == null)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            string result = GetAppearingArea(eqID);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = eq.Name + " 의 초기장비함 ・ 개발 레시피를 알수없습니다.";
            }

            MessageBox.Show(result, "입수방법보기", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void StripMenu_Edit_GoogleEquipmentName_Click(object sender, EventArgs e)
        {
            var eq = KCDatabase.Instance.MasterEquipments[ExpeditionID.Tag as int? ?? -1];
            if (eq == null)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            try
            {

                // google <装備名> 艦これ
                System.Diagnostics.Process.Start(@"https://www.google.co.jp/search?q=" + Uri.EscapeDataString(eq.Name) + "+%E8%89%A6%E3%81%93%E3%82%8C");

            }
            catch (Exception ex)
            {
                Utility.ErrorReporter.SendErrorReport(ex, "함선명의 구글 검색에 실패했습니다.");
            }
        }

        private void TableParameterMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TitleTorpedo_Click(object sender, EventArgs e)
        {

        }

        private void TitleAA_Click(object sender, EventArgs e)
        {

        }

        private void Description_Click(object sender, EventArgs e)
        {

        }

        private void TableAircraft_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TimeLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
