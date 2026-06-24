using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.BLL.Services;
using ClinicalKnowledgeControl.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI.Forms.Admin
{
    public partial class AssignmentsEditForm : Form
    {
        public int TestTemplateId { get; private set; }
        public int TargetType { get; private set; }
        public int TargetId { get; private set; }
        public DateTime Deadline { get; private set; }

        public AssignmentsEditForm()
        {
            this.Text = "Добавить новое назначение...";

            InitializeComponent();
            LoadData();
        }

        public AssignmentsEditForm(Assignment assignment) : this()
        {
            this.Text = "Редактирование назначения...";

            cmbTemplate.SelectedValue = assignment.TestTemplateId;
            cmbType.SelectedIndex = assignment.TargetType - 1;
            dtpDeadline.Value = assignment.Deadline;

            // TargetId будет установлен после загрузки cmbTarget
            if (cmbTarget.DataSource is DataTable dt)
            {
                if (dt.Columns.Contains("Id"))
                {
                    try { cmbTarget.SelectedValue = assignment.TargetId; } catch { }
                }
            }
        }
                
        private void LoadData()
        {
            // Загружаем шаблоны тестов
            var templateService = new TestTemplateService();
            var templates = templateService.GetAll();
            cmbTemplate.DataSource = templates;
            cmbTemplate.DisplayMember = "Name";
            cmbTemplate.ValueMember = "Id";
            cmbTemplate.SelectedIndex = 0;

            cmbType.Items.AddRange(new object[] { "Отделение", "Специальность", "Врач" });
            cmbType.SelectedIndex = 0;
            // Загружаем цели в зависимости от типа
            cmbType_SelectedIndexChanged(null, null);
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbTarget.DataSource = null;
            cmbTarget.Items.Clear();

            if (cmbType.SelectedIndex < 0) return;

            var userService = new UserService();

            switch (cmbType.SelectedIndex)
            {
                case 0: // Отделение
                    var departments = userService.GetDepartments();
                    cmbTarget.DataSource = departments;
                    cmbTarget.DisplayMember = "Name";
                    cmbTarget.ValueMember = "Id";
                    break;
                case 1: // Специальность
                    using (var conn = ConnectionManager.GetConnection())
                    {
                        string query = "SELECT DISTINCT Specialty FROM Users WHERE Specialty IS NOT NULL ORDER BY Specialty";
                        using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                        {
                            conn.Open();
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cmbTarget.Items.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                    if (cmbTarget.Items.Count > 0) cmbTarget.SelectedIndex = 0;
                    break;
                case 2: // Врач
                    var doctors = userService.GetAllDoctors();
                    cmbTarget.DataSource = doctors;
                    cmbTarget.DisplayMember = "FullName";
                    cmbTarget.ValueMember = "Id";

                    if (cmbTarget.Items.Count > 0)
                        cmbTarget.SelectedIndex = 0;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (!ValidateForm())
            {
                return;
            }

            // Сохранение данных в свойства
            TestTemplateId = Convert.ToInt32(cmbTemplate.SelectedValue);
            TargetType = cmbType.SelectedIndex + 1;
            Deadline = dtpDeadline.Value;

            // Определение TargetId в зависимости от типа
            switch (TargetType)
            {
                case 1: // Отделение
                    TargetId = Convert.ToInt32(cmbTarget.SelectedValue);
                    break;

                case 2: // Специальность
                    // Для специальности TargetId — это Id первого врача с такой специальностью
                    // (согласно логике MassAssignBySpecialty)
                    TargetId = GetFirstUserIdBySpecialty(cmbTarget.SelectedItem.ToString());
                    if (TargetId == 0)
                    {
                        MessageBox.Show("Не найдено врачей с указанной специальностью",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    break;

                case 3: // Врач
                    TargetId = Convert.ToInt32(cmbTarget.SelectedValue);
                    break;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Получить Id первого врача с указанной специальностью
        /// </summary>
        private int GetFirstUserIdBySpecialty(string specialty)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT TOP 1 Id 
                    FROM Users 
                    WHERE Specialty = @Specialty 
                      AND RoleId = 1 
                      AND IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Specialty", specialty);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// Проверка корректности заполнения формы
        /// </summary>
        private bool ValidateForm()
        {
            if (cmbTemplate.SelectedValue == null)
            {
                MessageBox.Show("Выберите шаблон теста", "Ошибка валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTemplate.Focus();
                return false;
            }

            if (cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите тип назначения", "Ошибка валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbType.Focus();
                return false;
            }

            if (cmbType.SelectedIndex == 1)
            {
                // Для специальности проверяем SelectedItem
                if (cmbTarget.SelectedItem == null)
                {
                    MessageBox.Show("Выберите специальность", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbTarget.Focus();
                    return false;
                }
            }
            else
            {
                // Для отделения и врача проверяем SelectedValue
                if (cmbTarget.SelectedValue == null)
                {
                    MessageBox.Show("Выберите цель назначения", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbTarget.Focus();
                    return false;
                }
            }

            if (dtpDeadline.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Дедлайн не может быть в прошлом", "Ошибка валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDeadline.Focus();
                return false;
            }

            return true;
        }
    }

}
