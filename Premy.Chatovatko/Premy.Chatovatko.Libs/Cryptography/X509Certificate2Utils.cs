using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Libs.Cryptography
{
    public static class X509Certificate2Utils
    {
        /// <summary>
        /// Exports the certificate without private key
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static string ExportToPem(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        public static X509Certificate2 ImportFromPem(string pem)
        {
            X509Certificate2 cer = new X509Certificate2(LUtils.GetBytes(pem));
            return cer;
        }

        /// <summary>
        /// Includes primary key
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static byte[] ExportToPkcs12(X509Certificate2 cert)
        {
            return cert.Export(X509ContentType.Pkcs12, String.Empty);
        }        

        /// <summary>
        /// Includes primary key
        /// Creates Base64 of Pkcs12
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static string ExportToBase64(X509Certificate2 cert)
        {
            return Convert.ToBase64String(ExportToPkcs12(cert), Base64FormattingOptions.InsertLineBreaks);
            
        }

        public static X509Certificate2 ImportFromPkcs12(byte[] data, bool exportable = false)
        {
            X509KeyStorageFlags flag;
            if (exportable)
            {
                flag = X509KeyStorageFlags.Exportable;
            }
            else
            {
                flag = X509KeyStorageFlags.PersistKeySet;
            }
            return new X509Certificate2(data, String.Empty, flag);
        }

        public static X509Certificate2 ImportFromBase64(string base64, bool exportable = false)
        {
            return ImportFromPkcs12(Convert.FromBase64String(base64), exportable);

        }

        public static void ExportToPkcs12File(X509Certificate2 cert, String fileName)
        {
            LUtils.WriteToFile(ExportToPkcs12(cert), fileName);
        }

        public static X509Certificate2 ImportFromPkcs12File(String fileName, bool exportable = false)
        {
            byte[] certData = LUtils.ReadFromFile(fileName);
            return ImportFromPkcs12(certData, exportable);
        }


    }
}
