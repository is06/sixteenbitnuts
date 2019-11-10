using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixteenBitNutsTests.Mocks;

namespace SixteenBitNutsTests
{
    [TestClass]
    public class CollisionTests
    {
        private MockedGame game;
        private GameTime gameTime;
        private MockedMap map;

        [TestInitialize]
        public void Initialize()
        {
            game = new MockedGame();
            gameTime = new GameTime();
            map = new MockedMap(game, "test_map");
        }

        [TestMethod]
        public void PlayerShouldBeAtPositionZeroOnStart()
        {
            map.Update(gameTime);

            Assert.AreEqual(new Vector2(0, 0), map.Player.Position);
        }

        [TestMethod]
        public void PlayerShouldBeStoppedWhenFallingOnAnObstacle()
        {
            map.LoadMockFromFile("player_stopped_on_obstacle");

            map.Update(gameTime);
            map.Update(gameTime);

            Assert.AreEqual(new Vector2(64, 40), map.Player.Position);
        }

        [TestMethod]
        public void PlayerShouldBeStoppedWhenRunningAgainstAWall()
        {

        }

        [TestCleanup]
        public void Cleanup()
        {
            game.Dispose();
        }
    }
}
