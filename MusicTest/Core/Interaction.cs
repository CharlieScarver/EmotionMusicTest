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

            LeftPosition = new Vector3(Engine.Renderer.Camera.WorldToScreen(new Vector2(250, 300)), 11);
            RightPosition = new Vector3(Engine.Renderer.Camera.WorldToScreen(new Vector2(1550, 100)), 11);

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
            composer.RenderSprite(
                new Vector3(Vector2.Zero, BackgroundZ),
                new Vector2(2200, 1200), // Engine.Host.Window.Size,
                Color.White,
                Texture.Texture
            );

            composer.RenderSprite(
                Unit1OnTheLeft ? LeftPosition : RightPosition,
                Unit1.Size * 3,
                Color.White,
                Unit1.TextureAsset.Texture,
                null,
                Unit1OnTheLeft
            );

            composer.RenderSprite(
                Unit1OnTheLeft ? RightPosition : LeftPosition,
                Unit2.Size * 3,
                Color.White,
                Unit2.TextureAsset.Texture,
                null,
                !Unit1OnTheLeft
            );
        }
    }
}
