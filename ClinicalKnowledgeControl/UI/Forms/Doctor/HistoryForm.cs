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
    public partial class HistoryForm : Form
    {
        private readonly int _userId;
        private readonly ReportService _reportService;

        public HistoryForm(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _reportService = new ReportService();
            LoadData();
        }

        private void LoadData()
        {
            var history = _reportService.GetDoctorHistory(_userId);
            dgv.DataSource = history;

            dgv.Columns[0].HeaderText = "Наименование теста";
            dgv.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[1].HeaderText = "Дедлайн";
            dgv.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[2].HeaderText = "Время начала теста";
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].HeaderText = "Время окончания теста";
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[4].HeaderText = "Балов";
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[5].HeaderText = "Статус";
            dgv.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[6].HeaderText = "Пройден";
            dgv.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}
