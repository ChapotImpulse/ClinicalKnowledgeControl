using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Models
{
    public class ClinicalGuideline
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ICDCode { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string FileLink { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
