using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.Cryptography;
using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.Handshake;
using Premy.Chatovatko.Server.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Premy.Chatovatko.Server.ClientListener.Scenarios
{
    public static class Handshake
    {
        public static UserCapsula Run(Stream stream, Action<string> log)
        {
            ClientHandshake clientHandshake = TextEncoder.ReadClientHandshake(stream);
            X509Certificate2 clientCertificate = X509Certificate2Utils.ImportFromPem(clientHandshake.PemCertificate);

            byte[] randomBytes = LUtils.GenerateRandomBytes(TcpConstants.HANDSHAKE_LENGHT);
            BinaryEncoder.SendBytes(stream, RSAEncoder.Encrypt(randomBytes, clientCertificate));
            if (!randomBytes.Equals(BinaryEncoder.ReceiveBytes(stream)))
            {

            }

        }
    }
}
