using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public enum PlatformerPlayerActionType
    {
        Jump
    }

    public delegate void PlatformerPlayerActionHandler(PlatformerPlayerActionType type);

    public abstract class PlatformerPlayer : Player
    {
        // Settings (read / write, even for authoring)

        public float Weight;
        public float RunSpeed;
        public float RunAcceleration;
        public float RunDeceleration;
        public float RunMaxAcceleration;
        public float JumpForce;
        public float JumpHorizontalSpeed;
        public float JumpDamping;

        // Debug properties (read only, for authoring)

        public float DebugRunHorizontalVelocityFactor { get { return runHorizontalVelocityFactor; } }

        // Properties

        public bool IsGravityEnabled { get; set; }
        public bool IsRunning { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDucking { get; set; }
        public bool IsFalling { get; set; }
        public bool IsTouchingTheGround { get; set; }
        public bool IsTouchingTheCeiling { get; set; }
        public bool WasOnPlatform { get; set; }
        public float DuckOffset { get; protected set; }
        public bool OverrideAllControls { get; set; }

        // Events

        public event PlatformerPlayerActionHandler? OnPerformAction;

        // Fields

        protected float runHorizontalVelocityFactor;

        private bool jumpButtonPressed;

        public PlatformerPlayer(Map map) : base(map)
        {
            Weight = 1f;
            RunSpeed = 5f;
            RunAcceleration = 0.04f;
            RunMaxAcceleration = 0.4f;
            RunDeceleration = 0.1f;
            JumpForce = -6f;
            DuckOffset = 8f;
            IsFalling = true;
            IsGravityEnabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!OverrideAllControls)
            {
                #region Ducking

                if (IsControllable)
                {
                    IsDucking = false;

                    if (!IsJumping && !IsFalling)
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
                    Velocity.X = 0;

                    if (!IsDucking)
                    {
                        // Gamepad
                        if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft) ||
                            GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                        {
                            Velocity.X = -RunSpeed;
                            IsRunning = true;
                            Direction = Direction.Left;
                            if (sprite != null) sprite.Direction = Direction.Left;
                        }

                        if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight) ||
                            GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                        {
                            Velocity.X = RunSpeed;
                            IsRunning = true;
                            Direction = Direction.Right;
                            if (sprite != null) sprite.Direction = Direction.Right;
                        }

                        // Keyboard
                        if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        {
                            Velocity.X = -RunSpeed;
                            IsRunning = true;
                            Direction = Direction.Left;
                            if (sprite != null) sprite.Direction = Direction.Left;
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        {
                            Velocity.X = RunSpeed;
                            IsRunning = true;
                            Direction = Direction.Right;
                            if (sprite != null) sprite.Direction = Direction.Right;
                        }
                    }
                }

                #endregion

                #region Jumps

                if (IsControllable)
                {
                    if (!IsDucking && IsTouchingTheGround && !jumpButtonPressed && Keyboard.GetState().IsKeyDown(Keys.C))
                    {
                        OnPerformAction?.Invoke(PlatformerPlayerActionType.Jump);
                        Velocity.Y = JumpForce;
                        IsTouchingTheGround = false;
                        jumpButtonPressed = true;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.C))
                    {
                        // If the player is moving up
                        if (Velocity.Y < 0)
                        {
                            Velocity.Y *= 0.5f;
                        }
                        jumpButtonPressed = false;
                    }

                    if (Velocity.Y > 0)
                    {
                        IsJumping = false;
                        IsFalling = true;
                    }
                    else if (Velocity.Y < 0)
                    {
                        IsJumping = true;
                        IsFalling = false;
                    }
                }

                #endregion

                #region Animations

                if (sprite != null)
                {
                    if (IsRunning)
                        sprite.AnimationName = "run";

                    if (IsJumping)
                        sprite.AnimationName = "jump";
                    else if (IsFalling)
                        sprite.AnimationName = "fall";
                    else if (IsDucking)
                        sprite.AnimationName = "duck";

                    if (!IsRunning && !IsJumping && !IsFalling && !IsDucking)
                        sprite.AnimationName = "idle";
                }

                #endregion
            }
        }

        public override void Draw(Matrix transform)
        {
            if (sprite != null)
            {
                sprite.Draw(position: DrawingPosition, layer: 0f, transform: transform);
            }
        }

        public override void ComputePhysics()
        {
            if (IsGravityEnabled)
            {
                Velocity += map.Gravity * Weight;

                if (IsTouchingTheGround)
                    Velocity.Y = 0;

                if (IsTouchingTheCeiling)
                {
                    IsTouchingTheCeiling = false;
                    Velocity.Y *= -0.5f;
                }
            }

            base.ComputePhysics();
        }

        public override void UpdateHitBoxes()
        {
            base.UpdateHitBoxes();

            HitBox = new HitBox(
                new Vector2(position.X, IsDucking ? position.Y + DuckOffset : position.Y),
                new Size(Size.Width, IsDucking ? Size.Height - DuckOffset : Size.Height)
            );
        }
    }
}
