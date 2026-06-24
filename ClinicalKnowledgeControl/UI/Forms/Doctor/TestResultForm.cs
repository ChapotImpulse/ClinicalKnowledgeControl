using ClinicalKnowledgeControl.BLL.Models;
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
    public partial class TestResultForm : Form
    {
        private readonly decimal _score;
        private readonly bool _isPassed;
        private readonly List<Question> _questions;

        public TestResultForm(decimal score, bool isPassed, List<Question> questions)
        {
            InitializeComponent();
            _score = score;
            _isPassed = isPassed;
            _questions = questions;
            InitializeUI();
        }

        private void InitializeUI()
        {
            lblResult.Text = _isPassed ? "ТЕСТ СДАН!" : "ТЕСТ НЕ СДАН";
            lblResult.ForeColor = _isPassed ? Color.Green : Color.Red;
            lblScore.Text = $"Ваш результат: {_score:F2}%";            
            
            var reviewText = "";
            for (int i = 0; i < _questions.Count; i++)
            {
                var q = _questions[i];
                reviewText += $"Вопрос {i + 1}:\r\n{q.Text}\r\n";
                if (!string.IsNullOrEmpty(q.Explanation))
                {
                    reviewText += $"Пояснение: {q.Explanation}\r\n";
                }
                reviewText += new string('-', 50) + "\r\n";
            }
            txtReview.Text = reviewText;

            btnClose.Click += (s, e) => this.Close();
        }
    }
}
