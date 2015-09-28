using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Microsoft.Xna.Framework;

namespace GameObjects.ProtoClasses
{
    [ProtoContract]
    public struct ProtoVector2
    {
        [ProtoMember(1)]
        public float X;
        [ProtoMember(2)]
        public float Y;

        public ProtoVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public static explicit operator Vector2(ProtoVector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static explicit operator ProtoVector2(Vector2 vector)
        {
            return new ProtoVector2(vector.X, vector.Y);
        }
    }
}
