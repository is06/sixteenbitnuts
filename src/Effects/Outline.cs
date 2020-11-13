using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class Outline : SpriteEffect
    {
        public float Thickness { get; set; }
        public Color Color { get; set; }

        public Outline(Game game) : base()
        {
            Effect = game.Content.Load<Effect>("EngineGraphics/Effects/outline");
        }

        public override void Update()
        {
            base.Update();

            Effect?.Parameters["OutlineColor"]?.SetValue(Color.ToVector4());
            Effect?.Parameters["Thickness"]?.SetValue(Thickness);
        }
    }
}
