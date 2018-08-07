using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database.JsonModels
{
    public class JContactDetail : IJType
    {
        public long ContactId { get; set; }
        public long ChangeContactPermission { get; set; }
        public string NickName { get; set; }

        public JsonTypes GetJsonType()
        {
            return JsonTypes.CONTACT_DETAIL;
        }
    }
}
