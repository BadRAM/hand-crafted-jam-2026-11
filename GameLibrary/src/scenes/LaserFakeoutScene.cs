using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class LaserFakeoutScene : Scene
{
    private int _step;
    private AnimCurve<float> _stepProgress = new AnimCurve<float>(0);
    private Sound _portalOpenSound;
    private DialogueAsset _tunnelingRemark;
    private DialogueAsset _siteRemark1;
    private DialogueAsset _siteRemark2;
    private Sprite _siteImage1;
    private Sprite _siteImage2;

    public LaserFakeoutScene(DialogueAsset tunnelingRemark, DialogueAsset siteRemark1, DialogueAsset siteRemark2, Sprite siteImage1, Sprite siteImage2)
    {
        _tunnelingRemark = tunnelingRemark;
        _siteRemark1 = siteRemark1;
        _siteRemark2 = siteRemark2;
        _siteImage1 = siteImage1;
        _siteImage2 = siteImage2;
        _portalOpenSound = Resources.Sounds["portal_open"].GetSound();
    }
    
    public override void Update()
    {
        Raylib.ClearBackground(Color.DarkBlue);
        if (_step != 9)
        {
            BackgroundDraw.Spiral();
            BackgroundDraw.Waveform2();
            Resources.Sprites["mask"].DrawCentered(360, 360, 200, 200, tint: Color.Black );
        }


        if (_step == 0)
        {
            Mixer.ClearDialogue();
            _tunnelingRemark.Play(true);
            _stepProgress = AnimCurve.NewFloat(0, 1, 1);
            Game.MoveDevice(new Vector2(180, 540), 0.5f,  1);
            _step++;
        }
        else if (_step == 1)
        {
            if ((!Mixer.IsDialoguePlaying() && _stepProgress.IsComplete()))
            {
                Raylib.PlaySound(_portalOpenSound);
                Game.PortalView = _siteImage1;
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 2)
        {
            if (_stepProgress.IsComplete())
            {
                _siteRemark1.Play(true);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 3)
        {
            if (!Mixer.IsDialoguePlaying() && _stepProgress.IsComplete())
            {
                Game.ClosePortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 4)
        {
            if (_stepProgress.IsComplete())
            {
                Raylib.PlaySound(_portalOpenSound);
                Game.PortalView = _siteImage2;
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 5)
        {
            if (_stepProgress.IsComplete())
            {
                _siteRemark2.Play(true);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 6)
        {
            if (!Mixer.IsDialoguePlaying() && _stepProgress.IsComplete())
            {
                Resources.Sounds["laser"].Play(volume:1);
                Game.FireLaser = true;
                _stepProgress = AnimCurve.NewFloat(0, 1, 2);
                _step++;
            }
        }
        else if (_step == 7)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Raylib.StopSound(_portalOpenSound);
                Game.PortalView = Resources.Sprites["site_dead"];
                Resources.Sounds["portal_close"].Play(volume:1);
                _stepProgress = AnimCurve.NewFloat(1, 0, 1);
                _step++;
            }
        }
        else if (_step == 8)
        {
            Game.LaserGlare = _stepProgress.Sample();
            if (_stepProgress.IsComplete())
            {
                Game.ClosePortal();
                Game.FireLaser = false;
                _stepProgress = AnimCurve.NewFloat(0, 1, 2);
                _step++;
            }
        }
        else if (_step == 9)
        {
            BackgroundDraw.Spiral(1 - _stepProgress.Sample());
            BackgroundDraw.Web(_stepProgress.Sample());
            BackgroundDraw.Waveform2();
            Resources.Sprites["mask"].DrawCentered(360, 360, Vector2.One * 200 * (1 - _stepProgress.Sample()), tint: Color.Black );
            if (_stepProgress.IsComplete())
            {
                Mixer.PlayNextMusic();
                Game.MoveDevice(new Vector2(360, 360), 1, 1);
                Game.LevelIndex++;
                Game.ActiveScene = new MainMenu(false);
            }
        }
        
    }
}