using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Models
{
    public partial class BlobMessages
    {
        public long Id { get; set; }
        public long? PublicId { get; set; }
        public long RecepientId { get; set; }
        public long SenderId { get; set; }
        public string Downloaded { get; set; }
        public string Uploaded { get; set; }
        public string DoDelete { get; set; }
        public byte[] Blob { get; set; }

        public Contacts Recepient { get; set; }
        public Contacts Sender { get; set; }
        public Alarms Alarms { get; set; }
        public ContactsDetail ContactsDetail { get; set; }
        public Messages Messages { get; set; }
        public MessagesThread MessagesThread { get; set; }
    }
}
