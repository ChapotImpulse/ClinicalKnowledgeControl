using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Services
{
    public class TestTemplateService
    {
        private readonly TestTemplateRepository _repository;

        public TestTemplateService()
        {
            _repository = new TestTemplateRepository();
        }

        public DataTable GetAll()
        {
            return _repository.GetAll();
        }

        /// <summary>
        /// Получить шаблон теста по Id
        /// </summary>
        public TestTemplate GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Получить активный шаблон теста по Id
        /// </summary>
        public TestTemplate GetActiveById(int id)
        {
            return _repository.GetActiveById(id);
        }

        /// <summary>
        /// Создать новый шаблон теста
        /// </summary>
        /// <param name="createdByUserId">ID пользователя, создавшего шаблон</param>
        public int Create(string name, int clinicalGuidelineId, int questionCount,
            int timeLimitMinutes, decimal passingScore, int maxAttempts, int createdByUserId)
        {
            Validate(name, clinicalGuidelineId, questionCount, timeLimitMinutes, passingScore, maxAttempts);
            return _repository.Insert(name, clinicalGuidelineId, questionCount,
                timeLimitMinutes, passingScore, maxAttempts, createdByUserId);
        }

        /// <summary>
        /// Обновить шаблон теста
        /// </summary>
        public void UpdateTemplate(int id, string name, int clinicalGuidelineId, int questionCount,
            int timeLimitMinutes, decimal passingScore, int maxAttempts, int updatedByUserId)
        {
            // Проверяем существование шаблона
            var existing = _repository.GetById(id);
            if (existing == null)
            {
                throw new InvalidOperationException($"Шаблон теста с Id={id} не найден");
            }

            // Валидируем новые данные
            Validate(name, clinicalGuidelineId, questionCount, timeLimitMinutes, passingScore, maxAttempts);

            _repository.Update(id, name, clinicalGuidelineId, questionCount,
                timeLimitMinutes, passingScore, maxAttempts, updatedByUserId);
        }

        /// <summary>
        /// Мягкое удаление шаблона теста
        /// </summary>
        public void DeleteTemplate(int id)
        {
            _repository.Delete(id);
        }

        /// <summary>
        /// Валидация параметров шаблона теста
        /// </summary>
        private void Validate(string name, int clinicalGuidelineId, int questionCount,
            int timeLimitMinutes, decimal passingScore, int maxAttempts)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название теста не может быть пустым");
            }

            if (clinicalGuidelineId <= 0)
            {
                throw new ArgumentException("Необходимо выбрать клиническую рекомендацию");
            }

            if (questionCount <= 0)
            {
                throw new ArgumentException("Количество вопросов должно быть больше 0");
            }

            // Проверяем, достаточно ли вопросов в базе для данной КР
            int availableCount = _repository.GetAvailableQuestionsCount(clinicalGuidelineId);
            if (questionCount > availableCount)
            {
                throw new ArgumentException(
                    $"Для данной клинической рекомендации доступно только {availableCount} вопросов, " +
                    $"а запрашивается {questionCount}");
            }

            if (timeLimitMinutes <= 0 || timeLimitMinutes > 300)
            {
                throw new ArgumentException("Лимит времени должен быть от 1 до 300 минут");
            }

            if (passingScore <= 0 || passingScore > 100)
            {
                throw new ArgumentException("Проходной балл должен быть от 1 до 100 процентов");
            }

            if (maxAttempts <= 0 || maxAttempts > 10)
            {
                throw new ArgumentException("Количество попыток должно быть от 1 до 10");
            }
        }
    }
}
