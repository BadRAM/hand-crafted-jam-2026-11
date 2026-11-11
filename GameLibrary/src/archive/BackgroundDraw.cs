using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace GameLibrary;

public static class BackgroundDraw
{
    private const int RingSize = 360;
    private static float[] _waveformRing = new float[RingSize];
    private static int _ringPos = 0;
    
    static BackgroundDraw()
    {
        Raylib.AttachAudioMixedProcessor(new AudioCallback<float>(buffer =>
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (i % 16 != 0) continue;
                _waveformRing[_ringPos] = buffer[i];
                _ringPos++;
                _ringPos %= RingSize;
            }
        }));
    }
    
    public static void Waveform4()
    {
        Vector2 pos = new Vector2(0, 360);
        Vector2 lastPos = pos;
        for (int i = 0; i < 360; i++)
        {
            float waveSample = _waveformRing[(i + _ringPos) % RingSize];
            pos = new Vector2(2 * i, MathF.Max(0, Easings.HalfSine(i / 360f)) * waveSample * 160f + 360f);
            Raylib.DrawLineEx(pos, lastPos, 2, Color.RayWhite);
            lastPos = pos;
        }
    }
    
    public static void Waveform1()
    {
        Vector2 pos = new Vector2(0, 360);
        Vector2 lastPos = pos;
        float pulse = Easings.HalfSine(MathF.Max(0, (Mixer.Beat() % 1) * 2f - 1f));
        float xOffset = Time.Scaled;
        for (int i = 0; i < 90; i++)
        {
            bool even = i % 2 == 0;
            pos = new Vector2(8 * i, MathF.Max(0, MathF.Sin(i * 0.2f - xOffset)) * (even ? 30f : -30f) * pulse + 360f);
            Raylib.DrawLineEx(pos, lastPos, 2, Color.RayWhite);
            lastPos = pos;
        }
    }
    
    public static void Waveform2()
    {
        Vector2 pos = new Vector2(0, 360);
        Vector2 lastPos = pos;
        // float pulse = float.Lerp(0.1f, 1f, Easings.HalfSine(MathF.Max(0, (Game.MusicPlaying.Beat() % 1) * 2f - 1f)));
        float pulse = Mixer.Pulse(0.3f) * Mixer.GetMusicVolume() * (Mixer.IsPaused ? 0 : 1);
        float xOffset = Time.Scaled;
        float f1 = 1.123f;
        float f2 = 0.6237f;
        float f3 = 0.21121f;
        float f4 = 0.081121f;
        for (int i = 0; i < 90; i++)
        {
            float sign = i % 2 == 0 ? 1f : -1f;
            float waveSample = 
                MathF.Sin(i * f1 + xOffset * 50) + 
                MathF.Sin(i * f2 + xOffset * -20) + 
                MathF.Sin(i * f3 + xOffset * 10) +
                MathF.Sin(i * f4 + xOffset * -3) * pulse;
            pos = new Vector2(8 * i, MathF.Max(0, Easings.HalfSine(i / 90f)) * waveSample * 40f * (pulse + 0.25f) + 360f);
            Raylib.DrawLineEx(pos, lastPos, 2, Color.RayWhite);
            lastPos = pos;
        }
    }
    
    public static void Waveform3()
    {
        if (Mixer.MusicPlaying == null) return;

        unsafe
        {
            Vector2 pos = new Vector2(0, 360);
            Vector2 lastPos = pos;
            for (int i = 0; i < 360; i++)
            {
                IntPtr rAudioBufferPtr = Mixer.MusicPlaying.Music.Stream.Buffer;
                var dataBufferPtr = *(IntPtr*)(rAudioBufferPtr + 368);
                int dataBufferIndex = Math.Min(4096 * 4, i * 4 * 2 * 4);
                float waveData = *(float*)(dataBufferPtr + dataBufferIndex);
                float waveSample = waveData;
                pos = new Vector2(2 * i, MathF.Max(0, Easings.HalfSine(i / 360f)) * waveSample * 80f + 360f);
                Raylib.DrawLineEx(pos, lastPos, 2, Color.RayWhite);
                lastPos = pos;
            }
        }
    }

    public static void Web(float alpha = 1f)
    {
        Resources.Sprites["radial"].Draw(0, 0, 720, 720, tint: Raylib.ColorAlpha(Color.Black, alpha));
        for (int i = 0; i < 8; i++)
        {
            Raylib.DrawCircleLines(360, 360, 45 * i, Color.Black);
        }
        Raylib.DrawLine(0, 0, 720, 720, Color.Black);
        Raylib.DrawLine(0, 720, 720, 0, Color.Black);
        Raylib.DrawLine(360, 0, 360, 720, Color.Black);
        Raylib.DrawLine(0, 360, 720, 360, Color.Black);
    }

    public static void Spiral(float alpha = 1f)
    {
        Sprite spiral = Resources.Sprites["spiral2"];
        spiral.DrawCentered(360, 360, 720, 720, rotation: -Time.Scaled * 180, tint: Raylib.ColorAlpha(Color.Black, alpha/2));
        Resources.Sprites["radial"].Draw(Vector2.Zero, new Vector2(720, 720), tint: Raylib.ColorAlpha(Color.Black, alpha));
    }
    
    public static void CirclePulse(float t)
    {
        t = Easings.OutQuad(t);
        Resources.Sprites["circle_soft"].DrawCentered(new Vector2(360, 360), new Vector2(820, 820) * t, tint: Raylib.ColorAlpha(Color.White, 1 - t));
    }

    public static void ChargeMeter(float t)
    {
        float height = 720 - t * 720;
        Color col = Raylib.ColorAlpha(Color.SkyBlue, 128);
        Resources.Sprites["charge_meter"].Draw(0, height, 720, 720, tint: col);
    }
}