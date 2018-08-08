using Premy.Chatovatko.Client.Libs.Database.JsonModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Libs.DataTransmission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Transactions;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public static class PushOperations
    {
        public static void SendIJType(Context context, IJType toSend, long recepientId, long myUserId)
        {
            long? blobId = null;
            if (myUserId == recepientId)
            {
                var blobMessage = new BlobMessages()
                {
                    SenderId = myUserId,
                    PublicId = null,
                    DoDelete = 0,
                    Failed = 0
                };
                context.BlobMessages.Add(blobMessage);
                context.SaveChanges();

                blobId = blobMessage.Id;
                PullMessageParser.ParseIJTypeMessage(context, toSend, myUserId, blobMessage.Id, myUserId);
                context.SaveChanges();
            }

            context.ToSendMessages.Add(new ToSendMessages()
            {
                RecepientId = recepientId,
                BlobMessagesId = blobId,
                Blob = JsonEncoder.GetJsonEncoded(context, toSend, recepientId)
            });

            context.SaveChanges();

        }
    }
}

