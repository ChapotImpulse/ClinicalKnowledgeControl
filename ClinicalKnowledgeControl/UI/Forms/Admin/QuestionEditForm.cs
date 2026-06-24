using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.BLL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI.Forms.Admin
{
    public partial class QuestionEditForm : Form
    {
        public int ClinicalGuidelineId { get; private set; }
        public string QuestionText { get; private set; }
        public int QuestionType { get; private set; }
        public string Explanation { get; private set; }
        public string Tags { get; private set; }
        public List<(string text, bool isCorrect, int? order)> Options { get; private set; }

        public QuestionEditForm()
        {
            Text = "Добавить новый вопрос...";

            InitializeComponent();
            LoadClinicalGuidelines();
        }

        public QuestionEditForm(Question question) : this()
        {
            Text = "Редактировать вопрос...";

            if (question == null) return;

            // Заполняем поля формы данными из модели
            cmbCG.SelectedValue = question.ClinicalGuidelineId;
            txtQuestion.Text = question.Text;
            cmbType.SelectedIndex = (int)question.QuestionType - 1;
            txtTags.Text = question.Tags ?? "";
            txtExplanation.Text = question.Explanation ?? "";

            // Заполняем варианты ответов
            foreach (var option in question.Options)
            {
                dgvOptions.Rows.Add(option.Text, option.IsCorrect, option.SequenceOrder);
            }
        }

        private void LoadClinicalGuidelines()
        {
            var cgService = new ClinicalGuidelinesService();
            var data = cgService.GetAll();
            cmbCG.DataSource = data;
            cmbCG.DisplayMember = "Name";
            cmbCG.ValueMember = "Id";

            cmbType.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCG.SelectedValue == null || string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Options = new List<(string text, bool isCorrect, int? order)>();
            foreach (DataGridViewRow row in dgvOptions.Rows)
            {
                if (row.IsNewRow) continue;
                string text = row.Cells["OptionText"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(text)) continue;

                bool isCorrect = false;
                bool.TryParse(row.Cells["IsCorrect"].Value?.ToString(), out isCorrect);

                int? order = null;
                if (int.TryParse(row.Cells["Order"].Value?.ToString(), out int o))
                {
                    order = o;
                }

                Options.Add((text, isCorrect, order));
            }

            if (Options.Count < 2)
            {
                MessageBox.Show("Добавьте минимум 2 варианта ответа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ClinicalGuidelineId = Convert.ToInt32(cmbCG.SelectedValue);
            QuestionText = txtQuestion.Text;
            QuestionType = cmbType.SelectedIndex + 1;
            Explanation = txtExplanation.Text;
            Tags = txtTags.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
