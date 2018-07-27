using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Logging
{
    public interface IServerLogger
    {
        void LogDatabaseError(String error);
        void LogDatabaseInfo(String output);
        void LogConnectionError(ulong id, String error);
        void LogConnectionInfo(ulong id, String output);
        void LogGodotError(ulong id, String error);
        void LogGodotInfo(ulong id, String output);
        void LogConfigError(String output);
        void LogClientListenerInfo(String output);
        void LogClientListenerError(String output);


    }
}
