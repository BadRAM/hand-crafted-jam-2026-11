using Raylib_cs;
using System.Numerics;

namespace GameLibrary;

public static class Game
{
    public static bool IsWeb;
    public static bool ShouldQuit;
    public static string Dir = "";
    public static Queue<Action> LateActions = new Queue<Action>(); // LateActions are dequeued and invoked after everything else has updated.
    public static Scene ActiveScene;
    public static Level Level => Assets.Levels[LevelIndex];
    public static int LevelIndex;
    public static MouseCursor MouseCursor;
    public static bool DebugMode;
    public static readonly Color ScreenBlack = new Color(8, 8, 8, 255);
    
    // Target Resolution: 720x720
    private static Camera2D _defaultCamera = new Camera2D(new Vector2(0, 0), Vector2.Zero, 0, 1);
    private static Camera2D _activeCamera = _defaultCamera;
    private static RenderTexture2D _screenRenderTarget;
    private static Shader _screenShader;
    private static int _screenShaderMaskLocation;

    private static RenderTexture2D _portalEdgeRenderTarget;
    private static RenderTexture2D _portalViewRenderTarget;
    public static AnimCurve<float> PortalSize = new AnimCurve<float>(0);
    public static Sprite PortalView;

    public static bool FireLaser;
    private static Vector2 _deviceShake;
    public static float LaserGlare;
    
    private static AnimCurve<Camera2D> _deviceCameraAnim = new AnimCurve<Camera2D>(new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1));
    
    private static ConfigFlags _defaultFlags = ConfigFlags.TransparentWindow | ConfigFlags.UndecoratedWindow | ConfigFlags.Msaa4xHint;
    private static Vector2 _lastWindowDelta;
    
    private static List<string> _scrollerTexts = new List<string>();
    private static float _scrollerAngle = 500;
    
    
    public static void Load(bool isWeb)
    {
        IsWeb = isWeb;
        Raylib.SetConfigFlags(_defaultFlags);
        Raylib.InitWindow(720, 720, "Cool Game :)");
        Raylib.SetTargetFPS(Time.FrameRate);
        Raylib.InitAudioDevice();
        Raylib.SetExitKey(KeyboardKey.Null);
        _screenRenderTarget = Raylib.LoadRenderTexture(720, 720);
        _portalViewRenderTarget = Raylib.LoadRenderTexture(720, 720);
        _portalEdgeRenderTarget = Raylib.LoadRenderTexture(720, 720);
        
        Resources.Load();
        _screenShader = Resources.Shaders["screen_fragment"];
        _screenShaderMaskLocation = Raylib.GetShaderLocation(_screenShader, "mask");

        PortalView = Resources.Sprites["site1"];
        
        ActiveScene = new IntroScene();
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Blank);
        Raylib.EndDrawing();
        
        // ScrollText("SCROLLING TEXT, ON A CIRCLE. ONLY POSSIBLE IN RAYLIB THROUGH BADRAM'S MAD SKILLS.");
        // ScrollText("DID YOU THINK I WAS DONE? I'VE ONLY BEGUN TO SCROLL MY TEXT!");
        // ScrollText("I'M A TEXT SCROLLING MACHINE!");
        // ScrollText("TEXT SCROLLS WILL RETURN.");
    }
    
    public static void Update()
    {
        Time.UpdateTime();
        Resources.LateLoad();
        
        // if (Raylib.IsKeyPressed(KeyboardKey.F3)) DebugMode = !DebugMode;
        
        MouseCursor = MouseCursor.Default;
        Vector2 mPos = GetCursorPosOnDevice();
        if (!Raylib.CheckCollisionPointCircle(mPos, new Vector2(360, 360), 360))
        {
            for (int i = 0; i < 4; i++)
            {
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(0, 640 + 23 * i, 720, 19)))
                {
                    MouseCursor = MouseCursor.IBeam;
                }
            }
        }
        // if (!Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), new Vector2(360, 360), 360))
        // {
        //     // :'(
        //     // Raylib.SetWindowState(ConfigFlags.MousePassthroughWindow);
        // }
        // else
        // {
        //     if (!Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), new Vector2(360, 360), 310))
        //     {
        //         HoverInteractable = true;
        //         if (Raylib.IsMouseButtonDown(MouseButton.Left))
        //         {
        //             _lastWindowDelta = Raylib.GetMouseDelta() + _lastWindowDelta;
        //             Vector2 winPos = Raylib.GetWindowPosition() + _lastWindowDelta;
        //             Raylib.SetWindowPosition((int)winPos.X, (int)winPos.Y);
        //         }
        //     }
        // }
        
        Mixer.Update();
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Blank);
        Resources.Sprites["fakebanner"].DrawCentered(360, 360);
        
        if (PortalSize.Sample() > 0) DrawPortal();
        
        Raylib.BeginTextureMode(_screenRenderTarget);
        Raylib.ClearBackground(Color.Black);
        SetCamera();
        
        ActiveScene.Update();
        if (_scrollerTexts.Count > 0)
        {
            ImGui.DrawTextRadial(_scrollerAngle, -280, _scrollerTexts[0], 160);
            _scrollerAngle -= 1;
            if (_scrollerAngle < -80 + -ImGui.MeasureTextAngle(280, _scrollerTexts[0]) / 2)
            {
                _scrollerTexts.RemoveAt(0);
                if (_scrollerTexts.Count > 0)
                {
                    _scrollerAngle = ImGui.MeasureTextAngle(280, _scrollerTexts[0]) / 2 + 80;
                }
            }
        }
        
        while (LateActions.Count > 0) LateActions.Dequeue().Invoke();
        
        Mask();
        Raylib.EndMode2D();
        Raylib.EndTextureMode();
        _activeCamera = _defaultCamera;

        _deviceCameraAnim.End.Offset -= _deviceShake;
        _deviceShake = Vector2.Zero;
        if (FireLaser) 
        {
            _deviceShake = Random.Shared.InsideUnitCircle() * 4;
            _deviceCameraAnim.End.Offset += _deviceShake;
            Vector2 laserShake = Random.Shared.InsideUnitCircle() * 4;
            Resources.Sprites["laser_halo"].Draw(laserShake, tint: Raylib.ColorFromHSV((Mixer.Beat()/2 * 360f) % 360f, 1f, 1f));
            Resources.Sprites["laser_core"].Draw(laserShake);
        }
        
        Raylib.BeginMode2D(_deviceCameraAnim.Sample());
        
        if (!DebugMode)
        {
            Raylib.BeginShaderMode(_screenShader);
            Raylib.SetShaderValueTexture(_screenShader, _screenShaderMaskLocation, Resources.Sprites["screen_mask_colored"].Texture);
            Raylib.DrawTextureRec(_screenRenderTarget.Texture, new Rectangle(0, 0, 720, -720), Vector2.Zero, Color.White);
            Raylib.EndShaderMode();
        }
        else
        {
            Raylib.DrawTextureRec(_screenRenderTarget.Texture, new Rectangle(0, 0, 720, -720), Vector2.Zero, Color.White);
        }
        
        DrawRing(Time.Scaled * 2, MathF.Sin(Time.Scaled / 2f) / 2f + 0.5f);
        
        Raylib.EndMode2D();
        
        Raylib.BeginBlendMode(BlendMode.CustomSeparate);
        Rlgl.SetBlendFactorsSeparate(Rlgl.ZERO, Rlgl.ONE, Rlgl.ONE, Rlgl.ZERO, Rlgl.FUNC_ADD, Rlgl.FUNC_ADD);
        Raylib.DrawRectangle(0, 0, 720, 720, Color.White);
        Raylib.EndBlendMode();

        if (Time.Frame == 1)
        {
            Raylib.ClearBackground(Color.Blank);
        }
        
        Raylib.EndDrawing();
        
        Raylib.SetMouseCursor(MouseCursor);
    }

    public static void MoveDevice(Vector2 center, float zoom, float duration, Func<float, float>? easing = null)
    {
        if (duration < 4 && Resources.Sounds.ContainsKey("whoosh"))
        {
            Resources.Sounds["whoosh"].Play();
        }
        _deviceCameraAnim = AnimCurve.NewCamera2D
        (
            _deviceCameraAnim.Sample(),
            new Camera2D(center, new Vector2(360, 360), 0, zoom), 
            duration, 
            easing ?? Easings.OutQuint
        );
    }
    
    public static bool IsDeviceMoving() => !_deviceCameraAnim.IsComplete();

    // angle is 0-360, tilt is 0-1
    private static void DrawRing(float angle, float tilt)
    {
        tilt = Math.Clamp(tilt, 0.01f, 0.99f);
        int frame1 = (int)MathF.Floor((tilt % 1) * 10);
        int frame2 = Math.Min(frame1 + 1, 9);
        float subframe = ((tilt % 1) * 10) % 1;
        Vector2 screen = new Vector2(720, 720);
        
        Resources.Sprites[$"glass_shine"].DrawCentered(screen/2, screen, rotation: -40);
        Resources.Sprites[$"ring{frame1}"].DrawCentered(screen/2, screen, rotation: angle);
        Resources.Sprites[$"ring{frame2}"].DrawCentered(screen/2, screen, rotation: angle, tint: Raylib.ColorAlpha(Color.White, subframe));
        
        // ImGui.DrawTextRadial(0, -240, $"F:{frame} S:{subframe:N2} A:{angle:N0}");
    }

    public static void Mask()
    {
        Raylib.BeginBlendMode(BlendMode.CustomSeparate);
        Rlgl.SetBlendFactorsSeparate(Rlgl.ZERO, Rlgl.ONE, Rlgl.ONE, Rlgl.ZERO, Rlgl.FUNC_ADD, Rlgl.FUNC_ADD);
        Resources.Sprites["mask"].Draw(0, 0);
        // Raylib.DrawRectangle(0, 0, 720, 720, Color.White);
        Raylib.EndBlendMode();
    }

    public static void DrawPortal()
    {
        Vector2 size = Vector2.One * PortalSize.Sample();
        Vector2 viewSize = size * 0.8f;
        
        Raylib.BeginTextureMode(_portalViewRenderTarget);
            PortalView.Draw(0, 0);
            Resources.Sprites["laser_splash"].DrawCentered(360, 360, Vector2.One * LaserGlare * 1440, tint: Raylib.ColorFromHSV((Mixer.Beat()/2 * 360f) % 360f, 1f, 1f));
            Resources.Sprites["laser_splash"].DrawCentered(360, 360, Vector2.One * LaserGlare * 720);
            Raylib.BeginBlendMode(BlendMode.CustomSeparate);
            Rlgl.SetBlendFactorsSeparate(Rlgl.ZERO, Rlgl.ONE, Rlgl.ONE, Rlgl.ZERO, Rlgl.FUNC_ADD, Rlgl.FUNC_ADD);
            Resources.Sprites["mask"].DrawCentered(360, 360, viewSize);
            
            Raylib.EndBlendMode();
        Raylib.EndTextureMode();
        
        Raylib.BeginTextureMode(_portalEdgeRenderTarget);
            Sprite fire = Resources.Sprites["fire1"];
            fire.DrawCentered(360, 360, 720, 720, new Rectangle(Time.Scaled * 10, 0, 512, 512), Time.Scaled);
            Raylib.BeginBlendMode(BlendMode.Additive);
            fire.DrawCentered(360, 360, 720, 720, new Rectangle(Time.Scaled * 10 + 128, 0, 512, 512), 120 + Time.Scaled);
            fire.DrawCentered(360, 360, 720, 720, new Rectangle(Time.Scaled * 10 + 256, 0, 512, 512), 240 + Time.Scaled);
            Raylib.EndBlendMode();
            Raylib.BeginBlendMode(BlendMode.CustomSeparate);
            Rlgl.SetBlendFactorsSeparate(Rlgl.ZERO, Rlgl.ONE, Rlgl.ONE, Rlgl.ZERO, Rlgl.FUNC_ADD, Rlgl.FUNC_ADD);
            Resources.Sprites["ringmask"].DrawCentered(360, 360, size);
            Raylib.EndBlendMode();
        Raylib.EndTextureMode();

        Rectangle viewSrc = new Rectangle(360 - viewSize.X/2 + 1, 360 - viewSize.Y/2 + 1, viewSize.X - 2, -viewSize.Y + 2);
        Rectangle viewDst = new Rectangle(360 - viewSize.X/2 + 1, 360 - viewSize.Y/2 + 1, viewSize.X - 2,  viewSize.Y - 2);
        if (FireLaser) viewDst.Position += Random.Shared.InsideUnitCircle() * 10;
        Raylib.DrawTexturePro(_portalViewRenderTarget.Texture, viewSrc, viewDst, Vector2.Zero, 0, Color.White);
        Rectangle edgeSrc = new Rectangle(360 - size.X/2 + 1, 360 - size.Y/2 + 1, size.X - 2, -size.Y + 2);
        Rectangle edgeDst = new Rectangle(360 - size.X/2 + 1, 360 - size.Y/2 + 1, size.X - 2,  size.Y - 2);
        Raylib.DrawTexturePro(_portalEdgeRenderTarget.Texture, edgeSrc, edgeSrc, Vector2.Zero, 0, Color.White);
    }
    
    public static void ScrollText(string text)
    {
        _scrollerTexts.Add(text);
        if (_scrollerTexts.Count == 1)
        {
            _scrollerAngle = ImGui.MeasureTextAngle(280, _scrollerTexts[0]) / 2 + 80;
        }
    }

    public static bool IsScrolling() => _scrollerTexts.Count > 0;

    public static void ClearTextScrolls()
    {
        _scrollerTexts.Clear();
    }

    public static Vector2 GetDevicePos()
    {
        Camera2D cam = _deviceCameraAnim.Sample();
        return cam.Offset;
    }
    
    public static void SetCamera(Camera2D? camera = null)
    {
        Camera2D cam = camera ?? _defaultCamera;
        
        if (_activeCamera.Offset == cam.Offset &&      // ReSharper disable once CompareOfFloatsByEqualityOperator
            _activeCamera.Zoom == cam.Zoom &&          // ReSharper disable once CompareOfFloatsByEqualityOperator
            _activeCamera.Rotation == cam.Rotation &&
            _activeCamera.Target == cam.Target)
        {
            return;
        }
        Raylib.EndMode2D();
        _activeCamera = cam;
        Raylib.BeginMode2D(_activeCamera);
    }

    public static Vector2 GetCursorPosOnDevice()
    {
        return Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), _deviceCameraAnim.Sample());
    }
    
    public static Camera2D GetActiveCamera() => _activeCamera;
    
    public static void OpenPortal()  => PortalSize = AnimCurve.NewFloat(0, 720, 1, Easings.OutQuart);
    public static void ClosePortal() => PortalSize = AnimCurve.NewFloat(720, 0, 1, Easings.InQuart);
}
