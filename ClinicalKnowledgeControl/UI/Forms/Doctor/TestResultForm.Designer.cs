namespace ClinicalKnowledgeControl.UI.Forms.Doctor
{
    partial class TestResultForm
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
            this.lblResult = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblQuestions = new System.Windows.Forms.Label();
            this.txtReview = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblResult.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblResult.Location = new System.Drawing.Point(12, 10);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(542, 49);
            this.lblResult.TabIndex = 0;
            this.lblResult.Text = "ТЕСТ СДАН!";
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblScore
            // 
            this.lblScore.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblScore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblScore.Location = new System.Drawing.Point(12, 73);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(542, 49);
            this.lblScore.TabIndex = 1;
            this.lblScore.Text = "Ваш результат: 0.0";
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 443);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(542, 45);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lblQuestions
            // 
            this.lblQuestions.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblQuestions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblQuestions.Location = new System.Drawing.Point(12, 122);
            this.lblQuestions.Name = "lblQuestions";
            this.lblQuestions.Size = new System.Drawing.Size(542, 27);
            this.lblQuestions.TabIndex = 3;
            this.lblQuestions.Text = "Разбор ответов:";
            this.lblQuestions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtReview
            // 
            this.txtReview.BackColor = System.Drawing.Color.White;
            this.txtReview.Location = new System.Drawing.Point(12, 152);
            this.txtReview.Multiline = true;
            this.txtReview.Name = "txtReview";
            this.txtReview.ReadOnly = true;
            this.txtReview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReview.Size = new System.Drawing.Size(542, 285);
            this.txtReview.TabIndex = 4;
            // 
            // TestResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 500);
            this.Controls.Add(this.txtReview);
            this.Controls.Add(this.lblQuestions);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblResult);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestResultForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Результат тестирования";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblQuestions;
        private System.Windows.Forms.TextBox txtReview;
    }
}