namespace ClinicalKnowledgeControl.UI.Forms.Doctor
{
    partial class TestTakingForm
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
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.btnNext = new System.Windows.Forms.Button();
            this.txtQuestion = new ClinicalKnowledgeControl.UI.Controls.SecureTextBox();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblProgress.Location = new System.Drawing.Point(20, 20);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(104, 16);
            this.lblProgress.TabIndex = 0;
            this.lblProgress.Text = "Вопрос 1 из 10";
            // 
            // lblTimer
            // 
            this.lblTimer.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTimer.ForeColor = System.Drawing.Color.Red;
            this.lblTimer.Location = new System.Drawing.Point(498, 20);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(362, 20);
            this.lblTimer.TabIndex = 1;
            this.lblTimer.Text = "Осталось: 30:00";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelOptions
            // 
            this.panelOptions.AutoScroll = true;
            this.panelOptions.Location = new System.Drawing.Point(20, 180);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(840, 350);
            this.panelOptions.TabIndex = 3;
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnNext.Location = new System.Drawing.Point(710, 548);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(150, 40);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Далее →";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // txtQuestion
            // 
            this.txtQuestion.BackColor = System.Drawing.SystemColors.Window;
            this.txtQuestion.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtQuestion.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtQuestion.Location = new System.Drawing.Point(20, 60);
            this.txtQuestion.Multiline = true;
            this.txtQuestion.Name = "txtQuestion";
            this.txtQuestion.ReadOnly = true;
            this.txtQuestion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtQuestion.ShortcutsEnabled = false;
            this.txtQuestion.Size = new System.Drawing.Size(840, 100);
            this.txtQuestion.TabIndex = 2;
            this.txtQuestion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TestTakingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 610);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.txtQuestion);
            this.Controls.Add(this.panelOptions);
            this.Controls.Add(this.btnNext);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestTakingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Прохождение теста";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblTimer;
        private ClinicalKnowledgeControl.UI.Controls.SecureTextBox txtQuestion;
        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.Button btnNext;
    }
}