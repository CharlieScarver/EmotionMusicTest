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
            config.DebugMode = true;
            //config.RendererCompatMode = true;
            //config.UseIntermediaryBuffer = true;
            config.GlDebugMode = true;
            Engine.Setup(config);

            TextAsset testRoom = Engine.AssetLoader.Get<TextAsset>("testRoom.json");

            Engine.SceneManager.SetScene(new MainScene(testRoom));
            Engine.Run();
        }
    }
}
