using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Actor
    {
        public delegate void CollisionHandler();

        public Point Position
        {
            get
            {
                return collider.Bounds.Location;
            }
            set
            {
                collider.Bounds.Location = value;
            }
        }

        protected Sprite? sprite;
        protected readonly Map map;

        private readonly Collider collider;
        private float xRemainder;
        private float yRemainder;

        public Actor(Map map, Point hitBoxSize)
        {
            this.map = map;
            collider = new Collider(hitBoxSize);
        }

        public virtual void Initialize()
        {
            sprite?.Initialize();
        }

        public virtual void LoadContent()
        {
            sprite?.LoadContent();
        }

        public virtual void Update()
        {
            if (sprite is Sprite spr)
            {
                spr.Position = Position;
                spr.Update();
            }
        }

        public virtual void Draw(Matrix transform)
        {
            sprite?.Draw(transform);
        }

        /// <summary>
        /// Moves the actor along the X axis. Will stop the actor if a solid is on the way
        /// </summary>
        /// <param name="value">Number of pixels to move the actor</param>
        /// <param name="onCollide">Called when a collision with a solid occurs</param>
        public void MoveX(float value, CollisionHandler? onCollide = null)
        {
            xRemainder += value;
            int move = (int)Math.Round(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                sbyte xStep = Calc.Sign(move);

                while (move != 0)
                {
                    if (!IsOverlappingWith(map.Solids, Position + new Point(xStep, 0)))
                    {
                        collider.Bounds.X += xStep;
                        move -= xStep;
                    }
                    else
                    {
                        onCollide?.Invoke();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Moves the actor along the Y axis. Will stop the actor if a solid is on the way
        /// </summary>
        /// <param name="value">Number of pixels to move the actor</param>
        /// <param name="onCollide">Called when a collision with a solid occurs</param>
        public void MoveY(float value, CollisionHandler? onCollide = null)
        {
            yRemainder += value;
            int move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                sbyte yStep = Calc.Sign(move);

                while (move != 0)
                {
                    if (!IsOverlappingWith(map.Solids, Position + new Point(0, yStep)))
                    {
                        collider.Bounds.Y += yStep;
                        move -= yStep;
                    }
                    else
                    {
                        onCollide?.Invoke();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the current actor collider is overlapping any solid from the map
        /// </summary>
        /// <param name="solids">List of solids to test</param>
        /// <param name="offset">An offset to apply to the overlapping test</param>
        /// <returns>True if the collider is overlapping any solid</returns>
        private bool IsOverlappingWith(List<Solid> solids, Point offset)
        {
            foreach (var solid in solids)
            {
                if (IsOverlappingRectToRect(solid.Bounds, collider.Bounds, offset))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if two rectangles are overlapping
        /// </summary>
        /// <param name="one">The first rectangle</param>
        /// <param name="two">The second rectangle</param>
        /// <param name="offset">An offset to apply to the overlapping test</param>
        /// <returns>True if rectangles are overlapping with the offset provided</returns>
        private bool IsOverlappingRectToRect(Rectangle one, Rectangle two, Point offset)
        {
            Rectangle offsetOneBounds = new Rectangle(one.Location + offset, one.Size);

            return offsetOneBounds.Intersects(two);
        }
    }
}
