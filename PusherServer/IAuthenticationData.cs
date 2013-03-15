using System.Runtime.Serialization;

namespace PusherServer
{
    public interface IAuthenticationData
    {
        string auth { get;  }

        /// <summary>
        /// Double encoded JSON containing presence channel user information.
        /// </summary>
        string channel_data { get;  }


        /// <summary>
        /// Returns a Json representation of the authentication data.
        /// </summary>
        /// <returns></returns>
        string ToJson();
    }
}
