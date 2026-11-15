using System.Numerics;

namespace GameLibrary;

public record struct Int2D
{
    public static readonly Int2D Zero  = new Int2D(0, 0);
    public static readonly Int2D Up    = new Int2D(0, -1);
    public static readonly Int2D Down  = new Int2D(0, 1);
    public static readonly Int2D Left  = new Int2D(-1, 0);
    public static readonly Int2D Right = new Int2D(1, 0);
    public static readonly Int2D[] Cardinals = new[] { Up, Down, Left, Right };
    public static readonly Int2D[] Adjacents = new[] { Up, Down, Left, Right, new Int2D(-1, -1), new Int2D(1, -1), new Int2D(-1, 1), new Int2D(1, 1) };
    public int X;
    public int Y;

    public Int2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }

    public override String ToString()
    {
        return $"{X},{Y}";
    }
    
    public static Int2D operator +(Int2D a, Int2D b) {
        return new Int2D 
        {
            X = a.X + b.X,
            Y = a.Y + b.Y
        };
    }
    
    public static Int2D operator -(Int2D a, Int2D b) {
        return new Int2D 
        {
            X = a.X - b.X,
            Y = a.Y - b.Y
        };
    }

    public bool EqualsCoords(int x, int y)
    {
        return x == X && y == Y;
    }

    // Returns one of the cardinal directions. (up, down, left, right)
    public static Int2D RandomCardinal(Random? random = null)
    {
        random ??= Random.Shared;
        switch (random.Next(4))
        {
            case 0: return Up;
            case 1: return Down;
            case 2: return Left;
            case 3: return Right;
        }
        throw new Exception("random value out of range");
    }
    
    // Returns one of the 8 adjacent square offsets
    public static Int2D RandomAdjacent(Random? random = null)
    {
        random ??= Random.Shared;
        switch (random.Next(8))
        {
            case 0: return Up;
            case 1: return Down;
            case 2: return Left;
            case 3: return Right;
            case 4: return new Int2D(1, 1);
            case 5: return new Int2D(-1, 1);
            case 6: return new Int2D(1, -1);
            case 7: return new Int2D(-1, -1);
        }
        throw new Exception("random value out of range");
    }
}