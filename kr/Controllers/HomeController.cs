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

        public async Task<IActionResult> Index(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }

            var link = await _context.TemporaryLinks
                .FirstOrDefaultAsync(l => l.Token == token);

            if (link == null)
            {
                ViewBag.Error = "Ссылка не найдена или недействительна";
                return View();
            }

            if (link.IsUsed)
            {
                ViewBag.Error = "Эта ссылка уже была использована";
                return View();
            }

            if (link.ExpiresAt < DateTime.UtcNow)
            {
                ViewBag.Error = "Срок действия ссылки истек";
                return View();
            }

            link.IsUsed = true;
            link.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            ViewBag.Success = true;
            ViewBag.FullName = link.FullName;
            ViewBag.Email = link.EmployeeEmail;
            ViewBag.CreatedAt = link.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            ViewBag.ExpiresAt = link.ExpiresAt.ToString("dd.MM.yyyy HH:mm");

            return View("Welcome");
        }
    }
}