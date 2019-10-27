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

        private const float ATTACK_BOX_HEIGHT = 14f;
        private const float ATTACK_BOX_OFFSET = 2f;
        private const float ATTACK_BOX_DISTANCE = 20f;
        private const float ATTACK_START_DELAY = 4f;

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

        // Attack
        private float attackDelay;
        private float attackPositionDelta;
        private Direction attackDirection;

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
        public BoundingBox HitBox { get; set; }
        public BoundingBox PreviousFrameHitBox { get; set; }
        public BoundingBox DistanceBox { get; set; }
        public BoundingBox AttackBox { get; set; }
        public BoundingBox NextFrameHitBox
        {
            get
            {
                return new BoundingBox(
                    HitBox.Min + new Vector3(Velocity.X, Velocity.Y, 0),
                    HitBox.Max + new Vector3(Velocity.X, Velocity.Y, 0)
                );
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
                HitBox = new BoundingBox()
                {
                    Min = new Vector3(position.X, position.Y, 0),
                    Max = new Vector3(position.X + (HitBox.Max.X - HitBox.Min.X), position.Y + (HitBox.Max.Y - HitBox.Min.Y), 0)
                };
            }
        }
        public float Left
        {
            get
            {
                return HitBox.Min.X;
            }
        }
        public float Right
        {
            get
            {
                return HitBox.Max.X;
            }
        }
        public float Top
        {
            get
            {
                return HitBox.Min.Y;
            }
        }
        public float Bottom
        {
            get
            {
                return HitBox.Max.Y;
            }
        }
        public Vector2 Velocity
        {
            get
            {
                // Previous frame hit box
                Vector2 previousFrameHitBoxPosition = new Vector2(
                    PreviousFrameHitBox.Min.X,
                    PreviousFrameHitBox.Min.Y
                );
                // Current frame hit box
                Vector2 currentFrameHitBoxPosition = new Vector2(
                    HitBox.Min.X,
                    HitBox.Min.Y
                );
                return currentFrameHitBoxPosition - previousFrameHitBoxPosition;
            }
        }

        #endregion

        #region Components

        private readonly Sprite sprite;
        private readonly DebugHitBox debugHitBox;
        private readonly DebugHitBox debugDistanceBox;
        private readonly DebugHitBox debugPreviousFrameHitBox;
        private readonly DebugHitBox debugAttackBox;

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
            HitBox = PreviousFrameHitBox = new BoundingBox(
                new Vector3(position.X, position.Y, DEPTH),
                new Vector3(position.X + HIT_BOX_WIDTH, position.Y + HIT_BOX_HEIGHT, DEPTH)
            );
            DistanceBox = new BoundingBox(
                new Vector3(position.X, position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT), DEPTH),
                new Vector3(position.X + DISTANCE_BOX_WIDTH, position.Y + DISTANCE_BOX_HEIGHT, DEPTH)
            );

            // Properties
            IsFalling = true;
            IsControllable = true;

            // Components
            sprite = new Sprite(map.Game, "gameplay/player");
            sprite.OnAnimationFinished += SpriteOnAnimationFinished;
            debugHitBox = new DebugHitBox(map.Game, 1, Color.Cyan);
            debugPreviousFrameHitBox = new DebugHitBox(map.Game, 2, Color.DarkOliveGreen);
            debugDistanceBox = new DebugHitBox(map.Game, 3, Color.DodgerBlue);
            debugAttackBox = new DebugHitBox(map.Game, 1, Color.Red);
        }

        /// <summary>
        /// Performs player calculations
        /// </summary>
        public void Update(GameTime gameTime)
        {
            #region Previous HitBoxes

            PreviousFrameHitBox = HitBox;

            if (IsDucking || IsAttacking)
            {
                DistanceBox = HitBox;
            }
            else
            {
                DistanceBox = new BoundingBox
                {
                    Min = new Vector3(Position.X, Position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT), 0),
                    Max = new Vector3(Position.X + DISTANCE_BOX_WIDTH, Position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT) + DISTANCE_BOX_HEIGHT, 0)
                };
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
                            attackDelay = 0f;
                            attackPositionDelta = 0f;
                            attackDirection = Direction;
                        }
                    }
                }

                debugAttackBox.Color = Color.Yellow;

                if (IsAttacking)
                {
                    debugAttackBox.Color = Color.Red;

                    if (attackDirection == Direction.Right)
                    {
                        if (attackDelay >= ATTACK_START_DELAY)
                            attackPositionDelta += 4f;
                        if (attackPositionDelta > ATTACK_BOX_DISTANCE)
                            attackDirection = Direction.Left;
                        if (Direction == Direction.Left && attackPositionDelta >= 0)
                            attackDirection = Direction.None;
                    }
                    if (attackDirection == Direction.Left)
                    {
                        if (attackDelay >= ATTACK_START_DELAY)
                            attackPositionDelta -= 4f;
                        if (attackPositionDelta <= -ATTACK_BOX_DISTANCE)
                            attackDirection = Direction.Right;
                        if (Direction == Direction.Right && attackPositionDelta <= 0)
                            attackDirection = Direction.None;
                    }

                    if (attackDirection == Direction.Right)
                    {
                        if (attackPositionDelta > 0)
                            AttackBox = new BoundingBox
                            {
                                Min = new Vector3(Position.X, Position.Y + ATTACK_BOX_OFFSET, 0),
                                Max = new Vector3(Position.X + HIT_BOX_WIDTH + attackPositionDelta, Position.Y + ATTACK_BOX_HEIGHT, 0)
                            };
                        else
                            AttackBox = new BoundingBox
                            {
                                Min = new Vector3(Position.X + attackPositionDelta, Position.Y + ATTACK_BOX_OFFSET, 0),
                                Max = new Vector3(Position.X + HIT_BOX_WIDTH, Position.Y + ATTACK_BOX_HEIGHT, 0)
                            };
                    }
                    else
                    {
                        if (attackPositionDelta < 0)
                            AttackBox = new BoundingBox
                            {
                                Min = new Vector3(Position.X + attackPositionDelta, Position.Y + ATTACK_BOX_OFFSET, 0),
                                Max = new Vector3(Position.X + HIT_BOX_WIDTH, Position.Y + ATTACK_BOX_HEIGHT, 0)
                            };
                        else
                            AttackBox = new BoundingBox
                            {
                                Min = new Vector3(Position.X, Position.Y + ATTACK_BOX_OFFSET, 0),
                                Max = new Vector3(Position.X + HIT_BOX_WIDTH + attackPositionDelta, Position.Y + ATTACK_BOX_HEIGHT, 0)
                            };
                    }

                    attackDelay++;
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
            HitBox = new BoundingBox
            {
                Min = new Vector3(Position.X, (IsDucking || IsAttacking) ? Position.Y + 8 : Position.Y, 0),
                Max = new Vector3(Position.X + HIT_BOX_WIDTH, Position.Y + HIT_BOX_HEIGHT, 0)
            };
        }

        public void UpdateDebugHitBoxes()
        {
            debugAttackBox.Update(AttackBox);
            debugDistanceBox.Update(DistanceBox);
            debugHitBox.Update(HitBox);
            debugPreviousFrameHitBox.Update(PreviousFrameHitBox);
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
            debugDistanceBox.Draw();
            debugPreviousFrameHitBox.Draw();
            debugHitBox.Draw();
            debugAttackBox.Draw();
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
