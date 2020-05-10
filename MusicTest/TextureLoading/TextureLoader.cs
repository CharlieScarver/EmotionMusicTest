using Emotion.Common;
using Emotion.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicTest.Collision
{
    public static class TextureLoader
    {
        public static void Load(List<string> textures)
        {
            Task[] tasks = new Task[textures.Count];

            for (int i = 0; i < textures.Count; i++)
            {
                tasks[i] = Engine.AssetLoader.GetAsync<TextureAsset>($"Textures/{textures[i]}");
            }

            Task.WaitAll(tasks);
        }
    }
}
