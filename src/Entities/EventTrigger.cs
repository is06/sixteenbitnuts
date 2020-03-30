using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public abstract class EventTrigger : Entity, ISerializable
    {
        public bool IsEnabled { get; set; }

        public EventTrigger(Map map, string name) : base(map, name)
        {

        }

        public override void Draw(Matrix transform)
        {
            // Empty draw method to prevent drawing a null texture
            // (we don't need to draw an event trigger after all)
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("w", Size.Width);
            info.AddValue("h", Size.Height);
        }
    }
}
