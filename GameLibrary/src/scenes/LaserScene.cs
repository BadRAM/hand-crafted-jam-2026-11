using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class LaserScene : Scene
{
    private int _step;
    private AnimCurve<float> _stepProgress = new AnimCurve<float>(0);
    private Sound _portalOpenSound;
    private DialogueAsset _tunnelingRemark;
    private DialogueAsset _siteRemark;
    private Sprite _siteImage;

    public LaserScene(DialogueAsset tunnelingRemark, DialogueAsset siteRemark, Sprite siteImage)
    {
        _tunnelingRemark = tunnelingRemark;
        _siteRemark = siteRemark;
        _siteImage = siteImage;
        _portalOpenSound = Resources.Sounds["portal_open"].GetSound();
    }
    
    public override void Update()
    {
        Raylib.ClearBackground(Color.DarkBlue);
        if (_step != 6)
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
                Game.PortalView = _siteImage;
                Game.OpenPortal();
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 2)
        {
            if (_stepProgress.IsComplete())
            {
                _siteRemark.Play(true);
                _stepProgress = AnimCurve.NewFloat(0, 1, 1);
                _step++;
            }
        }
        else if (_step == 3)
        {
            if (!Mixer.IsDialoguePlaying() && _stepProgress.IsComplete())
            {
                Resources.Sounds["laser"].Play(volume:1);
                Game.FireLaser = true;
                _stepProgress = AnimCurve.NewFloat(0, 1, 2);
                _step++;
            }
        }
        else if (_step == 4)
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
        else if (_step == 5)
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
        else if (_step == 6)
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