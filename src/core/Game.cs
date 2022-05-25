using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Config properties
        public string WindowTitle { get; protected set; }
        public Point WindowInitSize { get; protected set; }
        public ushort FrameRate { get; protected set; }
        public bool IsFullScreen { get; protected set; }
        public Point InternalSize { get; protected set; }
        
        // Service properties
        public InputInterface InputInterface { get; private set; }
        public IMapLoader? MapLoader { get; protected set; }
        public ITilesetLoader? TilesetLoader { get; protected set; }
        public SpriteLoader SpriteLoader { get; private set; }
        public SpriteBatch? SpriteBatch { get; private set; }
        public IAuthoringTool? AuthoringTool { get; protected set; }
        public IAudioManager? AudioManager { get; protected set; }
        
        // Component properties
        public Scene? CurrentScene { get; private set; }

        // Private members
        private readonly GraphicsDeviceManager graphics;
        private RenderTarget2D? inGameRenderSurface;
        private Point currentWindowSize;

        /// <summary>
        /// Constructor
        /// </summary>
        public Game() : base()
        {
            InputInterface = new InputInterface();
            MapLoader = new MapLoader();
            TilesetLoader = new TilesetLoader();
            SpriteLoader = new SpriteLoader();
            
            WindowTitle = "Untitled game";
            WindowInitSize = new Point(1280, 720);
            FrameRate = 60;

            graphics = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Inits all resources needed for the game
        /// </summary>
        protected override void Initialize()
        {
            SetWindowGraphics(WindowInitSize.X, WindowInitSize.Y, IsFullScreen);
            currentWindowSize = WindowInitSize;

            Window.AllowUserResizing = false;
            Window.Title = WindowTitle;
            IsMouseVisible = true;
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

            AuthoringTool?.Initialize();
            AudioManager?.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            AudioManager?.LoadContent();
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
            AudioManager?.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Change the render target to in game surface
            GraphicsDevice.SetRenderTarget(inGameRenderSurface);
            
            var rs = new RasterizerState();
            //rs.FillMode = FillMode.WireFrame;
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.RasterizerState = rs;

            // Draws everything in game in the surface
            CurrentScene?.Draw();
            CurrentScene?.DebugDraw();
                
            // Back to main framebuffer
            GraphicsDevice.SetRenderTarget(null);

            // In-game with main display effects
            SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            SpriteBatch?.Draw(
                texture: inGameRenderSurface,
                destinationRectangle: new Rectangle(Point.Zero, currentWindowSize),
                sourceRectangle: new Rectangle(Point.Zero, InternalSize),
                color: Color.White
            );
            SpriteBatch?.End();

            DrawUI(gameTime);
        }

        protected virtual void DrawUI(GameTime gameTime)
        {
            AuthoringTool?.Draw(gameTime);
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
            if (!graphics.IsFullScreen)
            {
                var currentDisplayMode = GraphicsDevice.Adapter.CurrentDisplayMode;
                SetWindowGraphics(currentDisplayMode.Width, currentDisplayMode.Height, true);
                currentWindowSize = new Point(currentDisplayMode.Width, currentDisplayMode.Height);
            }
            else
            {
                SetWindowGraphics(WindowInitSize.X, WindowInitSize.Y, false);
                currentWindowSize = WindowInitSize;
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            AudioManager?.UnloadContent();
        }

        protected override void Dispose(bool disposing)
        {
            AudioManager?.Dispose();
            AuthoringTool?.Dispose();

            base.Dispose(disposing);
        }
    }
}
