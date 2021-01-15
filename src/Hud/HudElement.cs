using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    abstract public class HudElement : IHudElement
    {
        protected Hud hud;

        public bool IsVisible { get; set; }
        public Vector2 Position { get; set; }
        public Size Size { get; set; }

        public HudElement(Hud hud)
        {
            this.hud = hud;
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Draw(Matrix transform)
        {
            
        }

        public virtual void DebugDraw(Matrix transform)
        {
            
        }
    }
}
