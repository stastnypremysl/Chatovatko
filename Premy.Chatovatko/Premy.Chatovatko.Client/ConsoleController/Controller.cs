using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Premy.Chatovatko.Client.ConsoleController
{
    public static class Controller
    {
        private static Thread keyLogger;
        
        private static void Run()
        {
            keyLogger = new Thread(() => KeyLoggerJob());
            keyLogger.Start();
        }

        public static LinkedList<int> buffer;
        private static void KeyLoggerJob()
        {
            while (true)
            {
                int key = Console.Read();
                if (key == -1)
                { 
                    Environment.Exit(0);
                }
                buffer.AddLast(key);
            }
        }
    }
}
