using System.Numerics;
using Raylib_cs;

namespace GameLibrary;

public class GameScene : Scene
{
    public Tilemap Level;
    public List<GameEntity> GameEntities = new List<GameEntity>();
    private Player _player;
    
    public GameScene()
    {
        Level = new Tilemap("tilemap/gym.ldtk");
        _player = new Player();
        GameEntities.Add(_player);
        GameEntities[0].Position = new Vector2(100, 100);
        GameEntities.Add(new Block());
        GameEntities[1].Position = new Vector2(150, 150);
    }
    
    
    public override void Update()
    {
        // Move everything
        GameEntities.RemoveAll(e => e.Update(this));
        
        // Draw background
        Raylib.ClearBackground(Color.DarkBlue);
        
        Game.SetCamera(new Camera2D(Raylib.GetScreenCenter(), _player.Position, 0, 2));
        Level.Draw();
        
        // Draw Player
        foreach (GameEntity entity in GameEntities)
        {
            entity.Draw();
        }
    }
}

public abstract class GameEntity
{
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Size = new Vector2(14, 14);
    public GameEntity? HeldBy;
    
    public abstract bool Update(GameScene world);
    public abstract void Draw();
    
    public virtual Rectangle BoundingBox() => new Rectangle(Position - Size / 2, Size);
}

public class Player : GameEntity
{
    private float _groundAccel = 600;
    private float _airAccel = 300;
    private float _speed = 100;
    private float _jumpStrength = 160;
    private float _gravity = 250;
    public int _lastGroundedTick;
    private bool _grabPrompt;
    private GameEntity? _held;
    private bool _facingRight;
    private bool _bufferedGrab;
    
    public override bool Update(GameScene world)
    {
        Velocity.Y = Velocity.Y.MoveTowards(200, _gravity * Time.DeltaTime);
        float wishVel = 0;
        if (Input.Held(InputAction.Left))  wishVel -= _speed;
        if (Input.Held(InputAction.Right)) wishVel += _speed;
        Velocity.X = Velocity.X.MoveTowards(wishVel, (IsGrounded() ? _groundAccel : _airAccel) * Time.DeltaTime);
        if (Input.Pressed(InputAction.Primary) && IsGrounded()) Velocity.Y = -_jumpStrength;
        if (!Input.Held(InputAction.Primary) && Velocity.Y < 0) // Stop vertical speed quickly if jump released
        {
            Velocity.Y += _gravity * Time.DeltaTime * 3;
        }
        // Grab/throw stuff
        _grabPrompt = false;

        if (_held != null)
        {
            if (Input.Pressed(InputAction.Secondary))
            {
                _held.HeldBy = null;
                if (_facingRight)_held.Velocity = Velocity + new Vector2(100, -100);
                else             _held.Velocity = Velocity + new Vector2(-100, -100);
                _held = null;
            }
        }
        else
        {
            if (Input.Pressed(InputAction.Secondary))
            {
                _bufferedGrab = true;
            }

            if (!Input.Held(InputAction.Secondary))
            {
                _bufferedGrab = false;
            }
            
            foreach (GameEntity entity in world.GameEntities)
            {
                if (entity == this) continue;
                if (!Raylib.CheckCollisionRecs(BoundingBox(), entity.BoundingBox())) continue;
                _grabPrompt = true;
                if (_bufferedGrab)
                {
                    entity.HeldBy = this;
                    _held = entity;
                    _bufferedGrab = false;
                }
            }
        }
        
        Tilemap.CollisionSideFlag collisions = world.Level.MoveAndCollide(this);
        if (collisions.HasFlag(Tilemap.CollisionSideFlag.Down))
        {
            _lastGroundedTick = Time.Tick;
        }
        if (_held != null) _held?.Position = Position + new Vector2(0, -16);

        if (IsGrounded())
        {
            if (Velocity.X > 0) _facingRight = true;
            if (Velocity.X < 0) _facingRight = false;
        }
        return false;
    }

    private bool IsGrounded()
    {
        return Time.Tick - _lastGroundedTick <= 10;
    }

    public override void Draw()
    {
        Raylib.DrawRectangleV(Position - Size/2, Size, Color.Red);
        if (_grabPrompt)
        {
            Raylib.DrawRectangleV(Position - Size/3, (Size / 3)*2, Color.White);
        }

        if (_facingRight)
        {
            Raylib.DrawCircleV(Position + new Vector2(6, -2), 2, Color.Black);
        }
        else
        {
            Raylib.DrawCircleV(Position + new Vector2(-6, -2), 2, Color.Black);
        }
        // Resources.Sprites["orb_base"].DrawCentered(Position, new Vector2(20, 20), tint: Color.Red);
    }
}

public class Block : GameEntity
{
    private float _gravity = 250;
    
    public override bool Update(GameScene world)
    {
        if (HeldBy != null) return false;
        Velocity.Y = Velocity.Y.MoveTowards(200, _gravity * Time.DeltaTime);
        Tilemap.CollisionSideFlag collisions = world.Level.MoveAndCollide(this);
        if (collisions.HasFlag(Tilemap.CollisionSideFlag.Down))
        {
            Velocity.X = Velocity.X.MoveTowards(0, Time.DeltaTime * 200);
        }
        
        return false;
    }

    public override void Draw()
    {
        Raylib.DrawRectangleV(Position - Size/2, Size, Color.Blue);
    }
}