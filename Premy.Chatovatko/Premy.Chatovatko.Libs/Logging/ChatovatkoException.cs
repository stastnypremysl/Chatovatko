using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.Logging
{
    public class ChatovatkoException : Exception
    {
        private readonly ILoggerMessage theLogMessage;
        public ChatovatkoException(ILoggable me, String message) : base()
        {
            message = message + "\n" + this.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, me.GetLogSource(), DateTime.Now, true);
            
        }

        public ChatovatkoException(object me, String message) : base()
        {
            message = message + "\n" + this.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, "", DateTime.Now, true);
        }

        public ChatovatkoException(object me, Exception ex, String message) : base()
        {
            message = message + "\n" + ex.Message + "\n" + ex.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, "", DateTime.Now, true);
        }

        public ILoggerMessage GetLogMessage => theLogMessage;
    }
}
