using ClinicalKnowledgeControl.BLL.Services;
using ClinicalKnowledgeControl.UI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI.Forms.Reports
{
    public partial class DepartmentReportForm : Form
    {
        private readonly int _departmentId;
        private readonly ReportService _service;

        public DepartmentReportForm(int departmentId)
        {
            InitializeComponent();
            _departmentId = departmentId;
            _service = new ReportService();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Отчет по отделению";
            this.Size = new System.Drawing.Size(1100, 700);

            var tabControl = new TabControl { Dock = DockStyle.Fill };

            var tabSummary = new TabPage("Сводка по отделению");
            var dgvSummary = new DataGridView
            {
                Name = "dgvSummary",
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
                RowHeadersVisible = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabSummary.Controls.Add(dgvSummary);
            tabControl.TabPages.Add(tabSummary);

            var tabRating = new TabPage("Рейтинг врачей");
            var dgvRating = new DataGridView
            {
                Name = "dgvRating",
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
                RowHeadersVisible = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabRating.Controls.Add(dgvRating);
            tabControl.TabPages.Add(tabRating);

            var tabQuestions = new TabPage("Статистика по вопросам");
            var dgvQuestions = new DataGridView
            {
                Name = "dgvQuestions",
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
                RowHeadersVisible = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabQuestions.Controls.Add(dgvQuestions);
            tabControl.TabPages.Add(tabQuestions);

            this.Controls.Add(tabControl);

            var panelButtons = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            var btnExport = new Button { Text = "Экспорт в Excel", Location = new System.Drawing.Point(20, 10), Size = new System.Drawing.Size(150, 30) };
            btnExport.Click += BtnExport_Click;
            panelButtons.Controls.Add(btnExport);

            var lblWarning = new Label { Name = "lblWarning", Location = new System.Drawing.Point(200, 15), AutoSize = true, ForeColor = System.Drawing.Color.Red, Font = new System.Drawing.Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Bold) };
            panelButtons.Controls.Add(lblWarning);

            this.Controls.Add(panelButtons);
        }

        private void LoadData()
        {
            var summary = _service.GetDepartmentReport(_departmentId);
            var dgvSummary = this.Controls.Find("dgvSummary", true)[0] as DataGridView;
            dgvSummary.DataSource = summary;

            dgvSummary.Columns[0].HeaderText = "ФИО врача";
            dgvSummary.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvSummary.Columns[1].HeaderText = "Специальность";
            dgvSummary.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvSummary.Columns[2].HeaderText = "Наименование теста";
            dgvSummary.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvSummary.Columns[3].HeaderText = "Дедлайн";
            dgvSummary.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSummary.Columns[4].HeaderText = "Статус";
            dgvSummary.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSummary.Columns[5].HeaderText = "Баллов";
            dgvSummary.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSummary.Columns[6].HeaderText = "Дата окончания";
            dgvSummary.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var rating = _service.GetDoctorRatingByDepartment(_departmentId);
            var dgvRating = this.Controls.Find("dgvRating", true)[0] as DataGridView;
            dgvRating.DataSource = rating;

            dgvRating.Columns[0].HeaderText = "ФИО врача";
            dgvRating.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvRating.Columns[1].HeaderText = "Отделение";
            dgvRating.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvRating.Columns[2].HeaderText = "Пройдено тестов";
            dgvRating.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRating.Columns[3].HeaderText = "Всего попыток";
            dgvRating.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRating.Columns[4].HeaderText = "Средний балл";
            dgvRating.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var questions = _service.GetQuestionStatisticsByDepartment(_departmentId);
            var dgvQuestions = this.Controls.Find("dgvQuestions", true)[0] as DataGridView;
            dgvQuestions.DataSource = questions;

            dgvQuestions.Columns[0].HeaderText = "Текст вопроса";
            dgvQuestions.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvQuestions.Columns[1].HeaderText = "Клиническая рекомендация";
            dgvQuestions.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvQuestions.Columns[2].HeaderText = "Всего ответов";
            dgvQuestions.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvQuestions.Columns[3].HeaderText = "Количество правильных ответов";
            dgvQuestions.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvQuestions.Columns[4].HeaderText = "Процент правильных ответов";
            dgvQuestions.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Проверка необходимости дополнительного обучения
            if (_service.NeedAdditionalTraining(_departmentId))
            {
                var lblWarning = this.Controls.Find("lblWarning", true)[0] as Label;
                lblWarning.Text = "⚠ Средний балл отделения менее 70%! Требуется организация обучения.";
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            var tabControl = this.Controls.Find("tabControl", false)[0] as TabControl;
            var currentTab = tabControl.SelectedTab;
            var dgv = currentTab.Controls[0] as DataGridView;

            if (dgv?.DataSource is DataTable dt)
            {
                ExcelExporter.ExportToExcel(dt, currentTab.Text);
            }
        }
    }
}
