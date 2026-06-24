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
    public partial class UsersEditForm : Form
    {
        public string FullName { get; private set; }
        public int? DepartmentId { get; private set; }
        public int RoleId { get; private set; }
        public string Specialty { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        public UsersEditForm()
        {
            InitializeComponent();
            LoadLookups();

            this.Text = "Добавить нового пользователя...";
        }

        public UsersEditForm(User user) : this()
        {
            this.Text = "Редактировать пользователя...";

            if (user == null) return;

            this.Text = $"Редактирование пользователя #{user.Id}";

            txtFullName.Text = user.FullName;
            txtSpecialty.Text = user.Specialty ?? "";
            txtLogin.Text = user.Login;

            if (user.DepartmentId.HasValue)
                cmbDepartment.SelectedValue = user.DepartmentId.Value;

            cmbRole.SelectedValue = user.RoleId;

            // В режиме редактирования поле пароля скрываем
            txtPassword.Visible = false;
            lblPassword.Visible = false;
        }

        private void LoadLookups()
        {
            var service = new UserService();

            var departments = service.GetDepartments();
            cmbDepartment.DataSource = departments;
            cmbDepartment.DisplayMember = "Name";
            cmbDepartment.ValueMember = "Id";
            cmbDepartment.SelectedIndex = 0;
            
            var roles = service.GetRoles();
            cmbRole.DataSource = roles;
            cmbRole.DisplayMember = "Name";
            cmbRole.ValueMember = "Id";
            cmbRole.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                cmbRole.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Заполните обязательные поля (ФИО, Роль, Логин)", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Visible && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FullName = txtFullName.Text;
            DepartmentId = cmbDepartment.SelectedValue as int?;
            RoleId = Convert.ToInt32(cmbRole.SelectedValue);
            Specialty = txtSpecialty.Text;
            Login = txtLogin.Text;
            Password = txtPassword.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
