using ClinicalKnowledgeControl.BLL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.DAL.Repositories
{
    public class UserRepository
    {
        public User Authenticate(string login, string passwordHash)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT u.Id, u.FullName, u.DepartmentId, u.RoleId, u.Specialty, u.Login, u.CreatedDate,
                           d.Name as DepartmentName, r.Name as RoleName, u.PasswordHash, u.IsActive
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.Login = @Login AND u.PasswordHash = @PasswordHash AND u.IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToUser(reader);
                        }
                    }
                }
            }
            return null;
        }

        public DataTable GetAllDoctors(int? departmentId = null)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT u.Id, u.FullName, u.Specialty, d.Name as DepartmentName
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    WHERE u.RoleId = 1";

                if (departmentId.HasValue)
                {
                    query += " AND u.DepartmentId = @DepartmentId";
                }

                query += " AND u.IsActive = 1 ORDER BY u.FullName";

                using (var cmd = new SqlCommand(query, conn))
                {
                    if (departmentId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@DepartmentId", departmentId.Value);
                    }

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Получить список всех пользователей (для администратора)
        /// </summary>
        public DataTable GetAll(bool includeInactive = false)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        u.Id, 
                        u.FullName, 
                        u.Login,
                        d.Name AS DepartmentName,
                        r.Name AS RoleName,
                        u.Specialty,
                        u.IsActive,
                        u.CreatedDate
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    INNER JOIN Roles r ON u.RoleId = r.Id";

                if (!includeInactive)
                {
                    query += " WHERE u.IsActive = 1";
                }

                query += " ORDER BY u.FullName";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Получить пользователя по Id
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Объект User или null, если не найден</returns>
        public User GetById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        u.Id, 
                        u.FullName, 
                        u.DepartmentId, 
                        u.RoleId, 
                        u.Specialty, 
                        u.Login,
                        u.PasswordHash,
                        u.IsActive,
                        u.CreatedDate,
                        d.Name AS DepartmentName,
                        r.Name AS RoleName
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToUser(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Получить активного пользователя по Id
        /// </summary>
        public User GetActiveById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        u.Id, u.FullName, u.DepartmentId, u.RoleId, u.Specialty, u.Login,
                        u.PasswordHash, u.IsActive, u.CreatedDate,
                        d.Name AS DepartmentName, r.Name AS RoleName
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.Id = @Id AND u.IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToUser(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        /// <param name="fullName">ФИО</param>
        /// <param name="departmentId">ID отделения (nullable)</param>
        /// <param name="roleId">ID роли</param>
        /// <param name="specialty">Специальность</param>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Хэш пароля</param>
        /// <returns>Id созданного пользователя</returns>
        /// <exception cref="InvalidOperationException">Если логин уже занят</exception>
        public int Insert(string fullName, int? departmentId, int roleId, string specialty,
            string login, string passwordHash)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                // Проверка уникальности логина
                if (IsLoginExists(login, conn))
                {
                    throw new InvalidOperationException($"Логин \"{login}\" уже используется");
                }

                string query = @"
                    INSERT INTO Users (FullName, DepartmentId, RoleId, Specialty, Login, PasswordHash, IsActive)
                    VALUES (@FullName, @DepartmentId, @RoleId, @Specialty, @Login, @PasswordHash, 1);
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@DepartmentId",
                        departmentId.HasValue ? (object)departmentId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.Parameters.AddWithValue("@Specialty",
                        string.IsNullOrEmpty(specialty) ? (object)DBNull.Value : specialty);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Обновить данные пользователя
        /// </summary>
        public void Update(int id, string fullName, int? departmentId, int roleId,
            string specialty, string login)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                // Проверяем существование пользователя
                if (!Exists(id, conn))
                {
                    throw new InvalidOperationException($"Пользователь с Id={id} не найден");
                }

                // Проверяем уникальность логина (исключая самого пользователя)
                if (IsLoginExistsForOtherUser(login, id, conn))
                {
                    throw new InvalidOperationException($"Логин \"{login}\" уже используется другим пользователем");
                }

                string query = @"
                    UPDATE Users
                    SET FullName = @FullName,
                        DepartmentId = @DepartmentId,
                        RoleId = @RoleId,
                        Specialty = @Specialty,
                        Login = @Login
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@DepartmentId",
                        departmentId.HasValue ? (object)departmentId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.Parameters.AddWithValue("@Specialty",
                        string.IsNullOrEmpty(specialty) ? (object)DBNull.Value : specialty);
                    cmd.Parameters.AddWithValue("@Login", login);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Изменить пароль пользователя
        /// </summary>
        public void ChangePassword(int id, string newPasswordHash)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@PasswordHash", newPasswordHash);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Пользователь с Id={id} не найден");
                    }
                }
            }
        }

        /// <summary>
        /// Активировать/деактивировать пользователя
        /// </summary>
        public void SetActive(int id, bool isActive)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "UPDATE Users SET IsActive = @IsActive WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Пользователь с Id={id} не найден");
                    }
                }
            }
        }

        /// <summary>
        /// Мягкое удаление пользователя (установка IsActive = 0)
        /// </summary>
        /// <param name="id">Id удаляемого пользователя</param>
        /// <param name="currentUserId">Id текущего пользователя (для защиты от сапоудаления)</param>
        public void Delete(int id, int currentUserId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                // Нельзя удалить самого себя
                if (id == currentUserId)
                {
                    throw new InvalidOperationException("Невозможно удалить свою собственную учётную запись");
                }

                // Проверяем существование пользователя
                if (!Exists(id, conn))
                {
                    throw new InvalidOperationException($"Пользователь с Id={id} не найден");
                }

                // Проверяем наличие незавершённых тестов
                string checkTestsQuery = @"
                    SELECT COUNT(*) FROM TestAttempts 
                    WHERE UserId = @Id AND Status = 1"; // 1 = InProgress

                using (var checkCmd = new SqlCommand(checkTestsQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    int activeTests = (int)checkCmd.ExecuteScalar();

                    if (activeTests > 0)
                    {
                        throw new InvalidOperationException(
                            "Невозможно удалить пользователя: у него есть незавершённые тесты");
                    }
                }

                // Проверяем, не является ли пользователь автором вопросов
                string checkQuestionsQuery = @"
                    SELECT COUNT(*) FROM Questions 
                    WHERE CreatedBy = @Id AND IsActive = 1";

                using (var checkCmd = new SqlCommand(checkQuestionsQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    int authoredQuestions = (int)checkCmd.ExecuteScalar();

                    if (authoredQuestions > 0)
                    {
                        throw new InvalidOperationException(
                            $"Невозможно удалить пользователя: он является автором {authoredQuestions} активных вопросов");
                    }
                }

                // Мягкое удаление
                string deleteQuery = "UPDATE Users SET IsActive = 0 WHERE Id = @Id";
                using (var deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@Id", id);
                    deleteCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Жёсткое удаление пользователя (только если нет зависимостей)
        /// </summary>
        public void HardDelete(int id, int currentUserId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                if (id == currentUserId)
                {
                    throw new InvalidOperationException("Невозможно удалить свою собственную учётную запись!");
                }

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Проверяем все зависимости
                        string[] checkQueries = new[]
                        {
                            "SELECT COUNT(*) FROM TestAttempts WHERE UserId = @Id",
                            "SELECT COUNT(*) FROM Questions WHERE CreatedBy = @Id",
                            "SELECT COUNT(*) FROM TestAssignments WHERE TargetType = 3 AND TargetId = @Id"
                        };

                        string[] errorMessages = new[]
                        {
                            "у пользователя есть попытки прохождения тестов",
                            "пользователь является автором вопросов",
                            "пользователю назначены индивидуальные тесты"
                        };

                        for (int i = 0; i < checkQueries.Length; i++)
                        {
                            using (var checkCmd = new SqlCommand(checkQueries[i], conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@Id", id);
                                int count = (int)checkCmd.ExecuteScalar();

                                if (count > 0)
                                {
                                    throw new InvalidOperationException(
                                        $"Невозможно удалить пользователя: {errorMessages[i]}");
                                }
                            }
                        }

                        // Удаляем пользователя
                        string deleteQuery = "DELETE FROM Users WHERE Id = @Id";
                        using (var deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@Id", id);
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new InvalidOperationException($"Пользователь с Id={id} не найден");
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Проверить существование пользователя
        /// </summary>
        public bool Exists(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                return Exists(id, conn);
            }
        }

        private bool Exists(int id, SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Id = @Id";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// Проверить, существует ли логин
        /// </summary>
        public bool IsLoginExists(string login)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                return IsLoginExists(login, conn);
            }
        }

        private bool IsLoginExists(string login, SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// Проверить, не занят ли логин другим пользователем
        /// </summary>
        private bool IsLoginExistsForOtherUser(string login, int excludeUserId, SqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND Id <> @ExcludeId";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeUserId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// Получить список ролей
        /// </summary>
        public DataTable GetRoles()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "SELECT Id, Name, Description FROM Roles ORDER BY Name";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Получить список отделений
        /// </summary>
        public DataTable GetDepartments()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        private User MapReaderToUser(SqlDataReader reader)
        {
            int departmentIdOrdinal = reader.GetOrdinal("DepartmentId");

            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                DepartmentId = reader.IsDBNull(departmentIdOrdinal)
                    ? (int?)null : reader.GetInt32(departmentIdOrdinal),
                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                Specialty = reader.IsDBNull(reader.GetOrdinal("Specialty"))
                    ? null : reader.GetString(reader.GetOrdinal("Specialty")),
                Login = reader.GetString(reader.GetOrdinal("Login")),
                PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash"))
                    ? null : reader.GetString(reader.GetOrdinal("PasswordHash")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName"))
                    ? null : reader.GetString(reader.GetOrdinal("DepartmentName")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName"))
            };
        }
    }
}
