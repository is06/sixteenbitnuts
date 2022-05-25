using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public struct QuadFragment
    {
        public Rectangle Source;
        public Rectangle Destination;
    }

    public class QuadBatch
    {
        private int fragmentCount;
        private VertexBuffer? vertexBuffer;
        private VertexPositionColorTexture[]? vertices;
        private IndexBuffer? indexBuffer;
        private int[]? indices;

        private readonly Game game;
        private string? textureName;
        private Texture2D? texture;
        private BasicEffect? effect;

        // Camera position
        private Vector3 cameraScrollPosition = new Vector3(0, 0, -1);
        private Vector3 cameraScrollLookAt = new Vector3(0, 0, 0);

        public QuadBatch(Game game)
        {
            this.game = game;
        }

        public void LoadContent(string textureName, QuadFragment[] fragments)
        {
            this.textureName = textureName;

            fragmentCount = fragments.Length;

            int vertexCount = fragments.Length * 4;
            vertices = new VertexPositionColorTexture[vertexCount];

            int indexCount = fragments.Length * 6;
            indices = new int[indexCount];

            texture = game.Content.Load<Texture2D>(textureName);

            Viewport vp = new Viewport(new Rectangle(Point.Zero, game.InternalSize));

            effect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true,
                Texture = texture,
                World = Matrix.Identity,
                Projection = Matrix.CreateScale(1, -1, 1) * Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, 1)
            };

            SetupVertices(fragments);
        }

        private void SetupVertices(QuadFragment[] fragments)
        {
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
        }

        public void DrawFragments()
        {
            if (vertices != null && indices != null)
            {
                vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(vertices);

                indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(indices);

                game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                game.GraphicsDevice.Indices = indexBuffer;

                if (effect is BasicEffect e)
                {
                    //Vector3 cameraUp = Vector3.Transform(new Vector3(0, -1, 0), Matrix.CreateRotationZ(0f));
                    e.View = Matrix.CreateLookAt(cameraScrollPosition, cameraScrollLookAt, new Vector3(0, -1, 0));

                    foreach (var pass in e.CurrentTechnique.Passes)
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
}
