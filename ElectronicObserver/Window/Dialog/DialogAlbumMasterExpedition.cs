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
            this.InitializeComponent();

            this.ExpeditionView.SuspendLayout();

            this.EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            this.ExpeditionView.Rows.Clear();

            List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.Mission.Values.Count(s => s.Name != "없음"));
            // 원정 리스트 만들기
            foreach (var ex in KCDatabase.Instance.Mission.Values)
            {

                if (ex.Name == "없음") continue;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.ExpeditionView);
                row.SetValues(ex.MissionID, ex.Name);
                rows.Add(row);

            }

            this.ExpeditionView.Rows.AddRange(rows.ToArray());

            this.EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            this.ExpeditionView.Sort(this.EquipmentView_ID, ListSortDirection.Ascending);
            this.ExpeditionView.ResumeLayout();

            this.BasePanelExpedition.Visible = false;

        }

        public DialogAlbumMasterExpedition(int equipmentID)
            : this()
        {

            this.UpdateAlbumPage(equipmentID);


            if (KCDatabase.Instance.MasterEquipments.ContainsKey(equipmentID))
            {
                var row = this.ExpeditionView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[this.EquipmentView_ID.Index].Value == equipmentID);
                if (row != null)
                    this.ExpeditionView.FirstDisplayedScrollingRowIndex = row.Index;
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
                int Expeditionid = (int)this.ExpeditionView.Rows[e.RowIndex].Cells[0].Value;

                if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
                {
                    this.Cursor = Cursors.AppStarting;
                    new DialogAlbumMasterExpedition(Expeditionid).Show(this.Owner);
                    this.Cursor = Cursors.Default;

                }
                else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
                {
                    this.UpdateAlbumPage(Expeditionid);
                }
            }

        }




        private void UpdateAlbumPage(int expeditionId)
        {
            // 원정 디테일 표시
            KCDatabase db = KCDatabase.Instance;
            MissionData mis = db.Mission[expeditionId];

            if (mis == null) return;

            this.BasePanelExpedition.SuspendLayout();

            this.ExpeditionName.Text = mis.Name;
            this.ExpeditionID.Tag   = expeditionId;
            this.ExpeditionID.Text = mis.ID.ToString();

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

            this.Difficulty.Text = DifficultyText;

            this.TitleFuel.ImageList    =
            this.TitleAmmo.ImageList    =
            this.TitleBaux.ImageList    =
            this.TitleSteel.ImageList   =
            this.TitleUseFuel.ImageList =
            this.TitleUseAmmo.ImageList =
                ResourceManager.Instance.Icons;

            this.TitleFuel.ImageIndex   = (int)ResourceManager.IconContent.ResourceFuel;
            this.TitleAmmo.ImageIndex   = (int)ResourceManager.IconContent.ResourceAmmo;
            this.TitleSteel.ImageIndex  = (int)ResourceManager.IconContent.ResourceSteel;
            this.TitleBaux.ImageIndex   = (int)ResourceManager.IconContent.ResourceBauxite;

            this.TitleUseFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            this.TitleUseAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;

            int time = mis.Time / 60;
            int minutes = (mis.Time % 60);

            string timestring = "";
            if (time != 0) { timestring += time + "시간 "; }
            if (minutes != 0 ) { timestring += minutes + "분"; }

            this.ExpeditionTime.Text = timestring;

            // 작업 체크포인트 원정 체크
            var Data = FormMain.Instance.Translator.GetExpeditionData(expeditionId.ToString());
            if (Data == null)
            {
                Utility.Logger.Add(3, "해당 원정의 데이터 등록이 되지 않았습니다.");
                return;
            }

            float FuelAmount    = Convert.ToInt32(Data["Fuel"].ToString());
            float AmmoAmount    = Convert.ToInt32(Data["Ammo"].ToString());
            float SteelAmount   = Convert.ToInt32(Data["Steel"].ToString());
            float BauxAmount    = Convert.ToInt32(Data["Baux"].ToString());
            int AdmiralExps     = Convert.ToInt32(Data["AdmiralExp"].ToString());
            int KanmusuExp1     = Convert.ToInt32(Data["KanmusuExp1"].ToString());
            int KanmusuExp2     = Convert.ToInt32(Data["KanmusuExp2"].ToString());
            int FlagShipLevel   = Convert.ToInt32(Data["FlagShipLevel"].ToString());
            int ItemAid         = Convert.ToInt32(Data["ItemA"].ToString());
            int ItemBid         = Convert.ToInt32(Data["ItemB"].ToString());
            int ItemAAmount     = Convert.ToInt32(Data["ItemAAmount"].ToString());
            int ItemBAmount     = Convert.ToInt32(Data["ItemBAmount"].ToString());
            int FleetLevel      = Convert.ToInt32(Data["FleetLevel"].ToString());
            int DrumBonus       = 0;

            string Desc         = Data["Desc"].ToString();
            string DrunNeed     = string.Empty;
            string DrunKanmusu  = string.Empty;


            if (Data["DrunNeed"] != null)
                if (string.IsNullOrEmpty(Data["DrunNeed"].ToString()) == false)
                    DrunNeed = Data["DrunNeed"].ToString();

            if (Data["DrumBonus"] != null)
                if (string.IsNullOrEmpty(Data["DrumBonus"].ToString()) == false)
                    DrumBonus = int.Parse(Data["DrumBonus"].ToString());

            if (Data["DrunKanmusu"] != null)
                if (string.IsNullOrEmpty(Data["DrunKanmusu"].ToString()) == false)
                    DrunKanmusu = Data["DrunKanmusu"].ToString();

            this.UseFuel.Text = (mis.Fuel * 100) + "%";
            this.UseAmmo.Text = (mis.Ammo * 100) + "%";

            if (FuelAmount != 0) this.BaseFuel.Text = FuelAmount.ToString();
            else this.BaseFuel.Text = "";

            if (AmmoAmount != 0) this.BaseAmmo.Text = AmmoAmount.ToString();
            else this.BaseAmmo.Text = "";

            if (SteelAmount != 0) this.BaseSteel.Text = SteelAmount.ToString();
            else this.BaseSteel.Text = "";

            if (BauxAmount != 0) this.BaseBaux.Text = BauxAmount.ToString();
            else this.BaseBaux.Text = "";

            string ExpString = KanmusuExp1.ToString();
            if (KanmusuExp2 != 0)
                ExpString += " , " + KanmusuExp2;

            this.KanmusuExp.Text = ExpString;

            if (ItemAid != 0)
            {
                var Item = KCDatabase.Instance.MasterUseItems[ItemAid];
                this.ItemList1.Text = Item.Name + " X " + ItemAAmount;
                this.imageLabel19.Text = "아이템1";
            }
            else
            {
                this.imageLabel19.Text = "";
                this.ItemList1.Text = "";
            }

            if (ItemBid != 0)
            {
                var Item = KCDatabase.Instance.MasterUseItems[ItemBid];
                this.ItemList2.Text = Item.Name + " X " + ItemBAmount;
                this.imageLabel20.Text = "아이템2";
            }
            else
            {
                this.imageLabel20.Text = "";
                this.ItemList2.Text = "";
            }

            float TimeFuel  = ((60f / mis.Time) * FuelAmount);
            float TimeAmmo  = (60f / mis.Time) * AmmoAmount;
            float TimeSteel = (60f / mis.Time) * SteelAmount;
            float TimeBaux  = (60f / mis.Time) * BauxAmount;

            if (TimeFuel != 0) this.FuelByTime.Text = TimeFuel.ToString("N1");
            else this.FuelByTime.Text = "";

            if (TimeAmmo != 0) this.AmmoByTime.Text = TimeAmmo.ToString("N1");
            else this.AmmoByTime.Text = "";

            if (TimeSteel != 0) this.SteelByTime.Text = TimeSteel.ToString("N1");
            else this.SteelByTime.Text = "";

            if (TimeBaux != 0) this.BauxByTime.Text = TimeBaux.ToString("N1");
            else this.BauxByTime.Text = "";

            if (KanmusuExp2 != 0)
                this.KanmusuExp.Text = KanmusuExp1 + "," + KanmusuExp2;
            else
                this.KanmusuExp.Text = KanmusuExp1.ToString();

            this.ToolTipInfo.SetToolTip(this.KanmusuExp, "경험치가 2개로 표시된 경우, 양쪽의 확률이 각각 50%입니다.");

            this.ToolTipInfo.SetToolTip(this.ItemList1, "아이템 리스트의 왼쪽 아이템은 대성공 여부와 관계없이 50%, 오른쪽 아이템은 대성공할시 100%로 얻을 수 있습니다. \r\n단일 아이템을 여러개 획득하는 원정이라면, 각 개수를 얻을 확률은 모두 같습니다.");
            this.ToolTipInfo.SetToolTip(this.ItemList2, "아이템 리스트의 왼쪽 아이템은 대성공 여부와 관계없이 50%, 오른쪽 아이템은 대성공할시 100%로 얻을 수 있습니다. \r\n단일 아이템을 여러개 획득하는 원정이라면, 각 개수를 얻을 확률은 모두 같습니다.");

            this.ToolTipInfo.SetToolTip(this.DrunCount, "괄호안에 있는 수만큼 드럼통을 장착하면 대성공 확률 보너스를 받을 수 있습니다.");

            this.AdmiralExp.Text = AdmiralExps.ToString();

            this.FlagshipLv.Text = FlagShipLevel.ToString();
            if (FlagShipLevel.Equals(0)) this.FlagshipLv.Text = "제한 없음";

            this.FleetLv.Text = FleetLevel.ToString();
            if (FleetLevel.Equals(0)) this.FleetLv.Text = "제한 없음";
            if (FleetLevel.Equals(-1)) this.FleetLv.Text = "요검증";

            this.DrunCount.Text = DrunNeed.ToString() + "(" + DrumBonus + ")";
            this.KanmusuDrum.Text = DrunKanmusu.ToString();

            this.Description.Text = Desc;

            this.BasePanelExpedition.Visible = true;
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
                this.TextSearch_TextChanged(sender, e);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void DialogAlbumMasterExpedition_FormClosed(object sender, FormClosedEventArgs e)
        {

            ResourceManager.DestroyIcon(this.Icon);

        }

        private void StripMenu_Edit_CopyExpedition_Click(object sender, EventArgs e)
        {
            var eq = KCDatabase.Instance.MasterEquipments[this.ExpeditionID.Tag as int? ?? -1];
            if (eq != null)
                Clipboard.SetText(eq.Name);
            else
                System.Media.SystemSounds.Exclamation.Play();
        }

        private void ExpeditionName_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var mi = KCDatabase.Instance.Mission[this.ExpeditionID.Tag as int? ?? -1];
                if (mi != null)
                    Clipboard.SetText(mi.Name);
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
