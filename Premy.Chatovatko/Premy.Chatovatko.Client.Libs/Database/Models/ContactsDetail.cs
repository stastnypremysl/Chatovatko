using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Models
{
    public partial class ContactsDetail
    {
        public long ContactId { get; set; }
        public long BlobMessagesId { get; set; }
        public string AlarmPermission { get; set; }
        public string ChangeContactsPermission { get; set; }
        public string NickName { get; set; }

        public BlobMessages BlobMessages { get; set; }
        public Contacts Contact { get; set; }
    }
}
