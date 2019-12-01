using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    class DebugLabel : Label
    {
        public DebugLabel(Scene scene) : base(scene)
        {

        }

        protected override void InitFont()
        {
            base.InitFont();

            font = scene.Game.Content.Load<SpriteFont>("Engine/fonts/console");
        }
    }
}
