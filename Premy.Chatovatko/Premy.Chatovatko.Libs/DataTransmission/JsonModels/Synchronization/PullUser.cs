using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization
{
    public class PullUser
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string PublicCertificate { get; set; }
    }
}
