using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;
using GameLibrary;

namespace RaylibWasm
{
    public partial class Application
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main()
        {
            Game.IsWeb = true;
            Game.Dir = "/resource/";
            
            Game.Load(true);
        }

        /// <summary>
        /// Updates frame
        /// </summary>
        [JSExport]
        public static void UpdateFrame()
        {
            Game.Update();
        }
    }
}
