using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public int TestTemplateId { get; set; }
        public string TestTemplateName { get; set; }
        public int TargetType { get; set; } // 1-отделение, 2-специальность, 3-врач
        public string TargetTypeName { get; set; }
        public int TargetId { get; set; }
        public string TargetName { get; set; } // ФИО врача / Название отделения / Специальность
        public DateTime Deadline { get; set; }
        public bool IsAutoAssigned { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
