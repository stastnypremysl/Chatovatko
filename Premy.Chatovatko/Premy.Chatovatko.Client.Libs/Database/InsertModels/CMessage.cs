using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.JsonModels;

namespace Premy.Chatovatko.Client.Libs.Database.InsertModels
{
    public class CMessage : JMessage, ICInsertModel
    {
        private readonly long recepientId;
        private readonly long myUserId;
        public CMessage(long messageThreadPublicId, string text, DateTime time, long recepientId, long myUserId)
        {
            this.MessageThreadId = messageThreadPublicId;
            this.Text = text;
            this.Time = time;
            this.recepientId = recepientId;
            this.myUserId = myUserId;
        }

        public InsertModelTypes GetModelType()
        {
            return InsertModelTypes.MESSAGE;
        }

        public IJType GetRecepientUpdate()
        {
            if (recepientId == myUserId)
            {
                return null;
            }
            return this;
        }

        public IJType GetSelfUpdate()
        {
            return this;
        }
    }
}
