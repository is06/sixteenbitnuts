using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;
using System;
using System.Diagnostics;

namespace SixteenBitNuts
{
    /// <summary>
    /// A 16-bit nuts game class that handles the main loop, rendering of the game, effects, audio
    /// </summary>
    public abstract class Game : Microsoft.Xna.Framework.Game
    {
        private const float mapSectionEditModeScale = 0.875f;

        #region Properties

        public string? WindowTitle { get; protected set; }
        public Size WindowSize { get; protected set; }
        public Size InternalSize { get; protected set; }
        public int FrameRate { get; protected set; }
        public bool FullScreen { get; protected set; }
        public Viewport InGameViewport { get; private set; }
        public Viewport SectionEditorViewPort
        {
            get
            {
                var isInMapSectionEditMode = currentScene is Map map && map.ShowSectionEditor;
                var sectionEditModeWidth = WindowSize.Width * mapSectionEditModeScale;
                var sectionEditModeHeight = WindowSize.Height * mapSectionEditModeScale;
                var sectionEditModeOffsetX = (WindowSize.Width - sectionEditModeWidth) / 2;
                var sectionEditModeOffsetY = WindowSize.Height - sectionEditModeHeight;

                return new Viewport(
                    x: isInMapSectionEditMode ? (int)sectionEditModeOffsetX : 0,
                    y: isInMapSectionEditMode ? (int)sectionEditModeOffsetY : 0,
                    width: isInMapSectionEditMode ? (int)sectionEditModeWidth : (int)WindowSize.Width,
                    height: isInMapSectionEditMode ? (int)sectionEditModeHeight : (int)WindowSize.Height
                );
            }
        }
        public float ScreenScale => WindowSize.Width / InternalSize.Width;
        public SpriteBatch? SpriteBatch { get; set; }
        public EffectService? EffectService { get; private set; }
        public TilesetService TilesetService { get; private set; }
        public IAudioManager? AudioManager { get; protected set; }
        public Scene? CurrentScene
        {
            get
            {
                return currentScene;
            }
        }
    
        #endregion

        #region Components

        private RenderTarget2D? inGameRenderSurface;
        private RenderTarget2D? preMainDisplayEffectRenderTarget;
        private RenderTarget2D? outGameRenderSurface;
        private readonly GraphicsDeviceManager graphics;
        private Process process;
        private double fps;

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
            graphics.IsFullScreen = FullScreen;
            graphics.ApplyChanges();

            // Misc
            Window.AllowUserResizing = false;
            Window.Title = WindowTitle;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = new TimeSpan((int)(1000f / FrameRate * 10000f));
            InGameViewport = new Viewport(0, 0, (int)InternalSize.Width, (int)InternalSize.Height);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            EffectService = new EffectService(this);
            AudioManager?.Initialize();

            // Render target texture of in game elements in 16-bit RGB format (65K colors)
            inGameRenderSurface = new RenderTarget2D(
                GraphicsDevice,
                (int)InternalSize.Width,
                (int)InternalSize.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth16
            );
            preMainDisplayEffectRenderTarget = new RenderTarget2D(GraphicsDevice, (int)InternalSize.Width, (int)InternalSize.Height);
            outGameRenderSurface = new RenderTarget2D(GraphicsDevice, (int)InternalSize.Width, (int)InternalSize.Height);

            base.Initialize();

            currentScene?.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            AudioManager?.LoadContent();

            currentScene?.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            process = Process.GetCurrentProcess();

            base.Update(gameTime);

            currentScene?.Update(gameTime);

            fps = (1 / gameTime.ElapsedGameTime.TotalSeconds);

            AudioManager?.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Big pixel graphics rendering
            {
                // Change the render target to surface
                GraphicsDevice.SetRenderTarget(inGameRenderSurface);
                GraphicsDevice.Clear(Color.Black);

                // Draws everything in the surface
                currentScene?.Draw();

                if (currentScene != null && currentScene.HasPreMainDisplayEffectDraws)
                {
                    GraphicsDevice.SetRenderTarget(preMainDisplayEffectRenderTarget);
                    currentScene.PreMainDisplayEffectDraw();
                }

                // Apply visual display effects
                if (EffectService is EffectService service && inGameRenderSurface != null)
                {
                    inGameRenderSurface = service.ApplyEnabledDisplayEffects(inGameRenderSurface);
                }

                // Out-game draws
                GraphicsDevice.SetRenderTarget(outGameRenderSurface);
                GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 0, 0, 0));
                currentScene?.OutGameDraw();

                // Debug visual representation
                currentScene?.DebugDraw();

                // Render the surface to have the ingame screen
                {
                    // Back to main framebuffer
                    GraphicsDevice.SetRenderTarget(null);

                    // In-game with main display effects
                    SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
                    SpriteBatch?.Draw(
                        texture: inGameRenderSurface,
                        destinationRectangle: SectionEditorViewPort.Bounds,
                        sourceRectangle: new Rectangle(0, 0, InGameViewport.Width, InGameViewport.Height),
                        color: Color.White
                    );
                    SpriteBatch?.End();

                    // Out-game display
                    SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp);
                    SpriteBatch?.Draw(
                        texture: outGameRenderSurface,
                        destinationRectangle: SectionEditorViewPort.Bounds,
                        sourceRectangle: new Rectangle(0, 0, InGameViewport.Width, InGameViewport.Height),
                        color: Color.White
                    );
                    SpriteBatch?.End();
                }
            }

            // Hi-res graphics rendering
            {
                // Render all UI elements in front of the render target texture (can be an HD Hud as well...)
                UIDraw(gameTime);
            }
        }

        public virtual void UIDraw(GameTime gameTime)
        {
            currentScene?.UIDraw();
        }

        public virtual void LoadMap(string name)
        {

        }

        public string GetRAM()
        {
            var value = Math.Round((process.PrivateMemorySize64 / (1024f * 1024f)), 2).ToString();
            return value + " MB";
        }

        public string GetFPS()
        {
            var value = Math.Round(fps, 2).ToString();
            return value + " FPS";
        }

        public void ToggleFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            AudioManager?.UnloadContent();
        }

        protected override void Dispose(bool disposing)
        {
            SpriteBatch?.Dispose();
            inGameRenderSurface?.Dispose();
            graphics.Dispose();
            AudioManager?.Dispose();

            base.Dispose(disposing);
        }
    }
}
