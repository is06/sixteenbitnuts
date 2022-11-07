using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SixteenBitNuts
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Public fields
        public bool IsDebugDisplayOn;

        // Config properties
        public string WindowTitle { get; protected set; }
        public Point WindowInitSize { get; protected set; }
        public ushort FrameRate { get; protected set; }
        public bool IsFullScreen { get; protected set; }
        public Point InternalSize { get; protected set; }
        
        // Service properties
        public InputInterface InputInterface { get; private set; }
        public SpriteBatch? SpriteBatch { get; private set; }
        public LineBatch? LineBatch { get; private set; }

        // Overridable services
        public EntityFactory? EntityFactory { get; protected set; }
        public IMapLoader? MapLoader { get; protected set; }
        public ITilesetLoader? TilesetLoader { get; protected set; }
        public ISpriteLoader? SpriteLoader { get; protected set; }
        public BinaryMapWriter? MapWriter { get; protected set; }
        public IAuthoringTool? AuthoringTool { get; protected set; }
        public IAudioManager? AudioManager { get; protected set; }
        public AssetManager? AssetManager { get; protected set; }
        
        // Component properties
        public Scene? CurrentScene { get; private set; }

        // Private members
        private readonly GraphicsDeviceManager graphics;
        private RenderTarget2D? inGameRenderSurface;
        private VirtualButton debugDisplayButton;
        private Rectangle gameRenderBounds;

        /// <summary>
        /// Constructor
        /// </summary>
        public Game() : base()
        {
            InputInterface = new InputInterface();
            EntityFactory = new EntityFactory();
            MapLoader = new TextFormatMapLoader();
            MapLoader.SetEntityFactory(EntityFactory);
            TilesetLoader = new TilesetLoader();
            SpriteLoader = new SpriteLoader();
            MapWriter = new BinaryMapWriter();
            
            WindowTitle = "Untitled game";
            WindowInitSize = new Point(1280, 720);
            InternalSize = new Point(320, 180);
            FrameRate = 60;

            graphics = new GraphicsDeviceManager(this);

            debugDisplayButton = new VirtualButton().AddKey(Keys.F3);
        }

        /// <summary>
        /// Inits all resources needed for the game
        /// </summary>
        protected override void Initialize()
        {
            SetWindowGraphics(WindowInitSize.X, WindowInitSize.Y, IsFullScreen);
            gameRenderBounds = new Rectangle(Point.Zero, WindowInitSize);

            Window.AllowUserResizing = false;
            Window.Title = WindowTitle;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace
            };

            // Time computation
            TargetElapsedTime = new TimeSpan((int)(1000f / FrameRate * 10000f));

            // Graphic batches
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            LineBatch = new LineBatch(this);
            LineBatch?.Initialize();

            inGameRenderSurface = new RenderTarget2D(
                GraphicsDevice,
                InternalSize.X,
                InternalSize.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24
            );

            // Other services initializations
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

            HandleDebugDisplay();
            CurrentScene?.Update();
            AudioManager?.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (CurrentScene?.RenderSurface is RenderTarget2D sceneSurface
                && CurrentScene?.RenderDestinationBounds is Rectangle sceneRenderDestinationBounds)
            {
                // Render in the scene render surface
                GraphicsDevice.SetRenderTarget(sceneSurface); // 256x176
                GraphicsDevice.Clear(Color.Black);

                // Draws everything in the scene surface
                CurrentScene?.Draw();

                // Draws debug boxes
                if (IsDebugDisplayOn)
                {
                    CurrentScene?.DebugDraw();
                }

                // Change the render target to in game surface
                GraphicsDevice.SetRenderTarget(inGameRenderSurface); // 256x224
                //GraphicsDevice.Clear(Color.Black);

                // Draws scene surface in game surface
                SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp);
                SpriteBatch?.Draw(
                    texture: sceneSurface, // 256x176
                    destinationRectangle: sceneRenderDestinationBounds, // 0,48 256x176
                    sourceRectangle: sceneSurface.Bounds, 
                    color: Color.White
                );
                SpriteBatch?.End();
            }
            else
            {
                // Change the render target to in game surface
                GraphicsDevice.SetRenderTarget(inGameRenderSurface);
                GraphicsDevice.Clear(Color.Black);

                // Draws everything in the game surface
                CurrentScene?.Draw();

                // Draws debug boxes
                if (IsDebugDisplayOn)
                {
                    CurrentScene?.DebugDraw();
                }
            }
                
            // Back to main framebuffer
            GraphicsDevice.SetRenderTarget(null);

            // In-game with main display effects
            SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            SpriteBatch?.Draw(
                texture: inGameRenderSurface,
                destinationRectangle: gameRenderBounds,
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

        /// <summary>
        /// Call this function to enter or leave the fullscreen mode
        /// Can be called from a authoring tool
        /// </summary>
        public void ToggleFullScreen()
        {
            if (!graphics.IsFullScreen)
            {
                var currentDisplayMode = GraphicsDevice.Adapter.CurrentDisplayMode;
                SetWindowGraphics(currentDisplayMode.Width, currentDisplayMode.Height, true);
                gameRenderBounds.Size = new Point(currentDisplayMode.Width, currentDisplayMode.Height);
            }
            else
            {
                SetWindowGraphics(WindowInitSize.X, WindowInitSize.Y, false);
                gameRenderBounds.Size = WindowInitSize;
            }
        }

        private void HandleDebugDisplay()
        {
            debugDisplayButton.Update();
            if (debugDisplayButton.IsPressedOnce())
            {
                IsDebugDisplayOn = !IsDebugDisplayOn;
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
