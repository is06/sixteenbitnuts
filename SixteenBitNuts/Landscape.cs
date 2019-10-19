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

    struct LandscapeLayer
    {
        public Texture2D Texture;
        public LayerIndex LayerIndex;
    }

    class Landscape
    {
        private readonly Map map;

        public List<LandscapeLayer> Layers { get; set; }

        public Landscape(Map map)
        {
            this.map = map;
            Layers = new List<LandscapeLayer>
            {
                new LandscapeLayer()
                {
                    Texture = map.Game.Content.Load<Texture2D>("Game/backgrounds/forest"),
                    LayerIndex = LayerIndex.Background1
                }
            };
        }

        public void Draw(int layerIndex)
        {
            foreach (var layer in Layers)
            {
                if ((int)layer.LayerIndex == layerIndex)
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
    }
}
