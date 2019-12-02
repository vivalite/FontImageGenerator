namespace TextImageGenerator
{
	partial class Main
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
		private async void InitializeComponent()
		{
            this.checkedListBoxFonts = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSCTC3K = new System.Windows.Forms.RadioButton();
            this.radioButtonSC3K = new System.Windows.Forms.RadioButton();
            this.radioButtonGBK = new System.Windows.Forms.RadioButton();
            this.buttonStart = new System.Windows.Forms.Button();
            this.checkBoxSplitData = new System.Windows.Forms.CheckBox();
            this.textBoxSelectWords = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonLoadFont = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBoxFonts
            // 
            this.checkedListBoxFonts.CheckOnClick = true;
            this.checkedListBoxFonts.FormattingEnabled = true;
            this.checkedListBoxFonts.Location = new System.Drawing.Point(13, 13);
            this.checkedListBoxFonts.Name = "checkedListBoxFonts";
            this.checkedListBoxFonts.Size = new System.Drawing.Size(279, 589);
            this.checkedListBoxFonts.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonSCTC3K);
            this.groupBox1.Controls.Add(this.radioButtonSC3K);
            this.groupBox1.Controls.Add(this.radioButtonGBK);
            this.groupBox1.Location = new System.Drawing.Point(303, 418);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 98);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Generating Group";
            // 
            // radioButtonSCTC3K
            // 
            this.radioButtonSCTC3K.AutoSize = true;
            this.radioButtonSCTC3K.Location = new System.Drawing.Point(7, 66);
            this.radioButtonSCTC3K.Name = "radioButtonSCTC3K";
            this.radioButtonSCTC3K.Size = new System.Drawing.Size(114, 17);
            this.radioButtonSCTC3K.TabIndex = 2;
            this.radioButtonSCTC3K.Text = "ST + TC Top 3000";
            this.radioButtonSCTC3K.UseVisualStyleBackColor = true;
            // 
            // radioButtonSC3K
            // 
            this.radioButtonSC3K.AutoSize = true;
            this.radioButtonSC3K.Checked = true;
            this.radioButtonSC3K.Location = new System.Drawing.Point(7, 43);
            this.radioButtonSC3K.Name = "radioButtonSC3K";
            this.radioButtonSC3K.Size = new System.Drawing.Size(88, 17);
            this.radioButtonSC3K.TabIndex = 1;
            this.radioButtonSC3K.TabStop = true;
            this.radioButtonSC3K.Text = "SC Top 3000";
            this.radioButtonSC3K.UseVisualStyleBackColor = true;
            // 
            // radioButtonGBK
            // 
            this.radioButtonGBK.AutoSize = true;
            this.radioButtonGBK.Location = new System.Drawing.Point(7, 20);
            this.radioButtonGBK.Name = "radioButtonGBK";
            this.radioButtonGBK.Size = new System.Drawing.Size(66, 17);
            this.radioButtonGBK.TabIndex = 0;
            this.radioButtonGBK.Text = "Full GBK";
            this.radioButtonGBK.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(303, 554);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(140, 48);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // checkBoxSplitData
            // 
            this.checkBoxSplitData.AutoSize = true;
            this.checkBoxSplitData.Checked = true;
            this.checkBoxSplitData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSplitData.Location = new System.Drawing.Point(310, 522);
            this.checkBoxSplitData.Name = "checkBoxSplitData";
            this.checkBoxSplitData.Size = new System.Drawing.Size(72, 17);
            this.checkBoxSplitData.TabIndex = 3;
            this.checkBoxSplitData.Text = "Split Data";
            this.checkBoxSplitData.UseVisualStyleBackColor = true;
            // 
            // textBoxSelectWords
            // 
            this.textBoxSelectWords.Location = new System.Drawing.Point(386, 383);
            this.textBoxSelectWords.Name = "textBoxSelectWords";
            this.textBoxSelectWords.Size = new System.Drawing.Size(57, 20);
            this.textBoxSelectWords.TabIndex = 4;
            this.textBoxSelectWords.Text = "1000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(306, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Words:";
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(303, 13);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(140, 24);
            this.buttonSelectAll.TabIndex = 6;
            this.buttonSelectAll.Text = "Select All";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // buttonLoadFont
            // 
            this.buttonLoadFont.Location = new System.Drawing.Point(303, 330);
            this.buttonLoadFont.Name = "buttonLoadFont";
            this.buttonLoadFont.Size = new System.Drawing.Size(140, 27);
            this.buttonLoadFont.TabIndex = 7;
            this.buttonLoadFont.Text = "Load Fonts";
            this.buttonLoadFont.UseVisualStyleBackColor = true;
            this.buttonLoadFont.Click += new System.EventHandler(this.buttonLoadFont_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 617);
            this.Controls.Add(this.buttonLoadFont);
            this.Controls.Add(this.buttonSelectAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSelectWords);
            this.Controls.Add(this.checkBoxSplitData);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkedListBoxFonts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Font Text Generator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox checkedListBoxFonts;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonSCTC3K;
		private System.Windows.Forms.RadioButton radioButtonSC3K;
		private System.Windows.Forms.RadioButton radioButtonGBK;
		private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.CheckBox checkBoxSplitData;
        private System.Windows.Forms.TextBox textBoxSelectWords;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonLoadFont;
    }
}

