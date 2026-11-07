namespace GameLibrary;

public abstract class Level
{
    public string Message;
    public DialogueAsset? SmallTalk;
    public Action TargetReachedAction;
    private AnimCurve<float> _progressCurve;

    protected Level(string message, DialogueAsset? smallTalk, Action targetReachedAction)
    {
        Message = message;
        SmallTalk = smallTalk;
        TargetReachedAction = targetReachedAction;
        _progressCurve = new AnimCurve<float>(0);
    }

    public virtual void Start(GameScene gameScene) { _progressCurve = new AnimCurve<float>(0); }
    public virtual void Update(GameScene gameScene) {}
    public abstract string GetGoalText(GameScene gameScene);
    public float GetProgress(GameScene gameScene) => _progressCurve.Sample();
    protected void SetProgress(float progress) => _progressCurve = AnimCurve.NewFloat(_progressCurve.Sample(), progress, 0.5f, easing: Easings.OutCubic);
    public abstract bool IsOver(GameScene gameScene);
    public virtual void BombExploded(GameScene gameScene) { }
}