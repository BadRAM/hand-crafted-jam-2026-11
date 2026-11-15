using Raylib_cs;

namespace GameLibrary;

public static class Resources
{
    public static Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, SoundResource> Sounds = new Dictionary<string, SoundResource>();
    public static Dictionary<string, Music> Musics = new Dictionary<string, Music>();
    public static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
    public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
    private static bool _assetsLoaded = false;

    public static void Load()
    {
        Console.WriteLine("Detecting game files:");
        foreach (string path in Directory.GetFiles(Game.IsWeb ? "/" : Game.Dir, "*", SearchOption.AllDirectories))
        {
            Console.WriteLine("    " + path);
        }
        
        foreach (string spritePath in Directory.GetFiles(Game.Dir + "sprite/", "*.png", SearchOption.AllDirectories))
        {
            Sprites.Add(Path.GetFileNameWithoutExtension(spritePath), new Sprite(spritePath));
            Console.WriteLine($"Sprites[{Path.GetFileNameWithoutExtension(spritePath)}]");
        }
        
        foreach (string soundPath in Directory.GetFiles(Game.Dir + "sound/", "*", SearchOption.AllDirectories))
        {
            Sounds.Add(Path.GetFileNameWithoutExtension(soundPath), new SoundResource(soundPath));
            Console.WriteLine($"Sounds[{Path.GetFileNameWithoutExtension(soundPath)}]");
        }
        
        foreach (string musicPath in Directory.GetFiles(Game.Dir + "music/", "*", SearchOption.AllDirectories))
        {
            Musics.Add(Path.GetFileNameWithoutExtension(musicPath), Raylib.LoadMusicStream(musicPath));
            Console.WriteLine($"Musics[{Path.GetFileNameWithoutExtension(musicPath)}]");
        }
        
        foreach (string fontPath in Directory.GetFiles(Game.Dir + "font/", "*", SearchOption.AllDirectories))
        {
            Fonts.Add(Path.GetFileNameWithoutExtension(fontPath), Raylib.LoadFont(fontPath));
            Console.WriteLine($"Fonts[{Path.GetFileNameWithoutExtension(fontPath)}]");
        }
        
        foreach (string shaderPath in Directory.GetFiles(Game.Dir + "shader/", "*", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileNameWithoutExtension(shaderPath);
            string version = fileName.Substring(fileName.Length - 3);
            if ( Game.IsWeb && version != "100") continue;
            if (!Game.IsWeb && version != "330") continue;
            Console.WriteLine($"Loading shader: {shaderPath}" + $"\n{File.ReadAllText(shaderPath)}");
            Shaders.Add(fileName.Substring(0, fileName.Length - 4), Raylib.LoadShader("", shaderPath));
        }
        
        Console.WriteLine("All game files loaded OK!");
    }
}