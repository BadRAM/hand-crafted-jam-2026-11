using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class AnimCurve<T>
{
    public T Start;
    public T End;
    public float Duration;
    public Func<float, float> Easing;
    public Func<T, T, float, T> Lerp;
    public float StartTime;

    // This AnimCurve is a placeholder that always returns the endpoint.
    // Useful for initializing things that will be animated later.
    // To make a working AnimCurve, use one of the static factory methods.
    public AnimCurve(T end)
    {
        Start = end;
        End = end;
        Duration = 0;
        Easing = _ => 1f;
        Lerp = (_, e, _) => e;
        StartTime = 0;
    }
    
    // This constructor is not designed for convenience. When possible, use the static factory methods instead.
    public AnimCurve(T start, T end, float? startTime, float duration, Func<float, float>? easing, Func<T, T, float, T> lerp)
    {
        Start = start;
        End = end;
        StartTime = startTime ?? Time.Scaled;
        Duration = duration;
        Easing = easing ?? Easings.Linear;
        Lerp = lerp;
    }
    
    // Core functionality
    public T Sample(float time) => Lerp.Invoke(Start, End, Progress(time));
    public T Sample() => Sample(Time.Scaled);
    
    public float Progress(float time) => (Duration == 0) ? 1f : Easing.Invoke(Math.Clamp((time - StartTime) / Duration, 0, 1));
    public float Progress() => Progress(Time.Scaled);

    public float ProgressUnEased(float time) => (Duration == 0) ? 1f : Math.Clamp((time - StartTime) / Duration, 0, 1);
    public float ProgressUnEased() => ProgressUnEased(Time.Scaled);
    
    public bool IsComplete(float time) => time - StartTime > Duration;
    public bool IsComplete() => IsComplete(Time.Scaled);
}

// Static factory methods
public static class AnimCurve
{
    public static AnimCurve<float> NewFloat(float start, float end, float duration, Func<float, float>? easing = null, float? startTime = null) 
        => new AnimCurve<float>(start, end, startTime, duration, easing, float.Lerp);
    
    public static AnimCurve<Vector2> NewVector2(Vector2 start, Vector2 end, float duration, Func<float, float>? easing = null, float? startTime = null) 
        => new AnimCurve<Vector2>(start, end, startTime, duration, easing, Vector2.Lerp);
    
    public static AnimCurve<Rectangle> NewRectangle(Rectangle start, Rectangle end, float duration, Func<float, float>? easing = null, float? startTime = null) 
        => new AnimCurve<Rectangle>(start, end, startTime, duration, easing, LerpRectangle);
    
    public static AnimCurve<Camera2D> NewCamera2D(Camera2D start, Camera2D end, float duration, Func<float, float>? easing = null, float? startTime = null) 
        => new AnimCurve<Camera2D>(start, end, startTime, duration, easing, LerpCamera2D);

    private static Rectangle LerpRectangle(Rectangle a, Rectangle b, float t)
    {
        Rectangle result = new Rectangle()
        {
            Position = Vector2.Lerp(a.Position, b.Position, t),
            Size = Vector2.Lerp(a.Size, b.Size, t),
        };
        return result;
    }
    
    private static Camera2D LerpCamera2D(Camera2D a, Camera2D b, float t)
    {
        Camera2D result = new Camera2D()
        {
            Target = Vector2.Lerp(a.Target, b.Target, t),
            Offset = Vector2.Lerp(a.Offset, b.Offset, t),
            Rotation = float.Lerp(a.Rotation, b.Rotation, t),
            Zoom = float.Lerp(a.Zoom, b.Zoom, t),
        };
        return result;
    }
}
