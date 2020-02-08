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

        public DebugHitBox(Game game, int thickness, Color color)
        {
            InitGraphicBox(game, thickness, color);
        }

        protected virtual void InitGraphicBox(Game game, int thickness, Color color)
        {
            graphicBox = new Box(
                game,
                new Rectangle(0, 0, 0, 0),
                thickness,
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

        public void Draw()
        {
            graphicBox?.Draw();
        }
    }
}
