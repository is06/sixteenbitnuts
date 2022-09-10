using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class PlatformerPlayer : Player
    {
        public float Weight = 1f;
        public float JumpForce = -8f;

        public bool IsJumping { get; private set; }
        public bool IsFalling { get; private set; }
        public bool IsTouchingTheGround { get; private set; }
        public bool IsTouchingTheCeiling { get; private set; }

        protected VirtualButton jumpButton;

        private bool isJumpButtonPressed = false;
        private bool wasOnTheGround = false;

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

        protected override void UpdateMoveDirection()
        {
            if (IsControllable)
            {
                if (runStick is VirtualStick stick)
                {
                    MoveDirection = DirectionHelper.FromNormalizedHorizontal((int)stick.Value.X);
                    if (MoveDirection != Direction.None)
                    {
                        LookDirection = MoveDirection;
                    }
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
            if (MoveDirection == Direction.Left)
            {
                Velocity.X = -RunSpeed;
                
            }
            else if (MoveDirection == Direction.Right)
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

            // Running
            if (MoveDirection == Direction.Left || MoveDirection == Direction.Right)
            {
                IsRunning = true;
            }
            else
            {
                IsRunning = false;
            }

            // Jump & Fall
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

        protected override void UpdateSprite()
        {
            base.UpdateSprite();

            if (sprite is Sprite && LookDirection != Direction.None)
            {
                sprite.Direction = LookDirection;
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
