using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.Logging
{
    public class Logger
    {
        private List<ILoggerOutput> outputs;

        public Logger()
        {
            outputs = new List<ILoggerOutput>();
        }

        public Logger(ILoggerOutput output) :this()
        {
            outputs.Add(output);
        }

        public List<ILoggerOutput> LoggerOutputs { get => outputs; }

        public void Log(ILoggerMessage message)
        {
            foreach(ILoggerOutput output in outputs)
            {
                output.Log(message);
            }
        }

        public void Log(String name, String source, String message, bool error)
        {
            DefaultLoggerMessage theLogMessage = new DefaultLoggerMessage(name, message, source, DateTime.Now, error);
            Log(theLogMessage);
        }

        public void Log(ILoggable me, String message, bool error)
        {
            DefaultLoggerMessage theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, me.GetLogSource(), DateTime.Now, error);
            Log(theLogMessage);
        }

        public void Log(object me, String message, bool error)
        {
            DefaultLoggerMessage theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, "", DateTime.Now, error);
            Log(theLogMessage);
        }

        public void LogException(ILoggable me, Exception exception)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(exception.Message);
            builder.Append("\n");
            builder.Append(exception.StackTrace);
            Log(me, builder.ToString(), true);
        }

        public void LogException(ChatovatkoException exception)
        {
            Log(exception.GetLogMessage);
        }



        public void Log(ILoggable me, String message) => Log(me, message);

        public void Log(object me, String message) => Log(me, message);
    }
}
