using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class PlatformerMap : Map
    {
        public Vector2 Gravity = new Vector2(0, 0.4f);

        public PlatformerMap(Game game, string name, bool loadFromDefinitionFile = true) : base(game, name, loadFromDefinitionFile)
        {

        }
    }
}
