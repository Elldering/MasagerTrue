using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client
{
    [Serializable]
    internal class massage
    {

        public string Username { get; set; }
        public string Message { get; set; }
        public string IP { get; set; }
    }
}
