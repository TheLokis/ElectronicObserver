using ElectronicObserver.Data;
using ElectronicObserver.Window.Support;
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
	public partial class DialogShipGroupSortOrder : Form
	{

		public List<KeyValuePair<string, ListSortDirection>> Result { get; private set; }
		public bool AutoSortEnabled => this.AutoSortFlag.Checked;



		public DialogShipGroupSortOrder(DataGridView target, ShipGroupData group)
		{
            this.InitializeComponent();

			var rows_enabled = new LinkedList<DataGridViewRow>();
			var rows_disabled = new LinkedList<DataGridViewRow>();

			var columns = target.Columns.Cast<DataGridViewColumn>();
			var names = columns.Select(c => c.Name);

			if (group.SortOrder == null)
				group.SortOrder = new List<KeyValuePair<string, ListSortDirection>>();

			foreach (var sort in group.SortOrder.Where(s => names.Contains(s.Key)))
			{

				var row = new DataGridViewRow();

				row.CreateCells(this.EnabledView);
				row.SetValues(target.Columns[sort.Key].HeaderText, sort.Value);
				row.Cells[this.EnabledView_Name.Index].Tag = sort.Key;
				row.Tag = columns.FirstOrDefault(c => c.Name == sort.Key).DisplayIndex;

				rows_enabled.AddLast(row);
			}

			foreach (var name in columns.Where(c => c.SortMode != DataGridViewColumnSortMode.NotSortable && !group.SortOrder.Any(s => c.Name == s.Key))
				.Select(c => c.Name))
			{

				var row = new DataGridViewRow();

				row.CreateCells(this.DisabledView);
				row.SetValues(target.Columns[name].HeaderText);
				row.Cells[this.DisabledView_Name.Index].Tag = name;
				row.Tag = columns.FirstOrDefault(c => c.Name == name).DisplayIndex;

				rows_disabled.AddLast(row);
			}

            this.EnabledView.Rows.AddRange(rows_enabled.ToArray());
            this.DisabledView.Rows.AddRange(rows_disabled.ToArray());


            this.AutoSortFlag.Checked = group.AutoSortEnabled;
		}

		private void DialogShipGroupSortOrder_Load(object sender, EventArgs e)
		{
			if (this.Owner != null)
                this.Icon = this.Owner.Icon;
		}



		// ボタン操作
		private void EnabledView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
            if (e.RowIndex < 0 || this.EnabledView.RowCount <= e.RowIndex)
                return;

            if (e.ColumnIndex == this.EnabledView_SortDirection.Index)
			{

                this.EnabledView[e.ColumnIndex, e.RowIndex].Value = ((ListSortDirection)this.EnabledView[e.ColumnIndex, e.RowIndex].Value) == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

			}
			else if (e.ColumnIndex == this.EnabledView_Up.Index)
			{

				if (!ControlHelper.RowMoveUp(this.EnabledView, e.RowIndex))
				{
					System.Media.SystemSounds.Exclamation.Play();
				}

			}
			else if (e.ColumnIndex == this.EnabledView_Down.Index)
			{

				if (!ControlHelper.RowMoveDown(this.EnabledView, e.RowIndex))
				{
					System.Media.SystemSounds.Exclamation.Play();
				}

			}
		}


		private void EnabledView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.RowIndex >= 0 && e.ColumnIndex == this.EnabledView_SortDirection.Index)
			{

				switch ((ListSortDirection)e.Value)
				{
					case ListSortDirection.Ascending:
						e.Value = "▲";
						e.FormattingApplied = true;
						break;
					case ListSortDirection.Descending:
						e.Value = "▼";
						e.FormattingApplied = true;
						break;
				}

			}

		}



		private void DisabledView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{

			e.SortResult = (int)this.DisabledView.Rows[e.RowIndex1].Tag -
				(int)this.DisabledView.Rows[e.RowIndex2].Tag;
			e.Handled = true;

		}



		private void ButtonUp_Click(object sender, EventArgs e)
		{

			if (this.EnabledView.SelectedRows.Count == 0 || !ControlHelper.RowMoveUp(this.EnabledView, this.EnabledView.SelectedRows[0].Index))
			{
				System.Media.SystemSounds.Exclamation.Play();
			}
		}


		private void ButtonDown_Click(object sender, EventArgs e)
		{

			if (this.EnabledView.SelectedRows.Count == 0 || !ControlHelper.RowMoveDown(this.EnabledView, this.EnabledView.SelectedRows[0].Index))
			{
				System.Media.SystemSounds.Exclamation.Play();
			}
		}



		private void ButtonLeft_Click(object sender, EventArgs e)
		{

			var selectedRows = this.DisabledView.SelectedRows;

			if (selectedRows.Count == 0)
			{
				System.Media.SystemSounds.Asterisk.Play();
				return;
			}

			var addrows = new DataGridViewRow[selectedRows.Count];
			int i = 0;

			foreach (DataGridViewRow src in selectedRows)
			{
				addrows[i] = new DataGridViewRow();
				addrows[i].CreateCells(this.EnabledView);
				addrows[i].SetValues(src.Cells[this.DisabledView_Name.Index].Value, ListSortDirection.Ascending);
				addrows[i].Cells[this.EnabledView_Name.Index].Tag = src.Cells[this.DisabledView_Name.Index].Tag;
				addrows[i].Tag = src.Tag;
                this.DisabledView.Rows.Remove(src);
				i++;
			}

            this.EnabledView.Rows.AddRange(addrows);
            this.DisabledView.Sort(this.DisabledView_Name, ListSortDirection.Ascending);
		}

		private void ButtonRight_Click(object sender, EventArgs e)
		{

			var selectedRows = this.EnabledView.SelectedRows;

			if (selectedRows.Count == 0)
			{
				System.Media.SystemSounds.Asterisk.Play();
				return;
			}

			var addrows = new DataGridViewRow[selectedRows.Count];
			int i = 0;

			foreach (DataGridViewRow src in selectedRows)
			{
				addrows[i] = new DataGridViewRow();
				addrows[i].CreateCells(this.DisabledView);
				addrows[i].SetValues(src.Cells[this.DisabledView_Name.Index].Value);
				addrows[i].Cells[this.DisabledView_Name.Index].Tag = src.Cells[this.EnabledView_Name.Index].Tag;
				addrows[i].Tag = src.Tag;
                this.EnabledView.Rows.Remove(src);
				i++;
			}

            this.DisabledView.Rows.AddRange(addrows);
            this.DisabledView.Sort(this.DisabledView_Name, ListSortDirection.Ascending);
		}


		private void ButtonLeftAll_Click(object sender, EventArgs e)
		{

			var addrows = new DataGridViewRow[this.DisabledView.Rows.Count];
			int i = 0;

			foreach (DataGridViewRow src in this.DisabledView.Rows)
			{
				addrows[i] = new DataGridViewRow();
				addrows[i].CreateCells(this.EnabledView);
				addrows[i].SetValues(src.Cells[this.DisabledView_Name.Index].Value, ListSortDirection.Ascending);
				addrows[i].Cells[this.EnabledView_Name.Index].Tag = src.Cells[this.DisabledView_Name.Index].Tag;
				addrows[i].Tag = src.Tag;
				i++;
			}

            this.DisabledView.Rows.Clear();
            this.EnabledView.Rows.AddRange(addrows);
            this.DisabledView.Sort(this.DisabledView_Name, ListSortDirection.Ascending);

		}

		private void ButtonRightAll_Click(object sender, EventArgs e)
		{

			var addrows = new DataGridViewRow[this.EnabledView.Rows.Count];
			int i = 0;

			foreach (DataGridViewRow src in this.EnabledView.Rows)
			{
				addrows[i] = new DataGridViewRow();
				addrows[i].CreateCells(this.DisabledView);
				addrows[i].SetValues(src.Cells[this.DisabledView_Name.Index].Value);
				addrows[i].Cells[this.DisabledView_Name.Index].Tag = src.Cells[this.EnabledView_Name.Index].Tag;
				addrows[i].Tag = src.Tag;
				i++;
			}

            this.EnabledView.Rows.Clear();
            this.DisabledView.Rows.AddRange(addrows);
            this.DisabledView.Sort(this.DisabledView_Name, ListSortDirection.Ascending);

		}




		private void ButtonOK_Click(object sender, EventArgs e)
		{

            this.Result = new List<KeyValuePair<string, ListSortDirection>>(this.EnabledView.Rows.Count);

			foreach (DataGridViewRow row in this.EnabledView.Rows)
			{
                this.Result.Add(new KeyValuePair<string, ListSortDirection>((string)row.Cells[this.EnabledView_Name.Index].Tag, (ListSortDirection)row.Cells[this.EnabledView_SortDirection.Index].Value));
			}


            this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}


	}
}
