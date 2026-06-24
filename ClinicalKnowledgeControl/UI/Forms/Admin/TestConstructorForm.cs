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
    public partial class TestConstructorForm : Form
    {
        private readonly TestTemplateService _service;
        private readonly ClinicalGuidelinesService _cgService;
        private readonly User _currentUser;

        public TestConstructorForm(User user)
        {
            InitializeComponent();
            _service = new TestTemplateService();
            _cgService = new ClinicalGuidelinesService();
            _currentUser = user;

            LoadData();
        }

        private void LoadData()
        {
            var data = _service.GetAll();
            dgv.DataSource = data;

            dgv.Columns[0].Visible = false;
            dgv.Columns[1].HeaderText = "Наименование теста";
            dgv.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[2].HeaderText = "Клиническая рекомендация";
            dgv.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[3].HeaderText = "Количество вопросов";
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].FillWeight = 50;
            dgv.Columns[4].HeaderText = "Время (минут)";
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[4].FillWeight = 50;
            dgv.Columns[5].HeaderText = "Проходной бал (%)";
            dgv.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[5].FillWeight = 50;
            dgv.Columns[6].HeaderText = "Макс. попыток";
            dgv.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[6].FillWeight = 50;
            dgv.Columns[7].Visible = false;
            dgv.Columns[8].Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите шаблон для редактирования!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int templateId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);

            try
            {
                TestTemplate template = _service.GetById(templateId);
                if (template == null)
                {
                    MessageBox.Show("Шаблон не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var dlg = new TestConstructorEditForm(template))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _service.UpdateTemplate(
                            templateId,
                            dlg.Name,
                            dlg.ClinicalGuidelineId,
                            dlg.QuestionCount,
                            dlg.TimeLimitMinutes,
                            dlg.PassingScore,
                            dlg.MaxAttempts,
                            _currentUser.Id
                        );

                        MessageBox.Show("Шаблон обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка валидации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int templateId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            string templateName = dgv.SelectedRows[0].Cells["Name"].Value?.ToString();

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить шаблон теста \"{templateName}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _service.DeleteTemplate(templateId);
                    MessageBox.Show("Шаблон удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Невозможно удалить", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new TestConstructorEditForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _service.Create(
                        dlg.Name,
                        dlg.ClinicalGuidelineId,
                        dlg.QuestionCount,
                        dlg.TimeLimitMinutes,
                        dlg.PassingScore,
                        dlg.MaxAttempts,
                        _currentUser.Id
                    );

                    MessageBox.Show("Шаблон теста создан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
