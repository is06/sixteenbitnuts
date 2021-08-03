using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace SixteenBitNuts
{
    public delegate void AnimationFinishedHandler(Sprite sprite);

    public class Sprite
    {
        public event AnimationFinishedHandler? OnAnimationFinished;

        #region Fields

        private string currentAnimationName;

        #endregion

        #region Properties

        public bool IsAnimated { get; set; }
        public bool IsVisible { get; set; }
        public Effects.SpriteEffect? Effect { get; set; }
        public float CurrentAnimationFrame { get; private set; }
        public int CurrentAnimationLoops { get; private set; }
        public string AnimationName
        {
            get
            {
                return currentAnimationName;
            }
            set
            {
                if (value != currentAnimationName)
                {
                    currentAnimationName = value;
                    CurrentAnimationFrame = 0f;
                    CurrentAnimationLoops = 0;
                    IsAnimated = true;
                }
            }
        }
        public SpriteAnimation CurrentAnimation
        {
            get
            {
                return animations[AnimationName];
            }
        }
        public Direction Direction { get; set; }

        #endregion

        #region Components

        private readonly Texture2D texture;
        private readonly Game game;
        private readonly Dictionary<string, SpriteAnimation> animations;

        #endregion

        public Sprite(Game game, string name)
        {
            this.game = game;

            // Fields
            currentAnimationName = "idle";

            // Properties
            CurrentAnimationFrame = 0f;
            CurrentAnimationLoops = 0;
            IsVisible = true;
            Direction = Direction.Right;

            // Components
            animations = game.DescriptorLoader.LoadSpriteAnimations(name);
            texture = game.Content.Load<Texture2D>("Graphics/Sprites/" + name);
        }

        public void Draw(Vector2 position, float layer, Matrix transform)
        {
            if (IsVisible)
            {
                var sourceOffset = new Point(
                    CurrentAnimation.Directions[Direction].Offset.X + (int)(Math.Floor(CurrentAnimationFrame) * CurrentAnimation.Size.Width),
                    CurrentAnimation.Directions[Direction].Offset.Y
                );

                SpriteEffects flipEffects = SpriteEffects.None;
                if (CurrentAnimation.Directions[Direction].FlippedHorizontally) flipEffects |= SpriteEffects.FlipHorizontally;
                if (CurrentAnimation.Directions[Direction].FlippedVertically) flipEffects |= SpriteEffects.FlipVertically;

                Effect?.Shader?.Parameters["TextureSize"]?.SetValue(texture.Bounds.Size.ToVector2());
                Effect?.Update();

                game.SpriteBatch?.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    Effect?.Shader,
                    transform
                );

                var drawPosition = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

                var drawPositionOffset = CurrentAnimation.HitBoxOffset.ToVector2();
                if (CurrentAnimation.Directions[Direction].OverrideHitBoxOffset is Point overrideHitBoxOffset)
                {
                    drawPositionOffset = overrideHitBoxOffset.ToVector2();
                }

                game.SpriteBatch?.Draw(
                    texture: texture,
                    position: drawPosition - drawPositionOffset,
                    sourceRectangle: new Rectangle(
                        sourceOffset.X,
                        sourceOffset.Y,
                        (int)CurrentAnimation.Size.Width,
                        (int)CurrentAnimation.Size.Height
                    ),
                    color: Color.White,
                    rotation: 0f,
                    origin: new Vector2(0, 0),
                    scale: Vector2.One,
                    effects: flipEffects,
                    layerDepth: layer
                );

                game.SpriteBatch?.End();

                // Increment animation frame counter
                if (IsAnimated)
                {
                    CurrentAnimationFrame += CurrentAnimation.Speed;
                }

                // End of animation sequence
                if (CurrentAnimationFrame >= CurrentAnimation.Length)
                {
                    if (CurrentAnimation.Looped)
                    {
                        CurrentAnimationFrame = 0;
                        CurrentAnimationLoops++;
                    }
                    else
                    {
                        CurrentAnimationFrame--;
                        OnAnimationFinished?.Invoke(this);
                        IsAnimated = false;
                    }
                }
            }
        }

        public void ResetCurrentAnimation()
        {
            CurrentAnimationFrame = 0f;
            CurrentAnimationLoops = 0;
        }
    }

    public struct SpriteDirection
    {
        public Point Offset;
        public bool FlippedHorizontally;
        public bool FlippedVertically;
        public Point? OverrideHitBoxOffset;
    }

    public struct SpriteAnimation
    {
        public string Name;
        public Size Size;
        public Point HitBoxOffset;
        public int Length;
        public float Speed;
        public Dictionary<Direction, SpriteDirection> Directions;
        public bool Looped;
    }
}
