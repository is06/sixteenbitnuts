using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class PrimitiveBatch
    {
        private static Matrix scale2dMatrix = Matrix.CreateScale(1, -1, 1);

        protected readonly Game game;
        protected BasicEffect effect;

        protected Vector3 cameraScrollPosition = new Vector3(0, 0, -1);
        protected Vector3 cameraScrollLookAt = new Vector3(0, 0, 0);

        public PrimitiveBatch(Game game)
        {
            this.game = game;

            effect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                Projection = scale2dMatrix * Matrix.CreateOrthographicOffCenter(0, game.InternalSize.X, game.InternalSize.Y, 0, 0, 1)
            };
        }
    }
}
