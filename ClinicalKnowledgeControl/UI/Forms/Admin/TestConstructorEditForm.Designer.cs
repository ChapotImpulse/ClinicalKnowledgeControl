namespace ClinicalKnowledgeControl.UI.Forms.Admin
{
    partial class TestConstructorEditForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmbCG = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.numTime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numAttempts = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numScore = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttempts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScore)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 126);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 47);
            this.panel1.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(753, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(123, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(624, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(123, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Наименование теста:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(191, 29);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(663, 21);
            this.txtName.TabIndex = 9;
            // 
            // cmbCG
            // 
            this.cmbCG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCG.FormattingEnabled = true;
            this.cmbCG.Location = new System.Drawing.Point(191, 56);
            this.cmbCG.Name = "cmbCG";
            this.cmbCG.Size = new System.Drawing.Size(663, 21);
            this.cmbCG.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(32, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Клиническая рекомендация:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(32, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Количество вопросов:";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(191, 83);
            this.numCount.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(76, 21);
            this.numCount.TabIndex = 13;
            this.numCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numTime
            // 
            this.numTime.Location = new System.Drawing.Point(374, 83);
            this.numTime.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTime.Name = "numTime";
            this.numTime.Size = new System.Drawing.Size(76, 21);
            this.numTime.TabIndex = 15;
            this.numTime.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(286, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Время (минут):";
            // 
            // numAttempts
            // 
            this.numAttempts.Location = new System.Drawing.Point(778, 83);
            this.numAttempts.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAttempts.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAttempts.Name = "numAttempts";
            this.numAttempts.Size = new System.Drawing.Size(76, 21);
            this.numAttempts.TabIndex = 19;
            this.numAttempts.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(685, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Макс. попыток:";
            // 
            // numScore
            // 
            this.numScore.Location = new System.Drawing.Point(589, 83);
            this.numScore.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numScore.Name = "numScore";
            this.numScore.Size = new System.Drawing.Size(76, 21);
            this.numScore.TabIndex = 17;
            this.numScore.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(473, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Проходной бал (%):";
            // 
            // TestConstructorEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 173);
            this.Controls.Add(this.numAttempts);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numScore);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbCG);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestConstructorEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TestConstructorEditForm";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttempts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cmbCG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.NumericUpDown numTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numAttempts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numScore;
        private System.Windows.Forms.Label label6;
    }
}