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
            BinaryEncoder.SendBytes(stream, RSAEncoder.EncryptAndSign(randomBytes, clientCertificate));

            ServerHandshake errorHandshake = new ServerHandshake()
            {
                Errors = "",
                NewUser = false,
                Succeeded = false,
                UserId = -1,
                UserName = ""
            };

            byte[] received = BinaryEncoder.ReceiveBytes(stream);
            if (!randomBytes.SequenceEqual(received))
            {
                log("Client's certificate verification failed.");
                errorHandshake.Errors = "Client's certificate verification failed.";
                TextEncoder.SendJson(stream, errorHandshake);
                throw new Exception(errorHandshake.Errors);
            }

            log("Certificate verification succeeded.");

            Users user;
            String message;
            Clients client;
            bool newUser = false;
            using (Context context = new Context(config))
            {
                byte[] hash = SHA256.Create().ComputeHash(clientCertificate.RawData);
                user = context.Users.SingleOrDefault(u => u.PublicCertificateSha2.SequenceEqual(hash));

                if (user == null){
                    log("User doesn't exist yet. I'll try to create him.");
                    newUser = true;

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
                        PublicCertificateSha2 = hash,
                        UserName = clientHandshake.UserName
                    };

                    context.Users.Add(user);
                    context.SaveChanges();

                    message = "User successfully created.";
                    log("User successfully created.");
                }
                else{
                    message = "User exists.";
                    log("User exists.");
                }

                client = new Clients()
                {
                    UserId = user.Id
                };

                if (clientHandshake.ClientId == null)
                {     
                    context.Add(client);
                    context.SaveChanges();

                    log($"Added client with Id {client.Id}.");
                }
                else
                {
                    client.Id = (int)clientHandshake.ClientId;
                    if(context.Clients.Where(u => u.Id == client.Id).Single().UserId != user.Id)
                    {
                        errorHandshake.Errors = "This client id isn't owned by this user.";
                        TextEncoder.SendJson(stream, errorHandshake);
                        throw new Exception(errorHandshake.Errors);
                    }
                    
                    log($"Client with Id {client.Id} has logged in.");
                }


            }

            ServerHandshake toSend = new ServerHandshake()
            {
                Errors = message,
                NewUser = newUser,
                Succeeded = true,
                UserId = user.Id,
                UserName = user.UserName,
                ClientId = client.Id
            };
            TextEncoder.SendJson(stream, toSend);

            UserCapsula ret = new UserCapsula(user, clientCertificate);
            log($"Handshake successeded. User {ret.UserName} with id {ret.UserId} has logged in");
            return ret;
        }
    }
}
