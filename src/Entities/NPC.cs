﻿using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public abstract class NPC : Entity, ISerializable
    {
        protected string subType;

        public NPC(Map map, string name) : base(map, name)
        {
            subType = "default";
        }

        public override string MapTextDescription
        {
            get
            {
                return "en " + GetType().Name +
                        " " + Name +
                        " " + Position.X +
                        " " + Position.Y +
                        " " + subType;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("sub_type", subType);
        }
    }
}
