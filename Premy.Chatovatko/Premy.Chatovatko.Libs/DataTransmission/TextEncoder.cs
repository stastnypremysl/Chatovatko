using Newtonsoft.Json;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Handshake;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization;
using System;
using System.IO;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public static class TextEncoder
    {
        
        public static String ReadString(Stream stream)
        {
            int lenght = ReadInt(stream);
            return LUtils.GetText(BinaryEncoder.ReceivePureBytes(stream, lenght));
        }

        public static void SendString(Stream stream, String message)
        {
            byte[] byteStr = LUtils.GetBytes(message);
            SendInt(stream, byteStr.Length);
            stream.Write(byteStr, 0, byteStr.Length);
        }

        public static int ReadInt(Stream stream)
        {
            byte[] readed = new byte[4];
            stream.Read(readed, 0, 4);
            return BitConverter.ToInt32(readed, 0);
        }

        public static void SendInt(Stream stream, int toSend)
        {
            byte[] data = BitConverter.GetBytes(toSend);
            stream.Write(data, 0, 4);
        }

        public static void SendCommand(Stream stream, ConnectionCommand command)
        {
            SendInt(stream, (int)command);
        }

        public static ConnectionCommand ReadCommand(Stream stream)
        {
            int readed = ReadInt(stream);
            return (ConnectionCommand)readed;
        }

        public static void SendJson(Stream stream, object obj)
        {
            String json = JsonConvert.SerializeObject(obj);
            SendString(stream, json);
        }

        // ////////////////////////////////////////////////
        // Json readers
        // ////////////////////////////////////////////////

        public static T ReadJson<T>(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<T>(json);
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

        public static ClientHandshake ReadClientHandshake(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<ClientHandshake>(json);
        }
        
        public static ServerHandshake ReadServerHandshake(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<ServerHandshake>(json);
        }

        public static InitClientSync ReadInitClientSync(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<InitClientSync>(json);
        }

        public static PullCapsula ReadPullCapsula(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<PullCapsula>(json);
        }

    }
}
