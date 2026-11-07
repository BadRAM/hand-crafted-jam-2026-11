using Raylib_cs;

namespace GameLibrary;

public class MusicAsset
{
    public Music Music;
    public string Title;
    public string ArtistName;
    public float BeatsPerMinute;
    public float FirstBeat;
    private float _beatLastUpdate;
    private bool _beatThisFrame;

    public MusicAsset(Music music, string title, string artistName, float beatsPerMinute, float firstBeat)
    {
        Music = music;
        Music.Looping = false;
        Title = title;
        ArtistName = artistName;
        BeatsPerMinute = beatsPerMinute;
        FirstBeat = firstBeat;
    }

    public void Update()
    {
        Raylib.UpdateMusicStream(Music);

        _beatThisFrame = (int)Beat() != (int)_beatLastUpdate;
        _beatLastUpdate = Beat();
    }

    public void Play()
    {
        Raylib.SetMusicVolume(Music, 0);
        Raylib.PlayMusicStream(Music);
    }

    public void Stop()
    {
        Raylib.StopMusicStream(Music);
    }

    public float Beat() => (Raylib.GetMusicTimePlayed(Music) - FirstBeat) * (BeatsPerMinute / 60);

    public float Pulse(float width) => Easings.Map(MathF.Max(width, MathF.Abs(Beat() % 1 - 0.5f) * 2), width, 1f, 0f, 1f);

    public bool IsBeatThisFrame()
    {
        return _beatThisFrame;
    }
}