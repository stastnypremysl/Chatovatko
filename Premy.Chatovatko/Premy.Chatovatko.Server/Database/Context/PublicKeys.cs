using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Server.chatovatkoDb
{
    public partial class PublicKeys
    {
        public int Id { get; set; }
        public int RecepientId { get; set; }
        public int SenderId { get; set; }
        public byte[] EncryptedSymKey { get; set; }
        public bool? Trusted { get; set; }

        public Users Recepient { get; set; }
        public Users Sender { get; set; }
    }
}
