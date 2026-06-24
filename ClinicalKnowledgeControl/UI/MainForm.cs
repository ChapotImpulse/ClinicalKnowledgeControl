using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.Common.Enums;
using ClinicalKnowledgeControl.UI.Forms.Admin;
using ClinicalKnowledgeControl.UI.Forms.Doctor;
using ClinicalKnowledgeControl.UI.Forms.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicalKnowledgeControl.UI
{
    public partial class MainForm : Form
    {
        private readonly User _currentUser;

        public MainForm(User user)
        {
            InitializeComponent();

            _currentUser = user;
            InitializeMenu();
            lblUserInfo.Text = $"{user.FullName} ({user.RoleName})";
        }

        private void InitializeMenu()
        {
            var menuStrip = new MenuStrip();

            // Меню для врача
            if (_currentUser.RoleId == (int)UserRole.Doctor)
            {
                var doctorMenu = new ToolStripMenuItem("Тестирование");
                doctorMenu.DropDownItems.Add("Мои задания", null, OpenDashboard);
                doctorMenu.DropDownItems.Add("История попыток", null, OpenHistory);
                menuStrip.Items.Add(doctorMenu);
            }

            // Меню для заведующего отделением
            if (_currentUser.RoleId == (int)UserRole.DepartmentHead)
            {
                var dreportMenu = new ToolStripMenuItem("Отчеты");
                dreportMenu.DropDownItems.Add("Отчет по отделению", null, OpenDepartmentReport);
                menuStrip.Items.Add(dreportMenu);
            }

            // Меню для зам. главврача
            if (_currentUser.RoleId == (int)UserRole.DeputyChiefDoctor)
            {
                var zreportMenu = new ToolStripMenuItem("Отчеты");
                zreportMenu.DropDownItems.Add("Отчет по учреждению", null, OpenInstitutionReport);
                menuStrip.Items.Add(zreportMenu);
            }

            // Меню ГВС
            if (_currentUser.RoleId == (int)UserRole.GVS)
            {
                var adminMenu = new ToolStripMenuItem("Справочники");
                adminMenu.DropDownItems.Add("Клинические рекомендации", null, OpenClinicalGuidelines);
                adminMenu.DropDownItems.Add("Банк вопросов", null, OpenQuestionsBank);
                menuStrip.Items.Add(adminMenu);
            }

            // Меню для администратора
            if (_currentUser.RoleId == (int)UserRole.Administrator)
            {
                var adminMenu = new ToolStripMenuItem("Администрирование");
                adminMenu.DropDownItems.Add("Клинические рекомендации", null, OpenClinicalGuidelines);
                adminMenu.DropDownItems.Add("Банк вопросов", null, OpenQuestionsBank);
                adminMenu.DropDownItems.Add("Конструктор тестов", null, OpenTestConstructor);
                adminMenu.DropDownItems.Add("Назначение тестов", null, OpenAssignments);
                adminMenu.DropDownItems.Add("Пользователи", null, OpenUsers);
                menuStrip.Items.Add(adminMenu);

                var reportMenu = new ToolStripMenuItem("Отчеты");
                reportMenu.DropDownItems.Add("Глобальная статистика", null, OpenInstitutionReport);
                menuStrip.Items.Add(reportMenu);
            }

            // Меню выхода
            var exitMenu = new ToolStripMenuItem("Выход");
            exitMenu.Click += (s, e) => Application.Exit();
            menuStrip.Items.Add(exitMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void OpenDashboard(object sender, EventArgs e)
        {
            OpenChildForm(new DashboardForm(_currentUser));
        }

        private void OpenHistory(object sender, EventArgs e)
        {
            OpenChildForm(new HistoryForm(_currentUser.Id));
        }

        private void OpenDepartmentReport(object sender, EventArgs e)
        {
            OpenChildForm(new DepartmentReportForm(_currentUser.DepartmentId.Value));
        }

        private void OpenInstitutionReport(object sender, EventArgs e)
        {
            OpenChildForm(new InstitutionReportForm());
        }

        private void OpenClinicalGuidelines(object sender, EventArgs e)
        {
            OpenChildForm(new ClinicalGuidelinesForm());
        }

        private void OpenQuestionsBank(object sender, EventArgs e)
        {
            OpenChildForm(new QuestionsBankForm(_currentUser));
        }

        private void OpenTestConstructor(object sender, EventArgs e)
        {
            OpenChildForm(new TestConstructorForm(_currentUser));
        }

        private void OpenAssignments(object sender, EventArgs e)
        {
            OpenChildForm(new AssignmentsForm(_currentUser));
        }

        private void OpenUsers(object sender, EventArgs e)
        {
            OpenChildForm(new UsersForm(_currentUser));
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                childForm.StartPosition = FormStartPosition.CenterScreen;
                childForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
