using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public abstract class Scene
    {
        public Game Game { get; private set; }
        public Point ScrollAreaSize { get; protected set; }
        public RenderTarget2D? RenderSurface { get; protected set; }
        public Rectangle RenderDestinationBounds { get; protected set; }

        public Scene(Game game)
        {
            Game = game;
            ScrollAreaSize = game.InternalSize;
            RenderDestinationBounds = new Rectangle(Point.Zero, game.InternalSize);
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void LoadContent()
        {
            
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

        public virtual void UnloadContent()
        {
            
        }
    }
}
