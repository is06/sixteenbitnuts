using System;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    /// <summary>
    /// Collision side flags enumeration
    /// </summary>
    [Flags]
    public enum CollisionSide
    {
        None = 0,
        Right = 1,
        Top = 2,
        Left = 4,
        Bottom = 8,
    }

    /// <summary>
    /// Helper class to offer some functions to perform collision detection
    /// Based on work by HopefulToad, Thanks to him/her
    /// https://hopefultoad.blogspot.com/2017/09/2d-aabb-collision-detection-and-response.html
    /// </summary>
    public static class CollisionManager
    {
        /// <summary>
        /// Returns the collision side between two objects
        /// </summary>
        /// <param name="moving">The previous frame moving object hitbox</param>
        /// <param name="stopped">The static object hitbox</param>
        /// <param name="movingVelocity">The velocity vector</param>
        /// <returns>CollisionSide value</returns>
        public static CollisionSide GetCollisionSide(BoundingBox moving, BoundingBox stopped, Vector2 movingVelocity)
        {
            double velocityRatio = movingVelocity.Y / movingVelocity.X;

            CollisionSide side = CollisionSide.None;

            double cornerVectorRise;
            double cornerVectorRun;

            if (moving.Max.X <= stopped.Min.X)
            {
                side |= CollisionSide.Left;
                cornerVectorRun = stopped.Min.X - moving.Max.X;

                if (moving.Max.Y <= stopped.Min.Y)
                {
                    side |= CollisionSide.Top;
                    cornerVectorRise = stopped.Min.Y - moving.Max.Y;
                }
                else if (moving.Min.Y >= stopped.Max.Y)
                {
                    side |= CollisionSide.Bottom;
                    cornerVectorRise = stopped.Max.Y - moving.Min.Y;
                }
                else
                {
                    return CollisionSide.Left;
                }
            }
            else if (moving.Min.X >= stopped.Max.X)
            {
                side |= CollisionSide.Right;
                cornerVectorRun = moving.Min.X - stopped.Max.X;

                if (moving.Max.Y <= stopped.Min.Y)
                {
                    side |= CollisionSide.Top;
                    cornerVectorRise = moving.Max.Y - stopped.Min.Y;
                }
                else if (moving.Min.Y >= stopped.Max.Y)
                {
                    side |= CollisionSide.Bottom;
                    cornerVectorRise = moving.Min.Y - stopped.Max.Y;
                }
                else
                {
                    return CollisionSide.Right;
                }
            }
            else
            {
                if (moving.Max.Y <= stopped.Min.Y)
                {
                    return CollisionSide.Top;
                }
                if (moving.Min.Y >= stopped.Max.Y)
                {
                    return CollisionSide.Bottom;
                }
                return CollisionSide.None;
            }

            return GetCollisionSideFromVectorComparison(side, velocityRatio, cornerVectorRise / cornerVectorRun);
        }

        /// <summary>
        /// Get the collision side when moving object has a non-straight movement
        /// </summary>
        /// <param name="sides">Side flags to enrich</param>
        /// <param name="velocityRatio">The velocity ratio between previous frame moving object corner point and static object corner point</param>
        /// <param name="nearestCornerRatio">The nearest corner ratio between corrected position corner point and static object corner point</param>
        /// <returns></returns>
        private static CollisionSide GetCollisionSideFromVectorComparison(CollisionSide sides, double velocityRatio, double nearestCornerRatio)
        {
            if ((sides & CollisionSide.Top) == CollisionSide.Top)
            {
                if ((sides & CollisionSide.Left) == CollisionSide.Left)
                {
                    return velocityRatio < nearestCornerRatio ? CollisionSide.Top : CollisionSide.Left;
                }
                if ((sides & CollisionSide.Right) == CollisionSide.Right)
                {
                    return velocityRatio > nearestCornerRatio ? CollisionSide.Top : CollisionSide.Right;
                }
            }
            else if ((sides & CollisionSide.Bottom) == CollisionSide.Bottom)
            {
                if ((sides & CollisionSide.Left) == CollisionSide.Left)
                {
                    return velocityRatio > nearestCornerRatio ? CollisionSide.Bottom : CollisionSide.Left;
                }
                if ((sides & CollisionSide.Right) == CollisionSide.Right)
                {
                    return velocityRatio < nearestCornerRatio ? CollisionSide.Bottom : CollisionSide.Right;
                }
            }

            return CollisionSide.None;
        }

        /// <summary>
        /// Return the corrected location to prevent from intersecting
        /// </summary>
        /// <param name="moving">Current frame moving object hitbox</param>
        /// <param name="stopped">Current frame static object hitbox</param>
        /// <param name="side">Collision side detected</param>
        /// <returns>Point</returns>
        public static Vector2 GetCorrectedPosition(BoundingBox moving, BoundingBox stopped, CollisionSide side)
        {
            Vector2 correctedPosition = new Vector2(moving.Min.X, moving.Min.Y);
            switch (side)
            {
                case CollisionSide.Left:
                    correctedPosition.X = stopped.Min.X - (moving.Max.X - moving.Min.X);
                    break;
                case CollisionSide.Right:
                    correctedPosition.X = stopped.Min.X + (stopped.Max.X - stopped.Min.X);
                    break;
                case CollisionSide.Top:
                    correctedPosition.Y = stopped.Min.Y - (moving.Max.Y - moving.Min.Y);
                    break;
                case CollisionSide.Bottom:
                    correctedPosition.Y = stopped.Min.Y + (stopped.Max.Y - stopped.Min.Y);
                    break;
            }
            return correctedPosition;
        }

        /// <summary>
        /// Returns the distance between two hitboxes
        /// </summary>
        /// <param name="first">First hitbox</param>
        /// <param name="second">Second hitbox</param>
        /// <returns>float</returns>
        public static float GetDistance(BoundingBox first, BoundingBox second)
        {
            Vector2 firstCenter = new Vector2(
                first.Min.X + (first.Max.X - first.Min.X) / 2f,
                first.Min.Y + (first.Max.Y - first.Min.Y) / 2f
            );
            Vector2 secondCenter = new Vector2(
                second.Min.X + (second.Max.X - second.Min.X) / 2f,
                second.Min.Y + (second.Max.Y - second.Min.Y) / 2f
            );

            return Vector2.Distance(firstCenter, secondCenter);
        }
    }
}
