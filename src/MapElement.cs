using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public class MapElement : IMapElement
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

        private Box debugHitBox;
        protected readonly Map map;

        #endregion

        public MapElement(Map map)
        {
            this.map = map;
            InitDebugHitBox();
            IsVisible = true;
        }

        protected virtual void InitDebugHitBox()
        {
            debugHitBox = new Box(
                map.Game,
                new Rectangle(Position.ToPoint(), Size.ToPoint()),
                DEBUG_BOX_THICKNESS,
                DebugColor
            );
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw()
        {

        }

        public virtual void EditorDraw()
        {

        }

        public virtual void DebugDraw()
        {
            debugHitBox.Color = DebugColor;
            debugHitBox.Bounds = new Rectangle(Position.ToPoint(), Size.ToPoint());
            debugHitBox.Update();
            debugHitBox.Draw();
        }

        protected Texture2D GetTexture(string textureName)
        {
            return map.Game.Content.Load<Texture2D>(textureName);
        }
    }
}
