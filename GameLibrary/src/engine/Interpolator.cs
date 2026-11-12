using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class Interpolator<T>
{
    public T StartValue;
    public T EndValue;
    public float StartTime;
    public float Duration;
    public Func<float, float> Easing;
    public Func<T, T, float, T> Lerp;

    public float EndTime => StartTime + Duration;

    private static readonly Dictionary<Type, Delegate> _lerps = new Dictionary<Type, Delegate>()
    {
        { typeof(float), float.Lerp },
        { typeof(Vector2), (Func<Vector2, Vector2, float, Vector2>)Vector2.Lerp },
        { typeof(Vector3), (Func<Vector3, Vector3, float, Vector3>)Vector3.Lerp },
        { typeof(Rectangle), Utils.LerpRectangle },
        { typeof(Camera2D), Utils.LerpCamera2D }
    };

    // This Constructor is a placeholder that always returns the endpoint.
    // Useful for initializing things that will be animated later.
    public Interpolator(T endValue)
    {
        StartValue = endValue;
        EndValue = endValue;
        StartTime = 0;
        Duration = 0;
        Easing = _ => 1f;
        Lerp = (_, e, _) => e;
    }
    
    public Interpolator(T startValue, T endValue, float duration, float? startTime = null, Func<float, float>? easing = null, Func<T, T, float, T>? lerp = null)
    {
        StartValue = startValue;
        EndValue = endValue;
        Duration = duration;
        StartTime = startTime ?? Time.Scaled;
        Easing = easing ?? Easings.Linear;
        Lerp = lerp;
        if (Lerp == null)
        {
            if (_lerps.TryGetValue(typeof(T), out var value)) { Lerp = (Func<T, T, float, T>)value; }
            else { throw new Exception($"Type: {typeof(T)} has no default lerp function, either add one to the Interpolator _lerps dictionary or supply one to the constructor"); }
        }
    }
    
    // Core functionality
    public T Sample(float time) => Lerp.Invoke(StartValue, EndValue, Progress(time));
    public T Sample() => Sample(Time.Scaled);
    
    public T SampleUnclamped(float time) => Lerp.Invoke(StartValue, EndValue, ProgressUnclamped(time));
    public T SampleUnclamped() => SampleUnclamped(Time.Scaled);
    
    public float Progress(float time) => (Duration == 0) ? 1f : Easing.Invoke(Math.Clamp((time - StartTime) / Duration, 0, 1));
    public float Progress() => Progress(Time.Scaled);
    
    public float ProgressUnclamped(float time) => Duration == 0 ? 1f : Easing.Invoke((time - StartTime) / Duration);
    public float ProgressUnclamped() => ProgressUnclamped(Time.Scaled);
    
    public float ProgressUnEased(float time) => (Duration == 0) ? 1f : Math.Clamp((time - StartTime) / Duration, 0, 1);
    public float ProgressUnEased() => ProgressUnEased(Time.Scaled);
    
    public bool IsComplete(float time) => time - StartTime > Duration;
    public bool IsComplete() => IsComplete(Time.Scaled);
}