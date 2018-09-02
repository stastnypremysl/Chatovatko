using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Sync
{
    public interface IAction
    {
        void Run();
        IAction GetNext();
    }
}
