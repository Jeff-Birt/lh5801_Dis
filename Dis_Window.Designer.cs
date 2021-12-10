namespace lh5801_Emu
{
    partial class Dis_Window
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
            this.tbDump = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbDump
            // 
            this.tbDump.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDump.Location = new System.Drawing.Point(74, 45);
            this.tbDump.Margin = new System.Windows.Forms.Padding(2);
            this.tbDump.Multiline = true;
            this.tbDump.Name = "tbDump";
            this.tbDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbDump.Size = new System.Drawing.Size(791, 512);
            this.tbDump.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(66, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "  Adr.   Raw                        Disassembled";
            // 
            // Dis_Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 591);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDump);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Dis_Window";
            this.Text = "Dis_Window";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDump;
        private System.Windows.Forms.Label label1;
    }
}