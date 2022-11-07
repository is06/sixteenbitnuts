using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class EntityFactory
    {
        public virtual Entity CreateEntity(Map map, string type, string name, Point position, string[] chunks)
        {
            switch (type)
            {
                case "SpawnPoint":
                    return new SpawnPoint(map, name)
                    {
                        Position = position
                    };
                default:
                    throw new EngineException("No entity of type " + type + " has been handled in EntityFactory");
            }
        }
    }
}
