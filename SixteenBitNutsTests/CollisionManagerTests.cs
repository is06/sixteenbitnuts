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
            BoundingBox first = new BoundingBox(
                new Vector3(0, 0, 0),
                new Vector3(5, 5, 0)
            );
            BoundingBox second = new BoundingBox(
                new Vector3(10, 0, 0),
                new Vector3(15, 5, 0)
            );

            Assert.AreEqual(10, CollisionManager.GetDistance(first, second));
        }

        [TestMethod]
        public void TestGetCollisionSideTopIntersection()
        {
            BoundingBox moving = new BoundingBox(
                new Vector3(0, 0, 0),
                new Vector3(5, 5, 0)
            );
            BoundingBox stopped = new BoundingBox(
                new Vector3(0, 6, 0),
                new Vector3(5, 11, 0)
            );
            Vector2 movingVelocity = new Vector2(0, 0.5f);

            CollisionSide actualSide = CollisionManager.GetCollisionSide(moving, stopped, movingVelocity);

            Assert.AreEqual(CollisionSide.Top, actualSide);
        }

        [TestMethod]
        public void TestGetCollisionSideAlreadyIntersecting()
        {
            BoundingBox moving = new BoundingBox(
                new Vector3(0, 0, 0),
                new Vector3(5, 5, 0)
            );
            BoundingBox stopped = new BoundingBox(
                new Vector3(0, 4, 0),
                new Vector3(5, 9, 0)
            );
            Vector2 movingVelocity = new Vector2(0, 0.5f);

            CollisionSide actualSide = CollisionManager.GetCollisionSide(moving, stopped, movingVelocity);

            Assert.AreEqual(CollisionSide.None, actualSide);
        }
    }
}
