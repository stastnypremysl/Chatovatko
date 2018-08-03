using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Premy.Chatovatko.Libs
{
    public static class NetworkUtils
    {
        public static string GetIpAddress(TcpClient client)
        {
            return ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString();
        }
    }
}
