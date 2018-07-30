using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Premy.Chatovatko.Server.Database
{
    internal class DatabaseIDGenerator
    {
        private ulong theLastOne = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ulong getNext()
        {
            return theLastOne++;
        }
    }
}
