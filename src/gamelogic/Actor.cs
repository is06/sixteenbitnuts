using Microsoft.Xna.Framework;
using System;

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

        public Point Center
        {
            get
            {
                return collider.Bounds.Center;
            }
        }

        public Point RelativeCenter
        {
            get
            {
                return new Point(collider.Bounds.Size.X / 2, collider.Bounds.Size.Y / 2);
            }
        }

        public Point RelativeCenterBottom
        {
            get
            {
                return new Point(collider.Bounds.Size.X / 2, collider.Bounds.Size.Y);
            }
        }

        protected Sprite? sprite;
        protected Point spriteOffset = Point.Zero;
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
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update()
        {
            collider.Update();

            if (sprite is Sprite)
            {
                sprite.Position = Position + spriteOffset;
                sprite.Update();
            }
        }

        public virtual void Draw(Matrix transform)
        {
            sprite?.Draw(transform);
        }

        public virtual void DebugDraw(Matrix transform)
        {
            collider.DebugDraw(transform);
        }

        /// <summary>
        /// Get the loaded sprite from the asset manager.
        /// </summary>
        /// <param name="name">The name of the loaded sprite to retrieve</param>
        /// <returns>The sprite if loaded, null otherwise</returns>
        protected Sprite? GetSprite(string name)
        {
            return map.Game.AssetManager?.GetSprite(name);
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
                    var solidOrNull = collider.GetOverlappingSolid(map.Solids, new Point(xStep, 0));

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
                    var solidOrNull = collider.GetOverlappingSolid(map.Solids, new Point(0, yStep));

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
    }
}
