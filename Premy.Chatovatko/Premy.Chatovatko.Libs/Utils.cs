using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Premy.Chatovatko.Libs
{
    public static class Utils
    {
        public static string GetIpAddress(TcpClient client)
        {
            return ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString();
        }

        public static byte[] GetBytes(String text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static String GetText(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
