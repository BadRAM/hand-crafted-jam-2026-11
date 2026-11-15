
namespace GameLibrary;

public class MenuScene : Scene
{
    public MenuScene()
    {
    }
    
    public override void Update()
    {
        Game.ActiveScene = new GameScene();
    }
}