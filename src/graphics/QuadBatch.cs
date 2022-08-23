using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public struct QuadFragment
    {
        public Rectangle Source;
        public Rectangle Destination;
        public bool IsFlippedHorizontally;
        public bool IsFlippedVertically;
    }

    public class QuadBatch : PrimitiveBatch
    {
        private int fragmentCount;
        private VertexBuffer? vertexBuffer;
        private VertexPositionColorTexture[]? vertices;
        private IndexBuffer? indexBuffer;
        private int[]? indices;
        private Texture2D? texture;

        public QuadBatch(Game game) : base(game)
        {
            
        }

        /// <summary>
        /// Loads texture for this quad batch
        /// </summary>
        /// <param name="texturePath"></param>
        public void LoadContent(string texturePath)
        {
            if (effect is Effect)
            {
                texture = game.Content.Load<Texture2D>(texturePath);
                effect.TextureEnabled = true;
                effect.Texture = texture;
            }
        }

        /// <summary>
        /// Draws all quad fragments for the current texture
        /// </summary>
        /// <param name="fragments">List of quad fragments to draw</param>
        /// <param name="transform">Transform matrix to apply</param>
        public void Draw(QuadFragment[] fragments, Matrix transform)
        {
            SetBuffers(fragments);
            DrawPrimitivesFromBuffers(transform);
        }

        /// <summary>
        /// Sets the vertex and index buffer from fragments to draw
        /// </summary>
        /// <param name="fragments">Quad fragments to draw</param>
        private void SetBuffers(QuadFragment[] fragments)
        {
            fragmentCount = fragments.Length;

            int vertexCount = fragments.Length * 4;
            vertices = new VertexPositionColorTexture[vertexCount];

            int indexCount = fragments.Length * 6;
            indices = new int[indexCount];

            if (texture is Texture2D tex && vertices != null && indices != null)
            {
                float textureWidth = (float)tex.Width;
                float textureHeight = (float)tex.Height;

                for (int i = 0; i < fragmentCount; i++)
                {
                    int firstVertex = i * 4;
                    int firstIndex = i * 6;

                    float left = (float)fragments[i].Source.X / textureWidth;
                    float top = (float)fragments[i].Source.Y / textureHeight;
                    float right = (float)fragments[i].Source.Width / textureWidth + left;
                    float bottom = (float)fragments[i].Source.Height / textureHeight + top;

                    vertices[firstVertex] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Left, fragments[i].Destination.Top, 0),
                        Color.White,
                        new Vector2(
                            fragments[i].IsFlippedHorizontally ? right : left,
                            fragments[i].IsFlippedVertically ? bottom : top
                        )
                    );
                    vertices[firstVertex + 1] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Left, fragments[i].Destination.Bottom, 0),
                        Color.White,
                        new Vector2(
                            fragments[i].IsFlippedHorizontally ? right : left,
                            fragments[i].IsFlippedVertically ? top : bottom
                        )
                    );
                    vertices[firstVertex + 2] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Right, fragments[i].Destination.Bottom, 0),
                        Color.White,
                        new Vector2(
                            fragments[i].IsFlippedHorizontally ? left : right,
                            fragments[i].IsFlippedVertically ? top : bottom
                        )
                    );
                    vertices[firstVertex + 3] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Right, fragments[i].Destination.Top, 0),
                        Color.White,
                        new Vector2(
                            fragments[i].IsFlippedHorizontally ? left : right,
                            fragments[i].IsFlippedVertically ? bottom : top
                        )
                    );

                    indices[firstIndex] = 0 + firstVertex;
                    indices[firstIndex + 1] = 2 + firstVertex;
                    indices[firstIndex + 2] = 1 + firstVertex;
                    indices[firstIndex + 3] = 2 + firstVertex;
                    indices[firstIndex + 4] = 0 + firstVertex;
                    indices[firstIndex + 5] = 3 + firstVertex;
                }
            }

            if (vertices != null && vertices.Length > 0)
            {
                vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(vertices);
            }
            
            if (indices != null && indices.Length > 0)
            {
                indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(indices);
            }
        }

        /// <summary>
        /// Actually draws all primitives from buffer into graphics device
        /// </summary>
        /// <param name="transform">Transform matrix to apply</param>
        private void DrawPrimitivesFromBuffers(Matrix transform)
        {
            if (vertices != null && vertices.Length > 0
                && indices != null && indices.Length > 0
                && effect != null)
            {
                game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                game.GraphicsDevice.Indices = indexBuffer;

                //Vector3 cameraUp = Vector3.Transform(new Vector3(0, -1, 0), Matrix.CreateRotationZ(0f));
                //var cameraPosition = new Vector3(0, 0, -1) - transform.Translation;
                var invertedTransform = -transform;
                var cameraPosition = invertedTransform.Translation + new Vector3(0, 0, -1);
                effect.View = Matrix.CreateLookAt(cameraPosition, invertedTransform.Translation, new Vector3(0, -1, 0));

                int primitiveCount = fragmentCount * 2;

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Draw current vertex and index buffer content
                    game.GraphicsDevice.DrawUserIndexedPrimitives(
                        primitiveType: PrimitiveType.TriangleList,
                        vertexData: vertices,
                        vertexOffset: 0,
                        numVertices: vertices.Length,
                        indexData: indices,
                        indexOffset: 0,
                        primitiveCount: primitiveCount
                    );
                }
            }
        }
    }
}
