using System;

namespace SixteenBitNuts.Interfaces
{
    public interface IAudioManager : IDisposable
    {
        void Initialize();
        void LoadContent();
        void Update();
        void PlayMusic(string name);
        void SetMusicParameter(string name, float value);
        void PlaySound(string name);
        void UnloadContent();
    }
}
