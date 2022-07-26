using System;

namespace SixteenBitNuts
{
    public interface IAudioManager : IDisposable
    {
        void Initialize();
        void LoadContent();
        void Update();
        void UnloadContent();
    }
}
