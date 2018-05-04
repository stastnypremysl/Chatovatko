using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Logging
{
    public static class Logger
    {
        private static void WriteToError(String text)
        {
            Console.Error.Write(String.Format("{0}; {1}\n", DateTime.Now.ToLongTimeString(), text));
        }

        private static void WriteToOutput(String text)
        {
            Console.Error.Write(String.Format("{0}; {1}\n", DateTime.Now.ToLongTimeString(), text));
        }


        ///<remark>
        ///Meaningful only with WriteToOutput and WriteToError
        ///</remark>
        private static String GetProperOutput(String nameOfUtility, String message)
        {
            return String.Format("{0}; {1}", nameOfUtility, message);
        }

        public static void LogDatabaseError(String error)
        {
            WriteToOutput(GetProperOutput("Database", error));
        }

        public static void LogDatabaseInfo(String output)
        {
            WriteToOutput(GetProperOutput("Database", output));
        }

        public static void LogConnectionError(ulong id, String error)
        {
            WriteToOutput(GetProperOutput(String.Format("Database, Connection {0}", id), error));
        }

        public static void LogConnectionInfo(ulong id, String output)
        {
            WriteToOutput(GetProperOutput(String.Format("Database, Connection {0}", id), output));
        }

        public static void LogGodotError(ulong id, String error)
        {
            WriteToOutput(GetProperOutput(String.Format("Godot {0}", id), error));
        }

        public static void LogGodotInfo(ulong id, String output)
        {
            WriteToOutput(GetProperOutput(String.Format("Godot {0}", id), output));
        }

        public static void LogConfigError(String output)
        {
            WriteToOutput(GetProperOutput("Config", output));
        }

        public static void LogClientListenerInfo(String output)
        {
            WriteToOutput(GetProperOutput("Client listener", output));
        }

        public static void LogClientListenerError(String output)
        {
            WriteToOutput(GetProperOutput("Client listener", output));
        }

    }
}
