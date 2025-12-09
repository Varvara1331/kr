using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using demo.Data;

namespace demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Welcome", new { token = token });
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Welcome(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index");
            }

            var employee = await _context.TemporaryLinks
                .FirstOrDefaultAsync(e => e.Token == token);

            if (employee == null)
            {
                ViewBag.Error = "Ссылка не найдена или недействительна";
                return View("Error");
            }

            if (employee.IsUsed)
            {
                ViewBag.Error = "Эта ссылка уже была использована";
                return View();
            }

            if (employee.ExpiresAt < DateTime.UtcNow)
            {
                ViewBag.Error = "Срок действия ссылки истек";
                return View();
            }

            ViewBag.FullName = employee.FullName;
            ViewBag.Email = employee.EmployeeEmail;
            ViewBag.CreatedAt = employee.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            ViewBag.ExpiresAt = employee.ExpiresAt.ToString("dd.MM.yyyy HH:mm");
            ViewBag.Token = token;

            return View();
        }

        [HttpGet]
        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}