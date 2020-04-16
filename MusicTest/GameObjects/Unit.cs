using Emotion.Common;
using Emotion.Game.Animation;
using Emotion.Game.Time;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using Emotion.Utility;
using MusicTest.Core;
using MusicTest.Core.Collision;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Unit : TransformRenderable
    {
        const float _gravityVelocity = -30;
        const float _jumpHeight = 200;
        const float _jumpVelocity = 100;

        public string Name { get; set; }
        public string TextureName { get; set; }

        public TextureAsset TextureAsset { get; set; }

        public AnimatedTexture Sprite { get; set; }

        public int VelocityX { get; set; } = 6;

        public List<DialoguePiece> Dialogues { get; set; }

        public Vector2 InteractionOffset { get; set; }

        // Jumping
        public AfterAndBack JumpTimer { get; set; }
        public AfterAndBack GravityTimer { get; set; }
        public float VelocityY { get; set; }
        public float StartingVelocityY { get; set; }

        public float InclineAngle { get; set; }

        public Transform CollisionBox { get; set; }

        #region Status Properties
        public bool IsIdle { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsMovingRight { get; set; }
        public bool IsFacingRight { get; set; }
        public bool isInteracting { get; set; }
        public bool isJumping { get; set; }
        public bool isFalling { get; set; }
        public bool isGrounded { get; set; }
        #endregion

        public Unit(string name, string textureName, Vector3 position, Vector2 size)
        {
            Name = name;
            TextureName = textureName;
            Position = position;
            Size = size;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>($"Textures/{TextureName}");

            JumpTimer = new AfterAndBack(600);
            GravityTimer = new AfterAndBack(300);
            GravityTimer.End(); // Set Progress to 1

            IsIdle = true;
            isGrounded = true;

            InclineAngle = 0;
        }

        protected void ManageMovement(Room currentRoom) 
        {
            
            if (IsMovingLeft) 
            {
                // Make sure movement is withing room borders
                if (CollisionBox.X > 0 + VelocityX)
                {
                    CollisionPlatform platform;

                    Rectangle futurePosition = new Rectangle(CollisionBox.X - VelocityX, CollisionBox.Y, CollisionBox.Width, CollisionBox.Height);
                    platform = Collision.IntersectsWithSlopedPlatforms(futurePosition);

                    if (platform == null)
                    {
                        X -= VelocityX;
                        CollisionBox.X -= VelocityX;
                        InclineAngle = 0;
                    }
                    else
                    {
                        do 
                        {
                            futurePosition.Y -= 3;
                            InclineAngle = platform.InclineAngleWithX;
                            platform = Collision.IntersectsWithSlopedPlatforms(futurePosition);
                        }
                        while (platform != null);

                        X -= VelocityX;
                        Y = futurePosition.Y;
                        CollisionBox.X -= VelocityX;
                        CollisionBox.Y = futurePosition.Y;
                    }
                }
                else
                {
                    X = 0;
                    CollisionBox.X = 0;
                }
            }
            
            if (IsMovingRight)
            {
                // Make sure movement is withing room borders
                if (CollisionBox.X + CollisionBox.Width < currentRoom.Size.X - VelocityX)
                {
                    CollisionPlatform platform;

                    Rectangle futurePosition = new Rectangle(CollisionBox.X + VelocityX, CollisionBox.Y, CollisionBox.Width, CollisionBox.Height);
                    platform = Collision.IntersectsWithSlopedPlatforms(futurePosition);

                    if (platform == null)
                    {
                        X += VelocityX;
                        CollisionBox.X += VelocityX;
                        InclineAngle = 0;
                    }
                    else
                    {
                        do
                        {
                            futurePosition.Y -= 3;
                            InclineAngle = platform.InclineAngleWithX;
                            platform = Collision.IntersectsWithSlopedPlatforms(futurePosition);
                        }
                        while (platform != null);
                        
                        X += VelocityX;
                        Y = futurePosition.Y;
                        CollisionBox.X += VelocityX;
                        CollisionBox.Y = futurePosition.Y;
                    }
                }
                else
                {
                    X = currentRoom.Size.X - Size.X;
                    CollisionBox.X = currentRoom.Size.X - Size.X;
                }
            }

            if (isJumping)
            {
                if (JumpTimer.Finished && !isFalling)
                {
                    isFalling = true;
                    JumpTimer.GoNormal();
                }

                if (isFalling)
                {
                    VelocityY = _gravityVelocity;
                }

                Rectangle futurePosition = new Rectangle(CollisionBox.X, CollisionBox.Y - (JumpTimer.Progress * VelocityY), CollisionBox.Width, CollisionBox.Height);
                if (!Collision.IntersectsWithPlatforms(futurePosition))
                {
                    Y -= JumpTimer.Progress * VelocityY;
                    CollisionBox.Y -= JumpTimer.Progress * VelocityY;
                }
                else
                {
                    isJumping = false;
                    isFalling = false;
                    isGrounded = true;
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

            CollisionPlatform platform = Collision.IntersectsWithSlopedPlatforms(futurePosition);
            if (platform != null)
            {
                InclineAngle = platform.InclineAngleWithX;
            }

            if (!Collision.IntersectsWithPlatforms(futurePosition) && platform == null)
            {
                Y -= VelocityY;
                CollisionBox.Y -= VelocityY;
                isGrounded = false;
                isFalling = true;
            }
            else
            {
                isGrounded = true;
                isFalling = false;
            }
        }

        public virtual void Update(Room currentRoom) 
        {
            ManageMovement(currentRoom);

            JumpTimer.Update();
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
                Color.White,
                TextureAsset.Texture,
                null,
                IsFacingRight
            );
            if (InclineAngle != 0f)
            {
                composer.PopModelMatrix();
            }

            if (isInteracting) 
            {
                string text = Dialogues[0].DialogueLines[0];
                composer.RenderString(Position + new Vector3(-30, -30, 0), Color.Black, text, Engine.AssetLoader.Get<FontAsset>("debugFont.otf").GetAtlas(16));
            }


            Rectangle futurePosition = new Rectangle(CollisionBox.X, CollisionBox.Y - (GravityTimer.Progress * _gravityVelocity), CollisionBox.Width, CollisionBox.Height);
            composer.RenderOutline(futurePosition, Color.Green, 1);
            composer.RenderOutline(CollisionBox.ToRectangle(), Color.Red, 1);
        }
    }
}
