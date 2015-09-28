using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace GameObjects
{
    [ProtoContract]
    public class Score
    {
        [ProtoMember(1)]
        public int PlayerId;
        [ProtoMember(2)]
        public int Kills = 0;
        [ProtoMember(3)]
        public int Deaths = 0;
    }
}
