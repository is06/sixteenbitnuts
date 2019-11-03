using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixteenBitNutsTests.Mocks;

namespace SixteenBitNutsTests
{
    [TestClass]
    public class CollisionTests
    {
        [TestMethod]
        public void TestMapCollision()
        {
            var game = new MockedGame();
            var map = new MockedMap(game, "test_map");
            map.Update(new GameTime());

            Vector2 expected = new Vector2(20, 0);
            Vector2 actual = map.Player.Position;

            Assert.AreEqual(expected, actual);
        }
    }
}
