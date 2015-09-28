using System;
using Microsoft.Xna.Framework;
namespace GameObjects
{
    public interface ISolid
    {
        /// <summary>
        /// Угол поворота.
        /// </summary>
        float Angle { get; set; }
        /// <summary>
        /// Размер зоны столкновения.
        /// </summary>
        Point CollisionOffset { get; }
        /// <summary>
        /// Координаты центра.
        /// </summary>
        Vector2 Position { get; set; }
        /// <summary>
        /// Вершины многоуольника, ограничивающего тело.
        /// </summary>
        Vector2[] Vertices { get; }
    }
}
