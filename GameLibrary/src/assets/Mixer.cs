using Raylib_cs;

namespace GameLibrary;

public static class Mixer
{
    public static bool AutoPlay = false;
    public static bool IsPaused = false;
    public static List<MusicAsset> Playlist = new List<MusicAsset>();
    public static MusicAsset? MusicPlaying;
    private static AnimCurve<float> _musicFade = new AnimCurve<float>(FullVolume);
    public static List<DialogueAsset> DialogueQueue = new List<DialogueAsset>();
    public static DialogueAsset? DialoguePlaying;

    private const float FullVolume = 0.5f;
    private const float ReducedVolume = 0.1f;

    static Mixer()
    {
        // Playlist = new List<MusicAsset>()
        // {
        //     Assets.Musics["null_function"],
        //     Assets.Musics["av_adr"],
        //     Assets.Musics["andreas_v_avalanche"],
        // };
        Playlist = Assets.Musics.Values.ToList();
    }

    public static void Update()
    {
        if (DialoguePlaying != null && !DialoguePlaying.IsPlaying())
        {
            DialoguePlaying = null;
            _musicFade = AnimCurve.NewFloat(_musicFade.Sample(), FullVolume, 1);
        }

        if (DialoguePlaying == null && DialogueQueue.Count > 0)
        {
            if (_musicFade.IsComplete())
            {
                if (_musicFade.Sample() > ReducedVolume + 0.1f)
                {
                    _musicFade = AnimCurve.NewFloat(_musicFade.Sample(), ReducedVolume, 0.5f);
                }
                else
                {
                    DialoguePlaying = DialogueQueue[0];
                    Game.ClearTextScrolls();
                    Game.ScrollText(DialoguePlaying.Text);
                    DialoguePlaying.Sound.Play(volume: 1f);
                    DialogueQueue.RemoveAt(0);
                }
            }
        }
        
        if (MusicPlaying != null)
        {
            Raylib.SetMusicVolume(MusicPlaying.Music, _musicFade.Sample());

            if (!Raylib.IsMusicStreamPlaying(MusicPlaying.Music) && !IsPaused)
            {
                MusicPlaying = null;
            }
        }

        if (MusicPlaying == null && AutoPlay)
        {
            PlayNextMusic();
        }
        
        MusicPlaying?.Update();
    }
    
    public static void PlayMusic(MusicAsset music)
    {
        IsPaused = false;
        AutoPlay = true;
        _musicFade = AnimCurve.NewFloat(0, FullVolume, 1);
        MusicPlaying?.Stop();
        MusicPlaying = music;
        MusicPlaying.Play();
        Game.ScrollText($"Now Playing {MusicPlaying.ArtistName}: {MusicPlaying.Title}");
    }

    public static void Pause()
    {
        IsPaused = true;
        Raylib.PauseMusicStream(MusicPlaying.Music);
    }

    public static void Resume()
    {
        IsPaused = false;
        Raylib.ResumeMusicStream(MusicPlaying.Music);
    }

    public static void PlayNextMusic()
    {
        var music = new List<MusicAsset>(Playlist); // clone list so we can modify it without breaking referenced static lists.
        if (MusicPlaying != null) music.Remove(MusicPlaying); // Prevent shuffle from picking the song that was already playing.
        if (music.Count == 0) throw new Exception("No music :(");
    
        PlayMusic(music.PickRandom());
    }

    public static void PlayDialogue(DialogueAsset dialogue)
    {
        dialogue.HasBeenPlayed = true;
        DialogueQueue.Add(dialogue);
    }

    public static bool IsDialoguePlaying()
    {
        return (DialoguePlaying != null) || (DialogueQueue.Count > 0);
    }
    
    public static void ClearDialogue()
    {
        if (DialoguePlaying == null) return;
        Raylib.StopSound(DialoguePlaying.Sound.GetSound());
        Game.ClearTextScrolls();
        DialoguePlaying = null;
        DialogueQueue.Clear();
    }

    public static float GetMusicVolume() => _musicFade.Sample();
    
    public static float Beat() => MusicPlaying?.Beat() ?? 0;
    public static float Pulse(float width) => MusicPlaying?.Pulse(width) ?? 0;

    public static bool IsBeatThisFrame()
    {
        return MusicPlaying?.IsBeatThisFrame() ?? false;
    }
}