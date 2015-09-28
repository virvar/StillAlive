using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameObjects.DrawableClasses;
using Microsoft.Xna.Framework;

namespace GameObjects.ProtoClasses
{
    [ProtoContract]
    public class ProtoSolid
    {
        [ProtoMember(1)]
        public ProtoPoint Offset;
        [ProtoMember(2)]
        public float Angle;
        [ProtoMember(3)]
        public ProtoVector2 Position;
        [ProtoMember(4)]
        public ProtoPoint Zone;
        [ProtoMember(5)]
        public ProtoPoint Size;

        public static explicit operator DrawableSolid(ProtoSolid protoSolid)
        {
            DrawableSolid solid = new DrawableSolid();
            solid.CollisionOffset = (Point)protoSolid.Offset;
            solid.Angle = protoSolid.Angle;
            solid.Position = (Vector2)protoSolid.Position;
            //solid.Zone = (Point)protoSolid.Zone;
            solid.Size = (Point)protoSolid.Size;
            return solid;
        }

        public static explicit operator ProtoSolid(Solid solid)
        {
            ProtoSolid protoSolid = new ProtoSolid();
            protoSolid.Offset = (ProtoPoint)solid.CollisionOffset;
            protoSolid.Angle = solid.Angle;
            protoSolid.Position = (ProtoVector2)solid.Position;
            //protoSolid.Zone = (ProtoPoint)solid.Zone;
            protoSolid.Size = (ProtoPoint)solid.Size;
            return protoSolid;
        }
    }
}
