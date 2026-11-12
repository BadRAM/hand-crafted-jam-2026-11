namespace GameLibrary;

// This is called EggTimer instead of Timer so that it doesn't compete with System.Timer or nestphalia.Time in autocomplete.
// Encapsulates a simple timer pattern that can be used to tell if Duration has passed since StartTick,
// or what percentage of that interval has elapsed.
public class EggTimer
{
    public int StartTick;
    public int TickDuration;
    public bool UnscaledTime;

    public EggTimer(int tick, int tickDuration, bool unscaledTime = false)
    {
        StartTick = tick;
        TickDuration = tickDuration;
        UnscaledTime = unscaledTime;
    }

    public EggTimer(int tickDuration, bool unscaledTime = false)
    {
        StartTick = Time.Tick;
        TickDuration = tickDuration;
        UnscaledTime = unscaledTime;
    }

    public EggTimer(float duration, bool unscaledTime = false)
    {
        StartTick = Time.Tick;
        TickDuration = (int)Math.Floor(duration * Time.FrameRate);
        UnscaledTime = unscaledTime;
    }

    // This creates a 'disabled' timer that will never* finish
    // *may finish after 414 days
    public EggTimer()
    {
        StartTick = 0;
        TickDuration = int.MaxValue;
        UnscaledTime = false;
    }

    public bool IsComplete() => GetTime() - StartTick >= TickDuration;
    public float Progress() => Math.Clamp(ProgressUnclamped(), 0f, 1f);
    public float ProgressUnclamped() => (float)(GetTime() - StartTick) / TickDuration;
    public int ElapsedTicks() => GetTime() - StartTick;
    public float ElapsedSeconds() => (float)ElapsedTicks() / Time.FrameRate;
    public int RemainingTicks() => TickDuration - ElapsedTicks();
    public float RemainingSeconds() => Math.Max(0, (float)RemainingTicks() / Time.FrameRate);

    private int GetTime() => UnscaledTime ? Time.Frame : Time.Tick;
}