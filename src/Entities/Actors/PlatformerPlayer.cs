using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class PlatformerPlayer : Player
    {
        public float RunSpeed { get; protected set; }
        public float JumpForce { get; protected set; }

        private bool jumpButtonPressed;

        public PlatformerPlayer(Map map) : base(map)
        {
            Position = map.CurrentMapSection.DefaultSpawnPoint.Position;
            Weight = 1f;
            RunSpeed = 1f;
            JumpForce = -6f;
        }

        public override void Update(GameTime _)
        {
            base.Update(_);

            #region Ducking

            if (IsControllable)
            {
                IsDucking = false;

                if (!IsBouncing && !IsAttacking && !IsJumping && !IsFalling)
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
    }
}
