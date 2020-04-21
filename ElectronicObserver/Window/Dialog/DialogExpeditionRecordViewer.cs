using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
    public partial class DialogExpeditionRecordViewer : Form
    {
        private ExpeditionRecord _record;

        private const string _mapAny = "*";
        private const string _resultAny = "(전부)";

        private class SearchArgument
        {
            public int MissionID { get; set; }
            public int FlagShipLevel { get; set; }
            public string FlagShipType { get; set; }
            public string FlagShipName { get; set; }
            public DateTime DateBegin { get; set; }
            public DateTime DateEnd { get; set; }
            public string Result { get; set; }
            public DataGridViewRow BaseRow { get; set; }
            public bool MergeRows { get; set; }
        }

        public DialogExpeditionRecordViewer()
        {
            this.InitializeComponent();

            this._record = RecordManager.Instance.Expedition;
        }

        private void DialogExpeditionRecordViewer_Load(object sender, EventArgs e)
        {
            try
            {
                this.DateBegin.Value = this.DateBegin.MinDate = this.DateEnd.MinDate = this._record.Record.First().Date.Date;
                this.DateEnd.Value = this.DateBegin.MaxDate = this.DateEnd.MaxDate = DateTime.Now.AddDays(1).Date;

                var includedExpeditionIds = this._record.Record
                    .Select(r => r.MissionID)
                    .Distinct();

                var includedExpeditionDatas = includedExpeditionIds
                    .Select(id => KCDatabase.Instance.Mission.Values.FirstOrDefault(ex => ex.MissionID == id))
                    .Where(s => s != null);

                var dtbase = new DataTable();
                dtbase.Columns.AddRange(new DataColumn[] {
                new DataColumn( "Value", typeof( int ) ),
                new DataColumn( "Display", typeof( string ) ),
            });

                {
                    DataTable dt = dtbase.Clone();
                    dt.Rows.Add(-1, _mapAny);
                    foreach (var i in this._record.Record
                        .Select(r => r.MissionID)
                        .Distinct()
                        .OrderBy(i => i))
                        dt.Rows.Add(i, i.ToString());

                    this.ExpeditionID.DisplayMember = "Display";
                    this.ExpeditionID.ValueMember = "Value";
                    this.ExpeditionID.DataSource = dt;
                    this.ExpeditionID.SelectedIndex = 0;

                    dt.AcceptChanges();
                }

                {
                    DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn( "Value", typeof( string ) ),
                    new DataColumn( "Display", typeof( string ) ),
                     });
                    dt.Rows.Add(_resultAny, _resultAny);
                    foreach (var i in this._record.Record
                        .GroupBy(r => r.Result, (key, r) => r.First())
                        .OrderBy(r => r.Result))
                    {
                        dt.Rows.Add(i.Result, i.Result.ToString());
                    }

                    this.ComboBoxResultList.DisplayMember = "Display";
                    this.ComboBoxResultList.ValueMember = "Value";
                    this.ComboBoxResultList.DataSource = dt;
                    this.ComboBoxResultList.SelectedIndex = 0;

                    dt.AcceptChanges();
                }


                foreach (DataGridViewColumn column in this.RecordView.Columns)
                    column.Width = 20;
                /*
                this.LabelShipName.ImageList = ResourceManager.Instance.Icons;
                this.LabelShipName.ImageIndex = (int)ResourceManager.IconContent.HeadQuartersShip;
                this.LabelItemName.ImageList = ResourceManager.Instance.Icons;
                this.LabelItemName.ImageIndex = (int)ResourceManager.IconContent.ItemPresentBox;
                this.LabelEquipmentName.ImageList = ResourceManager.Instance.Equipments;
                this.LabelEquipmentName.ImageIndex = (int)ResourceManager.EquipmentContent.MainGunL;
                */
                this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.ItemPresentBox]);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message + ":" + x.StackTrace);
            }
        }

        private void DialogExpeditionRecordViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            ResourceManager.DestroyIcon(this.Icon);
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
                MissionID = (int)this.ExpeditionID.SelectedValue,
                Result = this.ComboBoxResultList.SelectedValue.ToString(),
                DateBegin = this.DateBegin.Value,
                DateEnd = this.DateEnd.Value,
                MergeRows = this.MergeRows.Checked,
                BaseRow = row
            };

            this.RecordView.Tag = args;

            this.RecordView_Item1.Visible =
            this.RecordView_Item1Count.Visible =
            this.RecordView_Item2.Visible =
            this.RecordView_Item2Count.Visible = false;

            // column initialize
            if (this.MergeRows.Checked)
            {
                this.RecordView_Name.DisplayIndex = 2;
                this.RecordView_Header.HeaderText = "회수";
                this.RecordView_Header.Width = 100;
                this.RecordView_Header.DisplayIndex = 1;
                this.RecordView_DateTime.Visible =
                this.RecordView_Result.Visible = false;

            }
            else
            {
                this.RecordView_Header.HeaderText = "";
                this.RecordView_Header.Width = 50;
                this.RecordView_Header.DisplayIndex = 0;
                this.RecordView_DateTime.Width = 150;
                this.RecordView_DateTime.Visible =
                this.RecordView_Result.Visible = true;
            }
            this.RecordView.ColumnHeadersVisible = true;

            this.StatusInfo.Text = "검색중입니다...";
            this.StatusInfo.Tag = DateTime.Now;

            this.Searcher.RunWorkerAsync(args);
        }


        private void Searcher_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchArgument args = (SearchArgument)e.Argument;

            var records = RecordManager.Instance.Expedition.Record;
            var rows = new LinkedList<DataGridViewRow>();

            int i = 0;
            var materials = new Dictionary<int, Dictionary<string, int>>();
            var items = new Dictionary<int, Dictionary<int,int>>();
            try
            {
                foreach (var r in records)
                {
                    #region Filtering

                    var ex = KCDatabase.Instance.Mission[r.MissionID];

                    if (ex == null) continue;

                    if (r.MissionID != args.MissionID && args.MissionID != -1) continue;

                    if (r.Result.Equals(args.Result) == false && args.Result.Equals(_resultAny) == false) continue;

                    if (r.Date < args.DateBegin || args.DateEnd < r.Date)
                        continue;

                    #endregion
                    if (args.MergeRows == false)
                    {
                        var row = (DataGridViewRow)args.BaseRow.Clone();

                        row.SetValues(
                            i + 1,
                            r.MissionID,
                            ex.Name,
                            r.Fuel,
                            r.Ammo,
                            r.Steel,
                            r.Baux,
                            null,
                            null,
                            null,
                            null,
                            r.Result,
                            r.Date
                            );

                        rows.AddLast(row);
                    }
                    else
                    {
                        int key = r.MissionID;
                        if (materials.ContainsKey(key) == false)
                        {
                            materials.Add(key, new Dictionary<string, int>
                        {
                            {"Fuel", r.Fuel },
                            {"Ammo", r.Ammo },
                            {"Steel", r.Steel },
                            {"Baux", r.Baux },
                            {"Count", 1 },
                        });
                        }
                        else
                        {
                            var material = materials[key];
                            material["Fuel"] += r.Fuel;
                            material["Ammo"] += r.Ammo;
                            material["Steel"] += r.Steel;
                            material["Baux"] += r.Baux;
                            material["Count"]++;
                        }
                        /*
                        if (items.ContainsKey(r.MissionID) == false)
                        {
                            items.Add(r.MissionID, new Dictionary<int, int>());
                        }

                        if (r.Item1Id != -1)
                        {
                            if (items[r.MissionID].ContainsKey(r.Item1Id) == false)
                            {
                                items[r.MissionID].Add(r.Item1Id, r.Item1Count);
                            }
                            else
                            {
                                items[r.MissionID][r.Item1Id] += r.Item1Count;
                            }
                        }

                        if (r.Item2Id != -1)
                        {
                            if (items[r.MissionID].ContainsKey(r.Item2Id) == false)
                            {
                                items[r.MissionID].Add(r.Item2Id, r.Item2Count);
                            }
                            else
                            {
                                items[r.MissionID][r.Item2Id] += r.Item2Count;
                            }
                        }
                        */
                    }

                    i++;
                }

            } catch (Exception X)
            {
                Console.WriteLine(X.Message + ":" + X.StackTrace);
            }
            // foreach end

            if (args.MergeRows)
            {
                foreach (var c in materials)
                {
                    var row = (DataGridViewRow)args.BaseRow.Clone();

                    var mission = KCDatabase.Instance.Mission.Values.FirstOrDefault(x => x.MissionID == c.Key);

                    row.SetValues(
                        c.Value["Count"],
                        c.Key,
                        mission.Name,
                        c.Value["Fuel"],
                        c.Value["Ammo"],
                        c.Value["Steel"],
                        c.Value["Baux"]
                        );

                    row.Cells[0].Tag = c.Value["Count"];
                    row.Cells[1].Tag = c.Value;

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

                this.StatusInfo.Text = "검색이 완료되었습니다. (" + (int)(DateTime.Now - (DateTime)this.StatusInfo.Tag).TotalMilliseconds + " ms)";

            }
            else
            {

                this.StatusInfo.Text = "검색이 취소되었습니다. ";
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
            SearchArgument args = (SearchArgument)this.RecordView.Tag;
        }

        private void RecordView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SearchArgument args = (SearchArgument)this.RecordView.Tag;

        }

        private void RecordView_SelectionChanged(object sender, EventArgs e)
        {
            var args = this.RecordView.Tag as SearchArgument;
            if (args == null)
                return;
        }
    }
}
