using Emotion.Primitives;

namespace MusicTest.GameObjects
{
    public abstract class GameObject : TransformRenderable, IGameObject
    {
        public string Name { get; set; }
    }
}
