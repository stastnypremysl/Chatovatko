using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
        private static int readyToExist = 10;

        private static List<TCPGodot> godotPool;

        public static void IncreaseRunning()
        {
            running++;
        }

        public static void IncreaseDestroyed()
        {
            destroyed++;
            running--;
        }


        public static void Run()
        {
            ServerCertificateFile = Config.certAddress;
            ServerCertificatePassword = Config.certPasswd;
            godotPool = new List<TCPGodot>();

            for(int i = 0; i != readyToExist; i++)
            {
                godotPool.Add(new TCPGodot(created++));
            }

            TcpListener listener = new TcpListener(IPAddress.Any, ServerPort);
            listener.Start();

        }
    }
}
