using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Database.Models
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
        public string PublicCertificate { get; set; }
        public long Trusted { get; set; }
        public string AesKey { get; set; }

        public ContactsDetail ContactsDetail { get; set; }
        public ICollection<BlobMessages> BlobMessagesRecepient { get; set; }
        public ICollection<BlobMessages> BlobMessagesSender { get; set; }
    }
}
