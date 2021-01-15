using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    [Serializable()]
    public abstract class Scene
    {
        public Game Game { get; private set; }
        public virtual bool HasPreMainDisplayEffectDraws { get; }

        public Scene(Game game)
        {
            Game = game;
        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw()
        {

        }

        public virtual void DebugDraw()
        {

        }

        public virtual void PreMainDisplayEffectDraw()
        {

        }

        public virtual void OutGameDraw()
        {

        }

        public virtual void UIDraw()
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual void EditCurrentSection()
        {
            
        }

        public virtual void EditLayout()
        {
            
        }
    }
}
