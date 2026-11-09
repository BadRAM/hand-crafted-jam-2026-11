using System.Numerics;
using System.Runtime.InteropServices;
using Box2D;
using Raylib_cs;

namespace GameLibrary;

using System.Runtime.InteropServices.JavaScript;

internal static partial class BrowserLog
{
    [JSImport("globalThis.console.error")]
    internal static partial void Error(string message);
}

public class GameScene : Scene
{
    private World Box2dWorld;
    private static DebugDraw _debugDraw = Box2dDebugDrawRaylib.Create();
    float timeStep = 1.0f / 60.0f;
    int subStepCount = 4;
    
    private List<Ball> _balls = new List<Ball>();
    
    private int _score;
    private int _displayedScore;
    private int _combo;
    private float _lastClear;
    private List<ScoreAnim> _scoreAnims = new List<ScoreAnim>();
    private AnimCurve<float> _drainAnim = new AnimCurve<float>(1);
    private AnimCurve<float> _instructionTextRadius = new AnimCurve<float>(-280);
    private bool _overflow;

    private State _state = State.Gaming;
    
    enum State
    {
        Gaming,
        Draining
    }
    
    public GameScene()
    {
        try
        {
            BrowserLog.Error("Before WorldDef");

            var def = new Box2D.WorldDef();

            BrowserLog.Error("After WorldDef");

            var world = new Box2D.World(def);

            BrowserLog.Error("After World creation");
        }
        catch (Exception ex)
        {
            BrowserLog.Error("ENTERED CATCH");
            BrowserLog.Error(ex.ToString());
        }
        
        WorldDef worldDef = WorldDef.Default;
        
        worldDef.Gravity = new Vector2(0.0f, 0.0f);
        worldDef.HitEventThreshold = 15f;
        Box2dWorld = World.CreateWorld(worldDef);
        
        Assets.Dialogues[2].PlayIfUnheard(true);

        Start();
    }

    public void Start()
    {
        _overflow = false;
        _state = State.Gaming;
        if (Mixer.IsPaused)
        {
            Mixer.Resume();
        }

        if (Mixer.MusicPlaying == null)
        {
            Mixer.PlayNextMusic();
        }
        
        Assets.Dialogues[48].HasBeenPlayed = false;
        Assets.Dialogues[49].HasBeenPlayed = false;
        Game.Level.Start(this);
        Resources.Sounds["game_start"].Play();
    }
    
    public override void Update()
    {
        Raylib.ClearBackground(Color.DarkBlue);
        
        if (_state == State.Draining)
        {
            BackgroundDraw.Web(1 - _drainAnim.Sample());
            BackgroundDraw.Spiral(_drainAnim.Sample());
            if (_drainAnim.Start < _drainAnim.End)
            {
                BackgroundDraw.ChargeMeter(1 - _drainAnim.Sample());
            }
        }
        else
        {
            BackgroundDraw.Web();
            BackgroundDraw.ChargeMeter(Game.Level.GetProgress(this));
        }
        
        // if (Raylib.IsKeyDown(KeyboardKey.A)) Raylib.ClearBackground(new Color(32, 32, 32, 255));
        
        BackgroundDraw.CirclePulse(Math.Max(0, Mixer.Beat() / 4 - 0.5f) % 1);
        BackgroundDraw.CirclePulse(Math.Max(0, Mixer.Beat() / 4 - 0.0f) % 1);
        BackgroundDraw.Waveform2();

        int ballFreq = _combo >= 2 && _balls.Count < 200 ? 5 : 10;
        if (_state == State.Gaming && (Time.Frame % ballFreq == 0 || _balls.Count < 60))
        {
            Vector2 pos = Vector2.Normalize(Utils.InsideUnitCircle(Random.Shared)) * 32f + new Vector2(36, 36);
            MakeBall(pos, Ball.RandomDefaultType());
        }
        
        Box2dWorld.Step(timeStep, subStepCount);

        ContactEvents contactEvents = Box2dWorld.ContactEvents;

        List<Ball> outOfBoundsBalls = new List<Ball>();
        
        float gravityScale = 100 * (1 + (1 - _drainAnim.Sample()));
        foreach (Ball ball in _balls)
        {
            ball.Draw();
            
            Vector2 delta = new Vector2(36f, 36f) - ball.Position;
            if (delta.Length() > 45f) outOfBoundsBalls.Add(ball);
            Vector2 g = Vector2.Normalize(delta) * gravityScale;
            ball.Body.ApplyForce(g, ball.Position, true);
        }
        
        foreach (Ball ball in outOfBoundsBalls)
        {
            _score += 10 * _combo;
            ball.Body.Destroy();
            _balls.Remove(ball);
        }
        
        for (var i = 0; i < contactEvents.HitEvents.Length; i++)
        {
            var contact = contactEvents.HitEvents[i];
            // Raylib.DrawCircleV(contact.point.ToVec2() * 10, 3, new Color(255, 0, 0, 192));
            float volume = 0.3f;
            if      (contact.ApproachSpeed > 40) { Resources.Sounds["collision1"].PlayRandomPitch(-(contact.Point.X / 72), 1f, 1f   * volume); }
            else if (contact.ApproachSpeed > 35) { Resources.Sounds["collision2"].PlayRandomPitch(-(contact.Point.X / 72), 1f, 0.9f * volume); }
            else if (contact.ApproachSpeed > 30) { Resources.Sounds["collision3"].PlayRandomPitch(-(contact.Point.X / 72), 1f, 0.8f * volume); }
            else if (contact.ApproachSpeed > 25) { Resources.Sounds["collision4"].PlayRandomPitch(-(contact.Point.X / 72), 1f, 0.7f * volume); }
            else if (contact.ApproachSpeed > 20) { Resources.Sounds["collision5"].PlayRandomPitch(-(contact.Point.X / 72), 1f, 0.6f * volume); }
            else                                 { Resources.Sounds["collision6"].PlayRandomPitch(-(contact.Point.X / 72), 1f, 0.5f * volume); }
            // Console.WriteLine($"BALLS COLLIDING with speed:{contact.approachSpeed:N5} at: {contact.point.ToVec2()}");
        }

        if (_state == State.Gaming)
        {
            float charge = Game.Level.GetProgress(this);
            if (Game.Level.SmallTalk != null && charge > 0.25)
            {
                Game.Level.SmallTalk.PlayIfUnheard();
            }
            if (Game.Level.SmallTalk != null && charge > 0.50)
            {
                Assets.Dialogues[48].PlayIfUnheard();
            }
            if (Game.Level.SmallTalk != null && charge > 0.90)
            {
                Assets.Dialogues[49].PlayIfUnheard();
            }
            
            HandleInput();
            
            if (_combo > 1 && Time.Scaled - _lastClear > 3f)
            {
                EndCombo();
            }
            
            if (_balls.Count > 350 && Time.Scaled % 1f < 0.5f)
            {
                Color col = new Color(255, 0, 0, _balls.Count - 300);
                Resources.Sprites["radial_inverted"].Draw(0, 0, 720, 720, tint: col);
                ImGui.DrawTextCentered("Overflow\nImminent", 360, 360, 40);
            }
            
            if (_balls.Count > 400)
            {
                _score = 0;
                _overflow = true;
                Drain();
                Game.ScrollText("Orb Overflow! Charge vented.");
            }
            
            if (Game.Level.IsOver(this))
            {
                Drain();
            }
        }
        
        if (_state == State.Draining)
        {
            if (_balls.Count == 0 && _drainAnim.IsComplete())
            {
                if (!_overflow)
                {
                    Game.Level.TargetReachedAction.Invoke();
                }
                else
                {
                    Start();
                }
            }

            
            float size = 200 * _drainAnim.Sample();
            Resources.Sprites["mask"].DrawCentered(360, 360, size, size, tint: Color.Black );
            
            List<Ball> ballsToRemove = new List<Ball>();
            foreach (Ball ball in _balls)
            {
                if (Vector2.Distance(new Vector2(36, 36), ball.Position) < size / 20 - 1)
                {
                    ballsToRemove.Add(ball);
                }
            }
            
            foreach (Ball ball in ballsToRemove)
            {
                ball.Body.Destroy();
                _balls.Remove(ball);
            }
        }
        
        foreach (ScoreAnim scoreAnim in _scoreAnims)
        {
            scoreAnim.Draw();
        }

        _scoreAnims.RemoveAll(s => s.IsComplete());

        if (_displayedScore < _score - 3000) _displayedScore += 200;
        if (_displayedScore > _score) _displayedScore -= 500;
        if (_displayedScore < _score) _displayedScore += 10;
        if (!Game.IsScrolling() && _instructionTextRadius.End > -260) _instructionTextRadius = AnimCurve.NewFloat(_instructionTextRadius.Sample(), -280, 0.5f, Easings.OutQuad);
        if ( Game.IsScrolling() && _instructionTextRadius.End < -260) _instructionTextRadius = AnimCurve.NewFloat(_instructionTextRadius.Sample(), -240, 0.5f, Easings.OutQuad);
        ImGui.DrawTextRadial(0, _instructionTextRadius.Sample(), Game.Level.GetGoalText(this));
        // ImGui.DrawTextRadial(0, -240, $"Balls: {_balls.Count}");
        if (_combo > 1)
        {
            ImGui.DrawTextRadial(0, 280, $"{_combo}x Combo!");
            int comboPercentage = (int)Math.Clamp((Time.Scaled - _lastClear) * 10f, 0, 29);
            ImGui.DrawTextRadial(0, 240, "..............................".Substring(comboPercentage));
        }
        
        Game.Level.Update(this);
        
        // if (Raylib.IsKeyDown(KeyboardKey.V))
        //     b2World_Draw(WorldId, _debugDraw);
    }

    private void HandleInput()
    {
        Ball? hoveredBall = GetBallUnderCursor(16);
        List<Ball> ballsToMerge = new List<Ball>();
        bool makeBomb = false;

        if (hoveredBall != null)
        {
            List<Ball> mergeBalls = MergeBalls(hoveredBall, out makeBomb);

            if (mergeBalls.Count >= 100)
            {
                Assets.Dialogues[46].PlayIfUnheard();
            }

            if (hoveredBall.BallType == Ball.Type.Bomb)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    foreach (Ball ball in _balls)
                    {
                        if (Raylib.CheckCollisionPointCircle(ball.Position, hoveredBall.Position, 12f))
                        {
                            ballsToMerge.Add(ball);
                        }
                    }
                    
                    ExplosionDef explode = new ExplosionDef();
                    explode.Position = hoveredBall.Position;
                    explode.Radius = 20;
                    explode.ImpulsePerLength = 200;
                    
                    Box2dWorld.Explode(explode);
                    
                    Resources.Sounds["bomb"].Play();

                    Game.Level.BombExploded(this);
                }
            }
            else if (mergeBalls.Count < 4) {} // Don't clear normal groups less than 4
            else if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                ballsToMerge.AddRange(mergeBalls);
                if (makeBomb)
                {
                    MakeBall(mergeBalls[0].Position, Ball.Type.Bomb);
                    if (!Assets.Dialogues[5].PlayIfUnheard()) Assets.Dialogues[44].Play(false);
                }
                
                if (hoveredBall.BallType is Ball.Type.BlueCrystal or Ball.Type.YellowCrystal or Ball.Type.PurpleCrystal or Ball.Type.RedCrystal)
                {
                    Resources.Sounds["match_super"].Play();
                }
            }
            else
            {
                Color col = mergeBalls.Count > _combo ? Color.White : Color.Black;
                col = Raylib.ColorAlpha(col, 0.25f);
                foreach (Ball ball in mergeBalls)
                {
                    Vector2 pos = ball.Body.WorldCenterOfMass * 10;
                    Resources.Sprites["mask"].DrawCentered(pos, new Vector2(28, 28), tint: col);
                }
            }
        }

        if (ballsToMerge.Count > 0)
        {
            _lastClear = Time.Scaled;
            if ((hoveredBall?.IsSpecial() ?? false) || makeBomb || ballsToMerge.Count > _combo)
            { }
            else
            {
                EndCombo();
            }

            _combo++;
            if (_combo == 3 && !Mixer.IsDialoguePlaying()) Assets.Dialogues[6].PlayIfUnheard();
            if (ballsToMerge.Count > 50) Assets.Dialogues[43].PlayIfUnheard();
            if (ballsToMerge.Count > 25 && Random.Shared.NextSingle() < 0.25f) Assets.Dialogues[new List<int>(){40, 41, 42}.PickRandom()].Play(false);
            Resources.Sounds[$"match{Math.Clamp(_combo, 1, 14)}"].Play();
            int mergeScore = ballsToMerge.Count * _combo * 10;
            _score += mergeScore;
            _scoreAnims.Add(new ScoreAnim($"{ballsToMerge.Count}0x{_combo} = {mergeScore}", ballsToMerge[0].Position * 10, ballsToMerge));
        }
        
        foreach (Ball ball in ballsToMerge)
        {
            ball.Body.Destroy();
            _balls.Remove(ball);
        }
    }

    public void EndCombo()
    {
        if (_combo >= 8)
        {
            Resources.Sounds["combo_end_big"].Play();
        }
        else if (_combo >= 4)
        {
            Resources.Sounds["combo_end_small"].Play();
        }
        
        while (_combo >= 8)
        {
            Vector2 pos = Vector2.Normalize(Utils.InsideUnitCircle(Random.Shared)) * 32f + new Vector2(36, 36);
            MakeBall(pos, new []{Ball.Type.BlueCrystal, Ball.Type.PurpleCrystal, Ball.Type.YellowCrystal, Ball.Type.RedCrystal}[Random.Shared.Next(4)]);
            _combo -= 4;
            if (Assets.Dialogues[4].PlayIfUnheard() && Random.Shared.NextSingle() < 0.5f) Assets.Dialogues[new List<int>(){47, 45}.PickRandom()].Play(false);
        }
        
        _combo = 0;
    }

    private void Drain()
    {
        EndCombo();
        _score = 0;
        _displayedScore = 0;
        _state = State.Draining;
        _drainAnim = AnimCurve.NewFloat(0, 1, 2, Easings.OutQuad);
        Resources.Sounds["game_end"].Play();
    }
    
    private Ball? GetBallUnderCursor(float maxDistance)
    {
        maxDistance /= 10;
        KeyValuePair<Ball?, float> nearestBall = new KeyValuePair<Ball, float>(null, maxDistance);
        Vector2 cursorPos = Game.GetCursorPosOnDevice() / 10f;
        foreach (Ball ball in _balls)
        {
            Vector2 pos = ball.Body.WorldCenterOfMass;
            float dist = Vector2.Distance(pos, cursorPos);
            if (dist < nearestBall.Value)
            {
                nearestBall = new KeyValuePair<Ball?, float>(ball, dist);
            }
        }
        return nearestBall.Key;
    }
    
    private List<Ball> MergeBalls(Ball origin, out bool makeBomb)
    {
        makeBomb = false;
        if (origin.BallType == Ball.Type.BlueCrystal)   return _balls.Where(b => b.BallType is Ball.Type.Blue   or Ball.Type.BlueCrystal  ).ToList();
        if (origin.BallType == Ball.Type.YellowCrystal) return _balls.Where(b => b.BallType is Ball.Type.Yellow or Ball.Type.YellowCrystal).ToList();
        if (origin.BallType == Ball.Type.PurpleCrystal) return _balls.Where(b => b.BallType is Ball.Type.Purple or Ball.Type.PurpleCrystal).ToList();
        if (origin.BallType == Ball.Type.RedCrystal)    return _balls.Where(b => b.BallType is Ball.Type.Red    or Ball.Type.RedCrystal   ).ToList();

        List<Ball> validBalls = _balls.Where(b => b.BallType == origin.BallType).ToList();
        
        List<Ball> firstSet = GetNeighbors(origin, validBalls);
        if (firstSet.Count == 6) // If the origin can make a bomb
        {
            firstSet.Insert(0, origin);
            makeBomb = true;
            return firstSet;
        }
        foreach (Ball ball in firstSet) // check if any of the origin adjacent balls can make bombs.
        {
            List<Ball> neighbors = GetNeighbors(ball, validBalls);
            if (neighbors.Count == 6)
            {
                neighbors.Insert(0, ball);
                makeBomb = true;
                return neighbors;
            }
        }
        
        validBalls.Remove(origin);
        List<Ball> ballsToMerge = new List<Ball>() {origin};
        int checkIndex = 0;
        
        while (checkIndex < ballsToMerge.Count) // else find the full net of balls to merge
        {
            List<Ball> neighbors = GetNeighbors(ballsToMerge[checkIndex], validBalls);
            ballsToMerge.AddRange(neighbors);
            validBalls.RemoveAll(b => neighbors.Contains(b));
            checkIndex++;
        }
        
        return ballsToMerge;
    }
    
    private List<Ball> GetNeighbors(Ball centerBall, List<Ball> validBalls)
    {
        Vector2 pos = centerBall.Body.WorldCenterOfMass;
        return validBalls.Where(b => b != centerBall && Raylib.CheckCollisionCircles(b.Body.WorldCenterOfMass, 1.6f, pos, 1.6f)).ToList();
    }
    
    public void Destroy()
    {
        Box2dWorld.Destroy();
    }
    
    public int GetScore() => _score;
    public int GetCombo() => _combo;
    public int SetScore(int score) => _score = score;

    private void MakeBall(Vector2 pos, Ball.Type ballType)
    {
        BodyDef bodyDef = new BodyDef();
        bodyDef.Type = BodyType.Dynamic;
        bodyDef.Position = new Vector2(0, 0);
        bodyDef.LinearDamping = 0.05f;
        Body body = Box2dWorld.CreateBody(bodyDef);
        
        Circle dynamicCircle;
        dynamicCircle.Center = pos;
        dynamicCircle.Radius = 1.4f;
        
        ShapeDef shapeDef = new ShapeDef();
        shapeDef.EnableHitEvents = true;
        shapeDef.Density = 1.0f;
        shapeDef.Material.Friction = 0.3f;
        shapeDef.Material.Restitution = 0.75f;

        body.CreateShape(shapeDef, dynamicCircle);
        _balls.Add(new Ball(body, ballType));
    }
    
    public class Ball
    {
        public Body Body;
        public Type BallType;
        public Vector2 Position;
        
        public float Rotation => Body.Rotation.GetAngle() * 180 / MathF.PI;
        
        private static readonly Type[] DefaultTypes = { Type.Red, Type.Blue, Type.Yellow, Type.Purple };
        public static Type RandomDefaultType() => DefaultTypes[Random.Shared.Next(DefaultTypes.Length)];

        public enum Type
        {
            Red,
            Blue,
            Yellow,
            Purple,
            Bomb,
            RedCrystal,
            BlueCrystal,
            YellowCrystal,
            PurpleCrystal
        }

        public Ball(Body body, Type ballType)
        {
            Body = body;
            BallType = ballType;
        }
        
        public bool IsSpecial() => !(BallType is Type.Blue or Type.Purple or Type.Yellow or Type.Red);
        
        public void Draw()
        {
            Position = Body.WorldCenterOfMass;
            Vector2 pos = Position * 10;
            Vector2 size = new Vector2(28, 28);
            Sprite baseSprite = Resources.Sprites["orb_base"];
            Sprite? spinPattern = null;
            Sprite specular = Resources.Sprites["orb_shine"];
            Color col = Color.Pink;
            switch (BallType)
            {
                case Type.Red:
                    col = Color.Red;
                    break;
                case Type.Blue:
                    col = Color.SkyBlue;
                    break;
                case Type.Yellow:
                    col = Color.Yellow;
                    break;
                case Type.Purple:
                    col = Color.Purple;
                    break;
                case Type.Bomb:
                    col = Color.DarkGray;
                    break;
                case Type.BlueCrystal:
                    spinPattern = Resources.Sprites["orb_spiral"];
                    col = Color.SkyBlue;
                    break;
                case Type.PurpleCrystal:
                    spinPattern = Resources.Sprites["orb_spiral"];
                    col = Color.Purple;
                    break;
                case Type.RedCrystal:
                    spinPattern = Resources.Sprites["orb_spiral"];
                    col = Color.Red;
                    break;
                case Type.YellowCrystal:
                    spinPattern = Resources.Sprites["orb_spiral"];
                    col = Color.Yellow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            baseSprite.DrawCentered(pos, size, tint: col);
            if (spinPattern != null) spinPattern.DrawCentered(pos, size, rotation: Rotation, tint: Color.Lerp(col, Color.White, Easings.FullSine(Mixer.Beat() % 1f)/4f+0.5f));
            specular.DrawCentered(pos, size);
        }

        public Color GetTint()
        {
            Color col = Color.Pink;
            switch (BallType)
            {
                case Type.Red:
                case Type.RedCrystal:
                    col = Color.Red;
                    break;
                case Type.Blue:
                case Type.BlueCrystal:
                    col = Color.SkyBlue;
                    break;
                case Type.Yellow:
                case Type.YellowCrystal:
                    col = Color.Yellow;
                    break;
                case Type.Purple:
                case Type.PurpleCrystal:
                    col = Color.Purple;
                    break;
                case Type.Bomb:
                    col = Color.DarkGray;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return col;
        }
    }
    
    public class ScoreAnim
    {
        private string _text;
        private AnimCurve<float> _anim = AnimCurve.NewFloat(0, 1, 2);
        private AnimCurve<float> _mergeAnim = AnimCurve.NewFloat(0, 1, 0.35f);
        private Vector2 _pos;
        private List<Ball> _balls;

        public ScoreAnim(string text, Vector2 pos, List<Ball> balls)
        {
            _text = text;
            _pos = pos;
            _balls = balls;
        }

        public void Draw()
        {
            ImGui.DrawText(_text, _pos + new Vector2(0, -48) * _anim.Sample(), color: Raylib.ColorAlpha(Color.White, Easings.OutQuint(1 - _anim.Sample())));
            if (_mergeAnim.IsComplete()) return;
            foreach (Ball ball in _balls)
            {
                Resources.Sprites["radial"].DrawCentered(Vector2.Lerp(ball.Position * 10, _pos, _mergeAnim.Sample()), new Vector2(32, 32), tint: Raylib.ColorAlpha(ball.GetTint(), 1-_mergeAnim.Sample()));
            }
        }

        public bool IsComplete() => _anim.IsComplete();
    }
    
    static class Box2dDebugDrawRaylib
    {
        public static DebugDraw Create()
        {
            var debugDraw = new DebugDrawSimple();
            // debugDraw.DrawPolygon = DrawPolygon;
            // debugDraw.DrawSolidPolygon = DrawSolidPolygon;
            // debugDraw.DrawCircle = DrawCircle;
            // debugDraw.DrawSolidCircle = DrawSolidCircle;
            // debugDraw.DrawSolidCapsule = DrawSolidCapsule;
            // debugDraw.DrawSegment = DrawSegment;
            // debugDraw.DrawTransform = DrawTransform;
            // debugDraw.DrawPoint = DrawPoint;
            // debugDraw.DrawString = DrawString;
            //
            // debugDraw.drawShapes = true;
            // debugDraw.drawJoints = true;
            // debugDraw.drawJointExtras = true;
            // // debugDraw.drawBounds = true;
            // debugDraw.drawMass = true;
            // debugDraw.drawBodyNames = true;
            // debugDraw.drawGraphColors = true;
            // debugDraw.drawContactNormals = true;
            // // debugDraw.drawContactForces = true;
            // // debugDraw.drawContactFeatures = true;
            // // debugDraw.drawFrictionForces = true;
            // debugDraw.drawIslands = true;

            return debugDraw;
        }
        
        // private static void DrawPolygon(ReadOnlySpan<Vector2> vertices, HexColor color)
        // {
        //     for (int i = 0; i < vertices.Length; i++)
        //     {
        //         Raylib.DrawLineV(vertices[i].ToVec2(), vertices[(i + 1) % vertices.Length], color.ToRaylib());
        //     }
        // }
        //
        // private static void DrawSolidPolygon(in B2Transform transform, ReadOnlySpan<B2Vec2> vertices, int vertexCount, float radius, B2HexColor color, object o)
        // {
        //     Vector2[] verts = vertices.ToArray().Select(v => v.ToVec2()).ToArray();
        //
        //     for (var i = 0; i < verts.Length; i++)
        //     {
        //         verts[i] = new Vector2(verts[i].X * transform.q.c - verts[i].Y * transform.q.s, verts[i].X * transform.q.s + verts[i].Y * transform.q.c);
        //         verts[i] += transform.p.ToVec2();
        //     }
        //     for (int i = 0; i < vertexCount; i++)
        //     {
        //         Raylib.DrawLineV(verts[i], verts[(i + 1) % vertexCount], color.ToRaylib());
        //     }
        // }
        //
        // private static void DrawCircle(in B2Vec2 center, float radius, B2HexColor color, object o)
        // {
        //     Raylib.DrawCircleLinesV(center.ToVec2(), radius, color.ToRaylib());
        // }
        //
        // private static void DrawSolidCircle(in B2Transform transform, float radius, B2HexColor color, object o)
        // {
        //     Vector2 center = transform.p.ToVec2();
        //     Raylib.DrawCircleV(center, radius, Raylib.ColorAlpha(color.ToRaylib(), 0.25f));
        //     Raylib.DrawCircleLinesV(center, radius, color.ToRaylib());
        //     Raylib.DrawLineV(center, center + new Vector2(transform.q.c, transform.q.s) * radius, color.ToRaylib());
        // }
        //
        // private static void DrawSolidCapsule(in B2Vec2 point1, in B2Vec2 point2, float radius, B2HexColor color, object o)
        // {
        //     Vector2 p1 = point1.ToVec2();
        //     Vector2 p2 = point2.ToVec2();
        //     Raylib.DrawCircleV(p1, radius, color.ToRaylib());
        //     Raylib.DrawCircleV(p2, radius, color.ToRaylib());
        //     Raylib.DrawLineEx(p1, p2, radius, color.ToRaylib());
        // }
        //
        // private static void DrawSegment(in B2Vec2 point1, in B2Vec2 point2, B2HexColor color, object o)
        // {
        //     Vector2 p1 = point1.ToVec2();
        //     Vector2 p2 = point2.ToVec2();
        //     Raylib.DrawLineV(p1, p2, color.ToRaylib());
        // }
        //
        // private static void DrawTransform(in B2Transform transform, object o)
        // {
        //     DrawSolidCircle(transform, 4, B2HexColor.b2_colorAqua, o);
        // }
        //
        // private static void DrawPoint(in B2Vec2 point, float size, B2HexColor color, object o)
        // {
        //     Raylib.DrawCircleV(point.ToVec2(), size, color.ToRaylib());
        // }
        //
        // private static void DrawString(in B2Vec2 point, string s, B2HexColor color, object o)
        // {
        //     Vector2 p = point.ToVec2();
        //     Raylib.DrawText(s, (int)p.X, (int)p.Y, 4, color.ToRaylib());
        // }
    }
}