using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission.JsonModels.Synchronization
{
    public class InitClientSync
    {
        /// <summary>
        /// Already uploaded blob messages.
        /// </summary>
        public List<int> PublicBlobMessagesIds { get; set; }

        /// <summary>
        /// Already uploaded users. (independetly on aes keys)
        /// </summary>
        public List<int> UserIds { get; set; }

        /// <summary>
        /// Already uploaded aes keys.
        /// </summary>
        public List<int> AesKeysUserIds { get; set; }
    }
}
