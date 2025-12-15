using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using demo.Data;
using demo.Models;
using demo.ViewModels;

namespace demo.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Signature(string token)
        {
            var employee = await _context.TemporaryLinks
                .FirstOrDefaultAsync(e => e.Token == token);

            var viewModel = new EmployeeSignatureViewModel
            {
                FullName = employee.FullName,
                Email = employee.EmployeeEmail,
                Position = employee.Position ?? "",
                InternalNumber = employee.InternalNumber ?? "",
                Phone = employee.Phone ?? "",
                Token = token
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Signature(EmployeeSignatureViewModel model)
        {
            Console.WriteLine($"=== POST Signature ===");

            Console.WriteLine("=== Direct from Request.Form ===");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: '{Request.Form[key]}'");
            }

            Console.WriteLine($"=== From Model ===");
            Console.WriteLine($"Token: '{model.Token}'");
            Console.WriteLine($"Position: '{model.Position}'");
            Console.WriteLine($"Phone: '{model.Phone}'");
            Console.WriteLine($"InternalNumber: '{model.InternalNumber}'");
            Console.WriteLine($"FullName: '{model.FullName}'");
            Console.WriteLine($"Email: '{model.Email}'");

            var employee = await _context.TemporaryLinks
                .FirstOrDefaultAsync(e => e.Token == model.Token);

            if (employee == null)
            {
                Console.WriteLine("Сотрудник не найден по токену");
                return Content("Ошибка: данные не найдены");
            }

            employee.Position = model.Position ?? "";
            employee.InternalNumber = model.InternalNumber ?? "";
            employee.Phone = model.Phone ?? "";
            employee.IsUsed = true;
            employee.UsedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Save successful!");
                Console.WriteLine($"Saved Position: '{employee.Position}'");
                Console.WriteLine($"Saved Phone: '{employee.Phone}'");
                Console.WriteLine($"Saved InternalNumber: '{employee.InternalNumber}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
                return Content($"Ошибка сохранения: {ex.Message}");
            }

            model.IsSuccess = true;
            ViewBag.Success = true;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Download(string token)
        {
            var employee = await _context.TemporaryLinks
                .FirstOrDefaultAsync(e => e.Token == token);

            if (employee == null)
            {
                return Content("Данные не найдены");
            }

            var signatureHtml = GenerateSignatureHtml(employee);
            var fileName = $"Подпись_{employee.FullName.Replace(" ", "_")}.html";

            return File(System.Text.Encoding.UTF8.GetBytes(signatureHtml), "text/html", fileName);
        }

        private string GenerateSignatureHtml(TemporaryLink employee)
        {
            string phoneHtml = "";
            string internalNumberHtml = "";

            if (!string.IsNullOrEmpty(employee.Phone))
            {
                phoneHtml = $"<p style=\"margin: 5px 0;\"><strong>Телефон:</strong> {employee.Phone}</p>";
            }

            if (!string.IsNullOrEmpty(employee.InternalNumber))
            {
                internalNumberHtml = $"<p style=\"margin: 5px 0;\"><strong>Внутренний номер:</strong> {employee.InternalNumber}</p>";
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email подпись - {employee.FullName}</title>
</head>
<body>
    <div style=""font-family: Arial, sans-serif; font-size: 12px; color: #333;"">
        <br>
        <p style=""margin: 10px 0;"">
            С уважением,<br>
            <strong style=""font-size: 14px;"">{employee.FullName}</strong><br>
            <span style=""color: #555;"">{employee.Position}</span>
        </p>
        
        <div style=""border-left: 3px solid #007bff; padding-left: 10px; margin: 15px 0;"">
            <p style=""margin: 5px 0;"">
                <strong>Email:</strong> <a href=""mailto:{employee.EmployeeEmail}"" style=""color: #007bff; text-decoration: none;"">{employee.EmployeeEmail}</a>
            </p>
            
            {phoneHtml}
            {internalNumberHtml}
        </div>
        
        <div style=""margin-top: 20px; padding-top: 10px; border-top: 1px solid #eee; font-size: 11px; color: #777;"">
            <p style=""margin: 0;"">
                КР Автоматизация<br>
                600033, г. Владимир | ул. Мостостроевская, д. 18<br>
                +7 4922 37 24 80 | +7 4922 37 24 81<br>
                <a href=""http://kr-drive.ru/"" style=""color: #777; text-decoration: none;"">kr-drive.ru</a>
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}