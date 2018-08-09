using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.JsonModels;

namespace Premy.Chatovatko.Client.Libs.Database.InsertModels
{
    public class CAlarm : JAlarm ,ICInsertModel
    {
        public CAlarm(DateTime time, String text)
        {
            Date = time;
            Text = text;
        }


        public InsertModelTypes GetModelType()
        {
            return InsertModelTypes.ALARM;
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
