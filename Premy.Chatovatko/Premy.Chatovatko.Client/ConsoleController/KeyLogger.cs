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
            keyWH = new AutoResetEvent(false);
        }

        public static EventWaitHandle keyWH = new AutoResetEvent(false);
        public static LinkedList<char> buffer;
        public static bool writeToConsole = false;
        private static void KeyLoggerJob()
        {
            while (true)
            {
                char key = Console.ReadKey(!writeToConsole).KeyChar;
                if (key == 4)
                {
                    Logger.Close();
                    Environment.Exit(0);
                }
                buffer.AddLast(key);
                keyWH.Set();
            }
        }
    }
}
