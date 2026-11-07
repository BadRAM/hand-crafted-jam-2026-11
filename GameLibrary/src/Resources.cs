using Raylib_cs;

namespace GameLibrary;

public static class Resources
{
    public static Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, SoundResource> Sounds = new Dictionary<string, SoundResource>();
    public static Dictionary<string, Music> Musics = new Dictionary<string, Music>();
    public static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
    public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
    public static Queue<Action> LateLoadActions = new Queue<Action>();
    private static bool _assetsLoaded = false;

    public static void Load()
    {
        Console.WriteLine("Detecting game files:");
        
        foreach (string path in Directory.GetFiles(Game.IsWeb ? "/" : Game.Dir, "*", SearchOption.AllDirectories))
        {
            Console.WriteLine("    " + path);
        }
        
        foreach (string spritePath in Directory.GetFiles(Game.Dir + "preload/sprite/", "*.png", SearchOption.AllDirectories))
        {
            Sprites.Add(Path.GetFileNameWithoutExtension(spritePath), new Sprite(spritePath));
            Console.WriteLine($"Sprites[{Path.GetFileNameWithoutExtension(spritePath)}]");
        }
        
        foreach (string spritePath in Directory.GetFiles(Game.Dir + "lateload/sprite/", "*.png", SearchOption.AllDirectories))
        {
            LateLoadActions.Enqueue(() =>
            {
                Sprites.Add(Path.GetFileNameWithoutExtension(spritePath), new Sprite(spritePath));
                Console.WriteLine($"Sprites[{Path.GetFileNameWithoutExtension(spritePath)}]");
            });
        }
        
        foreach (string soundPath in Directory.GetFiles(Game.Dir + "preload/sound/", "*", SearchOption.AllDirectories))
        {
            Sounds.Add(Path.GetFileNameWithoutExtension(soundPath), new SoundResource(soundPath));
            Console.WriteLine($"Sounds[{Path.GetFileNameWithoutExtension(soundPath)}]");
        }
        
        foreach (string soundPath in Directory.GetFiles(Game.Dir + "lateload/sound/", "*", SearchOption.AllDirectories))
        {
            LateLoadActions.Enqueue(() =>
            {
                Sounds.Add(Path.GetFileNameWithoutExtension(soundPath), new SoundResource(soundPath));
                Console.WriteLine($"Sounds[{Path.GetFileNameWithoutExtension(soundPath)}]");
            });
        }
        
        foreach (string musicPath in Directory.GetFiles(Game.Dir + "lateload/music/", "*", SearchOption.AllDirectories))
        {
            LateLoadActions.Enqueue(() =>
            {
                Musics.Add(Path.GetFileNameWithoutExtension(musicPath), Raylib.LoadMusicStream(musicPath));
                Console.WriteLine($"Musics[{Path.GetFileNameWithoutExtension(musicPath)}]");
            });
        }
        
        foreach (string fontPath in Directory.GetFiles(Game.Dir + "preload/font/", "*", SearchOption.AllDirectories))
        {
            Fonts.Add(Path.GetFileNameWithoutExtension(fontPath), Raylib.LoadFont(fontPath));
            Console.WriteLine($"Fonts[{Path.GetFileNameWithoutExtension(fontPath)}]");
        }
        
        // Fonts.Add("sd_auto_pilot", Raylib.LoadFontEx(Game.Dir + "font/sd_auto_pilot.ttf", 21, null, 0));
        // Fonts.Add("arcadepix", Raylib.LoadFontEx(Game.Dir + "font/arcadepix.ttf", 10, null, 0));
        
        foreach (string shaderPath in Directory.GetFiles(Game.Dir + "preload/shader/", "*", SearchOption.AllDirectories))
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

    public static void LateLoad()
    {
        if (LateLoadActions.Count == 0)
        {
            if (!_assetsLoaded)
            {
                Assets.Load();
                _assetsLoaded = true;
            }
            return;
        }
        LateLoadActions.Dequeue().Invoke();
    }
}