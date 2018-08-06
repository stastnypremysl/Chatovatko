using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public enum ConnectionCommand
    {
        UNTRUST_CONTACT = 0,
        TRUST_CONTACT = 1,

        /// <summary>
        /// Client wants to send aes key.
        /// </summary>
        SEND_AES_KEY = 3,

        /// <summary>
        /// Client wants to push.
        /// </summary>
        PUSH = 5,
        /// <summary>
        /// Client wants to pull.
        /// </summary>
        PULL = 6,

        END_CONNECTION = 10
    }
}
