using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;
using GameLibrary;

namespace RaylibWasm
{
    public partial class Application : IAppContainer
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main()
        {
            JSHost.ImportAsync("main.js", "../main.js");
            Game.IsWeb = true;
            Game.AppContainer = new Application();
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

        public void Alert(string message)
        {
            _alert(message);
        }
        
        public void SaveImageToDisk(Image image, string name)
        {
            string fn = name + ".png";
            Raylib.ExportImage(image, fn);
            OfferFileAsDownload(fn);
        }
        
        [JSImport("globalThis.alert")]
        internal static partial void _alert([JSMarshalAs<JSType.String>] string message);
        
        [JSImport("offerFileAsDownload", "main.js")]
        internal static partial void OfferFileAsDownload([JSMarshalAs<JSType.String>] string fileName);
    }
}
