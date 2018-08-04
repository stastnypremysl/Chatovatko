using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Premy.Chatovatko.Libs.DataTransmission.TextEncoder;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public static class BinaryEncoder
    {
        public static void SendBytes (Stream mainStream, Stream dataStream, byte[] toSend)
        {
            SendInt(mainStream, toSend.Length);
            dataStream.Write(toSend, 0, toSend.Length);
        }

        public static byte[] ReceiveBytes(Stream mainStream, Stream dataStream)
        {
            int lenght = ReadInt(mainStream);

            byte[] buffer = new byte[lenght];
            dataStream.Read(buffer, 0, lenght);
            return buffer;

        }
    }
}
