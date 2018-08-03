using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class InfoService : ILoggable
    {
        private Task backgroundWork = null;
        private readonly Logger logger;
        private readonly ServerConfig config;

        public InfoService(Logger logger, ServerConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        public void Run()
        {
            while (true)
            {
                try
                {

                }
                catch (Exception exception)
                {
                    logger.LogException(this, exception);
                }
            }
        }

        public void RunInBackground()
        {
            backgroundWork = Task.Run(() => Run());
        }

        public string GetLogSource()
        {
            return "Information tcp service";
        }
    }
}
