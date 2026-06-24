using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Models
{
    public class TestTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClinicalGuidelineId { get; set; }
        public string ClinicalGuidelineName { get; set; } // Для отображения в UI
        public int QuestionCount { get; set; }
        public int TimeLimitMinutes { get; set; }
        public decimal PassingScore { get; set; }
        public int MaxAttempts { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
