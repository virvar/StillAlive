using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.Values;

namespace GameObjects
{
    public class PlayerMove1 : GameObjects.IMoveBehavior
    {
        Character _character;
        public Character Character
        {
            get { return _character; }
            set { _character = value; }
        }
        Vector2 _NewPosition;
        public Vector2 NewPosition
        {
            get { return _NewPosition; }
            set { _NewPosition = value; }
        }

        public PlayerMove1(Character character)
        {
            this._character = character;
        }

        public void Move(GameTime gameTime)
        {
            TimeSpan elapsedTime = gameTime.ElapsedGameTime;
            KeyboardState keyboardState = Keyboard.GetState();
            ProcessMovement(keyboardState, elapsedTime);
            MouseState mouseState = Mouse.GetState();
            ProcessRotation(mouseState, elapsedTime);
            ProcessShooting(mouseState, elapsedTime);
            // статистика игры
            Commands.ScoresButtonPressed = keyboardState.IsKeyDown(Keys.Tab);
            // консоль
            Commands.ConsoleButtonPressed = keyboardState.IsKeyDown(Keys.OemTilde);
        }

        private void ProcessMovement(KeyboardState keyboardState, TimeSpan elapsedTime)
        {
            float delta = (float)(_character.Speed * elapsedTime.TotalSeconds);
            _NewPosition = _character.Position;
            Vector2 move = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                move.X -= delta;
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                move.X += delta;
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                move.Y -= delta;
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                move.Y += delta;
            if ((move.X != 0) && (move.Y != 0))
            {
                move *= 0.707106769f;
            }
            _NewPosition += move;
        }

        private void ProcessRotation(MouseState mouseState, TimeSpan elapsedTime)
        {
            int dx = mouseState.X - Dimensions.CenterOfScreen.X;
            int dy = mouseState.Y - Dimensions.CenterOfScreen.Y;
            if (dy == 0)
            {
                _character.Angle = dx > 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2;
            }
            else
            {
                _character.Angle = (float)Math.Atan(-(double)dx / dy);
            }
            if (dy > 0)
            {
                _character.Angle += MathHelper.Pi;
            }
        }

        private void ProcessShooting(MouseState mouseState, TimeSpan elapsedTime)
        {
            _character.IsShooting = mouseState.LeftButton == ButtonState.Pressed;
            if (_character.IsShooting)
            {
                lock (_character)
                {
                    _character.ShootingTime += (float)elapsedTime.TotalSeconds;
                }
            }
        }
    }
}
