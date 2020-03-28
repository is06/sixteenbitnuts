using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public abstract class Entity : MapElement, IEntity, ISerializable
    {
        public string Name { get; private set; }
        public bool IsVisibleInPreMainDisplay { get; set; }
        public virtual string MapTextDescription
        {
            get
            {
                return "en " + GetType().Name +
                        " " + Name +
                        " " + Position.X +
                        " " + Position.Y;
            }
        }

        protected Texture2D? texture;
        protected Vector2 drawPosition;
        protected List<Movement> movements = new List<Movement>();

        public Entity(Map map, string name) : base(map)
        {
            Name = name;
            DebugColor = Color.Orange;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var movement in movements)
            {
                movement.Update(gameTime);
                Position += movement.Translation;
            }

            drawPosition = Position;

            base.Update(gameTime);
        }

        public void Destroy()
        {
            IsDestroying = true;
        }

        public void AddMovement(Movement movement)
        {
            movements.Add(movement);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("type", GetType().Name);
            info.AddValue("x", Position.X);
            info.AddValue("y", Position.Y);
        }
    }
}
