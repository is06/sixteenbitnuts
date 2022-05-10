using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public SpriteBatch? SpriteBatch { get; private set; }
        public Scene? CurrentScene { get; private set; }

        private readonly GraphicsDeviceManager graphics;

        public Game() : base()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected void LoadScene(Scene scene)
        {
            CurrentScene = scene;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentScene?.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            CurrentScene?.Draw();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
