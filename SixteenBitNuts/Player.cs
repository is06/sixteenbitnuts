using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SixteenBitNuts
{
    /// <summary>
    /// Class representing the playable character
    /// </summary>
    public class Player
    {
        #region Constants

        private const float RUN_SPEED = 1.75f;
        private const float JUMP_VELOCITY = 7f;
        private const float JUMP_VELOCITY_DECELERATION = 0.35f;
        private const float FALL_MAX_VELOCITY = 9f;
        private const float FALL_VELOCITY_DECELERATION = 0.35f;
        private const float DEPTH = 0f;

        private const float HIT_BOX_WIDTH = 16f;
        private const float HIT_BOX_HEIGHT = 24f;
        private const float DISTANCE_BOX_WIDTH = 16f;
        private const float DISTANCE_BOX_HEIGHT = 16f;

        #endregion

        #region Fields

        private float jumpCurrentVelocity;
        private float fallCurrentVelocity;
        private bool jumpButtonPressed;
        private bool jumpKeyPressed;
        private bool attackButtonPressed;
        private bool attackKeyPressed;
        private bool isFalling;
        private Vector2 position;
        private BoundingBox hitBox;
        private BoundingBox previousFrameHitBox;
        private BoundingBox distanceBox;

        #endregion

        #region Properties

        public bool IsControllable { get; set; }
        public bool IsRunning { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDucking { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsFalling
        {
            get
            {
                return isFalling;
            }
            set
            {
                fallCurrentVelocity = 0f;
                isFalling = value;
            }
        }
        public bool WasOnPlatform { get; set; }
        public Direction Direction { get; set; }
        public BoundingBox HitBox
        {
            get
            {
                return hitBox;
            }
            set
            {
                hitBox = value;
            }
        }
        public BoundingBox PreviousFrameHitBox
        {
            get
            {
                return previousFrameHitBox;
            }
            set
            {
                previousFrameHitBox = value;
            }
        }
        public BoundingBox NextFrameHitBox
        {
            get
            {
                return new BoundingBox(
                    hitBox.Min + new Vector3(Velocity.X, Velocity.Y, 0),
                    hitBox.Max + new Vector3(Velocity.X, Velocity.Y, 0)
                );
            }
        }
        public BoundingBox DistanceBox
        {
            get
            {
                return distanceBox;
            }
            set
            {
                distanceBox = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                hitBox = new BoundingBox()
                {
                    Min = new Vector3(position.X, position.Y, 0),
                    Max = new Vector3(position.X + (hitBox.Max.X - hitBox.Min.X), position.Y + (hitBox.Max.Y - hitBox.Min.Y), 0)
                };
            }
        }

        public Vector2 HitBoxSize
        {
            get
            {
                return new Vector2(
                    hitBox.Max.X - hitBox.Min.X,
                    hitBox.Max.Y - hitBox.Min.Y
                );
            }
        }

        public float Left
        {
            get
            {
                return hitBox.Min.X;
            }
        }
        public float Right
        {
            get
            {
                return hitBox.Max.X;
            }
        }
        public float Top
        {
            get
            {
                return hitBox.Min.Y;
            }
        }
        public float Bottom
        {
            get
            {
                return hitBox.Max.Y;
            }
        }
        public Vector2 Velocity
        {
            get
            {
                // Previous frame hit box
                Vector2 previousFrameHitBoxPosition = new Vector2(
                    previousFrameHitBox.Min.X,
                    previousFrameHitBox.Min.Y
                );
                // Current frame hit box
                Vector2 currentFrameHitBoxPosition = new Vector2(
                    hitBox.Min.X,
                    hitBox.Min.Y
                );
                return currentFrameHitBoxPosition - previousFrameHitBoxPosition;
            }
        }

        #endregion

        #region Components

        private readonly Sprite sprite;
        private readonly Box debugHitBox;
        private readonly Box debugDistanceBox;
        private readonly Box debugPreviousFrameHitBox;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        public Player(Map map, Vector2 position)
        {
            // Fields
            jumpCurrentVelocity = JUMP_VELOCITY;
            Direction = Direction.Right;
            this.position = position;
            hitBox = previousFrameHitBox = new BoundingBox(
                new Vector3(position.X, position.Y, DEPTH),
                new Vector3(position.X + HIT_BOX_WIDTH, position.Y + HIT_BOX_HEIGHT, DEPTH)
            );
            distanceBox = new BoundingBox(
                new Vector3(position.X, position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT), DEPTH),
                new Vector3(position.X + DISTANCE_BOX_WIDTH, position.Y + DISTANCE_BOX_HEIGHT, DEPTH)
            );

            // Properties
            IsFalling = true;
            IsControllable = true;

            // Components
            sprite = new Sprite(map.Game, "gameplay/player");
            sprite.OnAnimationFinished += SpriteOnAnimationFinished;
            debugHitBox = new Box(map.Game, new Rectangle(Position.ToPoint(), HitBoxSize.ToPoint()), 1, Color.Cyan);
            debugPreviousFrameHitBox = new Box(map.Game, new Rectangle(Position.ToPoint(), new Point(16, 24)), 2, Color.DarkOliveGreen);
            debugDistanceBox = new Box(map.Game, new Rectangle(Position.ToPoint(), new Point(16, 16)), 3, Color.DodgerBlue);
        }

        

        /// <summary>
        /// Performs player calculations
        /// </summary>
        public void Update()
        {
            #region Previous HitBoxes

            previousFrameHitBox = hitBox;

            if (IsDucking || IsAttacking)
            {
                distanceBox = hitBox;
            }
            else
            {
                distanceBox.Min.X = Position.X;
                distanceBox.Min.Y = Position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT);
                distanceBox.Max.X = Position.X + DISTANCE_BOX_WIDTH;
                distanceBox.Max.Y = Position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT) + DISTANCE_BOX_HEIGHT;
            }

            IsRunning = false;
            IsDucking = false;

            #endregion

            #region Run

            if (IsControllable)
            {
                if (!IsDucking && !IsAttacking)
                {
                    // Gamepad
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                    {
                        position.X -= RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Left;
                    }

                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                    {
                        position.X += RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Right;
                    }

                    // Keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        position.X -= RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Left;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        position.X += RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Right;
                    }
                }
            }

            #endregion

            #region Jumps

            if (IsControllable)
            {
                if (!IsJumping && !IsFalling && !IsDucking && !IsAttacking)
                {
                    if (!jumpButtonPressed && !jumpKeyPressed)
                    {
                        if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) ||
                            Keyboard.GetState().IsKeyDown(Keys.C))
                        {
                            jumpCurrentVelocity = JUMP_VELOCITY;
                            jumpButtonPressed = true;
                            jumpKeyPressed = true;
                            IsJumping = true;
                        }
                    }
                    if (GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.A))
                    {
                        jumpButtonPressed = false;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.C))
                    {
                        jumpKeyPressed = false;
                    }
                }
                if (IsJumping)
                {
                    position.Y -= jumpCurrentVelocity;
                    jumpCurrentVelocity -= JUMP_VELOCITY_DECELERATION;

                    if (jumpCurrentVelocity <= 0)
                    {
                        IsJumping = false;
                        IsFalling = true;
                        fallCurrentVelocity = 0f;
                    }
                }
                if (IsFalling)
                {
                    position.Y += fallCurrentVelocity;
                    if (fallCurrentVelocity <= FALL_MAX_VELOCITY)
                    {
                        fallCurrentVelocity += FALL_VELOCITY_DECELERATION;
                    }
                }
            }

            #endregion

            #region Ducking

            if (IsControllable)
            {
                if (!IsAttacking && !IsJumping && !IsFalling)
                {
                    // Gamepad
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                    {
                        IsDucking = true;
                    }

                    // Keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        IsDucking = true;
                    }
                }
            }

            #endregion

            #region Tail attack

            if (IsControllable)
            {
                if (!IsJumping && !IsFalling && IsDucking)
                {
                    if (!attackButtonPressed && !attackKeyPressed)
                    {
                        if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X) ||
                                Keyboard.GetState().IsKeyDown(Keys.X))
                        {
                            attackButtonPressed = true;
                            attackKeyPressed = true;
                            IsAttacking = true;
                        }
                    }
                }
            }

            #endregion

            #region Animations

            if (IsRunning)
            {
                sprite.AnimationName = "run";
            }

            if (IsJumping)
            {
                sprite.AnimationName = "jump";
            }
            else if (IsFalling)
            {
                sprite.AnimationName = "fall";
            }
            else if (IsAttacking)
            {
                sprite.AnimationName = "tail";
            }
            else if (IsDucking)
            {
                sprite.AnimationName = "duck";
            }

            if (!IsRunning && !IsJumping && !IsFalling && !IsDucking && !IsAttacking)
            {
                sprite.AnimationName = "idle";
            }

            #endregion

            UpdateHitBoxes();
        }

        /// <summary>
        /// Update hitbox position from player position
        /// </summary>
        public void UpdateHitBoxes()
        {
            hitBox.Min.X = Position.X;
            hitBox.Min.Y = (IsDucking || IsAttacking) ? Position.Y + 8 : Position.Y;
            hitBox.Max.X = Position.X + HIT_BOX_WIDTH;
            hitBox.Max.Y = Position.Y + HIT_BOX_HEIGHT;
        }

        /// <summary>
        /// Draw player sprite
        /// </summary>
        public void Draw()
        {
            Vector2 drawingPosition = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

            sprite.Draw(position: drawingPosition - sprite.HitBoxOffset, layer: 0f);
        }

        /// <summary>
        /// Draw debug info of the player sprite
        /// </summary>
        public void DebugDraw()
        {
            debugHitBox.Bounds = new Rectangle(new Point((int)hitBox.Min.X, (int)hitBox.Min.Y), HitBoxSize.ToPoint());
            debugHitBox.Update();

            debugDistanceBox.Bounds = new Rectangle(
                new Point((int)distanceBox.Min.X, (int)distanceBox.Min.Y),
                new Point((int)(distanceBox.Max.X - distanceBox.Min.X), (int)(distanceBox.Max.Y - distanceBox.Min.Y))
            );
            debugDistanceBox.Update();

            debugPreviousFrameHitBox.Bounds = new Rectangle(
                new Point((int)previousFrameHitBox.Min.X, (int)previousFrameHitBox.Min.Y),
                new Point((int)(previousFrameHitBox.Max.X - previousFrameHitBox.Min.X), (int)(previousFrameHitBox.Max.Y - previousFrameHitBox.Min.Y))
            );
            debugPreviousFrameHitBox.Update();

            debugDistanceBox.Draw();
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

        private void SpriteOnAnimationFinished(Sprite sender)
        {
            if (sender.AnimationName == "tail")
            {
                sprite.AnimationName = "idle";
                IsAttacking = false;
                attackButtonPressed = false;
                attackKeyPressed = false;
            }
        }
    }
}
