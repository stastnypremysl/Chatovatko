using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.UserData;

namespace Premy.Chatovatko.Client.Console
{
    internal class ConsoleClientDatabaseConfig : IClientDatabaseConfig
    {
        public string DatabaseAddress => Utils.GetDatabaseAddress();
    }
}
