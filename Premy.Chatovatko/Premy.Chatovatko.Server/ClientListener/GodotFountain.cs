using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Premy.Chatovatko.Server.ClientListener
{
    public static class GodotFountain
    {
        private static readonly int ServerPort = 8471;

        private static ulong running = 0;
        private static ulong destroyed = 0;
        private static ulong created = 0;
        private static int readyToExist = 10;

        private static List<TCPGodot> godotPool;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IncreaseRunning()
        {
            running++;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IncreaseDestroyed()
        {
            destroyed++;
            running--;
        }


        public static void Run()
        {
            godotPool = new List<TCPGodot>();

            for(int i = 0; i != readyToExist; i++)
            {
                godotPool.Add(new TCPGodot(created++));
            }

            TcpListener listener = new TcpListener(IPAddress.Any, ServerPort);
            listener.Start();
            int ite = 0;
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                godotPool[ite].Run(client);
                ite++;
                godotPool.Add(new TCPGodot(created++));
            }
        }
    }
}
