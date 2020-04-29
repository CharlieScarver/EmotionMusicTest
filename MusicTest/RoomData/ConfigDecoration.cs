using Emotion.Primitives;
using Newtonsoft.Json;
using System.Numerics;

namespace MusicTest.RoomData
{
    public class ConfigDecoration
    {
        public string Name { get; set; }

        public string TextureName { get; set; }

        public Vector2 Size { get; set; }

        public Vector3 Position { get; set; }

        public Vector2? DisplaySize { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public Rectangle? TextureArea { get; set; }

        public bool FlipX { get; set; }

        /// <summary>
        /// Determines the intensity of the Shadow effect.
        /// Values range from 255 (no shadow) to 0 (completely black).
        /// Defaults to 255.
        /// </summary>
        public int ShadowReverseIntensity { get; set; } = 255;

        /// <summary>
        /// Determines the intensity of the Blur effect.
        /// Defaults to 0.
        /// </summary>
        public float BlurIntensity { get; set; } = 0;
    }
}
