using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database.JsonModels
{
    public class JMessageThread : IJType
    {
        public string Name { get; set; }
        public long Onlive { get; set; }
        public long Archived { get; set; }
        public long WithUserId { get; set; }
        public long PublicId { get; set; }

        public JsonTypes GetJsonType()
        {
            return JsonTypes.MESSAGES_THREAD;
        }
    }
}
