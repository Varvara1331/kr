using System;

namespace demo.ViewModels
{
    public class AddressBookViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string InternalNumber { get; set; }
        public DateTime? UsedAt { get; set; }
        public bool HasSignature { get; set; }

        // URL для скачивания vCard
        public string VCardUrl => $"/addressbook/vcard/{Id}";
    }
}