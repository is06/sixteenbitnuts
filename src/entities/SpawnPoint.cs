using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class SpawnPoint : Entity
    {
        public SpawnPoint(Map map, string name) : base(map, name)
        {
            SetSize(16, 16);
        }
    }
}
