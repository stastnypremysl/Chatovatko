using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.SearchContact
{
    public class SearchCServerCapsula
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PemCertificate { get; set; }
        public bool Succeeded { get; set; }
    }
}
