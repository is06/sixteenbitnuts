using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public struct ImageCounterConfig
    {
        public string DisabledSpriteAnimationName;
        public string EnabledSpriteAnimationName;
    }

    public class ImageCounter : Counter
    {
        private readonly string disabledSpriteAnimationName;
        private readonly string enabledSpriteAnimationName;

        private readonly List<Sprite> sprites;

        public ImageCounter(Hud hud, CounterConfig config, ImageCounterConfig imageConfig, string spriteName) : base(hud, config)
        {
            sprites = new List<Sprite>();

            disabledSpriteAnimationName = imageConfig.DisabledSpriteAnimationName;
            enabledSpriteAnimationName = imageConfig.EnabledSpriteAnimationName;

            for (var i = 0; i < config.MaxValue;  i++)
            {
                var sprite = new Sprite(hud.Game, spriteName)
                {
                    AnimationName = disabledSpriteAnimationName
                };
                sprites.Add(sprite);
            }

            OnValueChanged += ImageCounter_OnValueChanged;

            ImageCounter_OnValueChanged(Value, 0);
        }

        public override void Draw(Matrix transform)
        {
            base.Draw(transform);

            float xpos = 0;
            foreach (var sprite in sprites)
            {
                sprite.Draw(new Vector2(Position.X + xpos, Position.Y), 0, Matrix.Identity);
                xpos += sprite.CurrentAnimation.Size.Width;
            }
        }

        private void ImageCounter_OnValueChanged(int value, int previousValue)
        {
            if (sprites.Count == MaxValue)
            {
                for (var i = 0; i < MaxValue; i++)
                {
                    sprites[i].AnimationName = i <= (value - 1)
                        ? enabledSpriteAnimationName
                        : disabledSpriteAnimationName;
                }
            }
        }
    }
}
