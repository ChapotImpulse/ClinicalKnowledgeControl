namespace ClinicalKnowledgeControl.UI.Forms.Admin
{
    partial class QuestionEditForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCG = new System.Windows.Forms.ComboBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtQuestion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvOptions = new System.Windows.Forms.DataGridView();
            this.OptionText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsCorrect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Order = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.txtExplanation = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOptions)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 530);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(858, 47);
            this.panel1.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(723, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(123, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(594, 11);
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
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(26, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Клиническая рекомендация:";
            // 
            // cmbCG
            // 
            this.cmbCG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCG.FormattingEnabled = true;
            this.cmbCG.Location = new System.Drawing.Point(185, 23);
            this.cmbCG.Name = "cmbCG";
            this.cmbCG.Size = new System.Drawing.Size(645, 21);
            this.cmbCG.TabIndex = 9;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Одиночный выбор",
            "Множественный выбор"});
            this.cmbType.Location = new System.Drawing.Point(185, 50);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(645, 21);
            this.cmbType.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(26, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Тип вопроса:";
            // 
            // txtQuestion
            // 
            this.txtQuestion.Location = new System.Drawing.Point(185, 77);
            this.txtQuestion.Multiline = true;
            this.txtQuestion.Name = "txtQuestion";
            this.txtQuestion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtQuestion.Size = new System.Drawing.Size(645, 110);
            this.txtQuestion.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(26, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Текст вопроса:";
            // 
            // txtTags
            // 
            this.txtTags.Location = new System.Drawing.Point(185, 193);
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(645, 21);
            this.txtTags.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(26, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Тэги:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(26, 220);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Варианты ответов:";
            // 
            // dgvOptions
            // 
            this.dgvOptions.AllowUserToResizeColumns = false;
            this.dgvOptions.AllowUserToResizeRows = false;
            this.dgvOptions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOptions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OptionText,
            this.IsCorrect,
            this.Order});
            this.dgvOptions.Location = new System.Drawing.Point(185, 220);
            this.dgvOptions.Name = "dgvOptions";
            this.dgvOptions.Size = new System.Drawing.Size(645, 150);
            this.dgvOptions.TabIndex = 17;
            // 
            // OptionText
            // 
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.OptionText.DefaultCellStyle = dataGridViewCellStyle1;
            this.OptionText.HeaderText = "Текст варианта";
            this.OptionText.Name = "OptionText";
            // 
            // IsCorrect
            // 
            this.IsCorrect.FillWeight = 20F;
            this.IsCorrect.HeaderText = "Правильный";
            this.IsCorrect.Name = "IsCorrect";
            // 
            // Order
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Order.DefaultCellStyle = dataGridViewCellStyle2;
            this.Order.FillWeight = 20F;
            this.Order.HeaderText = "Порядок";
            this.Order.Name = "Order";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(26, 376);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Пояснение:";
            // 
            // txtExplanation
            // 
            this.txtExplanation.Location = new System.Drawing.Point(185, 376);
            this.txtExplanation.Multiline = true;
            this.txtExplanation.Name = "txtExplanation";
            this.txtExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExplanation.Size = new System.Drawing.Size(645, 138);
            this.txtExplanation.TabIndex = 18;
            // 
            // QuestionEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 577);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtExplanation);
            this.Controls.Add(this.dgvOptions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTags);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtQuestion);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbCG);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuestionEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOptions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCG;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtQuestion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvOptions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtExplanation;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionText;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsCorrect;
        private System.Windows.Forms.DataGridViewTextBoxColumn Order;
    }
}