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
        public static CollisionSide GetCollisionSide(HitBox moving, HitBox stopped, Vector2 movingVelocity)
        {
            double velocityRatio = movingVelocity.Y / movingVelocity.X;

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

        public static List<IMapElement> GetResolutionElementsFromHitBox(HitBox nextFrameHitBox, HitBox currentHitBox, List<IMapElement> elements)
        {
            var result = new List<IMapElement>();

            // We take only elements that intersect the AABB
            var intersectingElements = new List<IMapElement>();
            foreach (var element in elements)
            {
                if (element.HitBox.Intersects(nextFrameHitBox))
                {
                    intersectingElements.Add(element);
                }
            }

            var leftElements = new List<IMapElement>();
            var rightElements = new List<IMapElement>();
            var topElements = new List<IMapElement>();
            var bottomElements = new List<IMapElement>();

            foreach (var element in intersectingElements)
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

            var nearestLeft = GetNearestElementFromHitBox(currentHitBox, leftElements);
            if (nearestLeft != null) result.Add(nearestLeft);
            var nearestRight = GetNearestElementFromHitBox(currentHitBox, rightElements);
            if (nearestRight != null) result.Add(nearestRight);
            var nearestTop = GetNearestElementFromHitBox(currentHitBox, topElements);
            if (nearestTop != null) result.Add(nearestTop);
            var nearestBottom = GetNearestElementFromHitBox(currentHitBox, bottomElements);
            if (nearestBottom != null) result.Add(nearestBottom);

            return result;
        }

        /// <summary>
        /// Retrieve the nearest element in a list from a hitbox
        /// </summary>
        /// <param name="hitBox">The origin hitbox, usually the player hitbox</param>
        /// <param name="elements">List of element to look into</param>
        /// <returns>The nearest element</returns>
        public static IMapElement GetNearestElementFromHitBox(HitBox hitBox, List<IMapElement> elements)
        {
            if (elements.Count == 0) return null;

            IMapElement nearest = elements.First();
            float minDistance = CollisionManager.GetDistance(hitBox, nearest.HitBox);

            foreach (var element in elements)
            {
                var distance = CollisionManager.GetDistance(hitBox, element.HitBox);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = element;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Returns the distance between two hitboxes
        /// </summary>
        /// <param name="first">First hitbox</param>
        /// <param name="second">Second hitbox</param>
        /// <returns>float</returns>
        public static float GetDistance(HitBox first, HitBox second)
        {
            Vector2 firstCenter = new Vector2(first.X + first.Width / 2f, first.Y + first.Height / 2f);
            Vector2 secondCenter = new Vector2(second.X + second.Width / 2f, second.Y + second.Height / 2f);

            return Vector2.Distance(firstCenter, secondCenter);
        }
    }
}
