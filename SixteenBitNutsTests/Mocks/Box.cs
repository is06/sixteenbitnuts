using SixteenBitNuts;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedBox : Box
    {
        public MockedBox(SixteenBitNuts.Game game, Rectangle bounds, int thickness, Color color) : base(game, bounds, thickness, color)
        {

        }

        protected override void InitLines(SixteenBitNuts.Game game, Color color)
        {
            
        }
    }
}
