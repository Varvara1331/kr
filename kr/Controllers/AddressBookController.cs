using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demo.Data;
using demo.Models;
using demo.ViewModels;

namespace demo.Controllers
{
    // УБЕРИТЕ этот атрибут: [Authorize]
    public class AddressBookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddressBookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.TemporaryLinks
                .Where(e => e.IsUsed && !string.IsNullOrEmpty(e.Position))
                .OrderBy(e => e.FullName)
                .Select(e => new AddressBookViewModel
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.EmployeeEmail,
                    Position = e.Position ?? string.Empty,
                    Phone = e.Phone ?? string.Empty,
                    InternalNumber = e.InternalNumber ?? string.Empty,
                    UsedAt = e.UsedAt,
                    HasSignature = !string.IsNullOrEmpty(e.Position) &&
                                   !string.IsNullOrEmpty(e.Phone) &&
                                   !string.IsNullOrEmpty(e.InternalNumber)
                })
                .ToListAsync();

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.TemporaryLinks
                .FirstOrDefaultAsync(e => e.Id == id && e.IsUsed);

            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new AddressBookViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.EmployeeEmail,
                Position = employee.Position ?? string.Empty,
                Phone = employee.Phone ?? string.Empty,
                InternalNumber = employee.InternalNumber ?? string.Empty,
                UsedAt = employee.UsedAt,
                HasSignature = !string.IsNullOrEmpty(employee.Position) &&
                               !string.IsNullOrEmpty(employee.Phone) &&
                               !string.IsNullOrEmpty(employee.InternalNumber)
            };

            return View(viewModel);
        }

        [HttpGet]
        // УБЕРИТЕ этот атрибут: [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Export()
        {
            var employees = await _context.TemporaryLinks
                .Where(e => e.IsUsed && !string.IsNullOrEmpty(e.Position))
                .OrderBy(e => e.FullName)
                .ToListAsync();

            var csv = "ФИО;Email;Должность;Телефон;Внутренний номер;Дата регистрации\n";

            foreach (var emp in employees)
            {
                csv += $"\"{emp.FullName}\";" +
                       $"\"{emp.EmployeeEmail}\";" +
                       $"\"{emp.Position ?? ""}\";" +
                       $"\"{emp.Phone ?? ""}\";" +
                       $"\"{emp.InternalNumber ?? ""}\";" +
                       $"\"{emp.UsedAt?.ToString("dd.MM.yyyy HH:mm") ?? ""}\"\n";
            }

            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "address_book.csv");
        }

        [HttpGet]
        public async Task<IActionResult> VCard(int id)
        {
            var employee = await _context.TemporaryLinks
                .FirstOrDefaultAsync(e => e.Id == id && e.IsUsed);

            if (employee == null)
            {
                return NotFound();
            }

            // Создаем vCard вручную в формате vCard 3.0
            var vcardBuilder = new StringBuilder();

            vcardBuilder.AppendLine("BEGIN:VCARD");
            vcardBuilder.AppendLine("VERSION:3.0");

            // Обрабатываем ФИО
            var nameParts = employee.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var lastName = nameParts.Length > 0 ? nameParts[0] : "";
            var firstName = nameParts.Length > 1 ? nameParts[1] : "";
            var middleName = nameParts.Length > 2 ? string.Join(" ", nameParts.Skip(2)) : "";

            // Формат N:Фамилия;Имя;Отчество;Префикс;Суффикс
            vcardBuilder.AppendLine($"N:{EscapeVCardValue(lastName)};{EscapeVCardValue(firstName)};{EscapeVCardValue(middleName)};;");

            // Полное имя
            vcardBuilder.AppendLine($"FN:{EscapeVCardValue(employee.FullName)}");

            // Email
            if (!string.IsNullOrEmpty(employee.EmployeeEmail))
            {
                vcardBuilder.AppendLine($"EMAIL;TYPE=WORK,INTERNET:{EscapeVCardValue(employee.EmployeeEmail)}");
            }

            // Телефон
            if (!string.IsNullOrEmpty(employee.Phone))
            {
                // Очищаем телефон от нецифровых символов (кроме + в начале)
                var cleanPhone = employee.Phone;
                if (!cleanPhone.StartsWith("+"))
                {
                    cleanPhone = new string(cleanPhone.Where(c => char.IsDigit(c)).ToArray());
                }
                vcardBuilder.AppendLine($"TEL;TYPE=WORK,VOICE:{EscapeVCardValue(cleanPhone)}");
            }

            // Должность
            if (!string.IsNullOrEmpty(employee.Position))
            {
                vcardBuilder.AppendLine($"TITLE:{EscapeVCardValue(employee.Position)}");
            }

            // Организация (можно настроить или сделать конфигурируемой)
            vcardBuilder.AppendLine($"ORG:{EscapeVCardValue("Ваша организация")};");

            // Внутренний номер в заметках
            if (!string.IsNullOrEmpty(employee.InternalNumber))
            {
                vcardBuilder.AppendLine($"NOTE:{EscapeVCardValue($"Внутренний номер: {employee.InternalNumber}")}");
            }

            // Дата и время создания карточки
            vcardBuilder.AppendLine($"REV:{DateTime.Now:yyyyMMddTHHmmssZ}");

            // Уникальный идентификатор
            vcardBuilder.AppendLine($"UID:employee-{employee.Id}-{Guid.NewGuid()}");

            vcardBuilder.AppendLine("END:VCARD");

            // Конвертируем в байты
            var vcardContent = vcardBuilder.ToString();
            var bytes = Encoding.UTF8.GetBytes(vcardContent);

            // Формируем имя файла
            var fileName = $"{SanitizeFileName(employee.FullName)}.vcf";

            return File(bytes, "text/vcard", fileName);
        }

        // Вспомогательный метод для экранирования специальных символов в vCard
        private string EscapeVCardValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Экранируем специальные символы в vCard
            return value
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\n", "\\n")
                .Replace("\r", "");
        }

        // Вспомогательный метод для очистки имени файла
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "employee";

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName
                .Where(ch => !invalidChars.Contains(ch))
                .ToArray())
                .Replace(" ", "_")
                .Replace(",", "")
                .Replace(".", "");

            return string.IsNullOrEmpty(sanitized) ? "employee" : sanitized;
        }
    }
}