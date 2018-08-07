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
            stackTrace = this.StackTrace;
            message = message + "\n" + this.GetType().Name + "\n" + this.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, me.GetLogSource(), DateTime.Now, true);
            
        }

        public ChatovatkoException(object me, String message) : base()
        {
            stackTrace = this.StackTrace;
            message = message + "\n" + this.GetType().Name + "\n" + this.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, "", DateTime.Now, true);
        }

        public ChatovatkoException(object me, Exception ex, String message) : base()
        {
            stackTrace = ex.StackTrace;
            message = message + "\n" + this.GetType().Name + "\n" + ex.Message + "\n" + ex.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, "", DateTime.Now, true);
        }

        public ChatovatkoException(ILoggable me, Exception ex, String message) : base()
        {
            stackTrace = ex.StackTrace;
            message = message + "\n" + this.GetType().Name + "\n" + ex.Message + "\n" + ex.StackTrace;
            theLogMessage = new DefaultLoggerMessage(me.GetType().Name, message, me.GetLogSource(), DateTime.Now, true);
        }


        private readonly string stackTrace;
        public override string StackTrace => stackTrace;
        public override string Message => theLogMessage.GetMessage();
        public ILoggerMessage GetLogMessage => theLogMessage;
    }
}
