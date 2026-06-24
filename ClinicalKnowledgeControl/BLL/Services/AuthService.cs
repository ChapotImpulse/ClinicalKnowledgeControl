using ClinicalKnowledgeControl.BLL.Models;
using ClinicalKnowledgeControl.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.BLL.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public User Login(string login, string password)
        {
            string passwordHash = HashPassword(password);
            return _userRepository.Authenticate(login, passwordHash);
        }

        private string HashPassword(string password)
        {
            // В реальном проекте используйте BCrypt или PBKDF2
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
