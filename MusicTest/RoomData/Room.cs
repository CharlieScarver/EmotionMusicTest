using Emotion.Primitives;
using MusicTest.Events;
using MusicTest.GameObjects;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.RoomData
{
    public class Room
    {
        // Meta
        public string Name { get; set; }
        public Vector2 Size { get; set; }

        // Player
        public Vector3 Spawn { get; set; }

        // Objects
        public List<ConfigDecoration> Backgrounds { get; set; }
        public List<Platform> Platforms { get; set; }
        public List<ConfigDecoration> BackgroundDecorations { get; set; }
        public List<ConfigDecoration> ForegroundDecorations { get; set; }
        public List<ConfigUnit> Units { get; set; }
        public List<ConfigCollisionPlatform> CollisionPlatforms { get; set; }
        public List<SceneChanger> SceneChangers { get; set; }

        // Textures
        public List<string> Textures { get; set; }

        public Rectangle ToRectangle()
        {
            return new Rectangle(Vector2.Zero, Size);
        }
    }
}
