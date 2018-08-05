using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Premy.Chatovatko.Libs.DataTransmission.TextEncoder;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public static class BinaryEncoder
    {
        public static void SendBytes (Stream mainStream, byte[] toSend)
        {
            SendInt(mainStream, toSend.Length);
            mainStream.Write(toSend, 0, toSend.Length);
        }

        public static byte[] ReceiveBytes(Stream mainStream)
        {
            int lenght = ReadInt(mainStream);

            byte[] buffer = new byte[lenght];
            mainStream.Read(buffer, 0, lenght);
            return buffer;

        }
    }
}
