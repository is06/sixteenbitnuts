using SixteenBitNuts;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedPlayer : Player
    {
        public MockedPlayer(SixteenBitNuts.Map map, Vector2 position) : base(map, position)
        {

        }

        protected override void InitSprites(SixteenBitNuts.Map map)
        {
            sprite = new MockedSprite(map.Game, "test_sprite");
        }

        protected override void InitDebugBoxes(SixteenBitNuts.Map map)
        {
            debugHitBox = new MockedDebugHitBox(map.Game, 1, Color.Cyan);
            debugPreviousFrameHitBox = new MockedDebugHitBox(map.Game, 2, Color.DarkOliveGreen);
            debugDistanceBox = new MockedDebugHitBox(map.Game, 3, Color.DodgerBlue);
            debugAttackBox = new MockedDebugHitBox(map.Game, 1, Color.Red);
        }
    }
}
