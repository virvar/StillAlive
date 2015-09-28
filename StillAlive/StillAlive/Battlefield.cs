using System.Text;
using GameObjects;
using GameObjects.DrawableClasses;
using GameObjects.Values;
using GameProcess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StillAlive
{
    class Battlefield : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private GameStateClient _gameState;
        private RealCharacter _player;
        private Vector2 _centerOfVirtualScreen;
        private Communicator _communicator;
        private Sprite _playerSprite;
        private SpriteFont _font;
        private Vector2 _virtualScreen = new Vector2(800, 480);
        private Matrix _scaleMatrix;
        private Settings _settings;
        public int PacketLength;

        public Battlefield(Game1 game, Settings settings, GameStateClient gameState)
            : base(game)
        {
            _settings = settings;
            _gameState = gameState;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _gameState.CharacterAdded += new System.Action<RealCharacter>(CreateCharacter);

            Point playerSize = new Point(32, 32);
            Texture2D playerTexture = Game.Content.Load<Texture2D>(@"Images\player");
            _playerSprite = new Sprite(
                playerTexture, playerSize, 0, Point.Zero, new Point(1, 1));
            Texture2D levelTexture = Game.Content.Load<Texture2D>(@"Images\level1");
            Textures.LazerTexture = Game.Content.Load<Texture2D>(@"Images\fire");
            Textures.RedTexture = Game.Content.Load<Texture2D>(@"Images\monster");
            Textures.HP = Game.Content.Load<Texture2D>(@"Images\hp");
            Textures.MP = Game.Content.Load<Texture2D>(@"Images\mp");
            _communicator = new Communicator(_settings, this, _gameState, out _player);
            _gameState.Level.Texture = levelTexture;
            Texture2D solidTexture = Game.Content.Load<Texture2D>(@"Images\solid");
            foreach (var solid in _gameState.Level.Solids)
            {
                solid.Sprite = new Sprite(
                    solidTexture, solid.Size, 0, Point.Zero, new Point(1, 1));
            }
            _font = Game.Content.Load<SpriteFont>(@"Fonts\SpriteFont1");
            Styles.Instance.Font = _font;

            base.LoadContent();
        }

        private void CreateCharacter(RealCharacter character)
        {
            character.Sprite = _playerSprite;
        }

        //public void RemoveCharacter(int playerId)
        //{
        //    _gameState.RemoveCharacter(playerId);
        //}

        public override void Update(GameTime gameTime)
        {
            _gameState.Update(gameTime);

            float widthScale = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / _virtualScreen.X;
            float heightScale = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / _virtualScreen.Y;
            _scaleMatrix = Matrix.CreateScale(widthScale, heightScale, 1);
            Dimensions.CenterOfScreen = new Point((int)(GraphicsDevice.PresentationParameters.BackBufferWidth / 2),
                (int)(GraphicsDevice.PresentationParameters.BackBufferHeight / 2));
            _centerOfVirtualScreen = new Vector2(Dimensions.CenterOfScreen.X / widthScale,
                Dimensions.CenterOfScreen.Y / heightScale);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, _scaleMatrix);
            // рисуем карту и объекты
            Vector2 levelPos = _centerOfVirtualScreen - _player.Position;
            _gameState.Level.Draw(_spriteBatch, levelPos);
            foreach (var solid in _gameState.Level.Solids)
            {
                solid.Draw(gameTime, _spriteBatch, levelPos);
            }

            // рисуем игрока
            _player.Draw(gameTime, _spriteBatch, _centerOfVirtualScreen);
            lock (_gameState.Players)
            {
                foreach (var character in _gameState.Players.Values)
                {
                    if (character != _player)
                        character.Draw(gameTime, _spriteBatch, character.Position + levelPos);
                }
            }
            string text = PacketLength == 0 ? "No connection." : "Packet length: " + PacketLength;
            _spriteBatch.DrawString(_font, text, new Vector2(10, 10), Color.Black,
                0, Vector2.Zero, 1, SpriteEffects.None, 1);

            // статистика игры
            if (Commands.ScoresButtonPressed && _gameState.Scores != null)
            {
                StringBuilder scoresText = new StringBuilder();
                foreach (var score in _gameState.Scores)
                {
                    scoresText.AppendFormat("{0}    Kills: {1}    Deaths: {2}\n", _gameState.Players[score.Key].Name, score.Value.Kills, score.Value.Deaths);
                }
                _spriteBatch.DrawString(_font, scoresText.ToString(), new Vector2(10, 50), Color.Black,
                    0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            _communicator.Stop();
            base.Dispose(disposing);
        }
    }
}
