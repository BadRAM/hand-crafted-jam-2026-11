namespace GameLibrary;

public abstract class Level
{
    public string Message;
    public Action TargetReachedAction;
    private Interpolator<float> _progressCurve;

    protected Level(string message, Action targetReachedAction)
    {
        Message = message;
        TargetReachedAction = targetReachedAction;
        _progressCurve = new Interpolator<float>(0);
    }

    public virtual void Start(GameScene gameScene) { _progressCurve = new Interpolator<float>(0); }
    public virtual void Update(GameScene gameScene) {}
    public abstract string GetGoalText(GameScene gameScene);
    public float GetProgress(GameScene gameScene) => _progressCurve.Sample();
    protected void SetProgress(float progress) => _progressCurve = new Interpolator<float>(_progressCurve.Sample(), progress, 0.5f, easing: Easings.OutCubic);
    public abstract bool IsOver(GameScene gameScene);
    public virtual void BombExploded(GameScene gameScene) { }
}