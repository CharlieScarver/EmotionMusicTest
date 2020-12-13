using Emotion.Graphics;
using Emotion.Primitives;
using MusicTest.GameObjects;
using System.Numerics;

namespace MusicTest.Events
{
    public class SceneChanger : GameObject
    {
        public string NextScene { get; set; }

        public SceneChanger(string name, string nextScene, Vector2 size, Vector3 position)
        {
            Name = name;
            NextScene = nextScene;
            Size = size;
            Position = position;
        }

        public override void Render(RenderComposer composer)
        {
            // Render
            composer.RenderOutline(
                Position,
                Size,
                Color.Pink,
                2
            );
        }
    }
}
