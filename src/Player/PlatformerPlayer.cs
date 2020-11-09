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
        public bool IsRunning { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDucking { get; set; }
        public bool IsFalling { get; set; }
        public bool IsTouchingTheGround { get; set; }
        public bool IsTouchingTheCeiling { get; set; }
        public bool WasOnPlatform { get; set; }
        public float Weight { get; protected set; }
        public float RunSpeed { get; protected set; }
        public float JumpForce { get; protected set; }
        public float DuckOffset { get; protected set; }

        public event PlatformerPlayerActionHandler? OnPerformAction;

        private bool jumpButtonPressed;

        public PlatformerPlayer(Map map) : base(map)
        {
            Weight = 1f;
            RunSpeed = 1f;
            JumpForce = -6f;
            DuckOffset = 8f;
            IsFalling = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
                velocity.X = 0;

                if (!IsDucking)
                {
                    // Gamepad
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                    {
                        velocity.X = -RunSpeed;
                        IsRunning = true;
                        Direction = Direction.Left;
                        if (sprite != null) sprite.Direction = Direction.Left;
                    }

                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight) ||
                        GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                    {
                        velocity.X = RunSpeed;
                        IsRunning = true;
                        Direction = Direction.Right;
                        if (sprite != null) sprite.Direction = Direction.Right;
                    }

                    // Keyboard
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        velocity.X = -RunSpeed;
                        IsRunning = true;
                        Direction = Direction.Left;
                        if (sprite != null) sprite.Direction = Direction.Left;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        velocity.X = RunSpeed;
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
                    velocity.Y = JumpForce;
                    IsTouchingTheGround = false;
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
                else if (velocity.Y < 0)
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

        public override void Draw(Matrix transform)
        {
            if (sprite != null)
            {
                var pos = DrawingPosition;

                if (IsDucking)
                {
                    pos.Y -= DuckOffset;
                }

                sprite.Draw(position: pos, layer: 0f, transform: transform);
            }
        }

        public override void ComputePhysics()
        {
            velocity += map.Gravity * Weight;

            if (IsTouchingTheGround)
                velocity.Y = 0;

            if (IsTouchingTheCeiling)
            {
                IsTouchingTheCeiling = false;
                velocity.Y *= -0.5f;
            }

            base.ComputePhysics();
        }

        public override void UpdateHitBox()
        {
            base.UpdateHitBox();

            HitBox = new HitBox(
                new Vector2(position.X, IsDucking ? position.Y + DuckOffset : position.Y),
                new Size(Size.Width, IsDucking ? Size.Height - DuckOffset : Size.Height)
            );
        }
    }
}
