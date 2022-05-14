using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class PlatformerPlayer : Player
    {
        public float Weight = 1f;
        public float JumpForce = -8f;

        public bool IsTouchingTheGround = false;
        public bool IsTouchingTheCeiling = false;

        protected VirtualButton jump;

        public PlatformerPlayer(Map map, Point hitBoxSize) : base(map, hitBoxSize)
        {
            OnCollideVertically += PlatformerPlayer_OnCollideVertically;

            run = new VirtualStick(map.Game)
                .AddKeys(Keys.Left, Keys.Right, Keys.Up, Keys.Down)
                .AddButtons(PlayerIndex.One, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp, Buttons.DPadDown);
            jump = new VirtualButton()
                .AddKey(Keys.C)
                .AddButton(PlayerIndex.One, Buttons.A);
        }

        public override void Update()
        {
            base.Update();

            if (IsControllable)
            {
                jump.Update();

                if (jump.IsPressed())
                {
                    Velocity.Y = JumpForce;
                    IsTouchingTheGround = false;
                }
                if (jump.IsReleased())
                {
                    if (Velocity.Y < 0)
                    {
                        Velocity.Y *= 0.5f;
                    }
                }
            }
            
        }

        protected override void UpdateDirection()
        {
            if (IsControllable)
            {
                if (run is VirtualStick stick)
                {
                    Direction = DirectionHelper.FromNormalizedHorizontal((int)stick.Value.X);
                }
            }
        }

        protected override void UpdateVelocity()
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

        protected override void BeforeApplyMove()
        {
            base.BeforeApplyMove();

            ApplyGravity();
        }

        protected virtual void ApplyGravity()
        {
            if (IsTouchingTheGround)
            {
                Velocity.Y = 0;
            }
            else
            {
                Velocity += ((PlatformerMap)map).Gravity * Weight;
            }

            if (IsTouchingTheCeiling)
            {
                IsTouchingTheCeiling = false;
                Velocity.Y *= -0.5f;
            }
        }

        private void PlatformerPlayer_OnCollideVertically(CollisionSide side)
        {
            if (side == CollisionSide.Top)
            {
                IsTouchingTheGround = true;
            }
        }
    }
}
