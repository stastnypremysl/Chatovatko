using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Premy.Chatovatko.Libs.DataTransmission.TextEncoder;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public static class BinaryEncoder
    {
        public static void SendBytes(Stream mainStream, byte[] toSend)
        {
            SendInt(mainStream, toSend.Length);
            SendPureBytes(mainStream, toSend);
        }

        public static byte[] ReceiveBytes(Stream mainStream)
        {
            int lenght = ReadInt(mainStream);
            return ReceivePureBytes(mainStream, lenght);

        }

        public static byte[] ReceivePureBytes(Stream stream, int resultSize)
        {
            MemoryStream memStream = new MemoryStream();
            byte[] buffer = new byte[2048];
            while (resultSize != 0)
            {
                if (buffer.Length < resultSize)
                {
                    resultSize -= buffer.Length;
                    stream.Read(buffer, 0, buffer.Length);
                    memStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    stream.Read(buffer, 0, resultSize);
                    memStream.Write(buffer, 0, resultSize);
                    resultSize = 0;
                }
            }
            return memStream.ToArray();
        }

        public static void SendPureBytes(Stream stream, byte[] bytes)
        {
            int index = 0;
            int packageSize = 2048;
            int remaining = bytes.Length;

            while (remaining != 0)
            {
                if (packageSize < remaining)
                {
                    stream.Write(bytes, index, packageSize);
                    remaining -= packageSize;
                    index += packageSize;
                }
                else
                {
                    stream.Write(bytes, index, remaining);
                    remaining = 0;
                }
            }
        }
    }
}
