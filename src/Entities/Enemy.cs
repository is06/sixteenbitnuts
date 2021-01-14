using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public abstract class Enemy : Entity, ISerializable
    {
        protected string subType;

        public override string MapTextDescription
        {
            get
            {
                return "en Enemy" +
                        " " + Name +
                        " " + Position.X +
                        " " + Position.Y +
                        " " + subType;
            }
        }

        public Enemy(Map map, string name) : base(map, name)
        {
            subType = "default";
            IsObstacle = false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("sub_type", subType);
        }
    }
}
