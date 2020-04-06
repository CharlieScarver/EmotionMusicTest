using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Primitives;
using MusicTest.GameObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MusicTest.Core
{
    public class Interaction
    {
        private const float _backgroundZ = 10;

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

            Texture = Engine.AssetLoader.Get<TextureAsset>("transparent-black.png");
        }

        private Unit Unit1 { get; set; }

        private Unit Unit2 { get; set; }

        private bool Unit1OnTheLeft { get; set; }

        private float BackgroundZ { get; set; }

        private Vector3 LeftPosition { get; set; }
        private Vector3 RightPosition { get; set; }

        private TextureAsset Texture { get; set; }

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
        }
    }
}
