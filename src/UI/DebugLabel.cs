using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    public class DebugLabel : Label
    {
        public DebugLabel(Scene scene) : base(scene)
        {
            try
            {
                font = scene.Game.Content.Load<SpriteFont>("EngineGraphics/Fonts/nes");
            }
            catch (Exception e)
            {
                throw new GameException("Exception while loading the console font (" + e.Message + ")");
            }
        }
    }
}
