using Emotion.Common;
using Emotion.Game.Animation;
using Emotion.Game.Time;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using MusicTest.Collision;
using System;
using System.Numerics;
using Emotion.Utility;
using MusicTest.Collision.Collision;
using System.Collections.Generic;

namespace MusicTest.GameObjects
{
    public class Midori : Unit
    {
        //const float _verticalVelocity = -100;
        const float _verticalVelocity = -40;
        const float _jumpVelocity = 45;
        const float _horizontalVelocity = 11;
        const string _portraitPath = "better-midori.png";
        const string _spriteSheetPath = "Textures/better-pixel-midori-full-sheet-horizontal.png";
        const string _name = "Midori";

        // TODO: Create consts for all hardcoded numbers

        // Gravity Push
        const int _gravityPushStartingFrame = 0;
        const int _gravityPushEndingFrame = 17;
        const int _gravityPushTimeBetweenFrames = 65;
        const int _gravityPushPushFrameIndex = 15;
        const int _gravityPushDefaultRange = 350;
        const int _gravityPushDefaultPower = 40; // VelocityX
        const int _gravityPushDefaultFloatPower = -2; // VelocityY
        const float _gravityPushDefaultFloatRotation = 3.1f; // In radians

        #region Status Properties
        public bool IsGravityPushActive { get; set; }
        #endregion

        // Gravity Push
        /// <summary>
        /// The range of the Gravity Push area in a single direction in pixels.
        /// Counted from the CollisionBox center.
        /// </summary>
        public float GravityPushRange { get; set; }
        public float GravityPushPower { get; set; }
        public List<Unit> ObjectsAffectedByGravityPush { get; set; }
        public bool GravityPushNoTargetsFound { get; set; }

        // Interactions
        private int InteractRange { get; set; }
        public Unit InteractTarget { get; set; }

        public Midori(Vector3 position) : base(_name, _portraitPath, position, new Vector2(360, 360))
        {
            Name = _name;
            IsPlayer = true;

            Position = position;
            Size = new Vector2(360, 360);
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{_portraitPath}");
            PortraitSize = new Vector2(750, 1033.467f); // Size(250, 344.489f) * 3
            // Full Portrait Size is 1773x2213

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

            StartingVelocityY = _verticalVelocity;
            VelocityX = _horizontalVelocity;

            // Set CollisionBox offsets
            // Reminder: Don't forget to update the SetCollisionBox methods!
            CollisionBox = new Transform(
                X + 140,
                Y + 40,
                Z + 15,
                80,
                320
            );


            RunTimer = new AfterAndBack(150); // Progress 0

            JumpTimer = new AfterAndBack(400);
            GravityTimer = new AfterAndBack(100);
            GravityTimer.End(); // Set Progress to 1

            // Gravity Push
            IsGravityPushActive = false;
            GravityPushRange = _gravityPushDefaultRange;
            GravityPushPower = _gravityPushDefaultPower;
            ObjectsAffectedByGravityPush = new List<Unit>();
            GravityPushNoTargetsFound = false;
        }

        public Midori(Unit unit) : base(unit)
        {}

        protected override void SetCollisionBoxX(float x)
        {
            CollisionBox.X = x;
            X = x - 140;
        }

        protected override void SetCollisionBoxY(float y)
        {
            CollisionBox.Y = y;
            Y = y - 40;
        }

        public override void ResetVelocities()
        {
            StartingVelocityY = _verticalVelocity;
            VelocityX = _horizontalVelocity;
        }

        private void Interact()
        {
            foreach (Unit unit in GameContext.Scene.NonPlayerUnits)
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
                IsInteracting = false;
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
            if (!IsUnableToMove)
            {
                IsIdle = true;
            }
            
            // Interaction
            if (Engine.InputManager.IsKeyDown(Key.F) && !IsUnableToMove)
            {
                if (!IsInteracting)
                {
                    IsMovingLeft = false;
                    IsMovingRight = false;
                    IsIdle = true;
                    IsInteracting = true;
                    Interact();
                    return;
                }
                else if (GameContext.Scene.CurrentInteration.Finished)
                {
                    IsInteracting = false;
                    InteractTarget = null;
                    return;
                }
            }

            if (IsInteracting)
            {
                return;
            }

            // Movement
            if (Engine.InputManager.IsKeyHeld(Key.A) && !IsUnableToMove)
            {
                if (RunTimer.Progress == 0 && !IsMovingLeft && !IsMovingRight)
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
            
            if (Engine.InputManager.IsKeyHeld(Key.D) && !IsUnableToMove)
            {
                if (RunTimer.Progress == 0 && !IsMovingLeft && !IsMovingRight)
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

            // Jumping
            if (Engine.InputManager.IsKeyHeld(Key.Space) && IsGrounded && !IsUnableToMove)
            {
                IsGrounded = false;
                IsJumping = true;
                JumpTimer.GoInReverse();
                VelocityY = _jumpVelocity;
            }

            // Gravity Push
            if (Engine.InputManager.IsKeyHeld(Key.G) && IsGrounded && !IsUnableToMove)
            {
                IsIdle = false;
                IsMovingLeft = false;
                IsMovingRight = false;
                IsUnableToMove = true;
                IsGravityPushActive = true;

                Sprite.Reset();
            }

            // Debug
            // Teleport to X
            if (Engine.InputManager.IsKeyHeld(Key.LeftControl))
            {
                IsGrounded = true;
                IsFalling = false;
                IsJumping = false;
                JumpTimer.End();
                SetCollisionBoxY(500);
                SetCollisionBoxX(700);
            }
            else if (Engine.InputManager.IsKeyHeld(Key.RightControl))
            {
                IsGrounded = true;
                IsFalling = false;
                IsJumping = false;
                JumpTimer.End();
                SetCollisionBoxY(500);
                SetCollisionBoxX(11500);
            }

            if (Engine.InputManager.IsKeyDown(Key.Q))
            {
                CodeSwitch = !CodeSwitch;
                Console.WriteLine(CodeSwitch);
            }
        }

        public void Action_GravityPush()
        {
            // First call
            if (Sprite.CurrentFrameIndex >= _gravityPushStartingFrame && Sprite.CurrentFrameIndex < _gravityPushPushFrameIndex && !GravityPushNoTargetsFound)
            {
                Rectangle gravityPushArea = new Rectangle(
                    CollisionBox.Center.X - GravityPushRange,
                    CollisionBox.Center.Y - GravityPushRange,
                    GravityPushRange * 2,
                    GravityPushRange * 2
                );
                for (int i = 0; i < GameContext.Scene.NonPlayerUnits.Count; i++)
                {
                    Unit unit = GameContext.Scene.NonPlayerUnits[i];
                    if (unit.ToRectangle().IntersectsInclusive(gravityPushArea))
                    {
                        ObjectsAffectedByGravityPush.Add(unit);

                        unit.IsMovingLeft = false;
                        unit.IsMovingRight = false;
                        unit.IsJumping = false;
                        unit.IsFalling = false;
                        unit.IsGrounded = false;
                        unit.IsIdle = false;

                        unit.IsUnableToMove = true;
                        unit.IsAffectedByGravityPush = true;

                        unit.VelocityX = 0;
                        unit.VelocityY = _gravityPushDefaultFloatPower;
                        unit.StartingVelocityY = _gravityPushDefaultFloatPower;
                        unit.GravityPushPushDurationTimer = null;
                    }
                }
                // If no targets are found the first time, save that to avoid this loop check on next frames
                // We can't check the frame index because we can't set the CurrentFrameIndex :(
                GravityPushNoTargetsFound = true;
            }
            else if (Sprite.CurrentFrameIndex == _gravityPushPushFrameIndex)
            {
                for (int i = 0; i < GameContext.Scene.NonPlayerUnits.Count; i++)
                {
                    Unit unit = GameContext.Scene.NonPlayerUnits[i];
                    if (unit.Center.X < Center.X)
                    {
                        // If the unit is on the left push it to the left
                        unit.VelocityX = -GravityPushPower;
                    }
                    else
                    {
                        // If the unit is on the right push it to the right
                        unit.VelocityX = GravityPushPower;
                    }
                    unit.GravityPushPushDurationTimer = new AfterAndBack(1000);
                    unit.GravityPushPushDurationTimer.End(); // Set Progress to 1
                    unit.GravityPushPushDurationTimer.GoInReverse();
                }
            }
            //else if (Sprite.CurrentFrameIndex > _gravityPushPushFrameIndex)
            //{
            //    // Nothing?
            //}
            else if (Sprite.CurrentFrameIndex == _gravityPushEndingFrame)
            {
                // Release the units
                //for (int i = 0; i < GameContext.Scene.NonPlayerUnits.Count; i++)
                //{
                //    Unit unit = GameContext.Scene.NonPlayerUnits[i];
                //    unit.ResetVelocities();
                //    unit.IsUnableToMove = false;
                //    unit.IsAffectedByGravityPush = false;
                //    unit.IsIdle = true;
                //}

                ObjectsAffectedByGravityPush.Clear();
                IsIdle = true;
                IsUnableToMove = false;
                IsGravityPushActive = false;

                GravityPushNoTargetsFound = false;
            }
        }

        public override void Update()
        {
            // We manage the input first so we can immediately react to it in the base.Update() before changing the animation
            // This prevents an animation being set for an input that has been negated by the base.Update()
            ManageInput();
            base.Update();

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
            else if (IsGravityPushActive)
            {
                Sprite.StartingFrame = _gravityPushStartingFrame;
                Sprite.EndingFrame = _gravityPushEndingFrame;
                Sprite.TimeBetweenFrames = _gravityPushTimeBetweenFrames;
            }

            Sprite.Update(Engine.DeltaTime);

            // Gravity Push
            // Positioned after Sprite.Update because it needs the updated sprite frame index
            if (IsGravityPushActive)
            {
                Action_GravityPush();
            }
        }

        public override void Render(RenderComposer composer)
        {
            if (InclineAngle != 0f)
            {
                composer.PushModelMatrix(
                    Matrix4x4.CreateRotationZ(InclineAngle, new Vector3(Center, 0))
                );
            }
            composer.RenderSprite(
                Position,
                new Vector2(360, 360),
                !IsGravityPushActive ? Color.White : Color.Pink,
                Sprite.Texture,
                Sprite.CurrentFrame,
                IsFacingRight, false
            );
            if (InclineAngle != 0f)
            {
                composer.PopModelMatrix();
            }

            if (IsGravityPushActive)
            {
                composer.RenderOutline(
                    new Vector3(
                        CollisionBox.Center.X - GravityPushRange,
                        CollisionBox.Center.Y - GravityPushRange,
                        15
                    ),
                    new Vector2(GravityPushRange * 2),
                    Color.Pink,
                    2
                );
            }
        }
    }
}
