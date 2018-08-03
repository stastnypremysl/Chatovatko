using System;
using System.IO;
using System.Net.Security;
using System.Text;

namespace Premy.Chatovatko.Libs
{
    public static class TextEncoder
    {
        private static byte[] GetBytes(String text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private static String GetText(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        private static readonly string CONST_SURFIX = "<E42x?OF>";

        public static String ReadStringFromStream(Stream stream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = stream.Read(buffer, 0, buffer.Length);

                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
               
                if (messageData.ToString().IndexOf(CONST_SURFIX) != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString().Substring(0, messageData.Length - 5);
        }

        public static void SendStringToStream(Stream stream, String message)
        {
            byte[] bytes = GetBytes(message + CONST_SURFIX);
            stream.Write(GetBytes(message + "<EOF>"), 0, bytes.Length);
        }
    }
}
