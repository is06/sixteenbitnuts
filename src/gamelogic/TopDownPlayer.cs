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

            HandleMoves();
        }

        private void HandleMoves()
        {
            // update direction of the player
            Direction = DirectionHelper.FromVirtualStickValue(virtualStick.Value);

            var radians = Direction.GetRadians();
            if (radians != -1)
            {
                // compute move with the direction
                double x = Math.Cos(radians);
                double y = -Math.Sin(radians);
                Vector2 move = new Vector2((float)x, (float)y);

                // actually move the player actor
                MoveX(move.X);
                MoveY(move.Y);
            }
        }
    }
}
