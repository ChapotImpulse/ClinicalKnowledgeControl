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
    public partial class QuestionsBankForm : Form
    {
        private readonly QuestionService _service;
        private readonly ClinicalGuidelinesService _cgService;
        private readonly User _currentUser;

        public QuestionsBankForm(User user)
        {
            InitializeComponent();
            _service = new QuestionService();
            _cgService = new ClinicalGuidelinesService();
            _currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            var data = _service.GetAllQuestions();
            dgv.DataSource = data;

            dgv.Columns[0].Visible = false;
            dgv.Columns[1].HeaderText = "Текст вопроса";
            dgv.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[2].HeaderText = "Тип вопроса";
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].HeaderText = "Тэги";
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[4].HeaderText = "Клиническая рекомендация";
            dgv.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new QuestionEditForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _service.CreateQuestion(
                        dlg.ClinicalGuidelineId,
                        dlg.QuestionText,
                        dlg.QuestionType,
                        dlg.Explanation,
                        dlg.Tags,
                        dlg.Options,
                        _currentUser.Id
                    );
                    LoadData();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            if (MessageBox.Show("Удалить вопрос?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
                _service.DeleteQuestion(id);
                LoadData();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            var model = _service.GetById(Convert.ToInt32(dgv.SelectedRows[0].Cells[0].Value));
            using (var dlg = new QuestionEditForm(model))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Вызываем метод обновления
                        _service.UpdateQuestion(
                            model.Id,
                            dlg.ClinicalGuidelineId,
                            dlg.QuestionText,
                            dlg.QuestionType,
                            dlg.Explanation,
                            dlg.Tags,
                            dlg.Options,
                            _currentUser.Id  // ID текущего пользователя (ГВС/Администратор)
                        );

                        MessageBox.Show("Вопрос успешно обновлен!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show($"Ошибка валидации: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
