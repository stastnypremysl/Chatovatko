using Premy.Chatovatko.Client.Libs.Database.JsonModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Client.Libs.Database.UpdateModels
{
    public class UContactDetail : IUpdateModel
    {
        public UContactDetail(ContactsDetail detail)
        {
            NickName = detail.NickName;
            ContactId = detail.ContactId;
            BlobMessageId = detail.BlobMessagesId;
            AlarmPermission = detail.AlarmPermission == 1;
            ChangeContactsPermission =  detail.ChangeContactsPermission == 1;
        }

        public string NickName { get; set; }
        public long ContactId { get; }
        public long BlobMessageId { get; }
        public bool AlarmPermission { get; set; }
        public bool ChangeContactsPermission { get; set; }

        public UpdateModelTypes GetModelType()
        {
            return UpdateModelTypes.CONTACT_DETAIL;
        }

        public IJType GetSelfUpdate()
        {
            return null;
        }

        public IJType GetRecepientUpdate()
        {
            return new JContactDetail()
            {
                ChangeContactPermission = this.ChangeContactsPermission ? 1 : 0,
                ContactId = this.ContactId,
                NickName = this.NickName
            };
        }
    }
}
