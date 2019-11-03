using SixteenBitNuts;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNutsTests.Mocks
{
    class MockedMapElement : MapElement, IMapElement
    {
        public MockedMapElement(SixteenBitNuts.Map map) : base(map)
        {

        }

        protected override void InitDebugHitBox()
        {
            
        }
    }
}
