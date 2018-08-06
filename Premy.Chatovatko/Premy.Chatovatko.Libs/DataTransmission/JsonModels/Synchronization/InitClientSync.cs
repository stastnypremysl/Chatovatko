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
        public IList<long?> PublicBlobMessagesIds { get; set; }

        /// <summary>
        /// Already uploaded users. (independetly on aes keys)
        /// </summary>
        public IList<long> UserIds { get; set; }

        /// <summary>
        /// Already uploaded aes keys.
        /// </summary>
        public IList<long> AesKeysUserIds { get; set; }
    }
}
