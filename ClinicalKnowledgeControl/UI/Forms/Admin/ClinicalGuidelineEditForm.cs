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
    public partial class ClinicalGuidelineEditForm : Form
    {
        public string Name { get; private set; }
        public string ICDCode { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public DateTime? EffectiveDate { get; private set; }
        public string FileLink { get; private set; }
        public string Description { get; private set; }
                
        public ClinicalGuidelineEditForm(string name = "", string icd = "", DateTime? update = null,
            DateTime? effective = null, string fileLink = "", string description = "")
        {
            InitializeComponent();

            txtName.Text = name;
            txtICD.Text = icd;
            dtpUpdate.Value = update ?? DateTime.Now;
            dtpEffective.Value = effective ?? DateTime.Now;
            txtFileLink.Text = fileLink;
            txtDescription.Text = description;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Name = txtName.Text;
            ICDCode = txtICD.Text;
            UpdateDate = dtpUpdate.Value;
            EffectiveDate = dtpEffective.Value;
            FileLink = txtFileLink.Text;
            Description = txtDescription.Text;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
