using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Models
{
    public class TestAttempt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestTemplateId { get; set; }
        public int? AssignmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? Score { get; set; }
        public int Status { get; set; }
        public bool? IsPassed { get; set; }
    }
}
