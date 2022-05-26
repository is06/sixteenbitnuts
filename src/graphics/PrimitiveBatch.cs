using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class PrimitiveBatch
    {
        protected readonly Game game;
        protected BasicEffect effect;

        protected Vector3 cameraScrollPosition = new Vector3(0, 0, -1);
        protected Vector3 cameraScrollLookAt = new Vector3(0, 0, 0);

        public PrimitiveBatch(Game game)
        {
            this.game = game;

            Viewport vp = new Viewport(new Rectangle(Point.Zero, game.InternalSize));
            effect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                Projection = Matrix.CreateScale(1, -1, 1) * Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, 1)
            };
        }
    }
}
