using Premy.Chatovatko.Libs;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Cryptography
{
    public class AESPassword
    {
        private readonly byte[] password;
        public AESPassword(String bitPassword)
        {
            this.password = Convert.FromBase64String(bitPassword);
        }

        public AESPassword(byte[] password)
        {
            this.password = password;
        }

        public byte[] Password => password;

        public static AESPassword GenerateAESPassword()
        {
            return new AESPassword(LUtils.GenerateRandomBytes(AESConstants.PASSWORD_LENGHT));
        }

        public String GetBitPassword()
        {
            return Convert.ToBase64String(password);
        }


    }
}
