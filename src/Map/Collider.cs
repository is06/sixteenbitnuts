using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    /// <summary>
    /// Colliders are a way to detect collisions between them and elements
    /// of a map. They are collision enabled by default, IsCollisionEnabled field
    /// can be set to false to disable collision detection if needed.
    /// </summary>
    public abstract class Collider
    {
        // Exposed fields
        public bool IsCollisionEnabled;
        public bool IsGravityEnabled;
        public HitBox HitBox { get; set; }
        public Vector2 Velocity;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                HitBox = new HitBox
                {
                    Position = position,
                    Size = HitBox.Size
                };
            }
        }

        // Read only properties
        public Size Size
        {
            get
            {
                return new Size(HitBox.Size.Width, HitBox.Size.Height);
            }
        }
        public HitBox PreviousFrameHitBox { get; protected set; }

        // Private members
        protected Map map;
        protected Vector2 position;
        protected DebugHitBox debugHitBox;

        /// <summary>
        /// Collider constructor
        /// </summary>
        /// <param name="map"></param>
        public Collider(Map map, Size hitBoxSize, Color debugHitBoxColor)
        {
            this.map = map;

            IsCollisionEnabled = true;

            // Hitboxes
            var initPosition = Vector2.Zero;
            HitBox = new HitBox(initPosition, hitBoxSize);
            PreviousFrameHitBox = new HitBox(initPosition, hitBoxSize);

            // Debug
            debugHitBox = new DebugHitBox(map.Game, debugHitBoxColor);
        }

        public virtual void Update(GameTime _)
        {
            // Memorize the previous frame hit box
            PreviousFrameHitBox = HitBox;
        }

        public virtual void UpdateDebugHitBoxes()
        {
            debugHitBox.Update(HitBox);
        }

        /// <summary>
        /// Draw debug info of the player sprite
        /// </summary>
        public virtual void DebugDraw(Matrix transform)
        {
            debugHitBox.Draw(transform);
        }
    }
}
