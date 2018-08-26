using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Console.Models
{
    public partial class ToSendMessages
    {
        public long Id { get; set; }
        public long RecepientId { get; set; }
        public byte[] Blob { get; set; }
        public long? BlobMessagesId { get; set; }

        public BlobMessages BlobMessages { get; set; }
        public Contacts Recepient { get; set; }
    }
}
