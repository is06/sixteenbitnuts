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

        private static Vector2 GRAVITY = new Vector2(0, 0.4f);

        private const float RUN_SPEED = 1.75f;

        private const float HIT_BOX_WIDTH = 16f;
        private const float HIT_BOX_HEIGHT = 24f;

        private const float JUMP_FORCE = -8f;

        //private const float ATTACK_BOX_HEIGHT = 12f;
        //private const float ATTACK_BOX_OFFSET = 2f;
        //private const float ATTACK_BOX_DISTANCE = 20f;
        //private const float ATTACK_START_DELAY = 4f;

        //private const float PUNCH_BOUNCE_VELOCITY = 7f;

        #endregion

        #region Fields

        private bool jumpButtonPressed;

        //private bool attackButtonPressed;
        //private bool attackKeyPressed;

        private Vector2 position;
        private Vector2 velocity;

        // Attack
        //private float attackDelay;
        //private float attackPositionDelta;
        //private Direction attackDirection;

        #endregion

        #region Properties

        public bool IsControllable { get; set; }
        public bool IsRunning { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDucking { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsPunching { get; set; }
        public bool IsDashing { get; set; }
        public bool IsDashFalling { get; set; }
        public bool IsFalling { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsTouchingTheCeiling { get; set; }
        public bool WasOnPlatform { get; set; }
        public Direction Direction { get; set; }
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
            Direction = Direction.Right;
            this.position = position;

            // Hitboxes
            HitBox = PreviousFrameHitBox = new HitBox(position, new Vector2(HIT_BOX_WIDTH, HIT_BOX_HEIGHT));

            // Properties
            IsFalling = true;
            IsControllable = true;

            // Sprites
            InitSprites(map);

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
        public void Update(GameTime _)
        {
            // Memorize the previous frame hit box
            PreviousFrameHitBox = HitBox;

            #region Ducking

            if (IsControllable)
            {
                IsDucking = false;

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

            #region Run

            if (IsControllable)
            {
                IsRunning = false;
                velocity.X = 0;

                if (!IsDucking && !IsAttacking)
                {
                    // Gamepad
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                    {
                        velocity.X = -RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Left;
                    }

                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                    {
                        velocity.X = RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Right;
                    }

                    // Keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        velocity.X = -RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Left;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        velocity.X = RUN_SPEED;
                        IsRunning = true;
                        Direction = sprite.Direction = Direction.Right;
                    }
                }
            }

            #endregion

            #region Jumps

            if (IsControllable)
            {
                if (!IsDucking && IsGrounded && !jumpButtonPressed && Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    velocity.Y = JUMP_FORCE;
                    IsGrounded = false;
                    jumpButtonPressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.C))
                {
                    // If the player is moving up
                    if (velocity.Y < 0)
                    {
                        velocity.Y *= 0.5f;
                    }
                    jumpButtonPressed = false;
                }

                if (velocity.Y > 0)
                {
                    IsJumping = false;
                    IsFalling = true;
                }
                else if(velocity.Y < 0)
                {
                    IsJumping = true;
                    IsFalling = false;
                }
            }

            #endregion

            #region Tail attack

            /*
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
            */

            #endregion

            #region Punching

            /*
            if (IsControllable)
            {
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown) ||
                    GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                {
                    IsPunching = true;
                }

                // Keyboard
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    IsPunching = true;
                }
            }
            */

            #endregion

            #region Animations

            if (IsRunning)
                sprite.AnimationName = "run";
            if (IsPunching)
                sprite.AnimationName = "dash";

            if (IsJumping)
                sprite.AnimationName = "jump";
            else if (IsFalling)
                sprite.AnimationName = "fall";
            else if (IsAttacking)
                sprite.AnimationName = "tail";
            else if (IsDucking)
                sprite.AnimationName = "duck";

            if (!IsRunning && !IsJumping && !IsFalling && !IsDucking && !IsAttacking && !IsPunching)
                sprite.AnimationName = "idle";

            #endregion

            #region Physics calculations

            velocity += GRAVITY;

            if (IsGrounded)
                velocity.Y = 0;

            if (IsTouchingTheCeiling)
            {
                IsTouchingTheCeiling = false;
                velocity.Y *= -0.5f;
            }
            
            position += velocity;

            #endregion

            HitBox = new HitBox(
                new Vector2(position.X, (IsDucking || IsAttacking) ? position.Y + 8 : position.Y),
                new Vector2(HIT_BOX_WIDTH, (IsDucking || IsAttacking) ? HIT_BOX_HEIGHT - 8 : HIT_BOX_HEIGHT)
            );
        }

        public void UpdateDebugHitBoxes()
        {
            debugAttackBox.Update(AttackBox);
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

        public void PunchBounce()
        {
            //jumpCurrentVelocity = PUNCH_BOUNCE_VELOCITY;
            IsJumping = true;
        }

        private void Sprite_OnAnimationFinished(Sprite sender)
        {
            if (sender.AnimationName == "tail")
            {
                sprite.AnimationName = "idle";
                IsAttacking = false;
                //attackKeyPressed = false;
            }
        }
    }
}
