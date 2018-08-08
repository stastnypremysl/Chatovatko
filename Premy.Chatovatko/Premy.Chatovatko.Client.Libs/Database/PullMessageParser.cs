using Premy.Chatovatko.Client.Libs.Database.JsonModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public static class PullMessageParser
    {
        public static void ParseEncryptedMessage(Context context, byte[] message, long senderId, long messageId, long myUserId)
        {
            IJType decoded = JsonEncoder.GetJsonDecoded(context, message, senderId);
            ParseIJTypeMessage(context, decoded, senderId, messageId, myUserId);    
        }

        public static void ParseIJTypeMessage(Context context, IJType decoded, long senderId, long messageId, long myUserId)
        {
            if(context.Contacts
                .Where(u => u.PublicId == senderId)
                .Select(u => u.Trusted)
                .SingleOrDefault() != 1)
            {
                throw new Exception($"User with id {senderId} isn't trusted.");
            }

            bool permission = senderId == myUserId;
            switch (decoded.GetJsonType())
            {
                case JsonTypes.ALARM:
                    JAlarm alarm = (JAlarm)decoded;
                    permission = permission || 
                        context.ContactsDetail
                        .Where(u => u.ContactId == senderId)
                        .Select(u => u.AlarmPermission)
                        .SingleOrDefault() == 1;

                    if (permission)
                    { 
                        context.Alarms.Add(new Alarms()
                        {
                            BlobMessagesId = messageId,
                            Text = alarm.Text,
                            Time = alarm.Date.GetChatovatkoString()
                        });
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to set alarm.");
                    }
                    break;

                case JsonTypes.CONTACT_DETAIL:
                    JContactDetail detail = (JContactDetail)decoded;
                    permission = permission ||
                       context.ContactsDetail
                       .Where(u => u.ContactId == senderId)
                       .Select(u => u.ChangeContactsPermission)
                       .SingleOrDefault() == 1;

                    if (permission)
                    {
                        context.ContactsDetail.Add(new ContactsDetail()
                        {
                            AlarmPermission = detail.ChangeContactPermission,
                            NickName = detail.NickName,
                            BlobMessagesId = messageId,
                            ContactId = detail.ContactId
                        });
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to set contact detail.");
                    }
                    break;

                case JsonTypes.MESSAGES:
                    JMessage jmessage = (JMessage)decoded;
                    permission = permission || (
                       from threads in context.MessagesThread
                       where threads.PublicId == jmessage.MessageThreadId
                       select threads.WithUser
                       ).SingleOrDefault() == senderId;

                    if (permission)
                    {
                        context.Messages.Add(new Messages()
                        {
                            Date = jmessage.Time.GetChatovatkoString(),
                            Text = jmessage.Text,
                            IdMessagesThread = jmessage.MessageThreadId,
                            BlobMessagesId = messageId
                        });
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to send this message.");
                    }
                    break;

                case JsonTypes.MESSAGES_THREAD:
                    JMessageThread messageThread = (JMessageThread)decoded;
                    permission = permission || messageThread.WithUserId == senderId;
                    if (permission)
                    {
                        context.MessagesThread.Add(new MessagesThread
                        {
                            Name = messageThread.Name,
                            PublicId = messageThread.PublicId,
                            Onlive = messageThread.Onlive,
                            Archived = messageThread.Archived,
                            WithUser = messageThread.WithUserId,
                            BlobMessagesId = messageId
                        });
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to create/edit this message thread.");
                    }
                    break;

                case JsonTypes.AES_KEY:
                    JAESKey aesKey = (JAESKey)decoded;
                    if (permission)
                    {
                        var contact = context.Contacts
                            .Where(c => c.PublicId == aesKey.UserId)
                            .SingleOrDefault();
                        if (contact.SendAesKey != null)
                        {

                        }
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to create AES keys to send.");
                    }
                    break;
            }
        }

        private static void DeleteBlobMessage(Context context, int BlobId)
        {
            
        }
    }

    
}
