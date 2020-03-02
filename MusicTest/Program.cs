using System;
using System.Numerics;
using Emotion.Common;

namespace MusicTest
{
    public class Program
    {        
        private static void Main()
        {
            // Configuration.
            Configurator config = new Configurator();
            config.HostSize = new Vector2(1280, 720);
            config.HostTitle = "Music Test";
            config.DebugMode = true;

            Engine.Setup(config);
            Engine.SceneManager.SetScene(new MainScene());
            Engine.Run();
        }
    }
}
