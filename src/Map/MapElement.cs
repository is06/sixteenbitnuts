using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public abstract class MapElement : IMapElement
    {
        #region Properties

        public Color DebugColor { get; set; }
        public bool IsVisible { get; set; }
        public bool IsObstacle { get; set; }
        public bool IsPlatform { get; set; }
        public Vector2 Position
        {
            get
            {
                return HitBox.Position;
            }
            set
            {
                HitBox = new HitBox(value, Size);
            }
        }
        public Size Size
        {
            get
            {
                return HitBox.Size;
            }
            set
            {
                HitBox = new HitBox(Position, value);
            }
        }
        public HitBox HitBox { get; set; }

        #endregion

        #region Fields

        private readonly Box debugHitBox;
        protected readonly Map map;

        #endregion

        public MapElement(Map map, HitBox? hitBox = null)
        {
            this.map = map;
            
            IsVisible = true;

            if (hitBox is HitBox hb)
            {
                HitBox = hb;
            }
            else
            {
                HitBox = new HitBox()
                {
                    Position = new Vector2(0, 0),
                    Size = new Size(16, 16),
                };
            }

            debugHitBox = new Box(
                map.Game,
                new Rectangle(Position.ToPoint(), Size.ToPoint()),
                DebugColor
            );
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(Matrix transform)
        {

        }

        public virtual void EditorDraw(Matrix transform)
        {

        }

        public virtual void DebugDraw(Matrix transform)
        {
            debugHitBox.Color = DebugColor;
            debugHitBox.Bounds = new Rectangle(Position.ToPoint(), Size.ToPoint());
            debugHitBox.Update();
            debugHitBox.Draw(transform);
        }

        protected Texture2D GetTexture(string textureName)
        {
            return map.Game.Content.Load<Texture2D>(textureName);
        }
    }
}
