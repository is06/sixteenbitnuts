using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public struct SpriteAnimation
    {
        public string Name { get; set; }
        public Point Size { get; set; }
        public Point HitBoxOffset { get; set; }
        public int Length { get; set; }
        public float Speed { get; set; }
        public Dictionary<Direction, Point> DirectionOffsets { get; set; }
        public bool Looped { get; set; }
    }
}
