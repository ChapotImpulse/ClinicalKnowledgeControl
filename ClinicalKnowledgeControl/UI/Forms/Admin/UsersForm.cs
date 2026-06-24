using ClinicalKnowledgeControl.BLL.Models;
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

namespace ClinicalKnowledgeControl.UI.Forms.Admin
{
    public partial class UsersForm : Form
    {
        private readonly UserService _service;
        private readonly User _currentUser;

        public UsersForm(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _service = new UserService();

            chkShowInactive.CheckedChanged += (s, e) => LoadData();

            LoadData();
        }

        private void LoadData()
        {
            var data = _service.GetAll(chkShowInactive.Checked);
            dgv.DataSource = data;

            dgv.Columns[0].Visible = false;
            dgv.Columns[1].HeaderText = "ФИО";
            dgv.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[2].HeaderText = "Логин";
            dgv.Columns[3].HeaderText = "Отделение";
            dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[4].HeaderText = "Роль";
            dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[5].HeaderText = "Специальность";
            dgv.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[5].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns[6].HeaderText = "Активен";
            dgv.Columns[7].HeaderText = "Дата добавления";
            dgv.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new UsersEditForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string passwordHash = PasswordHasher.HashPassword(dlg.Password);
                        _service.CreateUser(
                            dlg.FullName,
                            dlg.DepartmentId,
                            dlg.RoleId,
                            dlg.Specialty,
                            dlg.Login,
                            passwordHash
                        );
                        MessageBox.Show("Пользователь создан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int userId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            User user = _service.GetById(userId);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dlg = new UsersEditForm(user))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _service.UpdateUser(
                            userId,
                            dlg.FullName,
                            dlg.DepartmentId,
                            dlg.RoleId,
                            dlg.Specialty,
                            dlg.Login
                        );
                        MessageBox.Show("Пользователь обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int userId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);

            using (var dlg = new ChangePasswordForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string passwordHash = PasswordHasher.HashPassword(dlg.NewPassword);
                        _service.ChangePassword(userId, passwordHash);
                        MessageBox.Show("Пароль изменён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnToggleActive_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int userId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            bool currentActive = Convert.ToBoolean(dgv.SelectedRows[0].Cells["IsActive"].Value);
            bool newActive = !currentActive;

            string action = newActive ? "активировать" : "деактивировать";
            if (MessageBox.Show($"Вы уверены, что хотите {action} пользователя?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _service.SetActive(userId, newActive);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int userId = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            string fullName = dgv.SelectedRows[0].Cells["FullName"].Value?.ToString();

            if (MessageBox.Show($"Вы уверены, что хотите УДАЛИТЬ пользователя \"{fullName}\"?\n" +
                                "Это действие нельзя отменить.",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _service.DeleteUser(userId, _currentUser.Id);
                    MessageBox.Show("Пользователь удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Невозможно удалить", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
                btnToggleActive.Text = Convert.ToBoolean(dgv.SelectedRows[0].Cells["IsActive"].Value)
                    ? "Деактивировать" : "Активировать";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
