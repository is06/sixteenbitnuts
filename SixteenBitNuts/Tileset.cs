using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Tileset
    {
        public GraphicsDevice Graphics { get; private set; }

        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D texture;
        private readonly Box debugHitBox;
        private readonly Dictionary<int, TileElement> elements;

        public string Name { get; private set; }

        public Tileset(GraphicsDevice graphicsDevice, ContentManager contentManager, string name)
        {
            // Properties
            Name = name;
            Graphics = graphicsDevice;

            // Components
            spriteBatch = new SpriteBatch(graphicsDevice);
            texture = contentManager.Load<Texture2D>("Game/tilesets/" + name);
            elements = new Dictionary<int, TileElement>();
            debugHitBox = new Box(graphicsDevice, new Rectangle(0, 0, 16, 16), 1, Color.DarkRed);

            LoadFromFile("Data/tilesets/" + name + ".tileset");
        }

        public void Draw(Vector2 position, Vector2 size, Vector2 offset, float layer, Matrix transform)
        {
            spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                texture: texture,
                position: new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y)),
                sourceRectangle: new Rectangle((int)offset.X, (int)offset.Y, (int)size.X, (int)size.Y),
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: layer
            );
            spriteBatch.End();
        }

        public void DebugDraw(Point position, Point size, Matrix transform)
        {
            debugHitBox.Bounds = new Rectangle(position, size);
            debugHitBox.Update();
            debugHitBox.Draw(transform);
        }

        public Vector2 GetOffsetFromId(int id)
        {
            try
            {
                return elements[id].Offset;
            }
            catch (KeyNotFoundException)
            {
                throw new TileOffsetException("Offset for tile " + id + " was not found in tileset '" + Name + ".tileset'. The file may be empty.");
            }   
        }

        public Vector2 GetSizeFromId(int id)
        {
            try
            {
                return elements[id].Size;
            }
            catch (KeyNotFoundException)
            {
                throw new TileOffsetException("Size for tile " + id + " was not found in tileset '" + Name + ".tileset'. The file may be empty.");
            }
        }

        public TileType GetTypeFromId(int id)
        {
            return elements[id].Type;
        }

        private void LoadFromFile(string fileName)
        {
            int index = 0;
            foreach (string line in File.ReadAllLines(fileName))
            {
                string[] components = line.Split(' ');

                if (components[0] == "ti")
                {
                    TileElement element;
                    element.Size = new Vector2(int.Parse(components[1]), int.Parse(components[2]));
                    element.Offset = new Vector2(int.Parse(components[3]), int.Parse(components[4]));
                    element.Type = (TileType)int.Parse(components[5]);

                    elements[index] = element;
                    index++;
                }
            }
        }
    }
}
