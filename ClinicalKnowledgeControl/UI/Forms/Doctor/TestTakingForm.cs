using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.BLL.Services;
using ClinicalKnowledgeControl.Common.Enums;
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

namespace ClinicalKnowledgeControl.UI.Forms.Doctor
{
    public partial class TestTakingForm : Form
    {
        private readonly int _userId;
        private readonly int _testTemplateId;
        private readonly int? _assignmentId;
        private readonly TestService _testService;

        private List<Question> _questions;
        private int _currentQuestionIndex;
        private int _attemptId;
        private int _correctAnswers;
        private System.Windows.Forms.Timer _testTimer;
        private int _timeLeftInSeconds;
        private DateTime _questionStartTime;

        public TestTakingForm(int userId, int testTemplateId, int? assignmentId)
        {
            InitializeComponent();
            _userId = userId;
            _testTemplateId = testTemplateId;
            _assignmentId = assignmentId;
            _testService = new TestService();

            this.KeyPreview = true; // Для перехвата горячих клавиш
            InitializeTest();
        }

        private void InitializeTest()
        {
            try
            {
                _questions = _testService.GetQuestionsForTest(_testTemplateId);
                if (_questions == null || _questions.Count == 0)
                {
                    MessageBox.Show("Не удалось загрузить вопросы теста", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                _attemptId = _testService.StartTest(_userId, _testTemplateId, _assignmentId);
                _currentQuestionIndex = 0;
                _correctAnswers = 0;

                var template = new TestRepository().GetTestTemplate(_testTemplateId);
                _timeLeftInSeconds = template.TimeLimitMinutes * 60;

                StartTimer();
                ShowCurrentQuestion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации теста: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void StartTimer()
        {
            _testTimer = new System.Windows.Forms.Timer();
            _testTimer.Interval = 1000;
            _testTimer.Tick += testTimer_Tick;
            _testTimer.Start();
            UpdateTimerLabel();
        }

        private void testTimer_Tick(object sender, EventArgs e)
        {
            _timeLeftInSeconds--;
            UpdateTimerLabel();

            if (_timeLeftInSeconds <= 0)
            {
                _testTimer.Stop();
                MessageBox.Show("Время вышло! Тест автоматически завершен.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FinishTest(isTimeOut: true);
            }
        }

        private void UpdateTimerLabel()
        {
            lblTimer.Text = $"Осталось времени: {TimeSpan.FromSeconds(_timeLeftInSeconds):mm\\:ss}";
            lblTimer.Refresh();
        }

        private void ShowCurrentQuestion()
        {
            if (_currentQuestionIndex >= _questions.Count)
            {
                FinishTest(isTimeOut: false);
                return;
            }

            var question = _questions[_currentQuestionIndex];
            lblProgress.Text = $"Вопрос {_currentQuestionIndex + 1} из {_questions.Count}";
            txtQuestion.Text = question.Text;

            // Очищаем предыдущие варианты ответов
            panelOptions.Controls.Clear();

            _questionStartTime = DateTime.Now;

            // Создаем элементы управления для вариантов ответов
            switch (question.QuestionType)
            {
                case QuestionType.SingleChoice:
                    CreateSingleChoiceOptions(question);
                    break;
                case QuestionType.MultipleChoice:
                    CreateMultipleChoiceOptions(question);
                    break;
            }
        }

        private void CreateSingleChoiceOptions(Question question)
        {
            int yPos = 10;
            foreach (var option in question.Options)
            {
                var radioButton = new SecureRadioButton
                {
                    Text = option.Text,
                    Tag = option.Id,
                    Location = new Point(10, yPos),
                    AutoSize = true
                };
                panelOptions.Controls.Add(radioButton);
                yPos += 30;
            }
        }

        private void CreateMultipleChoiceOptions(Question question)
        {
            int yPos = 10;
            foreach (var option in question.Options)
            {
                var checkBox = new SecureCheckBox
                {
                    Text = option.Text,
                    Tag = option.Id,
                    Location = new Point(10, yPos),
                    AutoSize = true
                };
                panelOptions.Controls.Add(checkBox);
                yPos += 30;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var question = _questions[_currentQuestionIndex];
            var selectedOptions = GetSelectedOptions(question.QuestionType);

            if (selectedOptions.Count == 0)
            {
                MessageBox.Show("Выберите ответ", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int timeSpent = (int)(DateTime.Now - _questionStartTime).TotalSeconds;

            // Проверяем правильность ответа
            bool isCorrect = CheckAnswer(question, selectedOptions);
            if (isCorrect) _correctAnswers++;

            // Сохраняем ответ
            _testService.SaveAnswer(_attemptId, question, selectedOptions, timeSpent);

            _currentQuestionIndex++;
            ShowCurrentQuestion();
        }

        private List<int> GetSelectedOptions(QuestionType questionType)
        {
            var selected = new List<int>();

            switch (questionType)
            {
                case QuestionType.SingleChoice:
                    foreach (Control ctrl in panelOptions.Controls)
                    {
                        if (ctrl is SecureRadioButton rb && rb.Checked)
                        {
                            selected.Add((int)rb.Tag);
                            break;
                        }
                    }
                    break;

                case QuestionType.MultipleChoice:
                    foreach (Control ctrl in panelOptions.Controls)
                    {
                        if (ctrl is SecureCheckBox cb && cb.Checked)
                        {
                            selected.Add((int)cb.Tag);
                        }
                    }
                    break;
            }

            return selected;
        }

        private bool CheckAnswer(Question question, List<int> selectedOptions)
        {
            // Логика проверки аналогична TestService.CheckAnswer
            switch (question.QuestionType)
            {
                case QuestionType.SingleChoice:
                    if (selectedOptions.Count != 1) return false;
                    var option = question.Options.FirstOrDefault(o => o.Id == selectedOptions[0]);
                    return option != null && option.IsCorrect;

                case QuestionType.MultipleChoice:
                    var correctOptions = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).OrderBy(id => id).ToList();
                    var selected = selectedOptions.OrderBy(id => id).ToList();
                    return correctOptions.SequenceEqual(selected);

                default:
                    return false;
            }
        }

        private void FinishTest(bool isTimeOut)
        {
            _testTimer?.Stop();

            var (score, isPassed, status) = _testService.FinishTest(
                _attemptId,
                _testTemplateId,
                _questions.Count,
                _correctAnswers,
                isTimeOut
            );

            this.Hide();
            var resultForm = new TestResultForm(score, isPassed, _questions);
            resultForm.FormClosed += (s, args) => this.Close();
            resultForm.Show();
        }

        // Защита от копирования
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C) ||
                keyData == (Keys.Control | Keys.V) ||
                keyData == (Keys.Control | Keys.X) ||
                keyData == (Keys.Control | Keys.A) ||
                keyData == Keys.PrintScreen)
            {
                return true; // Блокируем
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0204) // WM_CONTEXTMENU
                return;
            base.WndProc(ref m);
        }
    }

    // Защищенные элементы управления
    /// <summary>
    /// Защищенный RadioButton с блокировкой копирования текста
    /// </summary>
    public class SecureRadioButton : RadioButton
    {
        public SecureRadioButton()
        {
            // Отключаем стандартное поведение выделения
            this.TabStop = false;
        }

        /// <summary>
        /// Блокировка горячих клавиш копирования/выделения
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Блокируем Ctrl+C, Ctrl+X, Ctrl+A, Ctrl+V, PrintScreen
            if (keyData == (Keys.Control | Keys.C) ||
                keyData == (Keys.Control | Keys.X) ||
                keyData == (Keys.Control | Keys.A) ||
                keyData == (Keys.Control | Keys.V) ||
                keyData == Keys.PrintScreen)
            {
                return true; // Подавляем нажатие
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Блокировка контекстного меню (правый клик)
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            const int WM_CONTEXTMENU = 0x007B;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_RBUTTONUP = 0x0205;

            // Блокируем правую кнопку мыши и контекстное меню
            if (m.Msg == WM_CONTEXTMENU || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_RBUTTONUP)
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Предотвращение выделения текста через клавиатуру
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control || e.KeyCode == Keys.PrintScreen)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>
    /// Защищенный CheckBox с блокировкой копирования текста
    /// </summary>
    public class SecureCheckBox : CheckBox
    {
        public SecureCheckBox()
        {
            this.TabStop = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C) ||
                keyData == (Keys.Control | Keys.X) ||
                keyData == (Keys.Control | Keys.A) ||
                keyData == (Keys.Control | Keys.V) ||
                keyData == Keys.PrintScreen)
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_CONTEXTMENU = 0x007B;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_RBUTTONUP = 0x0205;

            if (m.Msg == WM_CONTEXTMENU || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_RBUTTONUP)
            {
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control || e.KeyCode == Keys.PrintScreen)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>
    /// Защищенный Label с блокировкой копирования текста
    /// </summary>
    public class SecureLabel : Label
    {
        public SecureLabel()
        {
            this.TabStop = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C) ||
                keyData == (Keys.Control | Keys.X) ||
                keyData == (Keys.Control | Keys.A) ||
                keyData == (Keys.Control | Keys.V) ||
                keyData == Keys.PrintScreen)
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_CONTEXTMENU = 0x007B;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_RBUTTONUP = 0x0205;

            if (m.Msg == WM_CONTEXTMENU || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_RBUTTONUP)
            {
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control || e.KeyCode == Keys.PrintScreen)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            base.OnKeyDown(e);
        }
    }
}
