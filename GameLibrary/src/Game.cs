using Raylib_cs;
using System.Numerics;

namespace GameLibrary;

public static class Game
{
    public static IAppContainer AppContainer;
    public static Scene ActiveScene;
    public static Queue<Action> LateActions = new Queue<Action>(); // LateActions are dequeued and invoked after everything else has updated.
    // public static Level Level => Assets.Levels[LevelIndex];
    
    public static bool IsWeb;
    public static bool DebugMode;
    public static bool ShouldQuit;
    public static string Dir = "";
    public static MouseCursor MouseCursor;
    
    // Target Resolution: 720x720
    private static Camera2D _defaultCamera = new Camera2D(new Vector2(0, 0), Vector2.Zero, 0, 1);
    private static Camera2D _activeCamera = _defaultCamera;
    
    private static ConfigFlags _defaultFlags;
    
    public static void Load(bool isWeb)
    {
        IsWeb = isWeb;
        Raylib.SetConfigFlags(_defaultFlags);
        Raylib.InitWindow(720, 720, "Cool Game :)");
        Raylib.SetTargetFPS(Time.FrameRate);
        Raylib.InitAudioDevice();
        Raylib.SetExitKey(KeyboardKey.Null);
        
        Resources.Load();
        
        ActiveScene = new MainMenu(true);
        // ActiveScene = new IntroScene();
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Blank);
        Raylib.EndDrawing();
    }
    
    public static void Update()
    {
        Time.UpdateTime();
        if (!IsWeb && Raylib.IsKeyPressed(KeyboardKey.F3)) DebugMode = !DebugMode;
        MouseCursor = MouseCursor.Default;
        
        Mixer.Update();
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Blank);
        SetCamera();
        
        ActiveScene.Update();
        
        while (LateActions.Count > 0) LateActions.Dequeue().Invoke();
        
        Raylib.EndMode2D();
        _activeCamera = _defaultCamera;
        
        if (Time.Frame == 1)
        {
            Raylib.ClearBackground(Color.Blank);
        }
        
        Raylib.DrawFPS(0, 0);
        
        Raylib.EndDrawing();
        
        Raylib.SetMouseCursor(MouseCursor);
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

    public static Camera2D GetActiveCamera() => _activeCamera;
}
