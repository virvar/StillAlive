using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Virvar.Net
{
    [Serializable]
    [ProtoContract]
    public class ConnectionMessage
    {
        public string Login;
        [ProtoMember(1)]
        public int ClientPort;

        public ConnectionMessage() { }

        public ConnectionMessage(int localPort)
        {
            this.ClientPort = localPort;
        }
    }
}
