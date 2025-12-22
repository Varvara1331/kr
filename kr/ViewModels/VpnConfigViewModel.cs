using System;
using System.ComponentModel.DataAnnotations;

namespace demo.ViewModels
{
    public class VpnConfigViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название конфигурации обязательно")]
        [Display(Name = "Название конфигурации")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Тип конфигурации обязателен")]
        [Display(Name = "Тип VPN")]
        public string ConfigType { get; set; } = "OpenVPN";

        [Required(ErrorMessage = "Адрес сервера обязателен")]
        [Display(Name = "Адрес сервера")]
        public string ServerAddress { get; set; }

        [Required(ErrorMessage = "Порт сервера обязателен")]
        [Range(1, 65535, ErrorMessage = "Порт должен быть от 1 до 65535")]
        [Display(Name = "Порт сервера")]
        public int ServerPort { get; set; } = 1194;

        [Display(Name = "Протокол")]
        public string Protocol { get; set; } = "udp";

        [Required(ErrorMessage = "CA сертификат обязателен")]
        [Display(Name = "CA сертификат")]
        public string CaCertificate { get; set; }

        [Required(ErrorMessage = "Клиентский сертификат обязателен")]
        [Display(Name = "Клиентский сертификат")]
        public string ClientCertificate { get; set; }

        [Required(ErrorMessage = "Клиентский ключ обязателен")]
        [Display(Name = "Клиентский ключ")]
        public string ClientKey { get; set; }

        [Display(Name = "Ключ TLS аутентификации")]
        public string TlsAuthKey { get; set; }

        [Display(Name = "Параметры DH")]
        public string DhParameters { get; set; }

        [Display(Name = "Шифрование")]
        public string Cipher { get; set; } = "AES-256-GCM";

        [Display(Name = "Аутентификация")]
        public string Auth { get; set; } = "SHA256";

        [Display(Name = "MTU")]
        public int Mtu { get; set; } = 1500;

        [Display(Name = "Перенаправлять весь трафик")]
        public bool RedirectGateway { get; set; } = true;

        [Display(Name = "Блокировать DNS утечки")]
        public bool BlockDns { get; set; } = false;

        [Display(Name = "Дополнительные параметры")]
        public string AdditionalOptions { get; set; }

        [Display(Name = "Конфигурация по умолчанию")]
        public bool IsDefault { get; set; }

        [Display(Name = "Активна")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedAtFormatted => CreatedAt.ToString("dd.MM.yyyy HH:mm");
        public string UpdatedAtFormatted => UpdatedAt?.ToString("dd.MM.yyyy HH:mm") ?? "Не обновлялась";
    }
}