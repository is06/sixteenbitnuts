using Microsoft.Xna.Framework;
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

        private const float HIT_BOX_WIDTH = 16f;
        private const float HIT_BOX_HEIGHT = 24f;
        private const float DISTANCE_BOX_WIDTH = 16f;
        private const float DISTANCE_BOX_HEIGHT = 16f;

        private const float ATTACK_BOX_HEIGHT = 12f;
        private const float ATTACK_BOX_OFFSET = 2f;
        private const float ATTACK_BOX_DISTANCE = 20f;
        private const float ATTACK_START_DELAY = 4f;

        private const float DASH_SPEED = 6f;
        private const float DASH_DELAY = 0.1f;
        private const float DASH_BOUNCE_VELOCITY = 4f;

        #endregion

        #region Fields

        private float jumpCurrentVelocity;
        private float fallCurrentVelocity;
        private bool isFalling;
        private bool jumpButtonPressed;
        private bool jumpKeyPressed;

        private bool attackButtonPressed;
        private bool attackKeyPressed;

        private bool dashButtonPressed;
        private bool dashKeyPressed;

        private Vector2 position;

        // Attack
        private float attackDelay;
        private float attackPositionDelta;
        private Direction attackDirection;

        // Dash
        private readonly Timer dashDelayTimer;

        #endregion

        #region Properties

        public bool IsControllable { get; set; }
        public bool IsRunning { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDucking { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsDashing { get; set; }
        public bool IsDashFalling { get; set; }
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
        public HitBox HitBox { get; set; }
        public HitBox PreviousFrameHitBox { get; set; }
        public HitBox DistanceBox { get; set; }
        public HitBox AttackBox { get; set; }
        public HitBox NextFrameHitBox
        {
            get
            {
                return new HitBox(
                    HitBox.Position + Velocity,
                    HitBox.Size
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
        public float Left
        {
            get
            {
                return HitBox.Left;
            }
        }
        public float Right
        {
            get
            {
                return HitBox.Right;
            }
        }
        public float Top
        {
            get
            {
                return HitBox.Top;
            }
        }
        public float Bottom
        {
            get
            {
                return HitBox.Bottom;
            }
        }
        public Vector2 Velocity
        {
            get
            {
                return HitBox.Position - PreviousFrameHitBox.Position;
            }
        }

        #endregion

        #region Components

        protected Sprite sprite;
        protected DebugHitBox debugHitBox;
        protected DebugHitBox debugDistanceBox;
        protected DebugHitBox debugPreviousFrameHitBox;
        protected DebugHitBox debugAttackBox;

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

            // Hitboxes
            HitBox = PreviousFrameHitBox = new HitBox(position, new Vector2(HIT_BOX_WIDTH, HIT_BOX_HEIGHT));
            DistanceBox = new HitBox(
                new Vector2(position.X, position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT)),
                new Vector2(DISTANCE_BOX_WIDTH, DISTANCE_BOX_HEIGHT)
            );

            // Properties
            IsFalling = true;
            IsControllable = true;

            // Sprites
            InitSprites(map);

            // Dashing
            dashDelayTimer = new Timer { Duration = DASH_DELAY };
            dashDelayTimer.OnTimerFinished += DashDelayTimer_OnTimerFinished;

            // Debug
            InitDebugBoxes(map);
        }

        protected virtual void InitSprites(Map map)
        {
            sprite = new Sprite(map.Game, "gameplay/player");
            sprite.OnAnimationFinished += Sprite_OnAnimationFinished;
        }

        protected virtual void InitDebugBoxes(Map map)
        {
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
                if (!IsJumping && !IsFalling && !IsDucking && !IsAttacking && !IsDashing)
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
                    if (GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.X))
                    {
                        attackButtonPressed = false;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.X))
                    {
                        attackKeyPressed = false;
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
                            AttackBox = new HitBox(
                                new Vector2(Position.X, Position.Y + ATTACK_BOX_OFFSET),
                                new Vector2(HIT_BOX_WIDTH + attackPositionDelta, ATTACK_BOX_HEIGHT)
                            );
                        else
                            AttackBox = new HitBox(
                                new Vector2(Position.X + attackPositionDelta, Position.Y + ATTACK_BOX_OFFSET),
                                new Vector2(HIT_BOX_WIDTH + attackPositionDelta, ATTACK_BOX_HEIGHT)
                            );
                    }
                    else
                    {
                        if (attackPositionDelta < 0)
                            AttackBox = new HitBox(
                                new Vector2(Position.X + attackPositionDelta, Position.Y + ATTACK_BOX_OFFSET),
                                new Vector2(HIT_BOX_WIDTH + attackPositionDelta, ATTACK_BOX_HEIGHT)
                            );
                        else
                            AttackBox = new HitBox(
                                new Vector2(Position.X, Position.Y + ATTACK_BOX_OFFSET),
                                new Vector2(HIT_BOX_WIDTH + attackPositionDelta, ATTACK_BOX_HEIGHT)
                            );
                    }

                    attackDelay++;
                }
            }

            #endregion

            #region Down punch (dashing)

            if (IsControllable)
            {
                if (!IsDashing && !IsDashFalling && (IsJumping || IsFalling))
                {
                    if (!dashKeyPressed && !dashButtonPressed)
                    {
                        if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown) ||
                            GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown) ||
                            Keyboard.GetState().IsKeyDown(Keys.Down))
                        {
                            IsDashing = true;
                            IsJumping = false;
                            IsFalling = false;
                            IsControllable = false;
                            Direction = Direction.Bottom;

                            dashButtonPressed = true;
                            dashKeyPressed = true;
                            dashDelayTimer.Active = true;
                        }
                    }
                    if (GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.LeftThumbstickDown) &&
                        GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.DPadDown))
                    {
                        dashButtonPressed = false;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down))
                    {
                        dashKeyPressed = false;
                    }
                }
            }
            if (IsDashFalling)
            {
                position.Y += DASH_SPEED;
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
            else if (IsDashing)
            {
                sprite.AnimationName = "dash";
            }
            else if (IsDucking)
            {
                sprite.AnimationName = "duck";
            }

            if (!IsRunning && !IsJumping && !IsFalling && !IsDucking && !IsAttacking && !IsDashing)
            {
                sprite.AnimationName = "idle";
            }

            #endregion

            dashDelayTimer.Update(gameTime);

            UpdateHitBoxes();
        }

        /// <summary>
        /// Update hitbox position from player position
        /// </summary>
        public void UpdateHitBoxes()
        {
            HitBox = new HitBox(
                new Vector2(Position.X, (IsDucking || IsAttacking) ? Position.Y + 8 : Position.Y),
                new Vector2(HIT_BOX_WIDTH, (IsDucking || IsAttacking) ? HIT_BOX_HEIGHT - 8 : HIT_BOX_HEIGHT)
            );

            DistanceBox = new HitBox(
                new Vector2(Position.X, (IsDucking || IsAttacking) ? Position.Y : Position.Y + (HIT_BOX_HEIGHT - DISTANCE_BOX_HEIGHT)),
                new Vector2(DISTANCE_BOX_WIDTH, DISTANCE_BOX_HEIGHT)
            );
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
            sprite.Draw(position: DrawingPosition - sprite.HitBoxOffset, layer: 0f);
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

        public void DashBounce()
        {
            jumpCurrentVelocity = DASH_BOUNCE_VELOCITY;
            IsJumping = true;
        }

        private void Sprite_OnAnimationFinished(Sprite sender)
        {
            if (sender.AnimationName == "tail")
            {
                sprite.AnimationName = "idle";
                IsAttacking = false;
                attackKeyPressed = false;
            }
        }

        private void DashDelayTimer_OnTimerFinished()
        {
            IsDashFalling = true;
        }
    }
}
