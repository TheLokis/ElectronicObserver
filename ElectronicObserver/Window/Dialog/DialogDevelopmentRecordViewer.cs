using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility.Mathematics;
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
	public partial class DialogDevelopmentRecordViewer : Form
	{

		private DevelopmentRecord _record;

		private const string NameAny = "(전부)";
		private const string NameNotExist = "(실패)";
		private const string NameExist = "(성공)";


		private class SearchArgument
		{
			public int EquipmentCategory;
			public string EquipmentName;
			public int SecretaryCategory;
			public string SecretaryName;
			public DateTime DateBegin;
			public DateTime DateEnd;
			public string Recipe;
			public bool MergeRows;
			public DataGridViewRow BaseRow;
		}


		public DialogDevelopmentRecordViewer()
		{
            this.InitializeComponent();

            this._record = RecordManager.Instance.Development;
		}

		private void DialogDevelopmentRecordViewer_Load(object sender, EventArgs e)
		{

			var includedEquipmentNames = this._record.Record
				.Select(r => r.EquipmentName)
				.Distinct()
				.Except(new[] { NameNotExist });

			var includedEquipmentObjects = includedEquipmentNames
				.Select(name => KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eq => eq.Name == name))
				.Where(s => s != null);

			var removedEquipments = includedEquipmentNames.Except(includedEquipmentObjects.Select(eq => eq.Name));

			var includedSecretaryNames = this._record.Record
				.Select(r => r.FlagshipName).Distinct();

			var includedSecretaryObjects = includedSecretaryNames
				.Select(name => KCDatabase.Instance.MasterShips.Values.FirstOrDefault(ship => ship.NameWithClass == name))
				.Where(s => s != null);

			var removedSecretaryNames = includedSecretaryNames.Except(includedSecretaryObjects.Select(s => s.NameWithClass));


			{
				DataTable dt = new DataTable();
				dt.Columns.AddRange(new DataColumn[] {
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ),
				});
				dt.Rows.Add(-1, NameAny);
				foreach (var eq in includedEquipmentObjects
					.GroupBy(eq => eq.CategoryType, (key, eq) => eq.First())
					.OrderBy(eq => eq.CategoryType))
				{
					dt.Rows.Add(eq.CategoryType, eq.CategoryTypeInstance.Name);
				}
				dt.AcceptChanges();
                this.EquipmentCategory.DisplayMember = "Display";
                this.EquipmentCategory.ValueMember = "Value";
                this.EquipmentCategory.DataSource = dt;
                this.EquipmentCategory.SelectedIndex = 0;
			}

			{
                this.EquipmentName.Items.Add(NameAny);
                this.EquipmentName.Items.Add(NameExist);
                this.EquipmentName.Items.Add(NameNotExist);
                this.EquipmentName.Items.AddRange(includedEquipmentObjects
					.OrderBy(eq => eq.EquipmentID)
					.OrderBy(eq => eq.CategoryType)
					.Select(eq => eq.Name)
					.Union(removedEquipments.OrderBy(s => s))
					.ToArray());
                this.EquipmentName.SelectedIndex = 0;
			}

			{
				DataTable dt = new DataTable();
				dt.Columns.AddRange(new DataColumn[] {
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ),
				});
				dt.Rows.Add(-1, NameAny);
				foreach (var ship in includedSecretaryObjects
					.GroupBy(s => s.ShipType, (key, s) => s.First())
					.OrderBy(s => s.ShipType))
				{
					dt.Rows.Add(ship.ShipType, ship.ShipTypeName);
				}
				dt.AcceptChanges();
                this.SecretaryCategory.DisplayMember = "Display";
                this.SecretaryCategory.ValueMember = "Value";
                this.SecretaryCategory.DataSource = dt;
                this.SecretaryCategory.SelectedIndex = 0;
			}

			{
                this.SecretaryName.Items.Add(NameAny);
                this.SecretaryName.Items.AddRange(includedSecretaryObjects
					.OrderBy(s => s.NameReading)
					.OrderBy(s => s.ShipType)
					.Select(s => s.NameWithClass)
					.Union(removedSecretaryNames.OrderBy(s => s))
					.ToArray()
					);
                this.SecretaryName.SelectedIndex = 0;
			}

            this.DateBegin.Value = this.DateBegin.MinDate = this.DateEnd.MinDate = this._record.Record.First().Date.Date;
            this.DateEnd.Value = this.DateBegin.MaxDate = this.DateEnd.MaxDate = DateTime.Now.AddDays(1).Date;

            //checkme
            this.Recipe.Items.Add(NameAny);
            this.Recipe.Items.AddRange(this._record.Record
				.Select(r => this.GetRecipeStringForSorting(r))
				.Distinct()
				.OrderBy(s => s)
				.Select(r => this.GetRecipeString(this.GetResources(r)))
				.ToArray());
            this.Recipe.SelectedIndex = 0;


			foreach (DataGridViewColumn column in this.RecordView.Columns)
				column.Width = 20;

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormDevelopmentRecord]);
		}


		private string GetRecipeString(int[] resources)
		{
			return string.Join("/", resources);
		}

		private string GetRecipeString(int fuel, int ammo, int steel, int bauxite)
		{
			return this.GetRecipeString(new int[] { fuel, ammo, steel, bauxite });
		}

		private string GetRecipeString(DevelopmentRecord.DevelopmentElement record)
		{
			return this.GetRecipeString(new int[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite });
		}

		private string GetRecipeStringForSorting(int[] resources)
		{
			return string.Join("/", resources.Select(r => r.ToString("D4")));
		}

		private string GetRecipeStringForSorting(int fuel, int ammo, int steel, int bauxite)
		{
			return this.GetRecipeStringForSorting(new int[] { fuel, ammo, steel, bauxite });
		}

		private string GetRecipeStringForSorting(DevelopmentRecord.DevelopmentElement record)
		{
			return this.GetRecipeStringForSorting(new int[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite });
		}

		private int[] GetResources(string recipe)
		{
			return recipe.Split("/".ToCharArray()).Select(s => int.Parse(s)).ToArray();
		}


		private void DialogDevelopmentRecordViewer_FormClosed(object sender, FormClosedEventArgs e)
		{
			ResourceManager.DestroyIcon(this.Icon);
		}


		private void ButtonRun_Click(object sender, EventArgs e)
		{

			if (this.Searcher.IsBusy)
			{
				if (MessageBox.Show("검색을 취소 하시겠습니까?", "검색중입니다.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
					== System.Windows.Forms.DialogResult.Yes)
				{
                    this.Searcher.CancelAsync();
				}
				return;
			}

            this.RecordView.Rows.Clear();

			var row = new DataGridViewRow();
			row.CreateCells(this.RecordView);


			var args = new SearchArgument
			{
				EquipmentCategory = (int)this.EquipmentCategory.SelectedValue,
				EquipmentName = (string)this.EquipmentName.SelectedItem,
				SecretaryCategory = (int)this.SecretaryCategory.SelectedValue,
				SecretaryName = (string)this.SecretaryName.SelectedItem,
				DateBegin = this.DateBegin.Value,
				DateEnd = this.DateEnd.Value,
				Recipe = (string)this.Recipe.SelectedItem,
				MergeRows = this.MergeRows.Checked,
				BaseRow = row
			};

            this.RecordView.Tag = args;


			if (!this.MergeRows.Checked)
			{
                this.RecordView_Header.Width = 50;
                this.RecordView_Header.HeaderText = "";
                this.RecordView_Name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.RecordView_Name.HeaderText = "장비";
                this.RecordView_Date.Width = 140;
                this.RecordView_Date.Visible = true;
                this.RecordView_Recipe.Width = 120;
                this.RecordView_Recipe.Visible = true;
                this.RecordView_FlagshipType.Width = 60;
                this.RecordView_FlagshipType.Visible = true;
                this.RecordView_Flagship.Width = 60;
                this.RecordView_Flagship.Visible = true;
                this.RecordView_Detail.Visible = false;
			}
			else
			{
                this.RecordView_Header.Width = 150;
                this.RecordView_Header.HeaderText = "회수";
                this.RecordView_Name.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                this.RecordView_Name.Width = 160;
                this.RecordView_Name.HeaderText = ((this.EquipmentName.Text != NameAny && this.EquipmentName.Text != NameExist) || (int)this.EquipmentCategory.SelectedValue != -1) ? "레시피" : "장비";
                this.RecordView_Date.Visible = false;
                this.RecordView_Recipe.Visible = false;
                this.RecordView_FlagshipType.Visible = false;
                this.RecordView_Flagship.Visible = false;
                this.RecordView_Detail.HeaderText = (this.SecretaryName.Text != NameAny || (int)this.SecretaryCategory.SelectedValue != -1) ? "레시피별횟수" : "함종별횟수";
                this.RecordView_Detail.Visible = true;
			}
            this.RecordView.ColumnHeadersVisible = true;

            this.StatusInfo.Text = "검색중입니다...";
            this.StatusInfo.Tag = DateTime.Now;

            this.Searcher.RunWorkerAsync(args);

		}

		private void EquipmentCategory_SelectedIndexChanged(object sender, EventArgs e)
		{

			string name = (string)this.EquipmentName.SelectedItem;
			var category = (EquipmentTypes)this.EquipmentCategory.SelectedValue;

			if ((int)category != -1 && name != NameAny && name != NameExist)
			{
				var eq = KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eqm => eqm.Name == name);

				if (eq == null || eq.CategoryType != category)
                    this.EquipmentName.SelectedIndex = 0;

			}
		}

		private void EquipmentName_SelectedIndexChanged(object sender, EventArgs e)
		{

			string name = (string)this.EquipmentName.SelectedItem;
			var category = (EquipmentTypes)this.EquipmentCategory.SelectedValue;

			if ((int)category != -1 && name != NameAny && name != NameExist)
			{
				var eq = KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eqm => eqm.Name == name);

				if (eq == null || eq.CategoryType != category)
                    this.EquipmentCategory.SelectedIndex = 0;

			}
		}

		private void SecretaryCategory_SelectedIndexChanged(object sender, EventArgs e)
		{

			string name = (string)this.SecretaryName.SelectedItem;
			var category = (ShipTypes)this.SecretaryCategory.SelectedValue;

			if (name != NameAny && (int)category != -1)
			{
				var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.NameWithClass == name);

				if (ship == null || ship.ShipType != category)
                    this.SecretaryName.SelectedIndex = 0;
			}
		}

		private void SecretaryName_SelectedIndexChanged(object sender, EventArgs e)
		{

			string name = (string)this.SecretaryName.SelectedItem;
			var category = (ShipTypes)this.SecretaryCategory.SelectedValue;

			if (name != NameAny && (int)category != -1)
			{
				var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.NameWithClass == name);

				if (ship == null || ship.ShipType != category)
                    this.SecretaryCategory.SelectedIndex = 0;
			}
		}

		private void Searcher_DoWork(object sender, DoWorkEventArgs e)
		{

			var args = (SearchArgument)e.Argument;

			var records = RecordManager.Instance.Development.Record;
			var rows = new LinkedList<DataGridViewRow>();

			int prioritySecretary =
				args.SecretaryName != NameAny ? 2 :
				args.SecretaryCategory != -1 ? 1 : 0;

			int priorityEquipment =
				args.EquipmentName != NameAny && args.EquipmentName != NameExist ? 2 :
				args.EquipmentCategory != -1 ? 1 : 0;


			int i = 0;
			var counts = new Dictionary<string, int>();
			var allcounts = new Dictionary<string, int>();
			var countsdetail = new Dictionary<string, Dictionary<string, int>>();

			foreach (var r in records)
			{

				#region Filtering

				var eq = KCDatabase.Instance.MasterEquipments[r.EquipmentID];
				var secretary = KCDatabase.Instance.MasterShips[r.FlagshipID];
				string currentRecipe = this.GetRecipeString(r.Fuel, r.Ammo, r.Steel, r.Bauxite);
				var shiptype = KCDatabase.Instance.ShipTypes[r.FlagshipType];


				if (eq != null && eq.Name != r.EquipmentName) eq = null;
				if (secretary != null && secretary.Name != r.FlagshipName) secretary = null;


				if (r.Date < args.DateBegin || args.DateEnd < r.Date)
					continue;

				if (args.SecretaryCategory != -1 && args.SecretaryCategory != r.FlagshipType)
					continue;

				if (args.SecretaryName != NameAny && args.SecretaryName != r.FlagshipName)
					continue;



				if (args.MergeRows)
				{

					string key;

					if (priorityEquipment > 0)
						key = currentRecipe;
					else
						key = r.EquipmentName;

					if (!allcounts.ContainsKey(key))
					{
						allcounts.Add(key, 1);

					}
					else
					{
						allcounts[key]++;
					}

				}



				if (args.EquipmentCategory != -1 && (eq == null || args.EquipmentCategory != (int)eq.CategoryType))
					continue;

				switch (args.EquipmentName)
				{
					case NameAny:
						break;
					case NameExist:
						if (r.EquipmentID == -1)
							continue;
						break;
					case NameNotExist:
						if (r.EquipmentID != -1)
							continue;
						break;
					default:
						if (args.EquipmentName != r.EquipmentName)
							continue;
						break;
				}

				if (args.Recipe != NameAny && args.Recipe != currentRecipe)
					continue;

				#endregion


				if (!args.MergeRows)
				{
					var row = (DataGridViewRow)args.BaseRow.Clone();

					row.SetValues(
						i + 1,
						r.EquipmentName,
						r.Date,
                        this.GetRecipeString(r),
						shiptype?.Name ?? "(불명)",
						r.FlagshipName,
						null
						);

					row.Cells[1].Tag = (eq?.EquipmentID ?? 0) + 1000 * ((int?)eq?.CategoryType ?? 0);
					row.Cells[3].Tag = this.GetRecipeStringForSorting(r);
					row.Cells[4].Tag = shiptype?.TypeID ?? 0;
					row.Cells[5].Tag = ((int?)secretary?.ShipType ?? 0).ToString("D4") + (secretary?.NameReading ?? r.FlagshipName);

					rows.AddLast(row);

				}
				else
				{

					string key;
					if (priorityEquipment > 0)
						key = currentRecipe;
					else
						key = r.EquipmentName;

					if (!counts.ContainsKey(key))
					{
						counts.Add(key, 1);

					}
					else
					{
						counts[key]++;
					}



					if (priorityEquipment > 0)
						key = currentRecipe;
					else
						key = r.EquipmentName;

					string key2;
					if (prioritySecretary > 0)
						key2 = currentRecipe;
					else
						key2 = shiptype?.Name ?? "(불명)";

					if (!countsdetail.ContainsKey(key))
					{
						countsdetail.Add(key, new Dictionary<string, int>());
					}
					if (!countsdetail[key].ContainsKey(key2))
					{
						countsdetail[key].Add(key2, 1);
					}
					else
					{
						countsdetail[key][key2]++;
					}

				}

				if (this.Searcher.CancellationPending)
					break;

				i++;
			}


			if (args.MergeRows)
			{

				int sum = counts.Values.Sum();

				foreach (var c in counts)
				{
					var row = (DataGridViewRow)args.BaseRow.Clone();

					if (priorityEquipment > 0)
					{

						row.SetValues(
							c.Value,
							c.Key,
							null,
							null,
							null,
							null,
							string.Join(", ", countsdetail[c.Key].OrderByDescending(p => p.Value).Select(d => string.Format("{0}({1})", d.Key, d.Value)))
							);

						row.Cells[0].Tag = allcounts[c.Key];
						row.Cells[1].Tag = this.GetRecipeStringForSorting(this.GetResources(c.Key));

					}
					else
					{

						row.SetValues(
							c.Value,
							c.Key,
							null,
							null,
							null,
							null,
							string.Join(", ", countsdetail[c.Key].OrderByDescending(p => p.Value).Select(d => string.Format("{0}({1})", d.Key, d.Value)))
							);

						var eq = KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eqm => eqm.Name == c.Key);
						row.Cells[0].Tag = (double)c.Value / sum;
						row.Cells[1].Tag = (eq?.EquipmentID ?? 0) + 1000 * ((int?)eq?.CategoryType ?? 0);
					}

					rows.AddLast(row);

					if (this.Searcher.CancellationPending)
						break;
				}

			}



			e.Result = rows.ToArray();

		}

		private void Searcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (!e.Cancelled)
			{
				var rows = (DataGridViewRow[])e.Result;

                this.RecordView.Rows.AddRange(rows);
                this.RecordView.Sort(this.RecordView.SortedColumn ?? this.RecordView_Header,
                    this.RecordView.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);

                this.StatusInfo.Text = "검색이 완료되었습니다.(" + (int)(DateTime.Now - (DateTime)this.StatusInfo.Tag).TotalMilliseconds + " ms)";

			}
			else
			{

                this.StatusInfo.Text = "검색이 취소되었습니다.";
			}

		}

		private void RecordView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			object tag1 = this.RecordView[e.Column.Index, e.RowIndex1].Tag;
			object tag2 = this.RecordView[e.Column.Index, e.RowIndex2].Tag;

			if (tag1 != null && (tag1 is double || tag1 is int) && e.CellValue1 is int)
			{
				double c1 = 0, c2 = 0;

				if (tag1 is double)
				{
					c1 = (double)tag1;
					c2 = (double)tag2;
				}
				else if (tag1 is int)
				{
					c1 = (double)(int)e.CellValue1 / Math.Max((int)tag1, 1);
					c2 = (double)(int)e.CellValue2 / Math.Max((int)tag2, 1);
				}


				if (Math.Abs(c1 - c2) < 0.000001)
					e.SortResult = (int)e.CellValue1 - (int)e.CellValue2;
				else if (c1 < c2)
					e.SortResult = -1;
				else
					e.SortResult = 1;
				e.Handled = true;

			}
			else if (tag1 is string)
			{
				e.SortResult = ((IComparable)tag1).CompareTo(tag2);
				e.Handled = true;
			}
			else if (tag1 is int)
			{
				e.SortResult = (int)tag1 - (int)tag2;
				e.Handled = true;
			}


			if (!e.Handled)
			{
				e.SortResult = ((IComparable)e.CellValue1 ?? 0).CompareTo(e.CellValue2 ?? 0);
				e.Handled = true;
			}

			if (e.SortResult == 0)
			{
				e.SortResult = (int)(this.RecordView.Rows[e.RowIndex1].Tag ?? 0) - (int)(this.RecordView.Rows[e.RowIndex2].Tag ?? 0);
			}
		}

		private void RecordView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < this.RecordView.Rows.Count; i++)
			{
                this.RecordView.Rows[i].Tag = i;
			}
		}

		private void RecordView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.ColumnIndex == this.RecordView_Header.Index)
			{
				object tag = this.RecordView[e.ColumnIndex, e.RowIndex].Tag;

				if (tag != null)
				{
					if (tag is double)
					{
						e.Value = string.Format("{0} ({1:p1})", e.Value, (double)tag);
						e.FormattingApplied = true;
					}
					else if (tag is int)
					{
						e.Value = string.Format("{0}/{1} ({2:p1})", e.Value, (int)tag, (double)(int)e.Value / (int)tag);
						e.FormattingApplied = true;
					}
				}

			}
			else if (e.ColumnIndex == this.RecordView_Date.Index)
			{

				if (e.Value is DateTime)
				{
					e.Value = DateTimeHelper.TimeToCSVString((DateTime)e.Value);
					e.FormattingApplied = true;
				}
			}

		}


		private void RecordView_SelectionChanged(object sender, EventArgs e)
		{
			var args = this.RecordView.Tag as SearchArgument;
			if (args == null)
				return;

			int selectedCount = this.RecordView.Rows.GetRowCount(DataGridViewElementStates.Selected);

			if (selectedCount == 0)
				return;

			if (args.MergeRows)
			{
				int count = this.RecordView.SelectedRows.OfType<DataGridViewRow>().Select(r => (int)r.Cells[this.RecordView_Header.Index].Value).Sum();
				int allcount = this.RecordView.Rows.OfType<DataGridViewRow>().Select(r => (int)r.Cells[this.RecordView_Header.Index].Value).Sum();

                this.StatusInfo.Text = string.Format("선택 항목 총합: {0} / {1} ({2:p1})",
					count, allcount, (double)count / allcount);
			}
			else
			{
				int allcount = this.RecordView.RowCount;
                this.StatusInfo.Text = string.Format("선택 항목 총합: {0} / {1} ({2:p1})",
					selectedCount, allcount, (double)selectedCount / allcount);
			}
		}
	}
}
