using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class PlatformerPlayer : Player
    {
        public float Weight = 1f;
        public float JumpForce = -8f;

        public bool IsJumping = false;
        public bool IsFalling = false;
        public bool IsTouchingTheGround = false;
        public bool IsTouchingTheCeiling = false;

        private bool isJumpButtonPressed = false;
        private bool wasOnTheGround = false;

        protected VirtualButton jumpButton;

        public PlatformerPlayer(Map map, Point hitBoxSize) : base(map, hitBoxSize)
        {
            OnCollideVertically += PlatformerPlayer_OnCollideVertically;
            OnBeforeApplyMoveAndDetectCollisions += PlatformerPlayer_ApplyGravity;
            OnAfterApplyMoveAndDetectCollisions += PlatformerPlayer_UpdatePlatformRideState;

            runStick = new VirtualStick(map.Game)
                .AddKeys(Keys.Left, Keys.Right, Keys.Up, Keys.Down)
                .AddButtons(PlayerIndex.One, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp, Buttons.DPadDown);
            jumpButton = new VirtualButton()
                .AddKey(Keys.C)
                .AddButton(PlayerIndex.One, Buttons.A);
        }

        protected override void UpdateDirection()
        {
            if (IsControllable)
            {
                if (runStick is VirtualStick stick)
                {
                    Direction = DirectionHelper.FromNormalizedHorizontal((int)stick.Value.X);
                }
            }
        }

        /// <summary>
        /// Updates the player velocity during different stages like running or jumping
        /// </summary>
        protected override void UpdateVelocity()
        {
            if (IsControllable)
            {
                UpdateRunVelocity();
                UpdateJumpVelocity();
            }
        }

        private void UpdateRunVelocity()
        {
            if (Direction == Direction.Left)
            {
                Velocity.X = -RunSpeed;
            }
            else if (Direction == Direction.Right)
            {
                Velocity.X = RunSpeed;
            }
            else
            {
                Velocity.X = 0;
            }
        }

        private void UpdateJumpVelocity()
        {
            jumpButton.Update();

            if (!isJumpButtonPressed && IsTouchingTheGround && jumpButton.IsPressed())
            {
                Velocity.Y = JumpForce;
                IsTouchingTheGround = false;
                isJumpButtonPressed = true;
            }
            else if (isJumpButtonPressed && jumpButton.IsReleased())
            {
                if (Velocity.Y < 0)
                {
                    Velocity.Y *= 0.5f;
                }
                isJumpButtonPressed = false;
            }
        }

        protected override void UpdateStates()
        {
            base.UpdateStates();

            if (Velocity.Y > 0)
            {
                IsFalling = true;
                IsJumping = false;
            }
            else if (Velocity.Y < 0)
            {
                IsJumping = true;
                IsFalling = false;
            }
        }

        /// <summary>
        /// Before ApplyMoveAndDetectCollisions
        /// Sets the vertical velocity to apply gravity to the player
        /// </summary>
        private void PlatformerPlayer_ApplyGravity()
        {
            if (IsTouchingTheGround)
            {
                Velocity.Y = 0;
            }

            if (IsTouchingTheCeiling)
            {
                IsTouchingTheCeiling = false;
                Velocity.Y *= -0.5f;
            }

            Velocity += ((PlatformerMap)map).Gravity * Weight;
        }

        /// <summary>
        /// Sets the state of touching the ground or ceiling when a vertical collision with a solid occurs
        /// </summary>
        /// <param name="side">Side of the vertical collision</param>
        private void PlatformerPlayer_OnCollideVertically(CollisionSide side)
        {
            // Hit the ground
            IsTouchingTheGround = false;
            if (side == CollisionSide.Top)
            {
                IsTouchingTheGround = true;
                if (IsFalling)
                {
                    IsFalling = false;
                    wasOnTheGround = true;
                }
            }

            // Hit the ceiling
            IsTouchingTheCeiling = false;
            if (side == CollisionSide.Bottom)
            {
                IsTouchingTheCeiling = true;
            }
        }

        /// <summary>
        /// After ApplyMoveAndDetectCollisions
        /// Sets the state of leaving a platform and fall from it by running
        /// </summary>
        private void PlatformerPlayer_UpdatePlatformRideState()
        {
            if (!IsIntersectingWithObstacle && wasOnTheGround && !IsJumping)
            {
                wasOnTheGround = false;
                IsFalling = true;
                IsTouchingTheGround = false;
            }
        }
    }
}
