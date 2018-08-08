using Premy.Chatovatko.Client.Libs.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database.JsonModels
{
    /// <summary>
    /// Contains key for sending data
    /// </summary>
    public class JAESKey : IJType
    {
        public JAESKey(long UserId, AESPassword password)
        {
            this.UserId = UserId;
            this.AESKey = password.Password;
        }

        public JAESKey()
        {

        }

        public long UserId { get; set; }
        public byte[] AESKey { get; set; }

        public JsonTypes GetJsonType()
        {
            return JsonTypes.AES_KEY;
        }
    }
}
