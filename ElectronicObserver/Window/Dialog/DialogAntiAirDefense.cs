using ElectronicObserver.Data;
using ElectronicObserver.Resource;
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
	public partial class DialogAntiAirDefense : Form
	{

		private class AACutinComboBoxData
		{
			public readonly int Kind;
			public AACutinComboBoxData(int kind)
			{
                this.Kind = kind;
			}

			public override string ToString() => $"{this.Kind}: {Constants.GetAACutinKind(this.Kind)}";


			public static implicit operator int(AACutinComboBoxData data)
			{
				if (data == null)
					return -1;
				return data.Kind;
			}
		}

		private class FormationComboBoxData
		{
			public readonly int Formation;
			public FormationComboBoxData(int formation)
			{
                this.Formation = formation;
			}

			public override string ToString() => Constants.GetFormation(this.Formation);


			public static implicit operator int(FormationComboBoxData data)
			{
				if (data == null)
					return -1;
				return data.Formation;
			}
		}


		/// <summary>
		/// NumericUpDown から Value を正しく取得できないことがあるため、一旦これにキャッシュする
		/// </summary>
		/// <remarks>https://github.com/andanteyk/ElectronicObserver/pull/197</remarks>
		private int enemySlotCountValue;


		public DialogAntiAirDefense()
		{
            this.InitializeComponent();
            this.enemySlotCountValue = (int)this.EnemySlotCount.Value;
		}

		private void DialogAntiAirDefense_Load(object sender, EventArgs e)
		{

			if (!KCDatabase.Instance.Fleet.IsAvailable)
			{
				MessageBox.Show("함대 데이터가 로드되지 않았습니다. \r\n칸코레 시작후 열어주세요.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
				return;
			}

            if (this.FleetID.SelectedIndex == -1)
                this.FleetID.SelectedIndex = 0;
            this.Formation.SelectedIndex = 0;

            this.UpdateAACutinKind(this.ShowAll.Checked);
            this.UpdateFormation();

			this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormAntiAirDefense]);
		}

		private void DialogAntiAirDefense_FormClosed(object sender, FormClosedEventArgs e)
		{
			ResourceManager.DestroyIcon(this.Icon);
		}


		public void SetFleetID(int id)
		{
            this.FleetID.SelectedIndex = id - 1;
		}

		private void Updated()
		{

			ShipData[] ships = this.GetShips().ToArray();
			int formation = this.Formation.SelectedItem as FormationComboBoxData;
			int aaCutinKind = this.AACutinKind.SelectedItem as AACutinComboBoxData;
			int enemyAircraftCount = this.enemySlotCountValue;


			// 加重対空値
			double[] adjustedAAs = ships.Select(s => s == null ? 0.0 : Calculator.GetAdjustedAAValue(s)).ToArray();

			// 艦隊防空値
			double adjustedFleetAA = Calculator.GetAdjustedFleetAAValue(ships, formation);

			// 割合撃墜
			double[] proportionalAAs = adjustedAAs.Select((val, i) => Calculator.GetProportionalAirDefense(val, this.IsCombined ? (i < 6 ? 1 : 2) : -1)).ToArray();

			// 固定撃墜
			int[] fixedAAs = adjustedAAs.Select((val, i) => Calculator.GetFixedAirDefense(val, adjustedFleetAA, aaCutinKind, this.IsCombined ? (i < 6 ? 1 : 2) : -1)).ToArray();



			int[] shootDownBoth = adjustedAAs.Select((val, i) => ships[i] == null ? 0 :
			   Calculator.GetShootDownCount(enemyAircraftCount, proportionalAAs[i], fixedAAs[i], aaCutinKind)).ToArray();

			int[] shootDownProportional = adjustedAAs.Select((val, i) => ships[i] == null ? 0 :
			   Calculator.GetShootDownCount(enemyAircraftCount, proportionalAAs[i], 0, aaCutinKind)).ToArray();

			int[] shootDownFixed = adjustedAAs.Select((val, i) => ships[i] == null ? 0 :
			   Calculator.GetShootDownCount(enemyAircraftCount, 0, fixedAAs[i], aaCutinKind)).ToArray();

			int[] shootDownFailed = adjustedAAs.Select((val, i) => ships[i] == null ? 0 :
			   Calculator.GetShootDownCount(enemyAircraftCount, 0, 0, aaCutinKind)).ToArray();

            double[] aaRocketBarrageProbability = ships.Select(ship => Calculator.GetAARocketBarrageProbability(ship)).ToArray();

            this.ResultView.Rows.Clear();
			var rows = new DataGridViewRow[ships.Length];
			for (int i = 0; i < ships.Length; i++)
			{
				if (ships[i] == null)
					continue;

				rows[i] = new DataGridViewRow();
				rows[i].CreateCells(this.ResultView);

                rows[i].SetValues(
                    ships[i].Name,
                    ships[i].AABase,
                    adjustedAAs[i],
                    proportionalAAs[i],
                    fixedAAs[i],
                    shootDownBoth[i],
                    shootDownProportional[i],
                    shootDownFixed[i],
                    shootDownFailed[i],
                    aaRocketBarrageProbability[i]);
            }
            this.ResultView.Rows.AddRange(rows.Where(r => r != null).ToArray());

            this.AdjustedFleetAA.Text = adjustedFleetAA.ToString("0.0");
			{
				var allShootDown = shootDownBoth.Concat(shootDownProportional).Concat(shootDownFixed).Concat(shootDownFailed);
                this.AnnihilationProbability.Text = (allShootDown.Count(i => i >= enemyAircraftCount) / Math.Max(ships.Count(s => s != null) * 4, 1.0)).ToString("p1");
			}
		}


		private IEnumerable<ShipData> GetShips()
		{
			if (this.FleetID.SelectedIndex < 4)
				return KCDatabase.Instance.Fleet[this.FleetID.SelectedIndex + 1].MembersWithoutEscaped;
			else
				return KCDatabase.Instance.Fleet[1].MembersWithoutEscaped.Concat(KCDatabase.Instance.Fleet[2].MembersWithoutEscaped);
		}

		private bool IsCombined => this.FleetID.SelectedIndex == 4;


		private void UpdateAACutinKind(bool showAll)
		{

			AACutinComboBoxData[] list;

			if (showAll)
			{

				int max = Calculator.AACutinFixedBonus.Keys.Max();
				list = Enumerable.Range(0, max + 1).Select(kind => new AACutinComboBoxData(kind)).ToArray();

			}
			else
			{

				list = this.GetShips()
					.Where(s => s != null)
					.Select(s => Calculator.GetAACutinKind(s.ShipID, s.AllSlotMaster.ToArray()))
					.Concat(Enumerable.Repeat(0, 1))
					.Distinct()
					.OrderBy(i => i)
					.Select(kind => new AACutinComboBoxData(kind)).ToArray();

			}

            this.AACutinKind.Items.Clear();
            this.AACutinKind.Items.AddRange(list);
            this.AACutinKind.SelectedIndex = 0;
		}

		private void UpdateFormation()
		{
			var items = (this.IsCombined ? Enumerable.Range(11, 4) : Enumerable.Range(1, 6))
				.Select(i => new FormationComboBoxData(i)).ToArray();

			int selected = this.Formation.SelectedItem as FormationComboBoxData;
			int index = Array.FindIndex(items, item => item == selected);

            this.Formation.Items.Clear();
            this.Formation.Items.AddRange(items);
            this.Formation.SelectedIndex = Math.Max(index, 0);
		}


		private void ResultView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == this.ResultView_ShootDownBoth.Index ||
				e.ColumnIndex == this.ResultView_ShootDownProportional.Index ||
				e.ColumnIndex == this.ResultView_ShootDownFixed.Index ||
				e.ColumnIndex == this.ResultView_ShootDownFailed.Index)
			{

				int value = e.Value as int? ?? 0;
				int enemySlot = this.enemySlotCountValue;

				e.Value = string.Format("{0} ({1:p0})", value, (double)value / enemySlot);
				e.FormattingApplied = true;
				e.CellStyle.BackColor = e.CellStyle.SelectionBackColor =
					value >= enemySlot ? Color.MistyRose : SystemColors.Window;
			}

		}



		private void FleetID_SelectedIndexChanged(object sender, EventArgs e)
		{
            this.Updated();
            this.UpdateAACutinKind(this.ShowAll.Checked);
            this.UpdateFormation();
		}

		private void Formation_SelectedIndexChanged(object sender, EventArgs e)
		{
            this.Updated();
		}

		private void AACutinKind_SelectedIndexChanged(object sender, EventArgs e)
		{
            this.Updated();
		}

		private void EnemySlotCount_ValueChanged(object sender, EventArgs e)
		{
            this.enemySlotCountValue = (int)this.EnemySlotCount.Value;
            this.Updated();
		}

		private void ShowAll_CheckedChanged(object sender, EventArgs e)
		{
            this.UpdateAACutinKind(this.ShowAll.Checked);
		}


	}
}
