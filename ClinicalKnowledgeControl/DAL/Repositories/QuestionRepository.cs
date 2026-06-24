using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.DAL.Repositories
{
    public class QuestionRepository
    {
        public DataTable GetAll(int? clinicalGuidelineId = null)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT q.Id, q.Text, case q.QuestionType when 1 then 'Одиночный выбор' when 2 then 'Множественный выбор' end as QuestionType, 
                        q.Tags, cg.Name AS ClinicalGuideline
                    FROM Questions q
                    INNER JOIN ClinicalGuidelines cg ON q.ClinicalGuidelineId = cg.Id
                    WHERE q.IsActive = 1";

                if (clinicalGuidelineId.HasValue)
                {
                    query += " AND q.ClinicalGuidelineId = @CGId";
                }
                query += " ORDER BY cg.Name, q.Text";

                using (var cmd = new SqlCommand(query, conn))
                {
                    if (clinicalGuidelineId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@CGId", clinicalGuidelineId.Value);
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

        public int Insert(int clinicalGuidelineId, string text, int questionType, string explanation, string tags, int createdByUserId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO Questions (ClinicalGuidelineId, Text, QuestionType, Explanation, Tags)
                    VALUES (@CGId, @Text, @QuestionType, @Explanation, @Tags);
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CGId", clinicalGuidelineId);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@QuestionType", questionType);
                    cmd.Parameters.AddWithValue("@Explanation", string.IsNullOrEmpty(explanation) ? (object)DBNull.Value : explanation);
                    cmd.Parameters.AddWithValue("@Tags", string.IsNullOrEmpty(tags) ? (object)DBNull.Value : tags);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void InsertOption(int questionId, string text, bool isCorrect, int? sequenceOrder)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO QuestionOptions (QuestionId, Text, IsCorrect, SequenceOrder)
                    VALUES (@QuestionId, @Text, @IsCorrect, @SequenceOrder)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionId", questionId);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@IsCorrect", isCorrect);
                    cmd.Parameters.AddWithValue("@SequenceOrder", sequenceOrder.HasValue ? (object)sequenceOrder.Value : DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int questionId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "UPDATE Questions SET IsActive = 0 WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", questionId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Question> GetRandomQuestionsForTest(int testTemplateId, int questionCount)
        {
            var questions = new List<Question>();

            using (var conn = ConnectionManager.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_GetRandomQuestionsForTest", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@QuestionCount", questionCount);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                QuestionType = (QuestionType)reader.GetInt32(2),
                                Explanation = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }

                // Загружаем варианты ответов для каждого вопроса
                foreach (var question in questions)
                {
                    question.Options = GetQuestionOptions(question.Id, conn);
                }
            }

            return questions;
        }

        /// <summary>
        /// Получить вопрос по Id вместе с вариантами ответов
        /// </summary>
        /// <param name="id">Идентификатор вопроса</param>
        /// <returns>Объект Question с заполненными Options или null</returns>
        public Question GetById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                // ОДИН запрос с JOIN получает и вопрос, и все варианты ответов
                string query = @"
            SELECT 
                q.Id,
                q.ClinicalGuidelineId,
                cg.Name AS ClinicalGuidelineName,
                q.SectionName,
                q.Text,
                q.QuestionType,
                q.Difficulty,
                q.Tags,
                q.Explanation,
                q.IsActive,
                q.CreatedBy,
                u.FullName AS CreatedByName,
                q.CreatedDate,
                qo.Id AS OptionId,
                qo.Text AS OptionText,
                qo.IsCorrect AS OptionIsCorrect,
                qo.SequenceOrder AS OptionSequenceOrder
            FROM Questions q
            INNER JOIN ClinicalGuidelines cg ON q.ClinicalGuidelineId = cg.Id
            LEFT JOIN Users u ON q.CreatedBy = u.Id
            LEFT JOIN QuestionOptions qo ON q.Id = qo.QuestionId
            WHERE q.Id = @Id
            ORDER BY qo.Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        Question question = null;
                        var options = new List<QuestionOption>();

                        while (reader.Read())
                        {
                            // Вопрос инициализируем только один раз (из первой строки)
                            if (question == null)
                            {
                                question = MapReaderToQuestion(reader);
                            }

                            // Варианты ответов могут отсутствовать (LEFT JOIN), проверяем OptionId
                            int optionIdOrdinal = reader.GetOrdinal("OptionId");
                            if (!reader.IsDBNull(optionIdOrdinal))
                            {
                                options.Add(new QuestionOption
                                {
                                    Id = reader.GetInt32(optionIdOrdinal),
                                    Text = reader.GetString(reader.GetOrdinal("OptionText")),
                                    IsCorrect = reader.GetBoolean(reader.GetOrdinal("OptionIsCorrect")),
                                    SequenceOrder = reader.IsDBNull(reader.GetOrdinal("OptionSequenceOrder"))
                                        ? (int?)null : reader.GetInt32(reader.GetOrdinal("OptionSequenceOrder"))
                                });
                            }

                            if (question != null)
                            {
                                question.Options = options;
                            }
                        }

                        return question;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Получить только базовую информацию о вопросе (без вариантов ответов)
        /// Используется для быстрого получения данных без нагрузки на БД
        /// </summary>
        public Question GetBasicInfoById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        q.Id,
                        q.ClinicalGuidelineId,
                        cg.Name AS ClinicalGuidelineName,
                        q.Text,
                        q.QuestionType,
                        q.Tags,
                        q.Explanation,
                        q.IsActive,
                        q.CreatedBy,
                        u.FullName AS CreatedByName,
                        q.CreatedDate
                    FROM Questions q
                    INNER JOIN ClinicalGuidelines cg ON q.ClinicalGuidelineId = cg.Id
                    LEFT JOIN Users u ON q.CreatedBy = u.Id
                    WHERE q.Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToQuestion(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Маппинг SqlDataReader в объект Question
        /// </summary>
        private Question MapReaderToQuestion(SqlDataReader reader)
        {
            int questionTypeOrdinal = reader.GetOrdinal("QuestionType");
            int createdByOrdinal = reader.GetOrdinal("CreatedBy");

            return new Question
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ClinicalGuidelineId = reader.GetInt32(reader.GetOrdinal("ClinicalGuidelineId")),
                ClinicalGuidelineName = reader.IsDBNull(reader.GetOrdinal("ClinicalGuidelineName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ClinicalGuidelineName")),
                Text = reader.GetString(reader.GetOrdinal("Text")),
                QuestionType = (QuestionType)reader.GetInt32(questionTypeOrdinal),
                Tags = reader.IsDBNull(reader.GetOrdinal("Tags"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Tags")),
                Explanation = reader.IsDBNull(reader.GetOrdinal("Explanation"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Explanation")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedBy = reader.IsDBNull(createdByOrdinal)
                    ? (int?)null
                    : reader.GetInt32(createdByOrdinal),
                CreatedByName = reader.IsDBNull(reader.GetOrdinal("CreatedByName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("CreatedByName")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            };
        }

        /// <summary>
        /// Получить варианты ответов для вопроса
        /// </summary>
        private List<QuestionOption> GetQuestionOptions(int questionId, SqlConnection conn)
        {
            var options = new List<QuestionOption>();

            string query = @"
                SELECT Id, Text, IsCorrect, SequenceOrder
                FROM QuestionOptions
                WHERE QuestionId = @QuestionId
                ORDER BY Id";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@QuestionId", questionId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        options.Add(new QuestionOption
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            IsCorrect = reader.GetBoolean(reader.GetOrdinal("IsCorrect")),
                            SequenceOrder = reader.IsDBNull(reader.GetOrdinal("SequenceOrder"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("SequenceOrder"))
                        });
                    }
                }
            }

            return options;
        }

        /// <summary>
        /// Получить вопрос по Id с включением неактивных вопросов (для администратора)
        /// </summary>
        public Question GetByIdIncludeInactive(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        q.Id,
                        q.ClinicalGuidelineId,
                        cg.Name AS ClinicalGuidelineName,
                        q.Text,
                        q.QuestionType,
                        q.Tags,
                        q.Explanation,
                        q.IsActive,
                        q.CreatedBy,
                        u.FullName AS CreatedByName,
                        q.CreatedDate
                    FROM Questions q
                    INNER JOIN ClinicalGuidelines cg ON q.ClinicalGuidelineId = cg.Id
                    LEFT JOIN Users u ON q.CreatedBy = u.Id
                    WHERE q.Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var question = MapReaderToQuestion(reader);
                            question.Options = GetQuestionOptions(id, conn);
                            return question;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Обновить базовую информацию о вопросе
        /// </summary>
        public void Update(int id, int clinicalGuidelineId, string text, int questionType,
            string explanation, string tags, int updatedByUserId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    UPDATE Questions
                    SET ClinicalGuidelineId = @CGId,
                        Text = @Text,
                        QuestionType = @QuestionType,
                        Explanation = @Explanation,
                        Tags = @Tags,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = GETDATE()
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@CGId", clinicalGuidelineId);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@QuestionType", questionType);
                    cmd.Parameters.AddWithValue("@Explanation", string.IsNullOrEmpty(explanation) ? (object)DBNull.Value : explanation);
                    cmd.Parameters.AddWithValue("@Tags", string.IsNullOrEmpty(tags) ? (object)DBNull.Value : tags);
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedByUserId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Удалить все варианты ответов для вопроса (перед обновлением)
        /// </summary>
        public void DeleteAllOptions(int questionId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "DELETE FROM QuestionOptions WHERE QuestionId = @QuestionId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionId", questionId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновить вопрос вместе с вариантами ответов (транзакционно)
        /// </summary>
        public void UpdateWithOptions(int id, int clinicalGuidelineId, string text, int questionType,
            string explanation, string tags, int updatedByUserId, List<(string text, bool isCorrect, int? order)> options)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                conn.Open();

                // Начинаем транзакцию
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Обновляем базовую информацию о вопросе
                        string updateQuery = @"
                            UPDATE Questions
                            SET ClinicalGuidelineId = @CGId,
                                Text = @Text,
                                QuestionType = @QuestionType,
                                Explanation = @Explanation,
                                Tags = @Tags,
                                UpdatedBy = @UpdatedBy,
                                UpdatedDate = GETDATE()
                            WHERE Id = @Id";

                        using (var cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.Parameters.AddWithValue("@CGId", clinicalGuidelineId);
                            cmd.Parameters.AddWithValue("@Text", text);
                            cmd.Parameters.AddWithValue("@QuestionType", questionType);
                            cmd.Parameters.AddWithValue("@Explanation", string.IsNullOrEmpty(explanation) ? (object)DBNull.Value : explanation);
                            cmd.Parameters.AddWithValue("@Tags", string.IsNullOrEmpty(tags) ? (object)DBNull.Value : tags);
                            cmd.Parameters.AddWithValue("@UpdatedBy", updatedByUserId);

                            cmd.ExecuteNonQuery();
                        }

                        // 2. Удаляем старые варианты ответов
                        string deleteOptionsQuery = "DELETE FROM QuestionOptions WHERE QuestionId = @QuestionId";
                        using (var cmd = new SqlCommand(deleteOptionsQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@QuestionId", id);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Вставляем новые варианты ответов
                        string insertOptionQuery = @"
                            INSERT INTO QuestionOptions (QuestionId, Text, IsCorrect, SequenceOrder)
                            VALUES (@QuestionId, @Text, @IsCorrect, @SequenceOrder)";

                        foreach (var option in options)
                        {
                            using (var cmd = new SqlCommand(insertOptionQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuestionId", id);
                                cmd.Parameters.AddWithValue("@Text", option.text);
                                cmd.Parameters.AddWithValue("@IsCorrect", option.isCorrect);
                                cmd.Parameters.AddWithValue("@SequenceOrder", option.order.HasValue ? (object)option.order.Value : DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 4. Подтверждаем транзакцию
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // При ошибке откатываем транзакцию
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
