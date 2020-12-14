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

        /// <summary>
        /// Rotation in radians. 
        /// Decorations rotate around the bottom left corner to match Tiled rotation.
        /// </summary>
        public float Rotation { get; set; }

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

        public Decoration(string name, string textureName, Vector2 size, Vector3 position, Vector2? displaySize = null, Rectangle? textureArea = null, bool flipX = false, float rotation = 0, float blurIntensity = 0, int shadowReverseIntensity = 255) {
            Name = name;
            TextureName = textureName;
            // Used in MainScene.IsTransformOnSreen()
            Size = size;
            Position = position;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{TextureName}");
            DisplaySize = displaySize ?? size;
            TextureArea = textureArea;
            FlipX = flipX;
            Rotation = rotation;

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

            if (BlurIntensity > 0)
            {
                // Apply Blur effect if such is set
                composer.SetShader(BlurShader.Shader);
                BlurShader.Shader.SetUniformFloat("sigma", BlurIntensity);
            }

            Color color = Color.White;
            if (ShadowReverseIntensity < 255)
            {
                // Apply Shadow effect if such is set
                color = new Color(ShadowReverseIntensity, ShadowReverseIntensity, ShadowReverseIntensity); // 180 for a shadowy look
            }

            if (Rotation != 0f)
            {
                // Apply rotation if such is set
                // Tiled rotates images around the bottom left corner
                 composer.PushModelMatrix(
                    Matrix4x4.CreateRotationZ(Rotation, new Vector3(X, Y + DisplaySize.Y, 0))
                );
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

            if (Rotation != 0f)
            {
                // Remove the rotation matrix
                composer.PopModelMatrix();
            }

            if (BlurIntensity > 0)
            {
                // Remove the blur shader
                composer.SetShader(null);
            }
        }
    }
}
