using Microsoft.Xna.Framework;
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
        public TopDownPlayer(Map map) : base(map)
        {

        }

        /// <summary>
        /// Updates all data for this player
        /// </summary>
        public override void Update()
        {
            base.Update();

            UpdateDirection();
            UpdateVelocity();
            PerformMove();
        }

        /// <summary>
        /// Updates the current direction according to the virtual stick value
        /// </summary>
        private void UpdateDirection()
        {
            Direction = DirectionHelper.FromVirtualStickValue(virtualStick.Value);
        }

        /// <summary>
        /// Updates the velocity according to the current direction
        /// </summary>
        private void UpdateVelocity()
        {
            var radians = Direction.GetRadians();
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
