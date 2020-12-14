using Emotion.Standard.TMX;
using Emotion.Standard.TMX.Layer;
using Emotion.Standard.TMX.Object;
using Emotion.Standard.XML;
using Emotion.Utility;
using MusicTest.Collision;
using MusicTest.GameObjects;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.RoomData
{
    public class TiledMap : TmxMap
    {
        public TiledMap(XMLReader reader) : base(reader)
        {
            CollisionPlatforms = new List<LineSegment>();
            SlopedCollisionPlatforms = new List<LineSegment>();
            AxisAlignedCollisionPlatforms = new List<LineSegment>();
            MagicFlows = new List<MagicFlow>();
            BackgroundDecorations = new List<Decoration>();
            TilesetMap = new Dictionary<int, string>();

            Size = new Vector2(Width, Height);

            float spawnX;
            float.TryParse(Properties["SpawnX"], out spawnX);
            float spawnY;
            float.TryParse(Properties["SpawnY"], out spawnY);
            float spawnZ;
            float.TryParse(Properties["SpawnZ"], out spawnZ);

            Spawn = new Vector3(spawnX, spawnY, spawnZ);

            CreateObjects();
        }

        public Vector2 Size { get; set; }
        public Vector3 Spawn { get; set; }

        public List<LineSegment> CollisionPlatforms { get; set; }
        public List<LineSegment> SlopedCollisionPlatforms { get; set; }
        public List<LineSegment> AxisAlignedCollisionPlatforms { get; set; }
        public List<MagicFlow> MagicFlows { get; set; }
        public List<Decoration> BackgroundDecorations { get; set; }

        public Dictionary<int, string> TilesetMap { get; set; }

        public void CreateObjects()
        {
            // Create a dictionary to map GIDs to their source images
            foreach (TmxTileset tileset in Tilesets)
            {
                string[] sourceSplit = tileset.Source.Split("/");
                TilesetMap.Add(tileset.FirstGid, sourceSplit[sourceSplit.Length - 1]);
                // TODO - Possibly load textures asynchronously here
                // or add them to MainScene.Textures to pass to TextureLoader.Load although that won't help
                // because the Decoration constructor will Get/Load them before the TextureLoader
            }

            foreach (TmxObjectLayer objLayer in ObjectLayers)
            {
                foreach (TmxObject obj in objLayer.Objects)
                {
                    if (objLayer.Name.Contains("Platforms"))
                    {
                        if (obj.ObjectType == TmxObjectType.Polyline)
                        {
                            for (int i = 0; i < obj.Lines.Count; i++)
                            {
                                LineSegment platform = new LineSegment(obj.Lines[i].Start, obj.Lines[i].End);

                                if (platform.IsSloped)
                                {
                                    SlopedCollisionPlatforms.Add(platform);
                                }
                                else
                                {
                                    AxisAlignedCollisionPlatforms.Add(platform);
                                }

                                CollisionPlatforms.Add(platform);
                            }
                        }
                    }
                    else if (objLayer.Name.Contains("Magic Flows"))
                    {
                        if (obj.ObjectType == TmxObjectType.Polyline)
                        {
                            MagicFlow flow = new MagicFlow();

                            for (int i = 0; i < obj.Lines.Count; i++)
                            {
                                LineSegment platform = new LineSegment(obj.Lines[i].Start, obj.Lines[i].End);

                                flow.AddSegment(platform);
                            }

                            MagicFlows.Add(flow);
                        }
                    }
                    else if (objLayer.Name.Contains("Assets"))
                    {
                        if (obj.ObjectType == TmxObjectType.Image)
                        {
                            if (obj.Gid == null)
                            {
                                continue;
                            }

                            float decorationZ;
                            try
                            {
                                float.TryParse(obj.Properties["Z"], out decorationZ);
                            }
                            catch (KeyNotFoundException)
                            {
                                decorationZ = 3;
                            }

                            // In Tiled an image's X,Y coordinates represent the bottom-left corner of the image
                            Decoration decoration = new Decoration(
                                "Asset" + obj.X,
                                TilesetMap[(int)obj.Gid],
                                new Vector2((float)obj.Width, (float)obj.Height),
                                new Vector3((float)obj.X, (float)obj.Y, decorationZ),
                                new Vector2((float)obj.Width, (float)obj.Height),
                                null,
                                false,
                                Maths.DegreesToRadians((float)obj.Rotation)
                            );

                            BackgroundDecorations.Add(decoration);
                        }
                    }
                }
            }
        }

    }
}
