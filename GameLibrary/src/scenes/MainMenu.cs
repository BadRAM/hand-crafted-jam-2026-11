using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class MainMenu : Scene
{
    private Music _menuMusic = Resources.Musics["null_function"];
    
    private List<Font> _testFonts = new List<Font>();
    private AnimCurve<float> _loadAnim = new AnimCurve<float>(1);
    private AnimCurve<float> _menuMoveAnim = new AnimCurve<float>(0);
    
    public MainMenu(bool fromBlack)
    {
        Mixer.Playlist = Assets.Musics.Values.ToList();
        if (fromBlack) _loadAnim = AnimCurve.NewFloat(0, 1, 1);
        Assets.Dialogues[1].PlayIfUnheard();

        if (Game.LevelIndex == 5)
        {
            Assets.Dialogues[30].PlayIfUnheard(true);
        }
    }
    
    public override void Update()
    {
        // if (Raylib.IsKeyPressed(KeyboardKey.One  )) Game.MoveDevice(new Vector2(360, 360), 1,     1);
        // if (Raylib.IsKeyPressed(KeyboardKey.Two  )) Game.MoveDevice(new Vector2( 90,  90), 0.25f, 1);
        // if (Raylib.IsKeyPressed(KeyboardKey.Three)) Game.MoveDevice(new Vector2( 90, 630), 0.25f, 1);
        // if (Raylib.IsKeyPressed(KeyboardKey.Four )) Game.MoveDevice(new Vector2(180, 540), 0.5f,  1);
        // if (Raylib.IsKeyPressed(KeyboardKey.Five )) Game.MoveDevice(new Vector2(240, 480), 0.66f,  1);
        // if (Raylib.IsKeyPressed(KeyboardKey.G    )) Game.ActiveScene = new GameScene();
        if (Raylib.IsKeyPressed(KeyboardKey.H    )) Game.Level.TargetReachedAction.Invoke();
        if (Raylib.IsKeyPressed(KeyboardKey.N    )) Game.LevelIndex++;
        // if (Raylib.IsKeyPressed(KeyboardKey.K    )) Game.OpenPortal();
        // if (Raylib.IsKeyPressed(KeyboardKey.L    )) Game.ClosePortal();

        if (!Mixer.IsDialoguePlaying()) Mixer.AutoPlay = true;
        
        Raylib.ClearBackground(Color.DarkBlue);
        
        BackgroundDraw.Web();
        
        if (Raylib.IsKeyDown(KeyboardKey.A)) Raylib.ClearBackground(new Color(32, 32, 32, 255));
        
        BackgroundDraw.CirclePulse(Math.Max(0, Mixer.Beat() / 4 - 0.5f) % 1);
        BackgroundDraw.CirclePulse(Math.Max(0, Mixer.Beat() / 4 - 0.0f) % 1);
        BackgroundDraw.Waveform2();
        
        Resources.Sprites["cd"].DrawCentered(360, 360, Resources.Sprites["cd"].Size/2, rotation: Time.Scaled * 60);
        
        // if (Game.DebugMode && Mixer.MusicPlaying != null)
        // {
        //     if (Mixer.IsBeatThisFrame())
        //     {
        //         Resources.Sounds["metronome"].Play(volume:0.5f);
        //     }
        //
        //     if (Raylib.IsKeyPressed(KeyboardKey.Equal)) Mixer.MusicPlaying.FirstBeat += 0.01f;
        //     if (Raylib.IsKeyPressed(KeyboardKey.Minus)) Mixer.MusicPlaying.FirstBeat -= 0.01f;
        //     if (Raylib.IsKeyPressed(KeyboardKey.Enter)) Game.ScrollText(Mixer.MusicPlaying.Title + " " + Mixer.MusicPlaying.FirstBeat.ToString("N3"));
        // }

        DrawRadialMenu();

        ImGui.DrawTextRadial(0, 280, Game.Level.Message);

        if (_loadAnim.Sample() < 1) Raylib.DrawCircle(360, 360, 350, Raylib.ColorAlpha(Game.ScreenBlack, 1 - _loadAnim.Sample()));
        
        Raylib.UpdateMusicStream(_menuMusic);
    }

    private void DrawRadialMenu()
    {
        Camera2D spin = new Camera2D
        {
            Target = new Vector2(0, 0),
            Offset = new Vector2(360, 360),
            Rotation = _menuMoveAnim.Sample(),
            Zoom = 1
        };
        Vector2 pos = new Vector2(0, -150);
        Vector2 mPos = Raylib.GetMousePosition();
        
        Raylib.BeginMode2D(spin);
        bool hovered = _menuMoveAnim.IsComplete() && Raylib.CheckCollisionPointCircle(Raylib.GetScreenToWorld2D(mPos, spin), pos, 48);
        int rot = (int)spin.Rotation % 360 / 90;
        if (hovered)
        {
            Game.MouseCursor = MouseCursor.PointingHand;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                RotateTo(rot);
                if (rot == 0)
                {
                    Resources.Sounds["menu_click"].Play();
                    if (Mixer.MusicPlaying != null)
                    {
                        if (Mixer.IsPaused)
                        {
                            Mixer.Resume();
                        }
                        else
                        {
                            Mixer.Pause();
                        }
                    }
                    else
                    {
                        Mixer.PlayNextMusic();
                    }
                }
            }
        }
        Resources.Sprites[!Mixer.IsPaused ? "icon_pause" : "icon_play"].DrawCentered(pos, tint: hovered ? Color.White : new Color(192, 192, 192, 255));
        Raylib.EndMode2D();
        if (hovered && rot == 0) ImGui.DrawTextRadial(0, -200, !Mixer.IsPaused ? "Pause" : "Play");
        spin.Rotation += 90;
        Raylib.BeginMode2D(spin);
        rot = (int)spin.Rotation % 360 / 90;
        hovered = _menuMoveAnim.IsComplete() && Raylib.CheckCollisionPointCircle(Raylib.GetScreenToWorld2D(mPos, spin), pos, 48);
        if (hovered)
        {
            Game.MouseCursor = MouseCursor.PointingHand;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                RotateTo(rot);
                if (rot == 0)
                {
                    Resources.Sounds["menu_click"].Play();
                    Mixer.PlayNextMusic();
                }
            }
        }
        Resources.Sprites["icon_next"].DrawCentered(pos, tint: hovered ? Color.White : new Color(192, 192, 192, 255));
        Raylib.EndMode2D();
        if (hovered && rot == 0) ImGui.DrawTextRadial(0, -200, "Next");
        spin.Rotation += 90;
        Raylib.BeginMode2D(spin);
        rot = (int)spin.Rotation % 360 / 90;
        hovered = _menuMoveAnim.IsComplete() && Raylib.CheckCollisionPointCircle(Raylib.GetScreenToWorld2D(mPos, spin), pos, 48);
        if (hovered)
        {
            Game.MouseCursor = MouseCursor.PointingHand;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                RotateTo(rot);
                if (rot == 0)
                {
                    Game.ActiveScene = new GameScene();
                }
            }
        }
        Resources.Sprites["icon_charge"].DrawCentered(pos, tint: hovered ? Color.White : new Color(192, 192, 192, 255));
        Raylib.EndMode2D();
        if (hovered && rot == 0) ImGui.DrawTextRadial(0, -200, "Charge");
        spin.Rotation += 90;
        Raylib.BeginMode2D(spin);
        rot = (int)spin.Rotation % 360 / 90;
        hovered = _menuMoveAnim.IsComplete() && Raylib.CheckCollisionPointCircle(Raylib.GetScreenToWorld2D(mPos, spin), pos, 48);
        if (hovered)
        {
            Game.MouseCursor = MouseCursor.PointingHand;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                RotateTo(rot);
                if (rot == 0)
                {
                    Resources.Sounds["menu_click"].Play();
                    Assets.Dialogues[51].PlayIfUnheard();
                }
            }
        }
        Resources.Sprites["icon_snooping"].DrawCentered(pos, tint: hovered ? Color.White : new Color(192, 192, 192, 255));
        Raylib.EndMode2D();
    }

    private void RotateTo(int option)
    {
        if (option < 0) option += 4;
        if (option == 1)
        {
            Resources.Sounds["menu_left"].Play();
            _menuMoveAnim = AnimCurve.NewFloat(_menuMoveAnim.Sample(), _menuMoveAnim.Sample() - 90, 0.5f, Easings.InOutSine);
        }
        else if (option == 2)
        {
            Resources.Sounds["menu_right"].Play();
            _menuMoveAnim = AnimCurve.NewFloat(_menuMoveAnim.Sample(), _menuMoveAnim.Sample() + 180, 0.5f, Easings.InOutSine);
        }
        else if (option == 3)
        {
            Resources.Sounds["menu_left"].Play();
            _menuMoveAnim = AnimCurve.NewFloat(_menuMoveAnim.Sample(), _menuMoveAnim.Sample() + 90, 0.5f, Easings.InOutSine);
        }
    }
}