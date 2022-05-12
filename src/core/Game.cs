using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public string WindowTitle { get; protected set; }
        public Point WindowSize { get; protected set; }
        public ushort FrameRate { get; protected set; }
        public bool IsFullScreen { get; protected set; }
        public Point InternalSize { get; protected set; }
        
        public InputInterface InputInterface { get; protected set; }
        public MapLoader MapLoader { get; private set; }
        public TilesetLoader TilesetLoader { get; private set; }
        public SpriteLoader SpriteLoader { get; private set; }
        public SpriteBatch? SpriteBatch { get; private set; }
        public Scene? CurrentScene { get; private set; }

        private readonly GraphicsDeviceManager graphics;

        private RenderTarget2D? inGameRenderSurface;

        public Game() : base()
        {
            InputInterface = new InputInterface();
            MapLoader = new MapLoader();
            TilesetLoader = new TilesetLoader();
            SpriteLoader = new SpriteLoader();
            FrameRate = 60;
            WindowSize = new Point(1280, 720);
            WindowTitle = "Untitled game";

            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            SetWindowGraphics(WindowSize.X, WindowSize.Y, IsFullScreen);
            
            Window.AllowUserResizing = false;
            Window.Title = WindowTitle;
            
            Content.RootDirectory = "Content";
            
            TargetElapsedTime = new TimeSpan((int)(1000f / FrameRate * 10000f));

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            inGameRenderSurface = new RenderTarget2D(
                GraphicsDevice,
                InternalSize.X,
                InternalSize.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24
            );


            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected void LoadScene(Scene scene)
        {
            CurrentScene?.UnloadContent();
            CurrentScene = scene;
            CurrentScene?.Initialize();
            CurrentScene?.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentScene?.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Change the render target to in game surface
            GraphicsDevice.SetRenderTarget(inGameRenderSurface);
            GraphicsDevice.Clear(Color.Black);

            // Draws everything in game in the surface
            CurrentScene?.Draw();
            CurrentScene?.DebugDraw();

            // Back to main framebuffer
            GraphicsDevice.SetRenderTarget(null);

            // In-game with main display effects
            SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            SpriteBatch?.Draw(
                texture: inGameRenderSurface,
                destinationRectangle: new Rectangle(Point.Zero, WindowSize),
                sourceRectangle: new Rectangle(Point.Zero, InternalSize),
                color: Color.White
            );
            SpriteBatch?.End();
        }

        private void SetWindowGraphics(int width, int height, bool isFullScreen = false)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.IsFullScreen = isFullScreen;
            graphics.ApplyChanges();
        }

        public void ToggleFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();
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
