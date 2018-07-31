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

        public Logger(ILoggerOutput output) :base()
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

        public void Log(ILoggable me, String message, bool error)
        {

        }

        public void Log(object me, String message, bool error)
        {
            
        }

        public void LogException(ILoggable me, Exception exception)
        {

        }

        public void LogException(ChatovatkoException exception)
        {

        }



        public void Log(ILoggable me, String message) => Log(me, message);

        public void Log(object me, String message) => Log(me, message);
    }
}
