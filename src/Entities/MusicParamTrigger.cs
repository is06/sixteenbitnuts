using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public class MusicParamTrigger : EventTrigger
    {
        public string? ParamName { get; set; }
        public float? ParamValue { get; set; }

        public MusicParamTrigger(Map map, string name) : base(map, name)
        {
            
        }

        public override string MapTextDescription
        {
            get
            {
                return "en " + GetType().Name +
                       " " + Name +
                       " " + Position.X +
                       " " + Position.Y +
                       " " + Size.Width +
                       " " + Size.Height +
                       " " + ParamName +
                       " " + ParamValue;
            }
        }

        public void Trigger()
        {
            if (ParamName is string name && ParamValue is float value)
            {
                map.Game.SetMusicParameter(name, value);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("param_name", ParamName);
            info.AddValue("param_value", ParamValue);
        }
    }
}
