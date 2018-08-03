using MySql.Data.MySqlClient;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    public class DBPool
    {
        private readonly Logger logger;
        private readonly ServerConfig config;

        public DBPool(Logger logger, ServerConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        public MySqlConnection GetConnection()
        {
            MySqlConnection theConnection = new MySqlConnection(config.ConnectionString);
            theConnection.Open();
            return theConnection;
        }
    }
}
