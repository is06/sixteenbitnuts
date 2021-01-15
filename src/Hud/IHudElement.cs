using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    interface IHudElement
    {
        bool IsVisible { get; set; }
        Vector2 Position { get; set; }
        Size Size { get; set; }

        void Update(GameTime gameTime);
        void Draw(Matrix transform);
        void DebugDraw(Matrix transform);
    }
}
