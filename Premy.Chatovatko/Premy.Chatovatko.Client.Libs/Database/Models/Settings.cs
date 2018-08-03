using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Libs.Database.Models
{
    public partial class Settings
    {
        public long Id { get; set; }
        public long UserPublicId { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string ServerAddress { get; set; }
        public string ServerPublicKey { get; set; }
    }
}
