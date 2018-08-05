using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.Cryptography;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Handshake;
using Premy.Chatovatko.Server.chatovatkoDb;
using Premy.Chatovatko.Server.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Premy.Chatovatko.Server.ClientListener.Scenarios
{
    public static class Handshake
    {
        public static UserCapsula Run(Stream stream, Action<string> log, ServerConfig config)
        {
            UserCapsula ret;

            ClientHandshake clientHandshake = TextEncoder.ReadClientHandshake(stream);
            X509Certificate2 clientCertificate = X509Certificate2Utils.ImportFromPem(clientHandshake.PemCertificate);
            log($"Logging user sent username {clientHandshake.UserName}\n Certificate:\n {clientHandshake.PemCertificate}");

            log("Generating random bytes");
            byte[] randomBytes = LUtils.GenerateRandomBytes(TcpConstants.HANDSHAKE_LENGHT);

            log("Sending encrypted bytes");
            BinaryEncoder.SendBytes(stream, RSAEncoder.Encrypt(randomBytes, clientCertificate));

            if (!randomBytes.Equals(BinaryEncoder.ReceiveBytes(stream)))
            {
                log("Sending error to client.");
                TextEncoder.SendJson(stream, new ServerHandshake()
                {
                    Errors = "Client's certificate verification failed.",
                    NewUser = false,
                    Succeeded = false,
                    UserId = -1,
                    UserName = ""
                });
                throw new Exception("Client's certificate verification failed.");
            }

            log("Certificate verification succeeded.");

            using(Context context = new Context(config))
            {
                //context.Users.SingleOrDefault(u => u.PublicKey == clientHandshake.PemCertificate);
            }


            //log($"Handshake successeded. User {ret.UserName} has loged in");
            return null;
        }
    }
}
