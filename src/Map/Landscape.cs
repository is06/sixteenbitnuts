using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    [Serializable]
    public struct LandscapeLayer : ISerializable
    {
        public string Name;
        public int Index;
        public Texture2D Texture;
        public Vector2 TransformOffset;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("index", Index);
            info.AddValue("offsetX", TransformOffset.X);
            info.AddValue("offsetY", TransformOffset.Y);
        }
    }

    [Serializable]
    public class Landscape : ISerializable
    {
        private readonly Map map;

        public Dictionary<int, LandscapeLayer> Layers { get; set; }
        public string Name { get; }

        public Landscape(Map map, string name)
        {
            this.map = map;
            Name = name;
            Layers = new Dictionary<int, LandscapeLayer>();
        }

        public void Draw(int layerIndex, Matrix transform)
        {
            map.Game.SpriteBatch?.Begin(
                transformMatrix: transform,
                samplerState: SamplerState.PointWrap
            );
            map.Game.SpriteBatch?.Draw(
                texture: Layers[layerIndex].Texture,
                position: Vector2.Zero,
                sourceRectangle: new Rectangle(0, 0, (int)map.Size.Width, (int)map.Size.Height),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
            map.Game.SpriteBatch?.End();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("layers", Layers);
        }
    }
}
