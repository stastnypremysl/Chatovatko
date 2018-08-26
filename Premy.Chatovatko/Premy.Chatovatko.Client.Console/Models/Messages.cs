using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Console.Models
{
    public partial class Messages
    {
        public long Id { get; set; }
        public long IdMessagesThread { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public long BlobMessagesId { get; set; }

        public BlobMessages BlobMessages { get; set; }
        public MessagesThread IdMessagesThreadNavigation { get; set; }
    }
}
