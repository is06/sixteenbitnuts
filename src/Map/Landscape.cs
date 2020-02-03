using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    enum LayerIndex
    {
        StaticBackground = 0,
        Background4 = 1,
        Background3 = 2,
        Background2 = 3,
        Background1 = 4,
        Main = 5,
        Foreground1 = 6,
        Foreground2 = 7
    }

    [Serializable]
    struct LandscapeLayer : ISerializable
    {
        public string Name;
        public LayerIndex Index;
        public Texture2D Texture;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("index", Index);
        }
    }

    [Serializable]
    class Landscape : ISerializable
    {
        private readonly Map map;

        public List<LandscapeLayer> Layers { get; set; }
        public string Name { get; set; }

        public Landscape(Map map)
        {
            this.map = map;
            Layers = new List<LandscapeLayer>();
        }

        public void Draw(int layerIndex)
        {
            foreach (var layer in Layers)
            {
                if ((int)layer.Index == layerIndex)
                {
                    map.Game.SpriteBatch.Draw(
                        texture: layer.Texture,
                        position: Vector2.Zero,
                        sourceRectangle: new Rectangle(0, 0, 1024, 270),
                        color: Color.White,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: Vector2.One,
                        effects: SpriteEffects.None,
                        layerDepth: 0f
                    );
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("layers", Layers);
        }
    }
}
