using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Premy.Chatovatko.Client.Libs.Sync
{
    public class DelayAction : IAction
    {
        public int Miliseconds { get; set; }
        private DateTime timeToRun;

        public DelayAction(int miliseconds = 500)
        {
            Miliseconds = miliseconds;
            this.timeToRun = DateTime.Now;
            timeToRun.AddMilliseconds(miliseconds);
        }

        public IAction GetNext()
        {
            return new DelayAction(Miliseconds);
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
