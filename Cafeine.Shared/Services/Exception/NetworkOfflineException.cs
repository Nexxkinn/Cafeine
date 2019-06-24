using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Services.ExceptionExtension
{
    class NetworkOfflineException : System.Exception
    {
        public NetworkOfflineException() { }
        public NetworkOfflineException(string message) : base(message) { }
        public NetworkOfflineException(string message,System.Exception inner) : base(message, inner) { }
    }
}
