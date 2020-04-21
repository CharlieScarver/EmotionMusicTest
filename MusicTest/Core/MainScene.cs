using Emotion.Audio;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;
using MusicTest.Core;
using MusicTest.Core.Room;
using MusicTest.GameObjects;
using Newtonsoft.Json;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest
{
    public class MainScene : IScene
    {
        public byte CodeVariant { get; set; }
        public AudioLayer IntroLayer { get; set; }
        public AudioLayer MainLayer { get; set; }
        public AudioLayer SecondaryLayer { get; set; }

        public AudioAsset BackgroundMusic { get; set; }
        public AudioAsset BackgroundMusicIntro { get; set; }

        public Room LoadedRoom { get; set; }
        public Progress GameProgress { get; set; }

        public Midori Player { get; set; }
        public List<Unit> Units { get; set; }
        public List<CollisionPlatform> CollisionPlatforms { get; set; }
        public List<CollisionPlatform> SlopedCollisionPlatforms { get; set; }
        public List<CollisionPlatform> AxisAlignedCollisionPlatforms { get; set; }

        public Interaction CurrentInteration { get; set; }

        public MainScene(TextAsset progressFile, TextAsset mapFile)
        {
            // Deserialize map into model.
            LoadedRoom = JsonConvert.DeserializeObject<Room>(mapFile.Content);

            GameProgress = JsonConvert.DeserializeObject<Progress>(progressFile.Content);

            Player = new Midori(LoadedRoom.Spawn, this);

            Units = new List<Unit>();
            CollisionPlatforms = new List<CollisionPlatform>();
            SlopedCollisionPlatforms = new List<CollisionPlatform>();
            AxisAlignedCollisionPlatforms = new List<CollisionPlatform>();

            Engine.Renderer.Camera = new ScalableArtCamera(new Vector3(Player.X, 540, 0), 1f);
            //Engine.Renderer.Camera.Zoom = 0.5f;
            //Engine.Renderer.Camera.X = Player.X;
            //Engine.Renderer.Camera.Y = 540; // Middle of the screen

            Engine.Renderer.VSync = true;
            Console.WriteLine(OpenGL.Gl.CurrentLimits.MaxTextureSize);
            Console.WriteLine(OpenGL.Gl.CurrentExtensions.MapBufferRange_ARB);
            Console.WriteLine(OpenGL.Gl.CurrentExtensions.BufferStorage_ARB);
            Console.WriteLine(OpenGL.Gl.CurrentExtensions.GpuShader5_ARB);

            var testBuffer = new VertexBuffer(4);
            unsafe
            {
                byte* mapper = testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);
                int num = Helpers.GenerateRandomNumber(0, 1337);
                *(int*)mapper = num;
                Engine.Log.Info($"Test number is: {num}", "");
                testBuffer.FinishMapping();

                mapper = testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapReadBit);
                int readNumber = *(int*)mapper;
                Engine.Log.Info($"Reread number is: {readNumber}", "");
                testBuffer.FinishMapping();

                testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);
                testBuffer.FinishMapping();

                mapper = testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapReadBit);
                readNumber = *(int*)mapper;
                Engine.Log.Info($"Second time reread number is: {readNumber}", "");
                testBuffer.FinishMapping();
            }
        }

        public void PlayMainTrackOnMainLayer()
        {
            CustomAudioTrack fcMain = new CustomAudioTrack(BackgroundMusic, 90.35f, PlayMainTrackLoopOnIntroLayer);
            MainLayer.AddToQueue(fcMain);
        }

        public void PlayMainTrackLoopOnIntroLayer()
        {
            CustomAudioTrack fcMain = new CustomAudioTrack(BackgroundMusic, 90.35f, PlayMainTrackOnMainLayer);
            IntroLayer.AddToQueue(fcMain);
        }

        public bool IsPositionValid()
        {
            return true;
        }

        public void Load()
        {
            CodeVariant = 3;

            if (CodeVariant == 1)
            {
                BackgroundMusicIntro = Engine.AssetLoader.Get<AudioAsset>("Audio/Frozen Cave Intro.wav");
                BackgroundMusic = Engine.AssetLoader.Get<AudioAsset>("Audio/Frozen Cave 12.wav");

                IntroLayer = Engine.Host.Audio.CreateLayer("Audio/Intro layer", 1);
                CustomAudioTrack fcIntro = new CustomAudioTrack(BackgroundMusicIntro, 11.29f, PlayMainTrackOnMainLayer);
                IntroLayer.AddToQueue(fcIntro);

                MainLayer = Engine.Host.Audio.CreateLayer("Main layer", 1);
            }
            else if (CodeVariant == 2)
            {
                //BackgroundMusicIntro = Engine.AssetLoader.Get<AudioAsset>("Frozen Cave Intro 2.wav");
                //BackgroundMusic = Engine.AssetLoader.Get<AudioAsset>("Frozen Cave 12.wav");

                BackgroundMusicIntro = Engine.AssetLoader.Get<AudioAsset>("Audio/Frozen Cave Intro 4.wav");
                BackgroundMusic = Engine.AssetLoader.Get<AudioAsset>("Audio/Frozen Cave Loop 3.wav");

                SecondaryLayer = Engine.Host.Audio.CreateLayer("Secondary layer", 1);
                SecondaryLayer.AddToQueue(BackgroundMusicIntro);
                SecondaryLayer.AddToQueue(BackgroundMusic);
            }

            TextureLoader.Load(LoadedRoom.Textures);

            // Set the TextureArrayLimit to 1 for GPU's that support only zero indexing
            // Engine.Renderer.TextureArrayLimit = 1;

            foreach(ConfigUnit configUnit in LoadedRoom.Units)
            {
                Unit unit;
                switch (configUnit.Type)
                {
                    case "Shishi":
                        unit = new Shishi(configUnit.Name, configUnit.TextureName, configUnit.Position, configUnit.Size);
                        break;
                    default:
                        throw new Exception("No applicable classes");
                }
                Units.Add(unit);
            }

            for (int i = 0; i < LoadedRoom.CollisionPlatforms.Count; i++)
            {
                ConfigCollisionPlatform configPlatform = LoadedRoom.CollisionPlatforms[i];
                CollisionPlatform realPlatform = new CollisionPlatform(configPlatform.PointA, configPlatform.PointB);
                if (realPlatform.IsSloped)
                {
                    SlopedCollisionPlatforms.Add(realPlatform);
                }
                else
                {
                    AxisAlignedCollisionPlatforms.Add(realPlatform);
                }

                CollisionPlatforms.Add(realPlatform);
            }
        }

        public void Unload()
        {
        }

        public bool IsTransformOnSreen(Transform transform) 
        {
            Vector2 windowSize = Engine.Host.Window.Size;
            return transform.ToRectangle().IntersectsInclusive(
                Engine.Renderer.Camera.GetWorldBoundingRect()
                //new Rectangle(Engine.Renderer.Camera.Position2 - new Vector2(windowSize.X / 2, windowSize.Y / 2), windowSize)
            );
        }

        public void Update()
        {
            if (CodeVariant == 2)
            {
                if (SecondaryLayer.CurrentTrack?.File.Name == "frozen cave loop 3.wav")
                {
                    SecondaryLayer.LoopingCurrent = true;
                }
            }

            // Update the player
            Player.Update(LoadedRoom);

            // Update the camera
            Vector2 windowSize = Engine.Host.Window.Size;
            // The center of the player on the X axis only
            float playerXCenter = Player.X + Player.Size.X / 2;

            if (playerXCenter < Engine.Renderer.Camera.X)
            {
                // Check left camera side
                if (Engine.Renderer.Camera.X - (windowSize.X / 2) > 0 + Player.VelocityX)
                {
                    // Engine.Renderer.Camera.X -= VelocityX;
                    Engine.Renderer.Camera.X = playerXCenter;

                    // Move the foreground decorations faster than the player
                    foreach (Decoration dec in LoadedRoom.ForegroundDecorations)
                    {
                        dec.X += Player.VelocityX + dec.VelocityOffsetX;
                    }
                }
                else
                {
                    Engine.Renderer.Camera.X = 0 + (windowSize.X / 2);
                }
            }
            else if (playerXCenter > Engine.Renderer.Camera.X)
            {
                // Check right camera side
                if (Engine.Renderer.Camera.X + (windowSize.X / 2) < LoadedRoom.Size.X - Player.VelocityX)
                {
                    // Engine.Renderer.Camera.X += VelocityX;
                    Engine.Renderer.Camera.X = playerXCenter;

                    // Move the foreground decorations faster than the player
                    foreach (Decoration dec in LoadedRoom.ForegroundDecorations)
                    {
                        dec.X -= Player.VelocityX + dec.VelocityOffsetX;
                    }
                }
                else
                {
                    Engine.Renderer.Camera.X = LoadedRoom.Size.X - (windowSize.X / 2);
                }
            }

            // Interaction
            if (Player.isInteracting && CurrentInteration == null)
            {
                CurrentInteration = new Interaction(Player, Player.InteractTarget);
            }
            else if (!Player.isInteracting && CurrentInteration != null)
            {
                CurrentInteration = null;
            }

            // Quit on Escape press
            if (Engine.InputManager.IsKeyHeld(Key.Escape))
            {
                Engine.Quit();
            }
        }

        public void Draw(RenderComposer composer)
        {
            foreach (Decoration bg in LoadedRoom.Backgrounds)
            {
                bg.Render(composer);
            }

            foreach (Decoration plat in LoadedRoom.Platforms)
            {
                if (IsTransformOnSreen(plat))
                {
                    plat.Render(composer);
                }
            }
            
            foreach (Decoration dec in LoadedRoom.BackgroundDecorations)
            {
                if (IsTransformOnSreen(dec))
                {
                    dec.Render(composer);
                }
            }

            foreach (Unit unit in Units)
            {
                if (IsTransformOnSreen(unit))
                {
                    unit.Render(composer);
                }
            }

            foreach (CollisionPlatform plat in CollisionPlatforms)
            {
                plat.Render(composer);
            }

            foreach (CollisionPlatform plat in SlopedCollisionPlatforms)
            {
                plat.Render(composer);
            }

            Player.Render(composer);

            foreach (Decoration dec in LoadedRoom.ForegroundDecorations)
            {
                if (IsTransformOnSreen(dec))
                {
                    dec.Render(composer);
                }
            }

            composer.RenderCircle(Engine.Renderer.Camera.Position, 5, Color.Red);

            composer.RenderCircle(new Vector3(2450, 0, 5), 1, Color.Red);

            if (CurrentInteration != null)
            {
                CurrentInteration.Render(composer);
            }
        }

    }
}
