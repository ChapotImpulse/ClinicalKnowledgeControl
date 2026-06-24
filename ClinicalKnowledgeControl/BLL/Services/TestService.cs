using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.Common.Enums;
using ClinicalKnowledgeControl.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Services
{
    public class TestService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly TestRepository _testRepository;

        public TestService()
        {
            _questionRepository = new QuestionRepository();
            _testRepository = new TestRepository();
        }

        public bool CanStartTest(int userId, int testTemplateId)
        {
            var template = _testRepository.GetTestTemplate(testTemplateId);
            if (template == null) return false;

            return _testRepository.CanStartAttempt(userId, testTemplateId, template.MaxAttempts);
        }

        public List<Question> GetQuestionsForTest(int testTemplateId)
        {
            var template = _testRepository.GetTestTemplate(testTemplateId);
            if (template == null) return new List<Question>();

            return _questionRepository.GetRandomQuestionsForTest(testTemplateId, template.QuestionCount);
        }

        public int StartTest(int userId, int testTemplateId, int? assignmentId)
        {
            return _testRepository.CreateTestAttempt(userId, testTemplateId, assignmentId);
        }

        public void SaveAnswer(int attemptId, Question question, List<int> selectedOptionIds, int timeSpentSeconds)
        {
            bool isCorrect = CheckAnswer(question, selectedOptionIds);
            string selectedOptions = string.Join(",", selectedOptionIds);

            _testRepository.SaveAnswer(attemptId, question.Id, selectedOptions, isCorrect, timeSpentSeconds);
        }

        private bool CheckAnswer(Question question, List<int> selectedOptionIds)
        {
            switch (question.QuestionType)
            {
                case QuestionType.SingleChoice:
                    if (selectedOptionIds.Count != 1) return false;
                    var option = question.Options.FirstOrDefault(o => o.Id == selectedOptionIds[0]);
                    return option != null && option.IsCorrect;

                case QuestionType.MultipleChoice:
                    var correctOptions = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).OrderBy(id => id).ToList();
                    var selected = selectedOptionIds.OrderBy(id => id).ToList();
                    return correctOptions.SequenceEqual(selected);

                default:
                    return false;
            }
        }

        public (decimal score, bool isPassed, int status) FinishTest(int attemptId, int testTemplateId, int totalQuestions, int correctAnswers, bool isTimeOut)
        {
            var template = _testRepository.GetTestTemplate(testTemplateId);
            if (template == null) throw new Exception("Шаблон теста не найден");

            decimal score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
            bool isPassed = score >= template.PassingScore;

            int status;
            if (isTimeOut)
                status = (int)TestStatus.TimeOut;
            else if (isPassed)
                status = (int)TestStatus.Passed;
            else
                status = (int)TestStatus.Failed;

            _testRepository.FinishTestAttempt(attemptId, score, status, isPassed);

            return (score, isPassed, status);
        }
    }
}
