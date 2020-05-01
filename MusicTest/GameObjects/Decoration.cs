using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Primitives;
using Newtonsoft.Json;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Decoration : GameObject, IGameObject
    {
        public string TextureName { get; set; }

        public TextureAsset TextureAsset { get; set; }

        public Vector2 DisplaySize { get; set; }

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
        public float BlurIntensity { get; set; }

        public ShaderAsset ShadowShader { get; set; }
        public ShaderAsset BlurShader { get; set; }

        public int VelocityOffsetX { get; set; }

        public Decoration(string name, string textureName, Vector2 size, Vector3 position, Vector2? displaySize = null, Rectangle? textureArea = null, bool flipX = false, float blurIntensity = 0, int shadowReverseIntensity = 255) {
            Name = name;
            TextureName = textureName;
            Size = size;
            Position = position;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{TextureName}");
            DisplaySize = displaySize ?? size;
            TextureArea = textureArea;
            FlipX = flipX;

            // Effects
            // TODO: Default values for numerical parameters are not taken into account for some reason
            BlurIntensity = blurIntensity;
            ShadowReverseIntensity = shadowReverseIntensity == 0 ? 255 : shadowReverseIntensity; // temporary fix

            if (Name.Contains("Ceiling Icicle"))
            {
                VelocityOffsetX = 2;
            }
            else if (Name.Contains("Floor Icicle"))
            {
                VelocityOffsetX = 4;
            }

            ShadowShader = Engine.AssetLoader.Get<ShaderAsset>("Shaders/ShadowShader.xml");
            BlurShader = Engine.AssetLoader.Get<ShaderAsset>("Shaders/Blur.xml");
        }

        public override void Render(RenderComposer composer)
        {
            if (TextureAsset == null)
            {
                return;
            }

            // Apply Blur effect if such is set
            if (BlurIntensity > 0)
            {
                composer.SetShader(BlurShader.Shader);
                BlurShader.Shader.SetUniformFloat("sigma", BlurIntensity);
            }

            // Apply Shadow effect if such is set
            Color color = Color.White;
            if (ShadowReverseIntensity < 255)
            {
                color = new Color(ShadowReverseIntensity, ShadowReverseIntensity, ShadowReverseIntensity); // 180 for a shadowy look
            }

            // Render
            composer.RenderSprite(
                Position,
                DisplaySize,
                color,
                TextureAsset.Texture,
                TextureArea,
                FlipX
            );

            if (BlurIntensity > 0)
            {
                composer.SetShader(null);
            }
        }
    }
}
