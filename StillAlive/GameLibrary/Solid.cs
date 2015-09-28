using System;
using Microsoft.Xna.Framework;

namespace GameObjects
{
    /// <summary>
    /// Rectangle with tilt.
    /// </summary>
    public class Solid : ISolid
    {
        // todo:избавиться от одного из Collision
        public Rectangle CollisionRect { get; set; }
        public Point CollisionOffset { get; set; }
        public float Angle { get; set; }
        public Vector2 Position { get; set; }
        public Point Size { get; set; }
        public Vector2[] Vertices { get; private set; }
        public Vector2[] Axes { get; set; }

        public void InitAxes()
        {
            Vertices = new Vector2[4];
            double R = Math.Pow(Math.Pow(Size.X, 2) + Math.Pow(Size.Y, 2), 0.5) / 2.0;
            double phi1 = Math.Atan((double)Size.Y / Size.X);
            double phi2 = MathHelper.Pi - phi1;
            phi1 += Angle;
            phi2 += Angle;
            Vertices[0] = new Vector2((float)(Position.X + R * Math.Cos(phi2)), (float)(Position.Y + R * Math.Sin(phi2)));
            Vertices[1] = new Vector2((float)(Position.X + R * Math.Cos(phi1)), (float)(Position.Y + R * Math.Sin(phi1)));
            Vertices[2] = new Vector2((float)(Position.X - R * Math.Cos(phi2)), (float)(Position.Y - R * Math.Sin(phi2)));
            Vertices[3] = new Vector2((float)(Position.X - R * Math.Cos(phi1)), (float)(Position.Y - R * Math.Sin(phi1)));
            Rectangle rect = new Rectangle();
            rect.X = (int)Vertices[0].X;
            rect.Y = (int)Vertices[0].Y;
            rect.Width = 0;
            rect.Height = 0;
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i].X < rect.X)
                {
                    rect.Width += (int)(rect.X - Vertices[i].X);
                    rect.X = (int)Vertices[i].X;
                }
                if (Vertices[i].Y < rect.Y)
                {
                    rect.Height += (int)(rect.Y - Vertices[i].Y);
                    rect.Y = (int)Vertices[i].Y;
                }
                if (Vertices[i].X > rect.Right)
                {
                    rect.Width = (int)Vertices[i].X - rect.X;
                }
                if (Vertices[i].Y > rect.Bottom)
                {
                    rect.Height = (int)Vertices[i].Y - rect.Y;
                }
            }
            Axes = new Vector2[2];
            Axes[0] = new Vector2((float)Math.Cos(MathHelper.PiOver2 + Angle), (float)Math.Sin(MathHelper.PiOver2 + Angle));
            Axes[1] = new Vector2(-Axes[0].Y, Axes[0].X);
            CollisionRect = rect;
            CollisionOffset = new Point(CollisionRect.Width, CollisionRect.Height);
        }
    }
}
