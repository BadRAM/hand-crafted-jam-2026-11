using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class EndingScene : Scene
{
    private int _step;
    private AnimCurve<float> _stepProgress = new AnimCurve<float>(0);
    private Sound _portalOpenSound;

    public EndingScene()
    {
        _portalOpenSound = Resources.Sounds["portal_open"].GetSound();
    }
    
    public override void Update()
    {
        Raylib.ClearBackground(Color.DarkBlue);
        if (_step <= 1)
        {
            BackgroundDraw.Spiral();
            BackgroundDraw.Waveform2();
            Resources.Sprites["mask"].DrawCentered(360, 360, 200, 200, tint: Color.Black );
        }
        else
        {
            Resources.Sprites["bsod"].Draw(0, 0);
        }

        if (_step == 0)
        {
            Mixer.Pause();
            Mixer.ClearDialogue();
            Assets.Dialogues[38].Play(true);
            _stepProgress = AnimCurve.NewFloat(0, 1, 1);
            _step++;
        }
        else if (_step == 1)
        {
            if ((!Mixer.IsDialoguePlaying() && _stepProgress.IsComplete()))
            {
                Game.MoveDevice(new Vector2( 90, 630), 0.25f, 5);
                
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 2)
        {
            if (_stepProgress.IsComplete())
            {
                Game.PortalView = Resources.Sprites["site_dead"];
                Raylib.PlaySound(_portalOpenSound);
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 3)
        {
            if (_stepProgress.IsComplete())
            {
                Resources.Sounds["throb"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 4)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Raylib.StopSound(_portalOpenSound);
                Game.PortalView = Resources.Sprites["site1"];
                Resources.Sounds["portal_close"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(1, 0, 1);
                _step++;
            }
        }
        else if (_step == 5)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Game.ClosePortal();
                Game.FireLaser = false;
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 6)
        {
            if (_stepProgress.IsComplete())
            {
                Game.PortalView = Resources.Sprites["site_dead"];
                Raylib.PlaySound(_portalOpenSound);
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 7)
        {
            if (_stepProgress.IsComplete())
            {
                Resources.Sounds["throb"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 8)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Raylib.StopSound(_portalOpenSound);
                Game.PortalView = Resources.Sprites["site2"];
                Resources.Sounds["portal_close"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(1, 0, 1);
                _step++;
            }
        }
        else if (_step == 9)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Game.ClosePortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 10)
        {
            if (_stepProgress.IsComplete())
            {
                Game.PortalView = Resources.Sprites["site_dead"];
                Raylib.PlaySound(_portalOpenSound);
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 11)
        {
            if (_stepProgress.IsComplete())
            {
                Resources.Sounds["throb"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 12)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Raylib.StopSound(_portalOpenSound);
                Game.PortalView = Resources.Sprites["site3"];
                Resources.Sounds["portal_close"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(1, 0, 1);
                _step++;
            }
        }
        else if (_step == 13)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Game.ClosePortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 14)
        {
            if (_stepProgress.IsComplete())
            {
                Game.PortalView = Resources.Sprites["site5"];
                Raylib.PlaySound(_portalOpenSound);
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 15)
        {
            if (_stepProgress.IsComplete())
            {
                // Resources.Sounds["throb"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 16)
        {
            // Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Raylib.StopSound(_portalOpenSound);
                Game.PortalView = Resources.Sprites["site5"];
                Resources.Sounds["portal_close"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(1, 0, 1);
                _step++;
            }
        }
        else if (_step == 17)
        {
            // Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Game.ClosePortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 18)
        {
            if (_stepProgress.IsComplete())
            {
                Game.PortalView = Resources.Sprites["site_dead"];
                Raylib.PlaySound(_portalOpenSound);
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 19)
        {
            if (_stepProgress.IsComplete())
            {
                Resources.Sounds["throb"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 20)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Raylib.StopSound(_portalOpenSound);
                Game.PortalView = Resources.Sprites["site4"];
                Resources.Sounds["portal_close"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(1, 0, 1);
                _step++;
            }
        }
        else if (_step == 21)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Game.ClosePortal();
                Game.MoveDevice(new Vector2(360, 360), 1,     5);
                _step++;
            }
        }
    }
}