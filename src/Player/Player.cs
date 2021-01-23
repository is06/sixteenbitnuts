using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    /// <summary>
    /// Class representing the playable character
    /// </summary>
    public abstract class Player : Collider
    {
        // Exposed fields
        public bool IsControllable;

        // Read only properties
        public Direction Direction { get; protected set; }
        public Vector2 DrawingPosition
        {
            get
            {
                return new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
            }
        }
        
        // Private members
        protected Sprite? sprite;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        public Player(Map map) : base(map)
        {
            Direction = Direction.Right;
            IsControllable = true;
            IsPlayer = true;
        }

        /// <summary>
        /// Performs player calculations
        /// </summary>
        public virtual void Update(GameTime _)
        {
            // Memorize the previous frame hit box
            PreviousFrameHitBox = HitBox;
        }

        public virtual void ComputePhysics()
        {
            position += Velocity;
        }

        public virtual void UpdateHitBoxes()
        {
            
        }

        /// <summary>
        /// Draw player sprite
        /// </summary>
        public virtual void Draw(Matrix transform)
        {
            sprite?.Draw(
                position: DrawingPosition,
                layer: 0f,
                transform: transform
            );
        }

        public void MoveLeft(float value)
        {
            position.X -= value;
        }

        public void MoveRight(float value)
        {
            position.X += value;
        }

        public void MoveUp(float value)
        {
            position.Y -= value;
        }

        public void MoveDown(float value)
        {
            position.Y += value;
        }
    }
}
