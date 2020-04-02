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
        public Midori(Vector3 position, MainScene scene) : base("Midori", "midori.png", position, new Vector2(250, 344.489f))
        {
            Position = position;

            Name = "Midori";
            Size = new Vector2(250, 344.489f); // Full Size 1606x2213
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>("midori.png");

            InteractRange = 200;
            Scene = scene;
        }

        private MainScene Scene { get; set; }

        private int InteractRange { get; set; }

        public Unit InteractTarget { get; set; }

        private void Interact()
        {
            foreach (Unit unit in Scene.Units)
            {
                if (unit.ToRectangle().IntersectsInclusive(this.ToRectangle()))
                {
                    if (InteractTarget != null)
                    {
                        if (Vector2.Distance(Center, unit.Center) < Vector2.Distance(Center, InteractTarget.Center))
                        {
                            InteractTarget = unit;
                        }
                    }
                    else
                    {
                        InteractTarget = unit;
                    }
                }
            }

            if (InteractTarget == null)
            {
                isInteracting = false;
            }
        }

        protected void ManageInput()
        {
            IsIdle = true;
            IsMovingLeft = false;
            IsMovingRight = false;
            
            // Interaction
            if (Engine.InputManager.IsKeyDown(Key.F))
            {
                if (!isInteracting)
                {
                    IsMovingLeft = false;
                    IsMovingRight = false;
                    IsIdle = true;
                    isInteracting = true;
                    Interact();
                    return;
                }
                else
                {
                    isInteracting = false;
                    InteractTarget = null;
                    return;
                }
            }

            if (isInteracting)
            {
                return;
            }

            // Movement
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
            ManageInput();
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

            composer.RenderCircleOutline(new Vector3(Center, Z), InteractRange, Color.Red, true);
            composer.RenderOutline(Position, Size, Color.Red, 1);
        }
    }
}
