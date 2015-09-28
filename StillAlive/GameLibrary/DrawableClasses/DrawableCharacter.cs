using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameObjects.Values;

namespace GameObjects
{
    public class DrawableCharacter : Character, ISprite
    {
        public Sprite Sprite { get; set; }
        Rectangle _lazerRect = new Rectangle(0, 0, 32, 32);
        Sprite _lazerSprite;
        float _prevHealth = 100;

        public DrawableCharacter(int id)
            : base(id)
        {
            _lazerSprite = new Sprite(Textures.LazerTexture, new Point(32, 32), 0, Point.Zero, new Point(3, 1));
        }

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
            _lazerSprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position)
        {
            Sprite.Draw(gameTime, spriteBatch, position, Angle, new Color(255 - (int)Health, 55 + (int)Health << 1, 255));
            // hp
            spriteBatch.Draw(Textures.HP, new Vector2(position.X - (this.CollisionOffset.X >> 1) - 10, position.Y - (this.CollisionOffset.Y >> 1) - 5),
                new Rectangle(0, 0, (int)(30 * Health / 100), 4),
                Color.White, 0, Vector2.Zero, 1f,
                SpriteEffects.None, 0.6f);
            // mp
            spriteBatch.Draw(Textures.MP, new Vector2(position.X - (this.CollisionOffset.X >> 1) - 10, position.Y - (this.CollisionOffset.Y >> 1) - 1),
                new Rectangle(0, 0, (int)(30 * Mana / 100), 4),
                Color.White, 0, Vector2.Zero, 1f,
                SpriteEffects.None, 0.6f);
            if (IsShooting)
            {
                _lazerSprite.Draw(gameTime, spriteBatch, position, Angle, new Vector2(16, 58), Color.White, 1f);
            }
            if (_prevHealth != Health)
            {
                spriteBatch.Draw(Textures.RedTexture, position,
                    new Rectangle(0, 0, this.CollisionOffset.X + 8, this.CollisionOffset.Y + 8),
                    Color.White, Angle, new Vector2((this.CollisionOffset.X >> 1) + 4f, (this.CollisionOffset.Y >> 1) + 4f), 1f,
                    SpriteEffects.None, 0.4f);
                _prevHealth = Health;
            }
            // имя игрока
            spriteBatch.DrawString(Values.Styles.Instance.Font, this.Name, new Vector2(position.X - this.CollisionOffset.X, position.Y - this.CollisionOffset.Y - 12), Color.White,
                0, Vector2.Zero, 1, SpriteEffects.None, 1f);
        }
    }
}
