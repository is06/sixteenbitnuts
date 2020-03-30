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
        public EffectService? EffectService { get; private set; }
        public TilesetService TilesetService { get; private set; }

        #endregion

        #region Components

        private RenderTarget2D? renderSurface;
        private RenderTarget2D? preMainDisplayEffectRenderTarget;
        private readonly GraphicsDeviceManager graphics;
        private Process process;

        protected Scene? currentScene;

        #endregion

        #region FMOD

        public FMOD.Studio.System FmodSystem => fmodSystem;

        private FMOD.Studio.System fmodSystem;

        private FMOD.Studio.Bank masterBank;
        private FMOD.Studio.Bank stringBank;
        private FMOD.Studio.Bank musicBank;
        private FMOD.Studio.Bank sfxBank;

        private FMOD.Studio.EventInstance currentMusic;

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
            EffectService = new EffectService(this);

            renderSurface = new RenderTarget2D(GraphicsDevice, (int)InternalSize.Width, (int)InternalSize.Height);
            preMainDisplayEffectRenderTarget = new RenderTarget2D(GraphicsDevice, (int)InternalSize.Width, (int)InternalSize.Height);

            #region FMOD Initialization
            
            FMOD.RESULT result;

            // Initialize
            result = FMOD.Studio.System.create(out fmodSystem);
            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine(FMOD.Error.String(result));
            }

            result = fmodSystem.initialize(32, FMOD.Studio.INITFLAGS.LIVEUPDATE, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine(FMOD.Error.String(result));
            }

            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            fmodSystem.loadBankFile("Audio/Master.bank", FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out masterBank);
            fmodSystem.loadBankFile("Audio/Master.strings.bank", FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out stringBank);
            fmodSystem.loadBankFile("Audio/music.bank", FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out musicBank);
            fmodSystem.loadBankFile("Audio/sfx.bank", FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out sfxBank);
        }

        protected override void UnloadContent()
        {
            musicBank.unload();
            stringBank.unload();
            masterBank.unload();
            sfxBank.unload();
        }

        protected override void Update(GameTime gameTime)
        {
            process = Process.GetCurrentProcess();
            Window.Title = WindowTitle + " - " + (process.PrivateMemorySize64 / (1024f * 1024f)) + " MB";

            base.Update(gameTime);

            fmodSystem.update();

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

                if (currentScene != null && currentScene.HasPreMainDisplayEffectDraws)
                {
                    GraphicsDevice.SetRenderTarget(preMainDisplayEffectRenderTarget);
                    currentScene.PreMainDisplayEffectDraw();
                }

                // Apply visual display effects
                if (EffectService is EffectService service && renderSurface != null)
                {
                    renderSurface = service.ApplyEnabledDisplayEffects(renderSurface);
                }

                // Debug visual representation
                currentScene?.DebugDraw();

                // Back to the normal render method
                GraphicsDevice.SetRenderTarget(null);

                // Render the surface to have the ingame screen
                SpriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);

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

        public void PlayMusic(string name)
        {
            FMOD.RESULT result = fmodSystem.getEvent("event:/music/" + name, out FMOD.Studio.EventDescription eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine(FMOD.Error.String(result) + " (event:/music/" + name + ")");
            }
            eventDescription.createInstance(out currentMusic);

            result = eventDescription.loadSampleData();
            if (result == FMOD.RESULT.OK)
            {
                currentMusic.start();
            }
        }

        public void SetMusicParameter(string name, float value)
        {
            currentMusic.setParameterByName(name, value);
        }

        public void PlaySound(string name)
        {
            FMOD.RESULT result = fmodSystem.getEvent("event:/sfx/" + name, out FMOD.Studio.EventDescription eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine(FMOD.Error.String(result) + " (event:/sfx/" + name + ")");
            }

            eventDescription.createInstance(out FMOD.Studio.EventInstance sound);
            sound.start();
        }

        protected override void Dispose(bool disposing)
        {
            SpriteBatch?.Dispose();
            renderSurface?.Dispose();
            graphics.Dispose();

            fmodSystem.release();

            base.Dispose(disposing);
        }
    }
}
