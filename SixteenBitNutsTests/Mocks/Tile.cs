using SixteenBitNuts;
using SixteenBitNuts.Interfaces;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedTile : MockedMapElement, ITile
    {
        public int Id { get; private set; }

        public MockedTile(Map map, Tileset _, int id, Vector2 position, Vector2 size, TileType type) : base(map)
        {
            Id = id;
            Position = position;
            Size = size;
            IsObstacle = type == TileType.Obstacle;
        }

        protected override void InitDebugHitBox()
        {
            
        }
    }
}
