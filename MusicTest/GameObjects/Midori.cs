﻿using Emotion.Common;
using Emotion.Game.Animation;
using Emotion.Graphics;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using Emotion.Utility;
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
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>("Textures/midori.png");

            TextureAsset spriteAsset = Engine.AssetLoader.Get<TextureAsset>("Textures/pixel-midori-full-sheet-transparent.png");
            Sprite = new AnimatedTexture(
                spriteAsset.Texture,
                new Vector2(24, 24),
                AnimationLoopType.Normal,
                300,
                0,
                2
            );

            InteractRange = 360;
            InteractionOffset = new Vector2(0, 70);
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
            else
            {
                if (Center.X < InteractTarget.Center.X && InteractTarget.IsFacingRight)
                {
                    InteractTarget.IsFacingRight = false;
                }
                else if (Center.X > InteractTarget.Center.X && !InteractTarget.IsFacingRight)
                {
                    InteractTarget.IsFacingRight = true;
                }
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

            if (IsIdle)
            {
                Sprite.StartingFrame = 0;
                Sprite.EndingFrame = 1;
                Sprite.TimeBetweenFrames = 1500;
            }
            else if (IsMovingLeft || IsMovingRight)
            {
                Sprite.StartingFrame = 13;
                Sprite.EndingFrame = 25;
                Sprite.TimeBetweenFrames = 75;
            }

            Sprite.Update(Engine.DeltaTime);
        }

        public override void Render(RenderComposer composer)
        {
            //composer.RenderSprite(
            //    Position,
            //    Size,
            //    Color.White,
            //    TextureAsset.Texture,
            //    null,
            //    IsFacingRight
            //);

            composer.PushModelMatrix(
                Matrix4x4.CreateRotationZ(Maths.DegreesToRadians(45), new Vector3(Center, 0))
            );
            composer.RenderSprite(
                Position,
                new Vector2(360, 360),
                Color.White,
                Sprite.Texture,
                Sprite.CurrentFrame,
                IsFacingRight, false
            );
            composer.RenderOutline(Position, new Vector2(360, 360), Color.Red, 1);

            //composer.RenderCircleOutline(new Vector3(Center, Z), InteractRange, Color.Red, true);
            composer.RenderOutline(Position, Size, Color.Red, 1);

            composer.PopModelMatrix();
        }
    }
}