using Premy.Chatovatko.Libs.DataTransmission;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Server.Cryptography;
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
        private readonly int ServerPort = TcpConstants.MAIN_SERVER_PORT;

        
        private int readyToExist = 10;
        private readonly GodotCounter counter;

        private readonly Logger logger;
        private readonly ServerCert serverCert;

        private List<Godot> godotPool;
        private ServerConfig config;

        public GodotFountain(Logger logger, ServerConfig config, ServerCert serverCert)
        {
            this.counter = new GodotCounter();
            this.logger = logger;
            this.config = config;
            this.serverCert = serverCert;
        }

        public int ReadyToExist { get => readyToExist; set => readyToExist = value; }
                


        public void Run()
        {
            godotPool = new List<Godot>();

            for(int i = 0; i != ReadyToExist; i++)
            {
                godotPool.Add(new Godot(counter.Created, logger, config, serverCert, counter));
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
                godotPool.Add(new Godot(counter.Created, logger, config, serverCert, counter));
                counter.IncreaseCreated();
            }
        }
    }
}
