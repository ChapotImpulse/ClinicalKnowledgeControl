using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.DAL.Repositories
{
    public class ReportRepository
    {
        /// <summary>
        /// Отчёт по всему учреждению (для заместителя главного врача)
        /// </summary>
        public DataTable GetInstitutionReport()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        u.FullName AS DoctorName,
                        u.Specialty,
                        d.Name AS DepartmentName,
                        tt.Name AS TestName,
                        -- Дедлайн: максимальный из всех активных назначений для врача
                        (
                            SELECT MAX(ta.Deadline)
                            FROM TestAssignments ta
                            WHERE ta.TestTemplateId = tt.Id
                              AND ta.IsActive = 1
                              AND (
                                  (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                                  OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
                              )
                        ) AS Deadline,
                        -- Статус: приоритет Сдано > Не сдано > Просрочено > Ожидает > Не назначено
                        CASE
                            -- 1. Если есть хотя бы одна успешная попытка — Сдано
                            WHEN EXISTS (
                                SELECT 1 
                                FROM TestAttempts att
                                INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                                WHERE ta.TestTemplateId = tt.Id
                                  AND att.UserId = u.Id
                                  AND att.IsPassed = 1
                            ) THEN 'Сдано'
                            -- 2. Если есть попытки, но ни одна не успешна — Не сдано
                            WHEN EXISTS (
                                SELECT 1 
                                FROM TestAttempts att
                                INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                                WHERE ta.TestTemplateId = tt.Id
                                  AND att.UserId = u.Id
                            ) THEN 'Не сдано'
                            -- 3. Если дедлайн прошёл, но попыток нет — Просрочено
                            WHEN (
                                SELECT MAX(ta.Deadline)
                                FROM TestAssignments ta
                                WHERE ta.TestTemplateId = tt.Id
                                  AND ta.IsActive = 1
                                  AND (
                                      (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                                      OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
                                  )
                            ) < GETDATE() THEN 'Просрочено'
                            -- 4. Если есть назначение, но нет попыток — Ожидает
                            WHEN EXISTS (
                                SELECT 1 
                                FROM TestAssignments ta
                                WHERE ta.TestTemplateId = tt.Id
                                  AND ta.IsActive = 1
                                  AND (
                                      (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                                      OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
                                  )
                            ) THEN 'Ожидает прохождения'
                            -- 5. Иначе — Не назначено
                            ELSE 'Не назначено'
                        END AS Status,
                        -- Максимальный балл среди всех попыток
                        (
                            SELECT MAX(att.Score)
                            FROM TestAttempts att
                            INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                            WHERE ta.TestTemplateId = tt.Id
                              AND att.UserId = u.Id
                        ) AS Score,
                        -- Дата последней попытки
                        (
                            SELECT MAX(att.EndTime)
                            FROM TestAttempts att
                            INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                            WHERE ta.TestTemplateId = tt.Id
                              AND att.UserId = u.Id
                        ) AS CompletionDate
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    CROSS JOIN TestTemplates tt
                    WHERE u.RoleId = 1 
                      AND u.IsActive = 1 
                      AND tt.IsActive = 1
                    ORDER BY u.FullName, tt.Name";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Отчёт по отделению (для заведующего)
        /// </summary>
        public DataTable GetDepartmentReport(int departmentId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        u.FullName AS DoctorName,
                        u.Specialty,
                        tt.Name AS TestName,
                        (
                            SELECT MAX(ta.Deadline)
                            FROM TestAssignments ta
                            WHERE ta.TestTemplateId = tt.Id
                              AND ta.IsActive = 1
                              AND (
                                  (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                                  OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
                              )
                        ) AS Deadline,
                        CASE
                            WHEN EXISTS (
                                SELECT 1 
                                FROM TestAttempts att
                                INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                                WHERE ta.TestTemplateId = tt.Id
                                  AND att.UserId = u.Id
                                  AND att.IsPassed = 1
                            ) THEN 'Сдано'
                            WHEN EXISTS (
                                SELECT 1 
                                FROM TestAttempts att
                                INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                                WHERE ta.TestTemplateId = tt.Id
                                  AND att.UserId = u.Id
                            ) THEN 'Не сдано'
                            WHEN (
                                SELECT MAX(ta.Deadline)
                                FROM TestAssignments ta
                                WHERE ta.TestTemplateId = tt.Id
                                  AND ta.IsActive = 1
                                  AND (
                                      (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                                      OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
                                  )
                            ) < GETDATE() THEN 'Просрочено'
                            WHEN EXISTS (
                                SELECT 1 
                                FROM TestAssignments ta
                                WHERE ta.TestTemplateId = tt.Id
                                  AND ta.IsActive = 1
                                  AND (
                                      (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                                      OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
                                  )
                            ) THEN 'Ожидает прохождения'
                            ELSE 'Не назначено'
                        END AS Status,
                        (
                            SELECT MAX(att.Score)
                            FROM TestAttempts att
                            INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                            WHERE ta.TestTemplateId = tt.Id
                              AND att.UserId = u.Id
                        ) AS Score,
                        (
                            SELECT MAX(att.EndTime)
                            FROM TestAttempts att
                            INNER JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                            WHERE ta.TestTemplateId = tt.Id
                              AND att.UserId = u.Id
                        ) AS CompletionDate
                    FROM Users u
                    CROSS JOIN TestTemplates tt
                    WHERE u.RoleId = 1 
                      AND u.IsActive = 1 
                      AND u.DepartmentId = @DepartmentId
                      AND tt.IsActive = 1
                    ORDER BY u.FullName, tt.Name";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
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
        /// Рейтинг врачей конкретного отделения
        /// </summary>
        /// <param name="departmentId">Id отделения</param>
        public DataTable GetDoctorRatingByDepartment(int departmentId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        u.FullName AS DoctorName,
                        d.Name AS DepartmentName,
                        COUNT(CASE WHEN att.IsPassed = 1 THEN 1 END) AS PassedTests,
                        COUNT(att.Id) AS TotalAttempts,
                        ISNULL(AVG(att.Score), 0) AS AverageScore
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    LEFT JOIN TestAttempts att ON u.Id = att.UserId AND att.Status IN (3, 4, 5)
                    WHERE u.RoleId = 1 
                      AND u.IsActive = 1 
                      AND u.DepartmentId = @DepartmentId
                    GROUP BY u.Id, u.FullName, d.Name
                    ORDER BY AverageScore DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
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
        /// Рейтинг врачей по всему учреждению (для заместителя главного врача)
        /// </summary>
        public DataTable GetDoctorRating()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        u.FullName AS DoctorName,
                        d.Name AS DepartmentName,
                        COUNT(CASE WHEN att.IsPassed = 1 THEN 1 END) AS PassedTests,
                        COUNT(att.Id) AS TotalAttempts,
                        ISNULL(AVG(att.Score), 0) AS AverageScore
                    FROM Users u
                    LEFT JOIN Departments d ON u.DepartmentId = d.Id
                    LEFT JOIN TestAttempts att ON u.Id = att.UserId AND att.Status IN (3, 4, 5)
                    WHERE u.RoleId = 1 AND u.IsActive = 1
                    GROUP BY u.Id, u.FullName, d.Name
                    ORDER BY AverageScore DESC";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Статистика по вопросам на основе ответов врачей конкретного отделения
        /// </summary>
        public DataTable GetQuestionStatisticsByDepartment(int departmentId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        q.Text AS QuestionText,
                        cg.Name AS ClinicalGuideline,
                        COUNT(aa.Id) AS TotalAnswers,
                        SUM(CASE WHEN aa.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectAnswers,
                        CAST(
                            CASE 
                                WHEN COUNT(aa.Id) = 0 THEN 0
                                ELSE SUM(CASE WHEN aa.IsCorrect = 1 THEN 1.0 ELSE 0.0 END) / COUNT(aa.Id) * 100
                            END AS DECIMAL(5,2)
                        ) AS CorrectPercentage
                    FROM Questions q
                    INNER JOIN ClinicalGuidelines cg ON q.ClinicalGuidelineId = cg.Id
                    INNER JOIN AttemptAnswers aa ON q.Id = aa.QuestionId
                    INNER JOIN TestAttempts att ON aa.AttemptId = att.Id
                    WHERE q.IsActive = 1
                      AND att.UserId IN (
                          SELECT Id FROM Users 
                          WHERE DepartmentId = @DepartmentId 
                            AND RoleId = 1 
                            AND IsActive = 1
                      )
                    GROUP BY q.Id, q.Text, cg.Name
                    HAVING COUNT(aa.Id) > 0
                    ORDER BY CorrectPercentage ASC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
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
        /// Статистика по вопросам по всему учреждению
        /// </summary>
        public DataTable GetQuestionStatistics()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        q.Text AS QuestionText,
                        cg.Name AS ClinicalGuideline,
                        COUNT(aa.Id) AS TotalAnswers,
                        SUM(CASE WHEN aa.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectAnswers,
                        CAST(
                            CASE 
                                WHEN COUNT(aa.Id) = 0 THEN 0
                                ELSE SUM(CASE WHEN aa.IsCorrect = 1 THEN 1.0 ELSE 0.0 END) / COUNT(aa.Id) * 100
                            END AS DECIMAL(5,2)
                        ) AS CorrectPercentage
                    FROM Questions q
                    INNER JOIN ClinicalGuidelines cg ON q.ClinicalGuidelineId = cg.Id
                    LEFT JOIN AttemptAnswers aa ON q.Id = aa.QuestionId
                    WHERE q.IsActive = 1
                    GROUP BY q.Id, q.Text, cg.Name
                    HAVING COUNT(aa.Id) > 0
                    ORDER BY CorrectPercentage ASC";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GetDoctorHistory(int userId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        tt.Name AS TestName,
                        ta.Deadline,
                        att.StartTime,
                        att.EndTime,
                        att.Score,
                        CASE
                            WHEN att.Status = 3 THEN 'Сдано'
                            WHEN att.Status = 4 THEN 'Не сдано'
                            WHEN att.Status = 5 THEN 'Время вышло'
                            ELSE 'В процессе'
                        END AS Status,
                        att.IsPassed
                    FROM TestAttempts att
                    INNER JOIN TestTemplates tt ON att.TestTemplateId = tt.Id
                    LEFT JOIN TestAssignments ta ON att.AssignmentId = ta.Id
                    WHERE att.UserId = @UserId
                    ORDER BY att.StartTime DESC";

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

        public DataTable GetDoctorAssignments(int userId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        ta.Id AS AssignmentId,
                        tt.Id AS TestTemplateId,
                        tt.Name AS TestName,
                        cg.Name AS ClinicalGuideline,
                        ta.Deadline,
                        CASE
                            WHEN EXISTS (
                                SELECT 1 FROM TestAttempts att 
                                WHERE att.AssignmentId = ta.Id 
                                  AND att.UserId = @UserId 
                                  AND att.IsPassed = 1
                            ) THEN 'Выполнено'
                            WHEN ta.Deadline < GETDATE() THEN 'Просрочено'
                            ELSE 'Ожидает'
                        END AS Status,
                        tt.TimeLimitMinutes,
                        tt.MaxAttempts
                    FROM TestAssignments ta
                    INNER JOIN TestTemplates tt ON ta.TestTemplateId = tt.Id
                    INNER JOIN ClinicalGuidelines cg ON tt.ClinicalGuidelineId = cg.Id
                    INNER JOIN Users u ON u.Id = @UserId
                    WHERE ta.IsActive = 1
                      AND (
                          (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId)
                          OR (ta.TargetType = 3 AND ta.TargetId = u.Id)
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

        public DataTable GetMonthlySummary(int userId)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT
                        FORMAT(att.StartTime, 'yyyy-MM') AS Month,
                        COUNT(att.Id) AS TotalTests,
                        SUM(CASE WHEN att.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedTests,
                        ISNULL(AVG(att.Score), 0) AS AverageScore
                    FROM TestAttempts att
                    WHERE att.UserId = @UserId
                      AND att.StartTime >= DATEADD(MONTH, -1, GETDATE())
                    GROUP BY FORMAT(att.StartTime, 'yyyy-MM')
                    ORDER BY Month DESC";

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
    }
}
