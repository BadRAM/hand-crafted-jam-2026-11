namespace GameLibrary;

public class ComboLevel : Level
{
    private int _comboTarget;
    private int _prevCombo;
    private AnimCurve<float> _progressCurve = new AnimCurve<float>(0);
    
    public ComboLevel(string message, int comboTarget, DialogueAsset? smallTalk, Action targetReachedAction) : base(message, smallTalk, targetReachedAction)
    {
        _comboTarget = comboTarget;
    }
    
    public override void Update(GameScene gameScene)
    {
        if (gameScene.GetCombo() != _prevCombo)
        {
            SetProgress((float)gameScene.GetCombo() / _comboTarget);
            _prevCombo = gameScene.GetCombo();
        }
    }

    public override string GetGoalText(GameScene gameScene)
    {
        return $"Combo: {gameScene.GetCombo()} / {_comboTarget}";
    }

    public override bool IsOver(GameScene gameScene)
    {
        return gameScene.GetCombo() > _comboTarget;
    }
}