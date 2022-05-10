using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public abstract class Map : Scene
    {
        public List<Solid> Solids { get; private set; }

        protected Player? player;

        public Map(Game game, string name, bool loadFromDefinitionFile = true) : base(game)
        {
            Solids = new List<Solid>();
        }

        public override void Initialize()
        {
            base.Initialize();

            player?.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            player?.LoadContent();
        }

        public override void Update()
        {
            base.Update();

            player?.Update();
        }

        public override void Draw()
        {
            base.Draw();

            player?.Draw(Matrix.Identity);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            player?.UnloadContent();
        }
    }
}
