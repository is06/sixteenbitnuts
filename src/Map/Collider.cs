using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public abstract class Collider
    {
        public bool IsPlayer { get; set; }
        public Size Size { get; set; }
        public HitBox HitBox { get; set; }
        public HitBox PreviousFrameHitBox { get; set; }
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
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        protected Map map;
        protected Vector2 position;
        protected Vector2 velocity;

        protected DebugHitBox debugHitBox;

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
