using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Hud
    {
        public Game Game { get; private set; }

        public Dictionary<string, HudElement> Elements;

        public Hud(Game game)
        {
            Game = game;
            Elements = new Dictionary<string, HudElement>();
        }

        public virtual void Initialize()
        {
            foreach (var element in Elements)
            {
                element.Value.Initialize();
            }
        }

        public virtual void LoadContent()
        {
            foreach (var element in Elements)
            {
                element.Value.LoadContent();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var element in Elements)
            {
                element.Value.Update(gameTime);
            }
        }

        public virtual void Draw(Matrix transform)
        {
            foreach (var element in Elements)
            {
                element.Value.Draw(transform);
            }
        }

        public virtual void DebugDraw(Matrix transform)
        {
            foreach (var element in Elements)
            {
                element.Value.DebugDraw(transform);
            }
        }

        public virtual void UnloadContent()
        {
            foreach (var element in Elements)
            {
                element.Value.UnloadContent();
            }
        }
    }
}
