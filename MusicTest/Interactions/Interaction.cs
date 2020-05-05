using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Primitives;
using MusicTest.GameObjects;
using System.Numerics;

namespace MusicTest.Interactions
{
    public class Interaction
    {
        private const float _backgroundZ = 10;
        private const int _defaultFontSize = 24;

        public Interaction(Unit unit1, Unit unit2)
        {
            Unit1 = unit1;
            Unit2 = unit2;

            if (Unit1.X < Unit2.X)
            {
                Unit1OnTheLeft = true;
            }
            else
            {
                Unit1OnTheLeft = false;
            }

            BackgroundZ = _backgroundZ;

            LeftPosition = new Vector3(new Vector2(0, 0), 11);
            RightPosition = new Vector3(new Vector2(1200, 0), 11);

            Texture = Engine.AssetLoader.Get<TextureAsset>("Textures/better-transparent-black.png");

            FontSize = _defaultFontSize;
            Font = Engine.AssetLoader.Get<FontAsset>("debugFont.otf");
            Finished = false;

            CurrentLetterIndex = 0;
            LastLetterFrame = 0;
            CurrentFrame = 0;

            TextScrollSpeed = 5;
        }

        private Unit Unit1 { get; set; }

        private Unit Unit2 { get; set; }

        private bool Unit1OnTheLeft { get; set; }

        private float BackgroundZ { get; set; }

        private Vector3 LeftPosition { get; set; }
        private Vector3 RightPosition { get; set; }

        private TextureAsset Texture { get; set; }

        private int CurrentLetterIndex { get; set; }
        private int LastLetterFrame { get; set; }
        private int CurrentFrame{ get; set; }
        private int TextScrollSpeed { get; set; }

        private bool IsLineFinished { get; set; }

        public int FontSize { get; set; }
        public FontAsset Font { get; set; }

        public bool Finished { get; set; }

        public void Update() 
        {
            CurrentFrame++;
            if (CurrentFrame - LastLetterFrame == TextScrollSpeed)
            {
                CurrentLetterIndex++;
                LastLetterFrame = CurrentFrame;
            }
        }

        public void Render(RenderComposer composer)
        {
            // Disabled the camera and draw on Screen Space instead of World Space
            composer.SetUseViewMatrix(false);
            composer.RenderSprite(
                new Vector3(Vector2.Zero, BackgroundZ),
                new Vector2(1920, 1080),
                Color.White,
                Texture.Texture
            );

            Vector3 Unit1Position;
            if (Unit1OnTheLeft)
            {
                Unit1Position = LeftPosition + new Vector3(Unit1.InteractionOffset, 0);
            }
            else
            {
                Unit1Position = RightPosition + new Vector3(Unit1.InteractionOffset.X * -1, Unit1.InteractionOffset.Y, 0);
            }
            composer.RenderSprite(
                Unit1Position,
                Unit1.Size * 3,
                Color.White,
                Unit1.TextureAsset.Texture,
                null,
                Unit1OnTheLeft
            );

            Vector3 Unit2Position;
            if (Unit1OnTheLeft)
            {
                Unit2Position = RightPosition + new Vector3(Unit2.InteractionOffset.X * -1, Unit2.InteractionOffset.Y, 0);
            }
            else
            {
                Unit2Position = LeftPosition + new Vector3(Unit2.InteractionOffset, 0);
            }
            composer.RenderSprite(
                Unit2Position,
                Unit2.Size * 3,
                Color.White,
                Unit2.TextureAsset.Texture,
                null,
                !Unit1OnTheLeft
            );

            if(Unit2.Dialogues != null)
            {
                if (CurrentLetterIndex >= Unit2.Dialogues[0].DialogueLines[0].Length)
                {
                    // Set whenever the user initiates the next line
                    //CurrentLetterIndex = 0;
                    //LastLetterFrame = CurrentFrame;
                    Finished = true;

                    composer.RenderString(
                      new Vector3(760, 900, 12), // 200px left of the center
                      Color.Green,
                      Unit2.Dialogues[0].DialogueLines[0],
                      Font.GetAtlas(FontSize)
                );
                }
                else
                {
                    composer.RenderString(
                          new Vector3(760, 900, 12), // 200px left of the center
                          Color.Green,
                          Unit2.Dialogues[0].DialogueLines[0].Substring(0, CurrentLetterIndex + 1),
                          Font.GetAtlas(FontSize)
                    );
                }
            }
        }
    }
}
