using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Specialty { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
