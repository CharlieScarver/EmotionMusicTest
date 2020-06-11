using Emotion.Audio;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Standard.Image.PNG;
using Emotion.Tools.Windows;
using Emotion.Utility;
using MusicTest.Collision;
using MusicTest.RoomData;
using MusicTest.GameObjects;
using MusicTest.Interactions;
using Newtonsoft.Json;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using MusicTest.Debug;

namespace MusicTest
{
    public class MainScene : IScene
    {
        // MusicTest
        public byte CodeVariant { get; set; }
        public AudioLayer IntroLayer { get; set; }
        public AudioLayer MainLayer { get; set; }
        public AudioLayer SecondaryLayer { get; set; }

        public AudioAsset BackgroundMusic { get; set; }
        public AudioAsset BackgroundMusicIntro { get; set; }

        // Configs
        public Room LoadedRoom { get; set; }
        public Progress GameProgress { get; set; }

        // Game Objects
        public Midori Player { get; set; }
        public List<Unit> Units { get; set; }
        public List<Unit> NonPlayerUnits { get; set; }
        public List<Collision.LineSegment> CollisionPlatforms { get; set; }
        public List<Collision.LineSegment> SlopedCollisionPlatforms { get; set; }
        public List<Collision.LineSegment> AxisAlignedCollisionPlatforms { get; set; }
        public List<Decoration> Backgrounds { get; set; }
        public List<Decoration> BackgroundDecorations { get; set; }
        public List<Decoration> ForegroundDecorations { get; set; }
        public List<DebugObject> DebugObjects { get; set; }

        public List<MagicFlow> MagicFlows { get; set; }

        // Interaction
        public Interaction CurrentInteration { get; set; }

        public MainScene(TextAsset progressFile, TextAsset mapFile)
        { 
            // Deserialize the map into model
            // Note: Make sure not to load any textures inside the constructors here because it will be sequential!
            LoadedRoom = JsonConvert.DeserializeObject<Room>(mapFile.Content);

            // Decode and then encode a PNG with filter zero to make it sequential and faster to load (avoiding sequential parsing)
            //OtherAsset oa = Engine.AssetLoader.Get<OtherAsset>("textures/pixel-midori-full-sheet-horizontal.png");
            //byte[] bytes = PngFormat.Decode(oa.Content, out PngFileHeader header);
            //byte[] pngfile = PngFormat.Encode(bytes, header.Width, header.Height);
            //Engine.AssetLoader.Save(pngfile, "better-pixel-midori-full-sheet-horizontal.png");

            // Deserialize the progress file
            GameProgress = JsonConvert.DeserializeObject<Progress>(progressFile.Content);

            // Init collections
            Units = new List<Unit>();
            NonPlayerUnits = new List<Unit>();
            CollisionPlatforms = new List<Collision.LineSegment>();
            SlopedCollisionPlatforms = new List<Collision.LineSegment>();
            AxisAlignedCollisionPlatforms = new List<Collision.LineSegment>();
            Backgrounds = new List<Decoration>();
            BackgroundDecorations = new List<Decoration>();
            ForegroundDecorations = new List<Decoration>();
            DebugObjects = new List<DebugObject>();
            MagicFlows = new List<MagicFlow>();

            // Testing for jittering
            //Engine.Renderer.Camera.Zoom = 0.5f;
            //Engine.Renderer.Camera.X = Player.X;
            //Engine.Renderer.Camera.Y = 540; // Middle of the screen

            Engine.Renderer.VSync = true;
            //Console.WriteLine(OpenGL.Gl.CurrentLimits.MaxTextureSize);
            //Console.WriteLine(OpenGL.Gl.CurrentExtensions.MapBufferRange_ARB);
            //Console.WriteLine(OpenGL.Gl.CurrentExtensions.BufferStorage_ARB);
            //Console.WriteLine(OpenGL.Gl.CurrentExtensions.GpuShader5_ARB);

            //var testBuffer = new VertexBuffer(4);
            //unsafe
            //{
            //    byte* mapper = testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);
            //    int num = Helpers.GenerateRandomNumber(0, 1337);
            //    *(int*)mapper = num;
            //    Engine.Log.Info($"Test number is: {num}", "");
            //    testBuffer.FinishMapping();

            //    mapper = testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapReadBit);
            //    int readNumber = *(int*)mapper;
            //    Engine.Log.Info($"Reread number is: {readNumber}", "");
            //    testBuffer.FinishMapping();

            //    testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);
            //    testBuffer.FinishMapping();

            //    mapper = testBuffer.CreateUnsafeMapper(0, 4, BufferAccessMask.MapReadBit);
            //    readNumber = *(int*)mapper;
            //    Engine.Log.Info($"Second time reread number is: {readNumber}", "");
            //    testBuffer.FinishMapping();
            //}
        }

        public MainScene(TextAsset progressFile, TextAsset mapFile, TiledMap tiledMap) : this(progressFile, mapFile)
        {
            CollisionPlatforms = tiledMap.CollisionPlatforms;
            SlopedCollisionPlatforms = tiledMap.SlopedCollisionPlatforms;
            AxisAlignedCollisionPlatforms = tiledMap.AxisAlignedCollisionPlatforms;

            LoadedRoom.Spawn = tiledMap.Spawn;
            LoadedRoom.Size = tiledMap.Size;

        }

        public void Load()
        {
            // Music testing
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

            // Texture loading should be done before any texture usages
            // to make sure they are loaded in parallel
            TextureLoader.Load(LoadedRoom.Textures);

            // Init the player
            Player = new Midori(LoadedRoom.Spawn);
            Units.Add(Player);

            // Init the camera
            Engine.Renderer.Camera = new ScalableArtCamera(new Vector3(Player.X, 540, 0), 1f);

            // Set the TextureArrayLimit to 1 for GPU's that support only zero indexing
            // Engine.Renderer.TextureArrayLimit = 1;

            // Create units
            foreach (ConfigUnit configUnit in LoadedRoom.Units)
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
                NonPlayerUnits.Add(unit);
            }

            // Magic Flows
            MagicFlow m = new MagicFlow();
            m.Segments.Add(new Collision.LineSegment(100, 5400, 950, 4600));
            m.Segments.Add(new Collision.LineSegment(950, 4600, 1450, 4800));
            m.Segments.Add(new Collision.LineSegment(1450, 4800, 2000, 4200));
            MagicFlows.Add(m);

            m = new MagicFlow();
            m.Segments.Add(new Collision.LineSegment(1450, 5400, 1750, 5300));
            m.Segments.Add(new Collision.LineSegment(1750, 5300, 1950, 5200));
            m.Segments.Add(new Collision.LineSegment(1950, 5200, 2050, 5100));
            m.Segments.Add(new Collision.LineSegment(2050, 5100, 2100, 5000));
            m.Segments.Add(new Collision.LineSegment(2100, 5000, 2100, 4900));
            m.Segments.Add(new Collision.LineSegment(2100, 4900, 2050, 4800));
            m.Segments.Add(new Collision.LineSegment(2050, 4800, 1950, 4700));
            m.Segments.Add(new Collision.LineSegment(1950, 4700, 1750, 4600));
            MagicFlows.Add(m);

            // Create platforms
            for (int i = 0; i < LoadedRoom.CollisionPlatforms.Count; i++)
            {
                ConfigCollisionPlatform configPlatform = LoadedRoom.CollisionPlatforms[i];
                Collision.LineSegment realPlatform = new Collision.LineSegment(configPlatform.PointA, configPlatform.PointB);
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

            // Create decorations
            // Backgrounds
            for (int i = 0; i < LoadedRoom.Backgrounds.Count; i++)
            {
                ConfigDecoration configDecor = LoadedRoom.Backgrounds[i];
                Backgrounds.Add(
                    new Decoration(
                        configDecor.Name,
                        configDecor.TextureName,
                        configDecor.Size,
                        configDecor.Position,
                        configDecor.DisplaySize,
                        configDecor.TextureArea,
                        configDecor.FlipX,
                        configDecor.BlurIntensity,
                        configDecor.ShadowReverseIntensity
                    )
                );
            }
            // Background Decorations
            for (int i = 0; i < LoadedRoom.BackgroundDecorations.Count; i++)
            {
                ConfigDecoration configDecor = LoadedRoom.BackgroundDecorations[i];
                BackgroundDecorations.Add(
                    new Decoration(
                        configDecor.Name,
                        configDecor.TextureName,
                        configDecor.Size,
                        configDecor.Position,
                        configDecor.DisplaySize,
                        configDecor.TextureArea,
                        configDecor.FlipX,
                        configDecor.BlurIntensity,
                        configDecor.ShadowReverseIntensity
                    )
                );
            }
            // Foreground Decorations
            for (int i = 0; i < LoadedRoom.ForegroundDecorations.Count; i++)
            {
                ConfigDecoration configDecor = LoadedRoom.ForegroundDecorations[i];
                ForegroundDecorations.Add(
                    new Decoration(
                        configDecor.Name,
                        configDecor.TextureName,
                        configDecor.Size,
                        configDecor.Position,
                        configDecor.DisplaySize,
                        configDecor.TextureArea,
                        configDecor.FlipX,
                        configDecor.BlurIntensity,
                        configDecor.ShadowReverseIntensity
                    )
                );
            }
        }

        public void Unload()
        {
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

        public bool AddGameObjectIfCollidesWithMouse<T>(IList<T> collection) where T : GameObject
        {
            for (int i = 0; i < collection.Count; i++)
            {
                GameObject gameObj = collection[i];
                Vector2 worldMousePos = Engine.Renderer.Camera.ScreenToWorld(Engine.InputManager.MousePosition);
                // Check if object collides with the mouse pointer
                if (CollisionUtils.PointIsInRectangleInclusive(worldMousePos, gameObj.ToRectangle()))
                {
                    // Check if an object with this Name already exists in DebugObjects
                    DebugObject debugObj = new DebugObject(gameObj);
                    if (DebugObjects.Find(o => o.Item.Name == gameObj.Name) == null)
                    {
                        // Add the object to DebugObjects
                        DebugObjects.Add(debugObj);
                        // Return true to signal that an object was added and the other calls can be skipped
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddDebugObject()
        {
            // Add only one object per call to AddDebugObject
            // After an object is added the function returns
            // The calls below are ordered by "debug priority"
            if (AddGameObjectIfCollidesWithMouse(Units))
            {
                return;
            }
            if (AddGameObjectIfCollidesWithMouse(ForegroundDecorations))
            {
                return;
            }
            if (AddGameObjectIfCollidesWithMouse(BackgroundDecorations))
            {
                return;
            }
        }

        public bool IsTransformOnSreen(Transform transform) 
        {
            return transform.ToRectangle().IntersectsInclusive(
                Engine.Renderer.Camera.GetWorldBoundingRect()
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
            Player.Update();

            // Update the camera
            Vector2 windowSize = Engine.Host.Window.Size; // Change to render size?
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
                    foreach (Decoration dec in ForegroundDecorations)
                    {
                        dec.X += Player.VelocityX + dec.VelocityOffsetX;
                    }
                }
                // If not fix the camera along the edge of the room
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
                    foreach (Decoration dec in ForegroundDecorations)
                    {
                        dec.X -= Player.VelocityX + dec.VelocityOffsetX;
                    }
                }
                // If not fix the camera along the edge of the room
                else
                {
                    Engine.Renderer.Camera.X = LoadedRoom.Size.X - (windowSize.X / 2);
                }
            }

            float playYCenterWithOffset = Player.Center.Y - 216;
            if (playYCenterWithOffset < Engine.Renderer.Camera.Y)
            {
                // When the vertical velocity is negative treat it as 0 in the calculations
                float velocityY = Player.VelocityY > 0 ? Player.VelocityY : 0;

                // If the upper border of the camera has enough room to move up with velocityY, then move it
                if (Engine.Renderer.Camera.Y - (windowSize.Y / 2) > 0 + velocityY)
                {
                    Engine.Renderer.Camera.Y = playYCenterWithOffset; // CameraOffsetY
                }
                // If not fix the camera along the edge of the room
                else
                {
                    Engine.Renderer.Camera.Y = 0 + (windowSize.Y / 2);
                }
            }
            else if (playYCenterWithOffset > Engine.Renderer.Camera.Y)
            {
                // When falling the camera always follows the player
                Engine.Renderer.Camera.Y = playYCenterWithOffset; // CameraOffsetY
            }

            // Interaction
            if (Player.IsInteracting && CurrentInteration == null)
            {
                CurrentInteration = new Interaction(Player, Player.InteractTarget);
            }
            else if (!Player.IsInteracting && CurrentInteration != null)
            {
                CurrentInteration = null;
            }
            else if (Player.IsInteracting && CurrentInteration != null)
            {
                CurrentInteration.Update();
            }

            for (int i = 0; i < NonPlayerUnits.Count; i++)
            {
                NonPlayerUnits[i].Update();
            }

            // Debug
            // Mouse click to add to DebugObjects
            if (Engine.InputManager.IsMouseKeyDown(MouseKey.Left))
            {
                AddDebugObject();
            }
            else if (Engine.InputManager.IsMouseKeyDown(MouseKey.Right) && DebugObjects.Count > 0)
            {
                DebugObjects.RemoveAt(DebugObjects.Count - 1);
            }

            // Camera Zoom Out
            if (Engine.InputManager.IsKeyDown(Key.R))
            {
                if (Engine.Renderer.Camera.Zoom == 1f)
                {
                    Engine.Renderer.Camera.Zoom = 0.4f;
                }
                else if (Engine.Renderer.Camera.Zoom == 0.4f)
                {
                    Engine.Renderer.Camera.Zoom = 0.1f;
                }
                else if (Engine.Renderer.Camera.Zoom == 0.1f)
                {
                    Engine.Renderer.Camera.Zoom = 1f;
                }
            }

            // Quit on Escape press
            if (Engine.InputManager.IsKeyHeld(Key.Escape))
            {
                Engine.Quit();
            }
        }

        public void Draw(RenderComposer composer)
        {
            foreach (Decoration bg in Backgrounds)
            {
                bg.Render(composer);
            }

            foreach (Decoration dec in BackgroundDecorations)
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

            foreach (Collision.LineSegment plat in CollisionPlatforms)
            {
                plat.Render(composer);
            }

            // Draw Magic Flows
            foreach (MagicFlow mf in MagicFlows)
            {
                mf.Render(composer);
            }

            Player.Render(composer);

            foreach (Decoration dec in ForegroundDecorations)
            {
                if (IsTransformOnSreen(dec))
                {
                    dec.Render(composer);
                }
            }

            // Draw the room ceiling
            composer.RenderLine(new Vector3(0, 0, 15), new Vector3(LoadedRoom.Size.X, 0, 6), Color.Cyan, 1);

            // Draw camera position (probably the center of the screen)
            composer.RenderCircle(Engine.Renderer.Camera.Position, 1, Color.Cyan);

            // Display the current interaction
            if (CurrentInteration != null)
            {
                CurrentInteration.Render(composer);
            }

            // Draw DebugObjects' CollisionBoxes
            for (int i = 0; i < DebugObjects.Count; i++)
            {
                DebugObject debugObj = DebugObjects[i];
                debugObj.RenderObjectRectange(composer);
            }

            // Disabled the camera and draw on Screen Space instead of World Space
            composer.SetUseViewMatrix(false);

            // Draw DebugObjects
            for (int i = 0; i < DebugObjects.Count; i++)
            {
                DebugObject debugObj = DebugObjects[i];
                int fontSize = 18;
                float debugObjDisplayWidth = debugObj.LongestLine.Length * 8; // Magic number
                TextureAsset textureAsset = Engine.AssetLoader.Get<TextureAsset>("Textures/better-transparent-black.png");
                composer.RenderSprite(
                    new Vector3(debugObjDisplayWidth * i, 0, 15),
                    new Vector2(debugObjDisplayWidth, Engine.Configuration.RenderSize.Y),
                    Color.White,
                    textureAsset.Texture
                );
                composer.RenderString(
                    new Vector3(debugObjDisplayWidth * i, 0, 15),
                    Color.Red,
                    debugObj.ToString(),
                    Engine.AssetLoader.Get<FontAsset>("debugFont.otf").GetAtlas(fontSize)
                );
            }

            // Draw circle on mouse pointer
            composer.RenderCircle(new Vector3(Engine.InputManager.MousePosition, 15), 3, Color.Red, true);
            // Draw mouse coordinates
            composer.RenderString(
                new Vector3(20, Engine.Configuration.RenderSize.Y - 80, 15),
                Color.Red,
                Engine.InputManager.MousePosition.ToString(),
                Engine.AssetLoader.Get<FontAsset>("debugFont.otf").GetAtlas(18)
            );


            // Enable the camera again
            composer.SetUseViewMatrix(true);

            // Render the Emotion Tools UI
            //composer.RenderToolsMenu();
        }

    }
}
