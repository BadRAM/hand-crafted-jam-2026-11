namespace GameLibrary;

public class MakeBombLevel : Level
{
    private int _bombsCount;
    private int _bombsTarget;
    private AnimCurve<float> _progressCurve = new AnimCurve<float>(0);
    
    public MakeBombLevel(string message, int bombsTarget, DialogueAsset? smallTalk, Action targetReachedAction) : base(message, smallTalk, targetReachedAction)
    {
        _bombsTarget = bombsTarget;
    }

    public override void Start(GameScene gameScene)
    {
        base.Start(gameScene);
        _bombsCount = 0;
    }

    public override void Update(GameScene gameScene)
    {
        if (Math.Abs(_progressCurve.End - (float)gameScene.GetCombo() / _bombsTarget) > 0.001f)
        {
            _progressCurve = AnimCurve.NewFloat(_progressCurve.Sample(), (float)gameScene.GetCombo() / _bombsTarget, 0.5f, easing: Easings.OutCubic);
        }
    }

    public override string GetGoalText(GameScene gameScene)
    {
        return $"Bombs: {_bombsCount} / {_bombsTarget}";
    }

    public override bool IsOver(GameScene gameScene)
    {
        return _bombsCount >= _bombsTarget;
    }
    
    public override void BombExploded(GameScene gameScene)
    {
        _bombsCount++;
        SetProgress((float)_bombsCount / _bombsTarget);
    }
}