namespace ElectronicObserver.Window.Dialog
{
	partial class DialogExpChecker
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ExpControl = new System.Windows.Forms.CheckBox();
            this.Rank_List = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.MVP_Check = new System.Windows.Forms.CheckBox();
            this.FlagShip_Check = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.MapSelect = new System.Windows.Forms.ComboBox();
            this.TextShip = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchInFleet = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ShowAllASWEquipments = new System.Windows.Forms.CheckBox();
            this.ShowAllLevel = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ASWModernization = new System.Windows.Forms.NumericUpDown();
            this.ExpUnit = new System.Windows.Forms.NumericUpDown();
            this.GroupExp = new System.Windows.Forms.GroupBox();
            this.LevelView = new System.Windows.Forms.DataGridView();
            this.ColumnLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnExp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSortieCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnASW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnEquipment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ASWModernization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpUnit)).BeginInit();
            this.GroupExp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LevelView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ExpControl);
            this.groupBox1.Controls.Add(this.Rank_List);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.MVP_Check);
            this.groupBox1.Controls.Add(this.FlagShip_Check);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.MapSelect);
            this.groupBox1.Controls.Add(this.TextShip);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.SearchInFleet);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ShowAllASWEquipments);
            this.groupBox1.Controls.Add(this.ShowAllLevel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ASWModernization);
            this.groupBox1.Controls.Add(this.ExpUnit);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(599, 114);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "표시조건";
            // 
            // ExpControl
            // 
            this.ExpControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExpControl.AutoSize = true;
            this.ExpControl.Location = new System.Drawing.Point(470, 60);
            this.ExpControl.Name = "ExpControl";
            this.ExpControl.Size = new System.Drawing.Size(123, 19);
            this.ExpControl.TabIndex = 15;
            this.ExpControl.Text = "출격Exp 수동설정";
            this.ToolTipInfo.SetToolTip(this.ExpControl, "출격 Exp를 수동으로 설정합니다.");
            this.ExpControl.UseVisualStyleBackColor = true;
            this.ExpControl.CheckedChanged += new System.EventHandler(this.ExpControl_CheckChanged);
            // 
            // Rank_List
            // 
            this.Rank_List.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Rank_List.BackColor = System.Drawing.Color.White;
            this.Rank_List.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Rank_List.FormattingEnabled = true;
            this.Rank_List.Items.AddRange(new object[] {
            "S",
            "A",
            "B",
            "C",
            "D",
            "E"});
            this.Rank_List.Location = new System.Drawing.Point(150, 86);
            this.Rank_List.MinimumSize = new System.Drawing.Size(75, 0);
            this.Rank_List.Name = "Rank_List";
            this.Rank_List.Size = new System.Drawing.Size(75, 23);
            this.Rank_List.TabIndex = 14;
            this.ToolTipInfo.SetToolTip(this.Rank_List, "랭크를 설정합니다. 배율은 S = x1.2 / A,B = x1.0 / C = x0.8 / D = x0.6 / E = x0.5 입니다.");
            this.Rank_List.SelectedIndexChanged += new System.EventHandler(this.Rank_IndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(113, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 15);
            this.label5.TabIndex = 13;
            this.label5.Text = "랭크";
            // 
            // MVP_Check
            // 
            this.MVP_Check.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MVP_Check.AutoSize = true;
            this.MVP_Check.Location = new System.Drawing.Point(287, 88);
            this.MVP_Check.Name = "MVP_Check";
            this.MVP_Check.Size = new System.Drawing.Size(51, 19);
            this.MVP_Check.TabIndex = 12;
            this.MVP_Check.Text = "MVP";
            this.ToolTipInfo.SetToolTip(this.MVP_Check, "MVP 여부를 체크합니다. 현재 설정된 경험치 값이 2배로 조정됩니다.");
            this.MVP_Check.UseVisualStyleBackColor = true;
            this.MVP_Check.CheckedChanged += new System.EventHandler(this.MVP_CheckChanged);
            // 
            // FlagShip_Check
            // 
            this.FlagShip_Check.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FlagShip_Check.AutoSize = true;
            this.FlagShip_Check.Location = new System.Drawing.Point(231, 88);
            this.FlagShip_Check.Name = "FlagShip_Check";
            this.FlagShip_Check.Size = new System.Drawing.Size(50, 19);
            this.FlagShip_Check.TabIndex = 11;
            this.FlagShip_Check.Text = "기함";
            this.ToolTipInfo.SetToolTip(this.FlagShip_Check, "기함 여부를 체크합니다. 현재 설정된 경험치 값이 1.5배로 조정됩니다.");
            this.FlagShip_Check.UseVisualStyleBackColor = true;
            this.FlagShip_Check.CheckedChanged += new System.EventHandler(this.FlagShip_CheckChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(113, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "해역";
            // 
            // MapSelect
            // 
            this.MapSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapSelect.BackColor = System.Drawing.Color.White;
            this.MapSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MapSelect.FormattingEnabled = true;
            this.MapSelect.Items.AddRange(new object[] {
            "1-1",
            "1-2",
            "1-3",
            "1-4",
            "1-5",
            "2-1",
            "2-2",
            "2-3",
            "2-4",
            "2-5",
            "3-1",
            "3-2",
            "3-3",
            "3-4",
            "3-5",
            "4-1",
            "4-2",
            "4-3",
            "4-4",
            "4-5",
            "5-1",
            "5-2",
            "5-3",
            "5-4",
            "5-5",
            "6-1",
            "6-2"});
            this.MapSelect.Location = new System.Drawing.Point(150, 57);
            this.MapSelect.MinimumSize = new System.Drawing.Size(150, 0);
            this.MapSelect.Name = "MapSelect";
            this.MapSelect.Size = new System.Drawing.Size(150, 23);
            this.MapSelect.TabIndex = 9;
            this.MapSelect.SelectedIndexChanged += new System.EventHandler(this.MapSelect_IndexChanged);
            // 
            // TextShip
            // 
            this.TextShip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextShip.BackColor = System.Drawing.Color.White;
            this.TextShip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TextShip.FormattingEnabled = true;
            this.TextShip.Location = new System.Drawing.Point(55, 22);
            this.TextShip.MinimumSize = new System.Drawing.Size(150, 0);
            this.TextShip.Name = "TextShip";
            this.TextShip.Size = new System.Drawing.Size(245, 23);
            this.TextShip.TabIndex = 1;
            this.TextShip.SelectedIndexChanged += new System.EventHandler(this.TextShip_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "대상함";
            // 
            // SearchInFleet
            // 
            this.SearchInFleet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchInFleet.AutoSize = true;
            this.SearchInFleet.Location = new System.Drawing.Point(322, 24);
            this.SearchInFleet.Name = "SearchInFleet";
            this.SearchInFleet.Size = new System.Drawing.Size(74, 19);
            this.SearchInFleet.TabIndex = 2;
            this.SearchInFleet.Text = "함대에서";
            this.ToolTipInfo.SetToolTip(this.SearchInFleet, "현대 함대에 소속된 함선만 선택할 수 있습니다.");
            this.SearchInFleet.UseVisualStyleBackColor = true;
            this.SearchInFleet.CheckedChanged += new System.EventHandler(this.SearchInFleet_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(406, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "출격당 획득 Exp:";
            // 
            // ShowAllASWEquipments
            // 
            this.ShowAllASWEquipments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowAllASWEquipments.AutoSize = true;
            this.ShowAllASWEquipments.Location = new System.Drawing.Point(471, 24);
            this.ShowAllASWEquipments.Name = "ShowAllASWEquipments";
            this.ShowAllASWEquipments.Size = new System.Drawing.Size(122, 19);
            this.ShowAllASWEquipments.TabIndex = 4;
            this.ShowAllASWEquipments.Text = "대잠장비전체보기";
            this.ToolTipInfo.SetToolTip(this.ShowAllASWEquipments, "모든 소나/폭뢰의 조합을 표시합니다.");
            this.ShowAllASWEquipments.UseVisualStyleBackColor = true;
            this.ShowAllASWEquipments.CheckedChanged += new System.EventHandler(this.ShowAllASWEquipments_CheckedChanged);
            // 
            // ShowAllLevel
            // 
            this.ShowAllLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowAllLevel.AutoSize = true;
            this.ShowAllLevel.Location = new System.Drawing.Point(403, 24);
            this.ShowAllLevel.Name = "ShowAllLevel";
            this.ShowAllLevel.Size = new System.Drawing.Size(74, 19);
            this.ShowAllLevel.TabIndex = 3;
            this.ShowAllLevel.Text = "전체표시";
            this.ToolTipInfo.SetToolTip(this.ShowAllLevel, "Lv. 1부터 표시합니다.\r\n값이 알맞지 않은 경우 다음 레벨부터 표시합니다.");
            this.ShowAllLevel.UseVisualStyleBackColor = true;
            this.ShowAllLevel.CheckedChanged += new System.EventHandler(this.ShowAllLevel_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "대잠개수";
            // 
            // ASWModernization
            // 
            this.ASWModernization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ASWModernization.Location = new System.Drawing.Point(67, 56);
            this.ASWModernization.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.ASWModernization.Name = "ASWModernization";
            this.ASWModernization.Size = new System.Drawing.Size(40, 23);
            this.ASWModernization.TabIndex = 6;
            this.ASWModernization.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ToolTipInfo.SetToolTip(this.ASWModernization, "대잠개수값을 변경합니다.");
            this.ASWModernization.ValueChanged += new System.EventHandler(this.ASWModernization_ValueChanged);
            // 
            // ExpUnit
            // 
            this.ExpUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExpUnit.Location = new System.Drawing.Point(513, 85);
            this.ExpUnit.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ExpUnit.Name = "ExpUnit";
            this.ExpUnit.Size = new System.Drawing.Size(80, 23);
            this.ExpUnit.TabIndex = 8;
            this.ExpUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ToolTipInfo.SetToolTip(this.ExpUnit, "1회당 출격 Exp를 설정합니다.");
            this.ExpUnit.Value = new decimal(new int[] {
            2268,
            0,
            0,
            0});
            this.ExpUnit.ValueChanged += new System.EventHandler(this.ExpUnit_ValueChanged);
            // 
            // GroupExp
            // 
            this.GroupExp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupExp.Controls.Add(this.LevelView);
            this.GroupExp.Location = new System.Drawing.Point(12, 133);
            this.GroupExp.Name = "GroupExp";
            this.GroupExp.Size = new System.Drawing.Size(600, 325);
            this.GroupExp.TabIndex = 1;
            this.GroupExp.TabStop = false;
            this.GroupExp.Text = "필요Exp";
            // 
            // LevelView
            // 
            this.LevelView.AllowUserToAddRows = false;
            this.LevelView.AllowUserToDeleteRows = false;
            this.LevelView.AllowUserToResizeRows = false;
            this.LevelView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LevelView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.LevelView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnLevel,
            this.ColumnExp,
            this.ColumnSortieCount,
            this.ColumnASW,
            this.ColumnEquipment});
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.LevelView.DefaultCellStyle = dataGridViewCellStyle15;
            this.LevelView.Location = new System.Drawing.Point(7, 22);
            this.LevelView.Name = "LevelView";
            this.LevelView.ReadOnly = true;
            this.LevelView.RowHeadersVisible = false;
            this.LevelView.RowTemplate.Height = 21;
            this.LevelView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.LevelView.Size = new System.Drawing.Size(587, 303);
            this.LevelView.TabIndex = 0;
            // 
            // ColumnLevel
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColumnLevel.DefaultCellStyle = dataGridViewCellStyle11;
            this.ColumnLevel.HeaderText = "Lv";
            this.ColumnLevel.Name = "ColumnLevel";
            this.ColumnLevel.ReadOnly = true;
            this.ColumnLevel.Width = 40;
            // 
            // ColumnExp
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColumnExp.DefaultCellStyle = dataGridViewCellStyle12;
            this.ColumnExp.HeaderText = "필요Exp";
            this.ColumnExp.Name = "ColumnExp";
            this.ColumnExp.ReadOnly = true;
            this.ColumnExp.Width = 72;
            // 
            // ColumnSortieCount
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColumnSortieCount.DefaultCellStyle = dataGridViewCellStyle13;
            this.ColumnSortieCount.HeaderText = "출격회수";
            this.ColumnSortieCount.Name = "ColumnSortieCount";
            this.ColumnSortieCount.ReadOnly = true;
            this.ColumnSortieCount.Width = 72;
            // 
            // ColumnASW
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColumnASW.DefaultCellStyle = dataGridViewCellStyle14;
            this.ColumnASW.HeaderText = "대잠";
            this.ColumnASW.Name = "ColumnASW";
            this.ColumnASW.ReadOnly = true;
            this.ColumnASW.Width = 40;
            // 
            // ColumnEquipment
            // 
            this.ColumnEquipment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnEquipment.HeaderText = "선제대잠장비";
            this.ColumnEquipment.Name = "ColumnEquipment";
            this.ColumnEquipment.ReadOnly = true;
            this.ColumnEquipment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ToolTipInfo
            // 
            this.ToolTipInfo.AutoPopDelay = 30000;
            this.ToolTipInfo.InitialDelay = 500;
            this.ToolTipInfo.ReshowDelay = 100;
            this.ToolTipInfo.ShowAlways = true;
            // 
            // DialogExpChecker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(624, 470);
            this.Controls.Add(this.GroupExp);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Name = "DialogExpChecker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "경험치 계산기";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogExpChecker_FormClosed);
            this.Load += new System.EventHandler(this.DialogExpChecker_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ASWModernization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpUnit)).EndInit();
            this.GroupExp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LevelView)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox TextShip;
		private System.Windows.Forms.GroupBox GroupExp;
		private System.Windows.Forms.DataGridView LevelView;
		private System.Windows.Forms.NumericUpDown ExpUnit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown ASWModernization;
		private System.Windows.Forms.CheckBox ShowAllASWEquipments;
		private System.Windows.Forms.CheckBox ShowAllLevel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox SearchInFleet;
		private System.Windows.Forms.ToolTip ToolTipInfo;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLevel;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnExp;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSortieCount;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnASW;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnEquipment;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox MapSelect;
        private System.Windows.Forms.ComboBox Rank_List;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox MVP_Check;
        private System.Windows.Forms.CheckBox FlagShip_Check;
        private System.Windows.Forms.CheckBox ExpControl;
    }
}