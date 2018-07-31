using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client
{
    public static class Utils
    {
        public static String GetHomeDir()
        {
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            return homePath;
        }

        public static String GetConfigDirectory()
        {
            return GetHomeDir() + "/.chatovatko";
        }


        public static String GetConfigFile()
        {
            return GetConfigDirectory() + "/config.txt";
        }

        public static String GetClientCertificate()
        {
            return GetConfigDirectory() + "/client.crt";
        }

        public static String GetUserCertificate()
        {
            return GetConfigDirectory() + "/user.p12";
        }
    }
}
