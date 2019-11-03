using SixteenBitNuts;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNutsTests.Mocks
{
    class MockedSpawnPoint : MockedEntity, ISpawnPoint
    {
        public MockedSpawnPoint(SixteenBitNuts.Map map, string name) : base(map, name)
        {

        }
    }
}
