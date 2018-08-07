using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database.JsonModels
{
    public class JMessageThread
    {
        public string Name { get; set; }
        public bool Onlive { get; set; }
        public bool Archived { get; set; }
        public long WithUserId { get; set; }
        public long PublicId { get; set; }
    }
}
