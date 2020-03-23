using Microsoft.Xna.Framework;
using SixteenBitNuts.Interfaces;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public struct HitBox
    {
        public Vector2 Position { get; set; }
        public Size Size { get; set; }

        public float Left
        {
            get
            {
                return Position.X;
            }
        }
        public float Right
        {
            get
            {
                return Position.X + Size.Width;
            }
        }
        public float Top
        {
            get
            {
                return Position.Y;
            }
        }
        public float Bottom
        {
            get
            {
                return Position.Y + Size.Height;
            }
        }
        public float X
        {
            get
            {
                return Position.X;
            }
        }
        public float Y
        {
            get
            {
                return Position.Y;
            }
        }
        public float Width
        {
            get
            {
                return Size.Width;
            }
        }
        public float Height
        {
            get
            {
                return Size.Height;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(X + Width / 2f, Y + Height / 2f);
            }
        }

        public HitBox(Vector2 position, Size size)
        {
            Position = position;
            Size = size;
        }

        public bool Intersects(HitBox other)
        {
            if (Left < other.Right && Right > other.Left && Top <= other.Bottom && Bottom >= other.Top)
            {
                return true;
            }
            return false;
        }

        public float GetDistanceFrom(HitBox other)
        {
            return Vector2.Distance(Center, other.Center);
        }

        public IMapElement? GetNearestElementIn(List<IMapElement> elements)
        {
            if (elements.Count == 0) return null;

            IMapElement nearest = elements[0];
            float minDistance = GetDistanceFrom(nearest.HitBox);

            foreach (var element in elements)
            {
                var distance = GetDistanceFrom(element.HitBox);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = element;
                }
            }

            return nearest;
        }
    }
}
