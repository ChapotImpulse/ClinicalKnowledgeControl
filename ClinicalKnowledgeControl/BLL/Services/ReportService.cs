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

        public DataTable GetDoctorRating()
        {
            return _repository.GetDoctorRating();
        }

        public DataTable GetQuestionStatistics()
        {
            return _repository.GetQuestionStatistics();
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
