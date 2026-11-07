using Raylib_cs;

namespace GameLibrary;

public class KillLevel: Level
{
    private int _overheatThreshold;
    private int _hitsRequired;
    private int _prevScore;
    private int _hits;
    
    // if (Game.Level.Gimmick == Level.LevelGimmick.Kill)
    // {

    // }
    
    public KillLevel(string message, int overheatThreshold, int hitsRequired, DialogueAsset? smallTalk, Action targetReachedAction) : base(message, smallTalk, targetReachedAction)
    {
        _overheatThreshold = overheatThreshold;
        _hitsRequired = hitsRequired;
    }

    public override void Start(GameScene gameScene)
    {
        Assets.Dialogues[31].PlayIfUnheard(true);
    }

    public override void Update(GameScene gameScene)
    {
        if (gameScene.GetCombo() <= 1 && gameScene.GetScore() != 0)
        {
            gameScene.SetScore(0);
            SetProgress(0);
        }
        if (gameScene.GetScore() != _prevScore)
        {
            SetProgress((float)gameScene.GetScore() / _overheatThreshold);
            _prevScore = gameScene.GetScore();
        }

        if (_prevScore >= _overheatThreshold)
        {
            Assets.Dialogues[32].PlayIfUnheard(true);
            Color col = new Color(255, 0, 0, Math.Clamp((_prevScore - _overheatThreshold) / 100, 0, 128));
            Resources.Sprites["radial_inverted"].Draw(0, 0, 720, 720, tint: col);
            // ImGui.DrawTextCentered("Overcharged!\nDo not", 360, 360, 40);
        }
    }

    public override string GetGoalText(GameScene gameScene)
    {
        return gameScene.GetScore() >= _overheatThreshold ? "OVER CAPACITY! DO NOT USE BOMBS!" : "???";
    }

    public override bool IsOver(GameScene gameScene)
    {
        return _hits >= _hitsRequired;
    }

    public override void BombExploded(GameScene gameScene)
    {
        if (gameScene.GetScore() >= _overheatThreshold)
        {
            _hits++;
            gameScene.SetScore(0);
            gameScene.EndCombo();

            if (_hits == 1)
            {
                Assets.Dialogues[34].PlayIfUnheard(true);
            }
            if (_hits == 2)
            {
                Assets.Dialogues[36].PlayIfUnheard(true);
            }
            if (_hits == 3)
            {
                Assets.Dialogues[37].PlayIfUnheard(true);
            }
        }
    }
}