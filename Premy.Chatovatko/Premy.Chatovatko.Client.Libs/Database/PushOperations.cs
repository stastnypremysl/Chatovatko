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

namespace Premy.Chatovatko.Client.Libs.Database
{
    public static class PushOperations
    {

        public static void SendIJType(Context context, IJType toSend, long recepientId, long myUserId)
        {
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

        public static void Insert(Context context, String name, bool onlive, long recepientId, long myUserId)
        {
            long publicId = myUserId << 32;
            publicId += context.MessagesThread
                .OrderByDescending(u => u.Id)
                .Select(u => u.Id)
                .FirstOrDefault() + 1;

            JMessageThread forMe = new JMessageThread()
            {
                Archived = 0,
                Name = name,
                Onlive = onlive ? 1 : 0,
                PublicId = publicId,
                WithUserId = recepientId
            };
            SendIJType(context, forMe, myUserId, myUserId);

            JMessageThread forRecepient = new JMessageThread()
            {
                Archived = 0,
                Name = name,
                Onlive = onlive ? 1 : 0,
                PublicId = publicId,
                WithUserId = myUserId
            };
            SendIJType(context, forRecepient, recepientId, myUserId);
            
        }

        public static void Update(Context context, MessagesThread thread, long myUserId)
        {
            
            SendIJType(context, forMe, myUserId, myUserId);

            JMessageThread forRecepient = new JMessageThread()
            {
                Archived = thread.Archived,
                Name = thread.Name,
                Onlive = onlive ? 1 : 0,
                PublicId = publicId,
                WithUserId = myUserId
            };
            SendIJType(context, forRecepient, recepientId, myUserId);

            
        }

    }
}

