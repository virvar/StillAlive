using Microsoft.Xna.Framework;

namespace GameObjects.Helpers
{
    public static class CollisionHelper
    {
        public static bool Intersects(Rectangle rect, Solid solid)
        {
            Vector2 minDelta = Vector2.Zero;
            float minDeltaLength = 0;
            Vector2[] axes = new Vector2[solid.Axes.Length + 2];
            axes[0] = new Vector2(1, 0);
            axes[1] = new Vector2(0, 1);
            solid.Axes.CopyTo(axes, 2);
            foreach (var axis in axes)
            {
                // проекция персонажа
                Vector2 projBegin1;
                projBegin1 = GetProjection(new Vector2(rect.Left, rect.Top), axis);
                Vector2 proj = GetProjection(new Vector2(rect.Left, rect.Bottom), axis);
                if ((proj.X < projBegin1.X) || ((proj.X == projBegin1.X) && (proj.Y < projBegin1.Y)))
                    projBegin1 = proj;
                Vector2 projEnd1;
                projEnd1 = GetProjection(new Vector2(rect.Right, rect.Top), axis);
                proj = GetProjection(new Vector2(rect.Right, rect.Bottom), axis);
                if ((proj.X > projEnd1.X) || ((proj.X == projEnd1.X) && (proj.Y > projEnd1.Y)))
                    projEnd1 = proj;
                // проекция тела
                Vector2 projBegin2;
                Vector2 projEnd2;
                projBegin2 = projEnd2 = GetProjection(solid.Vertices[0], axis);
                for (int i = 1; i < solid.Vertices.Length; i++)
                {
                    proj = GetProjection(solid.Vertices[i], axis);
                    if ((proj.X < projBegin2.X) || ((proj.X == projBegin2.X) && (proj.Y < projBegin2.Y)))
                        projBegin2 = proj;
                    else if ((proj.X > projEnd2.X) || ((proj.X == projEnd2.X) && (proj.Y > projEnd2.Y)))
                        projEnd2 = proj;
                }
                Vector2 delta = Vector2.Zero;
                Vector2 d1 = projEnd1 - projBegin2;
                if ((d1.X > 0) || ((d1.X == 0) && (d1.Y > 0)))
                {
                    Vector2 d2 = projEnd2 - projBegin1;
                    if ((d2.X > 0) || ((d2.X == 0) && (d2.Y > 0)))
                    {
                        if ((d1.X < d2.X) || ((d1.X == d2.X) && (d1.Y <= d2.Y)))
                        {
                            delta = -d1;
                        }
                        else
                        {
                            delta = d2;
                        }
                    }
                }
                if (delta == Vector2.Zero)
                {
                    return false;
                }
                float deltaLength = delta.Length();
                if ((minDelta == Vector2.Zero) || (deltaLength < minDeltaLength))
                {
                    minDelta = delta;
                    minDeltaLength = deltaLength;
                }
            }
            return true;
        }

        private static Vector2 GetProjection(Vector2 a, Vector2 b)
        {
            return (a.X * b.X + a.Y * b.Y) * b;
        }
    }
}
