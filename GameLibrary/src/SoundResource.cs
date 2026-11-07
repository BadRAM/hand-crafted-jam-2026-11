using Raylib_cs;

namespace GameLibrary;

public class SoundResource
{
    private List<Sound> _soundBuffer;
    private int _bufferIndex;
    private int _lastPlayed;

    public SoundResource(string path)
    {
        _soundBuffer = new List<Sound>();
        _soundBuffer.Add(Raylib.LoadSound(path));
        Raylib.SetSoundVolume(_soundBuffer[0], 0.75f);
    }
    
    public void Play(float pan = 0.5f, float pitch = 1f, float volume = 0.75f)
    {
        if (_lastPlayed == Time.Frame) return;
        if (Raylib.IsSoundPlaying(_soundBuffer[_bufferIndex])) // Extend buffer on overlap
        {
            if (_soundBuffer.Count >= 32) return;
            if (_bufferIndex == 0) _bufferIndex++;
            _soundBuffer.Insert(_bufferIndex, Raylib.LoadSoundAlias(_soundBuffer[0]));
        }
        Raylib.SetSoundPan(_soundBuffer[_bufferIndex], pan);
        Raylib.SetSoundPitch(_soundBuffer[_bufferIndex], pitch);
        Raylib.SetSoundVolume(_soundBuffer[_bufferIndex], volume);
        Raylib.PlaySound(_soundBuffer[_bufferIndex]);
        _bufferIndex++;
        _bufferIndex %= _soundBuffer.Count;
        _lastPlayed = Time.Frame;
    }

    public void PlayRandomPitch(float pan = 0.5f, float pitch = 1f, float volume = 0.75f)
    {
        Play(pan, pitch * (0.8f + Random.Shared.NextSingle() * 0.4f), volume);
    }

    public Sound GetSound()
    {
        return _soundBuffer[0];
    }

    public bool IsPlaying()
    {
        foreach (Sound sound in _soundBuffer)
        {
            if (Raylib.IsSoundPlaying(sound)) return true;
        }
        return false;
    }

    ~SoundResource()
    {
        Game.LateActions.Enqueue(() => Raylib.UnloadSound(_soundBuffer[0]));
    }
}