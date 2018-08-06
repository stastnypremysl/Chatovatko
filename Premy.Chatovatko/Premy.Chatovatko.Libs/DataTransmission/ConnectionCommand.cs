using System;
using System.Collections.Generic;
using System.Text;

namespace Premy.Chatovatko.Libs.DataTransmission
{
    public enum ConnectionCommand
    {
        UNTRUST_CONTACT = 0,
        ADD_CONTACT = 1,

        /// <summary>
        /// Client wants to push.
        /// </summary>
        PUSH = 2,
        /// <summary>
        /// Client wants to pull.
        /// </summary>
        PULL = 3,

        END_CONNECTION = 4
    }
}
