using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MusicTest.GameObjects
{
    public class Midori : Unit
    {
        public Midori(Vector3 position) : base("Midori", "midori.png", position, new Vector2(250, 344.489f))
        {
            Position = position;

            Name = "Midori";
            Size = new Vector2(250, 344.489f); // Full Size 1606x2213
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>("midori.png");
        }

        protected void ManageInput(Room currentRoom)
        {
            IsIdle = true;
            IsMovingLeft = false;
            IsMovingRight = false;

            if (Engine.InputManager.IsKeyHeld(Key.A))
            {
                IsMovingLeft = true;
                IsIdle = false;
                IsFacingRight = false;
            }
            if (Engine.InputManager.IsKeyHeld(Key.D))
            {
                IsMovingRight = true;
                IsIdle = false;
                IsFacingRight = true;
            }
        }

        public override void Update(Room currentRoom)
        {
            base.Update(currentRoom);
            ManageInput(currentRoom);
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
