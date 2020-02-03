using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    class EditorLabel : Label
    {
        public EditorLabel(Scene scene) : base(scene)
        {
            
        }

        protected override void InitFont()
        {
            base.InitFont();

            font = scene.Game.Content.Load<SpriteFont>("Engine/fonts/numbers");
        }
    }
}
