using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.BLL.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI.Forms.Admin
{
    public partial class TestConstructorEditForm : Form
    {
        public string TestName { get; private set; }
        public int ClinicalGuidelineId { get; private set; }
        public int QuestionCount { get; private set; }
        public int TimeLimitMinutes { get; private set; }
        public decimal PassingScore { get; private set; }
        public int MaxAttempts { get; private set; }

        public TestConstructorEditForm()
        {
            InitializeComponent();
            LoadClinicalGuidelines();

            Text = "Добавить новый тест...";
        }

        public TestConstructorEditForm(TestTemplate template): this()
        {
            Text = "Редактировать тест...";

            txtName.Text = template.Name;
            cmbCG.SelectedValue = template.ClinicalGuidelineId;
            numCount.Value = template.QuestionCount;
            numTime.Value = template.TimeLimitMinutes;
            numScore.Value = template.PassingScore;
            numAttempts.Value = template.MaxAttempts;
        }

        private void LoadClinicalGuidelines()
        {
            var cgService = new ClinicalGuidelinesService();
            var data = cgService.GetAll();
            cmbCG.DataSource = data;
            cmbCG.DisplayMember = "Name";
            cmbCG.ValueMember = "Id";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cmbCG.SelectedIndex < 0)
            {
                MessageBox.Show("Укажите клиническую рекомендацию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Name = txtName.Text;
            ClinicalGuidelineId = Convert.ToInt32(cmbCG.SelectedValue);
            QuestionCount = (int)numCount.Value;
            TimeLimitMinutes = (int)numTime.Value;
            PassingScore = numScore.Value;
            MaxAttempts = (int)numAttempts.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
