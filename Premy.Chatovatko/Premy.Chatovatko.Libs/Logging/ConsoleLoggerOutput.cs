using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Server.Logging
{
    public class ConsoleLoggerOutput : ILoggerOutput
    {
        internal override void WriteToError(String text)
        {
            Console.Error.Write(String.Format("{0}; {1}\n", DateTime.Now.ToLongTimeString(), text));
        }

        internal override void WriteToOutput(String text)
        {
            Console.Error.Write(String.Format("{0}; {1}\n", DateTime.Now.ToLongTimeString(), text));
        }



    }
}
