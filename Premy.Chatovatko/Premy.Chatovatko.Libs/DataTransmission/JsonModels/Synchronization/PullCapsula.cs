using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization
{
    public class PullCapsula
    {
        public IList<PullUser> Users { get; set; }
        public IList<long> TrustedUserIds { get; set; }
        public IList<PullMessage>  Messages { get; set; }

        public IList<long> AesKeysUserIds { get; set; }
    }
}
