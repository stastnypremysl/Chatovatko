using Premy.Chatovatko.Client.Libs.Database.JsonModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database
{
    public static class PullMessageParser
    {
        public static void ParseEncryptedMessage(Context context, byte[] message, long senderId, long messageId)
        {
            IJType decoded = JsonEncoder.GetJsonDecoded(context, message, senderId);
            switch (decoded.GetJsonType())
            {
                case JsonTypes.ALARM:
                    JAlarm alarm = (JAlarm)decoded;
                    context.Alarms.Add(new Alarms()
                    {
                        BlobMessagesId = messageId,
                        Text = alarm.Text,
                        Time = alarm.Date.GetChatovatkoString()
                    });
                    break;

                case JsonTypes.CONTACT_DETAIL:
                    JContactDetail detail = (JContactDetail)decoded;
                    context.ContactsDetail.Add(new ContactsDetail()
                    {
                        AlarmPermission = detail.ChangeContactPermission,
                        NickName = detail.NickName,
                        BlobMessagesId = messageId,
                        ContactId = detail.ContactId
                    });
                    break;

                case JsonTypes.MESSAGES:
                    JMessage jmessage = (JMessage)decoded;
                    context.Messages.Add(new Messages()
                    {
                        Date = jmessage.Time.GetChatovatkoString(),
                        Text = jmessage.Text,
                        IdMessagesThread = jmessage.MessageThreadId,
                        BlobMessagesId = messageId
                    });
                    break;

                case JsonTypes.MESSAGES_THREAD:
                    JMessageThread messageThread = (JMessageThread)decoded;
                    context.MessagesThread.Add(new MessagesThread
                    {
                        Name = messageThread.Name,
                        PublicId = messageThread.PublicId,
                        Onlive = messageThread.Onlive,
                        Archived = messageThread.Archived,
                        WithUser = messageThread.WithUserId,
                        BlobMessagesId = messageId
                    });
                    break;
            }
        }
    }
}
