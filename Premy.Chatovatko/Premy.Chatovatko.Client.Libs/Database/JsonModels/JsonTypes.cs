using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database.JsonModels
{
    public enum JsonTypes : byte
    {
        ALARM = 1,
        CONTACT_DETAIL = 2,
        MESSAGES_THREAD = 3,
        MESSAGES = 4,
        AES_KEY = 5
    }
}
