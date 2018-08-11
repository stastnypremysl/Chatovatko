using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.JsonModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Premy.Chatovatko.Client.Libs.Database.InsertModels
{
    public class CMessageThread : ICInsertModel
    {
        private readonly long recepientId;
        private readonly long myUserId;
        public CMessageThread(Context context, String name, bool onlive, long recepientId, long myUserId, bool archived = false)
        {
            var settings = context.Settings.Single();
            PublicId = settings.LastUniqueId;
            ++settings.LastUniqueId;
            context.SaveChanges();

            this.recepientId = recepientId;
            this.myUserId = myUserId;

            Onlive = onlive;
            Name = name;
            Archived = archived;
        }

        public String Name { get; set; }
        public bool Onlive { get; set; }
        public long PublicId { get; }
        public bool Archived { get; set; }


        public InsertModelTypes GetModelType()
        {
            return InsertModelTypes.MESSAGE_THREAD;
        }

        public IJType GetRecepientUpdate()
        {
            return new JMessageThread()
            {
                Archived = Archived ? 1 : 0,
                Name = Name,
                Onlive = Onlive ? 1 : 0,
                PublicId = PublicId,
                WithUserId = myUserId
            };
        }

        public IJType GetSelfUpdate()
        {
            if(recepientId == myUserId)
            {
                return null;
            }
            return new JMessageThread()
            {
                Archived = Archived ? 1 : 0,
                Name = Name,
                Onlive = Onlive ? 1 : 0,
                PublicId = PublicId,
                WithUserId = recepientId
            };
        }
    }
}
