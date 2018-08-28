using Premy.Chatovatko.Client.Libs.Database.JsonModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Libs.DataTransmission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Transactions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Premy.Chatovatko.Client.Libs.Database.InsertModels;
using Premy.Chatovatko.Client.Libs.Database.UpdateModels;
using Premy.Chatovatko.Client.Libs.Database.DeleteModels;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public static class PushOperations
    {

        public static void SendIJType(Context context, JsonCapsula toSend, long recepientId, long myUserId)
        {
            if(toSend == null)
            {
                return;
            }

            var recepient = context.Contacts
                .Where(u => u.PublicId == recepientId)
                .SingleOrDefault();
            if(recepient == null)
            {
                throw new Exception($"User is not downloaded in local database.");
            }
            else if  (recepient.Trusted != 1)
            {
                throw new Exception($"User {recepient.PublicId} ({recepient.UserName}) is not trusted.");
            }

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

        public static void Insert(Context context, ICInsertModel model, long recepientId, long myUserId)
        {
            SendIJType(context, model.GetSelfUpdate(), myUserId, myUserId);
            SendIJType(context, model.GetRecepientUpdate(), recepientId, myUserId);
            
        }

        public static void Update(Context context, IUpdateModel model, long recepientId, long myUserId)
        {
            SendIJType(context, model.GetSelfUpdate(), myUserId, myUserId);
            SendIJType(context, model.GetRecepientUpdate(), recepientId, myUserId);            
        }

        public static void Delete(Context context, IDeleteModel model)
        {
            model.DoDelete(context);
        }

        internal static void DeleteBlobMessage(Context context, long BlobId, long myUserId)
        {
            var message = context.BlobMessages
                .Where(u => u.Id == BlobId)
                .SingleOrDefault();
            message.DoDelete = 1;
            if (message.SenderId == myUserId)
            {
                var toDelete = context.ToSendMessages
                    .Where(u => u.BlobMessagesId == BlobId)
                    .SingleOrDefault();
                if(toDelete != null)
                { 
                    context.ToSendMessages.Remove(toDelete);
                }

            }
        }

    }
}

