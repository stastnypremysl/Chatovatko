using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public class PullMessageParser : ILoggable
    {
        private readonly Logger logger;
        private readonly long clientUserId;

        public PullMessageParser(Logger logger, long clientUserId)
        {
            this.logger = logger;
            this.clientUserId = clientUserId;
        }

        public string GetLogSource()
        {
            return "Pull message parser";
        }

        private void Log(String message)
        {
            logger.Log(this, message);
        }

        /// <summary>
        /// If successful, returns true.
        /// </summary>
        public bool ParseEncryptedMessage(Context context, byte[] message, long senderId, long publicId)
        {
            throw new NotImplementedException();
        }
    }
}
