using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Premy.Chatovatko.Client.Libs.Sync
{
    public class DelayAction : IAction
    {
        private int miliseconds;
        private DateTime timeToRun;

        public DelayAction(int miliseconds = 30)
        {
            this.miliseconds = miliseconds;
            this.timeToRun = DateTime.Now;
            timeToRun.AddMilliseconds(miliseconds);
        }

        public IAction GetNext()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            DateTime now = DateTime.Now;
            if (timeToRun > now)
            { 
                Thread.Sleep((timeToRun - now).Milliseconds);
            }
        }
    }
}
