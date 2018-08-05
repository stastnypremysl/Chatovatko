using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Libs.Cryptography
{
    public static class RSAEncoder
    {
        public static byte[] Encrypt(byte[] data, X509Certificate2 cert)
        {
            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)cert.PublicKey.Key;
            return publicKey.Encrypt(data, true);
        }

        public static byte[] Decrypt(byte[] encrypted, X509Certificate2 cert)
        {
            RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)cert.PrivateKey;
            return privateKey.Decrypt(encrypted, true);
        }
    }
}
