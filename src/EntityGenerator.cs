using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public class EntityGenerator
    {
        private readonly Map map;
        private int counter;

        public EntityGenerator(Map map)
        {
            this.map = map;
        }

        public void Add(Entity entity, Vector2 position, string tag)
        {
            var name = "generated_" + counter;

            entity.Name = name;
            entity.Tag = tag;
            entity.Position = position;

            map.CurrentMapSection.Entities.Add(name, entity);
            counter++;
        }

        public Dictionary<string, IEntity> GetAllByTag(string tag)
        {
            return map.GetEntitiesFromTag(tag);
        }

        public void DestroyAllByTag(string tag)
        {
            foreach (var entity in map.GetEntitiesFromTag(tag))
            {
                entity.Value.Destroy();
            }
        }
    }
}
