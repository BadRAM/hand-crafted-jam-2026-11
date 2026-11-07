using GameLibrary;
using Raylib_cs;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Game.Dir = Directory.GetCurrentDirectory() + "/resource/";
        
        Game.Load(false);
        
        while (!Raylib.WindowShouldClose() && !Game.ShouldQuit)
        {
            Game.Update();
        }

        Raylib.CloseWindow();
    }
}