using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Models
{
    public partial class MessagesThread
    {
        public MessagesThread()
        {
            Messages = new HashSet<Messages>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long RecepientId { get; set; }
        public string Onlive { get; set; }
        public string Archived { get; set; }
        public long BlobMessagesId { get; set; }
        public long PublicId { get; set; }

        public BlobMessages BlobMessages { get; set; }
        public ICollection<Messages> Messages { get; set; }
    }
}
