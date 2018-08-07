using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public class PullMessageParser
    {
        private readonly Logger logger;
        private readonly SettingsCapsula settings;

        public PullMessageParser(Logger logger, SettingsCapsula settings)
        {

        }

        public void ParseEncryptedMessage(byte[] message, long senderId)
        {
            //byte[] decrypted =  settings
        }
    }
}
