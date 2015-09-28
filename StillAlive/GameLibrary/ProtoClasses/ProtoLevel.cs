using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameObjects.DrawableClasses;

namespace GameObjects.ProtoClasses
{
    [ProtoContract]
    public class ProtoLevel
    {
        [ProtoMember(1)]
        public int Width;
        [ProtoMember(2)]
        public int Height;
        [ProtoMember(3)]
        public HashSet<ProtoSolid> Solids = new HashSet<ProtoSolid>();

        public static explicit operator Level(ProtoLevel protoLevel)
        {
            Level level = new Level(protoLevel.Width, protoLevel.Height);
            foreach (var protoSolid in protoLevel.Solids)
            {
                level.Solids.Add((DrawableSolid)protoSolid);
            }
            return level;
        }

        public static explicit operator ProtoLevel(Level level)
        {
            ProtoLevel protoLevel = new ProtoLevel();
            protoLevel.Width = level.Width;
            protoLevel.Height = level.Height;
            foreach (var protoSolid in level.Solids)
            {
                protoLevel.Solids.Add((ProtoSolid)protoSolid);
            }
            return protoLevel;
        }
    }
}
