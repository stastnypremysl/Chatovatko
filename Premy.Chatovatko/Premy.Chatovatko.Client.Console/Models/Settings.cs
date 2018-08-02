using System;
using System.Collections.Generic;

namespace Premy.Chatovatko.Client.Console.Models
{
    public partial class Settings
    {
        public long Id { get; set; }
        public long PublicId { get; set; }
        public string PrivateKey { get; set; }
        public string Publicy { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string ServerAddress { get; set; }
    }
}
