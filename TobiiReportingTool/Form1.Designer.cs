namespace TobiiReportingTool
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
            this.saveThreshold_btn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.genReport_btn = new System.Windows.Forms.Button();
            this.path_textBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.threshold_trackBar = new System.Windows.Forms.TrackBar();
            this.deleteBtn_tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.threshold_upDown = new System.Windows.Forms.NumericUpDown();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.clearTextBox_btn1 = new System.Windows.Forms.Button();
            this.selectFolder_btn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_trackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_upDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // saveThreshold_btn
            // 
            this.saveThreshold_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveThreshold_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveThreshold_btn.Location = new System.Drawing.Point(14, 182);
            this.saveThreshold_btn.Name = "saveThreshold_btn";
            this.saveThreshold_btn.Size = new System.Drawing.Size(205, 40);
            this.saveThreshold_btn.TabIndex = 6;
            this.saveThreshold_btn.Text = "Save Threshold";
            this.saveThreshold_btn.UseVisualStyleBackColor = true;
            this.saveThreshold_btn.Click += new System.EventHandler(this.saveThreshold_btn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "%";
            // 
            // genReport_btn
            // 
            this.genReport_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.genReport_btn.Enabled = false;
            this.genReport_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.genReport_btn.Location = new System.Drawing.Point(14, 349);
            this.genReport_btn.Name = "genReport_btn";
            this.genReport_btn.Size = new System.Drawing.Size(204, 52);
            this.genReport_btn.TabIndex = 3;
            this.genReport_btn.Text = "Generate Report";
            this.genReport_btn.UseVisualStyleBackColor = true;
            this.genReport_btn.Click += new System.EventHandler(this.genReport_btn_Click);
            // 
            // path_textBox
            // 
            this.path_textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.path_textBox.Location = new System.Drawing.Point(14, 296);
            this.path_textBox.Name = "path_textBox";
            this.path_textBox.Size = new System.Drawing.Size(204, 20);
            this.path_textBox.TabIndex = 1;
            this.path_textBox.Enter += new System.EventHandler(this.path_textBox_Enter);
            this.path_textBox.Leave += new System.EventHandler(this.path_textBox_Leave);
            this.path_textBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.path_textBox_Enter);
            // 
            // threshold_trackBar
            // 
            this.threshold_trackBar.Location = new System.Drawing.Point(6, 134);
            this.threshold_trackBar.Maximum = 99;
            this.threshold_trackBar.Name = "threshold_trackBar";
            this.threshold_trackBar.Size = new System.Drawing.Size(166, 45);
            this.threshold_trackBar.TabIndex = 5;
            this.threshold_trackBar.TickFrequency = 5;
            this.threshold_trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.threshold_trackBar.Value = 70;
            this.threshold_trackBar.Scroll += new System.EventHandler(this.threshold_trackBar_Scroll);
            // 
            // threshold_upDown
            // 
            this.threshold_upDown.Location = new System.Drawing.Point(170, 144);
            this.threshold_upDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.threshold_upDown.Name = "threshold_upDown";
            this.threshold_upDown.Size = new System.Drawing.Size(40, 20);
            this.threshold_upDown.TabIndex = 7;
            this.threshold_upDown.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.threshold_upDown.ValueChanged += new System.EventHandler(this.threshold_upDown_ValueChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TobiiReportingTool.Properties.Resources.TobiiLogo;
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.InitialImage = global::TobiiReportingTool.Properties.Resources.TobiiLogo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(205, 87);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // clearTextBox_btn1
            // 
            this.clearTextBox_btn1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.clearTextBox_btn1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clearTextBox_btn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearTextBox_btn1.Image = global::TobiiReportingTool.Properties.Resources.icon_ClearTextBox;
            this.clearTextBox_btn1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearTextBox_btn1.Location = new System.Drawing.Point(190, 296);
            this.clearTextBox_btn1.Name = "clearTextBox_btn1";
            this.clearTextBox_btn1.Size = new System.Drawing.Size(28, 20);
            this.clearTextBox_btn1.TabIndex = 2;
            this.clearTextBox_btn1.UseVisualStyleBackColor = true;
            this.clearTextBox_btn1.Click += new System.EventHandler(this.clearTextBox_btn1_Click);
            // 
            // selectFolder_btn
            // 
            this.selectFolder_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectFolder_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectFolder_btn.Image = global::TobiiReportingTool.Properties.Resources.icon_mediafolder;
            this.selectFolder_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectFolder_btn.Location = new System.Drawing.Point(14, 253);
            this.selectFolder_btn.Name = "selectFolder_btn";
            this.selectFolder_btn.Size = new System.Drawing.Size(204, 37);
            this.selectFolder_btn.TabIndex = 0;
            this.selectFolder_btn.Text = "Select Data Resource Folder";
            this.selectFolder_btn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.selectFolder_btn.UseVisualStyleBackColor = true;
            this.selectFolder_btn.Click += new System.EventHandler(this.selectFolder_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Participant Validity Threshold";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Validation Threshold";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(231, 416);
            this.Controls.Add(this.threshold_upDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.saveThreshold_btn);
            this.Controls.Add(this.threshold_trackBar);
            this.Controls.Add(this.clearTextBox_btn1);
            this.Controls.Add(this.selectFolder_btn);
            this.Controls.Add(this.path_textBox);
            this.Controls.Add(this.genReport_btn);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Tobii Reporting Tool";
            ((System.ComponentModel.ISupportInitialize)(this.threshold_trackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold_upDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveThreshold_btn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button genReport_btn;
        private System.Windows.Forms.TextBox path_textBox;
        private System.Windows.Forms.Button selectFolder_btn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button clearTextBox_btn1;
        private System.Windows.Forms.TrackBar threshold_trackBar;
        private System.Windows.Forms.ToolTip deleteBtn_tooltip;
        private System.Windows.Forms.NumericUpDown threshold_upDown;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
    }
}

