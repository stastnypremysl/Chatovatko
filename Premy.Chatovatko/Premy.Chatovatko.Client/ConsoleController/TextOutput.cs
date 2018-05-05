using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Premy.Chatovatko.Client.ConsoleController
{
    
    public class TextOutput
    {
        public Dictionary<String, StreamWriter> outs;
        public TextOutput()
        {
            outs = new Dictionary<String, StreamWriter>();
        }

        public void EnableStandartOutput()
        {
            StreamWriter sw = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(sw);
            outs.Add("stdOut", sw);
        }

        public void DisableStandartOutput()
        {
            if (outs.ContainsKey("stdOut"))
            {
                StreamWriter sw = outs["stdOut"];
                outs.Remove("stdOut");
                sw.Close();
                sw.Dispose();                
            }
        }

        public void EnableErrorOutput()
        {
            StreamWriter sw = new StreamWriter(Console.OpenStandardError())
            {
                AutoFlush = true
            };
            Console.SetOut(sw);
            outs.Add("stdErr", sw);
        }

        public void DisableErrorOutput()
        {
            if (outs.ContainsKey("stdErr"))
            {
                StreamWriter sw = outs["stdErr"];
                outs.Remove("stdErr");
                sw.Close();
                sw.Dispose();
            }
        }

        public void AddFileOut(String path)
        {

        }
    }
}
