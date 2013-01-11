using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pusher.Server
{
    public interface IAuthenticationSignature
    {
        string auth { get; }
    }
}
