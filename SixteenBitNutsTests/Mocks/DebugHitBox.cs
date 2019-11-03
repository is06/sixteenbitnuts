using SixteenBitNuts;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedDebugHitBox : DebugHitBox
    {
        public MockedDebugHitBox(SixteenBitNuts.Game game, int thickness, Color color) : base(game, thickness, color)
        {

        }

        protected override void InitGraphicBox(SixteenBitNuts.Game game, int thickness, Color color)
        {
            graphicBox = new MockedBox(game, new Rectangle(0, 0, 16, 16), thickness, color);
        }
    }
}
