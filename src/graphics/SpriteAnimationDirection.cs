using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public struct SpriteAnimationDirection
    {
        public Point Offset;
        public bool IsFlippedHorizontally;
        public bool IsFlippedVertically;
        public Point? OverrideOffset;
    }
}
