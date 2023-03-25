namespace lh5801_Dis
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label9 = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.tbDisStart = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.rbME0 = new System.Windows.Forms.RadioButton();
            this.rbME1 = new System.Windows.Forms.RadioButton();
            this.tbDisEnd = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbPC1500 = new System.Windows.Forms.CheckBox();
            this.cbUseLibFile = new System.Windows.Forms.CheckBox();
            this.cbUseLables = new System.Windows.Forms.CheckBox();
            this.cbListFormat = new System.Windows.Forms.CheckBox();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOutputFile = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(347, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 22;
            this.label9.Text = "Memory";
            // 
            // tbAddress
            // 
            this.tbAddress.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAddress.Location = new System.Drawing.Point(146, 356);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(41, 27);
            this.tbAddress.TabIndex = 24;
            this.tbAddress.Text = "8000";
            this.tbAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbAddress_KeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(95, 360);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 15);
            this.label10.TabIndex = 25;
            this.label10.Text = "Address";
            // 
            // tbValue
            // 
            this.tbValue.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbValue.Location = new System.Drawing.Point(31, 385);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(156, 27);
            this.tbValue.TabIndex = 26;
            this.tbValue.Text = "00";
            this.tbValue.TextChanged += new System.EventHandler(this.tbValue_TextChanged);
            this.tbValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbValue_KeyPress);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(28, 366);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 15);
            this.label11.TabIndex = 27;
            this.label11.Text = "Value";
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(56)))), ((int)(((byte)(26)))));
            this.btnReset.Location = new System.Drawing.Point(31, 517);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(65, 25);
            this.btnReset.TabIndex = 30;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tbDisStart
            // 
            this.tbDisStart.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDisStart.Location = new System.Drawing.Point(146, 69);
            this.tbDisStart.Name = "tbDisStart";
            this.tbDisStart.Size = new System.Drawing.Size(41, 27);
            this.tbDisStart.TabIndex = 53;
            this.tbDisStart.Text = "89F5";
            this.tbDisStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbDisStart.TextChanged += new System.EventHandler(this.tbRegHL_TextChanged);
            this.tbDisStart.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRegHL_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 20);
            this.label3.TabIndex = 48;
            this.label3.Text = "Disassembly Start";
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.Chartreuse;
            this.btnRun.Location = new System.Drawing.Point(106, 454);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 25);
            this.btnRun.TabIndex = 54;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStep
            // 
            this.btnStep.BackColor = System.Drawing.Color.YellowGreen;
            this.btnStep.Location = new System.Drawing.Point(31, 454);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(65, 25);
            this.btnStep.TabIndex = 56;
            this.btnStep.Text = "STEP";
            this.btnStep.UseVisualStyleBackColor = false;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnResetAll
            // 
            this.btnResetAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(56)))), ((int)(((byte)(26)))));
            this.btnResetAll.Location = new System.Drawing.Point(102, 517);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(80, 25);
            this.btnResetAll.TabIndex = 57;
            this.btnResetAll.Text = "RESET ALL";
            this.btnResetAll.UseVisualStyleBackColor = false;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Yellow;
            this.btnPause.Location = new System.Drawing.Point(31, 486);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(150, 25);
            this.btnPause.TabIndex = 58;
            this.btnPause.Text = "PAUSE";
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // rbME0
            // 
            this.rbME0.AutoSize = true;
            this.rbME0.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbME0.Checked = true;
            this.rbME0.Location = new System.Drawing.Point(84, 295);
            this.rbME0.Name = "rbME0";
            this.rbME0.Size = new System.Drawing.Size(84, 19);
            this.rbME0.TabIndex = 59;
            this.rbME0.TabStop = true;
            this.rbME0.Text = "RAM ME0";
            this.rbME0.UseVisualStyleBackColor = true;
            this.rbME0.CheckedChanged += new System.EventHandler(this.rbME0_CheckedChanged);
            // 
            // rbME1
            // 
            this.rbME1.AutoSize = true;
            this.rbME1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbME1.Location = new System.Drawing.Point(84, 318);
            this.rbME1.Name = "rbME1";
            this.rbME1.Size = new System.Drawing.Size(84, 19);
            this.rbME1.TabIndex = 60;
            this.rbME1.Text = "RAM ME1";
            this.rbME1.UseVisualStyleBackColor = true;
            this.rbME1.CheckedChanged += new System.EventHandler(this.rbME1_CheckedChanged);
            // 
            // tbDisEnd
            // 
            this.tbDisEnd.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDisEnd.Location = new System.Drawing.Point(145, 98);
            this.tbDisEnd.Name = "tbDisEnd";
            this.tbDisEnd.Size = new System.Drawing.Size(42, 27);
            this.tbDisEnd.TabIndex = 63;
            this.tbDisEnd.Text = "89FB";
            this.tbDisEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbDisEnd.TextChanged += new System.EventHandler(this.tbRegHL_TextChanged);
            this.tbDisEnd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRegHL_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(27, 101);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(144, 20);
            this.label14.TabIndex = 64;
            this.label14.Text = "Disassembly End";
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Interval = 150;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmLoad});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 28);
            this.menuStrip1.TabIndex = 81;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmLoad
            // 
            this.tsmLoad.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.tsmLoad.Name = "tsmLoad";
            this.tsmLoad.Size = new System.Drawing.Size(46, 24);
            this.tsmLoad.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(125, 26);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(125, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbPC1500
            // 
            this.cbPC1500.AutoSize = true;
            this.cbPC1500.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbPC1500.Location = new System.Drawing.Point(61, 148);
            this.cbPC1500.Name = "cbPC1500";
            this.cbPC1500.Size = new System.Drawing.Size(112, 19);
            this.cbPC1500.TabIndex = 82;
            this.cbPC1500.Text = "PC-1500 Mode";
            this.cbPC1500.UseVisualStyleBackColor = true;
            this.cbPC1500.CheckedChanged += new System.EventHandler(this.cbPC1500_CheckedChanged);
            // 
            // cbUseLibFile
            // 
            this.cbUseLibFile.AutoSize = true;
            this.cbUseLibFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbUseLibFile.Location = new System.Drawing.Point(75, 171);
            this.cbUseLibFile.Name = "cbUseLibFile";
            this.cbUseLibFile.Size = new System.Drawing.Size(95, 19);
            this.cbUseLibFile.TabIndex = 83;
            this.cbUseLibFile.Text = "Use LIB File";
            this.cbUseLibFile.UseVisualStyleBackColor = true;
            this.cbUseLibFile.CheckedChanged += new System.EventHandler(this.cbUseLibFile_CheckedChanged);
            // 
            // cbUseLables
            // 
            this.cbUseLables.AutoSize = true;
            this.cbUseLables.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbUseLables.Location = new System.Drawing.Point(79, 194);
            this.cbUseLables.Name = "cbUseLables";
            this.cbUseLables.Size = new System.Drawing.Size(91, 19);
            this.cbUseLables.TabIndex = 84;
            this.cbUseLables.Text = "Use Lables";
            this.cbUseLables.UseVisualStyleBackColor = true;
            this.cbUseLables.CheckedChanged += new System.EventHandler(this.chUseLables_CheckedChanged);
            // 
            // cbListFormat
            // 
            this.cbListFormat.AutoSize = true;
            this.cbListFormat.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbListFormat.Location = new System.Drawing.Point(81, 217);
            this.cbListFormat.Name = "cbListFormat";
            this.cbListFormat.Size = new System.Drawing.Size(90, 19);
            this.cbListFormat.TabIndex = 85;
            this.cbListFormat.Text = "List Format";
            this.cbListFormat.UseVisualStyleBackColor = true;
            this.cbListFormat.CheckedChanged += new System.EventHandler(this.cbListFormat_CheckedChanged);
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(217, 478);
            this.tbStatus.Multiline = true;
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbStatus.Size = new System.Drawing.Size(631, 64);
            this.tbStatus.TabIndex = 86;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(214, 460);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 87;
            this.label1.Text = "Status";
            // 
            // cbOutputFile
            // 
            this.cbOutputFile.AutoSize = true;
            this.cbOutputFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbOutputFile.Location = new System.Drawing.Point(81, 240);
            this.cbOutputFile.Name = "cbOutputFile";
            this.cbOutputFile.Size = new System.Drawing.Size(88, 19);
            this.cbOutputFile.TabIndex = 88;
            this.cbOutputFile.Text = "Output File";
            this.cbOutputFile.UseVisualStyleBackColor = true;
            this.cbOutputFile.CheckedChanged += new System.EventHandler(this.cbOutputFile_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.cbOutputFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbStatus);
            this.Controls.Add(this.cbListFormat);
            this.Controls.Add(this.cbUseLables);
            this.Controls.Add(this.cbUseLibFile);
            this.Controls.Add(this.cbPC1500);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tbDisEnd);
            this.Controls.Add(this.rbME1);
            this.Controls.Add(this.rbME0);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnResetAll);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.tbDisStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbAddress);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "lh5801 Emu - Sharp lh5801 CPU Emulator";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox tbDisStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.RadioButton rbME0;
        private System.Windows.Forms.RadioButton rbME1;
        private System.Windows.Forms.TextBox tbDisEnd;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmLoad;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbPC1500;
        private System.Windows.Forms.CheckBox cbUseLibFile;
        private System.Windows.Forms.CheckBox cbUseLables;
        private System.Windows.Forms.CheckBox cbListFormat;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbOutputFile;
    }
}

