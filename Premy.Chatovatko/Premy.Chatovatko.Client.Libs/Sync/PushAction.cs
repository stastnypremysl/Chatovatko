using Premy.Chatovatko.Client.Libs.ClientCommunication;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Sync
{
    public class PushAction : IAction, ILoggable
    {
        private readonly Action reconnect;
        private readonly Connection connection;
        private readonly Logger logger;
        private readonly SettingsCapsula settings;

        public static bool Changed { get; set; } = true;

        public PushAction(Connection connection, Action reconnect, Logger logger, SettingsCapsula settings)
        {
            this.reconnect = reconnect;
            this.connection = connection;
            this.logger = logger;
            this.settings = settings;
        }

        public string GetLogSource()
        {
            return "Automatized push action";
        }

        public IAction GetNext()
        {
            return new PushAction(connection, reconnect, logger, settings);
        }

        public void Run()
        {
            if (Changed)
            {
                try
                {
                    connection.Push();
                    Changed = false;
                }
                catch (Exception ex)
                {
                    logger.LogException(this, ex);
                    reconnect();
                }
            }
        }
    }
}
