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
            if(decoded == null)
            {
                return;
            }
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
                        context.SaveChanges();
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
                        var toDelete = context.ContactsDetail
                            .Where(u => u.ContactId == detail.ContactId)
                            .SingleOrDefault();
                        if(toDelete != null)
                        {
                            context.ContactsDetail.Remove(toDelete);
                            PushOperations.DeleteBlobMessage(context, toDelete.BlobMessagesId, myUserId);
                        }
                        context.SaveChanges();

                        context.ContactsDetail.Add(new ContactsDetail()
                        {
                            AlarmPermission = detail.ChangeContactPermission,
                            NickName = detail.NickName,
                            BlobMessagesId = messageId,
                            ContactId = detail.ContactId
                        });
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to set contact detail.");
                    }
                    break;

                case JsonTypes.MESSAGES:
                    JMessage jmessage = (JMessage)decoded;
                    long threadWithUser =(
                       from threads in context.MessagesThread
                       where threads.PublicId == jmessage.MessageThreadId
                       select threads.WithUser
                       ).SingleOrDefault();
                    permission = permission || threadWithUser == senderId;

                    if (permission)
                    {
                        bool onlive = (from threads in context.MessagesThread
                                       where threads.PublicId == jmessage.MessageThreadId
                                       select threads.Onlive)
                            .SingleOrDefault() == 1;

                        if (onlive)
                        {
                            var toDeleteInfo = (from bmessages in context.BlobMessages
                                            join messages in context.Messages on bmessages.Id equals messages.BlobMessagesId
                                            where bmessages.SenderId == senderId && messages.IdMessagesThread == jmessage.MessageThreadId
                                            select new { messages.BlobMessagesId, messages.Id })
                                            .SingleOrDefault();
                            if(toDeleteInfo != null)
                            {
                                var toDelete = context.Messages
                                    .Where(m => m.Id == toDeleteInfo.Id)
                                    .SingleOrDefault();
                                context.Messages.Remove(toDelete);
                                PushOperations.DeleteBlobMessage(context, toDeleteInfo.BlobMessagesId, myUserId);
                            }
                        }
                        context.SaveChanges();

                        context.Messages.Add(new Messages()
                        {
                            Date = jmessage.Time.GetChatovatkoString(),
                            Text = jmessage.Text,
                            IdMessagesThread = jmessage.MessageThreadId,
                            BlobMessagesId = messageId
                        });
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to send this message.");
                    }
                    break;

                case JsonTypes.MESSAGES_THREAD:
                    JMessageThread messageThread = (JMessageThread)decoded;
                    permission = permission || (messageThread.WithUserId == senderId && !messageThread.DoOnlyDelete);
                    if (permission)
                    {
                        var old = context.MessagesThread
                            .Where(u => u.PublicId == messageThread.PublicId)
                            .SingleOrDefault();
                        if(messageThread.DoOnlyDelete && old != null)
                        { 
                            context.Remove(old);
                        }
                        else if (messageThread.DoOnlyDelete)
                        {

                        }
                        else if(old != null)
                        {
                            old.Name = messageThread.Name;
                            old.BlobMessagesId = messageId;
                            old.Archived = messageThread.Archived;
                        }
                        else                        
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
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to create/edit/delete this message thread.");
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
                            throw new Exception($"AES key of user {contact.UserName} already exist.");
                        }
                        contact.SendAesKey = aesKey.AESKey;
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"User with id {senderId} doesn't have permission to create AES keys to send.");
                    }
                    break;
            }
        }

        
    }

    
}
