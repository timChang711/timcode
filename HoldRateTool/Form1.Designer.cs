namespace HoldRateTool
{
    partial class Form1
    {
        private System.Windows.Forms.Button btnProcessFile;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label ProductSelect;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ComboBox ProductCombo;
        private System.Windows.Forms.DateTimePicker startDatePicker; // 新增的 DateTimePicker 控件
        private System.Windows.Forms.RadioButton meanRadioButton;
        private System.Windows.Forms.RadioButton segmaRadioButton;
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.btnProcessFile = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.ProductSelect = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ProductCombo = new System.Windows.Forms.ComboBox();
            this.startDatePicker = new System.Windows.Forms.DateTimePicker();
            this.StartDate = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.meanRadioButton = new System.Windows.Forms.RadioButton();
            this.segmaRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Font = new System.Drawing.Font("Calibri", 10.8F);
            this.richTextBoxOutput.Location = new System.Drawing.Point(9, 106);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.ReadOnly = true;
            this.richTextBoxOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxOutput.Size = new System.Drawing.Size(430, 200);
            this.richTextBoxOutput.TabIndex = 1;
            this.richTextBoxOutput.Text = "";
            // 
            // btnProcessFile
            // 
            this.btnProcessFile.Location = new System.Drawing.Point(657, 289);
            this.btnProcessFile.Margin = new System.Windows.Forms.Padding(8);
            this.btnProcessFile.Name = "btnProcessFile";
            this.btnProcessFile.Size = new System.Drawing.Size(391, 77);
            this.btnProcessFile.TabIndex = 1;
            this.btnProcessFile.Text = "SBL/Mean Trigger Check";
            this.btnProcessFile.UseVisualStyleBackColor = true;
            this.btnProcessFile.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(57, 307);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1133, 72);
            this.lblStatus.TabIndex = 3;
            // 
            // ProductSelect
            // 
            this.ProductSelect.AutoSize = true;
            this.ProductSelect.Location = new System.Drawing.Point(3, 9);
            this.ProductSelect.Name = "ProductSelect";
            this.ProductSelect.Size = new System.Drawing.Size(208, 40);
            this.ProductSelect.TabIndex = 4;
            this.ProductSelect.Text = "Product Select";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // ProductCombo
            // 
            this.ProductCombo.FormattingEnabled = true;
            this.ProductCombo.Items.AddRange(new object[] {
            "XAN",
            "YOK",
            "NPT"});
            this.ProductCombo.Location = new System.Drawing.Point(241, 6);
            this.ProductCombo.Name = "ProductCombo";
            this.ProductCombo.Size = new System.Drawing.Size(198, 48);
            this.ProductCombo.TabIndex = 7;
            this.ProductCombo.SelectedIndexChanged += new System.EventHandler(this.ProductCombo_SelectedIndexChanged);
            // 
            // startDatePicker
            // 
            this.startDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.startDatePicker.Location = new System.Drawing.Point(241, 60);
            this.startDatePicker.Name = "startDatePicker";
            this.startDatePicker.Size = new System.Drawing.Size(198, 48);
            this.startDatePicker.TabIndex = 8;
            // 
            // StartDate
            // 
            this.StartDate.AutoSize = true;
            this.StartDate.Location = new System.Drawing.Point(3, 60);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(266, 40);
            this.StartDate.TabIndex = 9;
            this.StartDate.Text = "Start date by week";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HodlRateTool.Properties.Resources.TDK;
            this.pictureBox1.Location = new System.Drawing.Point(732, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(301, 165);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // meanRadioButton
            // 
            this.meanRadioButton.AutoSize = true;
            this.meanRadioButton.Location = new System.Drawing.Point(592, 7);
            this.meanRadioButton.Name = "meanRadioButton";
            this.meanRadioButton.Size = new System.Drawing.Size(115, 44);
            this.meanRadioButton.TabIndex = 11;
            this.meanRadioButton.TabStop = true;
            this.meanRadioButton.Text = "Mean";
            this.meanRadioButton.UseVisualStyleBackColor = true;
            this.meanRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // segmaRadioButton
            // 
            this.segmaRadioButton.AutoSize = true;
            this.segmaRadioButton.Location = new System.Drawing.Point(459, 7);
            this.segmaRadioButton.Name = "segmaRadioButton";
            this.segmaRadioButton.Size = new System.Drawing.Size(119, 44);
            this.segmaRadioButton.TabIndex = 12;
            this.segmaRadioButton.TabStop = true;
            this.segmaRadioButton.Text = "Sigma";
            this.segmaRadioButton.UseVisualStyleBackColor = true;
            this.segmaRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 40F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1055, 377);
            this.Controls.Add(this.meanRadioButton);
            this.Controls.Add(this.segmaRadioButton);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.StartDate);
            this.Controls.Add(this.startDatePicker);
            this.Controls.Add(this.ProductCombo);
            this.Controls.Add(this.ProductSelect);
            this.Controls.Add(this.btnProcessFile);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("Calibri", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "Form1";
            this.Text = "HoldRateTool";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label StartDate;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
