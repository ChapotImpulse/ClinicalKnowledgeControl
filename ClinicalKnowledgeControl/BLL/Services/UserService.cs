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
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService()
        {
            _repository = new UserRepository();
        }

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        public DataTable GetAll(bool includeInactive = false)
        {
            return _repository.GetAll(includeInactive);
        }

        /// <summary>
        /// Получить пользователя по Id
        /// </summary>
        public User GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Получить активного пользователя по Id
        /// </summary>
        public User GetActiveById(int id)
        {
            return _repository.GetActiveById(id);
        }

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        public int CreateUser(string fullName, int? departmentId, int roleId, string specialty,
            string login, string passwordHash)
        {
            ValidateUser(fullName, roleId, login);
            ValidatePasswordHash(passwordHash);

            return _repository.Insert(fullName, departmentId, roleId, specialty, login, passwordHash);
        }

        /// <summary>
        /// Обновить данные пользователя
        /// </summary>
        public void UpdateUser(int id, string fullName, int? departmentId, int roleId,
            string specialty, string login)
        {
            ValidateUser(fullName, roleId, login);
            _repository.Update(id, fullName, departmentId, roleId, specialty, login);
        }

        /// <summary>
        /// Изменить пароль пользователя
        /// </summary>
        public void ChangePassword(int id, string newPasswordHash)
        {
            ValidatePasswordHash(newPasswordHash);
            _repository.ChangePassword(id, newPasswordHash);
        }

        /// <summary>
        /// Активировать/деактивировать пользователя
        /// </summary>
        public void SetActive(int id, bool isActive)
        {
            _repository.SetActive(id, isActive);
        }

        /// <summary>
        /// Мягкое удаление пользователя
        /// </summary>
        public void DeleteUser(int id, int currentUserId)
        {
            _repository.Delete(id, currentUserId);
        }

        /// <summary>
        /// Жёсткое удаление пользователя
        /// </summary>
        public void HardDeleteUser(int id, int currentUserId)
        {
            _repository.HardDelete(id, currentUserId);
        }

        /// <summary>
        /// Проверить, существует ли пользователь
        /// </summary>
        public bool Exists(int id)
        {
            return _repository.Exists(id);
        }

        /// <summary>
        /// Проверить, занят ли логин
        /// </summary>
        public bool IsLoginExists(string login)
        {
            return _repository.IsLoginExists(login);
        }

        /// <summary>
        /// Получить список ролей
        /// </summary>
        public DataTable GetRoles()
        {
            return _repository.GetRoles();
        }

        /// <summary>
        /// Получить список отделений
        /// </summary>
        public DataTable GetDepartments()
        {
            return _repository.GetDepartments();
        }

        /// <summary>
        /// Получить список врачей
        /// </summary>
        public DataTable GetAllDoctors(int? departmentId = null)
        {
            return _repository.GetAllDoctors(departmentId);
        }

        // ========================
        // Валидация
        // ========================

        private void ValidateUser(string fullName, int roleId, string login)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("ФИО не может быть пустым");
            }

            if (fullName.Length > 200)
            {
                throw new ArgumentException("ФИО не может превышать 200 символов");
            }

            if (roleId <= 0)
            {
                throw new ArgumentException("Необходимо выбрать роль");
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("Логин не может быть пустым");
            }

            if (login.Length < 3 || login.Length > 50)
            {
                throw new ArgumentException("Логин должен быть от 3 до 50 символов");
            }

            // Проверка допустимых символов в логине
            foreach (char c in login)
            {
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                {
                    throw new ArgumentException(
                        "Логин может содержать только буквы, цифры, символы подчёркивания и точки");
                }
            }
        }

        private void ValidatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Пароль не может быть пустым");
            }
        }
    }
}
