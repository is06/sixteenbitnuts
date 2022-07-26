using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace SixteenBitNuts
{
    /// <summary>
    /// Represents a player in a top-down game like Zelda
    /// </summary>
    public class TopDownPlayer : Player
    {
        /// <summary>
        /// Creates the player in the given map
        /// </summary>
        /// <param name="map">Map in which the player will be created</param>
        public TopDownPlayer(Map map, Point hitBoxSize) : base(map, hitBoxSize)
        {
            runStick = new VirtualStick(map.Game)
                .AddKeys(Keys.Left, Keys.Right, Keys.Up, Keys.Down)
                .AddKeys(Keys.Q, Keys.D, Keys.Z, Keys.S);
        }

        /// <summary>
        /// Updates the current direction according to the virtual stick value
        /// </summary>
        protected override void UpdateDirection()
        {
            if (runStick is VirtualStick stick)
            {
                Direction = DirectionHelper.FromNormalizedVector(stick.Value);
            }
        }

        /// <summary>
        /// Updates the velocity according to the current direction
        /// </summary>
        protected override void UpdateVelocity()
        {
            double radians = Direction.GetRadians();
            if (radians != -1)
            {
                // compute move with the direction
                double x = Math.Cos(radians);
                double y = -Math.Sin(radians);
                Velocity = new Vector2((float)x, (float)y) * RunSpeed;
            }
        }
    }
}
