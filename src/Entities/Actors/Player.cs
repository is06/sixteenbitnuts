using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    /// <summary>
    /// Class representing the playable character
    /// </summary>
    public abstract class Player
    {
        protected Vector2 position;
        protected Vector2 velocity;

        #region Properties

        public bool IsControllable { get; set; }
        public Direction Direction { get; set; }
        public Size Size { get; set; }
        public HitBox HitBox { get; set; }
        public HitBox PreviousFrameHitBox { get; set; }
        public HitBox AttackBox { get; set; }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                HitBox = new HitBox
                {
                    Position = position,
                    Size = HitBox.Size
                };
            }
        }
        public Vector2 DrawingPosition
        {
            get
            {
                return new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
            }
        }
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        #endregion

        #region Components

        protected Map map;
        protected Sprite? sprite;

        protected DebugHitBox debugHitBox;  
        protected DebugHitBox debugPreviousFrameHitBox;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        public Player(Map map)
        {
            // Fields
            this.map = map;
            Direction = Direction.Right;

            // Hitboxes
            HitBox = new HitBox(position, Size);
            PreviousFrameHitBox = new HitBox(position, Size);

            // Properties
            IsControllable = true;

            // Debug
            debugHitBox = new DebugHitBox(map.Game, 1, Color.Cyan);
            debugPreviousFrameHitBox = new DebugHitBox(map.Game, 2, Color.DarkOliveGreen);
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
            position += velocity;
        }

        public virtual void UpdateHitBox()
        {
            
        }

        public virtual void UpdateDebugHitBoxes()
        {
            debugHitBox.Update(HitBox);
            debugPreviousFrameHitBox.Update(PreviousFrameHitBox);
        }

        /// <summary>
        /// Draw player sprite
        /// </summary>
        public void Draw()
        {
            sprite?.Draw(position: DrawingPosition - sprite.HitBoxOffset, layer: 0f);
        }

        /// <summary>
        /// Draw debug info of the player sprite
        /// </summary>
        public virtual void DebugDraw()
        {
            debugPreviousFrameHitBox.Draw();
            debugHitBox.Draw();
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
