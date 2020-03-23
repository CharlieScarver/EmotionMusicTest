using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Primitives;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Decoration : TransformRenderable
    {
        public string Type { get; set; }
        public string FileName { get; set; }

        public TextureAsset TextureAsset { get; set; }

        public Decoration(string type, string fileName, Vector2 size, Vector3 position) {
            Type = type;
            FileName = fileName;
            Size = size;
            Position = position;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>(FileName);
        }

        public override void Render(RenderComposer composer)
        {
            composer.RenderSprite(
                Position,
                Size,
                Color.White,
                TextureAsset.Texture
            );
        }
    }
}
