using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public abstract class Collider
    {
        // Exposed fields
        public HitBox HitBox;
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
        public bool IsPlayer { get; protected set; }
        public Size Size { get; protected set; }
        public HitBox PreviousFrameHitBox { get; protected set; }

        // Private members
        protected Map map;
        protected Vector2 position;
        protected DebugHitBox debugHitBox;

        /// <summary>
        /// Collider constructor
        /// </summary>
        /// <param name="map"></param>
        public Collider(Map map)
        {
            this.map = map;

            // Hitboxes
            HitBox = new HitBox(position, Size);
            PreviousFrameHitBox = new HitBox(position, Size);

            // Debug
            debugHitBox = new DebugHitBox(map.Game, Color.Cyan);
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
