using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class Sprite
{
    public Texture2D Texture;
    public Rectangle SourceRect;

    public int Width => (int)SourceRect.Width;
    public int Height => (int)SourceRect.Height;
    public Vector2 Size => SourceRect.Size;
    public Vector2 Center => SourceRect.Size / 2;

    public Sprite(string path)
    {
        Texture = Raylib.LoadTexture(path);
        Raylib.SetTextureFilter(Texture, TextureFilter.Bilinear);
        if (Texture.Width == 512 && Texture.Height == 512) // Bwahahahaha it's the last day I can do whatever I want!
        {
            Raylib.SetTextureWrap(Texture, TextureWrap.Repeat);
        }
        SourceRect = Texture.Rect();
    }

    public void Draw(Vector2 position, Vector2? size = null, Vector2? origin = null, Rectangle? source = null, float rotation = 0, Color? tint = null)
    {
        size ??= Size;
        origin ??= Vector2.Zero;
        source ??= SourceRect;
        tint ??= Color.White;
        Rectangle dest = new Rectangle(position, size.Value);
        Raylib.DrawTexturePro(Texture, source.Value, dest, origin.Value, rotation, tint.Value);
    }

    public void Draw(float x, float y, Vector2? size = null, Vector2? origin = null, Rectangle? source = null, float rotation = 0, Color? tint = null) 
        => Draw(new Vector2(x, y), size, origin, source, rotation, tint);
    public void Draw(float x, float y, float width, float height, Vector2? origin = null, Rectangle? source = null, float rotation = 0, Color? tint = null) 
        => Draw(new Vector2(x, y), new Vector2(width, height), origin, source, rotation, tint);
    
    public void DrawCentered(Vector2 position, Vector2? size = null, Rectangle? source = null, float rotation = 0, Color? tint = null)
    {
        size ??= Size;
        source ??= SourceRect;
        tint ??= Color.White;
        Rectangle dest = new Rectangle(position, size.Value);
        Raylib.DrawTexturePro(Texture, source.Value, dest, size.Value/2, rotation, tint.Value);
    }
    
    public void DrawCentered(float x, float y, Vector2? size = null, Rectangle? source = null, float rotation = 0, Color? tint = null)
        => DrawCentered(new Vector2(x, y), size, source, rotation, tint);
    public void DrawCentered(float x, float y, float width, float height, Rectangle? source = null, float rotation = 0, Color? tint = null)
        => DrawCentered(new Vector2(x, y), new Vector2(width, height), source, rotation, tint);
}