using Emotion.Standard.TMX;
using Emotion.Standard.TMX.Layer;
using Emotion.Standard.TMX.Object;
using Emotion.Standard.XML;
using MusicTest.Collision;
using MusicTest.GameObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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

        public void CreateObjects()
        {
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
                }
            }
        }

    }
}
