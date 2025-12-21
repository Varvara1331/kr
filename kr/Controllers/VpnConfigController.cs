using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demo.Data;
using demo.Models;
using demo.ViewModels;

namespace demo.Controllers
{
    public class VpnConfigController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VpnConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string server, int port, string ca, string cert, string key)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(server) || port <= 0)
            {
                ViewBag.Error = "Заполните обязательные поля";
                return View();
            }

            var configContent = $@"
# VPN Configuration: {name}
remote {server} {port}
proto udp
dev tun
resolv-retry infinite
nobind
persist-key
persist-tun
cipher AES-256-GCM
auth SHA256
<ca>
{ca}
</ca>
<cert>
{cert}
</cert>
<key>
{key}
</key>
";

            var vpnConfig = new VpnConfig
            {
                Name = name,
                Description = "Простая конфигурация",
                ConfigType = "OpenVPN",
                ServerAddress = server,
                ServerPort = port,
                Protocol = "udp",
                ConfigContent = configContent,
                CaCertificate = ca,
                ClientCertificate = cert,
                ClientKey = key,
                Cipher = "AES-256-GCM",
                Auth = "SHA256",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.VpnConfigs.Add(vpnConfig);
            await _context.SaveChangesAsync();

            ViewBag.Success = $"Конфигурация '{name}' создана!";
            ViewBag.ConfigName = name;
            ViewBag.ConfigId = vpnConfig.Id;

            return View("Success");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var configs = await _context.VpnConfigs
                .Where(v => v.IsActive)
                .OrderBy(v => v.Name)
                .Select(v => new
                {
                    v.Id,
                    v.Name,
                    v.ServerAddress,
                    v.ServerPort,
                    v.CreatedAt
                })
                .ToListAsync();

            ViewBag.Configs = configs;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var vpnConfig = await _context.VpnConfigs
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vpnConfig == null)
            {
                ViewBag.Error = "Конфигурация не найдена";
                return View("Index");
            }

            var configContent = vpnConfig.ConfigContent;
            var fileName = $"{vpnConfig.Name.Replace(" ", "_")}.conf";
            var bytes = Encoding.UTF8.GetBytes(configContent);

            return File(bytes, "text/plain", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var vpnConfig = await _context.VpnConfigs
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vpnConfig == null)
            {
                ViewBag.Error = "Конфигурация не найдена";
                return View("Index");
            }

            ViewBag.Config = vpnConfig;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vpnConfig = await _context.VpnConfigs
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vpnConfig == null)
            {
                TempData["Error"] = "Конфигурация не найдена";
                return RedirectToAction("List");
            }

            ViewBag.Config = vpnConfig;
            return View("DeleteConfirm");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vpnConfig = await _context.VpnConfigs
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vpnConfig == null)
            {
                TempData["Error"] = "Конфигурация не найдена";
                return RedirectToAction("List");
            }

            string configName = vpnConfig.Name;

            _context.VpnConfigs.Remove(vpnConfig);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Конфигурация '{configName}' полностью удалена из базы данных";
            return RedirectToAction("List");
        }
    }
}