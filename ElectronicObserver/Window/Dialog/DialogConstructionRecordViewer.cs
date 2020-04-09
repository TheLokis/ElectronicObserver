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
	public partial class DialogConstructionRecordViewer : Form
	{


		private ConstructionRecord _record;

		private const string NameAny = "(전부)";


		private class SearchArgument
		{
			public int ShipCategory;
			public string ShipName;
			public int SecretaryCategory;
			public string SecretaryName;
			public DateTime DateBegin;
			public DateTime DateEnd;
			public string Recipe;
			public int DevelopmentMaterial;
			public int EmptyDock;
			public CheckState IsLargeConstruction;
			public bool MergeRows;
			public DataGridViewRow BaseRow;
		}


		public DialogConstructionRecordViewer()
		{
            this.InitializeComponent();

            this._record = RecordManager.Instance.Construction;
		}

		private void DialogConstructionRecordViewer_Load(object sender, EventArgs e)
		{

			var includedShipNames = this._record.Record
				.Select(r => r.ShipName)
				.Distinct();

			var includedShipObjects = includedShipNames
				.Select(name => KCDatabase.Instance.MasterShips.Values.FirstOrDefault(ship => ship.NameWithClass == name))
				.Where(s => s != null);

			var removedShipNames = includedShipNames.Except(includedShipObjects.Select(s => s.NameWithClass));


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
				foreach (var ship in includedShipObjects
					.GroupBy(s => s.ShipType, (key, s) => s.First())
					.OrderBy(s => s.ShipType))
				{
					dt.Rows.Add(ship.ShipType, ship.ShipTypeName);
				}
				dt.AcceptChanges();
                this.ShipCategory.DisplayMember = "Display";
                this.ShipCategory.ValueMember = "Value";
                this.ShipCategory.DataSource = dt;
                this.ShipCategory.SelectedIndex = 0;
			}

			{
                this.ShipName.Items.Add(NameAny);
                this.ShipName.Items.AddRange(includedShipObjects
					.OrderBy(s => s.NameReading)
					.OrderBy(s => s.ShipType)
					.Select(s => s.NameWithClass)
					.Union(removedShipNames.OrderBy(s => s))
					.ToArray()
					);
                this.ShipName.SelectedIndex = 0;
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

			{
				DataTable dt = new DataTable();
				dt.Columns.AddRange(new DataColumn[] {
					new DataColumn( "Value", typeof( string ) ),
					new DataColumn( "Display", typeof( string ) ),
				});
				dt.Rows.Add(NameAny, NameAny);

				var dict = new Dictionary<string, string>();
				foreach (var r in this._record.Record)
				{
					string key = this.GetRecipeStringForSorting(r);
					if (!dict.ContainsKey(key))
						dict.Add(key, this.GetRecipeString(r));
				}
				foreach (var recipe in dict.OrderBy(p => p.Key))
				{
					dt.Rows.Add(recipe.Key, recipe.Value);
				}

				dt.AcceptChanges();
                this.Recipe.DisplayMember = "Display";
                this.Recipe.ValueMember = "Value";
                this.Recipe.DataSource = dt;
                this.Recipe.SelectedIndex = 0;
			}

			{
				DataTable dt = new DataTable();
				dt.Columns.AddRange(new DataColumn[] {
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ),
				});
				dt.Rows.Add(-1, NameAny);
				foreach (var devmat in this._record.Record
					.GroupBy(r => r.DevelopmentMaterial, (key, r) => r.First())
					.Select(r => r.DevelopmentMaterial)
					.OrderBy(i => i))
				{
					dt.Rows.Add(devmat, devmat.ToString());
				}
				dt.AcceptChanges();
                this.DevelopmentMaterial.DisplayMember = "Display";
                this.DevelopmentMaterial.ValueMember = "Value";
                this.DevelopmentMaterial.DataSource = dt;
                this.DevelopmentMaterial.SelectedIndex = 0;
			}

			{
				DataTable dt = new DataTable();
				dt.Columns.AddRange(new DataColumn[] {
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ),
				});
				dt.Rows.Add(-1, NameAny);
				foreach (var dock in this._record.Record
					.GroupBy(r => r.EmptyDockAmount, (key, r) => r.First())
					.Select(r => r.EmptyDockAmount)
					.OrderBy(i => i))
				{
					dt.Rows.Add(dock, dock.ToString());
				}
				dt.AcceptChanges();
                this.EmptyDock.DisplayMember = "Display";
                this.EmptyDock.ValueMember = "Value";
                this.EmptyDock.DataSource = dt;
                this.EmptyDock.SelectedIndex = 0;
			}

            this.DateBegin.Value = this.DateBegin.MinDate = this.DateEnd.MinDate = this._record.Record.First().Date.Date;
            this.DateEnd.Value = this.DateBegin.MaxDate = this.DateEnd.MaxDate = DateTime.Now.AddDays(1).Date;

			// スクロールバーを非表示にするため(実際の幅は検索開始時に設定される)
			foreach (DataGridViewColumn column in this.RecordView.Columns)
				column.Width = 20;

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormConstructionRecord]);

		}


		private string GetRecipeString(int[] resources)
		{
			return string.Join("/", resources);
		}

		private string GetRecipeString(int fuel, int ammo, int steel, int bauxite)
		{
			return this.GetRecipeString(new[] { fuel, ammo, steel, bauxite });
		}

		private string GetRecipeString(int fuel, int ammo, int steel, int bauxite, int devmat)
		{
			return this.GetRecipeString(new[] { fuel, ammo, steel, bauxite, devmat });
		}

		private string GetRecipeString(ConstructionRecord.ConstructionElement record, bool containsDevmat = false)
		{
			if (containsDevmat)
				return this.GetRecipeString(new[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite, record.DevelopmentMaterial });
			else
				return this.GetRecipeString(new[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite });
		}

		private string GetRecipeStringForSorting(int[] resources)
		{
			return string.Join("/", resources.Select(r => r.ToString("D4")));
		}

		private string GetRecipeStringForSorting(int fuel, int ammo, int steel, int bauxite)
		{
			return this.GetRecipeStringForSorting(new[] { fuel, ammo, steel, bauxite });
		}

		private string GetRecipeStringForSorting(int fuel, int ammo, int steel, int bauxite, int devmat)
		{
			return this.GetRecipeStringForSorting(new[] { fuel, ammo, steel, bauxite, devmat });
		}

		private string GetRecipeStringForSorting(ConstructionRecord.ConstructionElement record, bool containsDevmat = false)
		{
			if (containsDevmat)
				return this.GetRecipeStringForSorting(new[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite, record.DevelopmentMaterial });
			else
				return this.GetRecipeStringForSorting(new[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite });
		}


		private void ButtonRun_Click(object sender, EventArgs e)
		{

			if (this.Searcher.IsBusy)
			{
				if (MessageBox.Show("검색을 취소하시겠습니까?", "검색중입니다.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
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
				ShipCategory = (int)this.ShipCategory.SelectedValue,
				ShipName = (string)this.ShipName.SelectedItem,
				SecretaryCategory = (int)this.SecretaryCategory.SelectedValue,
				SecretaryName = (string)this.SecretaryName.SelectedItem,
				DateBegin = this.DateBegin.Value,
				DateEnd = this.DateEnd.Value,
				Recipe = this.Recipe.Text,
				DevelopmentMaterial = (int)this.DevelopmentMaterial.SelectedValue,
				EmptyDock = (int)this.EmptyDock.SelectedValue,
				IsLargeConstruction = this.IsLargeConstruction.CheckState,
				MergeRows = this.MergeRows.Checked,
				BaseRow = row
			};

            this.RecordView.Tag = args;


			// column initialize
			if (!args.MergeRows)
			{
                this.RecordView_Header.DisplayIndex = 0;
                this.RecordView_Header.Width = 50;
                this.RecordView_Header.HeaderText = "";
                this.RecordView_Name.DisplayIndex = 1;
                this.RecordView_Name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.RecordView_Name.HeaderText = "함명";
                this.RecordView_Name.Visible = true;
                this.RecordView_Date.DisplayIndex = 2;
                this.RecordView_Date.Width = 140;
                this.RecordView_Date.HeaderText = "날짜";
                this.RecordView_Date.Visible = true;
                this.RecordView_Recipe.DisplayIndex = 3;
                this.RecordView_Recipe.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                this.RecordView_Recipe.Width = 200;
                this.RecordView_Recipe.HeaderText = "레시피";
                this.RecordView_Recipe.Visible = true;
                this.RecordView_SecretaryShip.DisplayIndex = 4;
                this.RecordView_SecretaryShip.Width = 60;
                this.RecordView_SecretaryShip.HeaderText = "비서함";
                this.RecordView_SecretaryShip.Visible = true;
                this.RecordView_Material100.Visible = false;
                this.RecordView_Material20.Visible = false;
                this.RecordView_Material1.Visible = false;

			}
			else
			{
				if (args.ShipName != NameAny)
				{
                    this.RecordView_Recipe.DisplayIndex = 0;
                    this.RecordView_Recipe.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    this.RecordView_Recipe.HeaderText = "레시피";
                    this.RecordView_Recipe.Visible = true;
                    this.RecordView_Name.Visible = false;
				}
				else
				{
                    this.RecordView_Name.DisplayIndex = 0;
                    this.RecordView_Name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    this.RecordView_Name.HeaderText = "함명";
                    this.RecordView_Name.Visible = true;
                    this.RecordView_Recipe.Visible = false;
				}
                this.RecordView_Header.DisplayIndex = 1;
                this.RecordView_Header.Width = 120;
                this.RecordView_Header.HeaderText = "회수";
                this.RecordView_Material100.DisplayIndex = 2;
                this.RecordView_Material100.Width = 120;
                this.RecordView_Material100.HeaderText = "개발자재x100";
                this.RecordView_Material20.DisplayIndex = 3;
                this.RecordView_Material20.Width = 120;
                this.RecordView_Material20.HeaderText = "개발자재x20";
                this.RecordView_Material1.DisplayIndex = 4;
                this.RecordView_Material1.Width = 120;
                this.RecordView_Material1.HeaderText = "개발자재x1";
				if (args.IsLargeConstruction == CheckState.Unchecked ||
					(args.Recipe != NameAny && args.Recipe.IndexOf("/") < 4) ||
					args.DevelopmentMaterial != -1)
				{
                    this.RecordView_Material100.Visible = false;
                    this.RecordView_Material20.Visible = false;
                    this.RecordView_Material1.Visible = false;
				}
				else
				{
                    this.RecordView_Material100.Visible = true;
                    this.RecordView_Material20.Visible = true;
                    this.RecordView_Material1.Visible = true;
				}
                this.RecordView_Date.Visible = false;
                this.RecordView_SecretaryShip.Visible = false;
			}
            this.RecordView.ColumnHeadersVisible = true;


            this.StatusInfo.Text = "검색중입니다...";
            this.StatusInfo.Tag = DateTime.Now;

            this.Searcher.RunWorkerAsync(args);

		}

		private void ShipCategory_SelectedIndexChanged(object sender, EventArgs e)
		{

			string name = (string)this.ShipName.SelectedItem;
			var category = (ShipTypes)this.ShipCategory.SelectedValue;

			if (name != NameAny && (int)category != -1)
			{
				var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.NameWithClass == name);

				if (ship == null || ship.ShipType != category)
                    this.ShipName.SelectedIndex = 0;
			}
		}

		private void ShipName_SelectedIndexChanged(object sender, EventArgs e)
		{

			string name = (string)this.ShipName.SelectedItem;
			var category = (ShipTypes)this.ShipCategory.SelectedValue;

			if (name != NameAny && (int)category != -1)
			{
				var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.NameWithClass == name);

				if (ship == null || ship.ShipType != category)
                    this.ShipCategory.SelectedIndex = 0;
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

			var records = RecordManager.Instance.Construction.Record;
			var rows = new LinkedList<DataGridViewRow>();

			int priorityShip = args.ShipName != NameAny ? 1 : 0;


			int i = 0;
			var counts = new Dictionary<string, int[]>();
			var allcounts = new Dictionary<string, int[]>();


			foreach (var r in records)
			{

				#region filtering

				var ship = KCDatabase.Instance.MasterShips[r.ShipID];
				var secretary = KCDatabase.Instance.MasterShips[r.FlagshipID];

				if (ship != null && ship.Name != r.ShipName) ship = null;
				if (secretary != null && secretary.Name != r.FlagshipName) secretary = null;


				if (args.SecretaryCategory != -1 && (secretary == null || args.SecretaryCategory != (int)secretary.ShipType))
					continue;

				if (args.SecretaryName != NameAny && (secretary == null || args.SecretaryName != secretary.NameWithClass))
					continue;


				if (r.Date < args.DateBegin || args.DateEnd < r.Date)
					continue;

				if (args.DevelopmentMaterial != -1 && args.DevelopmentMaterial != r.DevelopmentMaterial)
					continue;

				if (args.EmptyDock != -1 && args.EmptyDock != r.EmptyDockAmount)
					continue;

				if (args.IsLargeConstruction != CheckState.Indeterminate &&
					(args.IsLargeConstruction == CheckState.Checked) != r.IsLargeDock)
					continue;


				if (args.MergeRows)
				{
					string key;

					key = this.GetRecipeString(r);

					if (!allcounts.ContainsKey(key))
						allcounts.Add(key, new int[4]);

					allcounts[key][0]++;

					switch (r.DevelopmentMaterial)
					{
						case 100:
							allcounts[key][1]++;
							break;
						case 20:
							allcounts[key][2]++;
							break;
						case 1:
							allcounts[key][3]++;
							break;
					}
				}



				if (args.ShipCategory != -1 && (ship == null || args.ShipCategory != (int)ship.ShipType))
					continue;

				if (args.ShipName != NameAny && args.ShipName != r.ShipName)
					continue;


				if (args.Recipe != NameAny && args.Recipe != this.GetRecipeString(r))
					continue;

				#endregion


				if (!args.MergeRows)
				{

					var row = (DataGridViewRow)args.BaseRow.Clone();

					row.SetValues(
						i + 1,
						r.ShipName,
						r.Date,
                        this.GetRecipeString(r, true),
						r.FlagshipName,
						null,
						null,
						null
						);

					row.Cells[1].Tag = ((int?)ship?.ShipType ?? 0).ToString("D4") + (ship?.NameReading ?? r.ShipName);
					row.Cells[3].Tag = this.GetRecipeStringForSorting(r, true);
					row.Cells[4].Tag = ((int?)secretary?.ShipType ?? 0).ToString("D4") + (secretary?.NameReading ?? r.FlagshipName);

					rows.AddLast(row);

				}
				else
				{

					string key;

					if (priorityShip > 0)
						key = this.GetRecipeString(r);
					else
						key = r.ShipName;

					if (!counts.ContainsKey(key))
						counts.Add(key, new int[4]);

					counts[key][0]++;

					switch (r.DevelopmentMaterial)
					{
						case 100:
							counts[key][1]++;
							break;
						case 20:
							counts[key][2]++;
							break;
						case 1:
							counts[key][3]++;
							break;
					}

				}

				if (this.Searcher.CancellationPending)
					return;

				i++;
			}


			if (args.MergeRows)
			{

				foreach (var c in counts)
				{
					var row = (DataGridViewRow)args.BaseRow.Clone();

					if (priorityShip > 0)
					{

						row.SetValues(
							c.Value[0],
							null,
							null,
							c.Key,
							null,
							c.Value[1],
							c.Value[2],
							c.Value[3]
							);

						row.Cells[3].Tag = this.GetRecipeStringForSorting(c.Key.Split("/".ToCharArray()).Select(s => int.Parse(s)).ToArray());

						row.Cells[0].Tag = allcounts[c.Key][0];
						row.Cells[5].Tag = allcounts[c.Key][1];
						row.Cells[6].Tag = allcounts[c.Key][2];
						row.Cells[7].Tag = allcounts[c.Key][3];

					}
					else
					{

						row.SetValues(
							c.Value[0],
							c.Key,
							null,
							null,
							null,
							c.Value[1],
							c.Value[2],
							c.Value[3]
							);

						var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.Name == c.Key);
						row.Cells[1].Tag = (ship != null ? (int)ship.ShipType : 0).ToString("D4") + (ship?.NameReading ?? c.Key);

						if (args.Recipe != NameAny)
						{
							row.Cells[0].Tag = (double)c.Value[0] / Math.Max(allcounts[args.Recipe][0], 1);
							row.Cells[5].Tag = (double)c.Value[1] / Math.Max(allcounts[args.Recipe][1], 1);
							row.Cells[6].Tag = (double)c.Value[2] / Math.Max(allcounts[args.Recipe][2], 1);
							row.Cells[7].Tag = (double)c.Value[3] / Math.Max(allcounts[args.Recipe][3], 1);

						}
						else
						{
							row.Cells[0].Tag = (double)c.Value[0] / Math.Max(allcounts.Values.Sum(a => a[0]), 1);
							row.Cells[5].Tag = (double)c.Value[1] / Math.Max(allcounts.Values.Sum(a => a[1]), 1);
							row.Cells[6].Tag = (double)c.Value[2] / Math.Max(allcounts.Values.Sum(a => a[2]), 1);
							row.Cells[7].Tag = (double)c.Value[3] / Math.Max(allcounts.Values.Sum(a => a[3]), 1);
						}

					}

					rows.AddLast(row);

					if (this.Searcher.CancellationPending)
						return;
				}

			}



			e.Result = rows.ToArray();

		}

		private void Searcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (!e.Cancelled)
			{

                this.RecordView.Rows.AddRange((DataGridViewRow[])e.Result);

                this.RecordView.Sort(this.RecordView.SortedColumn ?? this.RecordView_Header,
                    this.RecordView.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);

                this.StatusInfo.Text = "검색이 완료되었습니다. (" + (int)(DateTime.Now - (DateTime)this.StatusInfo.Tag).TotalMilliseconds + " ms)";

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
					e.Value = string.Format("{0}/{1} ({2:p1})", e.Value, (int)tag, (double)(int)e.Value / Math.Max((int)tag, 1));
					e.FormattingApplied = true;
				}
			}

			if (e.Value is DateTime)
			{
				e.Value = DateTimeHelper.TimeToCSVString((DateTime)e.Value);
				e.FormattingApplied = true;
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
