using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{ 
    public struct Line
    {
        public Point Origin;
        public Point Destination;
        public Color Color;
    }

    public class LineBatch : PrimitiveBatch
    {
        public LineBatch(Game game) : base(game) {}

        public void Draw(Line[] lines, Matrix transform)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[lines.Length * 2];

            if (effect is Effect)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    int firstVertex = i * 2;

                    vertices[firstVertex] = new VertexPositionColor(new Vector3(lines[i].Origin.X, lines[i].Origin.Y, 0), lines[i].Color);
                    vertices[firstVertex + 1] = new VertexPositionColor(new Vector3(lines[i].Destination.X, lines[i].Destination.Y, 0), lines[i].Color);
                }

                //Vector3 cameraUp = Vector3.Transform(new Vector3(0, -1, 0), Matrix.CreateRotationZ(0f));
                var invertedTransform = -transform;
                var cameraPosition = invertedTransform.Translation + new Vector3(0, 0, -1);
                effect.View = Matrix.CreateLookAt(cameraPosition, invertedTransform.Translation, new Vector3(0, -1, 0));

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    game.GraphicsDevice.DrawUserPrimitives(
                        primitiveType: PrimitiveType.LineList,
                        vertexData: vertices,
                        vertexOffset: 0,
                        primitiveCount: lines.Length
                    );
                }
            }
        }
    }
}
