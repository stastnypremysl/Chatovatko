using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization
{
    public class ServerPullCapsula
    {
        public IList<PullMessage>  Messages { get; set; }
        public IList<long> AesKeysUserIds { get; set; }
    }
}
