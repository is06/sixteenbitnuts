using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Entity : Actor
    {
        public string Name { get; private set; }

        public Entity(Map map, Point hitBoxSize, string name) : base(map, hitBoxSize)
        {
            Name = name;
        }
    }
}
