using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Primitives;
using Newtonsoft.Json;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Decoration : TransformRenderable
    {
        public string Name { get; set; }

        public string TextureName { get; set; }

        public TextureAsset TextureAsset { get; set; }

        public Vector2 DisplaySize { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public Rectangle? TextureArea { get; set; }

        public bool FlipX { get; set; }

        public ShaderAsset ShadowShader { get; set; }

        public int VelocityOffsetX { get; set; }

        public Decoration(string name, string textureName, Vector2 size, Vector3 position, Vector2? displaySize = null, Rectangle? textureArea = null, bool flipX = false) {
            Name = name;
            TextureName = textureName;
            Size = size;
            Position = position;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{TextureName}");
            DisplaySize = displaySize ?? size;
            TextureArea = textureArea;
            FlipX = flipX;

            if (Name.Contains("Ceiling Icicle"))
            {
                VelocityOffsetX = 2;
            }
            else if (Name.Contains("Floor Icicle"))
            {
                VelocityOffsetX = 4;
            }

            ShadowShader = Engine.AssetLoader.Get<ShaderAsset>("Shaders/ShadowShader.xml");
        }

        public override void Render(RenderComposer composer)
        {
            if (TextureAsset == null)
            {
                return;
            }

            if (Name.Contains("Icicle") || Name.Contains("Snowy Rock 1 FG"))
            {
                //TextureAsset.Texture.Tile = false;
                composer.SetShader(Engine.AssetLoader.Get<ShaderAsset>("Shaders/Blur.xml").Shader);
                composer.SetShader(ShadowShader.Shader);
                composer.RenderSprite(
                    Position,
                    DisplaySize,
                    new Color(180, 180, 180),
                    TextureAsset.Texture,
                    TextureArea,
                    FlipX
                );
                //composer.SetShader(null);
                composer.SetShader(null);
            }
            else if (Name.Contains("Wall"))
            {
                //composer.SetShader(Engine.AssetLoader.Get<ShaderAsset>("Shaders/Blur.xml").Shader);
                composer.RenderSprite(
                    Position,
                    Size,
                    Color.White,
                    TextureAsset.Texture,
                    TextureArea,
                    FlipX
                );
                //composer.SetShader(null)
            }
            else if (Name.Contains("Snow Floor"))
            {
                composer.RenderSprite(
                    Position,
                    DisplaySize,
                    Color.White,
                    TextureAsset.Texture,
                    TextureArea,
                    FlipX
                );
            }
            else
            {
                composer.RenderSprite(
                    Position,
                    DisplaySize,
                    Color.White,
                    TextureAsset.Texture,
                    TextureArea,
                    FlipX
                );
            }
        }
    }
}
