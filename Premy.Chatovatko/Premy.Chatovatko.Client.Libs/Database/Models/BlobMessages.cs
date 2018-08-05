using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Database.Models
{
    public partial class BlobMessages
    {
        public long Id { get; set; }
        public long? PublicId { get; set; }
        public long RecepientId { get; set; }
        public long SenderId { get; set; }
        public long Downloaded { get; set; }
        public long Uploaded { get; set; }
        public long DoDelete { get; set; }
        public byte[] Blob { get; set; }

        public Contacts Recepient { get; set; }
        public Contacts Sender { get; set; }
        public Alarms Alarms { get; set; }
        public ContactsDetail ContactsDetail { get; set; }
        public Messages Messages { get; set; }
        public MessagesThread MessagesThread { get; set; }
    }
}
