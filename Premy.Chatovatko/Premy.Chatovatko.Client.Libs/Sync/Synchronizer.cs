using Premy.Chatovatko.Client.Libs.ClientCommunication;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Premy.Chatovatko.Client.Libs.Sync
{
    public class Synchronizer
    {
        public Queue<IAction> Queue { get; set; }

        public Synchronizer(Connection connection, Action reconnect, Logger logger, SettingsCapsula settings)
        {
            Queue = new Queue<IAction>();
            Queue.Enqueue(new PullAction(connection, reconnect, logger, settings));
            Queue.Enqueue(new PushAction(connection, reconnect, logger, settings));
            Queue.Enqueue(new DelayAction());
        }

        public void Run()
        {
            Task.Run(() =>
            {
                while (Queue.Count != 0)
                {
                    IAction action = Queue.Dequeue();
                    action.Run();
                    Queue.Enqueue(action.GetNext());
                }
            });
        }
    }
}
