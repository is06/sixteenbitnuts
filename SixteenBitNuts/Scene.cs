using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Scene
    {
        public Game Game { get; private set; }
        public GraphicsDevice Graphics { get; set; }
        public ContentManager Content { get; set; }

        public Scene(Game game)
        {
            Game = game;
            Graphics = game.GraphicsDevice;
            Content = game.Content;
        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {

        }

        public virtual void DebugDraw()
        {

        }

        public virtual void UIDraw(GameTime gameTime)
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual void EditCurrentSection()
        {
            
        }

        public virtual void EditLayout()
        {
            
        }
    }
}
