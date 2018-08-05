using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.Cryptography;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Handshake;
using Premy.Chatovatko.Server.Database.Models;
using Premy.Chatovatko.Server.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Premy.Chatovatko.Server.ClientListener.Scenarios
{
    public static class Handshake
    {
        public static UserCapsula Run(Stream stream, Action<string> log, ServerConfig config)
        {
            
            ClientHandshake clientHandshake = TextEncoder.ReadClientHandshake(stream);
            X509Certificate2 clientCertificate = X509Certificate2Utils.ImportFromPem(clientHandshake.PemCertificate);
            log($"Logging user sent username {clientHandshake.UserName}\n Certificate:\n {clientHandshake.PemCertificate}");

            log("Generating random bytes");
            byte[] randomBytes = LUtils.GenerateRandomBytes(TcpConstants.HANDSHAKE_LENGHT);

            log("Sending encrypted bytes");
            BinaryEncoder.SendBytes(stream, RSAEncoder.Encrypt(randomBytes, clientCertificate));

            ServerHandshake errorHandshake = new ServerHandshake()
            {
                Errors = "",
                NewUser = false,
                Succeeded = false,
                UserId = -1,
                UserName = ""
            };

            if (!randomBytes.Equals(BinaryEncoder.ReceiveBytes(stream)))
            {
                log("Sending error to client.");
                errorHandshake.Errors = "Client's certificate verification failed.";
                TextEncoder.SendJson(stream, errorHandshake);
                throw new Exception(errorHandshake.Errors);
            }

            log("Certificate verification succeeded.");

            Users user;
            using (Context context = new Context(config))
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] hash = sha.ComputeHash(clientCertificate.RawData);
                user = context.Users.SingleOrDefault(u => u.PublicCertificateSha1.Equals(hash));

                if (user == null){
                    log("User doesn't exist yet. I'll try to create him.");

                    log("Checking the uniquity of username.");
                    String userName = clientHandshake.UserName;
                    if (context.Users.SingleOrDefault(u => u.UserName.Equals(userName)) != null)
                    {
                        log("Username isn't unique.");
                        errorHandshake.Errors = "Username isn't unique.";
                        TextEncoder.SendJson(stream, errorHandshake);
                        throw new Exception(errorHandshake.Errors);
                    }

                    log("Creating user.");
                    user = new Users()
                    {
                        PublicCertificate = clientHandshake.PemCertificate,
                        PublicCertificateSha1 = hash,
                        UserName = clientHandshake.UserName
                    };
                    context.Users.Add(user);
                    context.SaveChanges();
                    log("User successfully created.");
                }
                else{
                    log("User exists.");
                }
            }

            UserCapsula ret = new UserCapsula(user, clientCertificate);

            log($"Handshake successeded. User {ret.UserName} with id {ret.UserId} has logged in");
            return null;
        }
    }
}
