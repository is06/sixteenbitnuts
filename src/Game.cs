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
        #region Properties

        public string? WindowTitle { get; protected set; }
        public Size WindowSize { get; protected set; }
        public Size InternalSize { get; protected set; }
        public int FrameRate { get; protected set; }
        public bool FullScreen { get; protected set; }
        public Viewport InGameViewport { get; private set; }
        public float ScreenScale => WindowSize.Width / InternalSize.Width;
        public SpriteBatch? SpriteBatch { get; set; }
        public EffectService? EffectService { get; private set; }
        public TilesetService TilesetService { get; private set; }
        public IAudioManager? AudioManager { get; protected set; }
    
        #endregion

        #region Components

        private RenderTarget2D? inGameRenderSurface;
        private RenderTarget2D? preMainDisplayEffectRenderTarget;
        private RenderTarget2D? outGameRenderSurface;
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
            graphics.IsFullScreen = FullScreen;
            graphics.ApplyChanges();

            // Misc
            Window.AllowUserResizing = false;
            Window.Title = WindowTitle;
            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan((int)(1000f / FrameRate * 10000f));
            InGameViewport = new Viewport(0, 0, (int)InternalSize.Width, (int)InternalSize.Height);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            EffectService = new EffectService(this);
            AudioManager?.Initialize();

            inGameRenderSurface = new RenderTarget2D(GraphicsDevice, (int)InternalSize.Width, (int)InternalSize.Height);
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
            Window.Title = WindowTitle + " - " + (process.PrivateMemorySize64 / (1024f * 1024f)) + " MB";

            base.Update(gameTime);

            currentScene?.Update(gameTime);

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

                // Debug visual representation
                currentScene?.DebugDraw();

                // Out-game draws
                GraphicsDevice.SetRenderTarget(outGameRenderSurface);
                GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 0, 0, 0));
                currentScene?.OutGameDraw();

                // Render the surface to have the ingame screen
                {
                    // Back to main framebuffer
                    GraphicsDevice.SetRenderTarget(null);

                    // In-game with main display effects
                    SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
                    SpriteBatch?.Draw(
                        texture: inGameRenderSurface,
                        destinationRectangle: new Rectangle(0, 0, (int)WindowSize.Width, (int)WindowSize.Height),
                        sourceRectangle: new Rectangle(0, 0, InGameViewport.Width, InGameViewport.Height),
                        color: Color.White
                    );
                    SpriteBatch?.End();

                    // Out-game display
                    SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
                    SpriteBatch?.Draw(
                        texture: outGameRenderSurface,
                        destinationRectangle: new Rectangle(0, 0, (int)WindowSize.Width, (int)WindowSize.Height),
                        sourceRectangle: new Rectangle(0, 0, InGameViewport.Width, InGameViewport.Height),
                        color: Color.White
                    );
                    SpriteBatch?.End();
                }
            }

            // Hi-res graphics rendering
            {
                // Render all UI elements in front of the render target texture (can be an HD Hud as well...)
                currentScene?.UIDraw();
            }
        }

        public virtual void LoadMap(string name)
        {

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
