using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization
{
    public class ClientPullCapsula
    {
        public IList<long> AesKeysUserIds { get; set; }
    }
}
