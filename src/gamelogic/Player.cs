using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public delegate void CollisionSideHandler(CollisionSide side);

    /// <summary>
    /// Represents all kind of player in games
    /// </summary>
    public abstract class Player : Actor
    {
        public event CollisionSideHandler? OnCollideVertically;
        public event CollisionSideHandler? OnCollideHorizontally;

        // Public fields (for authoring edit)
        public float RunSpeed;
        public Vector2 Velocity;

        // Properties
        public bool IsControllable { get; set; }
        public Direction Direction { get; set; }
        
        // Internal fields
        protected VirtualStick? run;

        /// <summary>
        /// Creates a player for the given map
        /// </summary>
        /// <param name="map">Map in which the player will be created</param>
        public Player(Map map, Point hitBoxSize) : base(map, hitBoxSize)
        {
            RunSpeed = 1;
            IsControllable = true;
        }

        /// <summary>
        /// Updates all data for this player
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            run?.Update();

            UpdateDirection();
            UpdateVelocity();
            BeforeApplyMove();
            ApplyMove();
        }

        /// <summary>
        /// Override this function to define how the direction is updated
        /// </summary>
        protected abstract void UpdateDirection();

        /// <summary>
        /// Override this function to defined how the velocity is updated
        /// </summary>
        protected abstract void UpdateVelocity();

        /// <summary>
        /// Override this function to make other computations before applying the final move
        /// </summary>
        protected virtual void BeforeApplyMove()
        {

        }

        /// <summary>
        /// Performs the movement of the player according to its velocity
        /// Should be called after every player updates regarding velocity computation
        /// (in order to use the newly computed velocity directly after, not in the next frame)
        /// </summary>
        protected void ApplyMove()
        {
            if (Velocity != Vector2.Zero)
            {
                MoveX(Velocity.X, (Solid solid) =>
                {
                    if (Velocity.X > 0)
                    {
                        OnCollideHorizontally?.Invoke(CollisionSide.Left);
                    }
                    else if (Velocity.X < 0)
                    {
                        OnCollideHorizontally?.Invoke(CollisionSide.Right);
                    }
                });
                MoveY(Velocity.Y, (Solid solid) =>
                {
                    if (Velocity.Y > 0)
                    {
                        OnCollideVertically?.Invoke(CollisionSide.Top);
                    }
                    else if (Velocity.Y < 0)
                    {
                        OnCollideVertically?.Invoke(CollisionSide.Bottom);
                    }
                });
            }
        }
    }
}
