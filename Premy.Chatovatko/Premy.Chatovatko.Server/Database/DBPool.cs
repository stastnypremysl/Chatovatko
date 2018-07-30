using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public class DBPool
    {
        private DatabaseIDGenerator idGenerator;
        private IServerLogger logger;
        private ServerConfig config;

        public DBPool(IServerLogger logger, ServerConfig config)
        {
            idGenerator = new DatabaseIDGenerator();
            this.logger = logger;
            this.config = config;
        }

        public DBConnection GetConnection()
        {
            return new DBConnection();
        }
}
    }
}
