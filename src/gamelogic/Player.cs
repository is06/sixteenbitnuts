﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    /// <summary>
    /// Represents all kind of player in games
    /// </summary>
    public abstract class Player : Actor
    {
        // Public fields (for authoring edit)
        public float RunSpeed;

        // Properties
        public Vector2 Velocity { get; set; }
        public Direction Direction { get; set; }
        
        // Internal fields
        protected VirtualStick virtualStick;

        /// <summary>
        /// Creates a player for the given map
        /// </summary>
        /// <param name="map">Map in which the player will be created</param>
        public Player(Map map) : base(map)
        {
            RunSpeed = 1;

            virtualStick = new VirtualStick(map.Game)
                .AddKeys(Keys.Left, Keys.Right, Keys.Up, Keys.Down)
                .AddKeys(Keys.Q, Keys.D, Keys.Z, Keys.S);
        }

        /// <summary>
        /// Updates all data for this player
        /// </summary>
        public override void Update()
        {
            base.Update();

            Velocity = Vector2.Zero;

            virtualStick.Update();
        }

        /// <summary>
        /// Performs the movement of the player according to its velocity
        /// Should be called after every player updates regarding velocity computation
        /// (in order to use the newly computed velocity directly after, not in the next frame)
        /// </summary>
        protected void PerformMove()
        {
            MoveX(Velocity.X);
            MoveY(Velocity.Y);
        }
    }
}
