using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Collider
    {
        public Rectangle Bounds;

        private readonly DebugBox debugBox;

        public Collider(Game game, Point size)
        {
            Bounds = new Rectangle(Point.Zero, size);

            debugBox = new DebugBox(game, Bounds, Color.Lime);
        }

        public void Update()
        {
            debugBox.Bounds = Bounds;
            debugBox.Update();
        }

        public void DebugDraw(Matrix transform)
        {
            debugBox.Draw(transform);
        }

        /// <summary>
        /// Determines if the current actor collider is overlapping any solid from the map
        /// </summary>
        /// <param name="solids">List of solids to test</param>
        /// <param name="offset">An offset to apply to the overlapping test</param>
        /// <returns>The solid that is overlapping, null if no overlapping occurs</returns>
        public Solid? GetOverlappingSolid(List<Solid> solids, Point offset)
        {
            foreach (var solid in solids)
            {
                if (IsOverlappingWith(solid.Bounds, offset))
                {
                    return solid;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if the current actor collider is overlapping any entity in the current map section
        /// </summary>
        /// <param name="entities">Dictionnary of entities to test</param>
        /// <param name="offset">An offset to apply to the overlapping test</param>
        /// <returns>The entity that is overlapping, null if no overlapping occurs</returns>
        public Entity? GetOverlappingEntity(Dictionary<string, Entity> entities, Point offset)
        {
            foreach (var entity in entities)
            {
                if (IsOverlappingWith(entity.Value.Bounds, offset))
                {
                    return entity.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if the collider is overlapping with a rectangle
        /// </summary>
        /// <param name="other">The other rectangle</param>
        /// <param name="offset">An offset to apply to the overlapping test</param>
        /// <returns>True if rectangles are overlapping with the offset provided</returns>
        public bool IsOverlappingWith(Rectangle other, Point offset)
        {
            Rectangle offsetBounds = new Rectangle(Bounds.Location + offset, Bounds.Size);

            return offsetBounds.Intersects(other);
        }
    }
}
