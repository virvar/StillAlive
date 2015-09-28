using GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using Xna = Microsoft.Xna.Framework;

namespace MapsEditor
{
    class SubLevel
    {
        public static SubLevel Instance { get; private set; }
        public Point Position { get; set; }
        public Point Size { get; set; }
        public List<SubSolid> Solids { get; set; }
        public List<SubRespawnPoint> RespawnPoints { get; set; }

        public SubLevel()
        {
            Instance = this;
            Solids = new List<SubSolid>();
            RespawnPoints = new List<SubRespawnPoint>();
        }
    }

    class SubObject
    {
        private static int _curId = 1;
        public int Id { get; set; }
        public Point Position { get; set; }
        public SubSolidState State { get; set; }

        public SubObject()
        {
            Id = _curId++;
        }

        public static void ResetId()
        {
            _curId = 1;
        }
    }

    class SubSolid : SubObject
    {
        public Point Size { get; set; }
        public float Angle { get; set; }
        public PointF[] Vertices { get; private set; }

        public SubSolid()
        {
            Vertices = new PointF[4];
        }

        public void CalcVertices()
        {
            double R = Math.Pow(Math.Pow(Size.X, 2) + Math.Pow(Size.Y, 2), 0.5) / 2.0;
            double phi1 = Math.Atan((double)Size.Y / Size.X);
            double phi2 = Math.PI - phi1;
            phi1 += Angle;
            phi2 += Angle;

            Point center = new Point(SubLevel.Instance.Position.X + Position.X + (Size.X >> 1),
               SubLevel.Instance.Position.Y + Position.Y + (Size.Y >> 1));

            Vertices[0] = new PointF((float)(center.X + R * Math.Cos(phi2)), (float)(center.Y + R * Math.Sin(phi2)));
            Vertices[1] = new PointF((float)(center.X + R * Math.Cos(phi1)), (float)(center.Y + R * Math.Sin(phi1)));
            Vertices[2] = new PointF((float)(center.X - R * Math.Cos(phi2)), (float)(center.Y - R * Math.Sin(phi2)));
            Vertices[3] = new PointF((float)(center.X - R * Math.Cos(phi1)), (float)(center.Y - R * Math.Sin(phi1)));
        }

        public void OnLevelLocationChanged(Point delta)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i] = new PointF(Vertices[i].X + delta.X, Vertices[i].Y + delta.Y);
            }
        }

        public Solid ToSolid()
        {
            var solid = new Solid()
            {
                Angle = Angle,
                Position = new Xna.Vector2(Position.X + (Size.X >> 1), Position.Y + (Size.Y >> 1)),
                Size = new Xna.Point(Size.X, Size.Y)
            };
            solid.InitAxes();
            return solid;
        }
    }

    class SubRespawnPoint : SubObject
    {
        public const int Offset = 10;
    }

    enum SubSolidState
    {
        Creating,
        Created,
        Saved
    }

    enum ObjectType
    {
        Solid,
        RespawnPoint,
    }
}
