using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Server.Database.Models
{
    public partial class Users
    {
        public Users()
        {
            BlobMessagesRecepient = new HashSet<BlobMessages>();
            BlobMessagesSender = new HashSet<BlobMessages>();
            PublicCertificatesRecepient = new HashSet<PublicCertificates>();
            PublicCertificatesSender = new HashSet<PublicCertificates>();
        }

        public int Id { get; set; }
        public string PublicCertificate { get; set; }
        public byte[] PublicCertificateSha1 { get; set; }
        public string UserName { get; set; }

        public ICollection<BlobMessages> BlobMessagesRecepient { get; set; }
        public ICollection<BlobMessages> BlobMessagesSender { get; set; }
        public ICollection<PublicCertificates> PublicCertificatesRecepient { get; set; }
        public ICollection<PublicCertificates> PublicCertificatesSender { get; set; }
    }
}
