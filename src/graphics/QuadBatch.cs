using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public struct QuadFragment
    {
        public Rectangle Source;
        public Rectangle Destination;
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
            effect.TextureEnabled = true;
        }

        /// <summary>
        /// Loads texture for this quad batch
        /// </summary>
        /// <param name="texturePath"></param>
        public void LoadContent(string texturePath)
        {
            texture = game.Content.Load<Texture2D>(texturePath);
            effect.Texture = texture;
        }

        /// <summary>
        /// Draws all quad fragments for the current texture
        /// </summary>
        /// <param name="fragments"></param>
        public void Draw(QuadFragment[] fragments)
        {
            SetBuffers(fragments);
            DrawPrimitivesFromBuffers();
        }

        /// <summary>
        /// Sets the vertex and index buffer from fragments to draw
        /// </summary>
        /// <param name="fragments"></param>
        private void SetBuffers(QuadFragment[] fragments)
        {
            fragmentCount = fragments.Length;

            int vertexCount = fragments.Length * 4;
            vertices = new VertexPositionColorTexture[vertexCount];

            int indexCount = fragments.Length * 6;
            indices = new int[indexCount];

            for (int i = 0; i < fragmentCount; i++)
            {
                int firstVertex = i * 4;
                int firstIndex = i * 6;

                if (texture is Texture2D tex && vertices != null && indices != null)
                {
                    float left = (float)fragments[i].Source.X / (float)tex.Width;
                    float top = (float)fragments[i].Source.Y / (float)tex.Height;
                    float right = (float)fragments[i].Source.Width / (float)tex.Width + left;
                    float bottom = (float)fragments[i].Source.Height / (float)tex.Height + top;

                    vertices[firstVertex] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Left, fragments[i].Destination.Top, 0),
                        Color.White,
                        new Vector2(left, top)
                    );
                    vertices[firstVertex + 1] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Left, fragments[i].Destination.Bottom, 0),
                        Color.White,
                        new Vector2(left, bottom)
                    );
                    vertices[firstVertex + 2] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Right, fragments[i].Destination.Bottom, 0),
                        Color.White,
                        new Vector2(right, bottom)
                    );
                    vertices[firstVertex + 3] = new VertexPositionColorTexture(
                        new Vector3(fragments[i].Destination.Right, fragments[i].Destination.Top, 0),
                        Color.White,
                        new Vector2(right, top)
                    );

                    indices[firstIndex] = 0 + firstVertex;
                    indices[firstIndex + 1] = 2 + firstVertex;
                    indices[firstIndex + 2] = 1 + firstVertex;
                    indices[firstIndex + 3] = 2 + firstVertex;
                    indices[firstIndex + 4] = 0 + firstVertex;
                    indices[firstIndex + 5] = 3 + firstVertex;
                }
            }

            if (vertices != null)
            {
                vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(vertices);
            }
            
            if (indices != null)
            {
                indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(indices);
            }
        }

        /// <summary>
        /// Actually draws all primitives from buffer into graphics device
        /// </summary>
        private void DrawPrimitivesFromBuffers()
        {
            if (vertices != null && indices != null && effect != null)
            {
                game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                game.GraphicsDevice.Indices = indexBuffer;

                //Vector3 cameraUp = Vector3.Transform(new Vector3(0, -1, 0), Matrix.CreateRotationZ(0f));
                effect.View = Matrix.CreateLookAt(cameraScrollPosition, cameraScrollLookAt, new Vector3(0, -1, 0));

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    int primitiveCount = fragmentCount * 2;

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
