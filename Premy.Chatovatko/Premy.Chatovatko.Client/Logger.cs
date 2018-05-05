using Premy.Chatovatko.Client.ConsoleController;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client
{
    public static class Logger
    {
        private static TextOutput logOut;
        public static void Init()
        {
            logOut = new TextOutput();
            logOut.EnableErrorOutput();
            try
            { 
                logOut.AddFileOut(String.Format("./Logs/catalina{0}.out", DateTime.Today.ToOADate()));
            }
            catch
            {
                LogCore("Can't create log file.");
            }
        }

        public static void Close()
        {
            logOut.Close();
        }

        public static void DisableConsoleOut()
        {
            logOut.DisableErrorOutput();
        }

        public static void EnableConsoleOut()
        {
            logOut.EnableErrorOutput();
        }

        private static void Write(String text)
        {
            logOut.Write(String.Format("{0}; {1}\n", DateTime.Now.ToLongTimeString(), text));
        }

        ///<remark>
        ///Meaningful only with Write
        ///</remark>
        private static String GetProperOutput(String nameOfUtility, String message)
        {
            return String.Format("{0}; {1}", nameOfUtility, message);
        }

        public static void LogCore(String output)
        {
            Write(GetProperOutput("Core", output));
        }

        public static void LogUserData(String output)
        {
            Write(GetProperOutput("UserData", output));
        }
    }
}
