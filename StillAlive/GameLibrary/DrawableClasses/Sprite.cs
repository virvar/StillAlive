using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameObjects
{
    public class Sprite
    {
        Texture2D textureImage;
        internal Point frameSize;
        Vector2 frameCenter;
        Point currentFrame;
        Point sheetSize;
        int collisionOffset;
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;

        public Sprite(Texture2D textureImage, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize)
            : this(textureImage, frameSize, collisionOffset, currentFrame,
            sheetSize, defaultMillisecondsPerFrame)
        {
        }

        public Sprite(Texture2D textureImage, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize,
            int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            //this.position = position;
            this.frameSize = frameSize;
            frameCenter = new Vector2(frameSize.X / 2, frameSize.Y / 2);
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            //this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }

        /// <summary>
        /// Позволяет игре запускать логику обновления мира,
        /// проверки столкновений, получения ввода и воспроизведения звуков.
        /// </summary>
        /// <param name="gameTime">Предоставляет моментальный снимок значений времени.</param>
        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        /// <summary>
        /// Вызывается, когда игра отрисовывается.
        /// </summary>
        /// <param name="gameTime">Предоставляет моментальный снимок значений времени.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Character character)
        {
            spriteBatch.Draw(textureImage,
                character.Position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                    Color.White, 0, Vector2.Zero,
                    1f, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Вызывается, когда игра отрисовывается.
        /// </summary>
        /// <param name="gameTime">Предоставляет моментальный снимок значений времени.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation, Color color)
        {
            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                //Color.White, rotation, Vector2.Zero,
                color, rotation, frameCenter,
                1f, SpriteEffects.None, 0.5f);
        }
        /// <summary>
        /// Вызывается, когда игра отрисовывается.
        /// </summary>
        /// <param name="gameTime">Предоставляет моментальный снимок значений времени.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 origin,
            Color color, float depth)
        {
            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                color, rotation, origin,
                1f, SpriteEffects.None, depth);
        }
    }
}
