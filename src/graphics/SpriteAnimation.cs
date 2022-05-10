using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public struct SpriteAnimation
    {
        public string Name;
        public Point Size;
        public Point Offset;
        public uint Length;
        public float Speed;
        public Dictionary<Direction, SpriteAnimationDirection> Directions;
        public bool IsLooped;
    }
}
