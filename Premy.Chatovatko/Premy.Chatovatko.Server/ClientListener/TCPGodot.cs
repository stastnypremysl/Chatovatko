using Premy.Chatovatko.Server.Database;
using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class TCPGodot
    {

        private ulong id;
        private DBConnection connection;
        private Thread theLife;
        private bool readyForLife = false;

        public TCPGodot(ulong id)
        {
            this.id = id;
            Thread initLife = new Thread(() => Init());
            theLife = new Thread(() => MyJob());
            initLife.Start();
        }

        private void Init()
        {
            connection = DBPool.GetConnection();
            readyForLife = true;
            Logger.LogGodotInfo(id, "Godot has been created.");
        }

        public void Run()
        {
            theLife.Start();
        }
        
        private void MyJob()
        {
            while (!readyForLife)
            {
                Thread.Sleep(50);
            }
            Logger.LogGodotInfo(id, "Godot has been activated.");
            GodotFountain.IncreaseRunning();

            GodotFountain.IncreaseDestroyed();
            Logger.LogGodotInfo(id, "Godot has died.");
        }

    }
}
