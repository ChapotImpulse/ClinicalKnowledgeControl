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
    public class TestRepository
    {
        public bool CanStartAttempt(int userId, int testTemplateId, int maxAttempts)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_CanStartAttempt", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@MaxAttempts", maxAttempts);

                    var canStartParam = new SqlParameter("@CanStart", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    var attemptsParam = new SqlParameter("@AttemptsToday", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(canStartParam);
                    cmd.Parameters.Add(attemptsParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return (bool)canStartParam.Value;
                }
            }
        }

        public int CreateTestAttempt(int userId, int testTemplateId, int? assignmentId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO TestAttempts (UserId, TestTemplateId, AssignmentId, StartTime, Status)
                    VALUES (@UserId, @TestTemplateId, @AssignmentId, @StartTime, 1);
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@TestTemplateId", testTemplateId);
                    cmd.Parameters.AddWithValue("@AssignmentId", assignmentId.HasValue ? (object)assignmentId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void SaveAnswer(int attemptId, int questionId, string selectedOptions, bool? isCorrect, int timeSpentSeconds)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO AttemptAnswers (AttemptId, QuestionId, SelectedOptions, IsCorrect, TimeSpentSeconds)
                    VALUES (@AttemptId, @QuestionId, @SelectedOptions, @IsCorrect, @TimeSpentSeconds)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                    cmd.Parameters.AddWithValue("@QuestionId", questionId);
                    cmd.Parameters.AddWithValue("@SelectedOptions", selectedOptions);
                    cmd.Parameters.AddWithValue("@IsCorrect", isCorrect.HasValue ? (object)isCorrect.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TimeSpentSeconds", timeSpentSeconds);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void FinishTestAttempt(int attemptId, decimal score, int status, bool isPassed)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    UPDATE TestAttempts
                    SET EndTime = @EndTime,
                        Score = @Score,
                        Status = @Status,
                        IsPassed = @IsPassed
                    WHERE Id = @AttemptId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                    cmd.Parameters.AddWithValue("@EndTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@IsPassed", isPassed);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public TestTemplate GetTestTemplate(int testTemplateId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT Id, Name, ClinicalGuidelineId, QuestionCount, TimeLimitMinutes, PassingScore, MaxAttempts
                    FROM TestTemplates
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", testTemplateId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TestTemplate
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ClinicalGuidelineId = reader.GetInt32(2),
                                QuestionCount = reader.GetInt32(3),
                                TimeLimitMinutes = reader.GetInt32(4),
                                PassingScore = (decimal)reader.GetDecimal(5),
                                MaxAttempts = reader.GetInt32(6)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
