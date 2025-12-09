using System;

namespace TempLinkApp.ViewModels
{
    public class LinkAccessViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}