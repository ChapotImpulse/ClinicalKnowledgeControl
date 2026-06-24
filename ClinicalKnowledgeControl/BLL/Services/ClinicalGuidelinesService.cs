using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Services
{
    public class ClinicalGuidelinesService
    {
        private readonly ClinicalGuidelinesRepository _repository;

        public ClinicalGuidelinesService()
        {
            _repository = new ClinicalGuidelinesRepository();
        }

        public DataTable GetAll()
        {
            return _repository.GetAll();
        }

        public ClinicalGuideline GetById(int id)
        {
            return _repository.GetById(id);
        }

        public int Create(string name, string icdCode, DateTime? updateDate, DateTime? effectiveDate, string fileLink, string description)
        {
            return _repository.Insert(name, icdCode, updateDate, effectiveDate, fileLink, description);
        }

        public void Update(int id, string name, string icdCode, DateTime? updateDate, DateTime? effectiveDate, string fileLink, string description)
        {
            _repository.Update(id, name, icdCode, updateDate, effectiveDate, fileLink, description);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
