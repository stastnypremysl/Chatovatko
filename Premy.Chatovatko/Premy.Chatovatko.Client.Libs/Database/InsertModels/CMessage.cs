using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.JsonModels;

namespace Premy.Chatovatko.Client.Libs.Database.InsertModels
{
    public class CMessage : JMessage, ICInsertModel
    {

        public CMessage(long messageThreadId, string text, DateTime time)
        {
            this.MessageThreadId = messageThreadId;
            this.Text = text;
            this.Time = time;
        }

        public InsertModelTypes GetModelType()
        {
            return InsertModelTypes.MESSAGE;
        }

        public IJType GetRecepientUpdate()
        {
            return this;
        }

        public IJType GetSelfUpdate()
        {
            return this;
        }
    }
}
