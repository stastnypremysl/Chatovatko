using Premy.Chatovatko.Client.Libs.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.UserData
{
    public class SettingsCapsula
    {
        public Settings Settings { get; }
        public SettingsCapsula(Settings settings)
        {
            Settings = settings;
        }


    }
}
