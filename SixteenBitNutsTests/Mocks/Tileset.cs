using SixteenBitNuts;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    public class MockedTileset : Tileset
    {
        public MockedTileset(SixteenBitNuts.Game game, string name) : base(game, name)
        {
            elements.Add(0, new TileElement()
            {
                Offset = new Vector2(0, 0),
                Size = new Vector2(16, 16),
                Type = TileType.Obstacle
            });
        }

        protected override void LoadTexture(string name)
        {

        }

        protected override void InitDebugHitBox()
        {
            
        }

        protected override void LoadFromFile(string fileName)
        {
            
        }
    }
}
