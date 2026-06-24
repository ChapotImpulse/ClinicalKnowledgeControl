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
    public class AssignmentRepository
    {
        public void Insert(int testTemplateId, int targetType, int targetId, DateTime deadline)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO TestAssignments (TestTemplateId, TargetType, TargetId, Deadline, IsActive)
                    VALUES (@TestTemplateId, @TargetType, @TargetId, @Deadline, 1)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@TargetType", targetType);
                    cmd.Parameters.AddWithValue("@TargetId", targetId);
                    cmd.Parameters.AddWithValue("@Deadline", deadline);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MassAssignToDepartment(int testTemplateId, int departmentId, DateTime deadline)
        {
            Insert(testTemplateId, 1, departmentId, deadline);
        }

        public void MassAssignBySpecialty(int testTemplateId, string specialty, DateTime deadline)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO TestAssignments (TestTemplateId, TargetType, TargetId, Deadline, IsActive)
                    SELECT @TestTemplateId, 2, u.Id, @Deadline, 1
                    FROM Users u
                    WHERE u.Specialty = @Specialty AND u.RoleId = 1 AND u.IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@Specialty", specialty);
                    cmd.Parameters.AddWithValue("@Deadline", deadline);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetAll()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT ta.Id, tt.Name AS TestName, ta.Deadline,
                           CASE ta.TargetType WHEN 1 THEN 'Отделение' WHEN 2 THEN 'Специальность' WHEN 3 THEN 'Врач' END AS TargetType,
                           CASE
                               WHEN ta.TargetType = 1 THEN (SELECT Name FROM Departments WHERE Id = ta.TargetId)
                               WHEN ta.TargetType = 2 THEN (SELECT Specialty FROM Users WHERE Id = ta.TargetId)
                               WHEN ta.TargetType = 3 THEN (SELECT FullName FROM Users WHERE Id = ta.TargetId)
                           END AS TargetName,
                           ta.IsAutoAssigned,
                           ta.IsActive
                    FROM TestAssignments ta
                    INNER JOIN TestTemplates tt ON ta.TestTemplateId = tt.Id
                    WHERE ta.IsActive = 1
                    ORDER BY ta.Deadline DESC";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

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

        /// <summary>
        /// Получить назначение теста по Id
        /// </summary>
        /// <param name="id">Идентификатор назначения</param>
        /// <returns>Объект Assignment или null, если не найден</returns>
        public Assignment GetById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        ta.Id,
                        ta.TestTemplateId,
                        tt.Name AS TestTemplateName,
                        ta.TargetType,
                        ta.TargetId,
                        CASE 
                            WHEN ta.TargetType = 1 THEN (SELECT Name FROM Departments WHERE Id = ta.TargetId)
                            WHEN ta.TargetType = 2 THEN (SELECT Specialty FROM Users WHERE Id = ta.TargetId)
                            WHEN ta.TargetType = 3 THEN (SELECT FullName FROM Users WHERE Id = ta.TargetId)
                        END AS TargetName,
                        ta.Deadline,
                        ta.IsAutoAssigned,
                        ta.IsActive,
                        ta.CreatedDate
                    FROM TestAssignments ta
                    INNER JOIN TestTemplates tt ON ta.TestTemplateId = tt.Id
                    WHERE ta.Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToAssignment(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Получить активное назначение по Id
        /// </summary>
        public Assignment GetActiveById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        ta.Id, ta.TestTemplateId, tt.Name AS TestTemplateName,
                        ta.TargetType, ta.TargetId,
                        CASE 
                            WHEN ta.TargetType = 1 THEN (SELECT Name FROM Departments WHERE Id = ta.TargetId)
                            WHEN ta.TargetType = 2 THEN (SELECT Specialty FROM Users WHERE Id = ta.TargetId)
                            WHEN ta.TargetType = 3 THEN (SELECT FullName FROM Users WHERE Id = ta.TargetId)
                        END AS TargetName,
                        ta.Deadline, ta.IsAutoAssigned, ta.IsActive, ta.CreatedDate
                    FROM TestAssignments ta
                    INNER JOIN TestTemplates tt ON ta.TestTemplateId = tt.Id
                    WHERE ta.Id = @Id AND ta.IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToAssignment(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Обновить назначение теста
        /// </summary>
        /// <param name="id">Id назначения</param>
        /// <param name="testTemplateId">Новый Id шаблона теста</param>
        /// <param name="targetType">Новый тип цели (1-отделение, 2-специальность, 3-врач)</param>
        /// <param name="targetId">Новый Id цели</param>
        /// <param name="deadline">Новый дедлайн</param>
        public void Update(int id, int testTemplateId, int targetType, int targetId, DateTime deadline)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                // Проверяем существование назначения
                if (!Exists(id, conn))
                {
                    throw new InvalidOperationException($"Назначение с Id={id} не найдено");
                }

                // Проверяем, нет ли уже пройденных тестов по этому назначению
                int completedAttempts = GetCompletedAttemptsCount(id, conn);
                if (completedAttempts > 0)
                {
                    throw new InvalidOperationException(
                        $"Невозможно изменить назначение: {completedAttempts} врачей уже прошли тест");
                }

                string query = @"
                    UPDATE TestAssignments
                    SET TestTemplateId = @TestTemplateId,
                        TargetType = @TargetType,
                        TargetId = @TargetId,
                        Deadline = @Deadline,
                        UpdatedDate = GETDATE()
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@TargetType", targetType);
                    cmd.Parameters.AddWithValue("@TargetId", targetId);
                    cmd.Parameters.AddWithValue("@Deadline", deadline);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Не удалось обновить назначение с Id={id}");
                    }
                }
            }
        }

        /// <summary>
        /// Обновить только дедлайн назначения (без изменения цели)
        /// </summary>
        public void UpdateDeadline(int id, DateTime newDeadline)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    UPDATE TestAssignments
                    SET Deadline = @Deadline, UpdatedDate = GETDATE()
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Deadline", newDeadline);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Назначение с Id={id} не найдено");
                    }
                }
            }
        }

        /// <summary>
        /// Мягкое удаление назначения (установка IsActive = 0)
        /// </summary>
        /// <param name="id">Id назначения</param>
        public void Delete(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                // Проверяем существование
                if (!Exists(id, conn))
                {
                    throw new InvalidOperationException($"Назначение с Id={id} не найдено");
                }

                // Проверяем наличие незавершённых тестов
                int inProgressAttempts = GetInProgressAttemptsCount(id, conn);
                if (inProgressAttempts > 0)
                {
                    throw new InvalidOperationException(
                        $"Невозможно удалить назначение: {inProgressAttempts} врачей находятся в процессе прохождения теста");
                }

                // Мягкое удаление
                string deleteQuery = @"
                    UPDATE TestAssignments 
                    SET IsActive = 0, UpdatedDate = GETDATE() 
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Не удалось удалить назначение с Id={id}");
                    }
                }
            }
        }

        /// <summary>
        /// Жёсткое удаление назначения (только если нет зависимостей)
        /// </summary>
        public void HardDelete(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Проверяем все зависимости
                        string checkQuery = @"
                            SELECT COUNT(*) FROM TestAttempts WHERE AssignmentId = @Id";

                        using (var checkCmd = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", id);
                            int attemptsCount = (int)checkCmd.ExecuteScalar();

                            if (attemptsCount > 0)
                            {
                                throw new InvalidOperationException(
                                    $"Невозможно удалить назначение: существует {attemptsCount} попыток прохождения теста");
                            }
                        }

                        // Удаляем назначение
                        string deleteQuery = "DELETE FROM TestAssignments WHERE Id = @Id";
                        using (var deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@Id", id);
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new InvalidOperationException($"Назначение с Id={id} не найдено");
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
        /// Проверить существование назначения
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
            string query = "SELECT COUNT(*) FROM TestAssignments WHERE Id = @Id";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// Получить количество завершённых попыток по назначению
        /// </summary>
        private int GetCompletedAttemptsCount(int assignmentId, SqlConnection conn)
        {
            string query = @"
                SELECT COUNT(*) FROM TestAttempts 
                WHERE AssignmentId = @AssignmentId 
                  AND Status IN (3, 4, 5)"; // 3-Passed, 4-Failed, 5-TimeOut

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Получить количество незавершённых попыток по назначению
        /// </summary>
        private int GetInProgressAttemptsCount(int assignmentId, SqlConnection conn)
        {
            string query = @"
                SELECT COUNT(*) FROM TestAttempts 
                WHERE AssignmentId = @AssignmentId AND Status = 1"; // 1 = InProgress

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Проверить, существует ли уже назначение для данной цели и шаблона
        /// </summary>
        public bool IsAssignmentExists(int testTemplateId, int targetType, int targetId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*) FROM TestAssignments 
                    WHERE TestTemplateId = @TestTemplateId 
                      AND TargetType = @TargetType 
                      AND TargetId = @TargetId 
                      AND IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@TargetType", targetType);
                    cmd.Parameters.AddWithValue("@TargetId", targetId);

                    conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        /// <summary>
        /// Получить список назначений для конкретного врача
        /// </summary>
        public DataTable GetAssignmentsForDoctor(int userId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        ta.Id,
                        tt.Name AS TestName,
                        ta.Deadline,
                        ta.IsAutoAssigned,
                        CASE
                            WHEN EXISTS (SELECT 1 FROM TestAttempts att WHERE att.AssignmentId = ta.Id AND att.UserId = @UserId AND att.IsPassed = 1) THEN 'Выполнено'
                            WHEN ta.Deadline < GETDATE() THEN 'Просрочено'
                            ELSE 'Ожидает'
                        END AS Status
                    FROM TestAssignments ta
                    INNER JOIN TestTemplates tt ON ta.TestTemplateId = tt.Id
                    WHERE ta.IsActive = 1
                      AND (
                          (ta.TargetType = 1 AND ta.TargetId IN (SELECT DepartmentId FROM Users WHERE Id = @UserId))
                          OR (ta.TargetType = 2 AND ta.TargetId IN (SELECT Id FROM Users WHERE Specialty = (SELECT Specialty FROM Users WHERE Id = @UserId) AND RoleId = 1))
                          OR (ta.TargetType = 3 AND ta.TargetId = @UserId)
                      )
                    ORDER BY ta.Deadline";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private Assignment MapReaderToAssignment(SqlDataReader reader)
        {
            return new Assignment
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TestTemplateId = reader.GetInt32(reader.GetOrdinal("TestTemplateId")),
                TestTemplateName = reader.IsDBNull(reader.GetOrdinal("TestTemplateName"))
                    ? null : reader.GetString(reader.GetOrdinal("TestTemplateName")),
                TargetType = reader.GetInt32(reader.GetOrdinal("TargetType")),
                TargetId = reader.GetInt32(reader.GetOrdinal("TargetId")),
                TargetName = reader.IsDBNull(reader.GetOrdinal("TargetName"))
                    ? null : reader.GetString(reader.GetOrdinal("TargetName")),
                Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline")),
                IsAutoAssigned = reader.GetBoolean(reader.GetOrdinal("IsAutoAssigned")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            };
        }
    }
}
