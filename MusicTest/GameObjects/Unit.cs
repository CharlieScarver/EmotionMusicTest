using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class Unit : TransformRenderable
    {
        public string Name { get; set; }
        public string TextureName { get; set; }

        public TextureAsset TextureAsset { get; set; }

        public int VelocityX { get; set; } = 6;

        public List<DialoguePiece> Dialogues { get; set; }

        #region Status Properties
        public bool IsIdle { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsMovingRight { get; set; }
        public bool IsFacingRight { get; set; }
        public bool isInteracting { get; set; }
        #endregion

        public Unit(string name, string textureName, Vector3 position, Vector2 size)
        {
            Name = name;
            TextureName = textureName;
            Position = position;
            Size = size;

            TextureAsset = Engine.AssetLoader.Get<TextureAsset>(TextureName);
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

            if (isInteracting) 
            {
                //string text = Dialogues[0].DialogueLines[0];
                //composer.RenderString(Position + new Vector3(-30, -30, 0), Color.Black, text, Engine.AssetLoader.Get<FontAsset>("debugFont.otf").Font.GetAtlas(16));
            }
        }
    }
}
