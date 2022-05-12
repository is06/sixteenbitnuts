using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class DebugQuad
    {
        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }

        private readonly Game game;
        private readonly Texture2D texture;

        public DebugQuad(Game game, Rectangle bounds, Color color)
        {
            this.game = game;
            texture = new Texture2D(game.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            Bounds = bounds;
            Color = color;
        }

        public void Draw()
        {
            if (game.SpriteBatch is SpriteBatch batch)
            {
                batch.Draw(
                    texture: texture,
                    position: Bounds.Location.ToVector2(),
                    sourceRectangle: new Rectangle(0, 0, 1, 1),
                    color: Color,
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: Bounds.Size.ToVector2(),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
            }
        }
    }
}
