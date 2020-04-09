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
using System.Text.RegularExpressions;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogAlbumMasterShip : Form
	{

		private int _shipID;

		private ImageLabel[] Aircrafts;
		private ImageLabel[] Equipments;

		private int loadingResourceShipID;


		public DialogAlbumMasterShip()
		{
            this.InitializeComponent();

            this.Aircrafts = new ImageLabel[] { this.Aircraft1, this.Aircraft2, this.Aircraft3, this.Aircraft4, this.Aircraft5 };
            this.Equipments = new ImageLabel[] { this.Equipment1, this.Equipment2, this.Equipment3, this.Equipment4, this.Equipment5 };

            this.loadingResourceShipID = -1;

            this.TitleHP.ImageList =
            this.TitleFirepower.ImageList =
            this.TitleTorpedo.ImageList =
            this.TitleAA.ImageList =
            this.TitleArmor.ImageList =
            this.TitleASW.ImageList =
            this.TitleEvasion.ImageList =
            this.TitleLOS.ImageList =
            this.TitleLuck.ImageList =
            this.TitleSpeed.ImageList =
            this.TitleRange.ImageList =
            this.Rarity.ImageList =
            this.Fuel.ImageList =
            this.Ammo.ImageList =
            this.TitleBuildingTime.ImageList =
            this.MaterialFuel.ImageList =
            this.MaterialAmmo.ImageList =
            this.MaterialSteel.ImageList =
            this.MaterialBauxite.ImageList =
            this.PowerUpFirepower.ImageList =
            this.PowerUpTorpedo.ImageList =
            this.PowerUpAA.ImageList =
            this.PowerUpArmor.ImageList =
            this.RemodelBeforeLevel.ImageList =
            this.RemodelBeforeAmmo.ImageList =
            this.RemodelBeforeSteel.ImageList =
            this.RemodelAfterLevel.ImageList =
            this.RemodelAfterAmmo.ImageList =
            this.RemodelAfterSteel.ImageList =
				ResourceManager.Instance.Icons;

            this.TitleAirSuperiority.ImageList =
            this.TitleDayAttack.ImageList =
            this.TitleNightAttack.ImageList =
            this.Equipment1.ImageList =
            this.Equipment2.ImageList =
            this.Equipment3.ImageList =
            this.Equipment4.ImageList =
            this.Equipment5.ImageList =
				ResourceManager.Instance.Equipments;

            this.TitleHP.ImageIndex = (int)ResourceManager.IconContent.ParameterHP;
            this.TitleFirepower.ImageIndex = (int)ResourceManager.IconContent.ParameterFirepower;
            this.TitleTorpedo.ImageIndex = (int)ResourceManager.IconContent.ParameterTorpedo;
            this.TitleAA.ImageIndex = (int)ResourceManager.IconContent.ParameterAA;
            this.TitleArmor.ImageIndex = (int)ResourceManager.IconContent.ParameterArmor;
            this.TitleASW.ImageIndex = (int)ResourceManager.IconContent.ParameterASW;
            this.TitleEvasion.ImageIndex = (int)ResourceManager.IconContent.ParameterEvasion;
            this.TitleLOS.ImageIndex = (int)ResourceManager.IconContent.ParameterLOS;
            this.TitleLuck.ImageIndex = (int)ResourceManager.IconContent.ParameterLuck;
            this.TitleSpeed.ImageIndex = (int)ResourceManager.IconContent.ParameterSpeed;
            this.TitleRange.ImageIndex = (int)ResourceManager.IconContent.ParameterRange;
            this.Fuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            this.Ammo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            this.TitleBuildingTime.ImageIndex = (int)ResourceManager.IconContent.FormArsenal;
            this.MaterialFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
            this.MaterialAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            this.MaterialSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
            this.MaterialBauxite.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;
            this.PowerUpFirepower.ImageIndex = (int)ResourceManager.IconContent.ParameterFirepower;
            this.PowerUpTorpedo.ImageIndex = (int)ResourceManager.IconContent.ParameterTorpedo;
            this.PowerUpAA.ImageIndex = (int)ResourceManager.IconContent.ParameterAA;
            this.PowerUpArmor.ImageIndex = (int)ResourceManager.IconContent.ParameterArmor;
            this.RemodelBeforeAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            this.RemodelBeforeSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
            this.RemodelAfterAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
            this.RemodelAfterSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
            this.TitleAirSuperiority.ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter;
            this.TitleDayAttack.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;
            this.TitleNightAttack.ImageIndex = (int)ResourceManager.EquipmentContent.Torpedo;

            this.ParameterLevel.Value = this.ParameterLevel.Maximum = ExpTable.ShipMaximumLevel;


            this.TableBattle.Visible = false;
            this.BasePanelShipGirl.Visible = false;


			ControlHelper.SetDoubleBuffered(this.TableShipName);
			ControlHelper.SetDoubleBuffered(this.TableParameterMain);
			ControlHelper.SetDoubleBuffered(this.TableParameterSub);
			ControlHelper.SetDoubleBuffered(this.TableConsumption);
			ControlHelper.SetDoubleBuffered(this.TableEquipment);
			ControlHelper.SetDoubleBuffered(this.TableArsenal);
			ControlHelper.SetDoubleBuffered(this.TableRemodel);
			ControlHelper.SetDoubleBuffered(this.TableBattle);

			ControlHelper.SetDoubleBuffered(this.ShipView);


            //ShipView Initialize
            this.ShipView.SuspendLayout();

            this.ShipView_ShipID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.ShipView_ShipType.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


            this.ShipView.Rows.Clear();

			List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.MasterShips.Values.Count(s => s.Name != "なし"));

			foreach (var ship in KCDatabase.Instance.MasterShips.Values)
			{

				if (ship.Name == "なし") continue;

				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(this.ShipView);
                row.SetValues(ship.ShipID, FormMain.Instance.Translator.GetTranslation
					(KCDatabase.Instance.ShipTypes[(int)ship.ShipType].Name, Utility.DataType.ShipType), ship.NameWithClass);
                //row.SetValues(ship.ShipID, row.SetValues(ship.ShipID, ship.ShipTypeName, ship.NameWithClass));
                row.Cells[this.ShipView_ShipType.Index].Tag = ship.ShipType;
				row.Cells[this.ShipView_Name.Index].Tag = ship.IsAbyssalShip ? null : ship.NameReading;
				rows.Add(row);

			}
            this.ShipView.Rows.AddRange(rows.ToArray());

            this.ShipView_ShipID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.ShipView_ShipType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            this.ShipView.Sort(this.ShipView_ShipID, ListSortDirection.Ascending);
            this.ShipView.ResumeLayout();
		}

		public DialogAlbumMasterShip(int shipID)
			: this()
		{

            this.UpdateAlbumPage(shipID);


			if (KCDatabase.Instance.MasterShips.ContainsKey(shipID))
			{
				var row = this.ShipView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[this.ShipView_ShipID.Index].Value == shipID);
				if (row != null)
                    this.ShipView.FirstDisplayedScrollingRowIndex = row.Index;
			}

		}



		private void DialogAlbumMasterShip_Load(object sender, EventArgs e)
		{

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAlbumShip]);

		}




		private void ShipView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			if (e.Column.Index == this.ShipView_ShipType.Index)
			{
				e.SortResult = (int)this.ShipView[e.Column.Index, e.RowIndex1].Tag - (int)this.ShipView[e.Column.Index, e.RowIndex2].Tag;

			}
			else if (e.Column.Index == this.ShipView_Name.Index)
			{

				// 艦娘優先; 艦娘同士なら読みで比べる、深海棲艦同士なら名前で比べる

				string tag1 = this.ShipView[e.Column.Index, e.RowIndex1].Tag as string;
				string tag2 = this.ShipView[e.Column.Index, e.RowIndex2].Tag as string;

				if (tag1 != null)
				{
					if (tag2 != null)
						e.SortResult = tag1.CompareTo(tag2);
					else
						e.SortResult = -1;
				}
				else
				{
					if (tag2 != null)
						e.SortResult = 1;
					else
						e.SortResult = 0;
				}

				if (e.SortResult == 0)
					e.SortResult = ((string)e.CellValue1).CompareTo(e.CellValue2);

			}
			else
			{
				e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);
			}

			if (e.SortResult == 0)
			{
				e.SortResult = (int)(this.ShipView.Rows[e.RowIndex1].Tag ?? 0) - (int)(this.ShipView.Rows[e.RowIndex2].Tag ?? 0);
			}

			e.Handled = true;
		}

		private void ShipView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < this.ShipView.Rows.Count; i++)
			{
                this.ShipView.Rows[i].Tag = i;
			}

		}



		private void ShipView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{

			if (e.RowIndex >= 0)
			{
				int shipID = (int)this.ShipView.Rows[e.RowIndex].Cells[0].Value;

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				{
                    this.Cursor = Cursors.AppStarting;
					new DialogAlbumMasterShip(shipID).Show(this.Owner);
                    this.Cursor = Cursors.Default;

				}
				else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
				{
                    this.UpdateAlbumPage(shipID);
				}
			}

		}




		private void UpdateAlbumPage(int shipID)
		{

			KCDatabase db = KCDatabase.Instance;
			ShipDataMaster ship = db.MasterShips[shipID];

			if (ship == null) return;


            this.BasePanelShipGirl.SuspendLayout();

            //header
            this.TableShipName.SuspendLayout();
            this._shipID = shipID;
            this.ShipID.Text = ship.ShipID.ToString();
            this.AlbumNo.Text = ship.AlbumNo.ToString();

            this.ResourceName.Text = $"{ship.ResourceName} {ship.ResourceGraphicVersion}/{ship.ResourceVoiceVersion}/{ship.ResourcePortVoiceVersion}";
            this.ToolTipInfo.SetToolTip(this.ResourceName, string.Format("리소스파일 이름: {0}\r\n그래픽 ver. {1}\r\n음성 ver. {2}\r\n모항 음성 ver. {3}\r\n({4})",
				ship.ResourceName, ship.ResourceGraphicVersion, ship.ResourceVoiceVersion, ship.ResourcePortVoiceVersion, Constants.GetVoiceFlag(ship.VoiceFlag)));


            this.ShipType.Text = ship.IsLandBase ? 
				"육상기지" : FormMain.Instance.Translator.GetTranslation(db.ShipTypes[(int)ship.ShipType].Name, Utility.DataType.ShipType);
            {
                var tip = new StringBuilder();
                if (ship.IsAbyssalShip)
                    tip.AppendLine($"함급ID: {ship.ShipClass}");
                else if (Constants.GetShipClass(ship.ShipClass) == "불명")
                    tip.AppendLine($"함급불명: {ship.ShipClass}");
                else
                    tip.AppendLine($"{Constants.GetShipClass(ship.ShipClass)}: {ship.ShipClass}");
                tip.AppendLine();
                tip.AppendLine("장착가능：");
                tip.AppendLine(this.GetEquippableString(shipID));
                this.ToolTipInfo.SetToolTip(this.ShipType, tip.ToString());
            }

            this.ShipName.Text = ship.NameWithClass;
            this.ShipName.ForeColor = ship.GetShipNameColor(true);

            this.ToolTipInfo.SetToolTip(this.ShipName, (!ship.IsAbyssalShip ? ship.NameReading + "\r\n" : "") + "(우클릭으로 복사)");
            this.TableShipName.ResumeLayout();


            //main parameter
            this.TableParameterMain.SuspendLayout();

			if (!ship.IsAbyssalShip)
			{

                this.TitleParameterMin.Text = "초기값";
                this.TitleParameterMax.Text = "최대값";

                this.HPMin.Text = ship.HPMin.ToString();
                this.HPMax.Text = ship.HPMaxMarried.ToString();
                this.ToolTipInfo.SetToolTip(this.HPMin, string.Format("개수후: {0} (+{1})", ship.HPMaxModernized, ship.HPMaxModernizable));
                this.ToolTipInfo.SetToolTip(this.HPMax, string.Format("개수후: {0} (+{1})\r\n(최대 내구: {2})", ship.HPMaxMarriedModernized, ship.HPMaxMarriedModernizable, ship.HPMax));

                this.FirepowerMin.Text = ship.FirepowerMin.ToString();
                this.FirepowerMax.Text = ship.FirepowerMax.ToString();

                this.TorpedoMin.Text = ship.TorpedoMin.ToString();
                this.TorpedoMax.Text = ship.TorpedoMax.ToString();

                this.AAMin.Text = ship.AAMin.ToString();
                this.AAMax.Text = ship.AAMax.ToString();

                this.ArmorMin.Text = ship.ArmorMin.ToString();
                this.ArmorMax.Text = ship.ArmorMax.ToString();

                this.ASWMin.Text = this.GetParameterMinBound(ship.ASW);
                this.ASWMax.Text = this.GetParameterMax(ship.ASW);

                this.EvasionMin.Text = this.GetParameterMinBound(ship.Evasion);
                this.EvasionMax.Text = this.GetParameterMax(ship.Evasion);

                this.LOSMin.Text = this.GetParameterMinBound(ship.LOS);
                this.LOSMax.Text = this.GetParameterMax(ship.LOS);

                this.LuckMin.Text = ship.LuckMin.ToString();
                this.LuckMax.Text = ship.LuckMax.ToString();

			}
			else
			{

				int hp = ship.HPMin;
				int firepower = ship.FirepowerMax;
				int torpedo = ship.TorpedoMax;
				int aa = ship.AAMax;
				int armor = ship.ArmorMax;
				int asw = ship.ASW != null && ship.ASW.Maximum != ShipParameterRecord.Parameter.MaximumDefault ? ship.ASW.Maximum : 0;
				int evasion = ship.Evasion != null && ship.Evasion.Maximum != ShipParameterRecord.Parameter.MaximumDefault ? ship.Evasion.Maximum : 0;
				int los = ship.LOS != null && ship.LOS.Maximum != ShipParameterRecord.Parameter.MaximumDefault ? ship.LOS.Maximum : 0;
				int luck = ship.LuckMax;

				if (ship.DefaultSlot != null)
				{
					int count = ship.DefaultSlot.Count;
					for (int i = 0; i < count; i++)
					{
						EquipmentDataMaster eq = KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[i]];
						if (eq == null) continue;

						firepower += eq.Firepower;
						torpedo += eq.Torpedo;
						aa += eq.AA;
						armor += eq.Armor;
						asw += eq.ASW;
						evasion += eq.Evasion;
						los += eq.LOS;
						luck += eq.Luck;
					}
				}

                this.TitleParameterMin.Text = "기본값";
                this.TitleParameterMax.Text = "장비포함";

                this.HPMin.Text = ship.HPMin > 0 ? ship.HPMin.ToString() : "???";
                this.HPMax.Text = hp > 0 ? hp.ToString() : "???";
                this.ToolTipInfo.SetToolTip(this.HPMin, null);
                this.ToolTipInfo.SetToolTip(this.HPMax, null);

                this.FirepowerMin.Text = ship.FirepowerMax.ToString();
                this.FirepowerMax.Text = firepower.ToString();

                this.TorpedoMin.Text = ship.TorpedoMax.ToString();
                this.TorpedoMax.Text = torpedo.ToString();

                this.AAMin.Text = ship.AAMax.ToString();
                this.AAMax.Text = aa.ToString();

                this.ArmorMin.Text = ship.ArmorMax.ToString();
                this.ArmorMax.Text = armor.ToString();

                this.ASWMin.Text = ship.ASW != null && ship.ASW.Maximum != ShipParameterRecord.Parameter.MaximumDefault ? ship.ASW.Maximum.ToString() : "???";
                this.ASWMax.Text = asw.ToString();

                this.EvasionMin.Text = ship.Evasion != null && ship.Evasion.Maximum != ShipParameterRecord.Parameter.MaximumDefault ? ship.Evasion.Maximum.ToString() : "???";
                this.EvasionMax.Text = evasion.ToString();

                this.LOSMin.Text = ship.LOS != null && ship.LOS.Maximum != ShipParameterRecord.Parameter.MaximumDefault ? ship.LOS.Maximum.ToString() : "???";
                this.LOSMax.Text = los.ToString();

                this.LuckMin.Text = ship.LuckMax > 0 ? ship.LuckMax.ToString() : "???";
                this.LuckMax.Text = luck > 0 ? luck.ToString() : "???";

			}
            this.UpdateLevelParameter(ship.ShipID);

            this.TableParameterMain.ResumeLayout();


            //sub parameter
            this.TableParameterSub.SuspendLayout();

            this.Speed.Text = Constants.GetSpeed(ship.Speed);
			if (!ship.IsAbyssalShip)
			{
                this.Range.Text = Constants.GetRange(ship.Range);
                this.ToolTipInfo.SetToolTip(this.Range, null);
			}
			else
			{
				var availableEquipments = (ship.DefaultSlot ?? Enumerable.Repeat(-1, 5))
					.Select(id => KCDatabase.Instance.MasterEquipments[id])
					.Where(eq => eq != null);
                this.Range.Text = Constants.GetRange(Math.Max(ship.Range, availableEquipments.Any() ? availableEquipments.Max(eq => eq.Range) : 0));
                this.ToolTipInfo.SetToolTip(this.Range, "기본사정: " + Constants.GetRange(ship.Range));
			}
            this.Rarity.Text = Constants.GetShipRarity(ship.Rarity);
            this.Rarity.ImageIndex = (int)ResourceManager.IconContent.RarityRed + ship.Rarity;

            this.TableParameterSub.ResumeLayout();

            this.TableConsumption.SuspendLayout();

            this.Fuel.Text = ship.Fuel.ToString();
            this.Ammo.Text = ship.Ammo.ToString();

			string tooltiptext = string.Format(
				"입거시 소비:\r\nHP1당: 강재 {0:F2} / 연료 {1:F2}\r\n최대: 강재 {2} / 연료 {3}\r\n",
				(ship.Fuel * 0.06),
				(ship.Fuel * 0.032),
				(int)(ship.Fuel * 0.06 * (ship.HPMaxMarried - 1)),
				(int)(ship.Fuel * 0.032 * (ship.HPMaxMarried - 1))
				);

            this.ToolTipInfo.SetToolTip(this.TableConsumption, tooltiptext);
            this.ToolTipInfo.SetToolTip(this.TitleConsumption, tooltiptext);
            this.ToolTipInfo.SetToolTip(this.Fuel, tooltiptext);
            this.ToolTipInfo.SetToolTip(this.Ammo, tooltiptext);

            this.TableConsumption.ResumeLayout();

            this.Description.Text = ship.MessageAlbum != "" ? this.FormatDescription(ship.MessageAlbum) : this.FormatDescription(ship.MessageGet);
            this.Description.Tag = ship.MessageAlbum != "" ? 1 : 0;


            //equipment
            this.TableEquipment.SuspendLayout();

			for (int i = 0; i < this.Equipments.Length; i++)
			{

				if (ship.Aircraft[i] > 0 || i < ship.SlotSize)
                    this.Aircrafts[i].Text = ship.Aircraft[i].ToString();
				else
                    this.Aircrafts[i].Text = "";


                this.ToolTipInfo.SetToolTip(this.Equipments[i], null);

				if (ship.DefaultSlot == null)
				{
					if (i < ship.SlotSize)
					{
                        this.Equipments[i].Text = "???";
                        this.Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Unknown;
					}
					else
					{
                        this.Equipments[i].Text = "";
                        this.Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Locked;
					}

				}
				else if (ship.DefaultSlot[i] != -1)
				{
					EquipmentDataMaster eq = db.MasterEquipments[ship.DefaultSlot[i]];
					if (eq == null)
					{
                        // 破損データが入っていた場合
                        this.Equipments[i].Text = "(없음)";
                        this.Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Nothing;

					}
					else
					{

                        this.Equipments[i].Text = eq.Name;

						int eqicon = eq.EquipmentType[3];
						if (eqicon >= (int)ResourceManager.EquipmentContent.Locked)
							eqicon = (int)ResourceManager.EquipmentContent.Unknown;

                        this.Equipments[i].ImageIndex = eqicon;

						{
							StringBuilder sb = new StringBuilder();

							sb.AppendFormat("{0} {1} (ID: {2})\r\n", eq.CategoryTypeInstance.Name, eq.Name, eq.EquipmentID);
							if (eq.Firepower != 0) sb.AppendFormat("화력 {0:+0;-0}\r\n", eq.Firepower);
							if (eq.Torpedo != 0) sb.AppendFormat("뇌장 {0:+0;-0}\r\n", eq.Torpedo);
							if (eq.AA != 0) sb.AppendFormat("대공 {0:+0;-0}\r\n", eq.AA);
							if (eq.Armor != 0) sb.AppendFormat("장갑 {0:+0;-0}\r\n", eq.Armor);
							if (eq.ASW != 0) sb.AppendFormat("대잠 {0:+0;-0}\r\n", eq.ASW);
							if (eq.Evasion != 0) sb.AppendFormat("회피 {0:+0;-0}\r\n", eq.Evasion);
							if (eq.LOS != 0) sb.AppendFormat("색적 {0:+0;-0}\r\n", eq.LOS);
							if (eq.Accuracy != 0) sb.AppendFormat("명중 {0:+0;-0}\r\n", eq.Accuracy);
							if (eq.Bomber != 0) sb.AppendFormat("폭장 {0:+0;-0}\r\n", eq.Bomber);
							sb.AppendLine("(우클릭으로 도감에)");

                            this.ToolTipInfo.SetToolTip(this.Equipments[i], sb.ToString());
						}
					}

				}
				else if (i < ship.SlotSize)
				{
                    this.Equipments[i].Text = "(없음)";
                    this.Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Nothing;

				}
				else
				{
                    this.Equipments[i].Text = "";
                    this.Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Locked;
				}
			}

            this.TableEquipment.ResumeLayout();


            //arsenal
            this.TableArsenal.SuspendLayout();
            this.BuildingTime.Text = DateTimeHelper.ToTimeRemainString(new TimeSpan(0, ship.BuildingTime, 0));

            this.MaterialFuel.Text = ship.Material[0].ToString();
            this.MaterialAmmo.Text = ship.Material[1].ToString();
            this.MaterialSteel.Text = ship.Material[2].ToString();
            this.MaterialBauxite.Text = ship.Material[3].ToString();

            this.PowerUpFirepower.Text = ship.PowerUp[0].ToString();
            this.PowerUpTorpedo.Text = ship.PowerUp[1].ToString();
            this.PowerUpAA.Text = ship.PowerUp[2].ToString();
            this.PowerUpArmor.Text = ship.PowerUp[3].ToString();

            this.TableArsenal.ResumeLayout();


			//remodel
			if (!ship.IsAbyssalShip)
			{

                this.TableRemodel.SuspendLayout();

				if (ship.RemodelBeforeShipID == 0)
				{
                    this.RemodelBeforeShipName.Text = "(없음)";
                    this.ToolTipInfo.SetToolTip(this.RemodelBeforeShipName, null);
                    this.RemodelBeforeLevel.Text = "";
                    this.RemodelBeforeLevel.ImageIndex = -1;
                    this.ToolTipInfo.SetToolTip(this.RemodelBeforeLevel, null);
                    this.RemodelBeforeAmmo.Text = "-";
                    this.RemodelBeforeSteel.Text = "-";
				}
				else
				{
					ShipDataMaster sbefore = ship.RemodelBeforeShip;
                    this.RemodelBeforeShipName.Text = sbefore.Name;
                    this.ToolTipInfo.SetToolTip(this.RemodelBeforeShipName, "(왼클릭으로 열기, 우클릭으로 새창)");
                    this.RemodelBeforeLevel.Text = string.Format("Lv. {0}", sbefore.RemodelAfterLevel);
                    this.RemodelBeforeLevel.ImageIndex = GetRemodelItemImageIndex(sbefore);
                    this.ToolTipInfo.SetToolTip(this.RemodelBeforeLevel, GetRemodelItem(sbefore));
                    this.RemodelBeforeAmmo.Text = sbefore.RemodelAmmo.ToString();
                    this.RemodelBeforeSteel.Text = sbefore.RemodelSteel.ToString();
				}

				if (ship.RemodelAfterShipID == 0)
				{
                    this.RemodelAfterShipName.Text = "(없음)";
                    this.ToolTipInfo.SetToolTip(this.RemodelAfterShipName, null);
                    this.RemodelAfterLevel.Text = "";
                    this.RemodelAfterLevel.ImageIndex = -1;
                    this.ToolTipInfo.SetToolTip(this.RemodelAfterLevel, null);
                    this.RemodelAfterAmmo.Text = "-";
                    this.RemodelAfterSteel.Text = "-";
				}
				else
				{
                    this.RemodelAfterShipName.Text = ship.RemodelAfterShip.Name;
                    this.ToolTipInfo.SetToolTip(this.RemodelAfterShipName, "(왼클릭으로 열기, 우클릭으로 새창)");
                    this.RemodelAfterLevel.Text = string.Format("Lv. {0}", ship.RemodelAfterLevel);
                    this.RemodelAfterLevel.ImageIndex = GetRemodelItemImageIndex(ship);
                    this.ToolTipInfo.SetToolTip(this.RemodelAfterLevel, GetRemodelItem(ship));
                    this.RemodelAfterAmmo.Text = ship.RemodelAmmo.ToString();
                    this.RemodelAfterSteel.Text = ship.RemodelSteel.ToString();
				}
                this.TableRemodel.ResumeLayout();


                this.TableRemodel.Visible = true;
                this.TableBattle.Visible = false;


			}
			else
			{

                this.TableBattle.SuspendLayout();

                this.AirSuperiority.Text = Calculator.GetAirSuperiority(ship).ToString();
                this.DayAttack.Text = Constants.GetDayAttackKind(Calculator.GetDayAttackKind(ship.DefaultSlot?.ToArray(), ship.ShipID, -1));
                this.NightAttack.Text = Constants.GetNightAttackKind(Calculator.GetNightAttackKind(ship.DefaultSlot?.ToArray(), ship.ShipID, -1));

                this.TableBattle.ResumeLayout();

                this.TableRemodel.Visible = false;
                this.TableBattle.Visible = true;

			}


			if (this.ShipBanner.Image != null)
			{
				var img = this.ShipBanner.Image;
                this.ShipBanner.Image = null;
				img.Dispose();
			}
			if (!this.ImageLoader.IsBusy)
			{
                this.loadingResourceShipID = ship.ShipID;
                this.ImageLoader.RunWorkerAsync(ship.ShipID);
            }



            this.BasePanelShipGirl.ResumeLayout();
            this.BasePanelShipGirl.Visible = true;


			this.Text = "함선도감 - " + ship.NameWithClass;

		}


		private void UpdateLevelParameter(int shipID)
		{

			ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];

			if (ship == null)
				return;

			if (!ship.IsAbyssalShip)
			{
                this.ASWLevel.Text = this.EstimateParameter((int)this.ParameterLevel.Value, ship.ASW);
                this.EvasionLevel.Text = this.EstimateParameter((int)this.ParameterLevel.Value, ship.Evasion);
                this.LOSLevel.Text = this.EstimateParameter((int)this.ParameterLevel.Value, ship.LOS);
                this.ASWLevel.Visible =
                this.ASWSeparater.Visible =
                this.EvasionLevel.Visible =
                this.EvasionSeparater.Visible =
                this.LOSLevel.Visible =
                this.LOSSeparater.Visible = true;

			}
			else
			{
                this.ASWLevel.Visible =
                this.ASWSeparater.Visible =
                this.EvasionLevel.Visible =
                this.EvasionSeparater.Visible =
                this.LOSLevel.Visible =
                this.LOSSeparater.Visible = false;
			}
		}

		private string EstimateParameter(int level, ShipParameterRecord.Parameter param)
		{

			if (param == null || param.Maximum == ShipParameterRecord.Parameter.MaximumDefault)
				return "???";

			int min = (int)(param.MinimumEstMin + (param.Maximum - param.MinimumEstMin) * level / 99.0);
			int max = (int)(param.MinimumEstMax + (param.Maximum - param.MinimumEstMax) * level / 99.0);

			if (min == max)
				return min.ToString();
			else
				return $"{Math.Min(min, max)}～{Math.Max(min, max)}";
		}


		private string GetParameterMinBound(ShipParameterRecord.Parameter param)
		{

			if (param == null || param.MinimumEstMax == ShipParameterRecord.Parameter.MaximumDefault)
				return "???";
			else if (param.MinimumEstMin == param.MinimumEstMax)
				return param.MinimumEstMin.ToString();
			else if (param.MinimumEstMin == ShipParameterRecord.Parameter.MinimumDefault && param.MinimumEstMax == param.Maximum)
				return "???";
			else
				return $"{param.MinimumEstMin}～{param.MinimumEstMax}";

		}

		private string GetParameterMax(ShipParameterRecord.Parameter param)
		{

			if (param == null || param.Maximum == ShipParameterRecord.Parameter.MaximumDefault)
				return "???";
			else
				return param.Maximum.ToString();

		}

        private string GetEquippableString(int shipID)
        {
            var db = KCDatabase.Instance;
            var ship = db.MasterShips[shipID];
            if (ship == null)
                return "";
            return string.Join("\r\n", ship.EquippableCategories.Select(id => db.EquipmentTypes[id].Name)
               .Concat(db.MasterEquipments.Values.Where(eq => eq.EquippableShipsAtExpansion.Contains(shipID)).Select(eq => eq.Name + " (보강슬롯)")));
        }


        private void ParameterLevel_ValueChanged(object sender, EventArgs e)
		{
			if (this._shipID != -1)
			{
                this.LevelTimer.Start();
				//UpdateLevelParameter( _shipID );
			}
		}

		private void LevelTimer_Tick(object sender, EventArgs e)
		{
			if (this._shipID != -1)
                this.UpdateLevelParameter(this._shipID);
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

		private void TableConsumption_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		private void TableEquipment_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		private void TableArsenal_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}

		private void TableRemodel_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			if (e.Row % 2 == 1)
				e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		}



		private void RemodelBeforeShipName_MouseClick(object sender, MouseEventArgs e)
		{

			if (this._shipID == -1) return;
			var ship = KCDatabase.Instance.MasterShips[this._shipID];

			if (ship != null && ship.RemodelBeforeShipID != 0)
			{

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
					new DialogAlbumMasterShip(ship.RemodelBeforeShipID).Show(this.Owner);

				else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
                    this.UpdateAlbumPage(ship.RemodelBeforeShipID);
			}
		}

		private void RemodelAfterShipName_MouseClick(object sender, MouseEventArgs e)
		{

			if (this._shipID == -1) return;
			var ship = KCDatabase.Instance.MasterShips[this._shipID];

			if (ship != null && ship.RemodelAfterShipID != 0)
			{

				if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
					new DialogAlbumMasterShip(ship.RemodelAfterShipID).Show(this.Owner);

				else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
                    this.UpdateAlbumPage(ship.RemodelAfterShipID);
			}
		}



		private void Equipment_MouseClick(object sender, MouseEventArgs e)
		{

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{

				for (int i = 0; i < this.Equipments.Length; i++)
				{
					if (sender == this.Equipments[i])
					{

						if (this._shipID != -1)
						{
							ShipDataMaster ship = KCDatabase.Instance.MasterShips[this._shipID];

							if (ship != null && ship.DefaultSlot != null && i < ship.DefaultSlot.Count && KCDatabase.Instance.MasterEquipments.ContainsKey(ship.DefaultSlot[i]))
							{
                                this.Cursor = Cursors.AppStarting;
								new DialogAlbumMasterEquipment(ship.DefaultSlot[i]).Show(this.Owner);
                                this.Cursor = Cursors.Default;
							}
						}
					}
				}

			}
		}


		private static int GetRemodelItemImageIndex(ShipDataMaster ship)
		{
			return
				ship.NeedCatapult > 0 ? (int)ResourceManager.IconContent.ItemCatapult :
				ship.NeedActionReport > 0 ? (int)ResourceManager.IconContent.ItemActionReport :
				ship.NeedBlueprint > 0 ? (int)ResourceManager.IconContent.ItemBlueprint :
				-1;
		}

		private static string GetRemodelItem(ShipDataMaster ship)
		{
			StringBuilder sb = new StringBuilder();
			if (ship.NeedBlueprint > 0)
				sb.AppendLine("개장설계도: " + ship.NeedBlueprint);
			if (ship.NeedCatapult > 0)
				sb.AppendLine("시제갑판캐터펄트: " + ship.NeedCatapult);
			if (ship.NeedActionReport > 0)
				sb.AppendLine("전투상보: " + ship.NeedActionReport);

			return sb.ToString();
		}


		private void StripMenu_File_OutputCSVUser_Click(object sender, EventArgs e)
		{

			if (this.SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{

				try
				{

					using (StreamWriter sw = new StreamWriter(this.SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
					{

						sw.WriteLine("함선ID,도감번호,함급,함종,이름,음독,정렬순서,개장전,개장후,개장Lv,개장탄약,개장강재,개장설계도,캐터펄트,전투상보,개장단계,초기내구,결혼내구,최대내구,초기화력,최대화력,초기뇌장,최대뇌장,초기대공,최대대공,초기장갑,최대장갑,초기대잠,최대대잠,초기회피,최대회피,초기색적,최대색적,운,최대운,속력,사정,레어도,슬롯수,탑재수1,탑재수2,탑재수3,탑재수4,탑재수5,초기장비1,초기장비2,초기장비3,초기장비4,초기장비5,건조시간,해체연료,해체탄약,해체강재,해체보키,개조화력,개조뇌장,개조대공,개조장갑,드롭대사,도감대사,소비연료,소비탄약,대사,リソース名,이미지버전,음성버전,모항음성버전");

                        foreach (ShipDataMaster ship in KCDatabase.Instance.MasterShips.Values)
						{

							if (ship.Name == "なし") continue;

							sw.WriteLine(string.Join(",",
								ship.ShipID,
								ship.AlbumNo,
								ship.IsAbyssalShip ? "심해서함" : Constants.GetShipClass(ship.ShipClass),
                                CsvHelper.EscapeCsvCell(ship.ShipTypeName),
                                CsvHelper.EscapeCsvCell(ship.Name),
                                CsvHelper.EscapeCsvCell(ship.NameReading),
                                ship.SortID,
                                CsvHelper.EscapeCsvCell(ship.RemodelBeforeShipID > 0 ? ship.RemodelBeforeShip.Name : "-"),
                                CsvHelper.EscapeCsvCell(ship.RemodelAfterShipID > 0 ? ship.RemodelAfterShip.Name : "-"),
                                ship.RemodelAfterLevel,
								ship.RemodelAmmo,
								ship.RemodelSteel,
								ship.NeedBlueprint > 0 ? ship.NeedBlueprint + "장" : "-",
								ship.NeedCatapult > 0 ? ship.NeedCatapult + "개" : "-",
								ship.NeedActionReport > 0 ? ship.NeedActionReport + "장" : "-",
								ship.RemodelTier,
								ship.HPMin,
								ship.HPMaxMarried,
                                ship.HPMaxMarriedModernized,
                                ship.FirepowerMin,
								ship.FirepowerMax,
								ship.TorpedoMin,
								ship.TorpedoMax,
								ship.AAMin,
								ship.AAMax,
								ship.ArmorMin,
								ship.ArmorMax,
								ship.ASW != null && !ship.ASW.IsMinimumDefault ? ship.ASW.Minimum.ToString() : "???",
								ship.ASW != null && !ship.ASW.IsMaximumDefault ? ship.ASW.Maximum.ToString() : "???",
								ship.Evasion != null && !ship.Evasion.IsMinimumDefault ? ship.Evasion.Minimum.ToString() : "???",
								ship.Evasion != null && !ship.Evasion.IsMaximumDefault ? ship.Evasion.Maximum.ToString() : "???",
								ship.LOS != null && !ship.LOS.IsMinimumDefault ? ship.LOS.Minimum.ToString() : "???",
								ship.LOS != null && !ship.LOS.IsMaximumDefault ? ship.LOS.Maximum.ToString() : "???",
								ship.LuckMin,
								ship.LuckMax,
								Constants.GetSpeed(ship.Speed),
								Constants.GetRange(ship.Range),
								Constants.GetShipRarity(ship.Rarity),
								ship.SlotSize,
								ship.Aircraft[0],
								ship.Aircraft[1],
								ship.Aircraft[2],
								ship.Aircraft[3],
								ship.Aircraft[4],
								ship.DefaultSlot != null ? (ship.DefaultSlot[0] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[0]].Name : (ship.SlotSize > 0 ? "(없음)" : "")) : "???",
								ship.DefaultSlot != null ? (ship.DefaultSlot[1] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[1]].Name : (ship.SlotSize > 1 ? "(없음)" : "")) : "???",
								ship.DefaultSlot != null ? (ship.DefaultSlot[2] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[2]].Name : (ship.SlotSize > 2 ? "(없음)" : "")) : "???",
								ship.DefaultSlot != null ? (ship.DefaultSlot[3] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[3]].Name : (ship.SlotSize > 3 ? "(없음)" : "")) : "???",
								ship.DefaultSlot != null ? (ship.DefaultSlot[4] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[4]].Name : (ship.SlotSize > 4 ? "(없음)" : "")) : "???",
                                DateTimeHelper.ToTimeRemainString(TimeSpan.FromMinutes(ship.BuildingTime)),
                                ship.Material[0],
								ship.Material[1],
								ship.Material[2],
								ship.Material[3],
								ship.PowerUp[0],
								ship.PowerUp[1],
								ship.PowerUp[2],
								ship.PowerUp[3],
                                CsvHelper.EscapeCsvCell(ship.MessageGet),
                                CsvHelper.EscapeCsvCell(ship.MessageAlbum),
                                ship.Fuel,
								ship.Ammo,
								Constants.GetVoiceFlag(ship.VoiceFlag),
                                CsvHelper.EscapeCsvCell(ship.ResourceName),
                                ship.ResourceGraphicVersion,
								ship.ResourceVoiceVersion,
								ship.ResourcePortVoiceVersion
								));

						}

					}

				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "함선도감 CSV의 출력에 실패했습니다.");
					MessageBox.Show("함선도감 CSV의 출력에 실패했습니다. \r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

						sw.WriteLine(string.Format("함선ID,도감번호,함급,함종,이름,음독,정렬순서,개장전,개장후,개장Lv,개장탄약,개장강재,개장설계도,캐터펄트,전투상보,개장단계,초기내구,최대내구,결혼내구,개장내구,초기화력,최대화력,초기뇌장,최대뇌장,초기대공,최대대공,초기장갑,최대장갑,초기대잠최소,초기대잠최대,최대대잠,대잠{0}최소,대잠{0}최대,초기회피최소,초기회피최대,최대회피,회피{0}최소,회피{0}최대,초기색적최소,초기색적최대,최대색적,색적{0}최소,색적{0}최대,초기운,최대운,속력,사정,레어도,슬롯수,탑재수1,탑재수2,탑재수3,탑재수4,탑재수5,초기장비1,초기장비2,초기장비3,초기장비4,초기장비5,건조시간,해체연료,해체탄약,해체강재,해체보키,개수화력,개수뇌장,개수대공,개수장갑,드롭대사,도감대사,소모연료,소모탄약,음성,리소스이름,이미지버전,음성버전,모항음성버전", ExpTable.ShipMaximumLevel));

						foreach (ShipDataMaster ship in KCDatabase.Instance.MasterShips.Values)
						{

							sw.WriteLine(string.Join(",",
								ship.ShipID,
								ship.AlbumNo,
                                CsvHelper.EscapeCsvCell(ship.Name),
                                CsvHelper.EscapeCsvCell(ship.NameReading),
                                (int)ship.ShipType,
								ship.ShipClass,
                                ship.SortID,
                                ship.RemodelBeforeShipID,
								ship.RemodelAfterShipID,
								ship.RemodelAfterLevel,
								ship.RemodelAmmo,
								ship.RemodelSteel,
								ship.NeedBlueprint,
								ship.NeedCatapult,
								ship.NeedActionReport,
								ship.RemodelTier,
								ship.HPMin,
								ship.HPMax,
								ship.HPMaxMarried,
								ship.FirepowerMin,
								ship.FirepowerMax,
								ship.TorpedoMin,
								ship.TorpedoMax,
								ship.AAMin,
								ship.AAMax,
								ship.ArmorMin,
								ship.ArmorMax,
								ship.ASW?.MinimumEstMin ?? ShipParameterRecord.Parameter.MinimumDefault,
								ship.ASW?.MinimumEstMax ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.ASW?.Maximum ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.ASW?.GetEstParameterMin(ExpTable.ShipMaximumLevel) ?? ShipParameterRecord.Parameter.MinimumDefault,
								ship.ASW?.GetEstParameterMax(ExpTable.ShipMaximumLevel) ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.Evasion?.MinimumEstMin ?? ShipParameterRecord.Parameter.MinimumDefault,
								ship.Evasion?.MinimumEstMax ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.Evasion?.Maximum ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.Evasion?.GetEstParameterMin(ExpTable.ShipMaximumLevel) ?? ShipParameterRecord.Parameter.MinimumDefault,
								ship.Evasion?.GetEstParameterMax(ExpTable.ShipMaximumLevel) ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.LOS?.MinimumEstMin ?? ShipParameterRecord.Parameter.MinimumDefault,
								ship.LOS?.MinimumEstMax ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.LOS?.Maximum ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.LOS?.GetEstParameterMin(ExpTable.ShipMaximumLevel) ?? ShipParameterRecord.Parameter.MinimumDefault,
								ship.LOS?.GetEstParameterMax(ExpTable.ShipMaximumLevel) ?? ShipParameterRecord.Parameter.MaximumDefault,
								ship.LuckMin,
								ship.LuckMax,
								ship.Speed,
								ship.Range,
								ship.Rarity,
								ship.SlotSize,
								ship.Aircraft[0],
								ship.Aircraft[1],
								ship.Aircraft[2],
								ship.Aircraft[3],
								ship.Aircraft[4],
								ship.DefaultSlot?[0] ?? -1,
								ship.DefaultSlot?[1] ?? -1,
								ship.DefaultSlot?[2] ?? -1,
								ship.DefaultSlot?[3] ?? -1,
								ship.DefaultSlot?[4] ?? -1,
								ship.BuildingTime,
								ship.Material[0],
								ship.Material[1],
								ship.Material[2],
								ship.Material[3],
								ship.PowerUp[0],
								ship.PowerUp[1],
								ship.PowerUp[2],
								ship.PowerUp[3],
                                CsvHelper.EscapeCsvCell(ship.MessageGet),
                                CsvHelper.EscapeCsvCell(ship.MessageAlbum),
                                ship.Fuel,
								ship.Ammo,
								ship.VoiceFlag,
                                CsvHelper.EscapeCsvCell(ship.ResourceName),
                                ship.ResourceGraphicVersion,
								ship.ResourceVoiceVersion,
								ship.ResourcePortVoiceVersion
								));

						}

					}

				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, "함선도감 CSV의 출력에 실패했습니다.");
					MessageBox.Show("함선도감 CSV의 출력에 실패했습니다.\r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

			}

		}

        private void StripMenu_Edit_CopySpecialEquipmentTable_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("|대상함ID|대상함|장착가능|장착불가능|");
            sb.AppendLine("|--:|:--|:--|:--|");

            foreach (var ship in KCDatabase.Instance.MasterShips.Values)
            {
                if (ship.SpecialEquippableCategories == null)
                    continue;

                var add = ship.SpecialEquippableCategories.Except(ship.ShipTypeInstance.EquippableCategories);
                var sub = ship.ShipTypeInstance.EquippableCategories.Except(ship.SpecialEquippableCategories);

                sb.AppendLine($"|{ship.ShipID}|{ship.NameWithClass}|{string.Join(", ", add.Select(id => KCDatabase.Instance.EquipmentTypes[id].Name))}|{string.Join(", ", sub.Select(id => KCDatabase.Instance.EquipmentTypes[id].Name))}|");
            }

            sb.AppendLine();

            {
                var nyan = new Dictionary<int, List<int>>();

                foreach (var eq in KCDatabase.Instance.MasterEquipments.Values)
                {
                    if (!(eq.EquippableShipsAtExpansion?.Any() ?? false))
                        continue;

                    foreach (var shipid in eq.EquippableShipsAtExpansion)
                    {
                        if (nyan.ContainsKey(shipid))
                            nyan[shipid].Add(eq.EquipmentID);
                        else
                            nyan.Add(shipid, new List<int>() { eq.EquipmentID });
                    }
                }

                sb.AppendLine("|대상함ID|대상함|장착가능ID|장착가능|");
                sb.AppendLine("|--:|:--|:--|:--|");

                foreach (var pair in nyan.OrderBy(p => p.Key))
                {
                    sb.AppendLine($"|{pair.Key}|{KCDatabase.Instance.MasterShips[pair.Key].NameWithClass}|{string.Join(", ", pair.Value)}|{string.Join(", ", pair.Value.Select(id => KCDatabase.Instance.MasterEquipments[id].Name))}|");
                }

            }
            Clipboard.SetText(sb.ToString());
        }

        private void DialogAlbumMasterShip_FormClosed(object sender, FormClosedEventArgs e)
		{

			ResourceManager.DestroyIcon(this.Icon);

		}



		private void Description_Click(object sender, EventArgs e)
		{

			int tag = this.Description.Tag as int? ?? 0;
			ShipDataMaster ship = KCDatabase.Instance.MasterShips[this._shipID];

			if (ship == null) return;

			if (tag == 0 && ship.MessageAlbum.Length > 0)
			{
                this.Description.Text = this.FormatDescription(ship.MessageAlbum);
                this.Description.Tag = 1;

			}
			else
			{
                this.Description.Text = this.FormatDescription(ship.MessageGet);
                this.Description.Tag = 0;
			}
		}

        private string FormatDescription(string description)
        {
            // 本家の改行がアレなので、区切り文字+改行 以外の改行を削除する
            var regex = new Regex(@"([^、。,\.！？!\?])\r\n");
            return regex.Replace(description, "$1");
        }


        private void ResourceName_MouseClick(object sender, MouseEventArgs e)
		{

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{

				var ship = KCDatabase.Instance.MasterShips[this._shipID];
				if (ship != null)
				{
					Clipboard.SetData(DataFormats.StringFormat, ship.ResourceName);
				}
			}

		}



		private void StripMenu_Edit_EditParameter_Click(object sender, EventArgs e)
		{

			if (this._shipID <= 0)
			{
				MessageBox.Show("함선을 선택하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}

			using (var dialog = new DialogAlbumShipParameter(this._shipID))
			{
				dialog.ShowDialog(this);
                this.UpdateAlbumPage(this._shipID);
			}

		}



		private void ImageLoader_DoWork(object sender, DoWorkEventArgs e)
		{

			string resourceName = e.Argument as string;

			//System.Threading.Thread.Sleep( 2000 );		// for test

			try
			{
                e.Result = KCResourceHelper.LoadShipImage(e.Argument as int? ?? 0, false, KCResourceHelper.ResourceTypeShipBanner);
            }
            catch (Exception)
			{
				e.Result = null;
			}

		}

		private void ImageLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (this.ShipBanner.Image != null)
			{
				var img = this.ShipBanner.Image;
                this.ShipBanner.Image = null;
				img.Dispose();
			}

			if (this.loadingResourceShipID != this._shipID)
			{
				if (e.Result != null)
					((Bitmap)e.Result).Dispose();

				if (!this.ImageLoader.IsBusy)
				{
                    this.loadingResourceShipID = this._shipID;
					var ship = KCDatabase.Instance.MasterShips[this._shipID];
					if (ship != null)
                        this.ImageLoader.RunWorkerAsync(this._shipID);
                }

				return;
			}

			if (e.Result != null)
			{
                this.ShipBanner.Image = e.Result as Bitmap;
                this.loadingResourceShipID = -1;
			}

		}



		private void TextSearch_TextChanged(object sender, EventArgs e)
		{

			if (string.IsNullOrWhiteSpace(this.TextSearch.Text))
				return;


			bool Search(string searchWord)
			{
				var target =
                    this.ShipView.Rows.OfType<DataGridViewRow>()
					.Select(r => KCDatabase.Instance.MasterShips[(int)r.Cells[this.ShipView_ShipID.Index].Value])
					.FirstOrDefault(
					ship =>
						Calculator.ToHiragana(ship.NameWithClass.ToLower()).StartsWith(searchWord) ||
						Calculator.ToHiragana(ship.NameReading.ToLower()).StartsWith(searchWord));

				if (target != null)
				{
                    this.ShipView.FirstDisplayedScrollingRowIndex = this.ShipView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[this.ShipView_ShipID.Index].Value == target.ShipID).Index;
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

		private void StripMenu_Edit_CopyShipName_Click(object sender, EventArgs e)
		{
			var ship = KCDatabase.Instance.MasterShips[this._shipID];
			if (ship != null)
				Clipboard.SetText(ship.NameWithClass);
			else
				System.Media.SystemSounds.Exclamation.Play();
		}

		private void ShipName_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				var ship = KCDatabase.Instance.MasterShips[this._shipID];
				if (ship != null)
					Clipboard.SetText(ship.NameWithClass);
				else
					System.Media.SystemSounds.Exclamation.Play();
			}
		}

		private void StripMenu_Edit_CopyShipData_Click(object sender, EventArgs e)
		{
			var ship = KCDatabase.Instance.MasterShips[this._shipID];
			if (ship == null)
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			var sb = new StringBuilder();

			var slot = (ship.DefaultSlot ?? Enumerable.Repeat(-2, 5)).ToArray();

			sb.AppendFormat("{0} {1}\r\n", ship.ShipTypeName, ship.NameWithClass);
			sb.AppendFormat("ID: {0} / 도감번호: {1} / 리소스: {2} ver. {3} / {4} / {5} ({6})\r\n", ship.ShipID, ship.AlbumNo,
				ship.ResourceName, ship.ResourceGraphicVersion, ship.ResourceVoiceVersion, ship.ResourcePortVoiceVersion,
				Constants.GetVoiceFlag(ship.VoiceFlag));
			sb.AppendLine();
			if (!ship.IsAbyssalShip)
			{
				sb.AppendFormat("내구: {0} / {1}\r\n", ship.HPMin, ship.HPMaxMarried);
				sb.AppendFormat("화력: {0} / {1}\r\n", ship.FirepowerMin, ship.FirepowerMax);
				sb.AppendFormat("뇌장: {0} / {1}\r\n", ship.TorpedoMin, ship.TorpedoMax);
				sb.AppendFormat("대공: {0} / {1}\r\n", ship.AAMin, ship.AAMax);
				sb.AppendFormat("장갑: {0} / {1}\r\n", ship.ArmorMin, ship.ArmorMax);
				sb.AppendFormat("대잠: {0} / {1}\r\n", this.GetParameterMinBound(ship.ASW), this.GetParameterMax(ship.ASW));
				sb.AppendFormat("회피: {0} / {1}\r\n", this.GetParameterMinBound(ship.Evasion), this.GetParameterMax(ship.Evasion));
				sb.AppendFormat("색적: {0} / {1}\r\n", this.GetParameterMinBound(ship.LOS), this.GetParameterMax(ship.LOS));
				sb.AppendFormat("운: {0} / {1}\r\n", ship.LuckMin, ship.LuckMax);
				sb.AppendFormat("속력: {0} / 사정: {1}\r\n", Constants.GetSpeed(ship.Speed), Constants.GetRange(ship.Range));
				sb.AppendFormat("소비연탄: 연료 {0} / 탄약 {1}\r\n", ship.Fuel, ship.Ammo);
				sb.AppendFormat("레어도: {0}\r\n", Constants.GetShipRarity(ship.Rarity));
			}
			else
			{
				var availableEquipments = slot.Select(id => KCDatabase.Instance.MasterEquipments[id]).Where(eq => eq != null);
				int luckSum = ship.LuckMax + availableEquipments.Sum(eq => eq.Luck);
				sb.AppendFormat("내구: {0}\r\n", ship.HPMin > 0 ? ship.HPMin.ToString() : "???");
				sb.AppendFormat("화력: {0} / {1}\r\n", ship.FirepowerMin, ship.FirepowerMax + availableEquipments.Sum(eq => eq.Firepower));
				sb.AppendFormat("뇌장: {0} / {1}\r\n", ship.TorpedoMin, ship.TorpedoMax + availableEquipments.Sum(eq => eq.Torpedo));
				sb.AppendFormat("대공: {0} / {1}\r\n", ship.AAMin, ship.AAMax + availableEquipments.Sum(eq => eq.AA));
				sb.AppendFormat("장갑: {0} / {1}\r\n", ship.ArmorMin, ship.ArmorMax + availableEquipments.Sum(eq => eq.Armor));
				sb.AppendFormat("대잠: {0} / {1}\r\n", this.GetParameterMax(ship.ASW), (ship.ASW != null && !ship.ASW.IsMaximumDefault ? ship.ASW.Maximum : 0) + availableEquipments.Sum(eq => eq.ASW));
				sb.AppendFormat("회피: {0} / {1}\r\n", this.GetParameterMax(ship.Evasion), (ship.Evasion != null && !ship.Evasion.IsMaximumDefault ? ship.Evasion.Maximum : 0) + availableEquipments.Sum(eq => eq.Evasion));
				sb.AppendFormat("색적: {0} / {1}\r\n", this.GetParameterMax(ship.LOS), (ship.LOS != null && !ship.LOS.IsMaximumDefault ? ship.LOS.Maximum : 0) + availableEquipments.Sum(eq => eq.LOS));
				sb.AppendFormat("운: {0} / {1}\r\n", ship.LuckMin > 0 ? ship.LuckMin.ToString() : "???", luckSum > 0 ? luckSum.ToString() : "???");
				sb.AppendFormat("속력: {0} / 사정: {1}\r\n", Constants.GetSpeed(ship.Speed),
					Constants.GetRange(Math.Max(ship.Range, availableEquipments.Any() ? availableEquipments.Max(eq => eq.Range) : 0)));
				if (ship.Fuel > 0 || ship.Ammo > 0)
					sb.AppendFormat("소비연탄: 연료 {0} / 탄약 {1}\r\n", ship.Fuel, ship.Ammo);
				if (ship.Rarity > 0)
					sb.AppendFormat("레어도: {0}\r\n", Constants.GetShipRarity(ship.Rarity));
			}
			sb.AppendLine();
			sb.AppendLine("초기장비:");
			{
				for (int i = 0; i < slot.Length; i++)
				{
					string name;
					var eq = KCDatabase.Instance.MasterEquipments[slot[i]];
					if (eq == null && i >= ship.SlotSize)
						continue;

					if (eq != null)
						name = eq.Name;
					else if (slot[i] == -1)
						name = "(없음)";
					else
						name = "(불명)";

					sb.AppendFormat("[{0}] {1}\r\n", ship.Aircraft[i], name);
				}
			}
			sb.AppendLine();
			if (!ship.IsAbyssalShip)
			{
				sb.AppendFormat("건조시간: {0}\r\n", DateTimeHelper.ToTimeRemainString(TimeSpan.FromMinutes(ship.BuildingTime)));
				sb.AppendFormat("해체자원: {0}\r\n", string.Join(" / ", ship.Material));
				sb.AppendFormat("개장강화: {0}\r\n", string.Join(" / ", ship.PowerUp));
				if (ship.RemodelBeforeShipID != 0)
				{
					var before = ship.RemodelBeforeShip;
					var append = new List<string>(4)
					{
						"탄약 " + before.RemodelAmmo,
						"강재 " + before.RemodelSteel
					};
					if (before.NeedBlueprint > 0)
						append.Add("개장설계도 필요");
					if (before.NeedCatapult > 0)
						append.Add("캐터펄트 필요");
					if (before.NeedActionReport > 0)
						append.Add("전투상보 필요");

					sb.AppendFormat("개장전: {0} Lv. {1} ({2})\r\n",
						before.NameWithClass, before.RemodelAfterLevel, string.Join(", ", append));
				}
				else
				{
					sb.AppendLine("개장전: (없음)");
				}
				if (ship.RemodelAfterShipID != 0)
				{
					var append = new List<string>(4)
					{
						"탄약 " + ship.RemodelAmmo,
						"강재 " + ship.RemodelSteel
					};
					if (ship.NeedBlueprint > 0)
						append.Add("개장설계도 필요");
					if (ship.NeedCatapult > 0)
						append.Add("캐터펄트 필요");
					if (ship.NeedActionReport > 0)
						append.Add("전투상보 필요");

					sb.AppendFormat("개장후: {0} Lv. {1} ({2})\r\n",
						ship.RemodelAfterShip.NameWithClass, ship.RemodelAfterLevel, string.Join(", ", append));
				}
				else
				{
					sb.AppendLine("개장후: (없음)");
				}
				sb.AppendLine();
				sb.AppendFormat("도감대사: \r\n{0}\r\n\r\n입수대사 \r\n{1}\r\n\r\n",
					!string.IsNullOrWhiteSpace(ship.MessageAlbum) ? ship.MessageAlbum : "(불명)",
					!string.IsNullOrWhiteSpace(ship.MessageGet) ? ship.MessageGet : "(불명)");
			}

			sb.AppendLine("드랍해역:");
			{
				string result = this.GetAppearingArea(ship.ShipID);
				if (string.IsNullOrEmpty(result))
					result = "(불명)";
				sb.AppendLine(result);
			}

			Clipboard.SetText(sb.ToString());
		}



		private string GetAppearingArea(int shipID)
		{

			var ship = KCDatabase.Instance.MasterShips[shipID];
			if (ship == null)
				return string.Empty;

			var sb = new StringBuilder();

			if (!ship.IsAbyssalShip)
			{

				foreach (var record in RecordManager.Instance.ShipDrop.Record
					.Where(s => s.ShipID == shipID && s.EnemyFleetID != 0)
					.Select(s => new
					{
						s.MapAreaID,
						s.MapInfoID,
						s.CellID,
						s.Difficulty,
						EnemyFleetName = RecordManager.Instance.EnemyFleet.Record.ContainsKey(s.EnemyFleetID) ?
						   RecordManager.Instance.EnemyFleet.Record[s.EnemyFleetID].FleetName : "적함대명불명"
					})
					.Distinct()
					.OrderBy(r => r.MapAreaID)
					.ThenBy(r => r.MapInfoID)
					.ThenBy(r => r.CellID)
					.ThenBy(r => r.Difficulty)
					)
				{
					sb.AppendFormat("{0}-{1}-{2}{3} ({4})\r\n",
						record.MapAreaID, record.MapInfoID, record.CellID, record.Difficulty > 0 ? " [" + Constants.GetDifficulty(record.Difficulty) + "]" : "", record.EnemyFleetName);
				}

				foreach (var record in RecordManager.Instance.Construction.Record
					.Where(s => s.ShipID == shipID)
					.Select(s => new
					{
						s.Fuel,
						s.Ammo,
						s.Steel,
						s.Bauxite,
						s.DevelopmentMaterial
					})
					.Distinct()
					.OrderBy(r => r.Fuel)
					.ThenBy(r => r.Ammo)
					.ThenBy(r => r.Steel)
					.ThenBy(r => r.Bauxite)
					.ThenBy(r => r.DevelopmentMaterial)
					)
				{
					sb.AppendFormat("건조 {0} / {1} / {2} / {3} - {4}\r\n",
						record.Fuel, record.Ammo, record.Steel, record.Bauxite, record.DevelopmentMaterial);
				}

			}
			else
			{

				foreach (var record in RecordManager.Instance.EnemyFleet.Record.Values
					.Where(r => r.FleetMember.Contains(shipID))
					.Select(s => new
					{
						s.MapAreaID,
						s.MapInfoID,
						s.CellID,
						s.Difficulty,
						EnemyFleetName = !string.IsNullOrWhiteSpace(s.FleetName) ? s.FleetName : "적함대명불명"
					})
					.Distinct()
					.OrderBy(r => r.MapAreaID)
					.ThenBy(r => r.MapInfoID)
					.ThenBy(r => r.CellID)
					.ThenBy(r => r.Difficulty)
					)
				{
					sb.AppendFormat("{0}-{1}-{2}{3} ({4})\r\n",
						record.MapAreaID, record.MapInfoID, record.CellID, record.Difficulty > 0 ? " [" + Constants.GetDifficulty(record.Difficulty) + "]" : "", record.EnemyFleetName);
				}

			}

			return sb.ToString();
		}

		private void StripMenu_View_ShowAppearingArea_Click(object sender, EventArgs e)
		{
			var ship = KCDatabase.Instance.MasterShips[this._shipID];
			if (ship == null)
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			string result = this.GetAppearingArea(ship.ShipID);

			if (string.IsNullOrEmpty(result))
				result = ship.NameWithClass + " 의 드랍해역을 알수없습니다.";

			MessageBox.Show(result, "드랍해역 검색", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}



		private void ShipBanner_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{

                this.StripMenu_View_ShowShipGraphicViewer.PerformClick();
			}
		}

		private void StripMenu_View_ShowShipGraphicViewer_Click(object sender, EventArgs e)
		{
			var ship = KCDatabase.Instance.MasterShips[this._shipID];
			if (ship != null)
			{
                new DialogShipGraphicViewer(ship.ShipID).Show(this.Owner);
            }
			else
			{
				MessageBox.Show("대상 함선을 지정하세요.", "뷰어：대상 미지정", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}


		private void StripMenu_Edit_GoogleShipName_Click(object sender, EventArgs e)
		{
			var ship = KCDatabase.Instance.MasterShips[this._shipID];
			if (ship == null)
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			try
			{

                // google <艦船名> 艦これ
                System.Diagnostics.Process.Start(@"https://www.google.co.jp/search?q=%22" + Uri.EscapeDataString(ship.NameWithClass) + "%22+%E8%89%A6%E3%81%93%E3%82%8C");

            }
			catch (Exception ex)
			{
				Utility.ErrorReporter.SendErrorReport(ex, "함선명의 구글 검색에 실패했습니다.");
			}
		}

        private void ShipView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BasePanelShipGirl_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
