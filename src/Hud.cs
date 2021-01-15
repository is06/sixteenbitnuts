using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Hud
    {
        public Game Game { get; private set; }
        public Dictionary<string, HudElement> Elements
        {
            get
            {
                return elements;
            }
        }

        private readonly Dictionary<string, HudElement> elements;

        public Hud(Game game)
        {
            Game = game;
            elements = new Dictionary<string, HudElement>();
        }

        public void AddElement(string name, HudElement element)
        {
            elements.Add(name, element);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var element in elements)
            {
                element.Value.Update(gameTime);
            }
        }

        public virtual void Draw(Matrix transform)
        {
            foreach (var element in elements)
            {
                element.Value.Draw(transform);
            }
        }

        public virtual void DebugDraw(Matrix transform)
        {
            foreach (var element in elements)
            {
                element.Value.DebugDraw(transform);
            }
        }
    }
}
