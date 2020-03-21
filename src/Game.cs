using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace SixteenBitNuts
{
    public abstract class Game : Microsoft.Xna.Framework.Game
    {
        #region Properties

        public string? WindowTitle { get; protected set; }
        public Size WindowSize { get; protected set; }
        public Size InternalSize { get; protected set; }
        public int FrameRate { get; protected set; }
        public Viewport InGameViewport { get; private set; }
        public float ScreenScale => WindowSize.Width / InternalSize.Width;
        public SpriteBatch? SpriteBatch { get; set; }
        public TilesetService TilesetService { get; private set; }

        #endregion

        #region Components

        private RenderTarget2D? renderSurface;        
        private readonly GraphicsDeviceManager graphics;
        private Process process;

        protected Scene? currentScene;

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            process = Process.GetCurrentProcess();

            TilesetService = new TilesetService(this);
        }

        protected override void Initialize()
        {
            // Graphics
            graphics.PreferredBackBufferWidth = (int)WindowSize.Width;
            graphics.PreferredBackBufferHeight = (int)WindowSize.Height;
            graphics.ApplyChanges();

            // Misc
            Window.AllowUserResizing = false;
            Window.Title = WindowTitle;
            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan((int)(1000f / FrameRate * 10000f));
            InGameViewport = new Viewport(0, 0, (int)InternalSize.Width, (int)InternalSize.Height);
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            renderSurface = new RenderTarget2D(GraphicsDevice, (int)InternalSize.Width, (int)InternalSize.Height);

            base.Initialize();
        }

        protected override void LoadContent()
        {

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            process = Process.GetCurrentProcess();
            Window.Title = WindowTitle + " - " + (process.PrivateMemorySize64 / (1024f * 1024f)) + " MB";

            base.Update(gameTime);

            currentScene?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Big pixel graphics rendering
            {
                // Change the render target to surface
                GraphicsDevice.SetRenderTarget(renderSurface);
                GraphicsDevice.Clear(Color.Black);

                // Draws everything in the surface
                currentScene?.Draw();

                // Debug visual representation
                currentScene?.DebugDraw();

                // Back to the normal render method
                GraphicsDevice.SetRenderTarget(null);

                // Render the surface to have the ingame screen
                SpriteBatch?.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    null, // Effect
                    null
                );
                SpriteBatch?.Draw(
                    texture: renderSurface,
                    destinationRectangle: new Rectangle(0, 0, (int)WindowSize.Width, (int)WindowSize.Height),
                    sourceRectangle: new Rectangle(0, 0, InGameViewport.Width, InGameViewport.Height),
                    color: Color.White
                );
                SpriteBatch?.End();
            }

            // Hi-res graphics rendering
            {
                // Render all UI elements in front of the render target texture
                currentScene?.UIDraw();
            }

            base.Draw(gameTime);
        }

        public virtual void LoadMap(string name)
        {

        }

        protected override void Dispose(bool disposing)
        {
            SpriteBatch?.Dispose();
            renderSurface?.Dispose();
            graphics.Dispose();

            base.Dispose(disposing);
        }
    }
}
