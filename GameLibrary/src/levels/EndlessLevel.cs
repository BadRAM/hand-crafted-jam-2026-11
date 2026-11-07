namespace GameLibrary;

public class EndlessLevel : Level
{
    public EndlessLevel() : base("", null, () => {}) { }

    public override string GetGoalText(GameScene gameScene)
    {
        return $"Score: {gameScene.GetScore()}";
    }

    public override bool IsOver(GameScene gameScene)
    {
        return false;
    }
}