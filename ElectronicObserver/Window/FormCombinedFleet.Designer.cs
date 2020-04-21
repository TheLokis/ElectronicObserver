namespace ElectronicObserver.Window
{
	partial class FormCombinedFleet
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.TableMember = new System.Windows.Forms.TableLayoutPanel();
            this.TableFleet = new System.Windows.Forms.TableLayoutPanel();
            this.ContextMenuFleet = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ContextMenuFleet_OutputFleetImage = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuFleet_CopyToFleetAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.ContextMenuFleet.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableMember
            // 
            this.TableMember.AutoSize = true;
            this.TableMember.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableMember.ColumnCount = 8;
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableMember.Location = new System.Drawing.Point(0, 24);
            this.TableMember.Name = "TableMember";
            this.TableMember.RowCount = 1;
            this.TableMember.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.TableMember.Size = new System.Drawing.Size(0, 21);
            this.TableMember.TabIndex = 1;
            this.TableMember.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.TableMember_CellPaint);
            this.TableMember.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            // 
            // TableFleet
            // 
            this.TableFleet.AutoSize = true;
            this.TableFleet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableFleet.ColumnCount = 8;
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableFleet.ContextMenuStrip = this.ContextMenuFleet;
            this.TableFleet.Location = new System.Drawing.Point(0, 0);
            this.TableFleet.Name = "TableFleet";
            this.TableFleet.RowCount = 1;
            this.TableFleet.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.TableFleet.Size = new System.Drawing.Size(0, 21);
            this.TableFleet.TabIndex = 7;
            // 
            // ContextMenuFleet
            // 
            this.ContextMenuFleet.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ContextMenuFleet.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.ContextMenuFleet_OutputFleetImage,
            this.ContextMenuFleet_CopyToFleetAnalysis});
            this.ContextMenuFleet.Name = "ContextMenuFleet";
            this.ContextMenuFleet.Size = new System.Drawing.Size(249, 164);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(245, 6);
            // 
            // ContextMenuFleet_OutputFleetImage
            // 
            this.ContextMenuFleet_OutputFleetImage.Name = "ContextMenuFleet_OutputFleetImage";
            this.ContextMenuFleet_OutputFleetImage.Size = new System.Drawing.Size(248, 22);
            this.ContextMenuFleet_OutputFleetImage.Text = "편성 이미지 출력(&I)";
            this.ContextMenuFleet_OutputFleetImage.Click += new System.EventHandler(this.ContextMenuFleet_OutputFleetImage_Click);
            // 
            // ContextMenuFleet_CopyToFleetAnalysis
            // 
            this.ContextMenuFleet_CopyToFleetAnalysis.Name = "ContextMenuFleet_CopyToFleetAnalysis";
            this.ContextMenuFleet_CopyToFleetAnalysis.Size = new System.Drawing.Size(248, 22);
            this.ContextMenuFleet_CopyToFleetAnalysis.Text = "함대 분석 페이지용 코드 복사(&F)";
            this.ContextMenuFleet_CopyToFleetAnalysis.Click += new System.EventHandler(this.ContextMenuFleet_CopyToFleetAnalysis_Click);
            // 
            // ToolTipInfo
            // 
            this.ToolTipInfo.AutoPopDelay = 30000;
            this.ToolTipInfo.InitialDelay = 500;
            this.ToolTipInfo.ReshowDelay = 100;
            this.ToolTipInfo.ShowAlways = true;
            // 
            // FormCombinedFleet
            // 
            this.AutoHidePortion = 150D;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(557, 200);
            this.Controls.Add(this.TableFleet);
            this.Controls.Add(this.TableMember);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormCombinedFleet";
            this.Text = "*not loaded*";
            this.Load += new System.EventHandler(this.FormCombinedFleet_Load);
            this.ContextMenuFleet.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel TableMember;
		private System.Windows.Forms.TableLayoutPanel TableFleet;
		private System.Windows.Forms.ToolTip ToolTipInfo;
		private System.Windows.Forms.ContextMenuStrip ContextMenuFleet;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ContextMenuFleet_OutputFleetImage;
        private System.Windows.Forms.ToolStripMenuItem ContextMenuFleet_CopyToFleetAnalysis;
    }
}