using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    /// <summary>
    /// Entity generator service available in every maps
    /// </summary>
    public class EntityGenerator
    {
        private readonly Map map;
        private int counter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map"></param>
        public EntityGenerator(Map map)
        {
            this.map = map;
        }

        /// <summary>
        /// Adds a tagged entity to the map
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <param name="tag"></param>
        public void Add(Entity entity, Vector2 position, string tag)
        {
            var name = "gen_entity_" + counter;

            entity.Name = name;
            entity.Tag = tag;
            entity.Position = position;

            map.CurrentMapSection.Entities.Add(name, entity);
            counter++;
        }

        /// <summary>
        /// Retrieve all entities that match the specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Dictionary<string, IEntity> GetAllByTag(string tag)
        {
            return map.GetEntitiesFromTag(tag);
        }

        /// <summary>
        /// Destroy all entities that match the specified tag
        /// </summary>
        /// <param name="tag"></param>
        public void DestroyAllByTag(string tag)
        {
            foreach (var entity in map.GetEntitiesFromTag(tag))
            {
                entity.Value.Destroy();
            }
        }
    }
}
