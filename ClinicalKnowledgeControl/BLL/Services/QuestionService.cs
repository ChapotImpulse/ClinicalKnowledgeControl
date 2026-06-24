using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.Common.Enums;
using ClinicalKnowledgeControl.DAL;
using ClinicalKnowledgeControl.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _repository;

        public QuestionService()
        {
            _repository = new QuestionRepository();
        }

        public DataTable GetAllQuestions(int? clinicalGuidelineId = null)
        {
            return _repository.GetAll(clinicalGuidelineId);
        }

        /// <summary>
        /// Получить вопрос по Id с вариантами ответов
        /// </summary>
        /// <param name="id">Идентификатор вопроса</param>
        /// <returns>Объект Question с заполненными Options или null</returns>
        public Question GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Получить базовую информацию о вопросе (без вариантов ответов)
        /// </summary>
        public Question GetBasicInfoById(int id)
        {
            return _repository.GetBasicInfoById(id);
        }

        /// <summary>
        /// Получить вопрос по Id (включая неактивные) — для администратора
        /// </summary>
        public Question GetByIdIncludeInactive(int id)
        {
            return _repository.GetByIdIncludeInactive(id);
        }

        /// <summary>
        /// Проверить существование вопроса
        /// </summary>
        public bool Exists(int id)
        {
            return _repository.GetById(id) != null;
        }

        public int CreateQuestion(int clinicalGuidelineId, string text, int questionType,
            string explanation, string tags, List<(string text, bool isCorrect, int? order)> options,
            int createdByUserId)
        {
            int questionId = _repository.Insert(clinicalGuidelineId, text, questionType,
                explanation, tags, createdByUserId);

            foreach (var option in options)
            {
                _repository.InsertOption(questionId, option.text, option.isCorrect, option.order);
            }

            return questionId;
        }

        public void DeleteQuestion(int questionId)
        {
            _repository.Delete(questionId);
        }

        public void UpdateQuestion(int id, int clinicalGuidelineId, string text, int questionType,
            string explanation, string tags, List<(string text, bool isCorrect, int? order)> options,
            int updatedByUserId)
        {
            // Валидация входных данных
            ValidateQuestion(text, questionType, options);

            // Обновляем вопрос с вариантами ответов (транзакционно)
            _repository.UpdateWithOptions(id, clinicalGuidelineId, text, questionType,
                explanation, tags, updatedByUserId, options);
        }

        /// <summary>
        /// Обновить только базовую информацию о вопросе (без вариантов ответов)
        /// </summary>
        public void UpdateQuestionBasicInfo(int id, int clinicalGuidelineId, string text,
            int questionType, string explanation, string tags, int updatedByUserId)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Текст вопроса не может быть пустым");
            }

            _repository.Update(id, clinicalGuidelineId, text, questionType,
                explanation, tags, updatedByUserId);
        }

        /// <summary>
        /// Валидация данных вопроса
        /// </summary>
        private void ValidateQuestion(string text, int questionType, List<(string text, bool isCorrect, int? order)> options)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Текст вопроса не может быть пустым");
            }

            if (options == null || options.Count < 2)
            {
                throw new ArgumentException("Вопрос должен содержать минимум 2 варианта ответа");
            }

            // Проверка для одиночного выбора - должен быть ровно 1 правильный ответ
            if (questionType == (int)QuestionType.SingleChoice)
            {
                int correctCount = 0;
                foreach (var option in options)
                {
                    if (option.isCorrect) correctCount++;
                }

                if (correctCount != 1)
                {
                    throw new ArgumentException("Для вопроса с одиночным выбором должен быть ровно 1 правильный ответ");
                }
            }

            // Проверка для множественного выбора - должен быть хотя бы 1 правильный ответ
            if (questionType == (int)QuestionType.MultipleChoice)
            {
                bool hasCorrect = false;
                foreach (var option in options)
                {
                    if (option.isCorrect)
                    {
                        hasCorrect = true;
                        break;
                    }
                }

                if (!hasCorrect)
                {
                    throw new ArgumentException("Для вопроса с множественным выбором должен быть хотя бы 1 правильный ответ");
                }
            }
        }
    }
}
