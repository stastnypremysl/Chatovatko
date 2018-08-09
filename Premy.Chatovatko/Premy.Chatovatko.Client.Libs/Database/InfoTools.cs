using Premy.Chatovatko.Client.Libs.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public static class InfoTools
    {
        public static long GetMessageSenderUserId(Context context, long messageId)
        {
            var message = context.Messages
                .Where(u => u.Id == messageId)
                .Single();

            var blobMessage = context.BlobMessages.
                Where(u => u.Id == message.BlobMessagesId)
                .Single();

            return blobMessage.SenderId;
        }
    }
}
