using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProcess;

namespace StillAlive
{
    public class GameConsole : DrawableGameComponent
    {
        private GameStateClient _gameState;

        private Vector2 _centerOfScreen;
        private SpriteBatch _spriteBatch;

        private SpriteFont _historyFont;
        private SpriteFont _currentFont;

        private RenderTarget2D _historyBackground;
        private Rectangle _historyBackgroundBounds;
        public Color HistoryBackgroundColor { get; set; }

        private RenderTarget2D _currentBackground;
        private Rectangle _currentBackgroundBounds;
        public Color CurrentBackgroundColor { get; set; }

        private bool _opening = false;
        private bool _closing = false;

        private float _showingSecondsTime = 0.5f;

        private Queue<string> _historyBuffer;
        private int _bufferCapacity = 10;
        private StringBuilder _currentLine = new StringBuilder();
        private Keys _pressedKey = Keys.None;

        public Queue<GameConsoleCommand> Commands { get; set; }

        public GameConsole(Game game, GameStateClient gameState)
            : base(game)
        {
            _gameState = gameState;
        }

        public override void Initialize()
        {
            base.Initialize();

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _centerOfScreen = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                Game.GraphicsDevice.Viewport.Height / 2);

            _historyBackgroundBounds = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, 0);

            Visible = false;
            _historyBuffer = new Queue<string>(_bufferCapacity);

            HistoryBackgroundColor = Color.FromNonPremultiplied(150, 150, 150, 150);
            CurrentBackgroundColor = Color.FromNonPremultiplied(0, 0, 0, 150);

            this.Commands = new Queue<GameConsoleCommand>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _historyFont = GameObjects.Values.Styles.Instance.Font;
            _currentFont = GameObjects.Values.Styles.Instance.Font;

            _historyBackground = new RenderTarget2D(Game.GraphicsDevice, 1, 1);
            _currentBackground = new RenderTarget2D(Game.GraphicsDevice, 1, 1);

            _currentBackgroundBounds = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, _historyFont.LineSpacing + 5);

            Game.GraphicsDevice.SetRenderTarget(_historyBackground);
            Game.GraphicsDevice.Clear(Color.White);
            Game.GraphicsDevice.SetRenderTarget(_currentBackground);
            Game.GraphicsDevice.Clear(Color.White);
            Game.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // нажата клавиша 'тильда' - открывается консоль
            if (GameObjects.Values.Commands.ConsoleButtonPressed)
            {
                if (!_opening && !_closing)
                {
                    if (!Visible)
                    {
                        Visible = true;
                        _opening = true;
                    }
                    else
                        _closing = true;
                }
            }

            if (Visible && (_opening || _closing))
            {
                var showingSpeed = _centerOfScreen.Y / _showingSecondsTime;
                if (_opening)
                {
                    _historyBackgroundBounds.Height += (int)Math.Max(1f, showingSpeed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (_historyBackgroundBounds.Height >= _centerOfScreen.Y)
                    {
                        _historyBackgroundBounds.Height = (int)_centerOfScreen.Y;
                        _opening = false;
                    }
                }
                else if (_closing)
                {
                    _historyBackgroundBounds.Height -= (int)Math.Max(1f, showingSpeed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (_historyBackgroundBounds.Height <= 0)
                    {
                        _historyBackgroundBounds.Height = 0;
                        _closing = false;
                        Visible = false;
                    }
                }
                _currentBackgroundBounds.Y = _historyBackgroundBounds.Height;
            }

            if (Visible && !_opening)
            {
                if (Keyboard.GetState().IsKeyUp(_pressedKey))
                    _pressedKey = Keys.None;
                if (_pressedKey == Keys.None)
                {
                    if (Keyboard.GetState().GetPressedKeys().Contains(Keys.Enter))
                    {
                        AddCommand(_currentLine.ToString());
                        _currentLine.Clear();
                        _pressedKey = Keys.Enter;
                    }
                    else
                    {
                        foreach (var key in Keyboard.GetState().GetPressedKeys())
                        {
                            if (key == Keys.Space)
                            {
                                _currentLine.Append(" ");
                            }
                            else if (key == Keys.Back)
                            {
                                if (_currentLine.Length > 0)
                                    _currentLine.Remove(_currentLine.Length - 1, 1);
                            }
                            else if (key == Keys.Delete)
                            {
                                _currentLine.Clear();
                            }
                            else if (key == Keys.Up)
                            {
                                if (_historyBuffer.Count > 0)
                                {
                                    _currentLine.Clear();
                                    _currentLine.Append(_historyBuffer.Last());
                                }
                            }
                            else
                            {
                                string name = key.ToString();
                                if (name.Length == 1)
                                    _currentLine.Append(name.ToLower());
                            }
                            _pressedKey = key;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Visible)
            {
                _spriteBatch.Begin();

                _spriteBatch.Draw(_historyBackground, _historyBackgroundBounds, HistoryBackgroundColor);
                _spriteBatch.Draw(_currentBackground, _currentBackgroundBounds, CurrentBackgroundColor);

                int linePosition = _historyBackgroundBounds.Bottom - _historyBuffer.Count * _historyFont.LineSpacing;
                foreach (var line in _historyBuffer)
                {
                    if (linePosition > 0)
                        _spriteBatch.DrawString(_historyFont, line, new Vector2(0, linePosition), Color.White);
                    linePosition += _historyFont.LineSpacing;
                }

                _spriteBatch.DrawString(_currentFont, _currentLine, new Vector2(0, _currentBackgroundBounds.Top), Color.White);

                _spriteBatch.End();
            }
        }

        public void AddCommand(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (_historyBuffer.Count == _bufferCapacity)
                {
                    _historyBuffer.Dequeue();
                }
                _historyBuffer.Enqueue(text);
                Commands.Enqueue(new GameConsoleCommand(_gameState, text));
            }
        }
    }
}
