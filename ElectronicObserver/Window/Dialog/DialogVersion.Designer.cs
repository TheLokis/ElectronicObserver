namespace ElectronicObserver.Window.Dialog
{
	partial class DialogVersion
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
            this.TextVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TextAuthor = new System.Windows.Forms.LinkLabel();
            this.TextTranslator = new System.Windows.Forms.LinkLabel();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.TextInformation = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.Translator = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TranslatorLink = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.GallLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // TextVersion
            // 
            this.TextVersion.AutoSize = true;
            this.TextVersion.Location = new System.Drawing.Point(12, 9);
            this.TextVersion.Margin = new System.Windows.Forms.Padding(3);
            this.TextVersion.Name = "TextVersion";
            this.TextVersion.Size = new System.Drawing.Size(208, 15);
            this.TextVersion.TabIndex = 1;
            this.TextVersion.Text = "試製七四式電子観測儀零型 (ver. 0.0)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "개발：";
            // 
            // TextAuthor
            // 
            this.TextAuthor.AutoSize = true;
            this.TextAuthor.Location = new System.Drawing.Point(61, 30);
            this.TextAuthor.Name = "TextAuthor";
            this.TextAuthor.Size = new System.Drawing.Size(55, 15);
            this.TextAuthor.TabIndex = 3;
            this.TextAuthor.TabStop = true;
            this.TextAuthor.Text = "Andante";
            this.TextAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TextAuthor_LinkClicked);
            // 
            // TextTranslator
            // 
            this.TextTranslator.AutoSize = true;
            this.TextTranslator.Location = new System.Drawing.Point(121, 60);
            this.TextTranslator.Name = "TextTranslator";
            this.TextTranslator.Size = new System.Drawing.Size(55, 15);
            this.TextTranslator.TabIndex = 3;
            this.TextTranslator.TabStop = true;
            this.TextTranslator.Text = "Andante";
            this.TextTranslator.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TextAuthor_LinkClicked);
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClose.Location = new System.Drawing.Point(377, 151);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 0;
            this.ButtonClose.Text = "닫기";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // TextInformation
            // 
            this.TextInformation.AutoSize = true;
            this.TextInformation.Location = new System.Drawing.Point(122, 51);
            this.TextInformation.Name = "TextInformation";
            this.TextInformation.Size = new System.Drawing.Size(235, 15);
            this.TextInformation.TabIndex = 5;
            this.TextInformation.TabStop = true;
            this.TextInformation.Text = "http://electronicobserver.blog.fc2.com/";
            this.TextInformation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TextInformation_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(122, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "일어판 페이지：";
            // 
            // Translator
            // 
            this.Translator.AutoSize = true;
            this.Translator.Location = new System.Drawing.Point(63, 78);
            this.Translator.Name = "Translator";
            this.Translator.Size = new System.Drawing.Size(53, 15);
            this.Translator.TabIndex = 6;
            this.Translator.TabStop = true;
            this.Translator.Text = "TheLoki";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "번역：";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(122, 78);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(309, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "번역자 페이지：(버그, 번역지원 등 이슈 사항은 여기에)";
            // 
            // TranslatorLink
            // 
            this.TranslatorLink.AutoSize = true;
            this.TranslatorLink.Location = new System.Drawing.Point(122, 99);
            this.TranslatorLink.Name = "TranslatorLink";
            this.TranslatorLink.Size = new System.Drawing.Size(167, 15);
            this.TranslatorLink.TabIndex = 9;
            this.TranslatorLink.TabStop = true;
            this.TranslatorLink.Text = "http://thelokis.egloos.com/";
            this.TranslatorLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TextTranslator_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 127);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "참조：DC인사이드 칸코레 갤러리";
            // 
            // GallLink
            // 
            this.GallLink.AutoSize = true;
            this.GallLink.Location = new System.Drawing.Point(12, 151);
            this.GallLink.Name = "GallLink";
            this.GallLink.Size = new System.Drawing.Size(292, 15);
            this.GallLink.TabIndex = 11;
            this.GallLink.TabStop = true;
            this.GallLink.Text = "http://gall.dcinside.com/board/lists/?id=kancolle";
            // 
            // DialogVersion
            // 
            this.AcceptButton = this.ButtonClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(464, 186);
            this.Controls.Add(this.GallLink);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TranslatorLink);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Translator);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextInformation);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.TextAuthor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextVersion);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogVersion";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "버전";
            this.Load += new System.EventHandler(this.DialogVersion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label TextVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel TextAuthor;
        private System.Windows.Forms.LinkLabel TextTranslator;
        private System.Windows.Forms.Button ButtonClose;
		private System.Windows.Forms.LinkLabel TextInformation;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel Translator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel TranslatorLink;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel GallLink;
    }
}