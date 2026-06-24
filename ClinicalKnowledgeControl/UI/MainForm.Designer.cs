namespace ClinicalKnowledgeControl.UI
{
    partial class MainForm
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
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblUserInfo.Location = new System.Drawing.Point(0, 777);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Padding = new System.Windows.Forms.Padding(5);
            this.lblUserInfo.Size = new System.Drawing.Size(89, 23);
            this.lblUserInfo.TabIndex = 0;
            this.lblUserInfo.Text = "Пользователь";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.lblUserInfo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Контроль знаний клинических рекомендаций";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblUserInfo;
    }
}