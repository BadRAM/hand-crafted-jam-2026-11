using GameLibrary;
using Raylib_cs;

public class Program : IAppContainer
{
    [STAThread]
    public static void Main()
    {
        Game.Dir = Directory.GetCurrentDirectory() + "/resource/";
        Game.AppContainer = new Program();
        
        Game.Load(false);
        
        while (!Raylib.WindowShouldClose() && !Game.ShouldQuit)
        {
            Game.Update();
        }

        Raylib.CloseWindow();
    }

    public void Alert(string message)
    {
    }

    public void SaveImageToDisk(Image image, string name)
    {
        Raylib.ExportImage(image, name + ".png");
    }
}