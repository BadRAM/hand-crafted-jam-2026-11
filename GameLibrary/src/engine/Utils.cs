using System.Numerics;
using Box2D.NET;
using Raylib_cs;

namespace GameLibrary;

// Designated dumping ground for extension methods and math functions
public static class Utils
{
    public static Vector2 Size(this Texture2D tex) => new(tex.Width, tex.Height);
    public static Rectangle Rect(this Texture2D tex) => new(Vector2.Zero, tex.Width, tex.Height);
    
    public static float MoveTowards(this float start, float target, float maxDistanceDelta)
    {
        if (Math.Abs(target - start) < maxDistanceDelta)
        {
            return target;
        } 
        return (start > target) ? (start - maxDistanceDelta) : (start + maxDistanceDelta);
    }

    public static Vector2 AngleLength(float angle, float length)
    {
        angle *= MathF.PI / 180;
        return new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * length;
    }
    
    public static B2Vec2 ToB2(this Vector2 vec)
    {
        return new B2Vec2(vec.X, vec.Y);
    }

    public static Vector2 ToVec2(this B2Vec2 vec)
    {
        return new Vector2(vec.X, vec.Y);
    }
    
    public static Color ToRaylib(this B2HexColor color)
    {
        int i = (int)color;        
        return new Color((i >> 16) & 0xFF, (i >> 8) & 0xFF, i & 0xFF, 255);
    }
    
    public static T PickRandom<T>(this List<T> list, Random? random = null)
    {
        random ??= Random.Shared;
        return list[random.Next(list.Count)];
    }
    
    public static Vector2 InsideUnitCircle(this Random random)
    {
        float theta = random.NextSingle() * 2 * Single.Pi;
        return new Vector2(MathF.Cos(theta), MathF.Sin(theta)) * MathF.Sqrt(random.NextSingle());
    }

    public static bool Compare(this Color c1, Color c2)
    {
        return c1.R == c2.R &&
               c1.G == c2.G &&
               c1.B == c2.B &&
               c1.A == c2.A;
    }
    
    public static Rectangle FlipX(this Rectangle rect)
    {
        rect.Width *= -1;
        return rect;
    }
    public static Rectangle FlipY(this Rectangle rect)
    {
        rect.Height *= -1;
        return rect;
    }
}