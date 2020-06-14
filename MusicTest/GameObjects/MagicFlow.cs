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

        /// <summary>
        /// The total length of the flow comprised of the combined length of all segments.
        /// The value is reliable only if the AddSegments method is used instead of Segments.Add().
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// If the player is currently traversing the flow from the beginning of the first
        /// segment or from the end of the last one
        /// </summary>
        public bool TraverseFirstToLast { get; set; } = true;

        public MagicFlow()
        {
            Segments = new List<Collision.LineSegment>();
            Length = 0f;
        }

        /// <summary>
        /// Adds the given segment to the list of segments and updates the total length.
        /// </summary>
        /// <param name="segment"></param>
        public void AddSegment(Collision.LineSegment segment)
        {
            Segments.Add(segment);
            Length += segment.Length;
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
