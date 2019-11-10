using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace SixteenBitNutsTests
{
    [TestClass]
    public class CollisionManagerTests
    {
        [TestMethod]
        public void TestDistance()
        {
            var first = new HitBox(
                new Vector2(0, 0),
                new Vector2(5, 5)
            );
            var second = new HitBox(
                new Vector2(10, 0),
                new Vector2(5, 5)
            );

            Assert.AreEqual(10, CollisionManager.GetDistance(first, second));
        }

        [TestMethod]
        public void TestGetCollisionSideTopIntersection()
        {
            var moving = new HitBox(
                new Vector2(0, 0),
                new Vector2(5, 5)
            );
            var stopped = new HitBox(
                new Vector2(0, 6),
                new Vector2(5, 5)
            );
            Vector2 movingVelocity = new Vector2(0, 0.5f);

            CollisionSide actualSide = CollisionManager.GetCollisionSide(moving, stopped, movingVelocity);

            Assert.AreEqual(CollisionSide.Top, actualSide);
        }

        [TestMethod]
        public void TestGetCollisionSideAlreadyIntersecting()
        {
            var moving = new HitBox(
                new Vector2(0, 0),
                new Vector2(5, 5)
            );
            var stopped = new HitBox(
                new Vector2(0, 4),
                new Vector2(5, 5)
            );
            Vector2 movingVelocity = new Vector2(0, 0.5f);

            CollisionSide actualSide = CollisionManager.GetCollisionSide(moving, stopped, movingVelocity);

            Assert.AreEqual(CollisionSide.None, actualSide);
        }
    }
}
