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

        private string? textureName;
        private string currentAnimationName;

        public Effects.SpriteEffect? Effect { get; set; }

        #endregion

        #region Properties

        public bool IsAnimated { get; set; }
        public bool IsVisible { get; set; }
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
            animations = new Dictionary<string, SpriteAnimation>();

            // Loading sprite descriptor and texture
            LoadFromFile("Content/Descriptors/Sprites/" + name + ".sprite");

            texture = game.Content.Load<Texture2D>("Graphics/Sprites/" + textureName);
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

                Effect?.Effect?.Parameters["TextureSize"]?.SetValue(texture.Bounds.Size.ToVector2());
                Effect?.Update();

                game.SpriteBatch?.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    Effect?.Effect,
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

        protected virtual void LoadFromFile(string fileName)
        {
            string animationName = "";
            string[] lines;

            try
            {
                lines = File.ReadAllLines(fileName);
            }
            catch (DirectoryNotFoundException)
            {
                throw new GameException("Unable to find sprite descriptor file " + fileName);
            }

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "tx": LoadTexture(components); break;
                    case "an": animationName = LoadAnimation(components); break;
                    case "di": LoadAnimationDirection(animationName, components); break;
                }
            }
        }

        private void LoadTexture(string[] components)
        {
            textureName = components[1];
        }

        private string LoadAnimation(string[] components)
        {
            var animationName = components[1];
            animations[animationName] = new SpriteAnimation()
            {
                Name = components[1],
                Size = new Size(int.Parse(components[2]), int.Parse(components[3])),
                HitBoxOffset = new Point(int.Parse(components[4]), int.Parse(components[5])),
                Length = int.Parse(components[6]),
                Speed = float.Parse(components[7], CultureInfo.InvariantCulture),
                Looped = int.Parse(components[8]) == 1,
                Directions = new Dictionary<Direction, SpriteDirection>()
            };
            return animationName;
        }

        private void LoadAnimationDirection(string animationName, string[] components)
        {
            Direction direction = (Direction)int.Parse(components[1]);

            var spriteDirection = new SpriteDirection
            {
                Offset = new Point(
                    int.Parse(components[2]),
                    int.Parse(components[3])
                ),
                FlippedHorizontally = int.Parse(components[4]) == 1,
                FlippedVertically = int.Parse(components[5]) == 1
            };
            try
            {
                spriteDirection.OverrideHitBoxOffset = new Point(
                    int.Parse(components[6]),
                    int.Parse(components[7])
                );
            }
            catch (IndexOutOfRangeException)
            {

            }

            animations[animationName].Directions[direction] = spriteDirection;
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
