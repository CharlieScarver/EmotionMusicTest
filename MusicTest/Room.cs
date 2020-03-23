using MusicTest.GameObjects;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest
{
    public class Room
    {
        // Meta
        public string Name { get; set; }
        public Vector2 Size { get; set; }

        // Player
        public Vector3 Spawn { get; set; }

        // Objects
        public List<Decoration> Backgrounds { get; set; }
        public List<Platform> Platforms { get; set; }
        public List<Decoration> Decorations { get; set; }
    }
}
