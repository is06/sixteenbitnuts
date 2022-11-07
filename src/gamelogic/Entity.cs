using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Entity : Actor
    {
        public string Name { get; private set; }

        public Entity(Map map, string name, Point? hitBoxSize = null) : base(map, hitBoxSize)
        {
            Name = name;
        }
    }
}
