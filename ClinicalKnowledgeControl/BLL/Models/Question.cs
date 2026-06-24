using ClinicalKnowledgeControl.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int ClinicalGuidelineId { get; set; }
        public string ClinicalGuidelineName { get; set; } // Для отображения в UI
        public string Text { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Tags { get; set; }
        public string Explanation { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; } // ФИО автора (ГВС)
        public DateTime CreatedDate { get; set; }
        public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
