using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameObjects.DrawableClasses
{
    public class DrawableSolid : Solid, ISprite
    {
        public Sprite Sprite { get; set; }
        
        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 levelPosition)
        {
            Sprite.Draw(gameTime, spriteBatch, Position + levelPosition, Angle, Color.White);         
        }
    }
}
