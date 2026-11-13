using Raylib_cs;

namespace GameLibrary;

// Game data assets (things like enemy stats, level names, etc.) can just be hard coded constants in this file. 
public static class Assets
{
    public static Dictionary<string, MusicAsset> Musics = new Dictionary<string, MusicAsset>();
    public static List<Level> Levels;
    
    public static void Load()
    {
        Musics = new Dictionary<string, MusicAsset>()
        {
        };
        
        Levels = new List<Level>()
        {
        };
        
        Console.WriteLine("All Assets loaded OK!");
    }
}