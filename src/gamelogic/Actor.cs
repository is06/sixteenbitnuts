using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    public class Actor
    {
        public delegate void CollisionHandler(Solid solid);
        public delegate void CollisionSideHandler(CollisionSide side);
        public delegate void CollisionWithEntityHandler(Entity entity);
        
        public event CollisionWithEntityHandler? OnCollideWithEntity;

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

        public Rectangle Bounds
        {
            get
            {
                return collider.Bounds;
            }
        }

        protected Sprite? sprite;
        protected Point spriteOffset = Point.Zero;
        protected readonly Map map;
        protected readonly Collider collider;

        // Used for moving the actor pixel by pixel until a solid stops it
        private float xRemainder;
        private float yRemainder;

        public Actor(Map map, Point? hitBoxSize = null)
        {
            this.map = map;
            collider = new Collider(map.Game, hitBoxSize ?? new Point(16, 16));
        }

        /// <summary>
        /// Initialize function for every actor
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Load content function for every actor
        /// </summary>
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

        /// <summary>
        /// Draws all debug information related to the actor like the collider hitbox
        /// </summary>
        /// <param name="transform">Transform matrix to apply while drawing debug info</param>
        public virtual void DebugDraw(Matrix transform)
        {
            collider.DebugDraw(transform);
        }

        /// <summary>
        /// Sets the size of the actor collider
        /// </summary>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        protected void SetSize(int width, int height)
        {
            collider.Bounds.Size = new Point(width, height);
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
        /// Sets the sprite used by this actor
        /// </summary>
        /// <param name="name">The name of the sprite file</param>
        protected void SetSprite(string name)
        {
            sprite = GetSprite(name);
        }

        /// <summary>
        /// Starts the animation on the main sprite of the actor
        /// </summary>
        /// <param name="name">Name of the animation to start</param>
        protected void StartAnimation(string name)
        {
            sprite?.StartAnimation(name);
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
