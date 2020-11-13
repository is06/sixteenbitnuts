using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace SixteenBitNuts
{
    class EditorLabel : Label
    {
        public EditorLabel(Scene scene) : base(scene)
        {
            try
            {
                font = scene.Game.Content.Load<SpriteFont>("EngineGraphics/Fonts/nes");
            }
            catch (Exception e)
            {
                if (e.InnerException is FileNotFoundException innerException)
                {
                    throw new GameException("Exception while loading the editor label font (" + innerException.FileName + " not found)");
                }
                throw new GameException("Exception while loading the editor label font (" + e.Message + ")");
            }
        }
    }
}
