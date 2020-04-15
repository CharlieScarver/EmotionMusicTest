using System.Numerics;

namespace MusicTest.Core.Room
{
    public class ConfigCollisionPlatform
    {
        public Vector2 PointA { get; set; }

        public Vector2 PointB { get; set; }

        public bool IsSloped { get; set; } = false;
    }
}
