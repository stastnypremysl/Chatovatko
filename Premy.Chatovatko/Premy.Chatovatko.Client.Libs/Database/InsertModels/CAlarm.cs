using System;
using System.Collections.Generic;
using System.Text;
using Premy.Chatovatko.Client.Libs.Database.JsonModels;

namespace Premy.Chatovatko.Client.Libs.Database.InsertModels
{
    public class CAlarm : ICInsertModel
    {
        public InsertModelTypes GetModelType()
        {
            throw new NotImplementedException();
        }

        public IJType GetRecepientUpdate()
        {
            throw new NotImplementedException();
        }

        public IJType GetSelfUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
