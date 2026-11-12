using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class IntroScene : Scene
{
    private List<Interpolator<float>> _pulses = new List<Interpolator<float>>();
    private Interpolator<float> _nextPulse = new Interpolator<float>(0);
    private Interpolator<float> _pulseInterval;
    private Sequencer _sequencer = new Sequencer(0);
    private bool _aPressed;
    private bool _cPressed;
    private bool _dPressed;
    
    public IntroScene()
    {
        Game.MoveDevice(new Vector2(464, 110), 0.15f, 0);
    }
    
    public override void Update()
    {
        DoPulses();

        if (Raylib.IsKeyPressed(KeyboardKey.G)) _sequencer.Progress = new Interpolator<float>(1);
        
        if (Raylib.IsKeyPressed(KeyboardKey.A)) _aPressed = true;
        if (Raylib.IsKeyPressed(KeyboardKey.C)) _cPressed = true;
        if (Raylib.IsKeyPressed(KeyboardKey.D)) _dPressed = true;
        
        if (_aPressed && _cPressed && _dPressed && _sequencer.Step < 3)
        {
            Game.MoveDevice(new Vector2(360, 360), 1, 2, Easings.InOutSine);
            _sequencer.SkipTo(3, 1);
        }
        
        Raylib.ClearBackground(_sequencer.Step >= 4 ? Game.ScreenBlack : Color.Black);
        
        if (_sequencer.Step == 0)
        {
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(287, 410, 146, 50)) ||
                Raylib.CheckCollisionPointCircle(Game.GetCursorPosOnDevice(), new Vector2(360, 360), 360))
            {
                Game.MouseCursor = MouseCursor.PointingHand;
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    _sequencer.Advance(10);
                    _nextPulse = new Interpolator<float>(1);
                    _pulseInterval = new Interpolator<float>(3, 0.15f, 15);
                }
            }
        }
        if (_sequencer.Step == 1)
        {
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(287, 410, 146, 50)) ||
                Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), Game.GetDevicePos(), 360f * 0.15f))
            {
                Game.MouseCursor = MouseCursor.PointingHand;
            }
        }
        if (_sequencer.TryAdvance(1, 8))
        {
            Game.MoveDevice(new Vector2(360, 360), 1, 10, Easings.InOutQuad);
        }
        if (_sequencer.TryAdvance(2, 2))
        {
            _nextPulse = new Interpolator<float>(0);
        }
        if (_sequencer.TryAdvance(3, 4))
        {
            Resources.Sounds["startup2"].Play();
        }
        _sequencer.TryAdvance(4, 1);
        if (_sequencer.Step == 4)
        {
            Sprite logo = Resources.Sprites["logo"];
            logo.DrawCentered(360, 360, logo.Size/2);
            ImGui.DrawTextRadial(0, -120, "powered by");
            Raylib.DrawCircle(360, 360, 350, Raylib.ColorAlpha(Game.ScreenBlack, 1 - (_sequencer.Progress.Sample() * 4 - 1f)));
        }
        _sequencer.TryAdvance(5, 1);
        if (_sequencer.Step == 5)
        {
            Sprite logo = Resources.Sprites["logo"];
            logo.DrawCentered(360, 360, logo.Size/2);
            ImGui.DrawTextRadial(0, -120, "powered by");
            Raylib.DrawCircle(360, 360, 350, Raylib.ColorAlpha(Game.ScreenBlack, _sequencer.Progress.Sample()));
        }
        if (_sequencer.CanAdvance(6))
        {
            if (_aPressed && _dPressed && _cPressed)
            {
                Game.LevelIndex = 6;
                Game.ActiveScene = new GameScene();
            }
            else
            {
                Game.ActiveScene = new MainMenu(true);
            }
        }
    }

    private void DoPulses()
    {
        if (_nextPulse.Sample() >= 1)
        {
            _nextPulse = new Interpolator<float>(0, 1, _pulseInterval.Sample());
            Resources.Sounds["throb"].Play();
            _pulses.Add(new Interpolator<float>(0, 1, 3));
        }
        
        foreach (Interpolator<float> pulse in _pulses)
        {
            BackgroundDraw.CirclePulse(pulse.Sample());
        }
        _pulses.RemoveAll(p => p.IsComplete());
    }
}