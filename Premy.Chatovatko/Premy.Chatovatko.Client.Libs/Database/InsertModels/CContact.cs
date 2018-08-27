using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.JsonModels;

namespace Premy.Chatovatko.Client.Libs.Database.InsertModels
{
    public class CContact: JContact, ICInsertModel
    {
        public CContactDetail(bool alarmPermission, bool changeContactPermission, string nickName, long contactId)
        {
            AlarmPermission = alarmPermission;
            ChangeContactPermission = changeContactPermission ? 1 : 0;
            NickName = nickName;
            ContactId = contactId;
        }


        public InsertModelTypes GetModelType()
        {
            return InsertModelTypes.CONTACT_DETAIL;
        }

        public IJType GetRecepientUpdate()
        {
            return this;
        }

        public IJType GetSelfUpdate()
        {
            return null;
        }
    }
}
