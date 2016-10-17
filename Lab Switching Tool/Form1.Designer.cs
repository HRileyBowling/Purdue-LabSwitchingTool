namespace Lab_Switching_Tool
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
            this.cboLab = new System.Windows.Forms.ComboBox();
            this.dtpSelectedDate = new System.Windows.Forms.DateTimePicker();
            this.btnPrint = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.txtNow = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblLastUpdated = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboLab
            // 
            this.cboLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLab.FormattingEnabled = true;
            this.cboLab.Items.AddRange(new object[] {
            "Select a lab",
            "AACC",
            "BCC",
            "BRES",
            "BRNG",
            "HAMP",
            "HEAV",
            "HIKS",
            "KRAN",
            "LCC",
            "LILY",
            "LYNN",
            "MATH",
            "MCUT",
            "MRDH",
            "MTHW",
            "NACC",
            "NLSN",
            "PHYS",
            "POTR",
            "RPHM",
            "SC",
            "STEW",
            "TERM",
            "WTHR",
            "[Show ITaP Labs Only]"});
            this.cboLab.Location = new System.Drawing.Point(12, 115);
            this.cboLab.Name = "cboLab";
            this.cboLab.Size = new System.Drawing.Size(90, 21);
            this.cboLab.TabIndex = 0;
            this.cboLab.SelectedIndexChanged += new System.EventHandler(this.cboLab_SelectedIndexChanged);
            // 
            // dtpSelectedDate
            // 
            this.dtpSelectedDate.Location = new System.Drawing.Point(12, 89);
            this.dtpSelectedDate.Name = "dtpSelectedDate";
            this.dtpSelectedDate.Size = new System.Drawing.Size(203, 20);
            this.dtpSelectedDate.TabIndex = 1;
            this.dtpSelectedDate.ValueChanged += new System.EventHandler(this.cboLab_SelectedIndexChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(108, 115);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(107, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(12, 173);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(387, 423);
            this.txtOutput.TabIndex = 3;
            this.txtOutput.WordWrap = false;
            // 
            // txtNow
            // 
            this.txtNow.Location = new System.Drawing.Point(243, 12);
            this.txtNow.Multiline = true;
            this.txtNow.Name = "txtNow";
            this.txtNow.ReadOnly = true;
            this.txtNow.Size = new System.Drawing.Size(156, 142);
            this.txtNow.TabIndex = 4;
            this.txtNow.Text = "No lab selected";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 144);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(203, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.cboLab_SelectedIndexChanged);
            // 
            // lblLastUpdated
            // 
            this.lblLastUpdated.AutoSize = true;
            this.lblLastUpdated.Location = new System.Drawing.Point(243, 154);
            this.lblLastUpdated.Name = "lblLastUpdated";
            this.lblLastUpdated.Size = new System.Drawing.Size(71, 13);
            this.lblLastUpdated.TabIndex = 7;
            this.lblLastUpdated.Text = "Last Updated";
            this.lblLastUpdated.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(196, 62);
            this.label3.TabIndex = 8;
            this.label3.Text = "Lab Switching\r\nTool";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 612);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLastUpdated);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtNow);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.dtpSelectedDate);
            this.Controls.Add(this.cboLab);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(427, 688);
            this.Name = "Form1";
            this.Text = "Lab Switching Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLab;
        private System.Windows.Forms.DateTimePicker dtpSelectedDate;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TextBox txtNow;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.Label label3;
    }
}

