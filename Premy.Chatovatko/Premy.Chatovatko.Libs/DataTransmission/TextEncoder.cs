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
            int lenght = BinaryEncoder.ReadInt(stream);
            return LUtils.GetText(BinaryEncoder.ReceivePureBytes(stream, lenght));
        }

        public static void SendString(Stream stream, String message)
        {
            byte[] byteStr = LUtils.GetBytes(message);
            BinaryEncoder.SendInt(stream, byteStr.Length);
            BinaryEncoder.SendPureBytes(stream, byteStr);
        }


        public static void SendJson(Stream stream, object obj)
        {
            String json = JsonConvert.SerializeObject(obj);
            SendString(stream, json);
        }


        public static T ReadJson<T>(Stream stream)
        {
            String json = ReadString(stream);
            return JsonConvert.DeserializeObject<T>(json);
        }


    }
}
