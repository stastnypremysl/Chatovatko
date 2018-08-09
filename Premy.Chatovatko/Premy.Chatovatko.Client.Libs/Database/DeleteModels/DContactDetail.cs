using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.Models;

namespace Premy.Chatovatko.Client.Libs.Database.DeleteModels
{
    public class DContactDetail : IDeleteModel
    {
        private readonly ContactsDetail contactDetail;
        private readonly long myUserId;

        public DContactDetail(ContactsDetail contactDetail, long myUserId)
        {
            this.contactDetail = contactDetail;
            this.myUserId = myUserId;
        }

        public void DoDelete(Context context)
        {
            context.ContactsDetail.Remove(contactDetail);
            PushOperations.DeleteBlobMessage(context, contactDetail.GetBlobId(), myUserId);
        }
    }
}
