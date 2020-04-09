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
    public partial class DialogDropRecordViewer : Form
    {

        private ShipDropRecord _record;

        private const string NameAny = "(전부)";
        private const string NameNotExist = "(없음)";
        private const string NameFullPort = "(여유공간X)";
        private const string NameExist = "(드롭)";

        private const string MapAny = "*";

        private Dictionary<int, DataTable> MapCellTable;


        private class SearchArgument
        {
            public string ShipName;
            public string ItemName;
            public string EquipmentName;
            public DateTime DateBegin;
            public DateTime DateEnd;
            public int MapAreaID;
            public int MapInfoID;
            public int MapCellID;
            public int MapDifficulty;
            public CheckState IsBossOnly;
            public bool RankS;
            public bool RankA;
            public bool RankB;
            public bool RankX;
            public bool MergeRows;
            public DataGridViewRow BaseRow;
        }


        public DialogDropRecordViewer()
        {
            this.InitializeComponent();

            this._record = RecordManager.Instance.ShipDrop;
        }

        private void DialogDropRecordViewer_Load(object sender, EventArgs e)
        {

            var includedShipNames = this._record.Record
                .Select(r => r.ShipName)
                .Distinct()
                .Except(new[] { NameNotExist, NameFullPort });

            var includedShipObjects = includedShipNames
                .Select(name => KCDatabase.Instance.MasterShips.Values.FirstOrDefault(ship => ship.NameWithClass == name))
                .Where(s => s != null);

            var removedShipNames = includedShipNames.Except(includedShipObjects.Select(s => s.NameWithClass));


            var includedItemNames = this._record.Record
                .Select(r => r.ItemName)
                .Distinct()
                .Except(new[] { NameNotExist });

            var includedItemObjects = includedItemNames
                .Select(name => KCDatabase.Instance.MasterUseItems.Values.FirstOrDefault(item => item.Name == name))
                .Where(s => s != null);

            var removedItemNames = includedItemNames.Except(includedItemObjects.Select(item => item.Name));

            var dtbase = new DataTable();
            dtbase.Columns.AddRange(new DataColumn[] {
                new DataColumn( "Value", typeof( int ) ),
                new DataColumn( "Display", typeof( string ) ),
            });



            this.MapCellTable = new Dictionary<int, DataTable>();
            {
                var dict = new Dictionary<int, HashSet<int>>();

                foreach (var r in this._record.Record)
                {
                    int id = r.MapAreaID * 10 + r.MapInfoID;

                    if (!dict.ContainsKey(id))
                    {
                        dict.Add(id, new HashSet<int>());
                    }

                    dict[id].Add(r.CellID);
                }

                foreach (var p in dict)
                {
                    int MapAreaID = p.Key / 10;
                    int MapInfoID = p.Key % 10;
                    this.MapCellTable.Add(p.Key, dtbase.Clone());
                    this.MapCellTable[p.Key].Rows.Add(-1, MapAny);

                    if (Utility.Configuration.Config.FormCompass.ToAlphabet)
                    {
                        foreach (var c in p.Value.OrderBy(k => k))
                            this.MapCellTable[p.Key].Rows.Add(c, c.ToString() + "(" + NodeData.GetNodeName(MapAreaID, MapInfoID, c) + ")");
                    }
                    else
                    {
                        foreach (var c in p.Value.OrderBy(k => k))
                            this.MapCellTable[p.Key].Rows.Add(c, c.ToString());
                    }
                    this.MapCellTable[p.Key].AcceptChanges();
                }
            }



            this.ShipName.Items.Add(NameAny);
            this.ShipName.Items.Add(NameExist);
            this.ShipName.Items.Add(NameNotExist);
            this.ShipName.Items.Add(NameFullPort);
            this.ShipName.Items.AddRange(includedShipObjects
                .OrderBy(s => s.NameReading)
                .OrderBy(s => s.ShipType)
                .Select(s => s.NameWithClass)
                .Union(removedShipNames.OrderBy(s => s))
                .ToArray()
                );
            this.ShipName.SelectedIndex = 0;

            this.ItemName.Items.Add(NameAny);
            this.ItemName.Items.Add(NameExist);
            this.ItemName.Items.Add(NameNotExist);
            this.ItemName.Items.AddRange(includedItemObjects
                .OrderBy(i => i.ItemID)
                .Select(i => i.Name)
                .Union(removedItemNames.OrderBy(i => i))
                .ToArray()
                );
            this.ItemName.SelectedIndex = 0;

            // not implemented: eq


            this.DateBegin.Value = this.DateBegin.MinDate = this.DateEnd.MinDate = this._record.Record.First().Date.Date;
            this.DateEnd.Value = this.DateBegin.MaxDate = this.DateEnd.MaxDate = DateTime.Now.AddDays(1).Date;

            {
                DataTable dt = dtbase.Clone();
                dt.Rows.Add(-1, MapAny);
                foreach (var i in this._record.Record
                    .Select(r => r.MapAreaID)
                    .Distinct()
                    .OrderBy(i => i))
                    dt.Rows.Add(i, i.ToString());
                dt.AcceptChanges();
                this.MapAreaID.DisplayMember = "Display";
                this.MapAreaID.ValueMember = "Value";
                this.MapAreaID.DataSource = dt;
                this.MapAreaID.SelectedIndex = 0;
            }

            {
                DataTable dt = dtbase.Clone();
                dt.Rows.Add(-1, MapAny);
                foreach (var i in this._record.Record
                    .Select(r => r.MapInfoID)
                    .Distinct()
                    .OrderBy(i => i))
                    dt.Rows.Add(i, i.ToString());
                dt.AcceptChanges();
                this.MapInfoID.DisplayMember = "Display";
                this.MapInfoID.ValueMember = "Value";
                this.MapInfoID.DataSource = dt;
                this.MapInfoID.SelectedIndex = 0;
            }

            {
                DataTable dt = dtbase.Clone();
                dt.Rows.Add(-1, MapAny);
                // 残りは都度生成する
                dt.AcceptChanges();
                this.MapCellID.DisplayMember = "Display";
                this.MapCellID.ValueMember = "Value";
                this.MapCellID.DataSource = dt;
                this.MapCellID.SelectedIndex = 0;
            }

            {
                DataTable dt = dtbase.Clone();
                dt.Rows.Add(0, MapAny);
                foreach (var diff in this._record.Record
                    .Select(r => r.Difficulty)
                    .Distinct()
                    .Except(new[] { 0 })
                    .OrderBy(i => i))
                    dt.Rows.Add(diff, Constants.GetDifficulty(diff));
                dt.AcceptChanges();
                this.MapDifficulty.DisplayMember = "Display";
                this.MapDifficulty.ValueMember = "Value";
                this.MapDifficulty.DataSource = dt;
                this.MapDifficulty.SelectedIndex = 0;
            }


            foreach (DataGridViewColumn column in this.RecordView.Columns)
                column.Width = 20;

            this.LabelShipName.ImageList = ResourceManager.Instance.Icons;
            this.LabelShipName.ImageIndex = (int)ResourceManager.IconContent.HeadQuartersShip;
            this.LabelItemName.ImageList = ResourceManager.Instance.Icons;
            this.LabelItemName.ImageIndex = (int)ResourceManager.IconContent.ItemPresentBox;
            this.LabelEquipmentName.ImageList = ResourceManager.Instance.Equipments;
            this.LabelEquipmentName.ImageIndex = (int)ResourceManager.EquipmentContent.MainGunL;

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.ItemPresentBox]);
        }

        private void DialogDropRecordViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            ResourceManager.DestroyIcon(this.Icon);
        }



        private string GetContentString(ShipDropRecord.ShipDropElement elem, bool ignoreShip = false, bool ignoreItem = false, bool ignoreEquipment = false)
        {

            if (elem.ShipID > 0 && !ignoreShip)
            {

                if (elem.ItemID > 0 && !ignoreItem)
                {
                    if (elem.EquipmentID > 0 && !ignoreEquipment)
                        return elem.ShipName + " + " + elem.ItemName + " + " + elem.EquipmentName;
                    else
                        return elem.ShipName + " + " + elem.ItemName;
                }
                else
                {
                    if (elem.EquipmentID > 0 && !ignoreEquipment)
                        return elem.ShipName + " + " + elem.EquipmentName;
                    else
                        return elem.ShipName;
                }

            }
            else
            {
                if (elem.ItemID > 0 && !ignoreItem)
                {
                    if (elem.EquipmentID > 0 && !ignoreEquipment)
                        return elem.ItemName + " + " + elem.EquipmentName;
                    else
                        return elem.ItemName;
                }
                else
                {
                    if (elem.EquipmentID > 0 && !ignoreEquipment)
                        return elem.EquipmentName;
                    else
                        return elem.ShipName;
                }
            }

        }

        private string GetContentStringForSorting(ShipDropRecord.ShipDropElement elem, bool ignoreShip = false, bool ignoreItem = false, bool ignoreEquipment = false)
        {

            var ship = KCDatabase.Instance.MasterShips[elem.ShipID];
            var item = KCDatabase.Instance.MasterUseItems[elem.ItemID];
            var eq = KCDatabase.Instance.MasterEquipments[elem.EquipmentID];

            if (ship != null && ship.Name != elem.ShipName) ship = null;
            if (item != null && item.Name != elem.ItemName) item = null;
            if (eq != null && eq.Name != elem.EquipmentName) eq = null;

            StringBuilder sb = new StringBuilder();


            if (elem.ShipID > 0 && !ignoreShip)
            {
                sb.AppendFormat("0{0:D4}{1}/{2}", (int?)ship?.ShipType ?? 0, ship?.NameReading ?? elem.ShipName, elem.ShipName);
            }

            if (elem.ItemID > 0 && !ignoreItem)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.AppendFormat("1{0:D4}{1}", item?.ItemID ?? 0, elem.ItemName);
            }

            if (elem.EquipmentID > 0 && !ignoreEquipment)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.AppendFormat("2{0:D4}{1}", eq?.EquipmentID ?? 0, elem.EquipmentName);
            }

            return sb.ToString();
        }


        private string ConvertContentString(string str)
        {

            if (str.Length == 0)
                return NameNotExist;

            StringBuilder sb = new StringBuilder();

            foreach (var s in str.Split(",".ToCharArray()))
            {

                if (sb.Length > 0)
                    sb.Append(" + ");

                switch (s[0])
                {
                    case '0':
                        sb.Append(s.Substring(s.IndexOf("/") + 1));
                        break;
                    case '1':
                    case '2':
                        sb.Append(s.Substring(5));
                        break;
                }
            }

            return sb.ToString();
        }



        private string GetMapString(int maparea, int mapinfo, int cell = -1, bool isboss = false, int difficulty = -1, bool insertEnemyFleetName = true)
        {
            var sb = new StringBuilder();
            sb.Append(maparea);
            sb.Append("-");
            sb.Append(mapinfo);
            if (difficulty != -1)
                sb.AppendFormat("[{0}]", Constants.GetDifficulty(difficulty));
            if (cell != -1)
            {
                sb.Append("-");
                if (Utility.Configuration.Config.FormCompass.ToAlphabet)
                {
                    sb.Append(NodeData.GetNodeName(maparea, mapinfo, cell));
                }
                else { sb.Append(cell); }
            }
            if (isboss)
                sb.Append(" [보스]");

            if (insertEnemyFleetName)
            {
                var enemy = RecordManager.Instance.EnemyFleet.Record.Values.FirstOrDefault(r => r.MapAreaID == maparea && r.MapInfoID == mapinfo && r.CellID == cell && r.Difficulty == difficulty);
                if (enemy != null)
                    sb.AppendFormat(" ({0})", enemy.FleetName);
            }

            return sb.ToString();
        }

        private string GetMapString(int serialID, bool insertEnemyFleetName = true)
        {
            return this.GetMapString(serialID >> 24 & 0xFF, serialID >> 16 & 0xFF, serialID >> 8 & 0xFF, (serialID & 1) != 0, (sbyte)((serialID >> 1 & 0x7F) << 1) >> 1, insertEnemyFleetName);
        }

        private int GetMapSerialID(int maparea, int mapinfo, int cell, bool isboss, int difficulty = -1)
        {
            return (maparea & 0xFF) << 24 | (mapinfo & 0xFF) << 16 | (cell & 0xFF) << 8 | (difficulty & 0x7F) << 1 | (isboss ? 1 : 0);
        }


        private void MapAreaID_SelectedIndexChanged(object sender, EventArgs e)
        {

            int maparea = (int)(this.MapAreaID.SelectedValue ?? -1);
            int mapinfo = (int)(this.MapInfoID.SelectedValue ?? -1);

            if (maparea == -1 || mapinfo == -1)
            {
                this.MapCellID.Enabled = false;
                if (this.MapCellID.Items.Count > 0)
                    this.MapCellID.SelectedIndex = 0;

            }
            else
            {
                this.MapCellID.Enabled = true;
                if (this.MapCellTable.ContainsKey(maparea * 10 + mapinfo))
                {
                    this.MapCellID.DataSource = this.MapCellTable[maparea * 10 + mapinfo];
                }
                else
                {
                    this.MapCellID.Enabled = false;
                }
                this.MapCellID.SelectedIndex = 0;
            }
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
                ShipName = this.ShipName.Text,
                ItemName = (string)this.ItemName.SelectedItem,
                EquipmentName = (string)this.EquipmentName.SelectedItem,
                DateBegin = this.DateBegin.Value,
                DateEnd = this.DateEnd.Value,
                MapAreaID = (int)this.MapAreaID.SelectedValue,
                MapInfoID = (int)this.MapInfoID.SelectedValue,
                MapCellID = (int)this.MapCellID.SelectedValue,
                MapDifficulty = (int)this.MapDifficulty.SelectedValue,
                IsBossOnly = this.IsBossOnly.CheckState,
                RankS = this.RankS.Checked,
                RankA = this.RankA.Checked,
                RankB = this.RankB.Checked,
                RankX = this.RankX.Checked,
                MergeRows = this.MergeRows.Checked,
                BaseRow = row
            };

            this.RecordView.Tag = args;


            // column initialize
            if (this.MergeRows.Checked)
            {
                this.RecordView_Name.DisplayIndex = 0;
                this.RecordView_Header.HeaderText = "회수";
                this.RecordView_Header.Width = 100;
                this.RecordView_Header.DisplayIndex = 1;
                this.RecordView_RankS.Width = 100;
                this.RecordView_RankS.Visible = true;
                this.RecordView_RankA.Width = 100;
                this.RecordView_RankA.Visible = true;
                this.RecordView_RankB.Width = 100;
                this.RecordView_RankB.Visible = true;

                this.RecordView_Date.Visible = false;
                this.RecordView_Map.Visible = false;
                this.RecordView_Rank.Visible = false;

            }
            else
            {
                this.RecordView_Header.HeaderText = "";
                this.RecordView_Header.Width = 50;
                this.RecordView_Header.DisplayIndex = 0;
                this.RecordView_Date.Width = 150;
                this.RecordView_Date.Visible = true;
                this.RecordView_Map.Width = 240;
                this.RecordView_Map.Visible = true;
                this.RecordView_Rank.Width = 40;
                this.RecordView_Rank.Visible = true;

                this.RecordView_RankS.Visible = false;
                this.RecordView_RankA.Visible = false;
                this.RecordView_RankB.Visible = false;

            }
            this.RecordView.ColumnHeadersVisible = true;


            this.StatusInfo.Text = "검색중입니다...";
            this.StatusInfo.Tag = DateTime.Now;

            this.Searcher.RunWorkerAsync(args);
        }


        private void Searcher_DoWork(object sender, DoWorkEventArgs e)
        {

            SearchArgument args = (SearchArgument)e.Argument;


            int priorityShip =
                args.ShipName == NameAny ? 0 :
                args.ShipName == NameExist ? 1 : 2;
            int priorityItem =
                args.ItemName == NameAny ? 0 :
                args.ItemName == NameExist ? 1 : 2;
            int priorityContent = Math.Max(priorityShip, priorityItem);

            var records = RecordManager.Instance.ShipDrop.Record;
            var rows = new LinkedList<DataGridViewRow>();


            //lock ( records ) 
            {
                int i = 0;
                var counts = new Dictionary<string, int[]>();
                var allcounts = new Dictionary<string, int[]>();

                List<int> Same_Node = new List<int>();
                Same_Node = NodeData.Get_Same_Node(args.MapAreaID, args.MapInfoID, args.MapCellID);
                bool exists_other_cell = false;
                int SameNode = -1;
                if (Utility.Configuration.Config.FormCompass.ToAlphabet)
                {
                    if (Same_Node.Count > 1)
                    {
                        exists_other_cell = true;
                        foreach (int Node in Same_Node)
                        {
                            if (args.MapCellID != Node)
                                SameNode = Node;
                        }
                    }
                }

                foreach (var r in records)
                {

                    #region Filtering

                    if (r.Date < args.DateBegin || args.DateEnd < r.Date)
                        continue;

                    if (((r.Rank == "SS" || r.Rank == "S") && !args.RankS) ||
                         ((r.Rank == "A") && !args.RankA) ||
                         ((r.Rank == "B") && !args.RankB) ||
                         ((Constants.GetWinRank(r.Rank) <= 3) && !args.RankX))
                        continue;


                    if (args.MapAreaID != -1 && args.MapAreaID != r.MapAreaID)
                        continue;
                    if (args.MapInfoID != -1 && args.MapInfoID != r.MapInfoID)
                        continue;

                    if (exists_other_cell)
                    {
                        if (args.MapCellID != -1 && args.MapCellID != r.CellID && SameNode != r.CellID)
                            continue;
                    }
                    else
                    {
                        if (args.MapCellID != -1 && (args.MapCellID != r.CellID))
                            continue;
                    }


                    switch (args.IsBossOnly)
                    {
                        case CheckState.Unchecked:
                            if (r.IsBossNode)
                                continue;
                            break;
                        case CheckState.Checked:
                            if (!r.IsBossNode)
                                continue;
                            break;
                    }
                    if (args.MapDifficulty != 0 && args.MapDifficulty != r.Difficulty)
                        continue;



                    if (args.MergeRows)
                    {
                        string key;

                        if (priorityContent == 2)
                        {
                            key = this.GetMapSerialID(r.MapAreaID, r.MapInfoID, r.CellID, r.IsBossNode, args.MapDifficulty == 0 ? -1 : r.Difficulty).ToString("X8");

                        }
                        else
                        {
                            key = this.GetContentString(r, priorityShip < priorityItem && priorityShip < 2, priorityShip >= priorityItem && priorityItem < 2);
                        }


                        if (!allcounts.ContainsKey(key))
                        {
                            allcounts.Add(key, new int[4]);
                        }

                        switch (r.Rank)
                        {
                            case "B":
                                allcounts[key][3]++;
                                break;
                            case "A":
                                allcounts[key][2]++;
                                break;
                            case "S":
                            case "SS":
                                allcounts[key][1]++;
                                break;
                        }
                        allcounts[key][0]++;
                    }



                    switch (args.ShipName)
                    {
                        case NameAny:
                            break;
                        case NameExist:
                            if (r.ShipID < 0)
                                continue;
                            break;
                        case NameNotExist:
                            if (r.ShipID != -1)
                                continue;
                            break;
                        case NameFullPort:
                            if (r.ShipID != -2)
                                continue;
                            break;
                        default:
                            if (r.ShipName != args.ShipName)
                                continue;
                            break;
                    }

                    switch (args.ItemName)
                    {
                        case NameAny:
                            break;
                        case NameExist:
                            if (r.ItemID < 0)
                                continue;
                            break;
                        case NameNotExist:
                            if (r.ItemID != -1)
                                continue;
                            break;
                        default:
                            if (r.ItemName != args.ItemName)
                                continue;
                            break;
                    }

                    #endregion


                    if (!args.MergeRows)
                    {
                        var row = (DataGridViewRow)args.BaseRow.Clone();

                        row.SetValues(
                            i + 1,
                            this.GetContentString(r),
                            r.Date,
                            this.GetMapString(r.MapAreaID, r.MapInfoID, r.CellID, r.IsBossNode, r.Difficulty),
                            Constants.GetWinRank(r.Rank),
                            null,
                            null,
                            null
                            );

                        row.Cells[1].Tag = this.GetContentStringForSorting(r);
                        row.Cells[3].Tag = this.GetMapSerialID(r.MapAreaID, r.MapInfoID, r.CellID, r.IsBossNode, r.Difficulty);

                        rows.AddLast(row);


                    }
                    else
                    {
                        //merged

                        string key;

                        if (priorityContent == 2)
                        {
                            key = this.GetMapSerialID(r.MapAreaID, r.MapInfoID, r.CellID, r.IsBossNode, args.MapDifficulty == 0 ? -1 : r.Difficulty).ToString("X8");

                        }
                        else
                        {
                            key = this.GetContentStringForSorting(r, priorityShip < priorityItem && priorityShip < 2, priorityShip >= priorityItem && priorityItem < 2);
                        }


                        if (!counts.ContainsKey(key))
                        {
                            counts.Add(key, new int[4]);
                        }

                        switch (r.Rank)
                        {
                            case "B":
                                counts[key][3]++;
                                break;
                            case "A":
                                counts[key][2]++;
                                break;
                            case "S":
                            case "SS":
                                counts[key][1]++;
                                break;
                        }
                        counts[key][0]++;

                    }



                    if (this.Searcher.CancellationPending)
                        break;

                    i++;
                }


                if (args.MergeRows)
                {

                    int[] allcountssum = Enumerable.Range(0, 4).Select(k => allcounts.Values.Sum(a => a[k])).ToArray();

                    foreach (var c in counts)
                    {
                        var row = (DataGridViewRow)args.BaseRow.Clone();

                        string name = c.Key;

                        if (int.TryParse(name, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out int serialID))
                            name = this.GetMapString(serialID);

                        // fixme: name != map だった時にソートキーが入れられない

                        row.SetValues(
                            c.Value[0],
                            serialID != 0 ? name : this.ConvertContentString(name),
                            null,
                            null,
                            null,
                            c.Value[1],
                            c.Value[2],
                            c.Value[3]
                            );


                        if (priorityContent == 2)
                        {
                            row.Cells[0].Tag = allcounts[c.Key][0];
                            if (serialID != 0)
                                row.Cells[1].Tag = serialID;
                            else
                                row.Cells[1].Tag = name;
                            row.Cells[5].Tag = allcounts[c.Key][1];
                            row.Cells[6].Tag = allcounts[c.Key][2];
                            row.Cells[7].Tag = allcounts[c.Key][3];

                        }
                        else
                        {
                            row.Cells[0].Tag = ((double)c.Value[0] / Math.Max(allcountssum[0], 1));
                            if (serialID != 0)
                                row.Cells[1].Tag = serialID;
                            else
                                row.Cells[1].Tag = name;
                            row.Cells[5].Tag = ((double)c.Value[1] / Math.Max(allcountssum[1], 1));
                            row.Cells[6].Tag = ((double)c.Value[2] / Math.Max(allcountssum[2], 1));
                            row.Cells[7].Tag = ((double)c.Value[3] / Math.Max(allcountssum[3], 1));

                        }

                        rows.AddLast(row);
                    }

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

            if (args.MergeRows)
            {
                // merged

                if (e.ColumnIndex == this.RecordView_Header.Index ||
                     e.ColumnIndex == this.RecordView_RankS.Index ||
                     e.ColumnIndex == this.RecordView_RankA.Index ||
                     e.ColumnIndex == this.RecordView_RankB.Index)
                {

                    if (this.RecordView[e.ColumnIndex, e.RowIndex].Tag is double)
                    {
                        e.Value = string.Format("{0} ({1:p1})", e.Value, (double)this.RecordView[e.ColumnIndex, e.RowIndex].Tag);
                    }
                    else
                    {
                        int max = (int)this.RecordView[e.ColumnIndex, e.RowIndex].Tag;
                        e.Value = string.Format("{0}/{1} ({2:p1})", e.Value, max, (double)((int)e.Value) / Math.Max(max, 1));
                    }
                    e.FormattingApplied = true;
                }

            }
            else
            {
                //not merged

                if (e.ColumnIndex == this.RecordView_Date.Index)
                {
                    e.Value = DateTimeHelper.TimeToCSVString((DateTime)e.Value);
                    e.FormattingApplied = true;

                }
                else if (e.ColumnIndex == this.RecordView_Rank.Index)
                {
                    e.Value = Constants.GetWinRank((int)e.Value);
                    e.FormattingApplied = true;
                }
            }

        }


        private void RecordView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            SearchArgument args = (SearchArgument)this.RecordView.Tag;
            if (args == null || args.MergeRows)
                return;

            try
            {

                DateTime time = Convert.ToDateTime(this.RecordView[this.RecordView_Date.Index, e.RowIndex].Value);


                if (!Directory.Exists(Data.Battle.BattleManager.BattleLogPath))
                {
                    this.StatusInfo.Text = "전투 로그를 찾을 수 없습니다.";
                    return;
                }

                this.StatusInfo.Text = "전투 로그를 검색하고 있습니다...";
                string battleLogFile = Directory.EnumerateFiles(Data.Battle.BattleManager.BattleLogPath,
                    time.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture) + "*.txt",
                    SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (battleLogFile == null)
                {
                    this.StatusInfo.Text = "전투 로그를 찾을 수 없습니다.";
                    return;
                }

                this.StatusInfo.Text = string.Format("전투 로그 {0} 를 엽니다.", Path.GetFileName(battleLogFile));
                System.Diagnostics.Process.Start(battleLogFile);


            }
            catch (Exception)
            {
                this.StatusInfo.Text = "전투 로그를 열 수 없습니다.";
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
