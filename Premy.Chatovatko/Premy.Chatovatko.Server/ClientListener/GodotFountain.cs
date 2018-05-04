using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.ClientListener
{
    public static class GodotFountain
    {
        private static readonly int ServerPort = 8471;
        private static string ServerCertificateFile;
        private static string ServerCertificatePassword;

        private static ulong running = 0;
        private static ulong destroyed = 0;
        private static ulong created = 0;
        

        public static void Run()
        {
            ServerCertificateFile = Config.certAddress;
            ServerCertificatePassword = Config.certPasswd;
        }
    }
}
