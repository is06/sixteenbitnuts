using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace SixteenBitNuts
{
    public delegate void AnimationFinishedHandler(Sprite sender);

    public class Sprite
    {
        public event AnimationFinishedHandler OnAnimationFinished;

        #region Fields

        private string textureName;
        private string currentAnimationName;
        private float currentAnimationFrame;
        private bool isAnimated;

        #endregion

        #region Properties

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
                    currentAnimationFrame = 0f;
                    isAnimated = true;
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

        public Vector2 HitBoxOffset
        {
            get
            {
                return animations[currentAnimationName].HitBoxOffset.ToVector2();
            }
        }

        #endregion

        #region Components

        private readonly Texture2D texture;
        private readonly SpriteBatch spriteBatch;
        private readonly Dictionary<string, SpriteAnimation> animations;

        #endregion

        public Sprite(string name, GraphicsDevice graphics, ContentManager content)
        {
            // Fields
            currentAnimationName = "idle";
            currentAnimationFrame = 0f;

            // Properties
            Direction = Direction.Right;

            // Components
            spriteBatch = new SpriteBatch(graphics);
            animations = new Dictionary<string, SpriteAnimation>();

            // Loading sprite descriptor and texture
            LoadFromFile("Data/sprites/" + name + ".sprite");
            texture = content.Load<Texture2D>("Game/sprites/" + textureName);
        }

        public void Draw(Vector2 position, float layer, Matrix transform)
        {
            Point size = new Point(
                CurrentAnimation.Size.X,
                CurrentAnimation.Size.Y
            );

            Point offset = new Point(
                CurrentAnimation.DirectionOffsets[Direction].X + ((int)Math.Floor(currentAnimationFrame) * size.X),
                CurrentAnimation.DirectionOffsets[Direction].Y
            );

            spriteBatch.Begin(transformMatrix: transform);
            spriteBatch.Draw(
                texture: texture,
                position: new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y)),
                sourceRectangle: new Rectangle(
                    offset.X,
                    offset.Y,
                    CurrentAnimation.Size.X,
                    CurrentAnimation.Size.Y
                ),
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: layer
            );
            spriteBatch.End();

            // Increment animation frame counter
            if (isAnimated)
            {
                currentAnimationFrame += CurrentAnimation.Speed;
            }
            
            // End of animation sequence
            if (currentAnimationFrame >= CurrentAnimation.Length)
            {
                if (CurrentAnimation.Looped)
                {
                    currentAnimationFrame = 0;
                }
                else
                {
                    OnAnimationFinished?.Invoke(this);
                    isAnimated = false;
                }
            }
        }

        public void DebugDraw()
        {
            // TODO: The problem is the variety of sprite size in animations
        }

        private void LoadFromFile(string fileName)
        {
            string animationName = "";

            foreach (string line in File.ReadAllLines(fileName))
            {
                string[] components = line.Split(' ');

                if (components[0] == "tx")
                {
                    textureName = components[1];
                }
                if (components[0] == "an")
                {
                    animationName = components[1];
                    animations[animationName] = new SpriteAnimation()
                    {
                        Name = components[1],
                        Size = new Point(int.Parse(components[2]), int.Parse(components[3])),
                        HitBoxOffset = new Point(int.Parse(components[4]), int.Parse(components[5])),
                        Length = int.Parse(components[6]),
                        Speed = float.Parse(components[7], CultureInfo.InvariantCulture),
                        Looped = int.Parse(components[8]) == 1,
                        DirectionOffsets = new Dictionary<Direction, Point>()
                    };
                }
                if (components[0] == "di")
                {
                    Direction direction = (Direction)int.Parse(components[1]);

                    animations[animationName].DirectionOffsets[direction] = new Point(
                        int.Parse(components[2]),
                        int.Parse(components[3])
                    );
                }
            }
        }
    }
}
