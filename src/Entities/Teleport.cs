﻿using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public class Teleport : Entity, ISerializable
    {
        public Vector2? DestinationPoint { get; set; }

        public Teleport(Map map, string name) : base(map, name)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (DestinationPoint is Vector2 destination && map.Player is Player player)
            {
                if (player.HitBox.Intersects(HitBox))
                {
                    player.Position = destination;
                }
            }
        }

        public override void Draw()
        {
            // Empty draw method to prevent drawing a null texture
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (DestinationPoint is Vector2 point)
            {
                info.AddValue("dx", point.X);
                info.AddValue("dy", point.Y);
            }
        }
    }
}
