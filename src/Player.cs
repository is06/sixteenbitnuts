using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    /// <summary>
    /// Class representing the playable character
    /// </summary>
    public class Player
    {
        protected Vector2 position;
        protected Vector2 velocity;

        #region Properties

        public bool IsControllable { get; set; }
        public bool IsRunning { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDucking { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsPunching { get; set; }
        public bool IsBouncing { get; set; }
        public bool IsDashFalling { get; set; }
        public bool IsFalling { get; set; }
        public bool IsTouchingTheGround { get; set; }
        public bool IsTouchingTheCeiling { get; set; }
        public bool WasOnPlatform { get; set; }
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
        }
        public float Weight { get; protected set; }

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
            IsFalling = true;
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

        public void ComputePhysics()
        {
            velocity += map.Gravity * Weight;

            if (IsTouchingTheGround)
                velocity.Y = 0;

            if (IsTouchingTheCeiling)
            {
                IsTouchingTheCeiling = false;
                velocity.Y *= -0.5f;
            }

            position += velocity;
        }

        public void UpdateHitBox()
        {
            HitBox = new HitBox(
                new Vector2(position.X, (IsDucking || IsAttacking) ? position.Y + 8 : position.Y),
                new Size(Size.Width, (IsDucking || IsAttacking) ? Size.Height - 8 : Size.Height)
            );
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
