using System;
using System.Numerics;
using Emotion.Common;
using Emotion.IO;

namespace MusicTest
{
    public class Program
    {        
        private static void Main()
        {
            int i = -1073740940;
            uint ui = (uint) i;
            Console.WriteLine(ui.ToString("X"));


            // Configuration.
            Configurator config = new Configurator();
            config.HostSize = new Vector2(1920, 1080);
            config.HostTitle = "Music Test";
            config.InitialDisplayMode = Emotion.Platform.DisplayMode.Windowed;
            config.DebugMode = true;
            //config.RendererCompatMode = true;
            //config.UseIntermediaryBuffer = true;
            config.GlDebugMode = true;
            config.RenderSize = new Vector2(1920, 1080);

            Engine.Setup(config);

            TextAsset progressFile = Engine.AssetLoader.Get<TextAsset>("progress.json");
            TextAsset testRoom = Engine.AssetLoader.Get<TextAsset>("testRoom.json");

            Engine.SceneManager.SetScene(new MainScene(progressFile, testRoom));
            Engine.Run();
        }
    }
}
