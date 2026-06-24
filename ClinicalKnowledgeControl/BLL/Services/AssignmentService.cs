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
    public class AssignmentService
    {
        private readonly AssignmentRepository _repository;

        public AssignmentService()
        {
            _repository = new AssignmentRepository();
        }

        // ========================
        // Назначение тестов
        // ========================
        public void AssignToDepartment(int testTemplateId, int departmentId, DateTime deadline)
        {
            ValidateAssignment(testTemplateId, deadline);

            if (_repository.IsAssignmentExists(testTemplateId, 1, departmentId))
            {
                throw new InvalidOperationException("Такое назначение уже существует для данного отделения");
            }

            _repository.MassAssignToDepartment(testTemplateId, departmentId, deadline);
        }

        public void AssignBySpecialty(int testTemplateId, string specialty, DateTime deadline)
        {
            ValidateAssignment(testTemplateId, deadline);

            if (string.IsNullOrWhiteSpace(specialty))
            {
                throw new ArgumentException("Необходимо указать специальность");
            }

            _repository.MassAssignBySpecialty(testTemplateId, specialty, deadline);
        }

        public void AssignToDoctor(int testTemplateId, int userId, DateTime deadline)
        {
            ValidateAssignment(testTemplateId, deadline);

            if (_repository.IsAssignmentExists(testTemplateId, 3, userId))
            {
                throw new InvalidOperationException("Такое назначение уже существует для данного врача");
            }

            _repository.Insert(testTemplateId, 3, userId, deadline);
        }

        public DataTable GetAll()
        {
            return _repository.GetAll();
        }

        public DataTable GetDepartments()
        {
            return _repository.GetDepartments();
        }

        /// <summary>
        /// Получить назначение по Id
        /// </summary>
        public Assignment GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Получить активное назначение по Id
        /// </summary>
        public Assignment GetActiveById(int id)
        {
            return _repository.GetActiveById(id);
        }

        /// <summary>
        /// Обновить назначение теста
        /// </summary>
        public void UpdateAssignment(int id, int testTemplateId, int targetType, int targetId, DateTime deadline)
        {
            // Проверяем существование
            var existing = _repository.GetById(id);
            if (existing == null)
            {
                throw new InvalidOperationException($"Назначение с Id={id} не найдено");
            }

            // Валидация
            ValidateAssignment(testTemplateId, deadline);

            if (targetType < 1 || targetType > 3)
            {
                throw new ArgumentException("Некорректный тип цели назначения");
            }

            if (targetId <= 0)
            {
                throw new ArgumentException("Необходимо выбрать цель назначения");
            }

            // Проверяем, не создаём ли дубликат
            if (_repository.IsAssignmentExists(testTemplateId, targetType, targetId))
            {
                // Исключаем само назначение из проверки
                var duplicate = _repository.GetById(id);
                if (duplicate.TestTemplateId != testTemplateId ||
                    duplicate.TargetType != targetType ||
                    duplicate.TargetId != targetId)
                {
                    throw new InvalidOperationException("Такое назначение уже существует");
                }
            }

            _repository.Update(id, testTemplateId, targetType, targetId, deadline);
        }

        /// <summary>
        /// Обновить только дедлайн назначения
        /// </summary>
        public void UpdateDeadline(int id, DateTime newDeadline)
        {
            if (newDeadline < DateTime.Today)
            {
                throw new ArgumentException("Дедлайн не может быть в прошлом");
            }

            _repository.UpdateDeadline(id, newDeadline);
        }

        /// <summary>
        /// Мягкое удаление назначения
        /// </summary>
        public void DeleteAssignment(int id)
        {
            _repository.Delete(id);
        }

        /// <summary>
        /// Жёсткое удаление назначения
        /// </summary>
        public void HardDeleteAssignment(int id)
        {
            _repository.HardDelete(id);
        }

        /// <summary>
        /// Проверить существование назначения
        /// </summary>
        public bool Exists(int id)
        {
            return _repository.Exists(id);
        }

        /// <summary>
        /// Получить назначения для конкретного врача
        /// </summary>
        public DataTable GetAssignmentsForDoctor(int userId)
        {
            return _repository.GetAssignmentsForDoctor(userId);
        }

        // ========================
        // Валидация
        // ========================
        private void ValidateAssignment(int testTemplateId, DateTime deadline)
        {
            if (testTemplateId <= 0)
            {
                throw new ArgumentException("Необходимо выбрать шаблон теста");
            }

            if (deadline < DateTime.Today)
            {
                throw new ArgumentException("Дедлайн не может быть в прошлом");
            }

            if (deadline > DateTime.Today.AddYears(1))
            {
                throw new ArgumentException("Дедлайн не может быть более чем через год");
            }
        }
    }
}
