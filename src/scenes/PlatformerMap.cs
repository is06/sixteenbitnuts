using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class PlatformerMap : Map
    {
        private Vector2 gravity;

        public PlatformerMap(Game game, string name, bool loadFromDefinitionFile = true) : base(game, name, loadFromDefinitionFile)
        {

        }
    }
}
