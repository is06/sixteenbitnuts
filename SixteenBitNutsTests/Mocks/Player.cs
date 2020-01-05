using SixteenBitNuts;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedPlayer : Player
    {
        public MockedPlayer(Map map) : base(map)
        {

        }

        protected override void InitSprites(Map map)
        {
            sprite = new MockedSprite(map.Game, "test_sprite");
        }

        protected override void InitDebugBoxes(Map map)
        {
            debugHitBox = new MockedDebugHitBox(map.Game, 1, Color.Cyan);
            debugPreviousFrameHitBox = new MockedDebugHitBox(map.Game, 2, Color.DarkOliveGreen);
            debugAttackBox = new MockedDebugHitBox(map.Game, 1, Color.Red);
        }
    }
}
