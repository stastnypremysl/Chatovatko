using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Database.Models
{
    public partial class ContactsDetail : IBlobbed
    {
        public long ContactId { get; set; }
        public long BlobMessagesId { get; set; }
        public long AlarmPermission { get; set; }
        public long ChangeContactsPermission { get; set; }
        public string NickName { get; set; }

        public BlobMessages BlobMessages { get; set; }
        public Contacts Contact { get; set; }

        public long GetBlobId()
        {
            return BlobMessagesId;
        }
    }
}
