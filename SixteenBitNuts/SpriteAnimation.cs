using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    struct SpriteAnimation
    {
        public string Name { get; private set; }
        public Point Size { get; private set; }
        public Point HitBoxOffset { get; private set; }
        public int Length { get; private set; }
        public float Speed { get; private set; }
        public Dictionary<Direction, Point> DirectionOffsets { get; private set; }

        public SpriteAnimation(string name, Point size, Point hitBoxOffset, int length, float speed)
        {
            Name = name;
            Size = size;
            HitBoxOffset = hitBoxOffset;
            Length = length;
            Speed = speed;
            DirectionOffsets = new Dictionary<Direction, Point>();
        }
    }
}
