using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Scene
    {
        public Game Game { get; private set; }

        public Scene(Game game)
        {
            Game = game;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw()
        {

        }

        public virtual void DebugDraw()
        {

        }

        public virtual void UIDraw()
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
