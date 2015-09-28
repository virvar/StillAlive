using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Microsoft.Xna.Framework;

namespace GameObjects.ProtoClasses
{
    [ProtoContract]
    public struct ProtoPoint
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;

        public ProtoPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static explicit operator Point(ProtoPoint point)
        {
            return new Point(point.X, point.Y);
        }

        public static explicit operator ProtoPoint(Point point)
        {
            return new ProtoPoint(point.X, point.Y);
        }
    }
}
