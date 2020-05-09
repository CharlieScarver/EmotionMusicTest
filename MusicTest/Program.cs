using System;
using System.Numerics;
using Emotion.Common;
using Emotion.IO;
using Emotion.Plugins.ImGuiNet;
using Emotion.Standard.XML;
using MusicTest.Core;
using MusicTest.RoomData;

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
            // TODO: Render size should be the lowest supported resolution
            config.RenderSize = new Vector2(1920, 1080);
            // Plugin needed for the UI of Emotion Tools
            config.AddPlugin(new ImGuiNetPlugin());

            Engine.Setup(config);

            // Read tmx map file and create a TiledMap
            TextAsset tmxMap = Engine.AssetLoader.Get<TextAsset>("Rooms/3x2.tmx");
            XMLReader reader = new XMLReader(tmxMap.Content);
            TiledMap tiledMap = new TiledMap(reader);

            TextAsset progressFile = Engine.AssetLoader.Get<TextAsset>("progress.json");
            TextAsset testRoom = Engine.AssetLoader.Get<TextAsset>("Rooms/testRoom.json");

            MainScene scene = new MainScene(progressFile, testRoom, tiledMap);
            GameContext.Scene = scene;

            Engine.SceneManager.SetScene(scene);
            Engine.Run();
        }
    }
}
