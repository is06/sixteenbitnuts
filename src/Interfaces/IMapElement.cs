using Microsoft.Xna.Framework;

namespace SixteenBitNuts.Interfaces
{
    public interface IMapElement
    {
        Color DebugColor { get; set; }
        bool IsVisible { get; set; }
        bool IsObstacle { get; set; }
        bool IsPlatform { get; set; }
        Vector2 Position { get; set; }
        Size Size { get; set; }
        HitBox HitBox { get; }

        void Update(GameTime gameTime);
        void Draw(Matrix transform);
        void EditorDraw(Matrix transform);
        void DebugDraw(Matrix transform);
    }
}
