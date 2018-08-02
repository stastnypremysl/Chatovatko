using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Models
{
    public partial class Contacts
    {
        public Contacts()
        {
            BlobMessagesRecepient = new HashSet<BlobMessages>();
            BlobMessagesSender = new HashSet<BlobMessages>();
        }

        public long PublicId { get; set; }
        public string UserName { get; set; }
        public string PublicKey { get; set; }
        public string SymmetricKey { get; set; }

        public ContactsDetail ContactsDetail { get; set; }
        public ICollection<BlobMessages> BlobMessagesRecepient { get; set; }
        public ICollection<BlobMessages> BlobMessagesSender { get; set; }
    }
}
