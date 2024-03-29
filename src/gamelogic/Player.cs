﻿using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    /// <summary>
    /// Represents all kind of player in games
    /// </summary>
    public abstract class Player : Actor
    {
        public delegate void LifeCycleStepHandler();

        public event LifeCycleStepHandler? OnBeforeApplyMoveAndDetectCollisions;
        public event LifeCycleStepHandler? OnAfterApplyMoveAndDetectCollisions;
        public event CollisionSideHandler? OnCollideVertically;
        public event CollisionSideHandler? OnCollideHorizontally;

        // Public fields (for authoring edit)
        public float RunSpeed;
        public Vector2 Velocity;

        // Properties
        public bool IsControllable { get; set; }
        public bool IsRunning { get; protected set; }
        public Direction MoveDirection { get; set; }
        public Direction LookDirection { get; set; }

        // Internal fields
        protected VirtualStick? runStick;
        protected bool IsIntersectingWithObstacle = false;

        /// <summary>
        /// Creates a player for the given map
        /// </summary>
        /// <param name="map">Map in which the player will be created</param>
        public Player(Map map, Point? hitBoxSize = null) : base(map, hitBoxSize)
        {
            RunSpeed = 1;
            IsControllable = true;
            LookDirection = Direction.Right;
        }

        /// <summary>
        /// Updates all data for this player
        /// </summary>
        public override void Update()
        {
            base.Update();

            runStick?.Update();

            UpdateMoveDirection();
            UpdateVelocity();
            UpdateStates();
            UpdateSprite();
            OnBeforeApplyMoveAndDetectCollisions?.Invoke();
            ApplyMoveAndDetectCollisions();
            OnAfterApplyMoveAndDetectCollisions?.Invoke();
        }

        /// <summary>
        /// Override this function to define how the direction is updated
        /// </summary>
        protected abstract void UpdateMoveDirection();

        /// <summary>
        /// Override this function to define how the velocity is updated
        /// </summary>
        protected abstract void UpdateVelocity();

        /// <summary>
        /// Override this function to define states for the player after velocity computation
        /// </summary>
        protected virtual void UpdateStates()
        {

        }

        protected virtual void UpdateSprite()
        {

        }

        /// <summary>
        /// Performs the movement of the player according to its velocity
        /// </summary>
        private void ApplyMoveAndDetectCollisions()
        {
            IsIntersectingWithObstacle = false;

            if (Velocity != Vector2.Zero)
            {
                MoveX(Velocity.X, (Solid solid) =>
                {
                    IsIntersectingWithObstacle = true;

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
                    IsIntersectingWithObstacle = true;

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
