using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.BLL.Services;
using ClinicalKnowledgeControl.DAL;
using ClinicalKnowledgeControl.DAL.Repositories;
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
    public partial class AssignmentsForm : Form
    {
        private readonly AssignmentService _service;
        private readonly TestTemplateService _templateService;
        private readonly User _currentUser;

        public AssignmentsForm(User user)
        {
            InitializeComponent();
            _service = new AssignmentService();
            _templateService = new TestTemplateService();
            _currentUser = user;

            LoadData();
        }

        private void LoadData()
        {
            var data = _service.GetAll();
            dgv.DataSource = data;

            dgv.Columns[0].Visible = false;
            dgv.Columns[5].Visible = false;
            dgv.Columns[1].HeaderText = "Наименование теста";
            dgv.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[2].HeaderText = "Дедлайн";
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[3].HeaderText = "Тип назначения";
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[4].HeaderText = "Цель";
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[6].HeaderText = "Активно";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new AssignmentsEditForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        switch (dlg.TargetType)
                        {
                            case 1: // Отделение
                                _service.AssignToDepartment(
                                    dlg.TestTemplateId,
                                    dlg.TargetId,
                                    dlg.Deadline
                                );
                                break;

                            case 2: // Специальность
                                    // Получаем название специальности из TargetId (это Id врача)
                                User user = new UserService().GetById(dlg.TargetId);
                                _service.AssignBySpecialty(
                                    dlg.TestTemplateId,
                                    user?.Specialty ?? "",
                                    dlg.Deadline
                                );
                                LoadData();
                                return;

                            case 3: // Врач
                                _service.AssignToDoctor(
                                    dlg.TestTemplateId,
                                    dlg.TargetId,
                                    dlg.Deadline
                                );
                                break;
                            default:
                                return;
                        }

                        MessageBox.Show($"Назначение создано.", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка назначения",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int assignmentId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            string testName = dgv.SelectedRows[0].Cells["TestName"].Value?.ToString();
            string targetName = dgv.SelectedRows[0].Cells["TargetName"].Value?.ToString();

            if (MessageBox.Show(
                $"Вы уверены, что хотите удалить назначение теста \"{testName}\" для \"{targetName}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _service.DeleteAssignment(assignmentId);
                    MessageBox.Show("Назначение удалено!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Невозможно удалить",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите назначение для редактирования", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int assignmentId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);

            try
            {
                Assignment assignment = _service.GetById(assignmentId);
                if (assignment == null)
                {
                    MessageBox.Show("Назначение не найдено", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var dlg = new AssignmentsEditForm(assignment))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _service.UpdateAssignment(
                            assignmentId,
                            dlg.TestTemplateId,
                            dlg.TargetType,
                            Convert.ToInt32(dlg.TargetId),
                            dlg.Deadline
                        );

                        MessageBox.Show("Назначение обновлено!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Невозможно изменить",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();    
        }
    }
}
