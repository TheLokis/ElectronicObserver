using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	partial class DialogAkashilist
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ToolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.LevelTimer = new System.Windows.Forms.Timer(this.components);
            this.SaveCSVDialog = new System.Windows.Forms.SaveFileDialog();
            this.ImageLoader = new System.ComponentModel.BackgroundWorker();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.CanMaterial_CheckBox = new System.Windows.Forms.CheckBox();
            this.Btn_Day6 = new System.Windows.Forms.Button();
            this.Btn_Day5 = new System.Windows.Forms.Button();
            this.Btn_Day4 = new System.Windows.Forms.Button();
            this.Btn_Day3 = new System.Windows.Forms.Button();
            this.Btn_Day2 = new System.Windows.Forms.Button();
            this.Btn_Day1 = new System.Windows.Forms.Button();
            this.Btn_Day0 = new System.Windows.Forms.Button();
            this.Can_KaisuCheck = new System.Windows.Forms.CheckBox();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.TextSearch = new System.Windows.Forms.TextBox();
            this.AkashiListView = new System.Windows.Forms.DataGridView();
            this.EqID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EqType = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Resource_Fuel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Resource_Ammo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Resource_Steel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Resource_Baux = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Kit_before5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Material_before5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Kit_after6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Material_After6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Change_Kit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Change_Equip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AkashiListView)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolTipInfo
            // 
            this.ToolTipInfo.AutoPopDelay = 30000;
            this.ToolTipInfo.InitialDelay = 500;
            this.ToolTipInfo.ReshowDelay = 100;
            this.ToolTipInfo.ShowAlways = true;
            // 
            // SaveCSVDialog
            // 
            this.SaveCSVDialog.Filter = "CSV|*.csv|File|*";
            this.SaveCSVDialog.Title = "CSV에 출력";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(20);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(25, 561);
            this.flowLayoutPanel2.TabIndex = 12;
            this.flowLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel2_Paint);
            // 
            // splitContainer1
            // 
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(269, 173);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 561);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 12;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.CanMaterial_CheckBox);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day6);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day5);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day4);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day3);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day2);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day1);
            this.splitContainer2.Panel1.Controls.Add(this.Btn_Day0);
            this.splitContainer2.Panel1.Controls.Add(this.Can_KaisuCheck);
            this.splitContainer2.Panel1.Controls.Add(this.SearchLabel);
            this.splitContainer2.Panel1.Controls.Add(this.TextSearch);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.AkashiListView);
            this.splitContainer2.Size = new System.Drawing.Size(1184, 561);
            this.splitContainer2.SplitterDistance = 135;
            this.splitContainer2.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(815, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "* 자재(변환)에 마우스 커서를 올리면 변환 후 장비가 나옵니다.";
            // 
            // CanMaterial_CheckBox
            // 
            this.CanMaterial_CheckBox.AutoSize = true;
            this.CanMaterial_CheckBox.Location = new System.Drawing.Point(578, 107);
            this.CanMaterial_CheckBox.Name = "CanMaterial_CheckBox";
            this.CanMaterial_CheckBox.Size = new System.Drawing.Size(199, 19);
            this.CanMaterial_CheckBox.TabIndex = 10;
            this.CanMaterial_CheckBox.Text = "장착/잠금된 장비를 재료로 취급";
            this.CanMaterial_CheckBox.UseVisualStyleBackColor = true;
            this.CanMaterial_CheckBox.CheckedChanged += new System.EventHandler(this.Material_Check_Click);
            // 
            // Btn_Day6
            // 
            this.Btn_Day6.BackColor = System.Drawing.SystemColors.Control;
            this.Btn_Day6.Location = new System.Drawing.Point(850, 12);
            this.Btn_Day6.Margin = new System.Windows.Forms.Padding(12, 3, 3, 40);
            this.Btn_Day6.Name = "Btn_Day6";
            this.Btn_Day6.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day6.TabIndex = 9;
            this.Btn_Day6.Text = "토";
            this.Btn_Day6.UseVisualStyleBackColor = false;
            this.Btn_Day6.Click += new System.EventHandler(this.Btn_Day6_Click);
            // 
            // Btn_Day5
            // 
            this.Btn_Day5.Location = new System.Drawing.Point(750, 12);
            this.Btn_Day5.Margin = new System.Windows.Forms.Padding(12, 3, 3, 20);
            this.Btn_Day5.Name = "Btn_Day5";
            this.Btn_Day5.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day5.TabIndex = 8;
            this.Btn_Day5.Text = "금";
            this.Btn_Day5.UseVisualStyleBackColor = true;
            this.Btn_Day5.Click += new System.EventHandler(this.Btn_Day5_Click);
            // 
            // Btn_Day4
            // 
            this.Btn_Day4.Location = new System.Drawing.Point(650, 12);
            this.Btn_Day4.Margin = new System.Windows.Forms.Padding(12, 3, 3, 20);
            this.Btn_Day4.Name = "Btn_Day4";
            this.Btn_Day4.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day4.TabIndex = 7;
            this.Btn_Day4.Text = "목";
            this.Btn_Day4.UseVisualStyleBackColor = true;
            this.Btn_Day4.Click += new System.EventHandler(this.Btn_Day4_Click);
            // 
            // Btn_Day3
            // 
            this.Btn_Day3.Location = new System.Drawing.Point(550, 12);
            this.Btn_Day3.Margin = new System.Windows.Forms.Padding(12, 3, 3, 20);
            this.Btn_Day3.Name = "Btn_Day3";
            this.Btn_Day3.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day3.TabIndex = 6;
            this.Btn_Day3.Text = "수";
            this.Btn_Day3.UseVisualStyleBackColor = true;
            this.Btn_Day3.Click += new System.EventHandler(this.Btn_Day3_Click);
            // 
            // Btn_Day2
            // 
            this.Btn_Day2.Location = new System.Drawing.Point(450, 12);
            this.Btn_Day2.Margin = new System.Windows.Forms.Padding(12, 3, 3, 20);
            this.Btn_Day2.Name = "Btn_Day2";
            this.Btn_Day2.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day2.TabIndex = 5;
            this.Btn_Day2.Text = "화";
            this.Btn_Day2.UseVisualStyleBackColor = true;
            this.Btn_Day2.Click += new System.EventHandler(this.Btn_Day2_Click);
            // 
            // Btn_Day1
            // 
            this.Btn_Day1.Location = new System.Drawing.Point(350, 12);
            this.Btn_Day1.Margin = new System.Windows.Forms.Padding(12, 3, 3, 20);
            this.Btn_Day1.Name = "Btn_Day1";
            this.Btn_Day1.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day1.TabIndex = 4;
            this.Btn_Day1.Text = "월";
            this.Btn_Day1.UseVisualStyleBackColor = true;
            this.Btn_Day1.Click += new System.EventHandler(this.Btn_Day1_Click);
            // 
            // Btn_Day0
            // 
            this.Btn_Day0.Location = new System.Drawing.Point(250, 12);
            this.Btn_Day0.Margin = new System.Windows.Forms.Padding(12, 20, 3, 20);
            this.Btn_Day0.Name = "Btn_Day0";
            this.Btn_Day0.Size = new System.Drawing.Size(75, 50);
            this.Btn_Day0.TabIndex = 3;
            this.Btn_Day0.Text = "일";
            this.Btn_Day0.UseVisualStyleBackColor = true;
            this.Btn_Day0.Click += new System.EventHandler(this.Btn_Day0_Click);
            // 
            // Can_KaisuCheck
            // 
            this.Can_KaisuCheck.AutoSize = true;
            this.Can_KaisuCheck.Location = new System.Drawing.Point(374, 107);
            this.Can_KaisuCheck.Name = "Can_KaisuCheck";
            this.Can_KaisuCheck.Size = new System.Drawing.Size(154, 19);
            this.Can_KaisuCheck.TabIndex = 3;
            this.Can_KaisuCheck.Text = "개수가능한 장비만 표시";
            this.Can_KaisuCheck.UseVisualStyleBackColor = true;
            this.Can_KaisuCheck.CheckedChanged += new System.EventHandler(this.Can_KaisuCheck_Click);
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Location = new System.Drawing.Point(12, 107);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(95, 15);
            this.SearchLabel.TabIndex = 2;
            this.SearchLabel.Text = "장비명으로 검색";
            // 
            // TextSearch
            // 
            this.TextSearch.Location = new System.Drawing.Point(113, 102);
            this.TextSearch.MaximumSize = new System.Drawing.Size(208, 0);
            this.TextSearch.Name = "TextSearch";
            this.TextSearch.Size = new System.Drawing.Size(208, 23);
            this.TextSearch.TabIndex = 1;
            this.TextSearch.TextChanged += new System.EventHandler(this.TextSearch_TextChanged);
            this.TextSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextSearch_KeyDown);
            // 
            // AkashiListView
            // 
            this.AkashiListView.AllowUserToAddRows = false;
            this.AkashiListView.AllowUserToDeleteRows = false;
            this.AkashiListView.AllowUserToResizeRows = false;
            this.AkashiListView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.AkashiListView.ColumnHeadersHeight = 38;
            this.AkashiListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AkashiListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EqID,
            this.EqType,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Resource_Fuel,
            this.Resource_Ammo,
            this.Resource_Steel,
            this.Resource_Baux,
            this.Kit_before5,
            this.Material_before5,
            this.Kit_after6,
            this.Material_After6,
            this.Change_Kit,
            this.Change_Equip});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.AkashiListView.DefaultCellStyle = dataGridViewCellStyle2;
            this.AkashiListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AkashiListView.Location = new System.Drawing.Point(0, 0);
            this.AkashiListView.MultiSelect = false;
            this.AkashiListView.Name = "AkashiListView";
            this.AkashiListView.ReadOnly = true;
            this.AkashiListView.RowHeadersVisible = false;
            this.AkashiListView.RowTemplate.Height = 40;
            this.AkashiListView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AkashiListView.Size = new System.Drawing.Size(1184, 422);
            this.AkashiListView.TabIndex = 4;
            this.AkashiListView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AkashiListView_CellContentClick);
            this.AkashiListView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.EquipmentView_CellFormatting);
            this.AkashiListView.SelectionChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // EqID
            // 
            this.EqID.HeaderText = "ID";
            this.EqID.Name = "EqID";
            this.EqID.ReadOnly = true;
            this.EqID.Visible = false;
            this.EqID.Width = 50;
            // 
            // EqType
            // 
            this.EqType.HeaderText = "";
            this.EqType.MinimumWidth = 2;
            this.EqType.Name = "EqType";
            this.EqType.ReadOnly = true;
            this.EqType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.EqType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EqType.Width = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 200F;
            this.dataGridViewTextBoxColumn1.HeaderText = "장비명";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 215;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 200F;
            this.dataGridViewTextBoxColumn2.HeaderText = "비서함";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // Resource_Fuel
            // 
            this.Resource_Fuel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Resource_Fuel.HeaderText = "연";
            this.Resource_Fuel.Name = "Resource_Fuel";
            this.Resource_Fuel.ReadOnly = true;
            this.Resource_Fuel.Width = 40;
            // 
            // Resource_Ammo
            // 
            this.Resource_Ammo.HeaderText = "탄";
            this.Resource_Ammo.Name = "Resource_Ammo";
            this.Resource_Ammo.ReadOnly = true;
            this.Resource_Ammo.Width = 40;
            // 
            // Resource_Steel
            // 
            this.Resource_Steel.HeaderText = "강";
            this.Resource_Steel.Name = "Resource_Steel";
            this.Resource_Steel.ReadOnly = true;
            this.Resource_Steel.Width = 40;
            // 
            // Resource_Baux
            // 
            this.Resource_Baux.HeaderText = "보";
            this.Resource_Baux.Name = "Resource_Baux";
            this.Resource_Baux.ReadOnly = true;
            this.Resource_Baux.Width = 40;
            // 
            // Kit_before5
            // 
            this.Kit_before5.HeaderText = "자재(~5)";
            this.Kit_before5.Name = "Kit_before5";
            this.Kit_before5.ReadOnly = true;
            this.Kit_before5.ToolTipText = "왼쪽이 개발자재, 오른쪽은 개수자재입니다. \r\n괄호안 수치는 확정시 소모량입니다.";
            this.Kit_before5.Width = 90;
            // 
            // Material_before5
            // 
            this.Material_before5.HeaderText = "재료(~5)";
            this.Material_before5.Name = "Material_before5";
            this.Material_before5.ReadOnly = true;
            this.Material_before5.Width = 150;
            // 
            // Kit_after6
            // 
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Kit_after6.DefaultCellStyle = dataGridViewCellStyle1;
            this.Kit_after6.HeaderText = "자재(6~)";
            this.Kit_after6.Name = "Kit_after6";
            this.Kit_after6.ReadOnly = true;
            this.Kit_after6.ToolTipText = "왼쪽이 개발자재, 오른쪽은 개수자재입니다. \r\n괄호안 수치는 확정시 소모량입니다.";
            this.Kit_after6.Width = 90;
            // 
            // Material_After6
            // 
            this.Material_After6.HeaderText = "재료(6~)";
            this.Material_After6.Name = "Material_After6";
            this.Material_After6.ReadOnly = true;
            this.Material_After6.Width = 150;
            // 
            // Change_Kit
            // 
            this.Change_Kit.HeaderText = "자재(변환)";
            this.Change_Kit.Name = "Change_Kit";
            this.Change_Kit.ReadOnly = true;
            this.Change_Kit.Width = 90;
            // 
            // Change_Equip
            // 
            this.Change_Equip.HeaderText = "재료(변환)";
            this.Change_Equip.Name = "Change_Equip";
            this.Change_Equip.ReadOnly = true;
            this.Change_Equip.Width = 150;
            // 
            // DialogAkashilist
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MinimumSize = new System.Drawing.Size(1200, 600);
            this.Name = "DialogAkashilist";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "개수공창";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogAlbumMasterShip_FormClosed);
            this.Load += new System.EventHandler(this.DialogAlbumMasterShip_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AkashiListView)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ToolTip ToolTipInfo;
		private System.Windows.Forms.Timer LevelTimer;
		private System.Windows.Forms.SaveFileDialog SaveCSVDialog;
		private System.ComponentModel.BackgroundWorker ImageLoader;
        private FlowLayoutPanel flowLayoutPanel2;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private Button Btn_Day6;
        private Button Btn_Day5;
        private Button Btn_Day4;
        private Button Btn_Day3;
        private Button Btn_Day2;
        private Button Btn_Day1;
        private Button Btn_Day0;
        private CheckBox Can_KaisuCheck;
        private Label SearchLabel;
        private TextBox TextSearch;
        private DataGridView AkashiListView;
        private CheckBox CanMaterial_CheckBox;
        private Label label1;
        private DataGridViewTextBoxColumn EqID;
        private DataGridViewImageColumn EqType;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn Resource_Fuel;
        private DataGridViewTextBoxColumn Resource_Ammo;
        private DataGridViewTextBoxColumn Resource_Steel;
        private DataGridViewTextBoxColumn Resource_Baux;
        private DataGridViewTextBoxColumn Kit_before5;
        private DataGridViewTextBoxColumn Material_before5;
        private DataGridViewTextBoxColumn Kit_after6;
        private DataGridViewTextBoxColumn Material_After6;
        private DataGridViewTextBoxColumn Change_Kit;
        private DataGridViewTextBoxColumn Change_Equip;
    }
}