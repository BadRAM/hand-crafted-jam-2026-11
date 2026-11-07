namespace GameLibrary;

public class ScoreLevel : Level
{
    private int _targetScore;
    private int _prevScore;
    
    public ScoreLevel(string message, int targetScore, DialogueAsset? smallTalk, Action targetReachedAction) : base(message, smallTalk, targetReachedAction)
    {
        _targetScore = targetScore;
    }

    public override void Update(GameScene gameScene)
    {
        if (gameScene.GetScore() != _prevScore)
        {
            SetProgress((float)gameScene.GetScore() / _targetScore);
            _prevScore = gameScene.GetScore();
        }
    }

    public override string GetGoalText(GameScene gameScene)
    {
        return $"Score: {gameScene.GetScore()} / {_targetScore}";
    }

    public override bool IsOver(GameScene gameScene)
    {
        return gameScene.GetScore() > _targetScore;
    }
}