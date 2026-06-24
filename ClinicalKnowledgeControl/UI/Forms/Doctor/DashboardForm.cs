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

namespace ClinicalKnowledgeControl.UI.Forms.Doctor
{
    public partial class DashboardForm : Form
    {
        private readonly User _currentUser;
        private readonly ReportService _reportService;
        private readonly TestService _testService;

        public DashboardForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _reportService = new ReportService();
            _testService = new TestService();

            lblTitle.Text = $"Здравствуйте, {user.FullName}!";

            LoadData();
        }

        
        private void LoadData()
        {
            var assignments = _reportService.GetDoctorAssignments(_currentUser.Id);
            dgv.DataSource = assignments;

            dgv.Columns[0].Visible = false;
            dgv.Columns[1].Visible = false;
            dgv.Columns[2].HeaderText = "Наименование теста";
            dgv.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[3].HeaderText = "Клиническая рекомендация";
            dgv.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[4].HeaderText = "Дедлайн";
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[5].HeaderText = "Статус";
            dgv.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[6].HeaderText = "Количество минут";
            dgv.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[7].HeaderText = "Максимальное количество попыток";
            dgv.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var summary = _reportService.GetMonthlySummary(_currentUser.Id);
            dgvSummary.DataSource = summary;

            dgvSummary.Columns[0].HeaderText = "Месяц";
            dgvSummary.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSummary.Columns[1].HeaderText = "Количество тестов";
            dgvSummary.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSummary.Columns[2].HeaderText = "Сдано тестов";
            dgvSummary.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSummary.Columns[3].HeaderText = "Средний балл";
            dgvSummary.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgv.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите тест для прохождения!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var row = dgv.SelectedRows[0];
                string status = row.Cells["Status"].Value?.ToString();

                if (status == "Выполнено")
                {
                    MessageBox.Show("Этот тест уже сдан!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int testTemplateId = Convert.ToInt32(row.Cells["TestTemplateId"].Value);
                int assignmentId = Convert.ToInt32(row.Cells["AssignmentId"].Value);

                if (!_testService.CanStartTest(_currentUser.Id, testTemplateId))
                {
                    MessageBox.Show("Вы исчерпали количество попыток на сегодня. Попробуйте завтра.",
                        "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Hide();
                var testForm = new TestTakingForm(_currentUser.Id, testTemplateId, assignmentId) { TopMost = true };
                testForm.FormClosed += (s, args) => this.Show();
                testForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
