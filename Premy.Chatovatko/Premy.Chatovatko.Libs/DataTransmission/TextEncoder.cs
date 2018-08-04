using Newtonsoft.Json;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels;
using System;
using System.IO;
using System.Net.Security;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public static class TextEncoder
    {
        
        private static readonly string CONST_SURFIX = "<E42x?OF>";

        public static String ReadString(Stream stream)
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

            return messageData.ToString().Substring(0, messageData.Length - CONST_SURFIX.Length);
        }

        public static void SendString(Stream stream, String message)
        {
            byte[] bytes = Utils.GetBytes(message + CONST_SURFIX);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static int ReadInt(Stream stream)
        {
            return Int32.Parse(ReadString(stream));
        }

        public static void SendInt(Stream stream, int toSend)
        {
            SendString(stream, toSend.ToString());
        }

        public static void SendJson(Stream stream, object obj)
        {
            String json = JsonConvert.SerializeObject(obj);
            SendString(stream, json);
        }

        public static ServerConnectionInfo ReadServerConnectionInfo(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<ServerConnectionInfo>(json);
        }

        public static ServerInfo ReadServerInfo(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<ServerInfo>(json);
        }

    }
}
