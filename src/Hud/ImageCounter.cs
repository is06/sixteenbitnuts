using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class ImageCounter : Counter
    {
        public string DisabledSpriteAnimationName { get; set; }
        public string EnabledSpriteAnimationName { get; set; }

        private readonly string spriteName;
        private readonly List<Sprite> sprites;

        public ImageCounter(Hud hud, string spriteName) : base(hud)
        {
            this.spriteName = spriteName;
            sprites = new List<Sprite>();

            DisabledSpriteAnimationName = "";
            EnabledSpriteAnimationName = "";

            OnValueChanged += ImageCounter_OnValueChanged;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            for (var i = 0; i < MaxValue; i++)
            {
                var sprite = new Sprite(hud.Game, spriteName)
                {
                    AnimationName = DisabledSpriteAnimationName
                };
                sprites.Add(sprite);
            }

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
                        ? EnabledSpriteAnimationName
                        : DisabledSpriteAnimationName;
                }
            }
        }
    }
}
