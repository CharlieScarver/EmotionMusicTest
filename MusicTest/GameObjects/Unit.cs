using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Unit : TransformRenderable
    {
        public string Name { get; set; }

        public TextureAsset TextureAsset { get; set; }

        public int VelocityX { get; set; } = 5;

        public bool IsFacingRight { get; set; }

        public Unit(Vector3 position)
        {
            Position = position;

            Name = "Midori";
            Size = new Vector2(250, 344.489f); // Full Size 1606x2213
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>("midori.png");
        }

        public void Update(Room currentRoom) 
        {
            if (Engine.InputManager.IsKeyHeld(Key.A))
            {
                if (X > 0 + VelocityX)
                {
                    X -= 5;
                    IsFacingRight = false;
                }
            }
            else if (Engine.InputManager.IsKeyHeld(Key.D))
            {
                if (X + Size.X < currentRoom.Size.X - VelocityX)
                {
                    X += 5;
                    IsFacingRight = true;
                }
            }
        }

        public override void Render(RenderComposer composer)
        {
            composer.RenderSprite(
                Position,
                Size,
                Color.White,
                TextureAsset.Texture,
                null,
                IsFacingRight
            );
        }
    }
}
