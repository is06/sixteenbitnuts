using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public enum TextAlignement
    {
        Left,
        Right,
    }

    public enum TextVerticalAlignment
    {
        Top,
        Bottom,
    }

    public class DigitCounter : Counter
    {
        private readonly SpriteFont spriteFont;
        protected TextAlignement alignement;
        protected TextVerticalAlignment verticalAlignement;

        public DigitCounter(Hud hud, string? fontName, CounterConfig config) : base(hud, config)
        {
            if (fontName == null)
            {
                fontName = "EngineGraphics/Fonts/nes";
            }
            spriteFont = hud.Game.Content.Load<SpriteFont>(fontName);
        }

        public override void Draw(Matrix transform)
        {
            base.Draw(transform);

            var str = Value.ToString().PadLeft(MaxValue.ToString().Length, '0');
            var size = spriteFont.MeasureString(str);
            var drawPosition = Position;

            if (alignement == TextAlignement.Right)
            {
                drawPosition.X -= size.X;
            }
            if (verticalAlignement == TextVerticalAlignment.Bottom)
            {
                drawPosition.Y -= size.Y;
            }

            hud.Game.SpriteBatch?.Begin();
            hud.Game.SpriteBatch?.DrawString(spriteFont, str, drawPosition, Color.White);
            hud.Game.SpriteBatch?.End();
        }
    }
}
