using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Premy.Chatovatko.Client.ConsoleController
{
    public static class KeyLogger
    {
        private static Thread keyLogger;
        
        public static void Run()
        {
            buffer = new LinkedList<char>();
            keyLogger = new Thread(() => KeyLoggerJob());
            keyLogger.Start();
        }

        public static LinkedList<char> buffer;
        private static void KeyLoggerJob()
        {
            while (true)
            {
                char key = Console.ReadKey(true).KeyChar;
                if (key == 4)
                { 
                    Environment.Exit(0);
                }
                buffer.AddLast(key);
            }
        }
    }
}
