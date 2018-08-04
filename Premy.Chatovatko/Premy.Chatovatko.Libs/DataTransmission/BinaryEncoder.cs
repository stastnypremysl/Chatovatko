using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Premy.Chatovatko.Libs.DataTransmission.TextEncoder;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public static class BinaryEncoder
    {
        public static void SendBytesFromStream (byte[] toSend)
        {

        }

        public static byte[] ReceiveBytesFromStream(Stream stream)
        {
            int lenght = ReadIntFromStream();
        }
    }
}
