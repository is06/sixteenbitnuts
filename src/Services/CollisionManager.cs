using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SixteenBitNuts.Interfaces;

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

    public static class CollisionSideMethods
    {
        public static Direction ToDirection(this CollisionSide side)
        {
            return side switch
            {
                CollisionSide.Top => Direction.Top,
                CollisionSide.Top | CollisionSide.Left => Direction.TopLeft,
                CollisionSide.Top | CollisionSide.Right => Direction.TopRight,
                CollisionSide.Bottom => Direction.Bottom,
                CollisionSide.Bottom | CollisionSide.Left => Direction.BottomLeft,
                CollisionSide.Bottom | CollisionSide.Right => Direction.BottomRight,
                CollisionSide.Left => Direction.Left,
                CollisionSide.Right => Direction.Right,
                _ => Direction.None,
            };
        }
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
        public static CollisionSide GetCollisionSideForResolution(HitBox moving, HitBox stopped, Vector2 movingVelocity)
        {
            double velocityRatio = 0.0;
            if (movingVelocity != Vector2.Zero)
            {
                velocityRatio = movingVelocity.Y / movingVelocity.X;
            }

            CollisionSide side = CollisionSide.None;

            double cornerVectorRise;
            double cornerVectorRun;

            if (moving.Right <= stopped.Left)
            {
                side |= CollisionSide.Left;
                cornerVectorRun = stopped.Left - moving.Right;

                if (moving.Bottom <= stopped.Top)
                {
                    side |= CollisionSide.Top;
                    cornerVectorRise = stopped.Top - moving.Bottom;
                }
                else if (moving.Top >= stopped.Bottom)
                {
                    side |= CollisionSide.Bottom;
                    cornerVectorRise = stopped.Bottom - moving.Top;
                }
                else
                {
                    return CollisionSide.Left;
                }
            }
            else if (moving.Left >= stopped.Right)
            {
                side |= CollisionSide.Right;
                cornerVectorRun = moving.Left - stopped.Right;

                if (moving.Bottom <= stopped.Top)
                {
                    side |= CollisionSide.Top;
                    cornerVectorRise = moving.Bottom - stopped.Top;
                }
                else if (moving.Top >= stopped.Bottom)
                {
                    side |= CollisionSide.Bottom;
                    cornerVectorRise = moving.Top - stopped.Bottom;
                }
                else
                {
                    return CollisionSide.Right;
                }
            }
            else
            {
                if (moving.Bottom <= stopped.Top)
                {
                    return CollisionSide.Top;
                }
                if (moving.Top >= stopped.Bottom)
                {
                    return CollisionSide.Bottom;
                }
                return CollisionSide.None;
            }

            if (velocityRatio == 0.0)
            {
                return side;
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

        public static CollisionSide GetCollisionSideBetweenStopped(HitBox one, HitBox two)
        {
            var side = CollisionSide.None;

            if (one.Left <= two.Right && one.Right > two.Right)
            {
                side |= CollisionSide.Right;
            }
            else if (one.Right >= two.Left && one.Left < two.Left)
            {
                side |= CollisionSide.Left;
            }

            if (one.Bottom >= two.Top && one.Top < two.Top)
            {
                side |= CollisionSide.Top;
            }
            else if (one.Top <= two.Bottom && one.Bottom > two.Bottom)
            {
                side |= CollisionSide.Bottom;
            }

            return side;
        }

        /// <summary>
        /// Return the corrected location to prevent from intersecting
        /// </summary>
        /// <param name="moving">Current frame moving object hitbox</param>
        /// <param name="stopped">Current frame static object hitbox</param>
        /// <param name="side">Collision side detected</param>
        /// <returns>Point</returns>
        public static Vector2 GetCorrectedPosition(HitBox moving, HitBox stopped, CollisionSide side)
        {
            Vector2 correctedPosition = moving.Position;
            switch (side)
            {
                case CollisionSide.Left:
                    correctedPosition.X = stopped.X - moving.Width;
                    break;
                case CollisionSide.Right:
                    correctedPosition.X = stopped.X + stopped.Width;
                    break;
                case CollisionSide.Top:
                    correctedPosition.Y = stopped.Y - moving.Height;
                    break;
                case CollisionSide.Bottom:
                    correctedPosition.Y = stopped.Y + stopped.Height;
                    break;
            }
            return correctedPosition;
        }

        public static List<IMapElement> GetSelectedElementsForDetection(HitBox nextFrameHitBox, List<IMapElement> elements)
        {
            var result = new List<IMapElement>();

            foreach (var element in elements)
            {
                if (element.HitBox.Intersects(nextFrameHitBox))
                {
                    result.Add(element);
                }
            }

            return result;
        }

        public static List<IMapElement> GetSelectedElementsForDetectionWithGravity(HitBox nextFrameHitBox, HitBox currentHitBox, List<IMapElement> elements)
        {
            var result = new List<IMapElement>();

            var leftElements = new List<IMapElement>();
            var rightElements = new List<IMapElement>();
            var topElements = new List<IMapElement>();
            var bottomElements = new List<IMapElement>();

            foreach (var element in GetSelectedElementsForDetection(nextFrameHitBox, elements))
            {
                if (currentHitBox.Bottom <= element.HitBox.Top)
                {
                    bottomElements.Add(element);
                }
                else
                {
                    if (currentHitBox.Left >= element.HitBox.Right)
                    {
                        leftElements.Add(element);
                    }
                    else if (currentHitBox.Right <= element.HitBox.Left)
                    {
                        rightElements.Add(element);
                    }
                    else if (currentHitBox.Top >= element.HitBox.Bottom)
                    {
                        topElements.Add(element);
                    }
                }
            }

            var nearestLeft = currentHitBox.GetNearestElementIn(leftElements);
            if (nearestLeft != null) result.Add(nearestLeft);
            var nearestRight = currentHitBox.GetNearestElementIn(rightElements);
            if (nearestRight != null) result.Add(nearestRight);
            var nearestTop = currentHitBox.GetNearestElementIn(topElements);
            if (nearestTop != null) result.Add(nearestTop);
            var nearestBottom = currentHitBox.GetNearestElementIn(bottomElements);
            if (nearestBottom != null) result.Add(nearestBottom);

            return result;
        }
    }
}
