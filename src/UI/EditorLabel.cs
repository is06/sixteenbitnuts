using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    class EditorLabel : Label
    {
        public EditorLabel(Scene scene) : base(scene)
        {
            try
            {
                font = scene.Game.Content.Load<SpriteFont>("Engine/fonts/nes");
            }
            catch (Exception e)
            {
                throw new GameException("Exception while loading the editor label font (" + e.Message + ")");
            }
        }
    }
}
