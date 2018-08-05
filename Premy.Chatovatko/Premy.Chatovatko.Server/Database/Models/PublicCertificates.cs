using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Server.chatovatkoDb
{
    public partial class PublicCertificates
    {
        public int Id { get; set; }
        public int RecepientId { get; set; }
        public int SenderId { get; set; }
        public string EncryptedAesKey { get; set; }
        public bool Trusted { get; set; }

        public Users Recepient { get; set; }
        public Users Sender { get; set; }
    }
}
