using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.Push
{
    public class PushCapsula
    {
        public IList<PushMessage> PushMessages { get; set; }
        public IList<long> MessageToDeleteIds { get; set; }
    }
}
