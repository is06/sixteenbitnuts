using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    public interface IAuthoringTool : IDisposable
    {
        void Initialize();
        void Draw(GameTime gameTime);
    }
}
