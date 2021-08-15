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
        public Player(Map map, Size hitBoxSize, Direction direction = Direction.Right) : base(map, hitBoxSize, Color.Cyan)
        {
            Direction = direction;
            IsControllable = true;
            IsGravityEnabled = true;
        }

        public virtual void PostCollisionUpdate()
        {

        }

        public virtual void ComputePhysics()
        {
            position += Velocity;
        }

        public virtual void UpdateHitBoxes()
        {
            
        }

        public virtual void UpdateSensors()
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
