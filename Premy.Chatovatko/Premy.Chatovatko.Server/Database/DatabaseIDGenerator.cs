using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    internal static class DatabaseIDGenerator
    {
        private static ulong theLastOne = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ulong getNext()
        {
            return theLastOne++;
        }
    }
}
