using System;
using System.ComponentModel.DataAnnotations;

namespace demo.Models
{
    public class VpnConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public string ConfigType { get; set; } = "OpenVPN";

        [Required]
        public string ServerAddress { get; set; }

        [Required]
        public int ServerPort { get; set; } = 1194;

        public string Protocol { get; set; } = "udp";

        [Required]
        public string ConfigContent { get; set; }

        [Required]
        public string CaCertificate { get; set; }

        [Required]
        public string ClientCertificate { get; set; }

        [Required]
        public string ClientKey { get; set; }

        public string TlsAuthKey { get; set; } = string.Empty;

        public string DhParameters { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string Cipher { get; set; } = "AES-256-GCM";
        public string Auth { get; set; } = "SHA256";
        public int Mtu { get; set; } = 1500;
        public bool RedirectGateway { get; set; } = true;
        public bool BlockDns { get; set; } = false;
        public string AdditionalOptions { get; set; } = string.Empty;
    }
}