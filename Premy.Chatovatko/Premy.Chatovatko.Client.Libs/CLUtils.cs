using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Premy.Chatovatko.Client.Libs
{
    public static class CLUtils
    {
        internal static string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        internal static byte[] GenerateRandomBytes(int lenght)
        {
            byte[] ret = new byte[lenght];
            RNGCryptoServiceProvider.Create().GetBytes(ret);
            return ret;
        }
    }
}
