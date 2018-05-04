using Premy.Chatovatko.Server.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Premy.Chatovatko.Server
{
    static class Config
    {
        public static String connectionString = "";
        public static String certPasswd = "";
        public static String certAddress = "";
        public static void LoadConfig()
        {
            using (StreamReader sr = new StreamReader("./config.txt"))
            {
                while (!sr.EndOfStream) {
                    String line = sr.ReadLine();
                    String[] parts = line.Split('=');
                    String name = parts[0];
                    String rest = "";
                    for (int i = 0; i != parts.Length; i++)
                    { 
                        rest += parts[i];
                        if(i != parts.Length - 1)
                        {
                            rest += '=';
                        }
                    }

                    switch (parts[0])
                    {
                        case "ConnectionString":
                            connectionString = rest;
                            break;
                        case "CertPasswd":
                            certPasswd = rest;
                            break;
                        case "CertAddress":
                            certAddress = rest;
                            break;
                        default:
                            Logger.LogConfigError(String.Format("The parameter {0} doesn't exist.", name));
                            break;
                    }
                }
            }
        }
    }
}
