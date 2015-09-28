using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using Microsoft.Xna.Framework;
using ProtoBuf;
using GameObjects.ProtoClasses;

namespace Virvar.Net
{
    [ProtoContract]
    public class ServerConnectionPacket
    {
        [ProtoMember(1)]
        public byte PlayerId;
        [ProtoMember(2)]
        public ProtoVector2 PlayerPosition;
        [ProtoMember(3)]
        public float Speed;
        [ProtoMember(4)]
        public byte Health;
        [ProtoMember(5)]
        public ProtoLevel Level;
    }
}
