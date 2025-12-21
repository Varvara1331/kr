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
    <div style=""font-family: Verdana, Geneva, Tahoma, sans-serif; font-size: 13px; color: #333;"">
        <br>
        <p style=""margin: 10px 0;"">
            С уважением,<br>
            <strong style=""font-size: 14px;"">{employee.FullName}</strong><br>
            <span color: #515151;"">{employee.Position}</span>
        </p>
        <img src=""http://localhost:5138//images/kr-logo.png""
            alt=""KR Automation""/>
        <div style=""border-left: 3px solid #ec5c2a; padding-left: 10px; margin: 15px 0;"">
            <p style=""margin: 5px 0;"">
                <strong>Email:</strong> <a href=""mailto:{employee.EmployeeEmail}"" style=""color: #ec5c2a; text-decoration: none;"">{employee.EmployeeEmail}</a>
            </p>
            
            {phoneHtml}
            {internalNumberHtml}
        </div>
        
        <div>
            <p style=""color: #ec5c2a; margin-bottom: 0;"">КР Автоматизация</p> 
            <p style=""margin: 0;"">600033, г. Владимир | ул. Мостостроевская, д. 18</p>
            <p style=""margin: 0;"">
                +7 4922 37 24 80 | +7 4922 37 24 81
            </p>
                <a style=""color: #515151"" href=""http://kr-drive.ru/"">kr-drive.ru</a>
            </p>
        </div>
        <img src=""http://localhost:5138//images/line.png"">
    </div>
</body>
</html>";
        }
    }
}