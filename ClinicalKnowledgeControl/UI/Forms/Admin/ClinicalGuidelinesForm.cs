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
    public partial class ClinicalGuidelinesForm : Form
    {
        private readonly ClinicalGuidelinesService _service;

        public ClinicalGuidelinesForm()
        {
            InitializeComponent();
            _service = new ClinicalGuidelinesService();
            LoadData();
        }

        private void LoadData()
        {
            var data = _service.GetAll();
            dgv.DataSource = data;

            dgv.Columns[0].Visible = false;
            dgv.Columns[1].HeaderText = "Название";
            dgv.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[2].HeaderText = "Код МКБ";
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].HeaderText = "Дата обновления";
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[4].HeaderText = "Дата вступления в силу";
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[5].HeaderText = "Ссылка на файл";
            dgv.Columns[5].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[6].HeaderText = "Описание";
            dgv.Columns[6].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new ClinicalGuidelineEditForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _service.Create(dlg.Name, dlg.ICDCode, dlg.UpdateDate, dlg.EffectiveDate, dlg.FileLink, dlg.Description);
                    LoadData();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            var model = _service.GetById(Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value));
            using (var dlg = new ClinicalGuidelineEditForm(model.Name, model.ICDCode, model.UpdateDate, model.EffectiveDate, model.FileLink, model.Description))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _service.Update(model.Id, dlg.Name, dlg.ICDCode, dlg.UpdateDate, dlg.EffectiveDate, dlg.FileLink, dlg.Description);
                    LoadData();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            if (MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
                _service.Delete(id);
                LoadData();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClinicalGuidelinesForm_Load(object sender, EventArgs e)
        {
        }
    }
}
