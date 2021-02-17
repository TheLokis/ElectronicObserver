using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Support;
using ElectronicObserver.Utility.Storage;
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
	public partial class DialogAlbumMasterEquipment : Form
	{


		public DialogAlbumMasterEquipment()
		{
            this.InitializeComponent();

            this.TitleFirepower.ImageList =
            this.TitleTorpedo.ImageList =
            this.TitleAA.ImageList =
            this.TitleArmor.ImageList =
            this.TitleASW.ImageList =
            this.TitleEvasion.ImageList =
            this.TitleLOS.ImageList =
            this.TitleAccuracy.ImageList =
            this.TitleBomber.ImageList =
            this.TitleSpeed.ImageList =
            this.TitleRange.ImageList =
            this.TitleAircraftCost.ImageList =
            this.TitleAircraftDistance.ImageList =
            this.Rarity.ImageList =
            this.MaterialFuel.ImageList =
            this.MaterialAmmo.ImageList =
            this.MaterialSteel.ImageList =
            this.MaterialBauxite.ImageList =
				ResourceManager.Instance.Icons;

            this.EquipmentType.ImageList = ResourceManager.Instance.Equipments;

            this.TitleFirepower.ImageIndex = (int)ResourceManager.IconContent.ParameterFirepower;
            this.TitleTorpedo.ImageIndex = (int)ResourceManager.IconContent.ParameterTorpedo;
            this.TitleAA.ImageIndex = (int)ResourceManager.IconContent.ParameterAA;
            this.TitleArmor.ImageIndex = (int)ResourceManager.IconContent.ParameterArmor;
            this.TitleASW.ImageIndex = (int)ResourceManager.IconContent.ParameterASW;
            this.TitleEvasion.ImageIndex = (int)ResourceManager.IconContent.ParameterEvasion;
            this.TitleLOS.ImageIndex = (int)ResourceManager.IconContent.ParameterLOS;
            this.TitleAccuracy.ImageIndex = (int)ResourceManager.IconContent.ParameterAccuracy;
            this.TitleBomber.ImageIndex = (int)ResourceManager.IconContent.ParameterBomber;
            this.TitleSpeed.ImageIndex = (int)ResourceManager.IconContent.ParameterSpeed;
            this.TitleRange.ImageIndex = (int)ResourceManager.IconContent.ParameterRange;
            this.TitleAircraftCost.ImageIndex = (int)ResourceManager.IconContent.ParameterAircraftCost;
            this.TitleAircraftDistance.ImageIndex = (int)ResourceManager.IconContent.ParameterAircraftDistance;
            this.MaterialFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            this.MaterialAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            this.MaterialSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
            this.MaterialBauxite.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;


            this.BasePanelEquipment.Visible = false;


			ControlHelper.SetDoubleBuffered(this.TableEquipmentName);
			ControlHelper.SetDoubleBuffered(this.TableParameterMain);
			ControlHelper.SetDoubleBuffered(this.TableParameterSub);
			ControlHelper.SetDoubleBuffered(this.TableArsenal);

			ControlHelper.SetDoubleBuffered(this.EquipmentView);


            //Initialize EquipmentView
            this.EquipmentView.SuspendLayout();

            this.EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.EquipmentView_Icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


            this.EquipmentView.Rows.Clear();

			List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.MasterEquipments.Values.Count(s => s.Name != "なし"));

			foreach (var eq in KCDatabase.Instance.MasterEquipments.Values)
			{

				if (eq.Name == "なし") continue;

				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(this.EquipmentView);
				row.SetValues(eq.EquipmentID, eq.IconType, FormMain.Instance.Translator.GetTranslation(eq.CategoryTypeInstance.Name, Utility.TranslateType.EquipmentType), eq.Name);
				rows.Add(row);

			}
            this.EquipmentView.Rows.AddRange(rows.ToArray());

            this.EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.EquipmentView_Icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            //EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            this.EquipmentView.Sort(this.EquipmentView_ID, ListSortDirection.Ascending);
            this.EquipmentView.ResumeLayout();

		}

		public DialogAlbumMasterEquipment(int equipmentID)
			: this()
		{

            this.UpdateAlbumPage(equipmentID);


			if (KCDatabase.Instance.MasterEquipments.ContainsKey(equipmentID))
			{
				var row = this.EquipmentView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[this.EquipmentView_ID.Index].Value == equipmentID);
				if (row != null)
                    this.EquipmentView.FirstDisplayedScrollingRowIndex = row.Index;
			}
		}



		private void DialogAlbumMasterEquipment_Load(object sender, EventArgs e)
		{

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumEquipment]);

		}




		private void EquipmentView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			if (e.Column.Name == this.EquipmentView_Type.Name)
			{
				e.SortResult =
					KCDatabase.Instance.MasterEquipments[(int)this.EquipmentView.Rows[e.RowIndex1].Cells[0].Value].EquipmentType[2] -
					KCDatabase.Instance.MasterEquipments[(int)this.EquipmentView.Rows[e.RowIndex2].Cells[0].Value].EquipmentType[2];
			}
			else
			{
				e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);
			}

			if (e.SortResult == 0)
			{
				e.SortResult = (int)(this.EquipmentView.Rows[e.RowIndex1].Tag ?? 0) - (int)(this.EquipmentView.Rows[e.RowIndex2].Tag ?? 0);
			}

			e.Handled = true;
		}

		private void EquipmentView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < this.EquipmentView.Rows.Count; i++)
			{
                this.EquipmentView.Rows[i].Tag = i;
			}
		}


		private void EquipmentView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == this.EquipmentView_Icon.Index)
			{
				e.Value = ResourceManager.GetEquipmentImage((int)e.Value);
				e.FormattingApplied = true;
			}

		}



		private void EquipmentView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{

			if (e.RowIndex >= 0)
			{
				int equipmentID = (int)this.EquipmentView.Rows[e.RowIndex].Cells[0].Value;

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				{
                    this.Cursor = Cursors.AppStarting;
					new DialogAlbumMasterEquipment(equipmentID).Show(this.Owner);
                    this.Cursor = Cursors.Default;

				}
				else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
				{
                    this.UpdateAlbumPage(equipmentID);
				}
			}

		}




		private void UpdateAlbumPage(int equipmentID)
		{

			KCDatabase db = KCDatabase.Instance;
			EquipmentDataMaster eq = db.MasterEquipments[equipmentID];

			if (eq == null) return;


            this.BasePanelEquipment.SuspendLayout();


            //header
            this.EquipmentID.Tag = equipmentID;
            this.EquipmentID.Text = eq.EquipmentID.ToString();
            this.ToolTipInfo.SetToolTip(this.EquipmentID, string.Format("Type: [ {0} ]", string.Join(", ", eq.EquipmentType)));
            this.AlbumNo.Text = eq.AlbumNo.ToString();


            this.TableEquipmentName.SuspendLayout();

            this.EquipmentType.Text = FormMain.Instance.Translator.GetTranslation(db.EquipmentTypes[eq.EquipmentType[2]].Name, Utility.TranslateType.EquipmentType);

            {
				int eqicon = eq.IconType;
				if (eqicon >= (int)ResourceManager.EquipmentContent.Locked)
					eqicon = (int)ResourceManager.EquipmentContent.Unknown;
                this.EquipmentType.ImageIndex = eqicon;

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("장착가능함종:");
				foreach (var stype in KCDatabase.Instance.ShipTypes.Values)
				{
                    if (stype.EquippableCategories.Contains((int)eq.CategoryType))
                        sb.AppendLine(FormMain.Instance.Translator.GetTranslation(stype.Name, Utility.TranslateType.ShipType));
                }
                this.ToolTipInfo.SetToolTip(this.EquipmentType, this.GetEquippableShips(equipmentID));
            }
            this.EquipmentName.Text = eq.Name;
            this.ToolTipInfo.SetToolTip(this.EquipmentName, "(우클릭으로 복사)");

            this.TableEquipmentName.ResumeLayout();


            //main parameter
            this.TableParameterMain.SuspendLayout();

            this.SetParameterText(this.Firepower, eq.Firepower);
            this.SetParameterText(this.Torpedo, eq.Torpedo);
            this.SetParameterText(this.AA, eq.AA);
            this.SetParameterText(this.Armor, eq.Armor);
            this.SetParameterText(this.ASW, eq.ASW);
            this.SetParameterText(this.Evasion, eq.Evasion);
            this.SetParameterText(this.LOS, eq.LOS);
            this.SetParameterText(this.Accuracy, eq.Accuracy);
            this.SetParameterText(this.Bomber, eq.Bomber);

			if (eq.CategoryType == EquipmentTypes.Interceptor)
			{
                this.TitleAccuracy.Text = "대폭";
                this.TitleAccuracy.ImageIndex = (int)ResourceManager.IconContent.ParameterAntiBomber;
                this.TitleEvasion.Text = "영격";
                this.TitleEvasion.ImageIndex = (int)ResourceManager.IconContent.ParameterInterception;
            }
			else
			{
                this.TitleAccuracy.Text = "명중";
                this.TitleAccuracy.ImageIndex = (int)ResourceManager.IconContent.ParameterAccuracy;
                this.TitleEvasion.Text = "회피";
                this.TitleEvasion.ImageIndex = (int)ResourceManager.IconContent.ParameterEvasion;
            }

            this.TableParameterMain.ResumeLayout();


            //sub parameter
            this.TableParameterSub.SuspendLayout();

            this.Speed.Text = "없음"; //Constants.GetSpeed( eq.Speed );
            this.Range.Text = Constants.GetRange(eq.Range);
            this.Rarity.Text = Constants.GetEquipmentRarity(eq.Rarity);
            this.Rarity.ImageIndex = (int)ResourceManager.IconContent.RarityRed + Constants.GetEquipmentRarityID(eq.Rarity);     //checkme

            this.TableParameterSub.ResumeLayout();


			// aircraft
			if (eq.IsAircraft)
			{
                this.TableAircraft.SuspendLayout();
                this.AircraftCost.Text = eq.AircraftCost.ToString();
                this.ToolTipInfo.SetToolTip(this.AircraftCost, "배치시 보키소비：" + ((eq.IsCombatAircraft ? 18 : 4) * eq.AircraftCost));
                this.AircraftDistance.Text = eq.AircraftDistance.ToString();
                this.TableAircraft.ResumeLayout();
                this.TableAircraft.Visible = true;
			}
			else
			{
                this.TableAircraft.Visible = false;
			}


            //default equipment
            this.DefaultSlots.BeginUpdate();
            this.DefaultSlots.Items.Clear();
			foreach (var ship in KCDatabase.Instance.MasterShips.Values)
			{
				if (ship.DefaultSlot != null && ship.DefaultSlot.Contains(equipmentID))
				{
                    this.DefaultSlots.Items.Add(ship);
				}
			}
            this.DefaultSlots.EndUpdate();


            this.Description.Text = eq.Message;


            //arsenal
            this.TableArsenal.SuspendLayout();

            this.MaterialFuel.Text = eq.Material[0].ToString();
            this.MaterialAmmo.Text = eq.Material[1].ToString();
            this.MaterialSteel.Text = eq.Material[2].ToString();
            this.MaterialBauxite.Text = eq.Material[3].ToString();

            this.TableArsenal.ResumeLayout();



			//装備画像を読み込んでみる
			{
                var img =
                        KCResourceHelper.LoadEquipmentImage(equipmentID, KCResourceHelper.ResourceTypeEquipmentCard) ??
                        KCResourceHelper.LoadEquipmentImage(equipmentID, KCResourceHelper.ResourceTypeEquipmentCardSmall);

                if (img != null)
                {
                    this.EquipmentImage.Image?.Dispose();
                    this.EquipmentImage.Image = img;
                }
				else
				{
					if (this.EquipmentImage.Image != null)
                        this.EquipmentImage.Image.Dispose();
                    this.EquipmentImage.Image = null;
				}
			}


            this.BasePanelEquipment.ResumeLayout();
            this.BasePanelEquipment.Visible = true;


			this.Text = "장비도감 - " + eq.Name;

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

        private string GetEquippableShips(int equipmentID)
        {
            var db = KCDatabase.Instance;
            var sb = new StringBuilder();
            sb.AppendLine("장착가능:");
            var eq = db.MasterEquipments[equipmentID];
            if (eq == null)
                return sb.ToString();
            int eqCategory = (int)eq.CategoryType;
            var specialShips = new Dictionary<ShipTypes, List<string>>();
            foreach (var ship in db.MasterShips.Values.Where(s => s.SpecialEquippableCategories != null))
            {
                bool usual = ship.ShipTypeInstance.EquippableCategories.Contains(eqCategory);
                bool special = ship.SpecialEquippableCategories.Contains(eqCategory);
                if (usual != special)
                {
                    if (specialShips.ContainsKey(ship.ShipType))
                        specialShips[ship.ShipType].Add(ship.NameWithClass);
                    else
                        specialShips.Add(ship.ShipType, new List<string>(new[] { ship.NameWithClass }));
                }
            }
            foreach (var shiptype in db.ShipTypes.Values)
            {
                if (shiptype.EquippableCategories.Contains(eqCategory))
                {
                    sb.Append(shiptype.Name);
                    if (specialShips.ContainsKey(shiptype.Type))
                    {
                        sb.Append(" (").Append(string.Join(", ", specialShips[shiptype.Type])).Append("を除く)");
                    }
                    sb.AppendLine();
                }
                else
                {
                    if (specialShips.ContainsKey(shiptype.Type))
                    {
                        sb.Append("○ ").AppendLine(string.Join(", ", specialShips[shiptype.Type]));
                    }
                }
            }
            if (eq.EquippableShipsAtExpansion.Any())
                sb.Append("[보강슬롯] ").AppendLine(string.Join(", ", eq.EquippableShipsAtExpansion.Select(id => db.MasterShips[id].NameWithClass)));
            return sb.ToString();
        }


        private void DefaultSlots_MouseDown(object sender, MouseEventArgs e)
		{

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				int index = this.DefaultSlots.IndexFromPoint(e.Location);
				if (index >= 0)
				{
                    this.Cursor = Cursors.AppStarting;
					new DialogAlbumMasterShip(((ShipDataMaster)this.DefaultSlots.Items[index]).ShipID).Show(this.Owner);
                    this.Cursor = Cursors.Default;
				}
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



		private void StripMenu_File_OutputCSVUser_Click(object sender, EventArgs e)
		{

			if (this.SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{

				try
				{

					using (StreamWriter sw = new StreamWriter(this.SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
					{

						sw.WriteLine("장비ID,도감번호,장비종류,장비명,대분류,도감카테고리ID,카테고리ID,아이콘ID,항공기그래픽ID,화력,뇌장,대공,장갑,대잠,회피,색적,운,명중,폭장,사정,레어도,해체시연료,해체시탄약,해체시강재,해체시보키,도감글,행동반경,배치비용");

						foreach (EquipmentDataMaster eq in KCDatabase.Instance.MasterEquipments.Values)
						{

							sw.WriteLine(string.Join(",",
								eq.EquipmentID,
								eq.AlbumNo,
                                CsvHelper.EscapeCsvCell(eq.CategoryTypeInstance.Name),
                                CsvHelper.EscapeCsvCell(eq.Name),
                                eq.EquipmentType[0],
								eq.EquipmentType[1],
								eq.EquipmentType[2],
								eq.EquipmentType[3],
								eq.EquipmentType[4],
								eq.Firepower,
								eq.Torpedo,
								eq.AA,
								eq.Armor,
								eq.ASW,
								eq.Evasion,
								eq.LOS,
								eq.Luck,
								eq.Accuracy,
								eq.Bomber,
								Constants.GetRange(eq.Range),
								Constants.GetEquipmentRarity(eq.Rarity),
								eq.Material[0],
								eq.Material[1],
								eq.Material[2],
								eq.Material[3],
                                CsvHelper.EscapeCsvCell(eq.Message),
                                eq.AircraftDistance,
								eq.AircraftCost
								));

						}

					}

				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "장비도감 CSV의 출력이 실패했습니다.");
					MessageBox.Show("장비도감 CSV의 출력이 실패했습니다. \r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

			}


		}


		private void StripMenu_File_OutputCSVData_Click(object sender, EventArgs e)
		{

			if (this.SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{

				try
				{

					using (StreamWriter sw = new StreamWriter(this.SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
					{

						sw.WriteLine("장비ID,도감번호,장비명,장비종1,장비종2,장비종3,장비종4,장비종5,화력,뇌장,대공,장갑,대잠,회피,색적,운,명중,폭장,사정,레어도,해체시연료,해체시탄약,해체시강재,해체시보키,도감글,행동반경,배치비용");

						foreach (EquipmentDataMaster eq in KCDatabase.Instance.MasterEquipments.Values)
						{

							sw.WriteLine(string.Join(",",
								eq.EquipmentID,
								eq.AlbumNo,
                                CsvHelper.EscapeCsvCell(eq.Name),
                                eq.EquipmentType[0],
								eq.EquipmentType[1],
								eq.EquipmentType[2],
								eq.EquipmentType[3],
								eq.EquipmentType[4],
								eq.Firepower,
								eq.Torpedo,
								eq.AA,
								eq.Armor,
								eq.ASW,
								eq.Evasion,
								eq.LOS,
								eq.Luck,
								eq.Accuracy,
								eq.Bomber,
								eq.Range,
								eq.Rarity,
								eq.Material[0],
								eq.Material[1],
								eq.Material[2],
								eq.Material[3],
                                CsvHelper.EscapeCsvCell(eq.Message),
                                eq.AircraftDistance,
								eq.AircraftCost
								));

						}

					}

				}
				catch (Exception ex)
				{

                    Utility.ErrorReporter.SendErrorReport(ex, "장비도감 CSV의 출력이 실패했습니다.");
                    MessageBox.Show("장비도감 CSV의 출력이 실패했습니다. \r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

			}

		}


		private void TextSearch_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.TextSearch.Text))
				return;


			bool Search(string searchWord)
			{
				var target =
                    this.EquipmentView.Rows.OfType<DataGridViewRow>()
					.Select(r => KCDatabase.Instance.MasterEquipments[(int)r.Cells[this.EquipmentView_ID.Index].Value])
					.FirstOrDefault(
						eq => Calculator.ToHiragana(eq.Name.ToLower()).Contains(searchWord));

				if (target != null)
				{
                    this.EquipmentView.FirstDisplayedScrollingRowIndex = this.EquipmentView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[this.EquipmentView_ID.Index].Value == target.EquipmentID).Index;
					return true;
				}
				return false;
			}

			if (!Search(Calculator.ToHiragana(this.TextSearch.Text.ToLower())))
				Search(Calculator.RomaToHira(this.TextSearch.Text));
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




		private void DialogAlbumMasterEquipment_FormClosed(object sender, FormClosedEventArgs e)
		{

			ResourceManager.DestroyIcon(this.Icon);

		}

		private void StripMenu_Edit_CopyEquipmentName_Click(object sender, EventArgs e)
		{
			var eq = KCDatabase.Instance.MasterEquipments[this.EquipmentID.Tag as int? ?? -1];
			if (eq != null)
				Clipboard.SetText(eq.Name);
			else
				System.Media.SystemSounds.Exclamation.Play();
		}

		private void EquipmentName_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				var eq = KCDatabase.Instance.MasterEquipments[this.EquipmentID.Tag as int? ?? -1];
				if (eq != null)
					Clipboard.SetText(eq.Name);
				else
					System.Media.SystemSounds.Exclamation.Play();
			}
		}

		private void StripMenu_Edit_CopyEquipmentData_Click(object sender, EventArgs e)
		{
			var eq = KCDatabase.Instance.MasterEquipments[this.EquipmentID.Tag as int? ?? -1];
			if (eq == null)
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			var sb = new StringBuilder();

			sb.AppendFormat("{0} {1}\r\n", eq.CategoryTypeInstance.Name, eq.Name);
			sb.AppendFormat("ID: {0} / 도감번호: {1} / 카테고리ID: [{2}]\r\n", eq.EquipmentID, eq.AlbumNo, string.Join(", ", eq.EquipmentType));

			sb.AppendLine();

			if (eq.Firepower != 0)
				sb.AppendFormat("화력: {0:+0;-0;0}\r\n", eq.Firepower);
			if (eq.Torpedo != 0)
				sb.AppendFormat("뇌장: {0:+0;-0;0}\r\n", eq.Torpedo);
			if (eq.AA != 0)
				sb.AppendFormat("대공: {0:+0;-0;0}\r\n", eq.AA);
			if (eq.Armor != 0)
				sb.AppendFormat("장갑: {0:+0;-0;0}\r\n", eq.Armor);
			if (eq.ASW != 0)
				sb.AppendFormat("대잠: {0:+0;-0;0}\r\n", eq.ASW);
			if (eq.Evasion != 0)
				sb.AppendFormat("{0}: {1:+0;-0;0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? "영격" : "회피", eq.Evasion);
			if (eq.LOS != 0)
				sb.AppendFormat("색적: {0:+0;-0;0}\r\n", eq.LOS);
			if (eq.Accuracy != 0)
				sb.AppendFormat("{0}: {1:+0;-0;0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? "대폭" : "명중", eq.Accuracy);
			if (eq.Bomber != 0)
				sb.AppendFormat("폭장: {0:+0;-0;0}\r\n", eq.Bomber);
			if (eq.Luck != 0)
				sb.AppendFormat("운: {0:+0;-0;0}\r\n", eq.Luck);

			if (eq.Range > 0)
				sb.Append("사정: ").AppendLine(Constants.GetRange(eq.Range));

			if (eq.AircraftCost > 0)
				sb.AppendFormat("배치비용: {0}\r\n", eq.AircraftCost);
			if (eq.AircraftDistance > 0)
				sb.AppendFormat("행동반경: {0}\r\n", eq.AircraftDistance);

			sb.AppendLine();

			sb.AppendFormat("레어도: {0}\r\n", Constants.GetEquipmentRarity(eq.Rarity));
			sb.AppendFormat("해체자재: {0}\r\n", string.Join(" / ", eq.Material));

			sb.AppendLine();

			sb.AppendFormat("도감설명: \r\n{0}\r\n",
				!string.IsNullOrWhiteSpace(eq.Message) ? eq.Message : "(불명)");

			sb.AppendLine();

			sb.AppendLine("초기장비/개발:");
			string result = this.GetAppearingArea(eq.EquipmentID);
			if (string.IsNullOrWhiteSpace(result))
				result = "(불명)\r\n";
			sb.AppendLine(result);


			Clipboard.SetText(sb.ToString());
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

			int eqID = this.EquipmentID.Tag as int? ?? -1;
			var eq = KCDatabase.Instance.MasterEquipments[eqID];

			if (eq == null)
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			string result = this.GetAppearingArea(eqID);

			if (string.IsNullOrWhiteSpace(result))
			{
				result = eq.Name + " 의 초기장비함 ・ 개발 레시피를 알수없습니다.";
			}

			MessageBox.Show(result, "입수방법보기", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}


		private void StripMenu_Edit_GoogleEquipmentName_Click(object sender, EventArgs e)
		{
			var eq = KCDatabase.Instance.MasterEquipments[this.EquipmentID.Tag as int? ?? -1];
			if (eq == null)
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			try
			{

				// google <装備名> 艦これ
				System.Diagnostics.Process.Start(@"https://www.google.co.jp/search?q=%22" + Uri.EscapeDataString(eq.Name.Replace("+", "＋")) + "%22+%E8%89%A6%E3%81%93%E3%82%8C");

			}
			catch (Exception ex)
			{
				Utility.ErrorReporter.SendErrorReport(ex, "함선명의 구글 검색에 실패했습니다.");
			}
		}


	}
}
