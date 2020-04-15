using Emotion.Common;
using Emotion.Game.Animation;
using Emotion.Game.Time;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
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
        }

        protected void ManageMovement(Room currentRoom) 
        {
            
            if (IsMovingLeft) 
            {
                // Make sure movement is withing room borders
                if (X > 0 + VelocityX)
                {
                    X -= VelocityX;
                }
                else
                {
                    X = 0;
                }
            }
            
            if (IsMovingRight)
            {
                // Make sure movement is withing room borders
                if (X + Size.X < currentRoom.Size.X - VelocityX)
                {
                    X += VelocityX;
                }
                else
                {
                    X = currentRoom.Size.X - Size.X;
                }
            }

            if (isJumping)
            {
                if (JumpTimer.Finished && !isFalling)
                {
                    isFalling = true;
                    //isJumping = false;
                    JumpTimer.GoNormal();
                }

                if (isFalling)
                {
                    VelocityY = _gravityVelocity;
                }

                Rectangle futurePosition = new Rectangle(X, Y - (JumpTimer.Progress * VelocityY), Width, Height);
                if (!Collision.IntersectsWithPlatforms(futurePosition))
                {
                    Y -= JumpTimer.Progress * VelocityY;
                }
                else
                {
                    isJumping = false;
                    isFalling = false;
                    isGrounded = true;
                    JumpTimer.End();
                    VelocityY = 0;
                    Y = 580;
                }
            }
            else
            {
                //ApplyGravity();
            }

        }

        protected void ApplyGravity() 
        {
            Rectangle futurePosition = new Rectangle(X, Y - (GravityTimer.Progress * _gravityVelocity), Width, Height);
            if (!Collision.IntersectsWithPlatforms(futurePosition))
            {
                Y -= GravityTimer.Progress * _gravityVelocity;
                isGrounded = false;

                // If we're not mid-jump then we've started falling
                if (!isJumping)
                {
                    isFalling = true;
                    GravityTimer.GoNormal();
                }
            }
            else
            {
                isJumping = false;
                isFalling = false;
                isGrounded = true;
                JumpTimer.End();
                VelocityY = 0;
                Y = 580;
            }
        }

        public virtual void Update(Room currentRoom) 
        {
            ManageMovement(currentRoom);

            JumpTimer.Update();
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

            if (isInteracting) 
            {
                string text = Dialogues[0].DialogueLines[0];
                composer.RenderString(Position + new Vector3(-30, -30, 0), Color.Black, text, Engine.AssetLoader.Get<FontAsset>("debugFont.otf").GetAtlas(16));
            }
        }
    }
}
