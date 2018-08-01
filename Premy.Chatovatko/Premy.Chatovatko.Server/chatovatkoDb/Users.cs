using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Server.chatovatkoDb
{
    public partial class Users
    {
        public Users()
        {
            BlobMessagesRecepient = new HashSet<BlobMessages>();
            BlobMessagesSender = new HashSet<BlobMessages>();
            PublicKeysRecepient = new HashSet<PublicKeys>();
            PublicKeysSender = new HashSet<PublicKeys>();
        }

        public int Id { get; set; }
        public byte[] PublicKey { get; set; }
        public string UserName { get; set; }

        public ICollection<BlobMessages> BlobMessagesRecepient { get; set; }
        public ICollection<BlobMessages> BlobMessagesSender { get; set; }
        public ICollection<PublicKeys> PublicKeysRecepient { get; set; }
        public ICollection<PublicKeys> PublicKeysSender { get; set; }
    }
}
