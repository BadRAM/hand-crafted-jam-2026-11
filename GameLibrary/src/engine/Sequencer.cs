namespace GameLibrary;

public class Sequencer
{
    public Interpolator<float> Progress;
    public int Step;

    public Sequencer(float firstDuration)
    {
        Progress = new Interpolator<float>(0, 1, firstDuration);
    }

    public bool CanAdvance(int step)
    {
        return Step == step && Progress.IsComplete();
    }
    
    public void Advance(float nextDuration)
    {
        Step++;
        Progress = new Interpolator<float>(0, 1, nextDuration);
    }
    
    public bool TryAdvance(int step, float nextDuration)
    {
        if (!CanAdvance(step)) return false;
        Advance(nextDuration);
        return true;
    }

    public void SkipTo(int step, float nextDuration)
    {
        Step = step;
        Progress = new Interpolator<float>(0, 1, nextDuration);
    }
}