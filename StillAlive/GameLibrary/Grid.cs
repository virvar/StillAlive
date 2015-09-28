using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameObjects
{
    /// <summary>
    /// Сетка на карте для оптимизации определения столкновений и попаданий.
    /// </summary>
    public class Grid
    {
        public int Width;
        public int Height;
        public List<List<HashSet<Solid>>> Solids = new List<List<HashSet<Solid>>>();
        public List<List<HashSet<Character>>> Characters = new List<List<HashSet<Character>>>();
        /// <summary>
        /// Формирует сетку и добавляем объекты карты в соответствующие ячейки.
        /// </summary>
        /// <param name="level">Карта</param>
        public void SetLevel(Level level)
        {
            foreach (var solid in level.Solids)
            {
                solid.InitAxes();
            }
            // создаём карты зон
            Height = (int)Math.Ceiling((double)level.Height / Level.ZoneSize);
            Width = (int)Math.Ceiling((double)level.Width / Level.ZoneSize);
            Solids = new List<List<HashSet<Solid>>>(Width);
            Characters = new List<List<HashSet<Character>>>(Width);
            for (int i = 0; i < Width; i++)
            {
                Solids.Add(new List<HashSet<Solid>>(Height));
                Characters.Add(new List<HashSet<Character>>(Height));
                for (int j = 0; j < Height; j++)
                {
                    Solids[i].Add(new HashSet<Solid>());
                    Characters[i].Add(new HashSet<Character>());
                }
            }
            // добавляем каждое тело на карте в зоны, которые оно пересекает
            foreach (var solid in level.Solids)
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        if (solid.CollisionRect.Intersects(new Rectangle(i * Level.ZoneSize, j * Level.ZoneSize, Level.ZoneSize, Level.ZoneSize)))
                        {
                            Solids[i][j].Add(solid);
                        }
                    }
                }
            }
        }

        public void Put(Character character)
        {
            int x = (int)Math.Floor(character.Position.X / Level.ZoneSize);
            int y = (int)Math.Floor(character.Position.Y / Level.ZoneSize);
            Characters[x][y].Add(character);
        }

        public void Relocate(Character character)
        {
            Point oldZone = GetZone(character.Position);
            Point newZone = GetZone(character.MoveBehavior.NewPosition);
            if (newZone.X != oldZone.X || newZone.Y != oldZone.Y)
            {
                Characters[oldZone.X][oldZone.Y].Remove(character);
                Characters[newZone.X][newZone.Y].Add(character);
            }
        }

        public Point GetZone(Vector2 position)
        {
            int newZoneX = (int)Math.Floor(position.X / Level.ZoneSize);
            int newZoneY = (int)Math.Floor(position.Y / Level.ZoneSize);
            return new Point(newZoneX, newZoneY);
        }
    }
}
