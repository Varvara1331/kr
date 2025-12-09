using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using demo.Data;
using demo.Models;

namespace demo.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLink(string employeeEmail, string fullName, int durationMinutes)
        {
            if (string.IsNullOrEmpty(employeeEmail) || string.IsNullOrEmpty(fullName) || durationMinutes <= 0)
            {
                ViewBag.Error = "Все поля обязательны для заполнения";
                return View("Index");
            }

            string token;
            do
            {
                token = GenerateToken();
            } while (await _context.TemporaryLinks.AnyAsync(t => t.Token == token));

            var expiresAt = DateTime.UtcNow.AddMinutes(durationMinutes);

            var existingEmployee = await _context.TemporaryLinks
               .FirstOrDefaultAsync(t => t.EmployeeEmail == employeeEmail);

            if (existingEmployee != null)
            {
                existingEmployee.Token = token;
                existingEmployee.ExpiresAt = expiresAt;
                existingEmployee.IsUsed = false;
                existingEmployee.UsedAt = null;

                if (existingEmployee.FullName != fullName)
                {
                    existingEmployee.FullName = fullName;
                }

                _context.TemporaryLinks.Update(existingEmployee);
                await _context.SaveChangesAsync();
            }
            else
            {
                var link = new TemporaryLink
                {
                    Token = token,
                    EmployeeEmail = employeeEmail,
                    FullName = fullName,
                    Phone = string.Empty,
                    InternalNumber = string.Empty,
                    Position = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt,
                    IsUsed = false
                };

                _context.TemporaryLinks.Add(link);
                await _context.SaveChangesAsync();
            }


            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var generatedLink = $"{baseUrl}/?token={token}";

            ViewBag.GeneratedLink = generatedLink;
            ViewBag.Token = token;
            ViewBag.EmployeeEmail = employeeEmail;
            ViewBag.FullName = fullName;
            ViewBag.ExpiresAt = expiresAt.ToString("dd.MM.yyyy HH:mm");
            ViewBag.IsUpdated = existingEmployee != null;
            ViewBag.Success = true;

            return View("Index");
        }

        private string GenerateToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData)
                    .Replace("/", "_")
                    .Replace("+", "-")
                    .Replace("=", "")
                    .Substring(0, 20);
            }
        }
    }
}