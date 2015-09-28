using GameProcess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StillAlive
{
    class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Battlefield battlefield;
        Settings settings;

        public Game1(Settings settings)
        {
            this.settings = settings;
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            GameStateClient gameState = new GameStateClient();
            battlefield = new Battlefield(this, settings, gameState);
            Components.Add(battlefield);
            Components.Add(new GameConsole(this, gameState));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else if ((keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt))
                && keyboardState.IsKeyDown(Keys.Enter))
            {
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 480;
                graphics.ToggleFullScreen();
                graphics.ApplyChanges();
                GraphicsDevice.Viewport = new Viewport(0, 0, 800, 480);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
