using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public delegate void AnimationFinishedHandler(Sprite sprite);

    public class Sprite
    {
        public event AnimationFinishedHandler? OnAnimationFinished;

        public bool IsAnimated = true;
        public Point Position = Point.Zero;

        public SpriteAnimation? Animation
        {
            get
            {
                if (animations.ContainsKey(currentAnimationName)) {
                    return animations[currentAnimationName];
                }
                return null;
            }
        }
        public Direction Direction { get; private set; }

        private readonly string name;
        private readonly Game game;
        private Texture2D? texture;
        private float currentFrame = 0.0f;
        private string currentAnimationName = "idle";
        private Dictionary<string, SpriteAnimation> animations = new Dictionary<string, SpriteAnimation>();
        private Point drawPositionOffset;
        private Point sourceOffset;
        private SpriteEffects flipEffects;

        public Sprite(Game game, string name)
        {
            this.game = game;
            this.name = name;
        }

        public void Initialize()
        {
            animations = game.SpriteLoader.LoadAnimations(name);
        }

        public void LoadContent()
        {
            texture = game.Content.Load<Texture2D>("Graphics/Sprites/" + name);
        }

        public void StartAnimation(string name)
        {
            currentAnimationName = name;
            currentFrame = 0.0f;
        }

        public void Update()
        {
            if (Animation is SpriteAnimation anim)
            {
                if (IsAnimated)
                {
                    currentFrame += anim.Speed;
                }

                if (currentFrame >= anim.Length)
                {
                    if (anim.IsLooped)
                    {
                        currentFrame = 0f;
                    }
                    else
                    {
                        currentFrame--;
                        IsAnimated = false;
                        OnAnimationFinished?.Invoke(this);
                    }
                }

                drawPositionOffset = anim.Offset;
                if (anim.Directions[Direction].OverrideOffset is Point overrideOffset)
                {
                    drawPositionOffset = overrideOffset;
                }

                sourceOffset = new Point(
                    anim.Directions[Direction].Offset.X + (int)Math.Floor(currentFrame) * anim.Size.X,
                    anim.Directions[Direction].Offset.Y
                );

                flipEffects = SpriteEffects.None;
                if (anim.Directions[Direction].IsFlippedHorizontally)
                {
                    flipEffects |= SpriteEffects.FlipHorizontally;
                }
                if (anim.Directions[Direction].IsFlippedVertically)
                {
                    flipEffects |= SpriteEffects.FlipVertically;
                }
            }
        }

        public void Draw(Matrix transform)
        {
            if (game is Game && game.SpriteBatch is SpriteBatch batch && Animation is SpriteAnimation anim)
            {
                batch.Begin(
                    sortMode: SpriteSortMode.Immediate,
                    blendState: BlendState.AlphaBlend,
                    samplerState: SamplerState.PointClamp,
                    depthStencilState: null,
                    rasterizerState: null,
                    effect: null,
                    transformMatrix: transform
                );

                batch.Draw(
                    texture: texture,
                    position: (Position - drawPositionOffset).ToVector2(),
                    sourceRectangle: new Rectangle(sourceOffset, anim.Size),
                    color: Color.White,
                    rotation: 0.0f,
                    origin: Vector2.Zero,
                    scale: Vector2.One,
                    effects: flipEffects,
                    layerDepth: 0.0f
                );

                batch.End();
            }
        }

        public void UnloadContent()
        {
            texture?.Dispose();
        }
    }
}
