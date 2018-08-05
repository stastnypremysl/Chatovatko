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
            RSA publicKey = cert.GetRSAPublicKey();
            return publicKey.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
        }

        public static byte[] Decrypt(byte[] encrypted, X509Certificate2 cert)
        {
            RSA privateKey = cert.GetRSAPrivateKey();
            return privateKey.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA1);
        }
    }
}
