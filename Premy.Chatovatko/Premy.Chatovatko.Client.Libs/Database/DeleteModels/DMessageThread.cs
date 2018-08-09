using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Premy.Chatovatko.Client.Libs.Database.DeleteModels
{
    public class DMessageThread : IDeleteModel
    {
        private readonly MessagesThread thread;
        private readonly long myUserId;

        public DMessageThread(MessagesThread thread, long myUserId)
        {
            this.thread = thread;
            this.myUserId = myUserId;
        }

        public void DoDelete(Context context)
        {
            foreach(var message in context.Messages
                .Where(u => u.IdMessagesThread == thread.PublicId))
            {
                DMessage dMessage = new DMessage(message, myUserId);
                dMessage.DoDelete(context);
            }

            context.MessagesThread.Remove(thread);
            //PushOperations.DeleteBlobMessage(context, thread.GetBlobId(), myUserId);
            //Piggy bug fix
        }
    }
}
