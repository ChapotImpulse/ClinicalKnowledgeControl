using ClinicalKnowledgeControl.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Services
{
    public class ReportService
    {
        private readonly ReportRepository _repository;

        public ReportService()
        {
            _repository = new ReportRepository();
        }

        public DataTable GetDepartmentReport(int departmentId)
        {
            return _repository.GetDepartmentReport(departmentId);
        }

        public DataTable GetInstitutionReport()
        {
            return _repository.GetInstitutionReport();
        }

        /// <summary>
        /// Рейтинг врачей по всему учреждению (для заместителя главного врача)
        /// </summary>
        public DataTable GetDoctorRating()
        {
            return _repository.GetDoctorRating();
        }

        /// <summary>
        /// Рейтинг врачей конкретного отделения (для заведующего)
        /// </summary>
        public DataTable GetDoctorRatingByDepartment(int departmentId)
        {
            return _repository.GetDoctorRatingByDepartment(departmentId);
        }

        /// <summary>
        /// Статистика по вопросам по всему учреждению
        /// </summary>
        public DataTable GetQuestionStatistics()
        {
            return _repository.GetQuestionStatistics();
        }

        /// <summary>
        /// Статистика по вопросам на основе ответов врачей конкретного отделения
        /// </summary>
        public DataTable GetQuestionStatisticsByDepartment(int departmentId)
        {
            return _repository.GetQuestionStatisticsByDepartment(departmentId);
        }

        public DataTable GetDoctorHistory(int userId)
        {
            return _repository.GetDoctorHistory(userId);
        }

        public DataTable GetDoctorAssignments(int userId)
        {
            return _repository.GetDoctorAssignments(userId);
        }

        public DataTable GetMonthlySummary(int userId)
        {
            return _repository.GetMonthlySummary(userId);
        }

        public bool NeedAdditionalTraining(int departmentId)
        {
            var report = GetDepartmentReport(departmentId);
            if (report.Rows.Count == 0) return false;

            decimal totalScore = 0;
            int count = 0;
            foreach (DataRow row in report.Rows)
            {
                if (row["Score"] != DBNull.Value)
                {
                    totalScore += Convert.ToDecimal(row["Score"]);
                    count++;
                }
            }

            if (count == 0) return false;
            decimal average = totalScore / count;
            return average < 70;
        }
    }
}
