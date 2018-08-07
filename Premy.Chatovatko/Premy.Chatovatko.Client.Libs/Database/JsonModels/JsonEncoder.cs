using Premy.Chatovatko.Client.Libs.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using Premy.Chatovatko.Client.Libs.Cryptography;
using Premy.Chatovatko.Libs.Logging;

namespace Premy.Chatovatko.Client.Libs.Database.JsonModels
{
    public static class JsonEncoder
    {
        public static IJType GetJsonDecoded(Context context, byte[] message, long senderId)
        {
            byte[] aesBinKey = context.Contacts
                .Where(u => u.PublicId == senderId)
                .Select(u => u.ReceiveAesKey)
                .SingleOrDefault();

            AESPassword key = new AESPassword(aesBinKey);
            byte[] decrypted = key.Decrypt(message);
            JsonTypes type = (JsonTypes)decrypted[0];
            string jsonText = Encoding.UTF8.GetString(message, 1, message.Length - 1);

            switch (type)
            {
                case JsonTypes.ALARM:
                    return JsonConvert.DeserializeObject<JAlarm>(jsonText);

                case JsonTypes.CONTACT_DETAIL:
                    return JsonConvert.DeserializeObject<JContactDetail>(jsonText);

                case JsonTypes.MESSAGES:
                    return JsonConvert.DeserializeObject<JMessage>(jsonText);

                case JsonTypes.MESSAGES_THREAD:
                    return JsonConvert.DeserializeObject<JMessageThread>(jsonText);
                default:
                    throw new Exception("Unknown JsonType.");
            }

        }

        public static byte[] GetJsonEncoded(Context context, IJType message, long receiverId)
        {
            byte[] aesBinKey = context.Contacts
                .Where(u => u.PublicId == receiverId)
                .Select(u => u.SendAesKey)
                .SingleOrDefault();

            AESPassword key = new AESPassword(aesBinKey);

            MemoryStream stream = new MemoryStream();
            stream.WriteByte((byte)message.GetJsonType());

            string json = JsonConvert.SerializeObject(message);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(json);

            byte[] notEncrypted = stream.ToArray();
            return key.Encrypt(notEncrypted);
        }
    }
}
