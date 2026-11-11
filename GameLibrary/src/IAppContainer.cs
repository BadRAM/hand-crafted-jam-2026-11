using Raylib_cs;

namespace GameLibrary;

public interface IAppContainer
{
    public void Alert(string message);
    public void SaveImageToDisk(Image image, string name);
}