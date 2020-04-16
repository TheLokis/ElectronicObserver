namespace ElectronicObserver.Window.Dialog
{
	partial class DialogDropRecordViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ItemName = new System.Windows.Forms.ComboBox();
            this.ShipName = new System.Windows.Forms.ComboBox();
            this.DateBegin = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DateEnd = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonRun = new System.Windows.Forms.Button();
            this.RecordView = new System.Windows.Forms.DataGridView();
            this.RecordView_Header = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecordView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecordView_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MergeRows = new System.Windows.Forms.CheckBox();
            this.LabelShipName = new ElectronicObserver.Window.Control.ImageLabel();
            this.LabelItemName = new ElectronicObserver.Window.Control.ImageLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.Searcher = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.RecordView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ItemName
            // 
            this.ItemName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemName.FormattingEnabled = true;
            this.ItemName.Location = new System.Drawing.Point(75, 39);
            this.ItemName.Name = "ItemName";
            this.ItemName.Size = new System.Drawing.Size(121, 23);
            this.ItemName.TabIndex = 3;
            this.ToolTipInfo.SetToolTip(this.ItemName, "검색 대상의 이름을 입력합니다.\r\n(드롭) 은 아이템이 드롭됬을때만 출력됩니다. ");
            // 
            // ShipName
            // 
            this.ShipName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ShipName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ShipName.FormattingEnabled = true;
            this.ShipName.Location = new System.Drawing.Point(75, 10);
            this.ShipName.Name = "ShipName";
            this.ShipName.Size = new System.Drawing.Size(121, 23);
            this.ShipName.TabIndex = 1;
            this.ToolTipInfo.SetToolTip(this.ShipName, "검색 대상의 이름을 입력합니다. \r\n(드롭)은 함선이 드롭됬을때만 출력됩니다. ");
            // 
            // DateBegin
            // 
            this.DateBegin.CustomFormat = "yyyy/MM/dd";
            this.DateBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateBegin.Location = new System.Drawing.Point(239, 10);
            this.DateBegin.Name = "DateBegin";
            this.DateBegin.Size = new System.Drawing.Size(140, 23);
            this.DateBegin.TabIndex = 7;
            this.ToolTipInfo.SetToolTip(this.DateBegin, "검색할 날짜의 시작점을 지정하세요.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "시작";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(202, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "종료";
            // 
            // DateEnd
            // 
            this.DateEnd.CustomFormat = "yyyy/MM/dd";
            this.DateEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateEnd.Location = new System.Drawing.Point(239, 39);
            this.DateEnd.Name = "DateEnd";
            this.DateEnd.Size = new System.Drawing.Size(140, 23);
            this.DateEnd.TabIndex = 9;
            this.ToolTipInfo.SetToolTip(this.DateEnd, "검색할 날짜의 종료지점을 지정하세요.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(385, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "해역 노드";
            // 
            // ButtonRun
            // 
            this.ButtonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonRun.Location = new System.Drawing.Point(543, 68);
            this.ButtonRun.Name = "ButtonRun";
            this.ButtonRun.Size = new System.Drawing.Size(69, 23);
            this.ButtonRun.TabIndex = 21;
            this.ButtonRun.Text = "검색";
            this.ButtonRun.UseVisualStyleBackColor = true;
            this.ButtonRun.Click += new System.EventHandler(this.ButtonRun_Click);
            // 
            // RecordView
            // 
            this.RecordView.AllowUserToAddRows = false;
            this.RecordView.AllowUserToDeleteRows = false;
            this.RecordView.AllowUserToResizeRows = false;
            this.RecordView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.RecordView.ColumnHeadersVisible = false;
            this.RecordView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RecordView_Header,
            this.RecordView_Name,
            this.RecordView_Date});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.RecordView.DefaultCellStyle = dataGridViewCellStyle10;
            this.RecordView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecordView.Location = new System.Drawing.Point(0, 0);
            this.RecordView.Name = "RecordView";
            this.RecordView.ReadOnly = true;
            this.RecordView.RowHeadersVisible = false;
            this.RecordView.RowTemplate.Height = 21;
            this.RecordView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.RecordView.Size = new System.Drawing.Size(624, 315);
            this.RecordView.TabIndex = 1;
            this.RecordView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.RecordView_CellDoubleClick);
            this.RecordView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.RecordView_CellFormatting);
            this.RecordView.SelectionChanged += new System.EventHandler(this.RecordView_SelectionChanged);
            this.RecordView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.RecordView_SortCompare);
            this.RecordView.Sorted += new System.EventHandler(this.RecordView_Sorted);
            // 
            // RecordView_Header
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.RecordView_Header.DefaultCellStyle = dataGridViewCellStyle6;
            this.RecordView_Header.HeaderText = "";
            this.RecordView_Header.Name = "RecordView_Header";
            this.RecordView_Header.ReadOnly = true;
            this.RecordView_Header.Width = 50;
            // 
            // RecordView_Name
            // 
            this.RecordView_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RecordView_Name.HeaderText = "이름";
            this.RecordView_Name.Name = "RecordView_Name";
            this.RecordView_Name.ReadOnly = true;
            // 
            // RecordView_Date
            // 
            this.RecordView_Date.HeaderText = "날짜";
            this.RecordView_Date.Name = "RecordView_Date";
            this.RecordView_Date.ReadOnly = true;
            this.RecordView_Date.Width = 150;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.MergeRows);
            this.splitContainer1.Panel1.Controls.Add(this.LabelShipName);
            this.splitContainer1.Panel1.Controls.Add(this.LabelItemName);
            this.splitContainer1.Panel1.Controls.Add(this.ItemName);
            this.splitContainer1.Panel1.Controls.Add(this.ButtonRun);
            this.splitContainer1.Panel1.Controls.Add(this.ShipName);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.DateBegin);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.DateEnd);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.RecordView);
            this.splitContainer1.Size = new System.Drawing.Size(624, 419);
            this.splitContainer1.SplitterDistance = 100;
            this.splitContainer1.TabIndex = 2;
            // 
            // MergeRows
            // 
            this.MergeRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MergeRows.AutoSize = true;
            this.MergeRows.Location = new System.Drawing.Point(487, 70);
            this.MergeRows.Name = "MergeRows";
            this.MergeRows.Size = new System.Drawing.Size(50, 19);
            this.MergeRows.TabIndex = 20;
            this.MergeRows.Text = "정리";
            this.ToolTipInfo.SetToolTip(this.MergeRows, "체크하면 동일 드랍 항목을 정리합니다.\r\n");
            this.MergeRows.UseVisualStyleBackColor = true;
            // 
            // LabelShipName
            // 
            this.LabelShipName.AutoSize = false;
            this.LabelShipName.BackColor = System.Drawing.Color.Transparent;
            this.LabelShipName.Location = new System.Drawing.Point(12, 13);
            this.LabelShipName.Name = "LabelShipName";
            this.LabelShipName.Size = new System.Drawing.Size(57, 16);
            this.LabelShipName.TabIndex = 0;
            this.LabelShipName.Text = "함선";
            this.LabelShipName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelItemName
            // 
            this.LabelItemName.AutoSize = false;
            this.LabelItemName.BackColor = System.Drawing.Color.Transparent;
            this.LabelItemName.Location = new System.Drawing.Point(12, 42);
            this.LabelItemName.Name = "LabelItemName";
            this.LabelItemName.Size = new System.Drawing.Size(57, 16);
            this.LabelItemName.TabIndex = 2;
            this.LabelItemName.Text = "아이템";
            this.LabelItemName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 419);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusInfo
            // 
            this.StatusInfo.Name = "StatusInfo";
            this.StatusInfo.Size = new System.Drawing.Size(12, 17);
            this.StatusInfo.Text = "-";
            // 
            // ToolTipInfo
            // 
            this.ToolTipInfo.AutoPopDelay = 30000;
            this.ToolTipInfo.InitialDelay = 500;
            this.ToolTipInfo.ReshowDelay = 100;
            this.ToolTipInfo.ShowAlways = true;
            // 
            // Searcher
            // 
            this.Searcher.WorkerSupportsCancellation = true;
            this.Searcher.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Searcher_DoWork);
            this.Searcher.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Searcher_RunWorkerCompleted);
            // 
            // DialogDropRecordViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Name = "DialogDropRecordViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "드랍기록";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogDropRecordViewer_FormClosed);
            this.Load += new System.EventHandler(this.DialogDropRecordViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RecordView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button ButtonRun;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DateTimePicker DateEnd;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker DateBegin;
		private System.Windows.Forms.ComboBox ShipName;
		private System.Windows.Forms.ComboBox ItemName;
		private Control.ImageLabel LabelItemName;
		private Control.ImageLabel LabelShipName;
		private System.Windows.Forms.DataGridView RecordView;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.CheckBox MergeRows;
		private System.Windows.Forms.ToolTip ToolTipInfo;
		private System.Windows.Forms.ToolStripStatusLabel StatusInfo;
		private System.ComponentModel.BackgroundWorker Searcher;
		private System.Windows.Forms.DataGridViewTextBoxColumn RecordView_Header;
		private System.Windows.Forms.DataGridViewTextBoxColumn RecordView_Name;
		private System.Windows.Forms.DataGridViewTextBoxColumn RecordView_Date;
	}
}