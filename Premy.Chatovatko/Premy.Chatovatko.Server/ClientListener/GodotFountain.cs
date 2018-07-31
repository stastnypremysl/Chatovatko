using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.Database;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Premy.Chatovatko.Server.ClientListener
{
    public class GodotFountain
    {
        private readonly int ServerPort = 8471;

        
        private int readyToExist = 10;
        private readonly GodotCounter counter;

        private readonly Logger logger;
        private readonly DBPool pool;
        private readonly ServerCert serverCert;

        private List<Godot> godotPool;

        public GodotFountain(Logger logger, DBPool pool, ServerCert serverCert)
        {
            this.counter = new GodotCounter();
            this.logger = logger;
            this.pool = pool;
            this.serverCert = serverCert;
        }

        public int ReadyToExist { get => readyToExist; set => readyToExist = value; }
                


        public void Run()
        {
            godotPool = new List<Godot>();

            for(int i = 0; i != ReadyToExist; i++)
            {
                godotPool.Add(new Godot(counter.Created, logger, pool, serverCert, counter));
                counter.IncreaseCreated();
            }

            TcpListener listener = new TcpListener(IPAddress.Any, ServerPort);
            listener.Start();
            int ite = 0;
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                godotPool[ite].Run(client);
                ite++;
                godotPool.Add(new Godot(counter.Created, logger, pool, serverCert, counter));
                counter.IncreaseCreated();
            }
        }
    }
}
