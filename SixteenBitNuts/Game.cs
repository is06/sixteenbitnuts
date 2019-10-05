using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SixteenBitNuts
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Properties

        protected Rectangle WindowSize { get; set; }
        protected Rectangle InternalSize { get; set; }
        protected int FrameRate { get; set; }

        public Viewport InGameViewport { get; private set; }
        public float ScreenScale
        {
            get
            {
                return WindowSize.Width / (float)InternalSize.Width;
            }
        }

        #endregion

        #region Components

        private SpriteBatch spriteBatch;
        private RenderTarget2D renderSurface;        
        private readonly GraphicsDeviceManager graphics;

        private bool isInConsoleMode;
        private bool keyConsolePressed;

        protected Console console;

        protected Scene currentScene;

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            
        }

        protected override void Initialize()
        {
            // Graphics
            graphics.PreferredBackBufferWidth = WindowSize.Width;
            graphics.PreferredBackBufferHeight = WindowSize.Height;
            graphics.ApplyChanges();

            // Misc
            Window.AllowUserResizing = false;
            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan((int)(1000f / FrameRate * 10000f));
            InGameViewport = new Viewport(0, 0, InternalSize.Width, InternalSize.Height);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderSurface = new RenderTarget2D(GraphicsDevice, InternalSize.Width, InternalSize.Height);

            base.Initialize();

            // Debug console
            KeyboardDispatcher keyboardDispatcher = new KeyboardDispatcher(Window);
            console = new Console(this, keyboardDispatcher);

            console.OnLoadMap += ConsoleLoadMap;
            console.OnExitGame += ConsoleExitGame;
            console.OnEditSection += ConsoleEditSection;
            console.OnEditMap += ConsoleEditMap;
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
            #region Debug console

            if (!keyConsolePressed && Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                keyConsolePressed = true;
                isInConsoleMode = !isInConsoleMode;
                console.Enabled = isInConsoleMode;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.F4))
            {
                keyConsolePressed = false;
            }

            console.Update();

            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Big pixel graphics rendering
            {
                // Change the render target to surface
                GraphicsDevice.SetRenderTarget(renderSurface);
                GraphicsDevice.Clear(Color.Black);

                // Draws everything in the surface
                currentScene.Draw();

                // Debug visual representation
                currentScene.DebugDraw();

                // Back to the normal render method
                GraphicsDevice.SetRenderTarget(null);

                // Render the surface to have the ingame screen
                spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp
                );
                spriteBatch.Draw(
                    texture: renderSurface,
                    destinationRectangle: new Rectangle(0, 0, WindowSize.Width, WindowSize.Height),
                    sourceRectangle: new Rectangle(0, 0, InGameViewport.Width, InGameViewport.Height),
                    color: Color.White
                );
                spriteBatch.End();
            }

            // Hi-res graphics rendering
            {
                // Render all UI elements in front of the render target texture
                currentScene.UIDraw(gameTime);

                console.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public virtual void LoadMap(string name)
        {

        }

        protected override void Dispose(bool disposing)
        {
            spriteBatch.Dispose();
            renderSurface.Dispose();
            graphics.Dispose();

            base.Dispose(disposing);
        }

        #region Console events

        private void ConsoleLoadMap(Console sender, ConsoleEventArgs args)
        {
            string mapName = args.Parameters[0];
            LoadMap(mapName);
        }

        private void ConsoleEditSection(Console sender, ConsoleEventArgs args)
        {
            currentScene.EditCurrentSection();
        }

        private void ConsoleEditMap(Console sender, ConsoleEventArgs args)
        {
            currentScene.EditLayout();
        }

        private void ConsoleExitGame(Console sender, ConsoleEventArgs args)
        {
            Exit();
        }

        #endregion
    }
}
