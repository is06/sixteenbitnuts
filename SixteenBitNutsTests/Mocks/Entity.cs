using SixteenBitNuts.Interfaces;

namespace SixteenBitNutsTests.Mocks
{
    class MockedEntity : MockedMapElement, IEntity
    {
        public string Name { get; private set; }

        public MockedEntity(SixteenBitNuts.Map map, string name) : base(map)
        {
            Name = name;
        }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}
