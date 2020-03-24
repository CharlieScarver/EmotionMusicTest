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

        public int VelocityX { get; set; } = 6;

        #region Status Properties
        public bool IsIdle { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsMovingRight { get; set; }
        public bool IsFacingRight { get; set; }
        public bool isInteracting { get; set; }
        #endregion

        public Unit(Vector3 position)
        {
            Position = position;
        }

        public Unit(string name, Vector3 position, Vector2 size, string textureFileName)
        {
            Name = name;
            Position = position;
            Size = size;
            TextureAsset = Engine.AssetLoader.Get<TextureAsset>(textureFileName);
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
        }

        public virtual void Update(Room currentRoom) 
        {
            ManageMovement(currentRoom);
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
