using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    interface IWebHook
    {
        bool IsValid
        {
            get;
        }

        /** Returns all WebHook data.
         *
         * @throws WebHookError when WebHook is invalid
         * @returns {Object}
         */
        Dictionary<string, string>[] Events
        {
            get;
        }

        /** Returns WebHook timestamp.
         *
         * @throws WebHookError when WebHook is invalid
         * @returns {Date}
         */
        string Time
        {
            get;
        }

    }
}