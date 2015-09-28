using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameObjects.DrawableClasses;

namespace GameObjects
{
    /// <summary>
    /// Уровень
    /// </summary>
    public class Level
    {
        public const int ZoneSize = 100;
        private Random _rand = new Random();
        private Rectangle Rect;
        public List<Vector2> RespawnPoints = new List<Vector2>();
        public Texture2D Texture;
        public readonly int Width;
        public readonly int Height;
        public HashSet<DrawableSolid> Solids = new HashSet<DrawableSolid>();

        public Level() { }

        public Level(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Rect = new Rectangle(0, 0, Width, Height);
        }
        public Level(Texture2D sprite)
            : this(sprite.Width, sprite.Height)
        {
            this.Texture = sprite;
        }
        public Level(Texture2D sprite, int width, int height)
            : this(width, height)
        {
            this.Texture = sprite;
        }
        /// <summary>
        /// Создаёт точки респауна по умолчанию.
        /// </summary>
        public void CreateDefaultRespawnPositions()
        {
            int delta = 50;
            RespawnPoints.Add(new Vector2(delta, delta));
            RespawnPoints.Add(new Vector2(delta, Height - delta));
            RespawnPoints.Add(new Vector2(Width - delta, delta));
            RespawnPoints.Add(new Vector2(Width - delta, Height - delta));
        }

        /// <summary>
        /// Вызывается, когда игра отрисовывается.
        /// </summary>
        /// <param name="gameTime">Предоставляет моментальный снимок значений времени.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture, position, Rect, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public void CurrectNewPositionIfOut(RealCharacter character)
        {
            Vector2 newPos = character.MoveBehavior.NewPosition;
            if (newPos.X - (character.CollisionOffset.X >> 1) < 0)
                newPos.X = (character.CollisionOffset.X >> 1);
            else if (newPos.X + (character.CollisionOffset.X >> 1) > Rect.Width)
                newPos.X = Rect.Width - (character.CollisionOffset.X >> 1);
            if (newPos.Y - (character.CollisionOffset.Y >> 1) < 0)
                newPos.Y = (character.CollisionOffset.Y >> 1);
            else if (newPos.Y + (character.CollisionOffset.Y >> 1) > Rect.Height)
                newPos.Y = Rect.Height - (character.CollisionOffset.Y >> 1);
            character.MoveBehavior.NewPosition = newPos;
        }

        public Vector2 GetRespawnPosition()
        {
            return RespawnPoints[_rand.Next(RespawnPoints.Count)];
        }
    }
}
