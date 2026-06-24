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
    public class TestTemplateRepository
    {
        public DataTable GetAll()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        tt.Id, 
                        tt.Name, 
                        cg.Name AS ClinicalGuideline, 
                        tt.QuestionCount,
                        tt.TimeLimitMinutes, 
                        tt.PassingScore, 
                        tt.MaxAttempts,
                        uc.FullName AS CreatedBy,
                        tt.CreatedDate
                    FROM TestTemplates tt
                    INNER JOIN ClinicalGuidelines cg ON tt.ClinicalGuidelineId = cg.Id
                    LEFT JOIN Users uc ON tt.CreatedBy = uc.Id
                    WHERE tt.IsActive = 1
                    ORDER BY tt.Name";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public TestTemplate GetById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
            SELECT 
                tt.Id,
                tt.Name,
                tt.ClinicalGuidelineId,
                cg.Name AS ClinicalGuidelineName,
                tt.QuestionCount,
                tt.TimeLimitMinutes,
                tt.PassingScore,
                tt.MaxAttempts,
                tt.IsActive,
                tt.CreatedBy,
                uc.FullName AS CreatedByName,
                tt.CreatedDate,
                tt.UpdatedBy,
                uu.FullName AS UpdatedByName,
                tt.UpdatedDate
            FROM TestTemplates tt
            INNER JOIN ClinicalGuidelines cg ON tt.ClinicalGuidelineId = cg.Id
            LEFT JOIN Users uc ON tt.CreatedBy = uc.Id
            LEFT JOIN Users uu ON tt.UpdatedBy = uu.Id
            WHERE tt.Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToTestTemplate(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Получить активный шаблон теста по Id (исключая удалённые)
        /// </summary>
        public TestTemplate GetActiveById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        tt.Id,
                        tt.Name,
                        tt.ClinicalGuidelineId,
                        cg.Name AS ClinicalGuidelineName,
                        tt.QuestionCount,
                        tt.TimeLimitMinutes,
                        tt.PassingScore,
                        tt.MaxAttempts,
                        tt.IsActive,
                        tt.CreatedDate,
                        tt.UpdatedBy,
                        u.FullName AS UpdatedByName,
                        tt.UpdatedDate
                    FROM TestTemplates tt
                    INNER JOIN ClinicalGuidelines cg ON tt.ClinicalGuidelineId = cg.Id
                    LEFT JOIN Users u ON tt.UpdatedBy = u.Id
                    WHERE tt.Id = @Id AND tt.IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToTestTemplate(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Обновить шаблон теста
        /// </summary>
        public void Update(int id, string name, int clinicalGuidelineId, int questionCount,
            int timeLimitMinutes, decimal passingScore, int maxAttempts, int updatedByUserId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    UPDATE TestTemplates
                    SET Name = @Name,
                        ClinicalGuidelineId = @ClinicalGuidelineId,
                        QuestionCount = @QuestionCount,
                        TimeLimitMinutes = @TimeLimitMinutes,
                        PassingScore = @PassingScore,
                        MaxAttempts = @MaxAttempts,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = GETDATE()
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ClinicalGuidelineId", clinicalGuidelineId);
                    cmd.Parameters.AddWithValue("@QuestionCount", questionCount);
                    cmd.Parameters.AddWithValue("@TimeLimitMinutes", timeLimitMinutes);
                    cmd.Parameters.AddWithValue("@PassingScore", passingScore);
                    cmd.Parameters.AddWithValue("@MaxAttempts", maxAttempts);
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedByUserId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Шаблон теста с Id={id} не найден");
                    }
                }
            }
        }

        /// <summary>
        /// Мягкое удаление шаблона теста (установка IsActive = 0)
        /// </summary>
        public void Delete(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                // Проверяем, нет ли активных назначений на этот шаблон
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM TestAssignments ta
                    INNER JOIN TestAttempts att ON ta.Id = att.AssignmentId
                    WHERE ta.TestTemplateId = @Id 
                      AND att.Status = 1"; // 1 = InProgress

                using (var checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    int activeTests = (int)checkCmd.ExecuteScalar();

                    if (activeTests > 0)
                    {
                        throw new InvalidOperationException(
                            "Невозможно удалить шаблон: есть незавершённые тесты, назначенные по этому шаблону");
                    }
                }

                // Мягкое удаление
                string deleteQuery = @"
                    UPDATE TestTemplates 
                    SET IsActive = 0, UpdatedDate = GETDATE() 
                    WHERE Id = @Id";

                using (var deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = deleteCmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException($"Шаблон теста с Id={id} не найден");
                    }
                }
            }
        }

        /// <summary>
        /// Жёсткое удаление шаблона теста (только если нет зависимых записей)
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
                        // Проверяем зависимости
                        string checkQuery = @"
                            SELECT COUNT(*) FROM TestAssignments WHERE TestTemplateId = @Id";

                        using (var checkCmd = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", id);
                            int assignmentsCount = (int)checkCmd.ExecuteScalar();

                            if (assignmentsCount > 0)
                            {
                                throw new InvalidOperationException(
                                    $"Невозможно удалить шаблон: существует {assignmentsCount} связанных назначений");
                            }
                        }

                        // Удаляем шаблон
                        string deleteQuery = "DELETE FROM TestTemplates WHERE Id = @Id";
                        using (var deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@Id", id);
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new InvalidOperationException($"Шаблон теста с Id={id} не найден");
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

        public int Insert(string name, int clinicalGuidelineId, int questionCount,
            int timeLimitMinutes, decimal passingScore, int maxAttempts, int createdByUserId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
            INSERT INTO TestTemplates 
                (Name, ClinicalGuidelineId, QuestionCount, TimeLimitMinutes, 
                 PassingScore, MaxAttempts, CreatedBy)
            VALUES 
                (@Name, @CGId, @QuestionCount, @TimeLimitMinutes, 
                 @PassingScore, @MaxAttempts, @CreatedBy);
            SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@CGId", clinicalGuidelineId);
                    cmd.Parameters.AddWithValue("@QuestionCount", questionCount);
                    cmd.Parameters.AddWithValue("@TimeLimitMinutes", timeLimitMinutes);
                    cmd.Parameters.AddWithValue("@PassingScore", passingScore);
                    cmd.Parameters.AddWithValue("@MaxAttempts", maxAttempts);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Получить количество вопросов в базе для указанной КР
        /// Используется для валидации при создании/обновлении шаблона
        /// </summary>
        public int GetAvailableQuestionsCount(int clinicalGuidelineId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM Questions 
                    WHERE ClinicalGuidelineId = @CGId AND IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CGId", clinicalGuidelineId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        private TestTemplate MapReaderToTestTemplate(SqlDataReader reader)
        {
            int createdByOrdinal = reader.GetOrdinal("CreatedBy");
            int updatedByOrdinal = reader.GetOrdinal("UpdatedBy");
            int updatedDateOrdinal = reader.GetOrdinal("UpdatedDate");

            return new TestTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                ClinicalGuidelineId = reader.GetInt32(reader.GetOrdinal("ClinicalGuidelineId")),
                ClinicalGuidelineName = reader.IsDBNull(reader.GetOrdinal("ClinicalGuidelineName"))
                    ? null : reader.GetString(reader.GetOrdinal("ClinicalGuidelineName")),
                QuestionCount = reader.GetInt32(reader.GetOrdinal("QuestionCount")),
                TimeLimitMinutes = reader.GetInt32(reader.GetOrdinal("TimeLimitMinutes")),
                PassingScore = reader.GetDecimal(reader.GetOrdinal("PassingScore")),
                MaxAttempts = reader.GetInt32(reader.GetOrdinal("MaxAttempts")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),

                // Поля создания
                CreatedBy = reader.IsDBNull(createdByOrdinal)
                    ? (int?)null : reader.GetInt32(createdByOrdinal),
                CreatedByName = reader.IsDBNull(reader.GetOrdinal("CreatedByName"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedByName")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),

                // Поля обновления
                UpdatedBy = reader.IsDBNull(updatedByOrdinal)
                    ? (int?)null : reader.GetInt32(updatedByOrdinal),
                UpdatedByName = reader.IsDBNull(reader.GetOrdinal("UpdatedByName"))
                    ? null : reader.GetString(reader.GetOrdinal("UpdatedByName")),
                UpdatedDate = reader.IsDBNull(updatedDateOrdinal)
                    ? (DateTime?)null : reader.GetDateTime(updatedDateOrdinal)
            };
        }
    }
}
