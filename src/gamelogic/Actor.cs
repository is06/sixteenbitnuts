using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Actor
    {
        public delegate void CollisionHandler(Solid solid);

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
        protected readonly Collider collider;

        private float xRemainder;
        private float yRemainder;

        public Actor(Map map, Point hitBoxSize)
        {
            this.map = map;
            collider = new Collider(map.Game, hitBoxSize);
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
            collider.Update();

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

        public virtual void DebugDraw()
        {
            collider.DebugDraw();
        }

        /// <summary>
        /// Moves the actor along the X axis. Will stop the actor if a solid is on the way
        /// </summary>
        /// <param name="value">Number of pixels to move the actor</param>
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
                    var solidOrNull = GetOverlappingSolid(map.Solids, new Point(xStep, 0));

                    if (solidOrNull is Solid solid)
                    {
                        onCollide?.Invoke(solid);
                        break;
                    }
                    else
                    {
                        collider.Bounds.X += xStep;
                        move -= xStep;
                    }
                }
            }
        }

        /// <summary>
        /// Moves the actor along the Y axis. Will stop the actor if a solid is on the way
        /// </summary>
        /// <param name="value">Number of pixels to move the actor</param>
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
                    var solidOrNull = GetOverlappingSolid(map.Solids, new Point(0, yStep));

                    if (solidOrNull is Solid solid)
                    {
                        onCollide?.Invoke(solid);
                        break;
                    }
                    else
                    {
                        collider.Bounds.Y += yStep;
                        move -= yStep;
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
        private Solid? GetOverlappingSolid(List<Solid> solids, Point offset)
        {
            foreach (var solid in solids)
            {
                if (IsOverlappingRectToRect(collider.Bounds, solid.Bounds, offset))
                {
                    return solid;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if two rectangles are overlapping
        /// </summary>
        /// <param name="one">The first rectangle</param>
        /// <param name="two">The second rectangle</param>
        /// <param name="offset">An offset to apply to the overlapping test</param>
        /// <returns>True if rectangles are overlapping with the offset provided</returns>
        private bool IsOverlappingRectToRect(Rectangle moving, Rectangle still, Point offset)
        {
            Rectangle offsetOneBounds = new Rectangle(moving.Location + offset, moving.Size);

            return offsetOneBounds.Intersects(still);
        }
    }
}
