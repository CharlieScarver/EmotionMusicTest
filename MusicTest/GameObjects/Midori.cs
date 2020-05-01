using Emotion.Common;
using Emotion.Game.Animation;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using MusicTest.Core;
using System;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Midori : Unit
    {
        const float _gravityVelocity = -30;
        const float _horizontalVelocity = 9;
        const string _portraitPath = "better-midori.png";
        const string _spriteSheetPath = "Textures/better-pixel-midori-full-sheet-horizontal.png";
        const string _name = "Midori";

        public Midori(Vector3 position) : base(_name, _portraitPath, position, new Vector2(250, 344.489f))
        {
            Name = _name;
            IsPlayer = true;

            Position = position;
            Size = new Vector2(250, 344.489f); // Full Size 1773x2213
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{_portraitPath}");

            TextureAsset spriteAsset = Engine.AssetLoader.Get<TextureAsset>(_spriteSheetPath);
            Sprite = new AnimatedTexture(
                spriteAsset,
                new Vector2(24, 24),
                AnimationLoopType.Normal,
                1500,
                0,
                1
            );

            InteractRange = 360;
            InteractionOffset = new Vector2(0, 70);

            StartingVelocityY = _gravityVelocity;
            VelocityX = _horizontalVelocity;

            CollisionBox = new Transform(
                X + 130,
                Y,
                Z,
                Width / 3,
                Height
            );
        }

        private int InteractRange { get; set; }
        public Unit InteractTarget { get; set; }

        protected override void SetCollisionBoxX(float x)
        {
            CollisionBox.X = x;
            X = x - 130;
        }

        protected override void SetCollisionBoxY(float y)
        {
            CollisionBox.Y = y;
            Y = y;
        }

        private void Interact()
        {
            foreach (Unit unit in GameContext.Scene.Units)
            {
                if (unit.IsPlayer)
                {
                    continue;
                }

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
            //IsMovingLeft = false;
            //IsMovingRight = false;
            
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
                else if (GameContext.Scene.CurrentInteration.Finished)
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
                if (RunTimer.Progress == 0 && !IsMovingLeft && !IsMovingRight && isGrounded)
                {
                    RunTimer.GoNormal();
                    Sprite.Reset();
                }
                IsMovingLeft = true;
                IsMovingRight = false;
                IsIdle = false;
                IsFacingRight = false;
            }
            else
            {
                IsMovingLeft = false;
                if (!IsMovingLeft && !IsMovingRight)
                {
                    RunTimer.GoInReverse();
                    RunTimer.End();
                }
            }
            //else
            //{
            //    if (IsMovingLeft && RunTimer.Progress == 1)
            //    {
            //        RunTimer.GoInReverse();
            //    }
            //    else if (IsMovingLeft && RunTimer.Progress == 0)
            //    {
            //        IsMovingLeft = false;
            //    }
            //}
            
            if (Engine.InputManager.IsKeyHeld(Key.D))
            {
                if (RunTimer.Progress == 0 && !IsMovingLeft && !IsMovingRight && isGrounded)
                {
                    RunTimer.GoNormal();
                    Sprite.Reset();
                }
                IsMovingRight = true;
                IsMovingLeft = false;
                IsIdle = false;
                IsFacingRight = true;
            }
            else
            {
                IsMovingRight = false;
                if (!IsMovingLeft && !IsMovingRight)
                {
                    RunTimer.GoInReverse();
                    RunTimer.End();
                }
            }
            //else
            //{
            //    if (IsMovingRight && RunTimer.Progress == 1)
            //    {
            //        RunTimer.GoInReverse();
            //    }
            //    else if (IsMovingRight && RunTimer.Progress == 0)
            //    {
            //        IsMovingLeft = false;
            //    }
            //}

            // Jumping
            if (Engine.InputManager.IsKeyHeld(Key.Space) && isGrounded)
            {
                isGrounded = false;
                isJumping = true;
                JumpTimer.GoInReverse();
                VelocityY = 20;
            }

            if (Engine.InputManager.IsKeyHeld(Key.LeftControl))
            {
                isGrounded = true;
                isFalling = false;
                isJumping = false;
                JumpTimer.End();
                SetCollisionBoxY(500);
                SetCollisionBoxX(700);
            }

            if (Engine.InputManager.IsKeyDown(Key.Q))
            {
                CodeSwitch = !CodeSwitch;
                Console.WriteLine(CodeSwitch);
            }
        }

        public override void Update()
        {
            base.Update();
            ManageInput();

            // TODO: Keep last state
            // TODO: Execute the following code once per change instead of on every frame
            // After that use Sprite.Reset() to avoid the fast-forwarded frames caused by the frame rate switching
            if (IsIdle)
            {
                Sprite.StartingFrame = 0;
                Sprite.EndingFrame = 1;
                Sprite.TimeBetweenFrames = 1500;
                //Sprite.Reset();
            }
            else if (IsMovingLeft || IsMovingRight)
            {
                Sprite.StartingFrame = 3;
                Sprite.EndingFrame = 17;
                Sprite.TimeBetweenFrames = 65;
               // Sprite.Reset();
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

            if (InclineAngle != 0f)
            {
                composer.PushModelMatrix(
                    Matrix4x4.CreateRotationZ(InclineAngle, new Vector3(Center, 0))
                );
            }
            composer.RenderSprite(
                Position,
                new Vector2(360, 360),
                Color.White,
                Sprite.Texture,
                Sprite.CurrentFrame,
                IsFacingRight, false
            );
            if (InclineAngle != 0f)
            {
                composer.PopModelMatrix();
            }
            //composer.RenderOutline(Position, new Vector2(360, 360), Color.Red, 1);

            //composer.RenderCircleOutline(new Vector3(Center, Z), InteractRange, Color.Red, true);
            //composer.RenderOutline(Position, Size, Color.Red, 1);

            Rectangle rect = CollisionBox.ToRectangle();
            //Vector3 topLeft = new Vector3(rect.TopLeft, 10);
            //Vector3 topRight = new Vector3(rect.TopRight, 10);
            //Vector3 bottomRight = new Vector3(rect.BottomRight, 10);
            //Vector3 bottomLeft = new Vector3(rect.X, rect.Bottom, 10);
            //composer.RenderLine(topLeft, topRight, Color.Red);
            //composer.RenderLine(topRight, bottomRight, Color.Green);
            //composer.RenderLine(bottomRight, bottomLeft, Color.Cyan);
            //composer.RenderLine(bottomLeft, topLeft, Color.Yellow);

            //Rectangle futurePosition = new Rectangle(X, Y - (GravityTimer.Progress * _gravityVelocity), Width, Height);
            //composer.RenderOutline(futurePosition, Color.Green, 1);
            //composer.PopModelMatrix();
        }
    }
}
