using System.Drawing;
using System.Numerics;
using Raylib_cs;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace GameLibrary;

// Immediate mode GUI functions, similar to raygui.
public static class ImGui
{
    private static SoundResource _buttonPress;
    private static SoundResource _buttonRelease;
    private static Font _defaultFont;
    private static int _defaultFontSize = 40;
    private static int _defaultTextSpacing = 3;

    static ImGui()
    {
        _buttonPress = Resources.Sounds["metronome"];
        _buttonRelease = Resources.Sounds["metronome"];
        _defaultFont = Resources.Fonts["conthrax"];
        Raylib.SetTextureFilter(_defaultFont.Texture, TextureFilter.Bilinear);
    }

    public static void DrawText(string text, Vector2 position, int size = 20, Color? color = null)
    {
        color ??= Color.White;
        Raylib.DrawTextEx(_defaultFont, text, position, size, 1, color.Value);
    }

    public static void DrawText(string text, int x, int y, int size = 20, Color? color = null)
    {
        DrawText(text, new Vector2(x, y), size, color);
    }
    
    public static void DrawTextCentered(string text, Vector2 position, int size = 20, Color? color = null)
    {
        color ??= Color.White;
        Vector2 bounds = Raylib.MeasureTextEx(_defaultFont, text, size, _defaultTextSpacing);
        Raylib.DrawTextEx(_defaultFont, text, position - bounds/2, size, 1, color.Value);
    }

    public static void DrawTextCentered(string text, int x, int y, int size = 20, Color? color = null)
    {
        color ??= Color.White;
        Vector2 bounds = Raylib.MeasureTextEx(_defaultFont, text, size, _defaultTextSpacing);
        Raylib.DrawTextEx(_defaultFont, text, new Vector2(x, y) - bounds/2, size, 1, color.Value);
    }
    
    public static void DrawTextRadial(float angle, float radius, string text, float maxArc = 180, Color? color = null)
    {
        color ??= Color.White;
        Vector2 size = Raylib.MeasureTextEx(_defaultFont, text, _defaultFontSize, _defaultTextSpacing);
        Camera2D spin = new Camera2D
        {
            Target = new Vector2(0, 0),
            Offset = new Vector2(360, 360),
            Rotation = MeasureTextAngle(radius, text) / 2 + angle,
            Zoom = 1
        };
        
        for (int i = 0; i < text.Length; i++)
        {
            string character = text.Substring(i, 1);
            float width = Raylib.MeasureTextEx(_defaultFont, character, _defaultFontSize, _defaultTextSpacing).X;
            float rotateBy = (180 * (width + _defaultTextSpacing)) / (MathF.PI * radius);
            spin.Rotation -= rotateBy / 2;
            float alpha = Math.Clamp((maxArc / 2 - MathF.Abs(spin.Rotation)) * 0.25f, 0f, 1f);
            if (alpha > 0)
            {
                Game.SetCamera(spin);
                Raylib.DrawTextCodepoint(_defaultFont, character[0], new Vector2(-width/2, radius - size.Y/2), _defaultFontSize, Raylib.ColorAlpha(color.Value, alpha));
            }
            spin.Rotation -= rotateBy / 2;
        }
        Game.SetCamera();
    }

    public static float MeasureTextAngle(float radius, string text)
    {
        Vector2 size = Raylib.MeasureTextEx(_defaultFont, text, _defaultFontSize, _defaultTextSpacing);
        return (180 * size.X) / (MathF.PI * radius);
    }
}