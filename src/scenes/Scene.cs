using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public abstract class Scene
    {
        public Game Game { get; private set; }
        public Point ScrollAreaSize { get; protected set; }

        public Scene(Game game)
        {
            Game = game;
            ScrollAreaSize = game.InternalSize;
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
