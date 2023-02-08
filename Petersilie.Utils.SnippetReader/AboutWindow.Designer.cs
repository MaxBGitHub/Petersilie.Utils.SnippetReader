namespace Petersilie.Utils.SnippetReader
{
    partial class AboutWindow
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
            this.pb_Icon = new System.Windows.Forms.PictureBox();
            this.lbl_AboutIconCreator = new System.Windows.Forms.Label();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.lbl_Date = new System.Windows.Forms.Label();
            this.lbl_Link1 = new System.Windows.Forms.LinkLabel();
            this.lbl_Link2 = new System.Windows.Forms.LinkLabel();
            this.lbl_Link3 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_Icon
            // 
            this.pb_Icon.Dock = System.Windows.Forms.DockStyle.Right;
            this.pb_Icon.Location = new System.Drawing.Point(349, 0);
            this.pb_Icon.MaximumSize = new System.Drawing.Size(64, 64);
            this.pb_Icon.MinimumSize = new System.Drawing.Size(64, 64);
            this.pb_Icon.Name = "pb_Icon";
            this.pb_Icon.Size = new System.Drawing.Size(64, 64);
            this.pb_Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Icon.TabIndex = 4;
            this.pb_Icon.TabStop = false;
            // 
            // lbl_AboutIconCreator
            // 
            this.lbl_AboutIconCreator.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_AboutIconCreator.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_AboutIconCreator.Location = new System.Drawing.Point(0, 0);
            this.lbl_AboutIconCreator.Name = "lbl_AboutIconCreator";
            this.lbl_AboutIconCreator.Size = new System.Drawing.Size(349, 24);
            this.lbl_AboutIconCreator.TabIndex = 5;
            this.lbl_AboutIconCreator.Text = "Icon creator:";
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Name.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.Location = new System.Drawing.Point(0, 24);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(23, 17);
            this.lbl_Name.TabIndex = 9;
            this.lbl_Name.Text = "{0}";
            // 
            // lbl_Date
            // 
            this.lbl_Date.AutoSize = true;
            this.lbl_Date.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbl_Date.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Date.Location = new System.Drawing.Point(0, 143);
            this.lbl_Date.Name = "lbl_Date";
            this.lbl_Date.Size = new System.Drawing.Size(49, 13);
            this.lbl_Date.TabIndex = 13;
            this.lbl_Date.Text = "Date: {0}";
            // 
            // lbl_Link1
            // 
            this.lbl_Link1.AutoSize = true;
            this.lbl_Link1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Link1.Location = new System.Drawing.Point(0, 41);
            this.lbl_Link1.MaximumSize = new System.Drawing.Size(280, 17);
            this.lbl_Link1.Name = "lbl_Link1";
            this.lbl_Link1.Size = new System.Drawing.Size(65, 17);
            this.lbl_Link1.TabIndex = 15;
            this.lbl_Link1.TabStop = true;
            this.lbl_Link1.Text = "linkLabel1";
            this.lbl_Link1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Click);
            // 
            // lbl_Link2
            // 
            this.lbl_Link2.AutoSize = true;
            this.lbl_Link2.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Link2.Location = new System.Drawing.Point(0, 58);
            this.lbl_Link2.MaximumSize = new System.Drawing.Size(280, 17);
            this.lbl_Link2.Name = "lbl_Link2";
            this.lbl_Link2.Size = new System.Drawing.Size(65, 17);
            this.lbl_Link2.TabIndex = 16;
            this.lbl_Link2.TabStop = true;
            this.lbl_Link2.Text = "linkLabel2";
            this.lbl_Link2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Click);
            // 
            // lbl_Link3
            // 
            this.lbl_Link3.AutoSize = true;
            this.lbl_Link3.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Link3.Location = new System.Drawing.Point(0, 75);
            this.lbl_Link3.MaximumSize = new System.Drawing.Size(280, 17);
            this.lbl_Link3.Name = "lbl_Link3";
            this.lbl_Link3.Size = new System.Drawing.Size(65, 17);
            this.lbl_Link3.TabIndex = 17;
            this.lbl_Link3.TabStop = true;
            this.lbl_Link3.Text = "linkLabel3";
            this.lbl_Link3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Click);
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(413, 156);
            this.Controls.Add(this.lbl_Link3);
            this.Controls.Add(this.lbl_Link2);
            this.Controls.Add(this.lbl_Link1);
            this.Controls.Add(this.lbl_Date);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.lbl_AboutIconCreator);
            this.Controls.Add(this.pb_Icon);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AboutWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About SnippetReader";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pb_Icon;
        private System.Windows.Forms.Label lbl_AboutIconCreator;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Label lbl_Date;
        private System.Windows.Forms.LinkLabel lbl_Link1;
        private System.Windows.Forms.LinkLabel lbl_Link2;
        private System.Windows.Forms.LinkLabel lbl_Link3;
    }
}