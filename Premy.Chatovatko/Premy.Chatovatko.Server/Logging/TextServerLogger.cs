using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Logging
{
    public abstract class TextServerLogger : IServerLogger
    {
        internal abstract void WriteToOutput(String text);
        internal abstract void WriteToError(String text);



        ///<remark>
        ///Meaningful only with WriteToOutput and WriteToError
        ///</remark>
        private static String GetProperOutput(String nameOfUtility, String message)
        {
            return String.Format("{0}; {1}", nameOfUtility, message);
        }

        public void LogDatabaseError(String error)
        {
            WriteToError(GetProperOutput("Database", error));
        }

        public void LogDatabaseInfo(String output)
        {
            WriteToOutput(GetProperOutput("Database", output));
        }

        public void LogConnectionError(ulong id, String error)
        {
            WriteToError(GetProperOutput(String.Format("Database, Connection {0}", id), error));
        }

        public void LogConnectionInfo(ulong id, String output)
        {
            WriteToOutput(GetProperOutput(String.Format("Database, Connection {0}", id), output));
        }

        public void LogGodotError(ulong id, String error)
        {
            WriteToError(GetProperOutput(String.Format("Godot {0}", id), error));
        }

        public void LogGodotInfo(ulong id, String output)
        {
            WriteToOutput(GetProperOutput(String.Format("Godot {0}", id), output));
        }

        public void LogConfigError(String output)
        {
            WriteToError(GetProperOutput("Config", output));
        }

        public void LogClientListenerInfo(String output)
        {
            WriteToOutput(GetProperOutput("Client listener", output));
        }

        public void LogClientListenerError(String output)
        {
            WriteToError(GetProperOutput("Client listener", output));
        }

        public void LogCoreError(String output)
        {
            WriteToError(GetProperOutput("Core", output));
        }
    }
}
