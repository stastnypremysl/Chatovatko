using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Premy.Chatovatko.Client.Libs.UserData
{
    public class SettingsLoader : ILoggable
    {
        private readonly Logger logger;
        private readonly IClientDatabaseConfig config;
        public SettingsLoader(Logger logger, IClientDatabaseConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        public bool Exists()
        {
            using(SqlClientContext context = new SqlClientContext(config))
            {
                return context.Settings.Any(o => true);
            }
        }

        public void Create()
        {
            if (Exists())
            {
                throw new ChatovatkoException(this, "Settings already exists");
            }
            
        }

        public string GetLogSource()
        {
            return "Settings creator and loader";
        }
    }
}
