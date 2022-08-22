using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class PrimitiveBatch
    {
        private static Matrix scale2dMatrix = Matrix.CreateScale(1, -1, 1);

        protected readonly Game game;
        protected BasicEffect? effect;

        public PrimitiveBatch(Game game)
        {
            this.game = game;
        }

        public virtual void Initialize()
        {
            var renderSize = game.CurrentScene != null && game.CurrentScene.RenderSurface != null
                ? game.CurrentScene.RenderSurface.Bounds.Size
                : game.InternalSize;

            effect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                Projection = scale2dMatrix * Matrix.CreateOrthographicOffCenter(0, renderSize.X, renderSize.Y, 0, 0, 1)
            };
        }
    }
}
