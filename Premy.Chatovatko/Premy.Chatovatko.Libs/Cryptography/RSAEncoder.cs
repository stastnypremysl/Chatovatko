using Premy.Chatovatko.Libs.DataTransmission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Libs.Cryptography
{
    public static class RSAEncoder
    {
        public static byte[] EncryptAndSign(byte[] data, X509Certificate2 receiverCert, X509Certificate2 myCert)
        {
            RSA publicKey = receiverCert.GetRSAPublicKey();

            MemoryStream stream = new MemoryStream();

            byte[] encrypted = publicKey.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            byte[] signiture = Sign(data, myCert);

            BinaryEncoder.SendBytes(stream, signiture);
            BinaryEncoder.SendBytes(stream, encrypted);

            stream.Flush();
            return stream.GetBuffer();
        }

        public static byte[] DecryptAndVerify(byte[] data, X509Certificate2 myCert, X509Certificate2 senderCert)
        {
            RSA privateKey = myCert.GetRSAPrivateKey();

            MemoryStream stream = new MemoryStream(data);

            byte[] signiture = BinaryEncoder.ReceiveBytes(stream);
            byte[] decrypted = privateKey.Decrypt(BinaryEncoder.ReceiveBytes(stream), RSAEncryptionPadding.OaepSHA256);

            if(!Verify(decrypted, signiture, senderCert))
            {
                throw new Exception("The signiture of AES key isn't correct.");
            }
            return decrypted;
        }

        public static byte[] Sign(byte[] data, X509Certificate2 cert)
        {
            RSA csp = cert.GetRSAPrivateKey();
            return csp.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        }

        public static bool Verify(byte[] data, byte[] signiture, X509Certificate2 cert)
        {
            RSA csp = cert.GetRSAPublicKey();
            return csp.VerifyData(data, signiture, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        }
    }
}
