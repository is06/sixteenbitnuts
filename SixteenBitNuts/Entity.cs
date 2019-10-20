using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Entity : MapElement
    {
        public string Name { get; private set; }

        protected Texture2D texture;
        protected Vector2 drawPosition;

        public Entity(Map map, string name) : base(map)
        {
            Name = name;
            DebugColor = Color.Orange;
        }

        public override void Update(GameTime gameTime)
        {
            drawPosition = Position;

            base.Update(gameTime);
        }

        public override void Draw()
        {
            if (IsVisible)
            {
                map.Game.SpriteBatch.Draw(
                    texture: texture,
                    position: drawPosition,
                    sourceRectangle: new Rectangle(Point.Zero, Size.ToPoint()),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Vector2.One,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );
            }

            base.Draw();
        }

        public void Destroy()
        {
            IsDestroying = true;
        }
    }
}
