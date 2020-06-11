using Emotion.Graphics;
using Emotion.Primitives;
using MusicTest.Collision;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MusicTest.GameObjects
{
    public class MagicFlow : GameObject
    {
        public List<Collision.LineSegment> Segments { get; set; }

        public MagicFlow()
        {
            Segments = new List<Collision.LineSegment>();
        }

        public override void Render(RenderComposer composer)
        {
            foreach (Collision.LineSegment seg in Segments)
            {
                composer.RenderLine(new Vector3(seg.PointA, 6), new Vector3(seg.PointB, 6), Color.Pink, 2);
            }
        }
    }
}
