using System;
using System.Collections.Generic;
using System.Text;


namespace Premy.Chatovatko.Client.Libs.Database
{
    public class DBPool
    {
        private readonly String connectionString;
        public DBPool(String address)
        {
            connectionString = String.Format("URI=file:{0}", address);
        }
       

    }
}
