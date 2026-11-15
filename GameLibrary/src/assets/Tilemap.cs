using System.Numerics;
using GameLibrary.Ldtk;
using Raylib_cs;

namespace GameLibrary;

public class Tilemap
{
    public LdtkJson LdtkJson;
    public Sprite[] TileTextures;
    public int tileSize = 16;
    
    public Tilemap(string jsonPath)
    {
        LdtkJson = LdtkJson.FromJson(File.ReadAllText(Game.Dir + jsonPath));

        TileTextures = LdtkJson.Levels[0].LayerInstances.Select(e =>
        {
            string tileSheetId = LdtkJson.Defs.Tilesets.First(t => t.Uid == e.TilesetDefUid).Identifier;
            if (Resources.Sprites.TryGetValue(tileSheetId, out Sprite sprite))
            {
                return sprite;
            }
            return Resources.Sprites["fire1"];
        }).ToArray();

        tileSize = (int)LdtkJson.Defs.Layers[0].GridSize;
    }
    
    public void Draw()
    {
        for (int i = LdtkJson.Levels[0].LayerInstances.Length-1; i >= 0; i--) // sneaky reverse for loop!
        {
            foreach (var tile in LdtkJson.Levels[0].LayerInstances[i].AutoLayerTiles)
            {
                int tID = (int)tile.T;
                int columns = TileTextures[i].Width / 16;
                Raylib.DrawTextureRec(TileTextures[i].Texture, new Rectangle((tID % columns) * 16, (tID / columns) * 16, 16, 16), new Vector2((int)tile.Px[0], (int)tile.Px[1]), Color.White);
            }
        }
    }

    public Int2D PosToTilePos(Vector2 position)
    {
        int x = (int)(position.X / tileSize);
        int y = (int)(position.Y / tileSize);
        return new Int2D(x, y);
    }

    public Rectangle TileRect(Int2D tile)
    {
        return new Rectangle(tile.X * tileSize, tile.Y * tileSize, tileSize, tileSize);
    }

    public Vector2 TileCenter(Int2D tile)
    {
        return new Vector2(tile.X * tileSize + tileSize / 2f, tile.Y * tileSize + tileSize / 2f);
    }

    public int TileCode(Int2D tile)
    {
        return (int)LdtkJson.Levels[0].LayerInstances[0].IntGridCsv[tile.X + tile.Y * LdtkJson.Levels[0].LayerInstances[0].CWid];
    }

    public CollisionSideFlag MoveAndCollide(GameEntity entity)
    {
        int subSteps = 4;
        
        Vector2 pos = entity.Position + entity.Velocity * Time.DeltaTime;
        Int2D tile = PosToTilePos(pos);

        CollisionSideFlag result = 0;
        for (int i = 0; i < subSteps; i++)
        {
            entity.Position += entity.Velocity * Time.DeltaTime / subSteps;
            result |= CollideWithTile(tile + Int2D.Down, entity);
            result |= CollideWithTile(tile + Int2D.Up, entity);
            result |= CollideWithTile(tile + Int2D.Left, entity);
            result |= CollideWithTile(tile + Int2D.Right, entity);
            result |= CollideWithTile(tile + Int2D.Right + Int2D.Up, entity);
            result |= CollideWithTile(tile + Int2D.Right + Int2D.Down, entity);
            result |= CollideWithTile(tile + Int2D.Left + Int2D.Up, entity);
            result |= CollideWithTile(tile + Int2D.Left + Int2D.Down, entity);
        }

        return result;
    }

    private CollisionSideFlag CollideWithTile(Int2D tile, GameEntity entity)
    {
        if (TileCode(tile) == 0) return CollisionSideFlag.None;
        if (!Raylib.CheckCollisionRecs(entity.BoundingBox(), TileRect(tile))) return CollisionSideFlag.None;
        Vector2 tileCenter = TileCenter(tile);
        Vector2 delta = entity.Position - tileCenter;
        if (Math.Abs(delta.X) > Math.Abs(delta.Y))
        {
            if (delta.X > 0)
            {
                entity.Position.X = tileCenter.X + tileSize / 2f + entity.Size.X / 2f;
                entity.Velocity.X = Math.Max(0, entity.Velocity.X);
                return CollisionSideFlag.Left;
            }
            else
            {
                entity.Position.X = tileCenter.X - tileSize / 2f - entity.Size.X / 2f;
                entity.Velocity.X = Math.Min(0, entity.Velocity.X);
                return CollisionSideFlag.Right;
            }
        }
        else
        {
            if (delta.Y < 0)
            {
                entity.Position.Y = tileCenter.Y - tileSize / 2f - entity.Size.Y / 2f;
                entity.Velocity.Y = Math.Min(0, entity.Velocity.Y);
                return CollisionSideFlag.Down;
            }
            else
            {
                entity.Position.Y = tileCenter.Y + tileSize / 2f + entity.Size.Y / 2f;
                entity.Velocity.Y = Math.Max(0, entity.Velocity.Y);
                return CollisionSideFlag.Up;
            }
        }
    }

    [Flags]
    public enum CollisionSideFlag
    {
        None  = 0,
        Left  = 0b0001,
        Right = 0b0010,
        Up    = 0b0100,
        Down  = 0b1000
    }
}