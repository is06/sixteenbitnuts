using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class DebugHitBox
    {
        protected Box? graphicBox;

        public Color Color
        {
            get
            {
                return graphicBox?.Color ?? Color.Transparent;
            }
            set
            {
                if (graphicBox != null)
                {
                    graphicBox.Color = value;
                }
            }
        }

        public DebugHitBox(Game game, Color color)
        {
            InitGraphicBox(game, color);
        }

        protected virtual void InitGraphicBox(Game game, Color color)
        {
            graphicBox = new Box(
                game,
                new Rectangle(0, 0, 0, 0),
                color
            );
        }

        public void Update(HitBox hitBox)
        {
            if (graphicBox != null)
            {
                graphicBox.Bounds = new Rectangle(hitBox.Position.ToPoint(), hitBox.Size.ToPoint());
            }
            graphicBox?.Update();
        }

        public void Draw(Matrix transform)
        {
            graphicBox?.Draw(transform);
        }
    }
}
