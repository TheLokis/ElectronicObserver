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

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogAlbumMasterExpedition : Form
	{


		public DialogAlbumMasterExpedition()
		{
			InitializeComponent();
            /*
			TitleFirepower.ImageList =
			TitleTorpedo.ImageList =
			TitleAA.ImageList =
			TitleArmor.ImageList =
			TitleASW.ImageList =
			TitleEvasion.ImageList =
			TitleLOS.ImageList =
			TitleAccuracy.ImageList =
			TitleBomber.ImageList =
			TitleSpeed.ImageList =
			TitleRange.ImageList =
			TitleAircraftCost.ImageList =
			TitleAircraftDistance.ImageList =
			Rarity.ImageList =
			MaterialFuel.ImageList =
			MaterialAmmo.ImageList =
			MaterialSteel.ImageList =
			MaterialBauxite.ImageList =
				ResourceManager.Instance.Icons;

			EquipmentType.ImageList = ResourceManager.Instance.Equipments;

			TitleFirepower.ImageIndex = (int)ResourceManager.IconContent.ParameterFirepower;
			TitleTorpedo.ImageIndex = (int)ResourceManager.IconContent.ParameterTorpedo;
			TitleAA.ImageIndex = (int)ResourceManager.IconContent.ParameterAA;
			TitleArmor.ImageIndex = (int)ResourceManager.IconContent.ParameterArmor;
			TitleASW.ImageIndex = (int)ResourceManager.IconContent.ParameterASW;
			TitleEvasion.ImageIndex = (int)ResourceManager.IconContent.ParameterEvasion;
			TitleLOS.ImageIndex = (int)ResourceManager.IconContent.ParameterLOS;
			TitleAccuracy.ImageIndex = (int)ResourceManager.IconContent.ParameterAccuracy;
			TitleBomber.ImageIndex = (int)ResourceManager.IconContent.ParameterBomber;
			TitleSpeed.ImageIndex = (int)ResourceManager.IconContent.ParameterSpeed;
			TitleRange.ImageIndex = (int)ResourceManager.IconContent.ParameterRange;
			TitleAircraftCost.ImageIndex = (int)ResourceManager.IconContent.ParameterAircraftCost;
			TitleAircraftDistance.ImageIndex = (int)ResourceManager.IconContent.ParameterAircraftDistance;
			MaterialFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
			MaterialAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
			MaterialSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
			MaterialBauxite.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;


			BasePanelEquipment.Visible = false;


			ControlHelper.SetDoubleBuffered(TableEquipmentName);
			ControlHelper.SetDoubleBuffered(TableParameterMain);
			ControlHelper.SetDoubleBuffered(TableParameterSub);
			ControlHelper.SetDoubleBuffered(TableArsenal);

			ControlHelper.SetDoubleBuffered(EquipmentView);

            */
			//Initialize EquipmentView
			EquipmentView.SuspendLayout();

			EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			EquipmentView_Icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			//EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


			EquipmentView.Rows.Clear();

			List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.MasterEquipments.Values.Count(s => s.Name != "なし"));

			foreach (var eq in KCDatabase.Instance.Mission.Values)
			{

				if (eq.Name == "なし") continue;

				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(EquipmentView);
				row.SetValues(eq.MissionID, FormMain.Instance.Translator.GetTranslation(eq.Name, Utility.TranslationType.ExpeditionTitle));
				rows.Add(row);

			}
			EquipmentView.Rows.AddRange(rows.ToArray());

			EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			EquipmentView_Icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			//EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

			EquipmentView.Sort(EquipmentView_ID, ListSortDirection.Ascending);
			EquipmentView.ResumeLayout();

		}

		public DialogAlbumMasterExpedition(int equipmentID)
			: this()
		{

			UpdateAlbumPage(equipmentID);


			if (KCDatabase.Instance.MasterEquipments.ContainsKey(equipmentID))
			{
				var row = EquipmentView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[EquipmentView_ID.Index].Value == equipmentID);
				if (row != null)
					EquipmentView.FirstDisplayedScrollingRowIndex = row.Index;
			}
		}



		private void DialogAlbumMasterEquipment_Load(object sender, EventArgs e)
		{

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumEquipment]);

		}




		private void EquipmentView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			if (e.Column.Name == EquipmentView_Type.Name)
			{
				e.SortResult =
					KCDatabase.Instance.MasterEquipments[(int)EquipmentView.Rows[e.RowIndex1].Cells[0].Value].EquipmentType[2] -
					KCDatabase.Instance.MasterEquipments[(int)EquipmentView.Rows[e.RowIndex2].Cells[0].Value].EquipmentType[2];
			}
			else
			{
				e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);
			}

			if (e.SortResult == 0)
			{
				e.SortResult = (int)(EquipmentView.Rows[e.RowIndex1].Tag ?? 0) - (int)(EquipmentView.Rows[e.RowIndex2].Tag ?? 0);
			}

			e.Handled = true;
		}

		private void EquipmentView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < EquipmentView.Rows.Count; i++)
			{
				EquipmentView.Rows[i].Tag = i;
			}
		}


		private void EquipmentView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == EquipmentView_Icon.Index)
			{
				e.Value = ResourceManager.GetEquipmentImage((int)e.Value);
				e.FormattingApplied = true;
			}

		}



		private void EquipmentView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{

			if (e.RowIndex >= 0)
			{
				int equipmentID = (int)EquipmentView.Rows[e.RowIndex].Cells[0].Value;

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				{
					Cursor = Cursors.AppStarting;
					new DialogAlbumMasterEquipment(equipmentID).Show(Owner);
					Cursor = Cursors.Default;

				}
				else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
				{
					UpdateAlbumPage(equipmentID);
				}
			}

		}




		private void UpdateAlbumPage(int equipmentID)
		{

			KCDatabase db = KCDatabase.Instance;
			EquipmentDataMaster eq = db.MasterEquipments[equipmentID];

			if (eq == null) return;


			BasePanelEquipment.SuspendLayout();


			//header
			EquipmentID.Tag = equipmentID;
			EquipmentID.Text = eq.EquipmentID.ToString();
			ToolTipInfo.SetToolTip(EquipmentID, string.Format("Type: [ {0} ]", string.Join(", ", eq.EquipmentType)));
            /*
            AlbumNo.Text = eq.AlbumNo.ToString();


			TableEquipmentName.SuspendLayout();

            EquipmentType.Text = FormMain.Instance.Translator.GetTranslation(db.EquipmentTypes[eq.EquipmentType[2]].Name, Utility.TranslationType.EquipmentType);

            {
				int eqicon = eq.IconType;
				if (eqicon >= (int)ResourceManager.EquipmentContent.Locked)
					eqicon = (int)ResourceManager.EquipmentContent.Unknown;
				EquipmentType.ImageIndex = eqicon;

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("장착가능함종:");
				foreach (var stype in KCDatabase.Instance.ShipTypes.Values)
				{
					if (stype.EquipmentType.Contains((int)eq.CategoryType))
                        sb.AppendLine(FormMain.Instance.Translator.GetTranslation(stype.Name, Utility.TranslationType.ShipTypes));
                }
				ToolTipInfo.SetToolTip(EquipmentType, sb.ToString());
			}
			EquipmentName.Text = eq.Name;
			ToolTipInfo.SetToolTip(EquipmentName, "(우클릭으로 복사)");

			TableEquipmentName.ResumeLayout();


			//main parameter
			TableParameterMain.SuspendLayout();

			SetParameterText(Firepower, eq.Firepower);
			SetParameterText(Torpedo, eq.Torpedo);
			SetParameterText(AA, eq.AA);
			SetParameterText(Armor, eq.Armor);
			SetParameterText(ASW, eq.ASW);
			SetParameterText(Evasion, eq.Evasion);
			SetParameterText(LOS, eq.LOS);
			SetParameterText(Accuracy, eq.Accuracy);
			SetParameterText(Bomber, eq.Bomber);

			if (eq.CategoryType == EquipmentTypes.Interceptor)
			{
				TitleAccuracy.Text = "대폭";
				TitleEvasion.Text = "영격";
			}
			else
			{
				TitleAccuracy.Text = "명중";
				TitleEvasion.Text = "회피";
			}

			TableParameterMain.ResumeLayout();


			//sub parameter
			TableParameterSub.SuspendLayout();

			Speed.Text = "없음"; //Constants.GetSpeed( eq.Speed );
			Range.Text = Constants.GetRange(eq.Range);
			Rarity.Text = Constants.GetEquipmentRarity(eq.Rarity);
			Rarity.ImageIndex = (int)ResourceManager.IconContent.RarityRed + Constants.GetEquipmentRarityID(eq.Rarity);     //checkme

			TableParameterSub.ResumeLayout();


			// aircraft
			if (eq.IsAircraft)
			{
				TableAircraft.SuspendLayout();
				AircraftCost.Text = eq.AircraftCost.ToString();
				ToolTipInfo.SetToolTip(AircraftCost, "배치시 보키소비：" + ((eq.IsCombatAircraft ? 18 : 4) * eq.AircraftCost));
				AircraftDistance.Text = eq.AircraftDistance.ToString();
				TableAircraft.ResumeLayout();
				TableAircraft.Visible = true;
			}
			else
			{
				TableAircraft.Visible = false;
			}


			//default equipment
			DefaultSlots.BeginUpdate();
			DefaultSlots.Items.Clear();
			foreach (var ship in KCDatabase.Instance.MasterShips.Values)
			{
				if (ship.DefaultSlot != null && ship.DefaultSlot.Contains(equipmentID))
				{
					DefaultSlots.Items.Add(ship);
				}
			}
			DefaultSlots.EndUpdate();


			Description.Text = eq.Message;


			//arsenal
			TableArsenal.SuspendLayout();

			MaterialFuel.Text = eq.Material[0].ToString();
			MaterialAmmo.Text = eq.Material[1].ToString();
			MaterialSteel.Text = eq.Material[2].ToString();
			MaterialBauxite.Text = eq.Material[3].ToString();

			TableArsenal.ResumeLayout();



			//装備画像を読み込んでみる
			{
				string path = string.Format(@"{0}\\resources\\image\\slotitem\\card\\{1:D3}.png", Utility.Configuration.Config.Connection.SaveDataPath, equipmentID);
				if (File.Exists(path))
				{
					try
					{

						EquipmentImage.Image = new Bitmap(path);

					}
					catch (Exception)
					{
						if (EquipmentImage.Image != null)
							EquipmentImage.Image.Dispose();
						EquipmentImage.Image = null;
					}
				}
				else
				{
					if (EquipmentImage.Image != null)
						EquipmentImage.Image.Dispose();
					EquipmentImage.Image = null;
				}
			}


			BasePanelEquipment.ResumeLayout();
			BasePanelEquipment.Visible = true;


			this.Text = "장비도감 - " + eq.Name;
            */
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
			var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
			if (eq != null)
				Clipboard.SetText(eq.Name);
			else
				System.Media.SystemSounds.Exclamation.Play();
		}

		private void EquipmentName_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
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

			int eqID = EquipmentID.Tag as int? ?? -1;
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
			var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
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
    }
}
