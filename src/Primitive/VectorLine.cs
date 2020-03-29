using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class VectorLine
    {
        private readonly Game game;

        public Vector2 Origin { get; set; }
        public Vector2 Destination { get; set; }
        public Color Color { get; set; }

        public VectorLine(Game game)
        {
            this.game = game;

            Color = Color.White;
        }

        public void Draw(Matrix transform)
        {
            var origin = new Vector3(Origin, 0) + transform.Translation;
            var destination = new Vector3(Destination, 0) + transform.Translation;

            var vertices = new[]
            {
                new VertexPositionColor(origin, Color),
                new VertexPositionColor(destination, Color),
            };

            game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
