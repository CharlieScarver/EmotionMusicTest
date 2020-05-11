using Emotion.Common;
using Emotion.Game.Animation;
using Emotion.Game.Time;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Primitives;
using Emotion.Utility;
using MusicTest.Collision;
using MusicTest.RoomData;
using MusicTest.Collision.Collision;
using System;
using System.Collections.Generic;
using System.Numerics;
using MusicTest.Interactions;

namespace MusicTest.GameObjects
{
    public abstract class Unit : GameObject, IGameObject
    {
        const float _defaultHorizontalVelocity = 9;
        const float _defaultVerticalVelocity = -30;
        const float _jumpHeight = 200;
        const float _jumpVelocity = 100;

        // Textures
        public string TextureName { get; set; }
        public TextureAsset TextureAsset { get; set; }
        public AnimatedTexture Sprite { get; set; }
        public Vector2 PortraitSize { get; set; }
        
        // Moving
        public float VelocityX { get; set; }
        public AfterAndBack RunTimer { get; set; }
               
        // Jumping
        public AfterAndBack JumpTimer { get; set; }
        public AfterAndBack GravityTimer { get; set; }
        public float VelocityY { get; set; }
        public float StartingVelocityY { get; set; }
        
        // Collision
        public Transform CollisionBox { get; set; }
        public float InclineAngle { get; set; }

        // Interaction
        public Vector2 InteractionOffset { get; set; }
        public bool IsPlayer { get; set; }

        // Gravity Push
        public AfterAndBack GravityPushPushDurationTimer { get; set; }

        // Other
        public List<DialoguePiece> Dialogues { get; set; }

        // Debug
        public bool CodeSwitch { get; set; }
        public Unit LastState { get; set; }

        #region Status Properties
        public bool IsIdle { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsMovingRight { get; set; }
        public bool IsFacingRight { get; set; }
        public bool IsInteracting { get; set; }
        public bool IsJumping { get; set; }
        public bool IsFalling { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsUnableToMove { get; set; }
        public bool IsAffectedByGravityPush { get; set; }
        #endregion

        public Unit(string name, string textureName, Vector3 position, Vector2 size)
        {
            Name = name;
            TextureName = textureName;
            Position = position;
            Size = size;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{TextureName}");

            IsPlayer = false;
            IsIdle = true;
            IsGrounded = true;

            RunTimer = new AfterAndBack(150); // Progress 0

            JumpTimer = new AfterAndBack(600);
            GravityTimer = new AfterAndBack(300);
            GravityTimer.End(); // Set Progress to 1

            StartingVelocityY = _defaultVerticalVelocity;
            VelocityX = _defaultHorizontalVelocity;

            InclineAngle = 0;
        }

        public Unit(Unit unit) 
        {
            Position = unit.Position;
            Size = unit.Size;

            TextureName = unit.TextureName;
            TextureAsset = unit.TextureAsset;
            Sprite = unit.Sprite;
            VelocityX = unit.VelocityX;
            RunTimer = unit.RunTimer;
            JumpTimer = unit.JumpTimer;
            GravityTimer = unit.GravityTimer;
            VelocityY = unit.VelocityY;
            StartingVelocityY = unit.StartingVelocityY;
            CollisionBox = unit.CollisionBox;
            InclineAngle = unit.InclineAngle;
            InteractionOffset = unit.InteractionOffset;
            IsPlayer = unit.IsPlayer;

            IsIdle = unit.IsIdle;
            IsMovingLeft = unit.IsMovingLeft;
            IsMovingRight = unit.IsMovingRight;
            IsFacingRight = unit.IsFacingRight;
            IsInteracting = unit.IsInteracting;
            IsJumping = unit.IsJumping;
            IsFalling = unit.IsFalling;
            IsGrounded = unit.IsGrounded;
        }

        protected abstract void SetCollisionBoxX(float x);
        protected abstract void SetCollisionBoxY(float y);
        public virtual void ResetVelocities()
        {

            VelocityX = _defaultHorizontalVelocity;
            StartingVelocityY = _defaultVerticalVelocity;
            VelocityY = _defaultVerticalVelocity;
        }

        protected void ManageHorizontalMovement(Rectangle futurePosition)
        {
            Collision.LineSegment intersectedPlatform = Collision.Collision.Collision.IntersectsWithPlatforms(futurePosition);

            if (intersectedPlatform == null)
            {
                SetCollisionBoxX(futurePosition.X);
                InclineAngle = 0;
            }
            else
            {
                if (intersectedPlatform.IsSloped)
                {
                    do
                    {
                        // Set the incline angle
                        InclineAngle = intersectedPlatform.InclineAngleWithX;
                        // Check for collision a little higher
                        futurePosition.Y -= 3;
                        intersectedPlatform = Collision.Collision.Collision.IntersectsWithSlopedPlatforms(futurePosition);
                    }
                    while (intersectedPlatform != null);

                    // Proceed forward and climb the slope
                    SetCollisionBoxX(futurePosition.X);
                    SetCollisionBoxY(futurePosition.Y);
                }
                else
                {
                    // If there is a step smaller than your horizontal velocity, climb it
                    if ((CollisionBox.Y + CollisionBox.Height) - intersectedPlatform.PointA.Y - 1 <= VelocityX)
                    {
                        SetCollisionBoxX(futurePosition.X);
                        SetCollisionBoxY(intersectedPlatform.PointA.Y - CollisionBox.Height - 1);
                    }
                    // Else don't move, because the movement is invalid
                }
            }
        }

        protected void ManageMovement()
        {
            if (IsUnableToMove)
            {
                return;
            }

            Room currentRoom = GameContext.Scene.LoadedRoom;
            float newVelocity = VelocityX * RunTimer.Progress;

            if (IsMovingLeft) 
            {
                // Make sure movement is within the room borders
                if (CollisionBox.X > 0 + newVelocity)
                {
                    Rectangle futurePosition = new Rectangle(CollisionBox.X - newVelocity, CollisionBox.Y, CollisionBox.Width, CollisionBox.Height);
                    ManageHorizontalMovement(futurePosition);
                }
                else
                {
                    SetCollisionBoxX(0);
                    IsMovingLeft = false;
                    IsIdle = true;
                }
            }
            
            if (IsMovingRight)
            {
                // Make sure movement is within the room borders
                if (CollisionBox.X + CollisionBox.Width < currentRoom.Size.X - newVelocity)
                {
                    Rectangle futurePosition = new Rectangle(CollisionBox.X + newVelocity, CollisionBox.Y, CollisionBox.Width, CollisionBox.Height);
                    ManageHorizontalMovement(futurePosition);
                }
                else
                {
                    SetCollisionBoxX(currentRoom.Size.X - CollisionBox.Width);
                    IsMovingRight = false;
                    IsIdle = true;
                }
            }

            if (IsJumping)
            {
                if (JumpTimer.Finished && !IsFalling)
                {
                    IsFalling = true;
                    JumpTimer.GoNormal();
                }

                if (IsFalling)
                {
                    VelocityY = StartingVelocityY;
                }

                Rectangle futurePosition = new Rectangle(CollisionBox.X, CollisionBox.Y - (JumpTimer.Progress * VelocityY), CollisionBox.Width, CollisionBox.Height);
                Collision.LineSegment intersectedPlatform = Collision.Collision.Collision.IntersectsWithPlatforms(futurePosition);
                if (intersectedPlatform == null)
                {
                    SetCollisionBoxY(CollisionBox.Y - JumpTimer.Progress * VelocityY);
                }
                else
                {
                    IsJumping = false;
                    IsFalling = false;
                    IsGrounded = true;
                    JumpTimer.End();
                    //Y = 580;
                }
            }
            else
            {
                VelocityY = StartingVelocityY;
                ApplyGravity();
            }
        }

        protected void ApplyGravity() 
        {
            Rectangle futurePosition = new Rectangle(CollisionBox.X, CollisionBox.Y - VelocityY, CollisionBox.Width, CollisionBox.Height);
            Collision.LineSegment intersectedPlatform = Collision.Collision.Collision.IntersectsWithSlopedPlatforms(futurePosition) ?? Collision.Collision.Collision.IntersectsWithAxisAlignedPlatforms(futurePosition);

            if (intersectedPlatform == null)
            {
                SetCollisionBoxY(CollisionBox.Y - VelocityY);
                IsGrounded = false;
                IsFalling = true;
            }
            else
            {
                if (intersectedPlatform.IsSloped)
                {
                    do
                    {
                        // Set the incline angle
                        if ((intersectedPlatform.IsAscending && CollisionBox.Y + CollisionBox.Height < intersectedPlatform.PointB.Y) ||
                            (!intersectedPlatform.IsAscending && CollisionBox.Y + CollisionBox.Height < intersectedPlatform.PointA.Y))
                        {
                            // Don't apply incline angle if we are completely above the platform
                            InclineAngle = 0;
                        }
                        else
                        {
                            InclineAngle = intersectedPlatform.IsAscending
                                ? Maths.DegreesToRadians(360) - intersectedPlatform.InclineAngleWithX
                                : intersectedPlatform.InclineAngleWithX;
                        }
                        // Check for collision a little higher
                        futurePosition.Y -= Math.Abs(StartingVelocityY * 0.1f);
                        intersectedPlatform = Collision.Collision.Collision.IntersectsWithSlopedPlatforms(futurePosition);
                    }
                    while (intersectedPlatform != null);

                    SetCollisionBoxY(futurePosition.Y);
                }
                else
                {
                    InclineAngle = 0;
                    float distanceToPlatform = intersectedPlatform.PointA.Y - (CollisionBox.Y + CollisionBox.Height) - 1;
                    SetCollisionBoxY(CollisionBox.Y + distanceToPlatform);
                }
                IsGrounded = true;
                IsFalling = false;
            }
        }

        public virtual void Update()
        {
            ManageMovement();

            if (IsAffectedByGravityPush)
            {
                float multiplier = 1;
                if (GravityPushPushDurationTimer != null)
                {
                    multiplier = GravityPushPushDurationTimer.Progress;
                    GravityPushPushDurationTimer.Update();
                }
                
                // Horizontal
                Rectangle futurePositionX = new Rectangle(CollisionBox.X + VelocityX * multiplier, CollisionBox.Y, CollisionBox.Width, CollisionBox.Height);
                ManageHorizontalMovement(futurePositionX);
                
                // Vertical
                Rectangle futurePositionY = new Rectangle(CollisionBox.X, CollisionBox.Y + VelocityY, CollisionBox.Width, CollisionBox.Height);
                SetCollisionBoxY(futurePositionY.Y);

                // Rotate clockwise in the air
                // Make one full rotation for the duration of the timer
                InclineAngle = 2 * (float)Math.PI * multiplier;

                // Return the state of the unit back to normal after the push is over
                if (GravityPushPushDurationTimer != null && GravityPushPushDurationTimer.Progress == 0)
                {
                    ResetVelocities();
                    IsUnableToMove = false;
                    IsAffectedByGravityPush = false;
                    IsIdle = true;
                    GravityPushPushDurationTimer = null;
                    InclineAngle = 0;
                }
            }


            RunTimer.Update();
            JumpTimer.Update();

            LastState = new Midori(this);
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
                Size,
                !IsAffectedByGravityPush ? Color.White : Color.Pink,
                TextureAsset.Texture,
                null,
                IsFacingRight
            );
            if (InclineAngle != 0f)
            {
                composer.PopModelMatrix();
            }

            if (IsInteracting) 
            {
                string text = Dialogues[0].DialogueLines[0];
                composer.RenderString(Position + new Vector3(-30, -30, 0), Color.Black, text, Engine.AssetLoader.Get<FontAsset>("debugFont.otf").GetAtlas(16));
            }


            //Rectangle futurePosition = new Rectangle(CollisionBox.X, CollisionBox.Y - (GravityTimer.Progress * StartingVelocityY), CollisionBox.Width, CollisionBox.Height);
            //composer.RenderOutline(futurePosition, Color.Green, 1);
            //composer.RenderOutline(CollisionBox.ToRectangle(), Color.Red, 2);
        }
    }
}
