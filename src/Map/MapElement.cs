using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public abstract class MapElement : IMapElement
    {
        #region Constants

        private const int DEBUG_BOX_THICKNESS = 1;

        #endregion

        #region Properties

        public Color DebugColor { get; set; }
        public bool IsVisible { get; set; }
        public bool IsObstacle { get; set; }
        public bool IsPlatform { get; set; }
        public bool IsCollectable { get; set; }
        public bool IsDestroying { get; set; }
        public Vector2 Position { get; set; }
        public Size Size { get; set; }
        public HitBox HitBox
        {
            get
            {
                return new HitBox(Position, Size);
            }
        }

        #endregion

        #region Fields

        private readonly Box debugHitBox;
        protected readonly Map map;

        #endregion

        public MapElement(Map map)
        {
            this.map = map;
            debugHitBox = new Box(
                map.Game,
                new Rectangle(Position.ToPoint(), Size.ToPoint()),
                DEBUG_BOX_THICKNESS,
                DebugColor
            );
            IsVisible = true;
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
