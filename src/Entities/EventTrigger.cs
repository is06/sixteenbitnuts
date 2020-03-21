using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public class EventTrigger : Entity, ISerializable
    {
        public bool IsEnabled { get; set; }

        public EventTrigger(Map map, string name) : base(map, name)
        {

        }

        public override void Draw()
        {
            // Empty draw method to prevent drawing a null texture
            // (we don't need to draw an event trigger after all)
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("type", GetType().Name);
            info.AddValue("x", Position.X);
            info.AddValue("y", Position.Y);
            info.AddValue("w", Size.Width);
            info.AddValue("h", Size.Height);
        }
    }
}
