using System.Numerics;
using ldtk;
using Raylib_cs;

namespace GameLibrary;

public class Tilemap
{
    public LdtkJson LdtkJson;
    public Texture2D[] TileTextures;
    
    public Tilemap(string jsonPath)
    {
        LdtkJson = LdtkJson.FromJson(File.ReadAllText(Game.Dir + jsonPath));
        
        TileTextures = LdtkJson.Levels[0].LayerInstances.Select(e =>
        {
            return Raylib.LoadTexture(Game.Dir + Path.GetFileName(e.TilesetRelPath));
        }).ToArray();
    }

    public void Draw()
    {
        for (int i = LdtkJson.Levels[0].LayerInstances.Length-1; i >= 0; i--) // sneaky reverse for loop!
        {
            foreach (var tile in LdtkJson.Levels[0].LayerInstances[i].AutoLayerTiles)
            {
                int tID = (int)tile.T;
                int columns = TileTextures[i].Width / 16;
                Raylib.DrawTextureRec(TileTextures[i], new Rectangle((tID % columns) * 16, (tID / columns) * 16, 16, 16), new Vector2((int)tile.Px[0], (int)tile.Px[1]), Color.White);
            }
        }
    }

    ~Tilemap() // destructor, prevents gpu memory leak if game loads levels more than once.
    {
        Game.LateActions.Enqueue(() =>
        {
            foreach (Texture2D tex in TileTextures)
            {
                Raylib.UnloadTexture(tex);
            }
        });
    }
}